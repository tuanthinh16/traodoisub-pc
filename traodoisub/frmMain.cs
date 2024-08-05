using DevExpress.XtraEditors;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using traodoisub.Config;
using traodoisub.Facebook;
using traodoisub.Model;
using static traodoisub.frmConfig;

namespace traodoisub
{
    public partial class frmMain : DevExpress.XtraEditors.XtraForm
    {
        ///private string TDS_token = "TDSQfiATMyVmdlNnI6IiclZXZzJCLiczMo5WaoRnbhVHdiojIyV2c1Jye"; // Replace with your actual token
        private static readonly ILog log = LogManager.GetLogger(typeof(frmMain));
        private string _accessToken;
        ApiRequest.Facebook.ApiRequest facebook ;
        private static readonly HttpClient client = new HttpClient();
        ConfigADO config = new ConfigADO();
        ConfigManager configManger = new ConfigManager();

        // EAAaKr7mzAZC8BOzPGAo32HmfzlltdxQiLGQyeZCieP4xwX7Ui2gmgHwBMPJnI4P2e0w3AXmh0dorJNr3tGOEgJSUMKAtlCvv1W42KMZBFZCUzw3Ok8ZBJPoW3MWNXjtDWOEuTPspZAYvZBQIJX8FmcgHbnZBujh3siKgZAhA7hksuGBJFf36RhvgIQ7ICaKoandSG0rIPub621a96lhuyxrd089ZBxtF3yZCbf5uceOCWdA5m6lVK8mWs89dS4EjLFWfeI2qHCLNZCK1PhgZD
        public frmMain()
        {
            InitializeComponent();
            loadDefaultAsync();
            
        }
       
        private async Task loadDefaultAsync()
        {
            try
            {
                ///EAAAAUaZA8jlABO6OKqyLgD550T4Tbc6ZBUFX11NL82C0VFPf2Oy3jcfDZClSKF1ntFnejR5dGHZA4ZAcnSYFNOzx4vyArfUQB0ocoNnD9NcZBTV8bw1Oz9UIv6xmzuxEsOnE8oZCvyl7eEkThaQXTB9vvwPaLjiOAj0tS81UbiuXLMxZCSyxBclMUouXdlDanif9dSTtE7ZC1DQZDZD
                log.Debug("==========================================================");
                log.Debug("==========================================================");
                log.Debug("==========================================================");
                log.Debug("==========================================================");
                log.Debug("==========================================================");
                log.Debug(" ");
                log.Debug("bat dau chay ung dung");

                btnCheck.Enabled = false;
                btnConfig.Enabled = false;
                btnLikePost.Enabled = false;
                this.Enabled = false;

                //"EAAAAUaZA8jlABO6OKqyLgD550T4Tbc6ZBUFX11NL82C0VFPf2Oy3jcfDZClSKF1ntFnejR5dGHZA4ZAcnSYFNOzx4vyArfUQB0ocoNnD9NcZBTV8bw1Oz9UIv6xmzuxEsOnE8oZCvyl7eEkThaQXTB9vvwPaLjiOAj0tS81UbiuXLMxZCSyxBclMUouXdlDanif9dSTtE7ZC1DQZDZD";

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public async Task<string> GetToken(string cookie)
        {
            try
            {

                var response = await client.GetStringAsync($"https://alotoi.com/fb/?cookie={cookie}&type=EAAA");
                log.Debug("GetToken Response :" + response);
                var jsonResponse =  JObject.Parse(response);
                if (jsonResponse["status"]?.ToString() == "success")
                {
                    return jsonResponse["token"]?.ToString();
                }
                else
                {
                    log.Warn("Failed to retrieve token: " + jsonResponse["message"]?.ToString());
                    return null; 
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;

            }
        }
        private async void FrmMain_LoadAsync(object sender, EventArgs e)
        {
            try
            {
                this.config = configManger.LoadConfig();
                if(this.config != null)
                {
                    _accessToken = this.config.access_token;
                    facebook = new ApiRequest.Facebook.ApiRequest(_accessToken);
                    UpdateConfig(this.config);
                }
                else
                {
                    DialogResult rs = MessageBox.Show(this, "Chưa có cấu hình. Bạn có muốn cấu hình?", "", MessageBoxButtons.YesNo);
                    if(rs == DialogResult.Yes)
                    {
                        btnConfig_Click(null, null);
                    }
                }
                
                
            }
            catch (Exception ex)
            {

                log.Error(ex);
            }
            finally
            {
                EnableControl();
            }
        }

        private void EnableControl()
        {
            try
            {
                this.Enabled = true;
                btnCheck.Enabled = true;
                btnConfig.Enabled = true;
                btnLikePost.Enabled = true;
            }
            catch (Exception ex)
            {

                log.Error(ex);
            }
        }

        private async void DisplayUserInfoAsync(UserInfo userInfo)
        {
            pInfo.Controls.Clear();
            LabelControl lblUser = new LabelControl { Text = $"Người dùng: {userInfo.User}", Location = new Point(10, 10) };
            LabelControl lblXu = new LabelControl { Text = $"Xu: {userInfo.Xu}", Location = new Point(10, 40) };
            LabelControl lblXuDie = new LabelControl { Text = $"Xu die: {userInfo.XuDie}", Location = new Point(10, 70) };
            pInfo.Controls.Add(lblUser);
            pInfo.Controls.Add(lblXu);
            pInfo.Controls.Add(lblXuDie);
            facebook = new ApiRequest.Facebook.ApiRequest(this.config.access_token);
            var userProfile = await facebook.GetFacebookDataAsync("me?fields=id,name,email");
            if (userProfile != null)
            {
                //pInfo.Controls.Clear();
                LabelControl name = new LabelControl { Text = $"Người dùng: {userProfile["name"]}", Location = new Point(10, 90) };
                LabelControl email = new LabelControl { Text = $"Email : {userProfile["email"]}", Location = new Point(10, 110) };

                pInfo.Controls.Add(name);
                pInfo.Controls.Add(email);
            }
            
        }

        private async void btnCheck_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                GetUserProfile();


            }
            catch (Exception ex)
            {

                log.Error(ex);
            }
        }
        private async void GetUserProfile()
        {
            var userProfile = await facebook.GetFacebookDataAsync("me?fields=id,name,email");
            if(userProfile != null)
            {
                //pInfo.Controls.Clear();
                LabelControl lblUser = new LabelControl { Text = $"Người dùng: {userProfile["name"]}", Location = new Point(10, 10) };
                LabelControl lblXu = new LabelControl { Text = $"Email : {userProfile["email"]}", Location = new Point(10, 40) };

                pInfo.Controls.Add(lblUser);
                pInfo.Controls.Add(lblXu);
            }
            else
            {
                MessageBox.Show("Không tìm thấy người dùng");
            }
        }

        
        

        private async void btnLikePost_ClickAsync(object sender, EventArgs e)
        {
            try
            {

                frmFacebook frm = new frmFacebook(this.config,UpdateConfig,this.config.access_token);
                frm.ShowDialog();
            }
            catch (Exception ex)
            {

                log.Error(ex);
            }
        }
        #region config token
        private void btnConfig_Click(object sender, EventArgs e)
        {
            try
            {
                OpenConfigForm();
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private void OpenConfigForm()
        {
            // Khởi tạo delegate
            UpdateConfigDelegate updateDelegate = new UpdateConfigDelegate(UpdateConfig);

            // Khởi tạo frmConfig và truyền delegate
            frmConfig configForm = new frmConfig(updateDelegate);
            configForm.ShowDialog(); // Hoặc sử dụng ShowDialog() nếu bạn muốn form là modal
        }

        private void UpdateConfig(ConfigADO config)
        {
            // Xử lý dữ liệu khi cấu hình được cập nhật từ frmConfig
            //MessageBox.Show($"Token TDS đã được cập nhật: {config.TokenTDS}");
            this.config = config;
            txtToken.Text = config.TokenTDS;
            DisplayUserInfoAsync(config.user);
        }
        #endregion

        
    }

    

    
}
