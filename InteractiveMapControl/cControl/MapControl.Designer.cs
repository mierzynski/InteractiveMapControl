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
            this.boardPanel = new System.Windows.Forms.Panel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.AddObjectTest = new System.Windows.Forms.ToolStripButton();
            this.buttonAddHall = new System.Windows.Forms.ToolStripButton();
            this.axisYPanel = new System.Windows.Forms.Panel();
            this.axisXPanel = new System.Windows.Forms.Panel();
            this.infoPanel = new System.Windows.Forms.Panel();
            this.listBox = new System.Windows.Forms.ListBox();
            this.toolStrip1.SuspendLayout();
            this.infoPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // boardPanel
            // 
            this.boardPanel.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.boardPanel.Location = new System.Drawing.Point(33, 28);
            this.boardPanel.Name = "boardPanel";
            this.boardPanel.Size = new System.Drawing.Size(551, 304);
            this.boardPanel.TabIndex = 0;
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddObjectTest,
            this.buttonAddHall});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(784, 27);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // AddObjectTest
            // 
            this.AddObjectTest.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.AddObjectTest.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.AddObjectTest.Image = ((System.Drawing.Image)(resources.GetObject("AddObjectTest.Image")));
            this.AddObjectTest.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.AddObjectTest.Margin = new System.Windows.Forms.Padding(4);
            this.AddObjectTest.Name = "AddObjectTest";
            this.AddObjectTest.Size = new System.Drawing.Size(65, 19);
            this.AddObjectTest.Text = "testObject";
            this.AddObjectTest.Click += new System.EventHandler(this.AddObjectTest_Click);
            // 
            // buttonAddHall
            // 
            this.buttonAddHall.BackColor = System.Drawing.SystemColors.InactiveCaption;
            this.buttonAddHall.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonAddHall.Image = ((System.Drawing.Image)(resources.GetObject("buttonAddHall.Image")));
            this.buttonAddHall.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonAddHall.Margin = new System.Windows.Forms.Padding(4);
            this.buttonAddHall.Name = "buttonAddHall";
            this.buttonAddHall.Size = new System.Drawing.Size(35, 19);
            this.buttonAddHall.Text = "Hala";
            this.buttonAddHall.Click += new System.EventHandler(this.buttonAddHall_Click_1);
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
            // MapControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.infoPanel);
            this.Controls.Add(this.axisXPanel);
            this.Controls.Add(this.axisYPanel);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.boardPanel);
            this.Name = "MapControl";
            this.Size = new System.Drawing.Size(784, 505);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.infoPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel boardPanel;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton AddObjectTest;
        private System.Windows.Forms.Panel axisYPanel;
        private System.Windows.Forms.Panel axisXPanel;
        private System.Windows.Forms.ToolStripButton buttonAddHall;
        private System.Windows.Forms.Panel infoPanel;
        private System.Windows.Forms.ListBox listBox;
    }
}
