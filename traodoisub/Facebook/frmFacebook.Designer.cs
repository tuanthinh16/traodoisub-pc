
namespace traodoisub.Facebook
{
    partial class frmFacebook
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel = new System.Windows.Forms.Panel();
            this.btnGetList = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.cboType = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // panel
            // 
            this.panel.Location = new System.Drawing.Point(12, 12);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(553, 399);
            this.panel.TabIndex = 0;
            // 
            // btnGetList
            // 
            this.btnGetList.Location = new System.Drawing.Point(713, 12);
            this.btnGetList.Name = "btnGetList";
            this.btnGetList.Size = new System.Drawing.Size(75, 23);
            this.btnGetList.TabIndex = 1;
            this.btnGetList.Text = "Lấy danh sách";
            this.btnGetList.UseVisualStyleBackColor = true;
            this.btnGetList.Click += new System.EventHandler(this.btnGetList_ClickAsync);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(713, 59);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 2;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_ClickAsync);
            // 
            // cboType
            // 
            this.cboType.FormattingEnabled = true;
            this.cboType.Items.AddRange(new object[] {
            "like",
            "follow",
            "likegiare",
            "likesieure",
            "reaction",
            "comment",
            "share",
            "reactcmt",
            "group",
            "page"});
            this.cboType.Location = new System.Drawing.Point(586, 12);
            this.cboType.Name = "cboType";
            this.cboType.Size = new System.Drawing.Size(121, 21);
            this.cboType.TabIndex = 3;
            // 
            // frmFacebook
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.cboType);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnGetList);
            this.Controls.Add(this.panel);
            this.Name = "frmFacebook";
            this.Text = "frmFacebook";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.Button btnGetList;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ComboBox cboType;
    }
}