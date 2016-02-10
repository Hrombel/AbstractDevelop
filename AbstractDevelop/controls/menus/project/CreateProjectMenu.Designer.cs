namespace AbstractDevelop.controls.menus
{
    partial class CreateProjectMenu
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
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.itemsHolder = new System.Windows.Forms.Panel();
            this.infoHolder = new System.Windows.Forms.Panel();
            this.downToolsHolder = new System.Windows.Forms.Panel();
            this.newFolderBox = new System.Windows.Forms.CheckBox();
            this.nameLbl = new System.Windows.Forms.Label();
            this.nameBox = new System.Windows.Forms.TextBox();
            this.createBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.downToolsHolder.SuspendLayout();
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
            this.splitContainer1.Panel1.Controls.Add(this.itemsHolder);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.infoHolder);
            this.splitContainer1.Panel2.Controls.Add(this.downToolsHolder);
            this.splitContainer1.Panel2MinSize = 50;
            this.splitContainer1.Size = new System.Drawing.Size(442, 355);
            this.splitContainer1.SplitterDistance = 147;
            this.splitContainer1.TabIndex = 2;
            // 
            // itemsHolder
            // 
            this.itemsHolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itemsHolder.Location = new System.Drawing.Point(0, 0);
            this.itemsHolder.Margin = new System.Windows.Forms.Padding(0);
            this.itemsHolder.Name = "itemsHolder";
            this.itemsHolder.Size = new System.Drawing.Size(147, 355);
            this.itemsHolder.TabIndex = 2;
            // 
            // infoHolder
            // 
            this.infoHolder.Dock = System.Windows.Forms.DockStyle.Fill;
            this.infoHolder.Location = new System.Drawing.Point(0, 0);
            this.infoHolder.Name = "infoHolder";
            this.infoHolder.Size = new System.Drawing.Size(291, 325);
            this.infoHolder.TabIndex = 2;
            // 
            // downToolsHolder
            // 
            this.downToolsHolder.Controls.Add(this.newFolderBox);
            this.downToolsHolder.Controls.Add(this.nameLbl);
            this.downToolsHolder.Controls.Add(this.nameBox);
            this.downToolsHolder.Controls.Add(this.createBtn);
            this.downToolsHolder.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.downToolsHolder.Location = new System.Drawing.Point(0, 325);
            this.downToolsHolder.Margin = new System.Windows.Forms.Padding(0);
            this.downToolsHolder.Name = "downToolsHolder";
            this.downToolsHolder.Size = new System.Drawing.Size(291, 30);
            this.downToolsHolder.TabIndex = 1;
            // 
            // newFolderBox
            // 
            this.newFolderBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.newFolderBox.AutoSize = true;
            this.newFolderBox.Checked = true;
            this.newFolderBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.newFolderBox.Location = new System.Drawing.Point(126, 7);
            this.newFolderBox.Name = "newFolderBox";
            this.newFolderBox.Size = new System.Drawing.Size(99, 17);
            this.newFolderBox.TabIndex = 8;
            this.newFolderBox.Text = "В новой папке";
            this.newFolderBox.UseVisualStyleBackColor = true;
            // 
            // nameLbl
            // 
            this.nameLbl.AutoSize = true;
            this.nameLbl.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.nameLbl.Location = new System.Drawing.Point(3, 8);
            this.nameLbl.Name = "nameLbl";
            this.nameLbl.Size = new System.Drawing.Size(60, 13);
            this.nameLbl.TabIndex = 10;
            this.nameLbl.Text = "Название:";
            // 
            // nameBox
            // 
            this.nameBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.nameBox.Location = new System.Drawing.Point(66, 5);
            this.nameBox.Name = "nameBox";
            this.nameBox.Size = new System.Drawing.Size(58, 20);
            this.nameBox.TabIndex = 7;
            this.nameBox.TextChanged += new System.EventHandler(this.nameBox_TextChanged);
            // 
            // createBtn
            // 
            this.createBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.createBtn.Location = new System.Drawing.Point(228, 3);
            this.createBtn.Name = "createBtn";
            this.createBtn.Size = new System.Drawing.Size(60, 23);
            this.createBtn.TabIndex = 9;
            this.createBtn.Text = "Создать проект";
            this.createBtn.UseVisualStyleBackColor = true;
            this.createBtn.Click += new System.EventHandler(this.createBtn_Click);
            // 
            // CreateProjectMenu
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.DimGray;
            this.Controls.Add(this.splitContainer1);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.Name = "CreateProjectMenu";
            this.Size = new System.Drawing.Size(442, 355);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.downToolsHolder.ResumeLayout(false);
            this.downToolsHolder.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Panel itemsHolder;
        private System.Windows.Forms.Button createBtn;
        private System.Windows.Forms.TextBox nameBox;
        private System.Windows.Forms.Label nameLbl;
        private System.Windows.Forms.CheckBox newFolderBox;
        private System.Windows.Forms.Panel downToolsHolder;
        private System.Windows.Forms.Panel infoHolder;
    }
}
