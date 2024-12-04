namespace InteractiveMapControl.cControl
{
    partial class MapControl
    {
        /// <summary> 
        /// Wymagana zmienna projektanta.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Wyczyść wszystkie używane zasoby.
        /// </summary>
        /// <param name="disposing">prawda, jeżeli zarządzane zasoby powinny zostać zlikwidowane; Fałsz w przeciwnym wypadku.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Kod wygenerowany przez Projektanta składników

        /// <summary> 
        /// Metoda wymagana do obsługi projektanta — nie należy modyfikować 
        /// jej zawartości w edytorze kodu.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MapControl));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.axisYPanel = new System.Windows.Forms.Panel();
            this.axisXPanel = new System.Windows.Forms.Panel();
            this.infoPanel = new System.Windows.Forms.Panel();
            this.listBox = new System.Windows.Forms.ListBox();
            this.backgroundPictureBox = new System.Windows.Forms.PictureBox();
            this.buttonResizeMode = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.infoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.backgroundPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonResizeMode});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.toolStrip1.Size = new System.Drawing.Size(970, 38);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // axisYPanel
            // 
            this.axisYPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.axisYPanel.Location = new System.Drawing.Point(10, 43);
            this.axisYPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.axisYPanel.Name = "axisYPanel";
            this.axisYPanel.Size = new System.Drawing.Size(38, 462);
            this.axisYPanel.TabIndex = 4;
            // 
            // axisXPanel
            // 
            this.axisXPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.axisXPanel.Location = new System.Drawing.Point(50, 506);
            this.axisXPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.axisXPanel.Name = "axisXPanel";
            this.axisXPanel.Size = new System.Drawing.Size(900, 38);
            this.axisXPanel.TabIndex = 5;
            // 
            // infoPanel
            // 
            this.infoPanel.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.infoPanel.Controls.Add(this.listBox);
            this.infoPanel.Location = new System.Drawing.Point(68, 554);
            this.infoPanel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new System.Drawing.Size(866, 206);
            this.infoPanel.TabIndex = 6;
            // 
            // listBox
            // 
            this.listBox.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.FormattingEnabled = true;
            this.listBox.ItemHeight = 20;
            this.listBox.Location = new System.Drawing.Point(0, 0);
            this.listBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(866, 206);
            this.listBox.TabIndex = 0;
            // 
            // backgroundPictureBox
            // 
            this.backgroundPictureBox.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.backgroundPictureBox.Location = new System.Drawing.Point(50, 43);
            this.backgroundPictureBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.backgroundPictureBox.Name = "backgroundPictureBox";
            this.backgroundPictureBox.Size = new System.Drawing.Size(900, 462);
            this.backgroundPictureBox.TabIndex = 7;
            this.backgroundPictureBox.TabStop = false;
            // 
            // buttonResizeMode
            // 
            this.buttonResizeMode.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonResizeMode.Image = ((System.Drawing.Image)(resources.GetObject("buttonResizeMode.Image")));
            this.buttonResizeMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonResizeMode.Name = "buttonResizeMode";
            this.buttonResizeMode.Size = new System.Drawing.Size(93, 33);
            this.buttonResizeMode.Text = "trybEdycji";
            // 
            // MapControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.backgroundPictureBox);
            this.Controls.Add(this.infoPanel);
            this.Controls.Add(this.axisXPanel);
            this.Controls.Add(this.axisYPanel);
            this.Controls.Add(this.toolStrip1);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "MapControl";
            this.Size = new System.Drawing.Size(970, 777);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.infoPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.backgroundPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Panel axisYPanel;
        private System.Windows.Forms.Panel axisXPanel;
        private System.Windows.Forms.Panel infoPanel;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.PictureBox backgroundPictureBox;
        private System.Windows.Forms.ToolStripButton buttonResizeMode;
    }
}
