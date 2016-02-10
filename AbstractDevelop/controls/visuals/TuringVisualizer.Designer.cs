namespace AbstractDevelop.controls.visuals
{
    partial class TuringVisualizer
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
            this.tapeBox = new System.Windows.Forms.GroupBox();
            this.tapeContainer = new System.Windows.Forms.Panel();
            this.tapePanel = new System.Windows.Forms.Panel();
            this.addTapeBtn = new System.Windows.Forms.Button();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.turingInfo = new AbstractDevelop.controls.visuals.additionals.TuringInfo();
            this.codeBox = new AbstractDevelop.controls.ui.ceditor.editors.TuringCodeEditor();
            this.tapeBox.SuspendLayout();
            this.tapeContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tapeBox
            // 
            this.tapeBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tapeBox.Controls.Add(this.tapeContainer);
            this.tapeBox.Location = new System.Drawing.Point(0, 59);
            this.tapeBox.Name = "tapeBox";
            this.tapeBox.Size = new System.Drawing.Size(526, 178);
            this.tapeBox.TabIndex = 0;
            this.tapeBox.TabStop = false;
            this.tapeBox.Text = "Ленты";
            // 
            // tapeContainer
            // 
            this.tapeContainer.AutoScroll = true;
            this.tapeContainer.Controls.Add(this.tapePanel);
            this.tapeContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tapeContainer.Location = new System.Drawing.Point(3, 16);
            this.tapeContainer.Name = "tapeContainer";
            this.tapeContainer.Size = new System.Drawing.Size(520, 159);
            this.tapeContainer.TabIndex = 1;
            // 
            // tapePanel
            // 
            this.tapePanel.AutoScroll = true;
            this.tapePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tapePanel.Location = new System.Drawing.Point(0, 0);
            this.tapePanel.Name = "tapePanel";
            this.tapePanel.Size = new System.Drawing.Size(520, 159);
            this.tapePanel.TabIndex = 0;
            // 
            // addTapeBtn
            // 
            this.addTapeBtn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.addTapeBtn.Location = new System.Drawing.Point(507, 3);
            this.addTapeBtn.Name = "addTapeBtn";
            this.addTapeBtn.Size = new System.Drawing.Size(25, 24);
            this.addTapeBtn.TabIndex = 1;
            this.addTapeBtn.Text = "+";
            this.addTapeBtn.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(3, 33);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.turingInfo);
            this.splitContainer1.Panel1.Controls.Add(this.tapeBox);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.codeBox);
            this.splitContainer1.Size = new System.Drawing.Size(526, 388);
            this.splitContainer1.SplitterDistance = 237;
            this.splitContainer1.TabIndex = 2;
            // 
            // turingInfo
            // 
            this.turingInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.turingInfo.Location = new System.Drawing.Point(3, 3);
            this.turingInfo.Margin = new System.Windows.Forms.Padding(0);
            this.turingInfo.Name = "turingInfo";
            this.turingInfo.Size = new System.Drawing.Size(520, 53);
            this.turingInfo.TabIndex = 1;
            // 
            // codeBox
            // 
            this.codeBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeBox.Location = new System.Drawing.Point(0, 0);
            this.codeBox.Name = "codeBox";
            this.codeBox.ReadOnly = false;
            this.codeBox.Size = new System.Drawing.Size(526, 147);
            this.codeBox.TabIndex = 0;
            // 
            // TuringVisualizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.addTapeBtn);
            this.Name = "TuringVisualizer";
            this.Size = new System.Drawing.Size(532, 424);
            this.tapeBox.ResumeLayout(false);
            this.tapeContainer.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox tapeBox;
        private System.Windows.Forms.Panel tapeContainer;
        private System.Windows.Forms.Button addTapeBtn;
        private System.Windows.Forms.Panel tapePanel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private ui.ceditor.editors.TuringCodeEditor codeBox;
        private additionals.TuringInfo turingInfo;
    }
}
