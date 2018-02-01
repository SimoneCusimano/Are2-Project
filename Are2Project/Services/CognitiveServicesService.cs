using Are2Project.Models;
using Are2Project.Utils;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Are2Project.Services
{
    public class CognitiveServicesService
    {
        private readonly ILogger _logger;
        private const string BaseAddress = "https://westcentralus.api.cognitive.microsoft.com/vision/v1.0/analyze/";

        public CognitiveServicesService(ILogger logger)
        {
            _logger = logger;
        }

        public async Task<PhotoDescription> GetPhotoMetadata(string photo)
        {
            if (String.IsNullOrEmpty(photo)) throw new ArgumentException("Value cannot be null or empty.", nameof(photo));

            PhotoDescription photoDescription = new PhotoDescription();
            try
            {
                const string requestParameters = "visualFeatures=Categories,Description,Color,Faces,ImageType,Adult";
                const string uri = BaseAddress + "?" + requestParameters;
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", Constant.COGNITIVE_SERVICE_API_KEY);
                    var json = JsonConvert.SerializeObject(new { url = photo });
                    var request = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(uri, request);
                    response.EnsureSuccessStatusCode();
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<CognitiveServicesResponse>(responseJson);
                    photoDescription = new PhotoDescription { Url = photo, Description = result };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"CognitiveServicesService->GetDescription: {ex.Message} - {ex.StackTrace}");
                throw;
            }

            return photoDescription;
        }

    }
}
