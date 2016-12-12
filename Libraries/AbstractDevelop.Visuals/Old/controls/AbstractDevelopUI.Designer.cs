namespace AbstractDevelop.controls
{
    partial class AbstractDevelopUI
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
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveItem = new System.Windows.Forms.ToolStripMenuItem();
            this.closeItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exitItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shortcutsItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutItem = new System.Windows.Forms.ToolStripMenuItem();
            this.backBtn = new System.Windows.Forms.ToolStripMenuItem();
            this.stateCtrl = new AbstractDevelop.controls.ui.statecontrol.StateControl();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileItem,
            this.helpItem,
            this.shortcutsItem,
            this.aboutItem,
            this.backBtn});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(564, 24);
            this.menuStrip.TabIndex = 4;
            this.menuStrip.Text = "menuStrip1";
            // 
            // fileItem
            // 
            this.fileItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createItem,
            this.openItem,
            this.saveItem,
            this.closeItem,
            this.exitItem});
            this.fileItem.Name = "fileItem";
            this.fileItem.Size = new System.Drawing.Size(48, 20);
            this.fileItem.Text = "Файл";
            // 
            // createItem
            // 
            this.createItem.Name = "createItem";
            this.createItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.createItem.Size = new System.Drawing.Size(173, 22);
            this.createItem.Text = "Создать...";
            this.createItem.Click += new System.EventHandler(this.createItem_Click);
            // 
            // openItem
            // 
            this.openItem.Name = "openItem";
            this.openItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openItem.Size = new System.Drawing.Size(173, 22);
            this.openItem.Text = "Открыть...";
            this.openItem.Click += new System.EventHandler(this.openItem_Click);
            // 
            // saveItem
            // 
            this.saveItem.Name = "saveItem";
            this.saveItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.saveItem.Size = new System.Drawing.Size(173, 22);
            this.saveItem.Text = "Сохранить";
            this.saveItem.Click += new System.EventHandler(this.saveItem_Click);
            // 
            // closeItem
            // 
            this.closeItem.Name = "closeItem";
            this.closeItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.W)));
            this.closeItem.Size = new System.Drawing.Size(173, 22);
            this.closeItem.Text = "Закрыть";
            this.closeItem.Click += new System.EventHandler(this.closeItem_Click);
            // 
            // exitItem
            // 
            this.exitItem.Name = "exitItem";
            this.exitItem.Size = new System.Drawing.Size(173, 22);
            this.exitItem.Text = "Выход";
            this.exitItem.Click += new System.EventHandler(this.exitItem_Click);
            // 
            // helpItem
            // 
            this.helpItem.Name = "helpItem";
            this.helpItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.helpItem.Size = new System.Drawing.Size(65, 20);
            this.helpItem.Text = "Справка";
            this.helpItem.Click += new System.EventHandler(this.helpItem_Click);
            // 
            // shortcutsItem
            // 
            this.shortcutsItem.Name = "shortcutsItem";
            this.shortcutsItem.Size = new System.Drawing.Size(108, 20);
            this.shortcutsItem.Text = "Горячие кнопки";
            this.shortcutsItem.Visible = false;
            // 
            // aboutItem
            // 
            this.aboutItem.Name = "aboutItem";
            this.aboutItem.Size = new System.Drawing.Size(94, 20);
            this.aboutItem.Text = "О программе";
            this.aboutItem.Click += new System.EventHandler(this.aboutItem_Click);
            // 
            // backBtn
            // 
            this.backBtn.Name = "backBtn";
            this.backBtn.Size = new System.Drawing.Size(51, 20);
            this.backBtn.Text = "Назад";
            this.backBtn.Click += new System.EventHandler(this.backBtn_Click);
            // 
            // stateCtrl
            // 
            this.stateCtrl.CurrentControl = null;
            this.stateCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stateCtrl.Location = new System.Drawing.Point(0, 24);
            this.stateCtrl.Name = "stateCtrl";
            this.stateCtrl.Size = new System.Drawing.Size(564, 272);
            this.stateCtrl.TabIndex = 5;
            // 
            // AbstractDevelopUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.stateCtrl);
            this.Controls.Add(this.menuStrip);
            this.Name = "AbstractDevelopUI";
            this.Size = new System.Drawing.Size(564, 296);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileItem;
        private System.Windows.Forms.ToolStripMenuItem createItem;
        private System.Windows.Forms.ToolStripMenuItem openItem;
        private System.Windows.Forms.ToolStripMenuItem saveItem;
        private System.Windows.Forms.ToolStripMenuItem closeItem;
        private System.Windows.Forms.ToolStripMenuItem exitItem;
        private ui.statecontrol.StateControl stateCtrl;
        private System.Windows.Forms.ToolStripMenuItem backBtn;
        private System.Windows.Forms.ToolStripMenuItem shortcutsItem;
        private System.Windows.Forms.ToolStripMenuItem aboutItem;
        private System.Windows.Forms.ToolStripMenuItem helpItem;
    }
}
