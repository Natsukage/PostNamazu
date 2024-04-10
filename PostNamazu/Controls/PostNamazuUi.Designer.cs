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
            this.components = new System.ComponentModel.Container();
            this.CheckAutoStart = new System.Windows.Forms.CheckBox();
            this.ButtonStop = new System.Windows.Forms.Button();
            this.ButtonStart = new System.Windows.Forms.Button();
            this.TextPort = new System.Windows.Forms.NumericUpDown();
            this.lbPort = new System.Windows.Forms.Label();
            this.mainGroupBox = new System.Windows.Forms.GroupBox();
            this.lstMessages = new System.Windows.Forms.ListBox();
            this.flowLayoutActions = new System.Windows.Forms.FlowLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.ButtonClearMessage = new System.Windows.Forms.Button();
            this.ButtonCopySelection = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.ButtonCopyProblematic = new System.Windows.Forms.Button();
            this.logTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.TextPort)).BeginInit();
            this.mainGroupBox.SuspendLayout();
            this.flowLayoutActions.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // CheckAutoStart
            // 
            this.CheckAutoStart.AutoSize = true;
            this.CheckAutoStart.Location = new System.Drawing.Point(7, 60);
            this.CheckAutoStart.Margin = new System.Windows.Forms.Padding(2);
            this.CheckAutoStart.Name = "CheckAutoStart";
            this.CheckAutoStart.Size = new System.Drawing.Size(120, 16);
            this.CheckAutoStart.TabIndex = 4;
            this.CheckAutoStart.Text = "自动启动HTTP监听";
            this.CheckAutoStart.UseVisualStyleBackColor = true;
            // 
            // ButtonStop
            // 
            this.ButtonStop.Enabled = false;
            this.ButtonStop.Location = new System.Drawing.Point(67, 35);
            this.ButtonStop.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonStop.Name = "ButtonStop";
            this.ButtonStop.Size = new System.Drawing.Size(54, 22);
            this.ButtonStop.TabIndex = 3;
            this.ButtonStop.Text = "停止";
            this.ButtonStop.UseVisualStyleBackColor = true;
            // 
            // ButtonStart
            // 
            this.ButtonStart.Location = new System.Drawing.Point(5, 35);
            this.ButtonStart.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonStart.Name = "ButtonStart";
            this.ButtonStart.Size = new System.Drawing.Size(54, 22);
            this.ButtonStart.TabIndex = 2;
            this.ButtonStart.Text = "启动";
            this.ButtonStart.UseVisualStyleBackColor = true;
            // 
            // TextPort
            // 
            this.TextPort.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.TextPort.Location = new System.Drawing.Point(63, 2);
            this.TextPort.Margin = new System.Windows.Forms.Padding(2);
            this.TextPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.TextPort.Name = "TextPort";
            this.TextPort.Size = new System.Drawing.Size(51, 21);
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
            this.lbPort.Location = new System.Drawing.Point(2, 5);
            this.lbPort.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbPort.Name = "lbPort";
            this.lbPort.Size = new System.Drawing.Size(53, 12);
            this.lbPort.TabIndex = 0;
            this.lbPort.Text = "HTTP端口";
            // 
            // mainGroupBox
            // 
            this.mainGroupBox.Controls.Add(this.lstMessages);
            this.mainGroupBox.Controls.Add(this.ButtonStart);
            this.mainGroupBox.Controls.Add(this.flowLayoutActions);
            this.mainGroupBox.Controls.Add(this.ButtonStop);
            this.mainGroupBox.Controls.Add(this.ButtonClearMessage);
            this.mainGroupBox.Controls.Add(this.ButtonCopySelection);
            this.mainGroupBox.Controls.Add(this.panel2);
            this.mainGroupBox.Controls.Add(this.ButtonCopyProblematic);
            this.mainGroupBox.Controls.Add(this.CheckAutoStart);
            this.mainGroupBox.Location = new System.Drawing.Point(13, 9);
            this.mainGroupBox.Margin = new System.Windows.Forms.Padding(2);
            this.mainGroupBox.Name = "mainGroupBox";
            this.mainGroupBox.Padding = new System.Windows.Forms.Padding(2);
            this.mainGroupBox.Size = new System.Drawing.Size(597, 352);
            this.mainGroupBox.TabIndex = 1;
            this.mainGroupBox.TabStop = false;
            this.mainGroupBox.Text = "鲶鱼精邮差";
            // 
            // lstMessages
            // 
            this.lstMessages.FormattingEnabled = true;
            this.lstMessages.HorizontalScrollbar = true;
            this.lstMessages.ItemHeight = 12;
            this.lstMessages.Location = new System.Drawing.Point(130, 15);
            this.lstMessages.Margin = new System.Windows.Forms.Padding(2);
            this.lstMessages.Name = "lstMessages";
            this.lstMessages.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstMessages.Size = new System.Drawing.Size(462, 328);
            this.lstMessages.TabIndex = 5;
            this.lstMessages.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lstMessages_MouseMove);
            // 
            // flowLayoutActions
            // 
            this.flowLayoutActions.AutoScroll = true;
            this.flowLayoutActions.Controls.Add(this.label1);
            this.flowLayoutActions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutActions.Location = new System.Drawing.Point(4, 79);
            this.flowLayoutActions.Margin = new System.Windows.Forms.Padding(2);
            this.flowLayoutActions.Name = "flowLayoutActions";
            this.flowLayoutActions.Size = new System.Drawing.Size(117, 174);
            this.flowLayoutActions.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(2, 0);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "启用以下动作";
            // 
            // ButtonClearMessage
            // 
            this.ButtonClearMessage.Location = new System.Drawing.Point(5, 323);
            this.ButtonClearMessage.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonClearMessage.Name = "ButtonClearMessage";
            this.ButtonClearMessage.Size = new System.Drawing.Size(116, 22);
            this.ButtonClearMessage.TabIndex = 6;
            this.ButtonClearMessage.Text = "清空日志";
            this.ButtonClearMessage.UseVisualStyleBackColor = true;
            this.ButtonClearMessage.Click += new System.EventHandler(this.cmdClearMessages_Click);
            // 
            // ButtonCopySelection
            // 
            this.ButtonCopySelection.Location = new System.Drawing.Point(5, 276);
            this.ButtonCopySelection.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonCopySelection.Name = "ButtonCopySelection";
            this.ButtonCopySelection.Size = new System.Drawing.Size(116, 22);
            this.ButtonCopySelection.TabIndex = 8;
            this.ButtonCopySelection.Text = "复制所选日志";
            this.ButtonCopySelection.UseVisualStyleBackColor = true;
            this.ButtonCopySelection.Click += new System.EventHandler(this.cmdCopySelection_Click);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.lbPort);
            this.panel2.Controls.Add(this.TextPort);
            this.panel2.Location = new System.Drawing.Point(5, 13);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(119, 26);
            this.panel2.TabIndex = 11;
            // 
            // ButtonCopyProblematic
            // 
            this.ButtonCopyProblematic.Location = new System.Drawing.Point(5, 300);
            this.ButtonCopyProblematic.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonCopyProblematic.Name = "ButtonCopyProblematic";
            this.ButtonCopyProblematic.Size = new System.Drawing.Size(116, 22);
            this.ButtonCopyProblematic.TabIndex = 7;
            this.ButtonCopyProblematic.Text = "复制全部日志";
            this.ButtonCopyProblematic.UseVisualStyleBackColor = true;
            this.ButtonCopyProblematic.Click += new System.EventHandler(this.cmdCopyProblematic_Click);
            // 
            // logTip
            // 
            this.logTip.AutoPopDelay = 32767;
            this.logTip.InitialDelay = 200;
            this.logTip.ReshowDelay = 200;
            this.logTip.ShowAlways = true;
            // 
            // PostNamazuUi
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mainGroupBox);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "PostNamazuUi";
            this.Size = new System.Drawing.Size(625, 373);
            ((System.ComponentModel.ISupportInitialize)(this.TextPort)).EndInit();
            this.mainGroupBox.ResumeLayout(false);
            this.mainGroupBox.PerformLayout();
            this.flowLayoutActions.ResumeLayout(false);
            this.flowLayoutActions.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.CheckBox CheckAutoStart;
        public System.Windows.Forms.Button ButtonStop;
        public System.Windows.Forms.Button ButtonStart;
        public System.Windows.Forms.NumericUpDown TextPort;
        public System.Windows.Forms.Label lbPort;
        public System.Windows.Forms.GroupBox mainGroupBox;
        public System.Windows.Forms.Button ButtonCopySelection;
        public System.Windows.Forms.Button ButtonCopyProblematic;
        public System.Windows.Forms.Button ButtonClearMessage;
        private System.Windows.Forms.ToolTip logTip;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutActions;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        public System.Windows.Forms.ListBox lstMessages;
    }
}
