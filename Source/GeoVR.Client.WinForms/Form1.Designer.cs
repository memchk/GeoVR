namespace GeoVR.Client.WinForms
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.tbUsername = new System.Windows.Forms.TextBox();
            this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            this.lonLatLabel = new System.Windows.Forms.Label();
            this.lblPTT = new System.Windows.Forms.Label();
            this.cbInput = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cbOutput = new System.Windows.Forms.ComboBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnConnect = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cbFrequency = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Input device:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(11, 89);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Frequency:";
            // 
            // tbUsername
            // 
            this.tbUsername.Location = new System.Drawing.Point(117, 62);
            this.tbUsername.Name = "tbUsername";
            this.tbUsername.Size = new System.Drawing.Size(212, 20);
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
            this.gMapControl1.Location = new System.Drawing.Point(38, 207);
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
            // lblPTT
            // 
            this.lblPTT.AutoSize = true;
            this.lblPTT.Location = new System.Drawing.Point(335, 38);
            this.lblPTT.Name = "lblPTT";
            this.lblPTT.Size = new System.Drawing.Size(58, 13);
            this.lblPTT.TabIndex = 14;
            this.lblPTT.Text = "Radio: Idle";
            // 
            // cbInput
            // 
            this.cbInput.FormattingEnabled = true;
            this.cbInput.Location = new System.Drawing.Point(117, 8);
            this.cbInput.Name = "cbInput";
            this.cbInput.Size = new System.Drawing.Size(212, 21);
            this.cbInput.TabIndex = 16;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(12, 37);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 16);
            this.label4.TabIndex = 0;
            this.label4.Text = "Output device:";
            // 
            // cbOutput
            // 
            this.cbOutput.FormattingEnabled = true;
            this.cbOutput.Location = new System.Drawing.Point(117, 35);
            this.cbOutput.Name = "cbOutput";
            this.cbOutput.Size = new System.Drawing.Size(212, 21);
            this.cbOutput.TabIndex = 16;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(12, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(74, 16);
            this.label5.TabIndex = 0;
            this.label5.Text = "Username:";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(117, 114);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(212, 23);
            this.btnConnect.TabIndex = 17;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(335, 11);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(161, 13);
            this.label3.TabIndex = 15;
            this.label3.Text = "The PTT button is the spacebar.";
            // 
            // cbFrequency
            // 
            this.cbFrequency.FormattingEnabled = true;
            this.cbFrequency.Location = new System.Drawing.Point(117, 88);
            this.cbFrequency.Name = "cbFrequency";
            this.cbFrequency.Size = new System.Drawing.Size(212, 21);
            this.cbFrequency.TabIndex = 18;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 618);
            this.Controls.Add(this.cbFrequency);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.cbOutput);
            this.Controls.Add(this.cbInput);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblPTT);
            this.Controls.Add(this.lonLatLabel);
            this.Controls.Add(this.gMapControl1);
            this.Controls.Add(this.tbUsername);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "GeoVR Test Client";
            this.Deactivate += new System.EventHandler(this.Form1_Deactivate);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyUp);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tbUsername;
        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private System.Windows.Forms.Label lonLatLabel;
        private System.Windows.Forms.Label lblPTT;
        private System.Windows.Forms.ComboBox cbInput;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cbOutput;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cbFrequency;
    }
}

