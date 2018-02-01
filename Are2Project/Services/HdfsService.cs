using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;

namespace Are2Project.Services
{
    public class HdfsService
    {
        private const string BaseAddress = "http://master:50070/webhdfs/v1/simone.cusimano/";

        private readonly ILogger _logger;
        private readonly IHostingEnvironment _hostingEnvironment;

        public HdfsService(ILogger logger, IHostingEnvironment hostingEnvironment)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        public async void SendToSpark(string userId, string json)
        {
            if (string.IsNullOrEmpty(json)) throw new ArgumentException("Json is null or empty", nameof(json));

            var fileName = $"{userId}_{Guid.NewGuid().ToString()}";

            try
            {
                using (var client = new HttpClient())
                {
                    var url = $"{BaseAddress}{fileName}.json?op=CREATE&overwrite=true";

                    var location = await client.PutAsync(new Uri(url), new StringContent(json));
                    if (location.StatusCode == System.Net.HttpStatusCode.Created)
                    {
                        IEnumerable<string> locations = null;
                        if (location.Headers.TryGetValues("Location", out locations))
                        {
                            var hdfs = locations.FirstOrDefault();
                            _logger.LogInformation($"[WEBHDFS] => New file @ {hdfs}");
                        }
                        else
                        {
                            _logger.LogInformation($"[WEBHDFS] => Unable to find new file location");
                        }
                    }
                    else
                    {
                        Fallback(fileName, json);
                        var content = await location.Content.ReadAsStringAsync();
                        _logger.LogError($"[WEBHDFS] => Request result: {location.StatusCode.ToString()} - Content: {content}");
                    }
                }
            }
            catch (Exception ex)
            {
                Fallback(fileName, json);
                _logger.LogError($"[WEBHDFS] => {ex.Message}");
            }
        }

        private void Fallback(string fileName, string json)
        {
            if (string.IsNullOrEmpty(json)) throw new ArgumentException("Json is null or empty", nameof(json));

            var path = "Crawls";
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            File.WriteAllText(Path.Combine(path, $"{fileName}.json"), json);
        }
    }
}
