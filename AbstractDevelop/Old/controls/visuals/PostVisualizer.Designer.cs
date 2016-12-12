namespace AbstractDevelop.controls.visuals
{
    partial class PostVisualizer
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
            this.tapeVisualizer = new AbstractDevelop.controls.visuals.TapeVisualizer();
            this.codeBox = new AbstractDevelop.controls.ui.ceditor.editors.PostCodeEditor();
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
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tapeVisualizer);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.codeBox);
            this.splitContainer1.Size = new System.Drawing.Size(442, 315);
            this.splitContainer1.SplitterDistance = 74;
            this.splitContainer1.TabIndex = 3;
            // 
            // tapeVisualizer
            // 
            this.tapeVisualizer.CurrentTape = null;
            this.tapeVisualizer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tapeVisualizer.ExternalSymbolSet = null;
            this.tapeVisualizer.InputMode = true;
            this.tapeVisualizer.Location = new System.Drawing.Point(0, 0);
            this.tapeVisualizer.MinimumSize = new System.Drawing.Size(10, 10);
            this.tapeVisualizer.Name = "tapeVisualizer";
            this.tapeVisualizer.Size = new System.Drawing.Size(442, 74);
            this.tapeVisualizer.TabIndex = 0;
            // 
            // codeBox
            // 
            this.codeBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.codeBox.Location = new System.Drawing.Point(0, 0);
            this.codeBox.Name = "codeBox";
            this.codeBox.Size = new System.Drawing.Size(442, 237);
            this.codeBox.TabIndex = 0;
            // 
            // PostVisualizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "PostVisualizer";
            this.Size = new System.Drawing.Size(442, 315);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private TapeVisualizer tapeVisualizer;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private ui.ceditor.editors.PostCodeEditor codeBox;


    }
}
