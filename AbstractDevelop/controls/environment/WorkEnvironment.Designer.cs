namespace AbstractDevelop.controls.environment
{
    partial class WorkEnvironment
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
            this.visualsCtrl = new AbstractDevelop.controls.environment.tabcontrol.VisualsControl();
            this.debug = new AbstractDevelop.controls.environment.debugwindow.DebugWindow();
            this.stateControl1 = new AbstractDevelop.controls.ui.statecontrol.StateControl();
            this.toolPanel = new AbstractDevelop.controls.environment.toolpanel.ToolPanel();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 33);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.visualsCtrl);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.debug);
            this.splitContainer1.Size = new System.Drawing.Size(424, 309);
            this.splitContainer1.SplitterDistance = 232;
            this.splitContainer1.TabIndex = 5;
            // 
            // visualsCtrl
            // 
            this.visualsCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.visualsCtrl.Location = new System.Drawing.Point(0, 0);
            this.visualsCtrl.Name = "visualsCtrl";
            this.visualsCtrl.Size = new System.Drawing.Size(424, 232);
            this.visualsCtrl.TabIndex = 6;
            // 
            // debug
            // 
            this.debug.Dock = System.Windows.Forms.DockStyle.Fill;
            this.debug.Location = new System.Drawing.Point(0, 0);
            this.debug.Name = "debug";
            this.debug.Size = new System.Drawing.Size(424, 73);
            this.debug.TabIndex = 0;
            // 
            // stateControl1
            // 
            this.stateControl1.CurrentControl = null;
            this.stateControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stateControl1.Location = new System.Drawing.Point(0, 33);
            this.stateControl1.Name = "stateControl1";
            this.stateControl1.Size = new System.Drawing.Size(424, 309);
            this.stateControl1.TabIndex = 4;
            // 
            // toolPanel
            // 
            this.toolPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.toolPanel.Location = new System.Drawing.Point(0, 0);
            this.toolPanel.Name = "toolPanel";
            this.toolPanel.Size = new System.Drawing.Size(424, 33);
            this.toolPanel.TabIndex = 1;
            // 
            // WorkEnvironment
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.stateControl1);
            this.Controls.Add(this.toolPanel);
            this.Name = "WorkEnvironment";
            this.Size = new System.Drawing.Size(424, 342);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private toolpanel.ToolPanel toolPanel;
        private ui.statecontrol.StateControl stateControl1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private tabcontrol.VisualsControl visualsCtrl;
        private debugwindow.DebugWindow debug;
    }
}
