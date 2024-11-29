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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.axisYPanel = new System.Windows.Forms.Panel();
            this.axisXPanel = new System.Windows.Forms.Panel();
            this.infoPanel = new System.Windows.Forms.Panel();
            this.listBox = new System.Windows.Forms.ListBox();
            this.backgroundPictureBox = new System.Windows.Forms.PictureBox();
            this.infoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.backgroundPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(784, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // axisYPanel
            // 
            this.axisYPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.axisYPanel.Location = new System.Drawing.Point(7, 28);
            this.axisYPanel.Name = "axisYPanel";
            this.axisYPanel.Size = new System.Drawing.Size(25, 304);
            this.axisYPanel.TabIndex = 4;
            // 
            // axisXPanel
            // 
            this.axisXPanel.BackColor = System.Drawing.SystemColors.ControlLight;
            this.axisXPanel.Location = new System.Drawing.Point(33, 329);
            this.axisXPanel.Name = "axisXPanel";
            this.axisXPanel.Size = new System.Drawing.Size(551, 25);
            this.axisXPanel.TabIndex = 5;
            // 
            // infoPanel
            // 
            this.infoPanel.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.infoPanel.Controls.Add(this.listBox);
            this.infoPanel.Location = new System.Drawing.Point(7, 360);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new System.Drawing.Size(577, 134);
            this.infoPanel.TabIndex = 6;
            // 
            // listBox
            // 
            this.listBox.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(0, 0);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(577, 134);
            this.listBox.TabIndex = 0;
            // 
            // backgroundPictureBox
            // 
            this.backgroundPictureBox.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.backgroundPictureBox.Location = new System.Drawing.Point(33, 28);
            this.backgroundPictureBox.Name = "backgroundPictureBox";
            this.backgroundPictureBox.Size = new System.Drawing.Size(551, 304);
            this.backgroundPictureBox.TabIndex = 7;
            this.backgroundPictureBox.TabStop = false;
            // 
            // MapControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.backgroundPictureBox);
            this.Controls.Add(this.infoPanel);
            this.Controls.Add(this.axisXPanel);
            this.Controls.Add(this.axisYPanel);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MapControl";
            this.Size = new System.Drawing.Size(784, 505);
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
    }
}
