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
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.userNameBox = new System.Windows.Forms.TextBox();
            this.frequencyBox = new System.Windows.Forms.TextBox();
            this.txTrackBar = new System.Windows.Forms.TrackBar();
            this.rxTrackBar = new System.Windows.Forms.TrackBar();
            this.gMapControl1 = new GMap.NET.WindowsForms.GMapControl();
            this.txDistLabel = new System.Windows.Forms.Label();
            this.rxDistLabel = new System.Windows.Forms.Label();
            this.lonLatLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.txTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.rxTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(35, 41);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Username:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(35, 67);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 16);
            this.label2.TabIndex = 1;
            this.label2.Text = "Frequency:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(39, 107);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(70, 16);
            this.label3.TabIndex = 2;
            this.label3.Text = "Tx Range:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(39, 147);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(71, 16);
            this.label4.TabIndex = 3;
            this.label4.Text = "Rx Range:";
            // 
            // userNameBox
            // 
            this.userNameBox.Location = new System.Drawing.Point(116, 36);
            this.userNameBox.Name = "userNameBox";
            this.userNameBox.Size = new System.Drawing.Size(100, 20);
            this.userNameBox.TabIndex = 4;
            this.userNameBox.Text = "EGLL_TWR";
            // 
            // frequencyBox
            // 
            this.frequencyBox.Location = new System.Drawing.Point(116, 67);
            this.frequencyBox.Name = "frequencyBox";
            this.frequencyBox.Size = new System.Drawing.Size(100, 20);
            this.frequencyBox.TabIndex = 5;
            this.frequencyBox.Text = "118.500";
            // 
            // txTrackBar
            // 
            this.txTrackBar.Location = new System.Drawing.Point(116, 96);
            this.txTrackBar.Maximum = 500;
            this.txTrackBar.Name = "txTrackBar";
            this.txTrackBar.Size = new System.Drawing.Size(198, 45);
            this.txTrackBar.TabIndex = 6;
            this.txTrackBar.Value = 50;
            this.txTrackBar.Scroll += new System.EventHandler(this.txTrackBar_Scroll);
            // 
            // rxTrackBar
            // 
            this.rxTrackBar.Location = new System.Drawing.Point(116, 147);
            this.rxTrackBar.Maximum = 500;
            this.rxTrackBar.Name = "rxTrackBar";
            this.rxTrackBar.Size = new System.Drawing.Size(198, 45);
            this.rxTrackBar.TabIndex = 7;
            this.rxTrackBar.Value = 50;
            this.rxTrackBar.Scroll += new System.EventHandler(this.rxTrackBar_Scroll);
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
            this.gMapControl1.MouseWheelZoomType = GMap.NET.MouseWheelZoomType.MousePositionAndCenter;
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
            // txDistLabel
            // 
            this.txDistLabel.AutoSize = true;
            this.txDistLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txDistLabel.Location = new System.Drawing.Point(320, 107);
            this.txDistLabel.Name = "txDistLabel";
            this.txDistLabel.Size = new System.Drawing.Size(22, 16);
            this.txDistLabel.TabIndex = 11;
            this.txDistLabel.Text = "50";
            // 
            // rxDistLabel
            // 
            this.rxDistLabel.AutoSize = true;
            this.rxDistLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rxDistLabel.Location = new System.Drawing.Point(320, 147);
            this.rxDistLabel.Name = "rxDistLabel";
            this.rxDistLabel.Size = new System.Drawing.Size(22, 16);
            this.rxDistLabel.TabIndex = 12;
            this.rxDistLabel.Text = "50";
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
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(740, 618);
            this.Controls.Add(this.lonLatLabel);
            this.Controls.Add(this.rxDistLabel);
            this.Controls.Add(this.txDistLabel);
            this.Controls.Add(this.gMapControl1);
            this.Controls.Add(this.rxTrackBar);
            this.Controls.Add(this.txTrackBar);
            this.Controls.Add(this.frequencyBox);
            this.Controls.Add(this.userNameBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "GeoVR Test Client";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.txTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.rxTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox userNameBox;
        private System.Windows.Forms.TextBox frequencyBox;
        private System.Windows.Forms.TrackBar txTrackBar;
        private System.Windows.Forms.TrackBar rxTrackBar;
        private GMap.NET.WindowsForms.GMapControl gMapControl1;
        private System.Windows.Forms.Label txDistLabel;
        private System.Windows.Forms.Label rxDistLabel;
        private System.Windows.Forms.Label lonLatLabel;
    }
}

