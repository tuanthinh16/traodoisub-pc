using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
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
            this.panel.AutoScroll = true;
            this.cboType.EditValue = "like";
        }

        int yOffset = 10;
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
                        yOffset = 10;
                        listTask.AddRange(rs);
                        panel.Controls.Clear(); // Xóa các điều khiển hiện có trên panel
                        // Khoảng cách dọc giữa các điều khiển

                        foreach(var item in rs)
                        {
                            Label lblTask = new Label
                            {
                                Text = item.ToString(),
                                AutoSize = true,
                                ForeColor = Color.Red,
                                Location = new System.Drawing.Point(10, yOffset) // Đặt vị trí của điều khiển trên panel
                            };

                            panel.Controls.Add(lblTask);
                            yOffset += lblTask.Height + 10;
                            panel.ScrollControlIntoView(panel.Controls[panel.Controls.Count - 1]);
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

        int countError = 0;
        private async void btnStart_ClickAsync(object sender, EventArgs e)
        {
            try
            {
                
                await GetNVAsync();
                this.btnStart.Enabled = false;
                log.Debug("bat dau chay");
                log.Debug("========== Loai nhiem vu: "+cboType.Text);
                Label lblTask2 = new Label
                {
                    Text = "========== Bắt đầu chạy",
                    AutoSize = true,
                    ForeColor = Color.Green,
                    Location = new System.Drawing.Point(10, yOffset) // Đặt vị trí của điều khiển trên panel
                };

                panel.Controls.Add(lblTask2);
                yOffset += lblTask2.Height + 10;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                
                foreach (var item in listTask)
                {
                    
                    log.Debug("bat dau like ID: "+item);
                    var success = await facebook.LikePostAsync(item);
                    if (Convert.ToInt64(success) == 200)
                    {
                        log.Debug("Like thanh cong");
                        await Task.Delay(10000);
                        DataCoin rs = await tds.GetCoinTask(cboType.Text, item);
                        if(rs != null)
                        {
                            log.Debug("nhan coin thanh cong");
                            txtTotal.Text = "Tổng : " + rs.xu + " xu";
                            string msg = "---------- ID: " + rs.ID + " msg: " + rs.msg;
                            Label lblTask = new Label
                            {
                                Text = msg,
                                AutoSize = true,
                                ForeColor = Color.Green,
                                Location = new System.Drawing.Point(10, yOffset) // Đặt vị trí của điều khiển trên panel
                            };

                            panel.Controls.Add(lblTask);
                            yOffset += lblTask.Height + 10;
                            panel.ScrollControlIntoView(panel.Controls[panel.Controls.Count - 1]);
                        }
                        else
                        {
                            log.Debug("loi nhan coin"+ rs);
                            Label lblTask = new Label
                            {
                                Text = item+" ========== Lỗi nhận coin",
                                AutoSize = true,
                                ForeColor = Color.Green,
                                Location = new System.Drawing.Point(10, yOffset) // Đặt vị trí của điều khiển trên panel
                            };

                            panel.Controls.Add(lblTask);
                            yOffset += lblTask.Height + 10;
                            panel.ScrollControlIntoView(panel.Controls[panel.Controls.Count - 1]);
                        }

                    }
                    else if(Convert.ToInt64(success) == 401)
                    {
                        countError += 1;
                        log.Debug("Loi token.lay lai di");
                    }
                    else
                    {
                        countError += 1;
                        log.Debug("Like that bai");
                    }
                    await Task.Delay(5000);
                }
                stopwatch.Stop();
                if (stopwatch.Elapsed < TimeSpan.FromMinutes(1))
                {
                    var delay = TimeSpan.FromMinutes(1) - stopwatch.Elapsed;
                    await Task.Delay(delay);
                }
                if(countError > 10)
                {
                    MessageBox.Show("Lỗi. Kiểm tra lại token hoặc log");
                    return;
                }
                log.Debug("Đã like hết danh sách");
                log.Debug("lay danh sach moi");
                panel.Controls.Clear();
                Label lblTask1 = new Label
                {
                    Text = "========== Đã làm hết nhiệm vụ. Lấy dữ liệu mới",
                    AutoSize = true,
                    ForeColor = Color.Green,
                    Location = new System.Drawing.Point(10, yOffset) 
                };

                panel.Controls.Add(lblTask1);
                yOffset += lblTask1.Height + 10;
                panel.ScrollControlIntoView(panel.Controls[panel.Controls.Count - 1]);
                await Task.Delay(10000);
                btnStart_ClickAsync(null, null);

            }
            catch (Exception ex)
            {

                log.Error(ex);
            }
        }
    }
}
