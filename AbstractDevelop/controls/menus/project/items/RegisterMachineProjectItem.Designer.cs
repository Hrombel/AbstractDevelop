namespace AbstractDevelop.controls.menus.project.items
{
    partial class RegisterMachineProjectItem
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

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Обязательный метод для поддержки конструктора - не изменяйте 
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.browser = new System.Windows.Forms.WebBrowser();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.parallelBtn = new System.Windows.Forms.RadioButton();
            this.classicBtn = new System.Windows.Forms.RadioButton();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // browser
            // 
            this.browser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browser.IsWebBrowserContextMenuEnabled = false;
            this.browser.Location = new System.Drawing.Point(0, 0);
            this.browser.Margin = new System.Windows.Forms.Padding(0);
            this.browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.browser.Name = "browser";
            this.browser.Size = new System.Drawing.Size(525, 276);
            this.browser.TabIndex = 2;
            this.browser.WebBrowserShortcutsEnabled = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.parallelBtn);
            this.panel1.Controls.Add(this.classicBtn);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 276);
            this.panel1.Margin = new System.Windows.Forms.Padding(0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(525, 26);
            this.panel1.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(232, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 13);
            this.label1.TabIndex = 11;
            this.label1.Text = "Модификации:";
            // 
            // parallelBtn
            // 
            this.parallelBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.parallelBtn.AutoSize = true;
            this.parallelBtn.Location = new System.Drawing.Point(422, 4);
            this.parallelBtn.Name = "parallelBtn";
            this.parallelBtn.Size = new System.Drawing.Size(99, 17);
            this.parallelBtn.TabIndex = 9;
            this.parallelBtn.Text = "Параллельная";
            this.parallelBtn.UseVisualStyleBackColor = true;
            // 
            // classicBtn
            // 
            this.classicBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.classicBtn.AutoSize = true;
            this.classicBtn.Checked = true;
            this.classicBtn.Location = new System.Drawing.Point(319, 4);
            this.classicBtn.Name = "classicBtn";
            this.classicBtn.Size = new System.Drawing.Size(97, 17);
            this.classicBtn.TabIndex = 10;
            this.classicBtn.TabStop = true;
            this.classicBtn.Text = "Классическая";
            this.classicBtn.UseVisualStyleBackColor = true;
            // 
            // RegisterMachineProjectItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.browser);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "RegisterMachineProjectItem";
            this.Size = new System.Drawing.Size(525, 302);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.WebBrowser browser;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton classicBtn;
        private System.Windows.Forms.RadioButton parallelBtn;
        private System.Windows.Forms.Label label1;
    }
}
