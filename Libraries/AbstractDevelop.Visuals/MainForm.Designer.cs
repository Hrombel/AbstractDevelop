namespace AbstractDevelop
{
    partial class MainForm
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.ui = new AbstractDevelop.controls.AbstractDevelopUI();
            this.frameControl1 = new AbstractDevelop.controls.environment.tabcontrol.VisualsControl();
            this.SuspendLayout();
            // 
            // ui
            // 
            this.ui.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ui.Location = new System.Drawing.Point(0, 0);
            this.ui.Name = "ui";
            this.ui.Size = new System.Drawing.Size(624, 340);
            this.ui.TabIndex = 0;
            this.ui.ExitRequest += new System.EventHandler(this.ui_ExitRequest);
            // 
            // frameControl1
            // 
            this.frameControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.frameControl1.Location = new System.Drawing.Point(0, 33);
            this.frameControl1.Name = "frameControl1";
            this.frameControl1.Size = new System.Drawing.Size(624, 307);
            this.frameControl1.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 340);
            this.Controls.Add(this.ui);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Название программы";
            this.ResumeLayout(false);

        }

        #endregion

        private controls.environment.tabcontrol.VisualsControl frameControl1;
        private controls.AbstractDevelopUI ui;












    }
}

