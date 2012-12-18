namespace MandelZoom
{
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Drawing;
	using System.Windows.Forms;
	using Properties;

	internal sealed partial class SettingsForm : Form
	{
		internal SettingsForm()
		{
			this.InitializeComponent();
		}
		private void SettingsForm_Load(object sender, System.EventArgs e)
		{
			this.colorSchemeComboBox.SelectedIndex = Settings.Default.ColorScheme;
			this.colorSchemeRandomizeCheckBox.Checked = Settings.Default.RandomColorScheme;
			this.opacityTrackBar.Value = Settings.Default.OpacityPercent;
			this.opacityRandomizeCheckBox.Checked = Settings.Default.RandomOpacity;
			this.spanScreensCheckBox.Checked = Settings.Default.SpanScreens;
		}

		private void ColorSchemeComboBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			switch (this.colorSchemeComboBox.SelectedIndex)
			{
				default:
					this.pictureBox.Image = Resources.SilverFrost;
					this.creatorLinkLabel.LinkColor = Color.Silver;
					this.creatorLinkLabel.ActiveLinkColor = Color.White;
					break;
				case 1:
					this.pictureBox.Image = Resources.NeonGlow;
					this.creatorLinkLabel.LinkColor = Color.FromArgb(0, 64, 255);
					this.creatorLinkLabel.ActiveLinkColor = Color.Lime;
					break;
				case 2:
					this.pictureBox.Image = Resources.SolarFlare;
					this.creatorLinkLabel.LinkColor = Color.Orange;
					this.creatorLinkLabel.ActiveLinkColor = Color.Yellow;
					break;
				case 3:
					this.pictureBox.Image = Resources.Psychedelic;
					this.creatorLinkLabel.LinkColor = Color.LightPink;
					this.creatorLinkLabel.ActiveLinkColor = Color.LightGreen;
					break;
			}
			Settings.Default.ColorScheme = this.colorSchemeComboBox.SelectedIndex;
		}
		private void ColorSchemeRandomizeCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			this.colorSchemeComboBox.Enabled = !this.colorSchemeRandomizeCheckBox.Checked;
			Settings.Default.RandomColorScheme = this.colorSchemeRandomizeCheckBox.Checked;
		}
		private void OpacityTrackBar_ValueChanged(object sender, System.EventArgs e)
		{
			this.opacityGroupBox.Text = "Opacity: " + this.opacityTrackBar.Value + "%";
			if (!this.opacityRandomizeCheckBox.Checked) this.Opacity = this.opacityTrackBar.Value / 100D;
			Settings.Default.OpacityPercent = this.opacityTrackBar.Value;
		}
		private void OpacityRandomizeCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			this.opacityTrackBar.Enabled = !this.opacityRandomizeCheckBox.Checked;
			if (this.opacityRandomizeCheckBox.Checked) this.Opacity = 1;
			else this.Opacity = this.opacityTrackBar.Value / 100D;
			Settings.Default.RandomOpacity = this.opacityRandomizeCheckBox.Checked;
		}
		private void SpanScreensCheckBox_CheckedChanged(object sender, System.EventArgs e)
		{
			Settings.Default.SpanScreens = this.spanScreensCheckBox.Checked;
		}

		private void OkButton_Click(object sender, System.EventArgs e)
		{
			Settings.Default.Save();
			this.Close();
		}
		private void CancelButton_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
		private void CreatorLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				Process.Start("mailto:steve.niles@gmail.com");
			}
			catch (Win32Exception x)
			{
				if (x.NativeErrorCode == 1155) MessageBox.Show("Send comments or questions to steve.niles@gmail.com", "No default email client found");
			}
		}
	}
}
