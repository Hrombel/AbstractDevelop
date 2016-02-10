namespace AbstractDevelop.controls.visuals
{
    partial class RegisterMachineVisualizer
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.registersVis = new AbstractDevelop.controls.visuals.InfiniteRegistersVisualizer();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tabCtrl = new System.Windows.Forms.TabControl();
            this.tabContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.createUnitItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameUnitItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteUnitItem = new System.Windows.Forms.ToolStripMenuItem();
            this.threadFrame = new AbstractDevelop.controls.visuals.additionals.ThreadFrame();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabContext.SuspendLayout();
            this.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.registersVis);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(498, 439);
            this.splitContainer1.SplitterDistance = 234;
            this.splitContainer1.TabIndex = 0;
            // 
            // registersVis
            // 
            this.registersVis.Columns = 5;
            this.registersVis.Dock = System.Windows.Forms.DockStyle.Fill;
            this.registersVis.Location = new System.Drawing.Point(0, 0);
            this.registersVis.Name = "registersVis";
            this.registersVis.Registers = null;
            this.registersVis.Size = new System.Drawing.Size(498, 234);
            this.registersVis.TabIndex = 0;
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
            this.splitContainer2.Panel2.Controls.Add(this.threadFrame);
            this.splitContainer2.Size = new System.Drawing.Size(498, 201);
            this.splitContainer2.SplitterDistance = 344;
            this.splitContainer2.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tabCtrl);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(344, 201);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Редактор кода";
            // 
            // tabCtrl
            // 
            this.tabCtrl.ContextMenuStrip = this.tabContext;
            this.tabCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabCtrl.Location = new System.Drawing.Point(3, 16);
            this.tabCtrl.Margin = new System.Windows.Forms.Padding(0);
            this.tabCtrl.Name = "tabCtrl";
            this.tabCtrl.SelectedIndex = 0;
            this.tabCtrl.Size = new System.Drawing.Size(338, 182);
            this.tabCtrl.TabIndex = 2;
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
            // threadFrame
            // 
            this.threadFrame.Dock = System.Windows.Forms.DockStyle.Fill;
            this.threadFrame.Location = new System.Drawing.Point(0, 0);
            this.threadFrame.Name = "threadFrame";
            this.threadFrame.Size = new System.Drawing.Size(150, 201);
            this.threadFrame.TabIndex = 0;
            // 
            // RegisterMachineVisualizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "RegisterMachineVisualizer";
            this.Size = new System.Drawing.Size(498, 439);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tabContext.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private InfiniteRegistersVisualizer registersVis;
        private System.Windows.Forms.ContextMenuStrip tabContext;
        private System.Windows.Forms.ToolStripMenuItem createUnitItem;
        private System.Windows.Forms.ToolStripMenuItem deleteUnitItem;
        private System.Windows.Forms.ToolStripMenuItem renameUnitItem;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private additionals.ThreadFrame threadFrame;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TabControl tabCtrl;
    }
}
