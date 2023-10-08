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
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutActions = new System.Windows.Forms.FlowLayoutPanel();
            this.ButtonCopyProblematic = new System.Windows.Forms.Button();
            this.ButtonClearMessage = new System.Windows.Forms.Button();
            this.lstMessages = new System.Windows.Forms.ListBox();
            ((System.ComponentModel.ISupportInitialize)(this.TextPort)).BeginInit();
            this.mainGroupBox.SuspendLayout();
            this.flowLayoutActions.SuspendLayout();
            this.SuspendLayout();
            // 
            // CheckAutoStart
            // 
            this.CheckAutoStart.AutoSize = true;
            this.CheckAutoStart.Location = new System.Drawing.Point(19, 111);
            this.CheckAutoStart.Name = "CheckAutoStart";
            this.CheckAutoStart.Size = new System.Drawing.Size(234, 28);
            this.CheckAutoStart.TabIndex = 4;
            this.CheckAutoStart.Text = "自动启动HTTP监听";
            this.CheckAutoStart.UseVisualStyleBackColor = true;
            // 
            // ButtonStop
            // 
            this.ButtonStop.Enabled = false;
            this.ButtonStop.Location = new System.Drawing.Point(140, 70);
            this.ButtonStop.Name = "ButtonStop";
            this.ButtonStop.Size = new System.Drawing.Size(113, 35);
            this.ButtonStop.TabIndex = 3;
            this.ButtonStop.Text = "停止";
            this.ButtonStop.UseVisualStyleBackColor = true;
            // 
            // ButtonStart
            // 
            this.ButtonStart.Location = new System.Drawing.Point(19, 70);
            this.ButtonStart.Name = "ButtonStart";
            this.ButtonStart.Size = new System.Drawing.Size(113, 35);
            this.ButtonStart.TabIndex = 2;
            this.ButtonStart.Text = "启动";
            this.ButtonStart.UseVisualStyleBackColor = true;
            // 
            // TextPort
            // 
            this.TextPort.Location = new System.Drawing.Point(121, 29);
            this.TextPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.TextPort.Name = "TextPort";
            this.TextPort.Size = new System.Drawing.Size(132, 35);
            this.TextPort.TabIndex = 1;
            this.TextPort.Value = new decimal(new int[] {
            2019,
            0,
            0,
            0});
            // 
            // lbPort
            // 
            this.lbPort.AutoSize = true;
            this.lbPort.Location = new System.Drawing.Point(15, 34);
            this.lbPort.Name = "lbPort";
            this.lbPort.Size = new System.Drawing.Size(106, 24);
            this.lbPort.TabIndex = 0;
            this.lbPort.Text = "HTTP端口";
            // 
            // mainGroupBox
            // 
            this.mainGroupBox.Controls.Add(this.CheckAutoStart);
            this.mainGroupBox.Controls.Add(this.flowLayoutActions);
            this.mainGroupBox.Controls.Add(this.ButtonCopyProblematic);
            this.mainGroupBox.Controls.Add(this.ButtonClearMessage);
            this.mainGroupBox.Controls.Add(this.lstMessages);
            this.mainGroupBox.Controls.Add(this.ButtonStop);
            this.mainGroupBox.Controls.Add(this.ButtonStart);
            this.mainGroupBox.Controls.Add(this.TextPort);
            this.mainGroupBox.Controls.Add(this.lbPort);
            this.mainGroupBox.Location = new System.Drawing.Point(26, 18);
            this.mainGroupBox.Name = "mainGroupBox";
            this.mainGroupBox.Size = new System.Drawing.Size(764, 630);
            this.mainGroupBox.TabIndex = 1;
            this.mainGroupBox.TabStop = false;
            this.mainGroupBox.Text = "鲶鱼精邮差";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(154, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "启用以下动作";
            // 
            // flowLayoutActions
            // 
            this.flowLayoutActions.AutoScroll = true;
            this.flowLayoutActions.Controls.Add(this.label1);
            this.flowLayoutActions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutActions.Location = new System.Drawing.Point(19, 145);
            this.flowLayoutActions.Name = "flowLayoutActions";
            this.flowLayoutActions.Size = new System.Drawing.Size(234, 385);
            this.flowLayoutActions.TabIndex = 2;
            // 
            // ButtonCopyProblematic
            // 
            this.ButtonCopyProblematic.Location = new System.Drawing.Point(19, 536);
            this.ButtonCopyProblematic.Name = "ButtonCopyProblematic";
            this.ButtonCopyProblematic.Size = new System.Drawing.Size(234, 35);
            this.ButtonCopyProblematic.TabIndex = 7;
            this.ButtonCopyProblematic.Text = "复制到剪贴板";
            this.ButtonCopyProblematic.UseVisualStyleBackColor = true;
            this.ButtonCopyProblematic.Click += new System.EventHandler(this.cmdCopyProblematic_Click);
            // 
            // ButtonClearMessage
            // 
            this.ButtonClearMessage.Location = new System.Drawing.Point(21, 577);
            this.ButtonClearMessage.Name = "ButtonClearMessage";
            this.ButtonClearMessage.Size = new System.Drawing.Size(232, 32);
            this.ButtonClearMessage.TabIndex = 6;
            this.ButtonClearMessage.Text = "清空日志";
            this.ButtonClearMessage.UseVisualStyleBackColor = true;
            this.ButtonClearMessage.Click += new System.EventHandler(this.cmdClearMessages_Click);
            // 
            // lstMessages
            // 
            this.lstMessages.FormattingEnabled = true;
            this.lstMessages.ItemHeight = 24;
            this.lstMessages.Location = new System.Drawing.Point(259, 29);
            this.lstMessages.Name = "lstMessages";
            this.lstMessages.Size = new System.Drawing.Size(485, 580);
            this.lstMessages.TabIndex = 5;
            this.lstMessages.HorizontalScrollbar = true;
            this.lstMessages.HorizontalExtent = 0;

            // 
            // PostNamazuUi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainGroupBox);
            this.Name = "PostNamazuUi";
            this.Size = new System.Drawing.Size(816, 662);
            ((System.ComponentModel.ISupportInitialize)(this.TextPort)).EndInit();
            this.mainGroupBox.ResumeLayout(false);
            this.mainGroupBox.PerformLayout();
            this.flowLayoutActions.ResumeLayout(false);
            this.flowLayoutActions.PerformLayout();
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
        private System.Windows.Forms.FlowLayoutPanel flowLayoutActions;
        private System.Windows.Forms.Label label1;
    }
}
