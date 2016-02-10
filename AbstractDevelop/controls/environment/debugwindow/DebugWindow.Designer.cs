namespace AbstractDevelop.controls.environment.debugwindow
{
    partial class DebugWindow
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
            this.components = new System.ComponentModel.Container();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listView = new System.Windows.Forms.ListView();
            this.numCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.senderCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listViewStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearItem = new System.Windows.Forms.ToolStripMenuItem();
            this.textCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1.SuspendLayout();
            this.listViewStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listView);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(384, 136);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Окно отладки";
            // 
            // listView
            // 
            this.listView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.numCol,
            this.senderCol,
            this.textCol});
            this.listView.ContextMenuStrip = this.listViewStrip;
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(3, 16);
            this.listView.Margin = new System.Windows.Forms.Padding(0);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.Size = new System.Drawing.Size(378, 117);
            this.listView.TabIndex = 0;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            // 
            // numCol
            // 
            this.numCol.Text = "№";
            // 
            // senderCol
            // 
            this.senderCol.Text = "Источник";
            this.senderCol.Width = 81;
            // 
            // listViewStrip
            // 
            this.listViewStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearItem});
            this.listViewStrip.Name = "listViewStrip";
            this.listViewStrip.Size = new System.Drawing.Size(148, 26);
            // 
            // clearItem
            // 
            this.clearItem.Name = "clearItem";
            this.clearItem.Size = new System.Drawing.Size(147, 22);
            this.clearItem.Text = "Очистить все";
            this.clearItem.Click += new System.EventHandler(this.clearItem_Click);
            // 
            // textCol
            // 
            this.textCol.Text = "Текст";
            // 
            // DebugWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBox1);
            this.Name = "DebugWindow";
            this.Size = new System.Drawing.Size(384, 136);
            this.groupBox1.ResumeLayout(false);
            this.listViewStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader numCol;
        private System.Windows.Forms.ColumnHeader senderCol;
        private System.Windows.Forms.ContextMenuStrip listViewStrip;
        private System.Windows.Forms.ToolStripMenuItem clearItem;
        private System.Windows.Forms.ColumnHeader textCol;
    }
}
