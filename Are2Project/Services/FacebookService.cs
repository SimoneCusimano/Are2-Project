using Are2Project.Models;
using Are2Project.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Are2Project.Services
{
    public class FacebookService
    {
        private const string BaseAddress = "https://graph.facebook.com/v2.11/";

        public static async Task<FacebookResponse> GetAccessToken(string code, string redirectUri)
        {
            if (code == null) throw new ArgumentNullException(nameof(code));

            using (var client = new HttpClient())
            {
                var url = String.Concat(BaseAddress, "oauth/access_token?", $"client_id={Constant.FACEBOOK_API_KEY}&", $"redirect_uri={redirectUri}&", $"client_secret={Constant.FACEBOOK_CLIENT_SECRET}&", $"code={code}");
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var facebookResponseJson = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<FacebookResponse>(facebookResponseJson);
            }
        }


        public static async Task<Profile> GetProfile(string fields, string token)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            if (token == null) throw new ArgumentNullException(nameof(token));

            var profileDataJson = await Get($"me?fields={fields}", token);
            return JsonConvert.DeserializeObject<Profile>(profileDataJson);
        }

        public static async Task<IList<string>> GetPhotos(string fields, string token, string limit)
        {
            if (fields == null) throw new ArgumentNullException(nameof(fields));
            if (token == null) throw new ArgumentNullException(nameof(token));
            if (limit == null) throw new ArgumentNullException(nameof(limit));

            var urls = new List<string>();
            var photoDataJson = await Get($"me/photos?fields={fields}&limit={limit}", token);
            var photoData = JsonConvert.DeserializeObject<FacebookPhotoResponse>(photoDataJson);
            urls.AddRange(photoData.Photos.Select(p => p.Photos.OrderByDescending(r => r.Width).ThenByDescending(s => s.Height).FirstOrDefault()?.Url));

            while (photoData.PagingInfo?.Next != null)
            {
                photoDataJson = await Get(photoData.PagingInfo.Next, token);
                photoData = JsonConvert.DeserializeObject<FacebookPhotoResponse>(photoDataJson.ToString());
                urls.AddRange(photoData.Photos.Select(p => p.Photos.OrderByDescending(r => r.Width).ThenByDescending(s => s.Height).FirstOrDefault()?.Url));
            }

            return urls;
        }

        private static async Task<string> Get(string resource, string token)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            if (token == null) throw new ArgumentNullException(nameof(token));

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(BaseAddress);
                var resp = await client.GetAsync($"{resource}&access_token={token}");
                return await resp.Content.ReadAsStringAsync();
            }
        }
    }
}