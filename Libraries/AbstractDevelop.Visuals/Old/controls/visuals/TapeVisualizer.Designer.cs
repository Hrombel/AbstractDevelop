﻿namespace AbstractDevelop.controls.visuals
{
    partial class TapeVisualizer
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
            this.SuspendLayout();
            // 
            // TapeVisualizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(10, 10);
            this.Name = "TapeVisualizer";
            this.Size = new System.Drawing.Size(310, 81);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.TapeVisualizer_Paint);
            this.ResumeLayout(false);

        }

        #endregion
    }
}