using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using traodoisub.Config;
using traodoisub.Model;

namespace traodoisub.Facebook
{
    public partial class frmFacebook : Form
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(frmFacebook));
        ApiRequest.Traodoisub.ApiRequest tds;
        ApiRequest.Facebook.ApiRequest facebook;
        public delegate void UpdateConfigDelegate(ConfigADO config);
        UpdateConfigDelegate updateConfig;
        ConfigADO config;
        string _token = "";
        string _tokenFB = "";
        List<string> listTask;
        
        public frmFacebook(ConfigADO config, UpdateConfigDelegate UpdateConfig,string _tokenFB)
        {
            this.config = config;
            this._token = config.TokenTDS;
            this._tokenFB = _tokenFB;
            InitializeComponent();
            tds = new ApiRequest.Traodoisub.ApiRequest(this._token);
            facebook = new ApiRequest.Facebook.ApiRequest(this._tokenFB);
            updateConfig = UpdateConfig;
        }

        private async void btnGetList_ClickAsync(object sender, EventArgs e)
        {
            _ = GetNVAsync();
        }
        public async Task GetNVAsync()
        {
            try
            {
                listTask = new List<string>();
                log.Debug("Lay danh sach nv.____Start");
                if (cboType.Text != null)
                {
                    var rs = await tds.GetListTask(cboType.Text);
                    if (rs != null)
                    {
                        listTask.AddRange(rs);
                        panel.Controls.Clear(); // Xóa các điều khiển hiện có trên panel
                        int yOffset = 10; // Khoảng cách dọc giữa các điều khiển

                        foreach (var item in rs)
                        {
                            Label lblTask = new Label
                            {
                                Text = item.ToString(), // Bạn có thể thay đổi để hiển thị thông tin cần thiết
                                AutoSize = true,
                                Location = new System.Drawing.Point(10, yOffset) // Đặt vị trí của điều khiển trên panel
                            };

                            panel.Controls.Add(lblTask);
                            yOffset += lblTask.Height + 10; // Cập nhật vị trí cho điều khiển tiếp theo
                        }
                        log.Debug("Lay danh sach nv.____End");
                    }
                }
            }
            catch (Exception ex)
            {

                log.Error(ex);
            }
        }
        private async void btnStart_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                this.btnStart.Enabled = false;
                log.Debug("bat dau chay");
                foreach (var item in listTask)
                {
                    await Task.Delay(10000);
                    log.Debug("bat dau like ID: "+item);
                    bool success = await facebook.LikePostAsync(item);
                    if (success)
                    {
                        log.Debug("Like thanh cong");
                        await Task.Delay(10000);
                        DataCoin rs = await tds.GetCoinTask(cboType.Text, item);
                        if(rs != null)
                        {
                            log.Debug("nhan coin thanh cong");
                        }
                        else
                        {
                            log.Debug("loi nhan coin"+ rs);
                        }

                    }
                    await Task.Delay(5000);
                }
                
                log.Debug("da like het list");
                log.Debug("lay danh sach moi");
                await Task.Delay(50000);
                _ =GetNVAsync();
                btnStart.PerformClick();


            }
            catch (Exception ex)
            {

                log.Error(ex);
            }
        }
    }
}
