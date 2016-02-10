namespace AbstractDevelop.controls.menus.help
{
    partial class HelpWindow
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Меню создания проекта");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Рабочая среда");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Интерфейс", new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2});
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Машина Поста");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Машина Тьюринга");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Машина с бесконечными регистрами");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Абстрактные вычислители", new System.Windows.Forms.TreeNode[] {
            treeNode4,
            treeNode5,
            treeNode6});
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Окно отладки");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Точки останова и пошаговое выполнение");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Отладка", new System.Windows.Forms.TreeNode[] {
            treeNode8,
            treeNode9});
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView = new System.Windows.Forms.TreeView();
            this.browser = new System.Windows.Forms.WebBrowser();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.browser);
            this.splitContainer1.Size = new System.Drawing.Size(511, 346);
            this.splitContainer1.SplitterDistance = 99;
            this.splitContainer1.TabIndex = 1;
            // 
            // treeView
            // 
            this.treeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView.Location = new System.Drawing.Point(0, 0);
            this.treeView.Margin = new System.Windows.Forms.Padding(0);
            this.treeView.Name = "treeView";
            treeNode1.Name = "createProjItem";
            treeNode1.Tag = "ProjectCreateInfo";
            treeNode1.Text = "Меню создания проекта";
            treeNode2.Name = "envItem";
            treeNode2.Tag = "WorkEnvironmentInfo";
            treeNode2.Text = "Рабочая среда";
            treeNode3.Name = "interfaceItem";
            treeNode3.Text = "Интерфейс";
            treeNode4.Name = "postMachineItem";
            treeNode4.Tag = "PostInfo";
            treeNode4.Text = "Машина Поста";
            treeNode5.Name = "turingMachineItem";
            treeNode5.Tag = "TuringInfo";
            treeNode5.Text = "Машина Тьюринга";
            treeNode6.Name = "regMachineItem";
            treeNode6.Tag = "RegisterInfo";
            treeNode6.Text = "Машина с бесконечными регистрами";
            treeNode7.Name = "machinesItem";
            treeNode7.Text = "Абстрактные вычислители";
            treeNode8.Name = "debugWindow";
            treeNode8.Tag = "DebugInfo";
            treeNode8.Text = "Окно отладки";
            treeNode9.Name = "breakPointItem";
            treeNode9.Tag = "BreakPointInfo";
            treeNode9.Text = "Точки останова и пошаговое выполнение";
            treeNode10.Name = "debugItem";
            treeNode10.Text = "Отладка";
            this.treeView.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode3,
            treeNode7,
            treeNode10});
            this.treeView.Size = new System.Drawing.Size(99, 346);
            this.treeView.TabIndex = 1;
            this.treeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.NodeClickHandler);
            // 
            // browser
            // 
            this.browser.Dock = System.Windows.Forms.DockStyle.Fill;
            this.browser.Location = new System.Drawing.Point(0, 0);
            this.browser.MinimumSize = new System.Drawing.Size(20, 20);
            this.browser.Name = "browser";
            this.browser.Size = new System.Drawing.Size(408, 346);
            this.browser.TabIndex = 2;
            this.browser.WebBrowserShortcutsEnabled = false;
            // 
            // HelpWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 346);
            this.Controls.Add(this.splitContainer1);
            this.Name = "HelpWindow";
            this.ShowIcon = false;
            this.Text = "Помощь";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.WebBrowser browser;
    }
}