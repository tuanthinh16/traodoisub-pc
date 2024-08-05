using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace traodoisub.ApiRequest.Facebook
{
    public class ApiRequest
    {
        string _accessToken = "";
        private static readonly ILog log = LogManager.GetLogger(typeof(ApiRequest));
        private static readonly HttpClient client = new HttpClient();
        public ApiRequest(string _token)
        {
            this._accessToken = _token;
        }
        public async Task<JObject> GetFacebookDataAsync(string endpoint)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
                var response = await client.GetStringAsync(string.Format("https://graph.facebook.com/{0}", endpoint));
                //log.Debug("Get Data Response :" + response);
                return JObject.Parse(response);
            }
            catch (Exception ex)
            {
                log.Error("Loi lay thong tin facebook user"+ex);
                return null;
                
            }
        }


        public async Task<Object> LikePostAsync(string postId)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
                var response = await client.PostAsync(string.Format("https://graph.facebook.com/{0}/likes", postId), null);
                if(!response.IsSuccessStatusCode)log.Debug("Like Response :"+response);
                
                return response.StatusCode;
                
            }
            catch (Exception ex)
            {

                log.Error(ex);
                return 404;
            }
        }
        public async Task<bool> FollowUserAsync(string userId)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
                var response = await client.PostAsync(string.Format("https://graph.facebook.com/me/friends/{0}", userId), null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {

                log.Error(ex);
                return false;
            }
        }

        public async Task<bool> CommentOnPostAsync(string postId, string message)
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _accessToken);
                var content = new StringContent(string.Format("message={0}", message), Encoding.UTF8, "application/x-www-form-urlencoded");
                var response = await client.PostAsync(string.Format("https://graph.facebook.com/{0}/comments", postId), content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {

                log.Error(ex);
                return false;
            }
        }
        public async Task<string> GetPostIdFromLinkAsync(string link)
        {
            try
            {
                //log.Info("Starting GetPostIdFromLinkAsync operation");
                //log.Info($"Link: {link}");

                string apiUrl = "https://id.traodoisub.com/api.php";
                var formData = new MultipartFormDataContent
        {
            { new StringContent(link), "link" },
        };

                //// Log thông tin của formData
                //foreach (var content in formData)
                //{
                //    log.Info($"Form Data - Name: {content.Headers.ContentDisposition.Name}, Value: {await content.ReadAsStringAsync()}");
                //}

                var response = await client.PostAsync(apiUrl, formData);
                //log.Info($"Response: {response}");

                // Log toàn bộ phản hồi HTTP
                //log.Info($"Response Status Code: {response.StatusCode}");
                //log.Info($"Response Headers: {response.Headers}");

                var responseString = await response.Content.ReadAsStringAsync();
                //log.Info($"Response Content: {responseString}");

                if (!string.IsNullOrEmpty(responseString))
                {
                    try
                    {
                        var jsonResponse = JObject.Parse(responseString);
                        string postId = jsonResponse["id"].ToString();

                        log.Info(string.Format("Retrieved Post ID: {0}", postId));

                        return postId;
                    }
                    catch (JsonReaderException ex)
                    {
                        log.Error("Error parsing JSON response", ex);
                       
                        return null;
                    }
                }
                else
                {
                    log.Warn("Response content is null or empty");
                    return null;
                }
            }
            catch (Exception ex)
            {
                log.Error("Error in GetPostIdFromLinkAsync", ex);
                return null;
            }
        }



    }
}
