namespace GeoVR.Client.VATSIM.WinForms
{
    partial class Form1
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
            this.tbUsername = new System.Windows.Forms.TextBox();
            this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            this.lonLatLabel = new System.Windows.Forms.Label();
            this.cbInput = new System.Windows.Forms.ComboBox();
            this.cbOutput = new System.Windows.Forms.ComboBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.RadioLabel = new MetroFramework.Controls.MetroLabel();
            this.lblPTT = new MetroFramework.Controls.MetroLabel();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel7 = new MetroFramework.Controls.MetroLabel();
            this.lblReceivedAudioBytes = new MetroFramework.Controls.MetroLabel();
            this.lblLastSender = new MetroFramework.Controls.MetroLabel();
            this.metroLabel6 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel8 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel9 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel10 = new MetroFramework.Controls.MetroLabel();
            this.lblFrequency = new MetroFramework.Controls.MetroLabel();
            this.SuspendLayout();
            // 
            // tbUsername
            // 
            this.tbUsername.Location = new System.Drawing.Point(476, 100);
            this.tbUsername.Name = "tbUsername";
            this.tbUsername.Size = new System.Drawing.Size(123, 20);
            this.tbUsername.TabIndex = 4;
            // 
            // gMapControl1
            // 
            this.gMapControl1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.gMapControl1.Bearing = 0F;
            this.gMapControl1.CanDragMap = true;
            this.gMapControl1.EmptyTileColor = System.Drawing.Color.Navy;
            this.gMapControl1.GrayScaleMode = false;
            this.gMapControl1.HelperLineOption = GMap.NET.WindowsForms.HelperLineOptions.DontShow;
            this.gMapControl1.LevelsKeepInMemmory = 5;
            this.gMapControl1.Location = new System.Drawing.Point(52, 318);
            this.gMapControl1.MarkersEnabled = true;
            this.gMapControl1.MaxZoom = 2;
            this.gMapControl1.MinZoom = 2;
            this.gMapControl1.MouseWheelZoomEnabled = true;
            this.gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.ViewCenter;
            this.gMapControl1.Name = "gMapControl1";
            this.gMapControl1.NegativeMode = false;
            this.gMapControl1.PolygonsEnabled = true;
            this.gMapControl1.RetryLoadTile = 0;
            this.gMapControl1.RoutesEnabled = true;
            this.gMapControl1.ScaleMode = GMap.NET.WindowsForms.ScaleModes.Integer;
            this.gMapControl1.SelectedAreaFillColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(65)))), ((int)(((byte)(105)))), ((int)(((byte)(225)))));
            this.gMapControl1.ShowTileGridLines = false;
            this.gMapControl1.Size = new System.Drawing.Size(669, 376);
            this.gMapControl1.TabIndex = 10;
            this.gMapControl1.Zoom = 0D;
            this.gMapControl1.OnPositionChanged += new GMap.NET.PositionChanged(this.gMapControl1_OnPositionChanged);
            // 
            // lonLatLabel
            // 
            this.lonLatLabel.AutoSize = true;
            this.lonLatLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lonLatLabel.Location = new System.Drawing.Point(234, 593);
            this.lonLatLabel.Name = "lonLatLabel";
            this.lonLatLabel.Size = new System.Drawing.Size(28, 16);
            this.lonLatLabel.TabIndex = 13;
            this.lonLatLabel.Text = "-----";
            // 
            // cbInput
            // 
            this.cbInput.FormattingEnabled = true;
            this.cbInput.Location = new System.Drawing.Point(128, 98);
            this.cbInput.Name = "cbInput";
            this.cbInput.Size = new System.Drawing.Size(212, 21);
            this.cbInput.TabIndex = 16;
            // 
            // cbOutput
            // 
            this.cbOutput.FormattingEnabled = true;
            this.cbOutput.Location = new System.Drawing.Point(128, 127);
            this.cbOutput.Name = "cbOutput";
            this.cbOutput.Size = new System.Drawing.Size(212, 21);
            this.cbOutput.TabIndex = 16;
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(281, 172);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(212, 23);
            this.btnConnect.TabIndex = 17;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(23, 49);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(122, 19);
            this.metroLabel1.TabIndex = 21;
            this.metroLabel1.Text = "Powered by GeoVR";
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(28, 100);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(83, 19);
            this.metroLabel2.TabIndex = 26;
            this.metroLabel2.Text = "Input Device:";
            // 
            // metroLabel3
            // 
            this.metroLabel3.AutoSize = true;
            this.metroLabel3.Location = new System.Drawing.Point(16, 130);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(95, 19);
            this.metroLabel3.TabIndex = 27;
            this.metroLabel3.Text = "Output Device:";
            // 
            // RadioLabel
            // 
            this.RadioLabel.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.RadioLabel.AutoSize = true;
            this.RadioLabel.Location = new System.Drawing.Point(16, 215);
            this.RadioLabel.Name = "RadioLabel";
            this.RadioLabel.Size = new System.Drawing.Size(46, 19);
            this.RadioLabel.TabIndex = 29;
            this.RadioLabel.Text = "Radio:";
            // 
            // lblPTT
            // 
            this.lblPTT.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblPTT.AutoSize = true;
            this.lblPTT.CustomForeColor = true;
            this.lblPTT.ForeColor = System.Drawing.Color.DimGray;
            this.lblPTT.Location = new System.Drawing.Point(68, 215);
            this.lblPTT.Name = "lblPTT";
            this.lblPTT.Size = new System.Drawing.Size(36, 19);
            this.lblPTT.Style = MetroFramework.MetroColorStyle.Blue;
            this.lblPTT.TabIndex = 30;
            this.lblPTT.Text = "Idle..";
            // 
            // metroLabel5
            // 
            this.metroLabel5.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.metroLabel5.AutoSize = true;
            this.metroLabel5.Location = new System.Drawing.Point(177, 215);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(95, 19);
            this.metroLabel5.TabIndex = 31;
            this.metroLabel5.Text = "Received Data:";
            // 
            // metroLabel7
            // 
            this.metroLabel7.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.metroLabel7.AutoSize = true;
            this.metroLabel7.Location = new System.Drawing.Point(413, 215);
            this.metroLabel7.Name = "metroLabel7";
            this.metroLabel7.Size = new System.Drawing.Size(80, 19);
            this.metroLabel7.TabIndex = 32;
            this.metroLabel7.Text = "Last Sender:";
            // 
            // lblReceivedAudioBytes
            // 
            this.lblReceivedAudioBytes.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblReceivedAudioBytes.AutoSize = true;
            this.lblReceivedAudioBytes.CustomForeColor = true;
            this.lblReceivedAudioBytes.ForeColor = System.Drawing.Color.DimGray;
            this.lblReceivedAudioBytes.Location = new System.Drawing.Point(278, 215);
            this.lblReceivedAudioBytes.Name = "lblReceivedAudioBytes";
            this.lblReceivedAudioBytes.Size = new System.Drawing.Size(36, 19);
            this.lblReceivedAudioBytes.Style = MetroFramework.MetroColorStyle.Blue;
            this.lblReceivedAudioBytes.TabIndex = 33;
            this.lblReceivedAudioBytes.Text = "Idle..";
            // 
            // lblLastSender
            // 
            this.lblLastSender.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblLastSender.AutoSize = true;
            this.lblLastSender.CustomForeColor = true;
            this.lblLastSender.ForeColor = System.Drawing.Color.DimGray;
            this.lblLastSender.Location = new System.Drawing.Point(499, 215);
            this.lblLastSender.Name = "lblLastSender";
            this.lblLastSender.Size = new System.Drawing.Size(36, 19);
            this.lblLastSender.Style = MetroFramework.MetroColorStyle.Blue;
            this.lblLastSender.TabIndex = 34;
            this.lblLastSender.Text = "Idle..";
            // 
            // metroLabel6
            // 
            this.metroLabel6.AutoSize = true;
            this.metroLabel6.Location = new System.Drawing.Point(429, 100);
            this.metroLabel6.Name = "metroLabel6";
            this.metroLabel6.Size = new System.Drawing.Size(30, 19);
            this.metroLabel6.TabIndex = 35;
            this.metroLabel6.Text = "CID";
            // 
            // metroLabel8
            // 
            this.metroLabel8.AutoSize = true;
            this.metroLabel8.Location = new System.Drawing.Point(398, 130);
            this.metroLabel8.Name = "metroLabel8";
            this.metroLabel8.Size = new System.Drawing.Size(72, 19);
            this.metroLabel8.TabIndex = 36;
            this.metroLabel8.Text = "Frequency:";
            // 
            // metroLabel9
            // 
            this.metroLabel9.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.metroLabel9.AutoSize = true;
            this.metroLabel9.Location = new System.Drawing.Point(608, 215);
            this.metroLabel9.Name = "metroLabel9";
            this.metroLabel9.Size = new System.Drawing.Size(39, 19);
            this.metroLabel9.TabIndex = 37;
            this.metroLabel9.Text = "Data:";
            // 
            // metroLabel10
            // 
            this.metroLabel10.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.metroLabel10.AutoSize = true;
            this.metroLabel10.CustomForeColor = true;
            this.metroLabel10.ForeColor = System.Drawing.Color.DimGray;
            this.metroLabel10.Location = new System.Drawing.Point(652, 215);
            this.metroLabel10.Name = "metroLabel10";
            this.metroLabel10.Size = new System.Drawing.Size(98, 19);
            this.metroLabel10.Style = MetroFramework.MetroColorStyle.Blue;
            this.metroLabel10.TabIndex = 38;
            this.metroLabel10.Text = "Not Connected";
            // 
            // lblFrequency
            // 
            this.lblFrequency.AutoSize = true;
            this.lblFrequency.ForeColor = System.Drawing.Color.DimGray;
            this.lblFrequency.Location = new System.Drawing.Point(476, 130);
            this.lblFrequency.Name = "lblFrequency";
            this.lblFrequency.Size = new System.Drawing.Size(50, 19);
            this.lblFrequency.TabIndex = 39;
            this.lblFrequency.Text = "119.500";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(834, 240);
            this.Controls.Add(this.lblFrequency);
            this.Controls.Add(this.metroLabel10);
            this.Controls.Add(this.metroLabel9);
            this.Controls.Add(this.metroLabel8);
            this.Controls.Add(this.metroLabel6);
            this.Controls.Add(this.lblLastSender);
            this.Controls.Add(this.lblReceivedAudioBytes);
            this.Controls.Add(this.metroLabel7);
            this.Controls.Add(this.metroLabel5);
            this.Controls.Add(this.lblPTT);
            this.Controls.Add(this.RadioLabel);
            this.Controls.Add(this.metroLabel3);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.cbOutput);
            this.Controls.Add(this.cbInput);
            this.Controls.Add(this.lonLatLabel);
            this.Controls.Add(this.gMapControl1);
            this.Controls.Add(this.tbUsername);
            this.Name = "Form1";
            this.Resizable = false;
            this.Text = "A.F.V Test Client";
            this.Deactivate += new System.EventHandler(this.Form1_Deactivate);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox tbUsername;
        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private System.Windows.Forms.Label lonLatLabel;
        private System.Windows.Forms.ComboBox cbInput;
        private System.Windows.Forms.ComboBox cbOutput;
        private System.Windows.Forms.Button btnConnect;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroLabel RadioLabel;
        private MetroFramework.Controls.MetroLabel lblPTT;
        private MetroFramework.Controls.MetroLabel metroLabel5;
        private MetroFramework.Controls.MetroLabel metroLabel7;
        private MetroFramework.Controls.MetroLabel lblReceivedAudioBytes;
        private MetroFramework.Controls.MetroLabel lblLastSender;
        private MetroFramework.Controls.MetroLabel metroLabel6;
        private MetroFramework.Controls.MetroLabel metroLabel8;
        private MetroFramework.Controls.MetroLabel metroLabel9;
        private MetroFramework.Controls.MetroLabel metroLabel10;
        private MetroFramework.Controls.MetroLabel lblFrequency;
    }
}

