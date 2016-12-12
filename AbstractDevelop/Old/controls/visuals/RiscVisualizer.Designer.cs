using AbstractDevelop.controls.ui.ceditor.editors;

namespace AbstractDevelop.controls.visuals
{
    partial class RiscVisualizer
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
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
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ListViewGroup listViewGroup1 = new System.Windows.Forms.ListViewGroup("ListViewGroup", System.Windows.Forms.HorizontalAlignment.Left);
            System.Windows.Forms.ListViewGroup listViewGroup2 = new System.Windows.Forms.ListViewGroup("ListViewGroup", System.Windows.Forms.HorizontalAlignment.Left);
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.codeEditor = new AbstractDevelop.controls.ui.ceditor.editors.RiscCodeEditor();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.listView = new System.Windows.Forms.ListView();
            this.idCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.progCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.createUnitItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameUnitItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteUnitItem = new System.Windows.Forms.ToolStripMenuItem();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.memoryVisualizer = new AbstractDevelop.controls.visuals.RiscRegistersVisualizer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabContext.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer2.Size = new System.Drawing.Size(669, 174);
            this.splitContainer2.SplitterDistance = 462;
            this.splitContainer2.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.codeEditor);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(462, 174);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Редактор кода";
            // 
            // codeEditor
            // 
            this.codeEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeEditor.Location = new System.Drawing.Point(3, 16);
            this.codeEditor.Name = "codeEditor";
            this.codeEditor.ReadOnly = false;
            this.codeEditor.Size = new System.Drawing.Size(456, 155);
            this.codeEditor.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.listView);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(203, 174);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Регистры";
            // 
            // listView
            // 
            this.listView.AllowColumnReorder = true;
            this.listView.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.idCol,
            this.progCol});
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            listViewGroup1.Header = "ListViewGroup";
            listViewGroup1.Name = "listViewGroup1";
            listViewGroup2.Header = "ListViewGroup";
            listViewGroup2.Name = "listViewGroup2";
            this.listView.Groups.AddRange(new System.Windows.Forms.ListViewGroup[] {
            listViewGroup1,
            listViewGroup2});
            this.listView.LabelEdit = true;
            this.listView.LabelWrap = false;
            this.listView.Location = new System.Drawing.Point(3, 16);
            this.listView.Margin = new System.Windows.Forms.Padding(0);
            this.listView.MultiSelect = false;
            this.listView.Name = "listView";
            this.listView.ShowGroups = false;
            this.listView.Size = new System.Drawing.Size(197, 155);
            this.listView.TabIndex = 1;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.DoubleClick += new System.EventHandler(this.listView_DoubleClick);
            // 
            // idCol
            // 
            this.idCol.Text = "#";
            this.idCol.Width = 40;
            // 
            // progCol
            // 
            this.progCol.Text = "Значение";
            this.progCol.Width = 140;
            // 
            // tabContext
            // 
            this.tabContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.createUnitItem,
            this.renameUnitItem,
            this.deleteUnitItem});
            this.tabContext.Name = "tabContext";
            this.tabContext.Size = new System.Drawing.Size(162, 70);
            this.tabContext.Text = "Действия";
            // 
            // createUnitItem
            // 
            this.createUnitItem.Name = "createUnitItem";
            this.createUnitItem.Size = new System.Drawing.Size(161, 22);
            this.createUnitItem.Text = "Новый";
            // 
            // renameUnitItem
            // 
            this.renameUnitItem.Name = "renameUnitItem";
            this.renameUnitItem.Size = new System.Drawing.Size(161, 22);
            this.renameUnitItem.Text = "Переименовать";
            // 
            // deleteUnitItem
            // 
            this.deleteUnitItem.Name = "deleteUnitItem";
            this.deleteUnitItem.Size = new System.Drawing.Size(161, 22);
            this.deleteUnitItem.Text = "Удалить";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.memoryVisualizer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(669, 381);
            this.splitContainer1.SplitterDistance = 203;
            this.splitContainer1.TabIndex = 1;
            // 
            // memoryVisualizer
            // 
            this.memoryVisualizer.Columns = 5;
            this.memoryVisualizer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.memoryVisualizer.Location = new System.Drawing.Point(0, 0);
            this.memoryVisualizer.Name = "memoryVisualizer";
            this.memoryVisualizer.Registers = null;
            this.memoryVisualizer.Size = new System.Drawing.Size(669, 203);
            this.memoryVisualizer.TabIndex = 0;
            // 
            // RiscVisualizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.splitContainer1);
            this.Name = "RiscVisualizer";
            this.Size = new System.Drawing.Size(669, 381);
            this.Load += new System.EventHandler(this.RiscVisualizer_Load);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tabContext.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ContextMenuStrip tabContext;
        private System.Windows.Forms.ToolStripMenuItem createUnitItem;
        private System.Windows.Forms.ToolStripMenuItem renameUnitItem;
        private System.Windows.Forms.ToolStripMenuItem deleteUnitItem;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private RiscRegistersVisualizer memoryVisualizer;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ListView listView;
        private System.Windows.Forms.ColumnHeader idCol;
        private System.Windows.Forms.ColumnHeader progCol;
        private RiscCodeEditor codeEditor;
    }
}
