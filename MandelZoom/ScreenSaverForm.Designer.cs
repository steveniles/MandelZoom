namespace MandelZoom
{
	internal sealed partial class ScreenSaverForm
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
            this.SuspendLayout();
            // 
            // ScreenSaverForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "ScreenSaverForm";
            this.Text = "MandelZoom";
            this.Load += new System.EventHandler(this.ScreenSaverForm_Load);
            this.Shown += new System.EventHandler(this.ScreenSaverForm_Shown);
            this.ResumeLayout(false);

		}

		#endregion
	}
}
