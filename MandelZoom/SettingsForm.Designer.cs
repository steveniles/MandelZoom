namespace MandelZoom
{
	internal sealed partial class SettingsForm
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
			if (disposing && (this.components != null))
			{
				this.components.Dispose();
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
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.pictureBox = new System.Windows.Forms.PictureBox();
			this.colorSchemeComboBox = new System.Windows.Forms.ComboBox();
			this.spanScreensCheckBox = new System.Windows.Forms.CheckBox();
			this.creatorLinkLabel = new System.Windows.Forms.LinkLabel();
			this.colorSchemeRandomizeCheckBox = new System.Windows.Forms.CheckBox();
			this.colorSchemeGroupBox = new System.Windows.Forms.GroupBox();
			this.opacityGroupBox = new System.Windows.Forms.GroupBox();
			this.opacityTrackBar = new System.Windows.Forms.TrackBar();
			this.opacityRandomizeCheckBox = new System.Windows.Forms.CheckBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
			this.colorSchemeGroupBox.SuspendLayout();
			this.opacityGroupBox.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.opacityTrackBar)).BeginInit();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(150, 209);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(66, 23);
			this.okButton.TabIndex = 3;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.OkButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(222, 209);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(66, 23);
			this.cancelButton.TabIndex = 4;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.CancelButton_Click);
			// 
			// pictureBox
			// 
			this.pictureBox.BackColor = System.Drawing.Color.Black;
			this.pictureBox.Dock = System.Windows.Forms.DockStyle.Left;
			this.pictureBox.Location = new System.Drawing.Point(0, 0);
			this.pictureBox.Name = "pictureBox";
			this.pictureBox.Size = new System.Drawing.Size(144, 244);
			this.pictureBox.TabIndex = 2;
			this.pictureBox.TabStop = false;
			// 
			// colorSchemeComboBox
			// 
			this.colorSchemeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.colorSchemeComboBox.FormattingEnabled = true;
			this.colorSchemeComboBox.Items.AddRange(new object[] {
            "Silver Frost",
            "Neon Glow",
            "Solar Flare",
            "Psychedelic"});
			this.colorSchemeComboBox.Location = new System.Drawing.Point(6, 19);
			this.colorSchemeComboBox.Name = "colorSchemeComboBox";
			this.colorSchemeComboBox.Size = new System.Drawing.Size(126, 21);
			this.colorSchemeComboBox.TabIndex = 0;
			this.colorSchemeComboBox.SelectedIndexChanged += new System.EventHandler(this.ColorSchemeComboBox_SelectedIndexChanged);
			// 
			// spanScreensCheckBox
			// 
			this.spanScreensCheckBox.AutoSize = true;
			this.spanScreensCheckBox.Location = new System.Drawing.Point(156, 186);
			this.spanScreensCheckBox.Name = "spanScreensCheckBox";
			this.spanScreensCheckBox.Size = new System.Drawing.Size(132, 17);
			this.spanScreensCheckBox.TabIndex = 2;
			this.spanScreensCheckBox.Text = "Span Multiple Screens";
			this.spanScreensCheckBox.UseVisualStyleBackColor = true;
			this.spanScreensCheckBox.CheckedChanged += new System.EventHandler(this.SpanScreensCheckBox_CheckedChanged);
			// 
			// creatorLinkLabel
			// 
			this.creatorLinkLabel.ActiveLinkColor = System.Drawing.Color.White;
			this.creatorLinkLabel.AutoSize = true;
			this.creatorLinkLabel.BackColor = System.Drawing.Color.Black;
			this.creatorLinkLabel.ForeColor = System.Drawing.Color.White;
			this.creatorLinkLabel.LinkArea = new System.Windows.Forms.LinkArea(11, 11);
			this.creatorLinkLabel.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
			this.creatorLinkLabel.LinkColor = System.Drawing.Color.Cyan;
			this.creatorLinkLabel.Location = new System.Drawing.Point(12, 214);
			this.creatorLinkLabel.Name = "creatorLinkLabel";
			this.creatorLinkLabel.Size = new System.Drawing.Size(120, 17);
			this.creatorLinkLabel.TabIndex = 5;
			this.creatorLinkLabel.TabStop = true;
			this.creatorLinkLabel.Text = "Created by Steve Niles";
			this.creatorLinkLabel.UseCompatibleTextRendering = true;
			this.creatorLinkLabel.VisitedLinkColor = System.Drawing.Color.Red;
			this.creatorLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.CreatorLinkLabel_LinkClicked);
			// 
			// colorSchemeRandomizeCheckBox
			// 
			this.colorSchemeRandomizeCheckBox.AutoSize = true;
			this.colorSchemeRandomizeCheckBox.Location = new System.Drawing.Point(6, 46);
			this.colorSchemeRandomizeCheckBox.Name = "colorSchemeRandomizeCheckBox";
			this.colorSchemeRandomizeCheckBox.Size = new System.Drawing.Size(79, 17);
			this.colorSchemeRandomizeCheckBox.TabIndex = 1;
			this.colorSchemeRandomizeCheckBox.Text = "Randomize";
			this.colorSchemeRandomizeCheckBox.UseVisualStyleBackColor = true;
			this.colorSchemeRandomizeCheckBox.CheckedChanged += new System.EventHandler(this.ColorSchemeRandomizeCheckBox_CheckedChanged);
			// 
			// colorSchemeGroupBox
			// 
			this.colorSchemeGroupBox.Controls.Add(this.colorSchemeComboBox);
			this.colorSchemeGroupBox.Controls.Add(this.colorSchemeRandomizeCheckBox);
			this.colorSchemeGroupBox.Location = new System.Drawing.Point(150, 12);
			this.colorSchemeGroupBox.Name = "colorSchemeGroupBox";
			this.colorSchemeGroupBox.Size = new System.Drawing.Size(138, 69);
			this.colorSchemeGroupBox.TabIndex = 0;
			this.colorSchemeGroupBox.TabStop = false;
			this.colorSchemeGroupBox.Text = "Color Scheme";
			// 
			// opacityGroupBox
			// 
			this.opacityGroupBox.Controls.Add(this.opacityTrackBar);
			this.opacityGroupBox.Controls.Add(this.opacityRandomizeCheckBox);
			this.opacityGroupBox.Location = new System.Drawing.Point(150, 87);
			this.opacityGroupBox.Name = "opacityGroupBox";
			this.opacityGroupBox.Size = new System.Drawing.Size(138, 93);
			this.opacityGroupBox.TabIndex = 1;
			this.opacityGroupBox.TabStop = false;
			this.opacityGroupBox.Text = "Opacity: 100%";
			// 
			// opacityTrackBar
			// 
			this.opacityTrackBar.Location = new System.Drawing.Point(6, 19);
			this.opacityTrackBar.Maximum = 100;
			this.opacityTrackBar.Minimum = 25;
			this.opacityTrackBar.Name = "opacityTrackBar";
			this.opacityTrackBar.Size = new System.Drawing.Size(126, 45);
			this.opacityTrackBar.TabIndex = 0;
			this.opacityTrackBar.TickFrequency = 5;
			this.opacityTrackBar.TickStyle = System.Windows.Forms.TickStyle.Both;
			this.opacityTrackBar.Value = 100;
			this.opacityTrackBar.ValueChanged += new System.EventHandler(this.OpacityTrackBar_ValueChanged);
			// 
			// opacityRandomizeCheckBox
			// 
			this.opacityRandomizeCheckBox.AutoSize = true;
			this.opacityRandomizeCheckBox.Location = new System.Drawing.Point(6, 70);
			this.opacityRandomizeCheckBox.Name = "opacityRandomizeCheckBox";
			this.opacityRandomizeCheckBox.Size = new System.Drawing.Size(79, 17);
			this.opacityRandomizeCheckBox.TabIndex = 1;
			this.opacityRandomizeCheckBox.Text = "Randomize";
			this.opacityRandomizeCheckBox.UseVisualStyleBackColor = true;
			this.opacityRandomizeCheckBox.CheckedChanged += new System.EventHandler(this.OpacityRandomizeCheckBox_CheckedChanged);
			// 
			// SettingsForm
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(300, 244);
			this.Controls.Add(this.opacityGroupBox);
			this.Controls.Add(this.colorSchemeGroupBox);
			this.Controls.Add(this.creatorLinkLabel);
			this.Controls.Add(this.spanScreensCheckBox);
			this.Controls.Add(this.pictureBox);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SettingsForm";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "MandelZoom Settings";
			this.Load += new System.EventHandler(this.SettingsForm_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
			this.colorSchemeGroupBox.ResumeLayout(false);
			this.colorSchemeGroupBox.PerformLayout();
			this.opacityGroupBox.ResumeLayout(false);
			this.opacityGroupBox.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.opacityTrackBar)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.PictureBox pictureBox;
		private System.Windows.Forms.ComboBox colorSchemeComboBox;
		private System.Windows.Forms.CheckBox spanScreensCheckBox;
		private System.Windows.Forms.LinkLabel creatorLinkLabel;
		private System.Windows.Forms.CheckBox colorSchemeRandomizeCheckBox;
		private System.Windows.Forms.GroupBox colorSchemeGroupBox;
		private System.Windows.Forms.GroupBox opacityGroupBox;
		private System.Windows.Forms.CheckBox opacityRandomizeCheckBox;
		private System.Windows.Forms.TrackBar opacityTrackBar;
	}
}