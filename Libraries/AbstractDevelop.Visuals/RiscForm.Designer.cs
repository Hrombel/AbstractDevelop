namespace AbstractDevelop
{
    partial class RiscForm
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
            this.workEnvironment = new AbstractDevelop.controls.environment.WorkEnvironment();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.openMenuButton = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMenuButton = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // workEnvironment
            // 
            this.workEnvironment.Dock = System.Windows.Forms.DockStyle.Fill;
            this.workEnvironment.Location = new System.Drawing.Point(0, 24);
            this.workEnvironment.Name = "workEnvironment";
            this.workEnvironment.Size = new System.Drawing.Size(608, 321);
            this.workEnvironment.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openMenuButton,
            this.saveMenuButton});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(608, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // openMenuButton
            // 
            this.openMenuButton.Name = "openMenuButton";
            this.openMenuButton.Size = new System.Drawing.Size(66, 20);
            this.openMenuButton.Text = "Открыть";
            this.openMenuButton.Click += new System.EventHandler(this.openMenuButton_Click);
            // 
            // saveMenuButton
            // 
            this.saveMenuButton.Name = "saveMenuButton";
            this.saveMenuButton.Size = new System.Drawing.Size(77, 20);
            this.saveMenuButton.Text = "Сохранить";
            this.saveMenuButton.Click += new System.EventHandler(this.saveMenuButton_Click);
            // 
            // RiscForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(608, 345);
            this.Controls.Add(this.workEnvironment);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "RiscForm";
            this.Text = "AbstractDevelop (RISC Edition)";
            this.Load += new System.EventHandler(this.RiscForm_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private controls.environment.WorkEnvironment workEnvironment;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem openMenuButton;
        private System.Windows.Forms.ToolStripMenuItem saveMenuButton;
    }
}