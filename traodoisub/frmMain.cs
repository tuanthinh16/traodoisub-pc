using DevExpress.XtraEditors;
using Newtonsoft.Json;
using System;
using System.Drawing;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using traodoisub.Config;
using traodoisub.Model;
using static traodoisub.frmConfig;

namespace traodoisub
{
    public partial class frmMain : DevExpress.XtraEditors.XtraForm
    {
        ///private string TDS_token = "TDSQfiATMyVmdlNnI6IiclZXZzJCLiczMo5WaoRnbhVHdiojIyV2c1Jye"; // Replace with your actual token

        public frmMain()
        {
            InitializeComponent();
            
        }
        private void DisplayUserInfo(UserInfo userInfo)
        {
            pInfo.Controls.Clear();
            LabelControl lblUser = new LabelControl { Text = $"Người dùng: {userInfo.User}", Location = new Point(10, 10) };
            LabelControl lblXu = new LabelControl { Text = $"Xu: {userInfo.Xu}", Location = new Point(10, 40) };
            LabelControl lblXuDie = new LabelControl { Text = $"Xu die: {userInfo.XuDie}", Location = new Point(10, 70) };
            pInfo.Controls.Add(lblUser);
            pInfo.Controls.Add(lblXu);
            pInfo.Controls.Add(lblXuDie);
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            try
            {
                string token = txtToken.Text;
                //loadUserInfo(token);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

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
            configForm.Show(); // Hoặc sử dụng ShowDialog() nếu bạn muốn form là modal
        }

        private void UpdateConfig(ConfigADO config)
        {
            // Xử lý dữ liệu khi cấu hình được cập nhật từ frmConfig
            MessageBox.Show($"Token TDS đã được cập nhật: {config.TokenTDS}");
            txtToken.Text = config.TokenTDS;
            DisplayUserInfo(config.user);
        }
    }

    public class ApiResponse
    {
        [JsonProperty("success")]
        public int Success { get; set; }

        [JsonProperty("data")]
        public UserInfo Data { get; set; }
    }

    
}
