namespace elbbp_ui
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._renderPanel = new Components.DoubleBufferedPanel();
            this.SuspendLayout();
            //
            // renderPanel
            //
            this._renderPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._renderPanel.Location = new System.Drawing.Point(0, 0);
            this._renderPanel.Name = "renderPanel";
            this._renderPanel.Size = new System.Drawing.Size(256, 256);
            this._renderPanel.TabIndex = 0;
            //
            // MainForm
            //
            this.components = new System.ComponentModel.Container();
            this.AllowDrop = true;
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this._renderPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = ProgramNameVersion;
            this.ResumeLayout(false);
        }

        #endregion

        private Components.DoubleBufferedPanel _renderPanel;
    }
}

