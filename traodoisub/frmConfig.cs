using DevExpress.XtraEditors;
using DevExpress.XtraEditors.DXErrorProvider;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using traodoisub.Config;
using traodoisub.Model;

namespace traodoisub
{
    public partial class frmConfig : DevExpress.XtraEditors.XtraForm
    {
        ConfigManager config;
        
        public delegate void UpdateConfigDelegate(ConfigADO config);
        UpdateConfigDelegate updateConfig;
        ConfigADO configAdo;
        private static readonly ILog log = LogManager.GetLogger(typeof(frmConfig));
        private static readonly HttpClient client = new HttpClient();
        public frmConfig(UpdateConfigDelegate UpdateConfig)
        {
            InitializeComponent();
            this.updateConfig = UpdateConfig;
            
        }
        private void frmConfig_Load(object sender, EventArgs e)
        {
            try
            {
                config = new ConfigManager();
                configAdo = new ConfigADO();
                loadDefaultConfigAsync();

            }
            catch (Exception ex)
            {

                log.Error(ex);
            }
        }

        private async Task loadDefaultConfigAsync()
        {
            try
            {
                configAdo = this.config.LoadConfig();
                if(configAdo != null && configAdo.access_token != null)
                {
                    txtToken.Text = configAdo.TokenTDS;
                    txtCookie.Text = configAdo.cookieFB;

                    txtToken.Enabled = false;
                    txtCookie.Enabled = false;
                    DialogResult rs = MessageBox.Show(this,"Đã có config. Bạn có muốn quay lại?","",MessageBoxButtons.YesNo);
                    if(rs == DialogResult.Yes)
                    {
                        
                        await CheckUser(this.configAdo.TokenTDS);
                        await loadDefaultAsync();
                        this.Close();
                    }
                }
                else
                {
                    configAdo = new ConfigADO();
                }
            }
            catch (Exception ex)
            {

                log.Error(ex);
            }
        }

        private async Task loadDefaultAsync()
        {
            try
            {
                string token = txtToken.Text;
                string cookie = txtCookie.Text;
                // Kiểm tra người dùng với token
                //log.Debug("Convert cookie to token");
                //string cookieString = "b=-XGHZuHwncXt5xKeVsPkthS2;%20datr=-XGHZkG2zw95ie9mKavKZn74;%20ps_n=1;%20ps_l=1;%20locale=vi_VN;%20c_user=100082413946084;%20m_page_voice=100082413946084;%20presence=C%7B\"t3\"%3A%5B%5D%2C\"utc3\"%3A1722486447265%2C\"v\"%3A1%7D;%20wd=1920x953;%20xs=19%3Am_aU7K0vfZFu4g%3A2%3A1722476862%3A-1%3A6242%3A%3AAcWPOB7heKYVL0Pna1SIqbCCzGtXBAPs-SYgWTmdnw;%20fr=1GvLx3ElpKVqNX8ib.AWXxI7xlBdELLC34m68C8iu1rLw.Bmqw7X..AAA.0.0.Bmqw7p.AWW8pW4CZqg";

                var responseString = await GetToken(cookie);
                configAdo.access_token = responseString;
                configAdo.cookieFB = cookie;
                bool isValid = await CheckUser(token);
                if(!string.IsNullOrEmpty(responseString))
                {
                    if (isValid)
                    {
                        // Nếu người dùng hợp lệ, lưu cấu hình
                        configAdo.TokenTDS = token;
                        config.SaveConfig(configAdo);

                        // Gọi delegate để cập nhật dữ liệu
                        updateConfig.Invoke(configAdo);

                        MessageBox.Show("Cấu hình đã được lưu thành công!");
                        this.Close(); // Đóng form sau khi lưu
                    }
                    else
                    {
                        MessageBox.Show("Token không hợp lệ. Vui lòng kiểm tra lại.");
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        public async Task<string> GetToken(string cookie)
        {
            try
            {

                var response = await client.GetStringAsync(string.Format("https://alotoi.com/fb/?cookie={0}&type=EAAA",cookie));
                log.Debug("GetToken Response :" + response);
                var jsonResponse = JObject.Parse(response);
                if (jsonResponse["status"].ToString() == "success")
                {
                    return jsonResponse["token"].ToString();
                }
                else
                {
                    log.Warn("Failed to retrieve token: " + jsonResponse["message"].ToString());
                    return null;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;

            }
        }
        private async void btnSave_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtToken.Text)) dxErrorProvider1.SetError(txtToken, "Trường dữ liệu bắt buộc",DevExpress.XtraEditors.DXErrorProvider.ErrorType.Warning);
                else dxErrorProvider1.SetError(txtToken, "", ErrorType.None);
                if (string.IsNullOrEmpty(txtCookie.Text)) dxErrorProvider1.SetError(txtCookie, "Trường dữ liệu bắt buộc");
                else dxErrorProvider1.SetError(txtCookie, "", ErrorType.None);
                if (dxErrorProvider1.HasErrors) return;
                string token = txtToken.Text;
                await loadDefaultAsync();

            }
            catch (Exception ex)
            {

                log.Error(ex);
            }
        }
        private async Task<bool> CheckUser(string TDS_token)
        {
            bool success = false;
            try
            {
                string apiUrl = string.Format("https://traodoisub.com/api/?fields=profile&access_token={0}",TDS_token);

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();

                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(responseBody);

                    if (apiResponse.Success == 200)
                    {
                        this.configAdo.user = apiResponse.Data;
                        success = true;
                    }
                    else
                    {
                        MessageBox.Show("Không thể tải thông tin người dùng.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Đã xảy ra lỗi: " + ex.Message);
            }

            return success;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            try
            {
                txtToken.Enabled = true;
                txtToken.Text = "";
                txtCookie.Enabled = true;
                txtCookie.Text = "";
                btnSave.Enabled = true;
                dxErrorProvider1.ClearErrors();
            }
            catch (Exception ex)
            {

                log.Error(ex);
            }
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            try
            {
                txtCookie.Enabled = true;
                txtToken.Enabled = true;
                btnSave.Enabled = true;
            }
            catch (Exception ex)
            {

                log.Error(ex);
            }
        }
    }
}