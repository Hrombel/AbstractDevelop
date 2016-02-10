namespace AbstractDevelop.controls.visuals.additionals
{
    partial class TapeToolUnit
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
            this.tapeVis = new AbstractDevelop.controls.visuals.TapeVisualizer();
            this.SuspendLayout();
            // 
            // tapeVis
            // 
            this.tapeVis.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tapeVis.CurrentTape = null;
            this.tapeVis.ExternalSymbolSet = null;
            this.tapeVis.InputMode = true;
            this.tapeVis.Location = new System.Drawing.Point(0, 0);
            this.tapeVis.MinimumSize = new System.Drawing.Size(10, 10);
            this.tapeVis.Name = "tapeVis";
            this.tapeVis.Size = new System.Drawing.Size(450, 75);
            this.tapeVis.TabIndex = 0;
            // 
            // TapeToolUnit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tapeVis);
            this.Name = "TapeToolUnit";
            this.Size = new System.Drawing.Size(478, 75);
            this.ResumeLayout(false);

        }

        #endregion

        private TapeVisualizer tapeVis;
    }
}
