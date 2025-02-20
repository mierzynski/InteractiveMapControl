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
            this.groupVisibilityByLevelComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.infoPanel = new System.Windows.Forms.Panel();
            this.listBox = new System.Windows.Forms.ListBox();
            this.backgroundPictureBox = new System.Windows.Forms.PictureBox();
            this.panelScroll = new System.Windows.Forms.Panel();
            this.findObjectComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStrip1.SuspendLayout();
            this.infoPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.backgroundPictureBox)).BeginInit();
            this.panelScroll.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.groupVisibilityByLevelComboBox,
            this.findObjectComboBox});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStrip1.Size = new System.Drawing.Size(1051, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // groupVisibilityByLevelComboBox
            // 
            this.groupVisibilityByLevelComboBox.Name = "groupVisibilityByLevelComboBox";
            this.groupVisibilityByLevelComboBox.Size = new System.Drawing.Size(121, 25);
            this.groupVisibilityByLevelComboBox.SelectedIndexChanged += new System.EventHandler(this.groupVisibilityByLevelComboBox_SelectedIndexChanged);
            // 
            // infoPanel
            // 
            this.infoPanel.BackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.infoPanel.Controls.Add(this.listBox);
            this.infoPanel.Location = new System.Drawing.Point(806, 25);
            this.infoPanel.Name = "infoPanel";
            this.infoPanel.Size = new System.Drawing.Size(242, 500);
            this.infoPanel.TabIndex = 6;
            // 
            // listBox
            // 
            this.listBox.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.listBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox.FormattingEnabled = true;
            this.listBox.Location = new System.Drawing.Point(0, 0);
            this.listBox.Name = "listBox";
            this.listBox.Size = new System.Drawing.Size(242, 500);
            this.listBox.TabIndex = 0;
            // 
            // backgroundPictureBox
            // 
            this.backgroundPictureBox.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.backgroundPictureBox.Location = new System.Drawing.Point(0, 0);
            this.backgroundPictureBox.Name = "backgroundPictureBox";
            this.backgroundPictureBox.Size = new System.Drawing.Size(800, 500);
            this.backgroundPictureBox.TabIndex = 7;
            this.backgroundPictureBox.TabStop = false;
            // 
            // panelScroll
            // 
            this.panelScroll.AutoScroll = true;
            this.panelScroll.AutoScrollMinSize = new System.Drawing.Size(600, 600);
            this.panelScroll.Controls.Add(this.backgroundPictureBox);
            this.panelScroll.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelScroll.Location = new System.Drawing.Point(0, 25);
            this.panelScroll.MaximumSize = new System.Drawing.Size(800, 500);
            this.panelScroll.Name = "panelScroll";
            this.panelScroll.Size = new System.Drawing.Size(800, 500);
            this.panelScroll.TabIndex = 8;
            // 
            // findObjectComboBox
            // 
            this.findObjectComboBox.Name = "findObjectComboBox";
            this.findObjectComboBox.Size = new System.Drawing.Size(121, 25);
            this.findObjectComboBox.SelectedIndexChanged += new System.EventHandler(this.findObjectComboBox_SelectedIndexChanged);
            // 
            // MapControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panelScroll);
            this.Controls.Add(this.infoPanel);
            this.Controls.Add(this.toolStrip1);
            this.Name = "MapControl";
            this.Size = new System.Drawing.Size(1051, 534);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.infoPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.backgroundPictureBox)).EndInit();
            this.panelScroll.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.Panel infoPanel;
        private System.Windows.Forms.ListBox listBox;
        private System.Windows.Forms.PictureBox backgroundPictureBox;
        private System.Windows.Forms.Panel panelScroll;
        private System.Windows.Forms.ToolStripComboBox groupVisibilityByLevelComboBox;
        private System.Windows.Forms.ToolStripComboBox findObjectComboBox;
    }
}
