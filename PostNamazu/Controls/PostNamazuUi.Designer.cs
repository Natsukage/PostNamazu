namespace PostNamazu
{
    partial class PostNamazuUi
    {
        /// <summary> 
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary> 
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        public void InitializeComponent()
        {
            this.CheckAutoStart = new System.Windows.Forms.CheckBox();
            this.ButtonStop = new System.Windows.Forms.Button();
            this.ButtonStart = new System.Windows.Forms.Button();
            this.TextPort = new System.Windows.Forms.NumericUpDown();
            this.lbPort = new System.Windows.Forms.Label();
            this.mainGroupBox = new System.Windows.Forms.GroupBox();
            this.ButtonCopyProblematic = new System.Windows.Forms.Button();
            this.ButtonClearMessage = new System.Windows.Forms.Button();
            this.lstMessages = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.TextPort)).BeginInit();
            this.mainGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // CheckAutoStart
            // 
            this.CheckAutoStart.AutoSize = true;
            this.CheckAutoStart.Location = new System.Drawing.Point(453, 45);
            this.CheckAutoStart.Name = "CheckAutoStart";
            this.CheckAutoStart.Size = new System.Drawing.Size(138, 28);
            this.CheckAutoStart.TabIndex = 4;
            this.CheckAutoStart.Text = "自动启动";
            this.CheckAutoStart.UseVisualStyleBackColor = true;
            // 
            // ButtonStop
            // 
            this.ButtonStop.Enabled = false;
            this.ButtonStop.Location = new System.Drawing.Point(330, 43);
            this.ButtonStop.Name = "ButtonStop";
            this.ButtonStop.Size = new System.Drawing.Size(117, 35);
            this.ButtonStop.TabIndex = 3;
            this.ButtonStop.Text = "停止";
            this.ButtonStop.UseVisualStyleBackColor = true;
            // 
            // ButtonStart
            // 
            this.ButtonStart.Location = new System.Drawing.Point(207, 43);
            this.ButtonStart.Name = "ButtonStart";
            this.ButtonStart.Size = new System.Drawing.Size(117, 35);
            this.ButtonStart.TabIndex = 2;
            this.ButtonStart.Text = "启动";
            this.ButtonStart.UseVisualStyleBackColor = true;
            // 
            // TextPort
            // 
            this.TextPort.Location = new System.Drawing.Point(81, 43);
            this.TextPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.TextPort.Name = "TextPort";
            this.TextPort.Size = new System.Drawing.Size(120, 35);
            this.TextPort.TabIndex = 1;
            // 
            // lbPort
            // 
            this.lbPort.AutoSize = true;
            this.lbPort.Location = new System.Drawing.Point(17, 45);
            this.lbPort.Name = "lbPort";
            this.lbPort.Size = new System.Drawing.Size(58, 24);
            this.lbPort.TabIndex = 0;
            this.lbPort.Text = "端口";
            // 
            // mainGroupBox
            // 
            this.mainGroupBox.Controls.Add(this.ButtonCopyProblematic);
            this.mainGroupBox.Controls.Add(this.ButtonClearMessage);
            this.mainGroupBox.Controls.Add(this.lstMessages);
            this.mainGroupBox.Controls.Add(this.CheckAutoStart);
            this.mainGroupBox.Controls.Add(this.ButtonStop);
            this.mainGroupBox.Controls.Add(this.ButtonStart);
            this.mainGroupBox.Controls.Add(this.TextPort);
            this.mainGroupBox.Controls.Add(this.lbPort);
            this.mainGroupBox.Location = new System.Drawing.Point(26, 18);
            this.mainGroupBox.Name = "mainGroupBox";
            this.mainGroupBox.Size = new System.Drawing.Size(614, 742);
            this.mainGroupBox.TabIndex = 1;
            this.mainGroupBox.TabStop = false;
            this.mainGroupBox.Text = "鲶鱼精邮差";
            // 
            // ButtonCopyProblematic
            // 
            this.ButtonCopyProblematic.Location = new System.Drawing.Point(276, 670);
            this.ButtonCopyProblematic.Name = "ButtonCopyProblematic";
            this.ButtonCopyProblematic.Size = new System.Drawing.Size(201, 44);
            this.ButtonCopyProblematic.TabIndex = 7;
            this.ButtonCopyProblematic.Text = "复制到剪贴板";
            this.ButtonCopyProblematic.UseVisualStyleBackColor = true;
            // 
            // ButtonClearMessage
            // 
            this.ButtonClearMessage.Location = new System.Drawing.Point(21, 670);
            this.ButtonClearMessage.Name = "ButtonClearMessage";
            this.ButtonClearMessage.Size = new System.Drawing.Size(201, 44);
            this.ButtonClearMessage.TabIndex = 6;
            this.ButtonClearMessage.Text = "清空日志";
            this.ButtonClearMessage.UseVisualStyleBackColor = true;
            // 
            // lstMessages
            // 
            this.lstMessages.FormattingEnabled = true;
            this.lstMessages.ItemHeight = 24;
            this.lstMessages.Location = new System.Drawing.Point(21, 84);
            this.lstMessages.Name = "lstMessages";
            this.lstMessages.Size = new System.Drawing.Size(558, 580);
            this.lstMessages.TabIndex = 5;
            // 
            // UIControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainGroupBox);
            this.Name = "UIControl";
            this.Size = new System.Drawing.Size(660, 796);
            ((System.ComponentModel.ISupportInitialize)(this.TextPort)).EndInit();
            this.mainGroupBox.ResumeLayout(false);
            this.mainGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.CheckBox CheckAutoStart;
        public System.Windows.Forms.Button ButtonStop;
        public System.Windows.Forms.Button ButtonStart;
        public System.Windows.Forms.NumericUpDown TextPort;
        public System.Windows.Forms.Label lbPort;
        public System.Windows.Forms.GroupBox mainGroupBox;
        public System.Windows.Forms.Button ButtonCopyProblematic;
        public System.Windows.Forms.Button ButtonClearMessage;
        public System.Windows.Forms.ListBox lstMessages;
    }
}
