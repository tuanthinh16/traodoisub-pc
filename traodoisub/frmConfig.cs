using DevExpress.XtraEditors;
using Newtonsoft.Json;
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

                throw;
            }
        }

        private async Task loadDefaultConfigAsync()
        {
            try
            {
                configAdo = this.config.LoadConfig();
                if(configAdo != null)
                {
                    txtToken.Text = configAdo.TokenTDS;
                    txtToken.Enabled = false;
                    DialogResult rs = MessageBox.Show(this,"Đã có config. Bạn có muốn quay lại?","",MessageBoxButtons.YesNo);
                    if(rs == DialogResult.Yes)
                    {
                        this.Close();
                        await CheckUser(this.configAdo.TokenTDS);
                        updateConfig.Invoke(configAdo);
                    }
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        private async Task loadDefaultAsync()
        {
            try
            {
                string token = txtToken.Text;

                // Kiểm tra người dùng với token
                bool isValid = await CheckUser(token);
                if (isValid)
                {
                    // Nếu người dùng hợp lệ, lưu cấu hình
                    this.configAdo.TokenTDS = token;
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
            catch (Exception ex)
            {
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                
                string token = txtToken.Text;
                loadDefaultAsync();

            }
            catch (Exception ex)
            {

                throw;
            }
        }
        private async Task<bool> CheckUser(string TDS_token)
        {
            bool success = false;
            try
            {
                string apiUrl = $"https://traodoisub.com/api/?fields=profile&access_token={TDS_token}";

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
                MessageBox.Show($"Đã xảy ra lỗi: {ex.Message}");
            }

            return success;
        }


    }
}