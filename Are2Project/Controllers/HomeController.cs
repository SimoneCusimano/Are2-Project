using Are2Project.Models;
using Are2Project.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Are2Project.Utils;

namespace Are2Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger _logger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly string _hostPath;

        public HomeController(ILogger<HomeController> logger, IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _hostPath = _hostingEnvironment.IsDevelopment() ? "http://localhost:5001" : "http://192.167.155.71/scusimano";
        }

        public IActionResult Index()
        {
            ViewBag.Step = "Step-1";
            ViewBag.Path = _hostPath;
            return View("Index");
        }

        public IActionResult FacebookLogin() => Redirect($"https://www.facebook.com/v2.11/dialog/oauth?client_id={Constant.FACEBOOK_API_KEY}&redirect_uri={_hostPath}/Home/ReturnFacebookLogin&response_type=code&scope=user_photos");

        public async Task<IActionResult> ReturnFacebookLogin(string code)
        {
            if (code == null) throw new ArgumentNullException(nameof(code));

            try
            {
                var redirectUrl = $"{_hostPath}/Home/ReturnFacebookLogin";
                var facebookAuthData = await FacebookService.GetAccessToken(code, redirectUrl);

                var profile = await FacebookService.GetProfile("id,address,birthday,devices,email,education,first_name,gender,last_name", facebookAuthData.AccessToken);
                if (TempData.ContainsKey("ProfileId"))
                    TempData.Remove("ProfileId");
                TempData.Add("ProfileId", profile.Id);

                var photos = await FacebookService.GetPhotos("images", facebookAuthData.AccessToken, "100");
                if (TempData.ContainsKey("Photos"))
                    TempData.Remove("Photos");
                TempData.Add("Photos", photos);
            }
            catch (Exception ex)
            {
                _logger.LogError($"HomeController->ReturnFacebookLogin: {ex.Message} - {ex.StackTrace}");
                throw;
            }

            ViewBag.Path = _hostPath;
            return View("Index");
        }

        public void RetrievePhotosMetadata()
        {
            Response.ContentType = "text/event-stream";

            var photos = TempData["Photos"] as IEnumerable<string>;
            var profileId = TempData["ProfileId"] as string;
            var cognitiveService = new CognitiveServicesService(_logger);
            var photoDescriptions = new List<PhotoDescription>();
            StringBuilder sb = new StringBuilder();
            foreach (var photo in photos)
            {
                try
                {
                    var photoDescription = cognitiveService.GetPhotoMetadata(photo);
                    photoDescriptions.Add(photoDescription.Result);

                    if ((photoDescriptions.IndexOf(photoDescription.Result) + 1) % 10 == 0)
                    {
                        var jsonPhoto = JsonConvert.SerializeObject(PreparePhotoDescriptionsForView(photoDescriptions.TakeLast(10)));
                        byte[] data = Encoding.UTF8.GetBytes($"data: {jsonPhoto}\n\n");
                        Response.Body.Write(data, 0, data.Length);
                        Response.Body.Flush();
                    }

                    if ((photoDescriptions.IndexOf(photoDescription.Result) + 1) % 20 == 0)
                        Task.Delay(60000).Wait();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"HomeController->ReturnFacebookLogin: {ex.Message} - {ex.StackTrace}");
                }
                
            }

            Task.Run(() =>
            {
                var hdfsService = new HdfsService(_logger, _hostingEnvironment);
                hdfsService.SendToSpark(profileId, JsonConvert.SerializeObject(photoDescriptions));
            });
        }

        private List<PhotoDescriptionViewModel> PreparePhotoDescriptionsForView(IEnumerable<PhotoDescription> photoDescriptions)
        {
            return photoDescriptions.Select(
                t => new PhotoDescriptionViewModel
                {
                    title = "More Info",
                    description =
                        $"<table>" +
                        $"<tr><td><b>Description</b>:</td><td>{t.Description?.Description?.Captions?.FirstOrDefault()?.Text} ({t.Description?.Description?.Captions?.FirstOrDefault()?.Confidence})</td></tr>" +
                        $"<tr><td><b>Tags</b>:</td><td>{t.Description?.Description?.Tags?.DefaultIfEmpty().Aggregate((i, j) => i + ", " + j)}</td></tr>" +
                        $"<tr><td><b>Faces</b> (Gender, Age, Top, Left, Height, Width):</td><td> {t.Description?.Faces?.Select(p => $"[{p.Gender}, {p.Age}, {p.FaceRectangle.Top}, {p.FaceRectangle.Left}, {p.FaceRectangle.Height}, {p.FaceRectangle.Width}]")?.DefaultIfEmpty().Aggregate((i, j) => i + ", " + j)}</td></tr>" +
                        $"<tr><td><b>Categories</b>:</td><td>{t.Description?.Categories?.Select(c => $"{c.Name} ({c.Score})")?.DefaultIfEmpty().Aggregate((i, j) => i + ", " + j)}</td></tr>" +
                        $"<tr><td><b>Clip Art Type</b>:</td><td>{t.Description?.ImageType?.ClipArtType}</td></tr>" +
                        $"<tr><td><b>Line Drawing Type</b>:</td><td>{t.Description?.ImageType?.LineDrawingType}</td></tr>" +
                        $"<tr><td><b>Image Format</b>:</td><td>{t.Description?.Metadata?.Format}</td></tr>" +
                        $"<tr><td><b>Image Dimensions</b>:</td><td>{t.Description?.Metadata?.Height} x{t.Description?.Metadata?.Width}</td></tr>" +
                        $"<tr><td><b>Black and White</b>:</td><td>{t.Description?.Color?.IsBwImg}</td></tr>" +
                        $"<tr><td><b>Accent Color</b>:</td><td>{t.Description?.Color?.AccentColor}</td></tr>" +
                        $"<tr><td><b>Dominant Color Background</b>:</td><td>{t.Description?.Color?.DominantColorBackground}</td></tr>" +
                        $"<tr><td><b>Dominant Color Foreground</b>:</td><td>{t.Description?.Color?.DominantColorForeground}</td></tr>" +
                        $"<tr><td><b>Colors</b>:</td><td>{t.Description?.Color?.DominantColors?.DefaultIfEmpty().Aggregate((i, j) => i + ", " + j)}</td></tr>" +
                        $"<tr><td><b>Adult Content</b>:</td><td>{t.Description?.Adult?.IsAdultContent}</td></tr>" +
                        $"<tr><td><b>Adult Score</b>:</td><td>{t.Description?.Adult?.AdultScore}</td></tr>" +
                        $"<tr><td><b>Racy Content</b>:</td><td>{t.Description?.Adult?.IsRacyContent}</td></tr>" +
                        $"<tr><td><b>Racy Score</b>:</td><td>{t.Description?.Adult?.RacyScore}</td></tr>" +
                        $"</table>",
                    thumbnail = new List<string>() { t.Url },
                    large = new List<string>() { String.Empty },
                    button_list = new List<string>(),
                    tags = new List<string> { "Gallery" }
                }).ToList();
        }

    }
}
