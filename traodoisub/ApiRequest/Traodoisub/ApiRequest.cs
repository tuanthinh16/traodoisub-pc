using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using traodoisub.Model;

namespace traodoisub.ApiRequest.Traodoisub
{
    public class ApiRequest
    {
        string _accessToken = "";
        private static readonly ILog log = LogManager.GetLogger(typeof(ApiRequest));
        private static readonly HttpClient client = new HttpClient();
        public ApiRequest(string token)
        {
            this._accessToken = token;
        }
        public async Task<List<string>> GetListTask(string type)
        {
            try
            {
                List<string> idlist = new List<string>();
                var request = new HttpRequestMessage(HttpMethod.Get, string.Format("https://traodoisub.com/api/?fields={0}&access_token={1}",type, _accessToken));
                var response = await client.SendAsync(request);
                //log.Debug("Response lấy dữ liệu nhiệm vụ:" + response);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();

                JArray jsonResponse = JArray.Parse(responseBody);

                // Lấy danh sách ID
                List<string> idList = jsonResponse.Select(j => j["id"].ToString()).ToList();

                return idList;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }
        public async Task<DataCoin> GetCoinTask(string type,string id)
        {
            try
            {
                List<string> idlist = new List<string>();
                var request = new HttpRequestMessage(HttpMethod.Get, string.Format("https://traodoisub.com/api/coin/?type={0}&id={1}&access_token={2}", type, id, _accessToken));

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponseGetCoin>(responseBody);
                log.Debug("Du lieu tra ve nhan coin: " + responseBody);
                return apiResponse.Data;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

    }
}
