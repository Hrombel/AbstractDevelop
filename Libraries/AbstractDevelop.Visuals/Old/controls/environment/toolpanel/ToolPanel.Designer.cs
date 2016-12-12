namespace AbstractDevelop.controls.environment.toolpanel
{
    partial class ToolPanel
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
            this.strip = new System.Windows.Forms.ToolStrip();
            this.runBtn = new System.Windows.Forms.ToolStripButton();
            this.stepBtn = new System.Windows.Forms.ToolStripButton();
            this.pauseBtn = new System.Windows.Forms.ToolStripButton();
            this.stopBtn = new System.Windows.Forms.ToolStripButton();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.shortcutsItem = new System.Windows.Forms.ToolStripMenuItem();
            this.startItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stepItem = new System.Windows.Forms.ToolStripMenuItem();
            this.strip.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // strip
            // 
            this.strip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.strip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runBtn,
            this.stepBtn,
            this.pauseBtn,
            this.stopBtn});
            this.strip.Location = new System.Drawing.Point(0, 0);
            this.strip.Name = "strip";
            this.strip.Size = new System.Drawing.Size(462, 26);
            this.strip.TabIndex = 0;
            this.strip.Text = "toolStrip1";
            // 
            // runBtn
            // 
            this.runBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.runBtn.Image = global::AbstractDevelop.Properties.Resources.PlayBtn;
            this.runBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.runBtn.Name = "runBtn";
            this.runBtn.Size = new System.Drawing.Size(23, 23);
            this.runBtn.Text = "Запуск F5";
            this.runBtn.Click += new System.EventHandler(this.BtnClick);
            // 
            // stepBtn
            // 
            this.stepBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stepBtn.Image = global::AbstractDevelop.Properties.Resources.StepBtn;
            this.stepBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stepBtn.Name = "stepBtn";
            this.stepBtn.Size = new System.Drawing.Size(23, 23);
            this.stepBtn.Text = "Шаг F10";
            this.stepBtn.Click += new System.EventHandler(this.BtnClick);
            // 
            // pauseBtn
            // 
            this.pauseBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.pauseBtn.Image = global::AbstractDevelop.Properties.Resources.PauseBtn;
            this.pauseBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.pauseBtn.Name = "pauseBtn";
            this.pauseBtn.Size = new System.Drawing.Size(23, 23);
            this.pauseBtn.Text = "Прервать F6";
            this.pauseBtn.Click += new System.EventHandler(this.BtnClick);
            // 
            // stopBtn
            // 
            this.stopBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopBtn.Image = global::AbstractDevelop.Properties.Resources.StopBtn;
            this.stopBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopBtn.Name = "stopBtn";
            this.stopBtn.Size = new System.Drawing.Size(23, 23);
            this.stopBtn.Text = "Остановить отладку Shift + F5";
            this.stopBtn.Click += new System.EventHandler(this.BtnClick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.shortcutsItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(462, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.Visible = false;
            // 
            // shortcutsItem
            // 
            this.shortcutsItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.startItem,
            this.stopItem,
            this.pauseItem,
            this.stepItem});
            this.shortcutsItem.Name = "shortcutsItem";
            this.shortcutsItem.Size = new System.Drawing.Size(118, 20);
            this.shortcutsItem.Text = "Горячие клавиши";
            // 
            // startItem
            // 
            this.startItem.Name = "startItem";
            this.startItem.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.startItem.Size = new System.Drawing.Size(152, 22);
            this.startItem.Text = "Старт";
            this.startItem.Click += new System.EventHandler(this.ShortPressed);
            // 
            // stopItem
            // 
            this.stopItem.Name = "stopItem";
            this.stopItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Shift | System.Windows.Forms.Keys.F5)));
            this.stopItem.Size = new System.Drawing.Size(152, 22);
            this.stopItem.Text = "Стоп";
            this.stopItem.Click += new System.EventHandler(this.ShortPressed);
            // 
            // pauseItem
            // 
            this.pauseItem.Name = "pauseItem";
            this.pauseItem.ShortcutKeys = System.Windows.Forms.Keys.F6;
            this.pauseItem.Size = new System.Drawing.Size(152, 22);
            this.pauseItem.Text = "Пауза";
            this.pauseItem.Click += new System.EventHandler(this.ShortPressed);
            // 
            // stepItem
            // 
            this.stepItem.Name = "stepItem";
            this.stepItem.ShortcutKeys = System.Windows.Forms.Keys.F10;
            this.stepItem.Size = new System.Drawing.Size(152, 22);
            this.stepItem.Text = "Шаг";
            this.stepItem.Click += new System.EventHandler(this.ShortPressed);
            // 
            // ToolPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.strip);
            this.Name = "ToolPanel";
            this.Size = new System.Drawing.Size(462, 26);
            this.strip.ResumeLayout(false);
            this.strip.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip strip;
        private System.Windows.Forms.ToolStripButton runBtn;
        private System.Windows.Forms.ToolStripButton stopBtn;
        private System.Windows.Forms.ToolStripButton pauseBtn;
        private System.Windows.Forms.ToolStripButton stepBtn;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem shortcutsItem;
        private System.Windows.Forms.ToolStripMenuItem startItem;
        private System.Windows.Forms.ToolStripMenuItem stopItem;
        private System.Windows.Forms.ToolStripMenuItem pauseItem;
        private System.Windows.Forms.ToolStripMenuItem stepItem;
    }
}
