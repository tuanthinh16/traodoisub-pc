using DevExpress.XtraEditors;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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
        private static readonly ILog log = LogManager.GetLogger(typeof(frmMain));
        private string _accessToken;
        ApiRequest.Facebook.ApiRequest facebook ;
        private static readonly HttpClient client = new HttpClient();
        ConfigADO config = new ConfigADO();
        List<ConfigADO> listConfig = new List<ConfigADO>();
        ConfigManager configManger = new ConfigManager();
        List<UserInfo> listUser = new List<UserInfo>();
        List<UserInfo> listSelectedUser = new List<UserInfo>();


        public frmMain()
        {
            InitializeComponent();
            loadDefaultAsync();
            
        }
       
        private async Task loadDefaultAsync()
        {
            try
            {
                log.Debug("==========================================================");
                log.Debug("==========================================================");
                log.Debug("==========================================================");
                log.Debug("==========================================================");
                log.Debug("==========================================================");
                log.Debug(" ");
                log.Debug("bat dau chay ung dung");

                //btnCheck.Enabled = false;
                btnConfig.Enabled = false;
                btnLikePost.Enabled = false;
                this.Enabled = false;

                
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
                var jsonResponse =  JObject.Parse(response);
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
        private async void FrmMain_LoadAsync(object sender, EventArgs e)
        {
            try
            {
                var _cf = configManger.LoadConfig();
                if(_cf != null)
                {
                    this.listConfig.Clear();
                    this.listConfig.AddRange(_cf);
                    DisplayUserInfoAsync();
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
                //btnCheck.Enabled = true;
                btnConfig.Enabled = true;
                btnLikePost.Enabled = true;
            }
            catch (Exception ex)
            {

                log.Error(ex);
            }
        }
        
        private async void DisplayUserInfoAsync()
        {
            gridControl.BeginUpdate();
            foreach(var _cf in this.listConfig)
            {
                facebook = new ApiRequest.Facebook.ApiRequest(_cf.access_token);
                var userProfile = await facebook.GetFacebookDataAsync("me?fields=id,name,email");
                if (userProfile != null)
                {
                    if (userProfile.ContainsKey("name")) _cf.user.NameFB = userProfile["name"].ToString();
                    if (userProfile.ContainsKey("email")) _cf.user.email = userProfile["email"].ToString();
                }
                var existingUser = listUser.FirstOrDefault(u => u.User == _cf.user.User);
                if (existingUser != null)
                {
                    // Cập nhật thông tin người dùng nếu đã tồn tại
                    existingUser.NameFB = _cf.user.NameFB; // Cập nhật tên
                    existingUser.email = _cf.user.email; // Cập nhật email
                }
                else
                {
                    // Nếu chưa tồn tại, thêm người dùng vào danh sách
                    listUser.Add(_cf.user);
                }

            }
            
            gridControl.DataSource = listUser;
            gridControl.EndUpdate();
        }
        

        private async void btnLikePost_ClickAsync(object sender, EventArgs e)
        {
            try
            {

                if(listSelectedUser != null && listSelectedUser.Count > 0)
                {
                    foreach(var item in listSelectedUser)
                    {
                        ConfigADO _config = listConfig.Where(s => s.user.User == item.User).FirstOrDefault() ;
                        frmFacebook frm = new frmFacebook(_config ?? new ConfigADO(), UpdateConfig, _config.access_token);
                        frm.Show();
                    }
                }
                else
                {
                    MessageBox.Show(this, "Chưa chọn tài khoản để chạy !!!", "Thông báo", MessageBoxButtons.OK);
                }
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

                log.Error(ex);
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
            listConfig.Add(config);
            this.config = config;
            configManger.SaveConfig(listConfig);
            //txtToken.Text = config.TokenTDS;
            DisplayUserInfoAsync();
        }

        #endregion

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                Application.Exit();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        private void gridView_CustomUnboundColumnData(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDataEventArgs e)
        {
            try
            {
                // Lấy dữ liệu từ nguồn dữ liệu của GridView
                var dataSource = gridView.DataSource as List<UserInfo>;

                // Lấy chỉ số của hàng hiện tại
                int rowIndex = e.ListSourceRowIndex;

                if (dataSource != null && rowIndex >= 0 && rowIndex < dataSource.Count)
                {
                    var row = dataSource[rowIndex];

                    if (e.Column.FieldName == "STT")
                    {
                        // Gán giá trị STT dựa trên chỉ số hàng hiện tại (bắt đầu từ 1)
                        e.Value = (rowIndex + 1).ToString();
                    }
                }
                if (e.Column.FieldName == "SELECT")
                {
                    UserInfo row = (UserInfo)gridView.GetRow(e.ListSourceRowIndex);
                    if (e.IsGetData)
                    {
                        e.Value = listSelectedUser.Contains(row);
                    }
                }
            }
            catch (Exception ex)
            {

                log.Error(ex);
            }
        }

        private void gridView_RowCellClick(object sender, DevExpress.XtraGrid.Views.Grid.RowCellClickEventArgs e)
        {
            try
            {
                var gridView = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (gridView == null) return;

                // Lấy dữ liệu hàng hiện tại
                var data = (UserInfo)gridView.GetRow(e.RowHandle);

                if (data != null)
                {
                    // Kiểm tra nếu hàng đã tồn tại trong danh sách
                    if (listSelectedUser.Contains(data))
                    {
                        // Nếu hàng đã tồn tại, xóa khỏi danh sách (bỏ chọn)
                        listSelectedUser.Remove(data);
                    }
                    else
                    {
                        // Nếu hàng chưa tồn tại, thêm vào danh sách (chọn)
                        listSelectedUser.Add(data);
                    }
                }
                if (e.Column.FieldName == "SELECT")
                {
                    UserInfo row = (UserInfo)gridView.GetRow(e.RowHandle);
                    if (listSelectedUser.Contains(row))
                    {
                        listSelectedUser.Remove(row);
                    }
                    else
                    {
                        listSelectedUser.Add(row);
                    }

                    // Refresh lại ô checkbox để hiển thị trạng thái mới
                    gridView.RefreshRowCell(e.RowHandle, e.Column);
                }
                gridView.RefreshRow(e.RowHandle);

            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }
        private void btnDel_ButtonClick(object sender, DevExpress.XtraEditors.Controls.ButtonPressedEventArgs e)
        {
            try
            {
                if(MessageBox.Show(this,"Bạn có muốn xóa bỏ dữ liệu ?","",MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    gridControl.BeginUpdate();
                    var data = (UserInfo)gridView.GetFocusedRow();
                    if (data != null)
                    {
                        var rs = listUser.Where(s => s.User == data.User).FirstOrDefault();
                        listUser.Remove(rs);
                        var conf = listConfig.Where(s => s.user.User == data.User).FirstOrDefault();
                        listConfig.Remove(conf);
                    }
                    gridControl.EndUpdate();
                }

            }
            catch (Exception ex)
            {

                log.Error(ex);
            }
        }

        private void gridView_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                var gridView = sender as DevExpress.XtraGrid.Views.Grid.GridView;
                if (gridView == null) return;


                var data = gridView.GetFocusedRow() as UserInfo;
                if (data != null)
                {
                    UpdateConfigDelegate updateDelegate = new UpdateConfigDelegate(UpdateConfig);
                    ConfigADO _cf = listConfig.Where(s => s.user.User == data.User).FirstOrDefault();
                    frmConfig frm = new frmConfig(updateDelegate, _cf);
                    frm.ShowDialog();
                }
                

            }
            catch (Exception ex)
            {

                log.Error(ex);
            }
        }
    }

    

    
}
