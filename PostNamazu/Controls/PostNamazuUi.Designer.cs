﻿namespace PostNamazu
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
            this.mainPanel = new System.Windows.Forms.Panel();
            this.mainTable = new System.Windows.Forms.TableLayoutPanel();
            this.leftTable = new System.Windows.Forms.TableLayoutPanel();
            this.rightTable = new System.Windows.Forms.TableLayoutPanel();
            this.tableHttp = new System.Windows.Forms.TableLayoutPanel();
            this.lstMessages = new System.Windows.Forms.ListBox();
            this.flowLayoutActions = new System.Windows.Forms.FlowLayoutPanel();
            this.ButtonClearMessage = new System.Windows.Forms.Button();
            this.ButtonCopySelection = new System.Windows.Forms.Button();
            this.ButtonCopyProblematic = new System.Windows.Forms.Button();
            this.grpHttp = new System.Windows.Forms.GroupBox();
            this.grpEnabledCmd = new System.Windows.Forms.GroupBox();
            this.grpLang = new System.Windows.Forms.GroupBox();
            this.radioButtonEN = new System.Windows.Forms.RadioButton();
            this.radioButtonCN = new System.Windows.Forms.RadioButton();
            this.grpWaymarks = new System.Windows.Forms.GroupBox();
            this.tableWaymarks = new System.Windows.Forms.TableLayoutPanel();
            this.btnWaymarksImport = new System.Windows.Forms.Button();
            this.btnWaymarksExport = new System.Windows.Forms.Button();
            this.logTip = new System.Windows.Forms.ToolTip(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.TextPort)).BeginInit();
            this.mainGroupBox.SuspendLayout();
            this.mainTable.SuspendLayout();
            this.tableHttp.SuspendLayout();
            this.grpHttp.SuspendLayout();
            this.grpEnabledCmd.SuspendLayout();
            this.grpLang.SuspendLayout();
            this.tableWaymarks.SuspendLayout();
            this.grpWaymarks.SuspendLayout();
            this.flowLayoutActions.SuspendLayout();
            this.mainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainPanel
            // 
            this.mainPanel.Controls.Add(this.mainGroupBox);
            this.mainPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainPanel.Margin = new System.Windows.Forms.Padding(10);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Padding = new System.Windows.Forms.Padding(10);
            // 
            // mainGroupBox
            // 
            this.mainGroupBox.Controls.Add(this.mainTable);
            this.mainGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainGroupBox.Margin = new System.Windows.Forms.Padding(10);
            this.mainGroupBox.Name = "mainGroupBox";
            this.mainGroupBox.Padding = new System.Windows.Forms.Padding(10);            
            this.mainGroupBox.Text = "鲶鱼精邮差";
            // 
            // mainTable
            // 
            this.mainTable.AutoSize = true;
            this.mainTable.ColumnCount = 2;
            this.mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTable.Controls.Add(this.leftTable, 0, 0);
            this.mainTable.Controls.Add(this.rightTable, 1, 0);
            this.mainTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTable.Name = "mainTable";
            this.mainTable.RowCount = 1;
            this.mainTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            // 
            // leftTable
            // 
            this.leftTable.AutoSize = true;
            this.leftTable.ColumnCount = 1;
            this.leftTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.leftTable.Controls.Add(this.grpHttp, 0, 0);
            this.leftTable.Controls.Add(this.grpEnabledCmd, 0, 1);
            this.leftTable.Controls.Add(this.grpLang, 0, 2);
            this.leftTable.Controls.Add(this.grpWaymarks, 0, 3);
            this.leftTable.Dock = System.Windows.Forms.DockStyle.Left;
            this.leftTable.Name = "leftTable";
            this.leftTable.RowCount = 4;
            this.leftTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.leftTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.leftTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.leftTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            // 
            // rightTable
            // 
            this.rightTable.AutoSize = true;
            this.rightTable.ColumnCount = 3;
            this.rightTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.rightTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.rightTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33F));
            this.rightTable.Controls.Add(this.lstMessages, 0, 0);
            this.rightTable.Controls.Add(this.ButtonCopySelection, 0, 1);
            this.rightTable.Controls.Add(this.ButtonCopyProblematic, 1, 1);
            this.rightTable.Controls.Add(this.ButtonClearMessage, 2, 1);
            this.rightTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightTable.Name = "rightTable";
            this.rightTable.RowCount = 2;
            this.rightTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rightTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            // 
            // grpHttp
            // 
            this.grpHttp.AutoSize = true;
            this.grpHttp.Controls.Add(this.tableHttp);
            this.grpHttp.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpHttp.Margin = new System.Windows.Forms.Padding(5, 0, 5, 5);
            this.grpHttp.Name = "grpHttp";
            this.grpHttp.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.grpHttp.Text = "HTTP";
            // 
            // tableHttp
            // 
            this.tableHttp.AutoSize = true;
            this.tableHttp.ColumnCount = 2;
            this.tableHttp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableHttp.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableHttp.Controls.Add(this.CheckAutoStart, 0, 0);
            this.tableHttp.Controls.Add(this.lbPort, 0, 1);
            this.tableHttp.Controls.Add(this.TextPort, 1, 1);
            this.tableHttp.Controls.Add(this.ButtonStart, 0, 2);
            this.tableHttp.Controls.Add(this.ButtonStop, 1, 2);
            this.tableHttp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableHttp.Name = "tableHttp";
            this.tableHttp.RowCount = 3;
            this.tableHttp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tableHttp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.tableHttp.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            // 
            // lbPort
            // 
            this.lbPort.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbPort.AutoSize = true;
            this.lbPort.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lbPort.Name = "lbPort";
            this.lbPort.Text = "端口";
            // 
            // TextPort
            // 
            this.TextPort.AutoSize = true;
            this.TextPort.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.TextPort.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TextPort.Location = new System.Drawing.Point(63, 2);
            this.TextPort.Margin = new System.Windows.Forms.Padding(2);
            this.TextPort.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.TextPort.Name = "TextPort";
            this.TextPort.TabIndex = 1;
            this.TextPort.Value = new decimal(new int[] {
            2019,
            0,
            0,
            0});
            // 
            // ButtonStart
            // 
            this.ButtonStart.AutoSize = true;
            this.ButtonStart.Dock = System.Windows.Forms.DockStyle.Top;
            this.ButtonStart.Location = new System.Drawing.Point(5, 35);
            this.ButtonStart.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonStart.Name = "ButtonStart";
            this.ButtonStart.Padding = new System.Windows.Forms.Padding(0);
            this.ButtonStart.TabStop = false;
            this.ButtonStart.Text = "启动";
            this.ButtonStart.UseVisualStyleBackColor = true;
            // 
            // ButtonStop
            // 
            this.ButtonStop.AutoSize = true;
            this.ButtonStop.Dock = System.Windows.Forms.DockStyle.Top;
            this.ButtonStop.Enabled = false;
            this.ButtonStop.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonStop.Name = "ButtonStop";
            this.ButtonStop.Padding = new System.Windows.Forms.Padding(0);
            this.ButtonStop.TabStop = false;
            this.ButtonStop.Text = "停止";
            this.ButtonStop.UseVisualStyleBackColor = true;
            // 
            // CheckAutoStart
            // 
            this.tableHttp.SetColumnSpan(CheckAutoStart, 2);
            this.CheckAutoStart.AutoSize = true;
            this.CheckAutoStart.Location = new System.Drawing.Point(7, 60);
            this.CheckAutoStart.Margin = new System.Windows.Forms.Padding(2);
            this.CheckAutoStart.Name = "CheckAutoStart";
            this.CheckAutoStart.Size = new System.Drawing.Size(120, 16);
            this.CheckAutoStart.TabIndex = 4;
            this.CheckAutoStart.Text = "自动启动监听";
            this.CheckAutoStart.UseVisualStyleBackColor = true;
            // 
            // grpEnabledCmd
            // 
            this.grpEnabledCmd.AutoSize = true;
            this.grpEnabledCmd.Controls.Add(this.flowLayoutActions);
            this.grpEnabledCmd.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpEnabledCmd.Margin = new System.Windows.Forms.Padding(5);
            this.grpEnabledCmd.Name = "grpEnabledCmd";
            this.grpEnabledCmd.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.grpEnabledCmd.Text = "启用以下动作";
            // 
            // flowLayoutActions
            // 
            this.flowLayoutActions.AutoScroll = false;
            this.flowLayoutActions.AutoSize = true;
            this.flowLayoutActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutActions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutActions.Location = new System.Drawing.Point(4, 79);
            this.flowLayoutActions.Margin = new System.Windows.Forms.Padding(2);
            this.flowLayoutActions.Name = "flowLayoutActions";
            this.flowLayoutActions.Size = new System.Drawing.Size(117, 174);
            // 
            // grpLang
            // 
            this.grpLang.AutoSize = true;
            this.grpLang.Controls.Add(radioButtonCN);
            this.grpLang.Controls.Add(radioButtonEN);
            this.grpLang.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpLang.Margin = new System.Windows.Forms.Padding(5);
            this.grpLang.Name = "grpLang";
            this.grpLang.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.grpLang.Text = "语言";
            // 
            // radioButtonEN
            // 
            this.radioButtonEN.AutoSize = true;
            this.radioButtonEN.Dock = System.Windows.Forms.DockStyle.Top;
            this.radioButtonEN.Name = "radioButtonEN";
            this.radioButtonEN.TabStop = false;
            this.radioButtonEN.Text = " English";
            this.radioButtonEN.UseVisualStyleBackColor = true;
            this.radioButtonEN.CheckedChanged += new System.EventHandler(this.LanguageRadioButton_CheckedChanged);
            // 
            // radioButtonCN
            // 
            this.radioButtonCN.AutoSize = true;
            this.radioButtonCN.Dock = System.Windows.Forms.DockStyle.Top;
            this.radioButtonCN.Name = "radioButtonCN";
            this.radioButtonCN.TabStop = false;
            this.radioButtonCN.Text = " 中文";
            this.radioButtonCN.UseVisualStyleBackColor = true;
            this.radioButtonCN.CheckedChanged += new System.EventHandler(this.LanguageRadioButton_CheckedChanged);
            // 
            // grpWaymarks
            // 
            this.grpWaymarks.AutoSize = true;
            this.grpWaymarks.Controls.Add(tableWaymarks);
            this.grpWaymarks.Dock = System.Windows.Forms.DockStyle.Top;
            this.grpWaymarks.Margin = new System.Windows.Forms.Padding(5);
            this.grpWaymarks.Name = "grpWaymarks";
            this.grpWaymarks.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.grpWaymarks.Text = "场地标点";
            // 
            // tableWaymarks
            // 
            this.tableWaymarks.AutoSize = true;
            this.tableWaymarks.ColumnCount = 2;
            this.tableWaymarks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableWaymarks.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableWaymarks.Controls.Add(this.btnWaymarksImport, 0, 0);
            this.tableWaymarks.Controls.Add(this.btnWaymarksExport, 1, 0);
            this.tableWaymarks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableWaymarks.Name = "tableWaymarks";
            this.tableWaymarks.RowCount = 1;
            this.tableWaymarks.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            // 
            // btnWaymarksImport
            // 
            this.btnWaymarksImport.AutoSize = true;
            this.btnWaymarksImport.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnWaymarksImport.Location = new System.Drawing.Point(5, 323);
            this.btnWaymarksImport.Margin = new System.Windows.Forms.Padding(2);
            this.btnWaymarksImport.Name = "btnWaymarksImport";
            this.btnWaymarksImport.Size = new System.Drawing.Size(116, 22);
            this.btnWaymarksImport.TabStop = false;
            this.btnWaymarksImport.Text = "导入";
            this.btnWaymarksImport.UseVisualStyleBackColor = true;
            this.btnWaymarksImport.Click += new System.EventHandler(this.btnWaymarksImport_Click);
            // 
            // btnWaymarksExport
            // 
            this.btnWaymarksExport.AutoSize = true;
            this.btnWaymarksExport.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnWaymarksExport.Location = new System.Drawing.Point(5, 323);
            this.btnWaymarksExport.Margin = new System.Windows.Forms.Padding(2);
            this.btnWaymarksExport.Name = "btnWaymarksExport";
            this.btnWaymarksExport.Size = new System.Drawing.Size(116, 22);
            this.btnWaymarksExport.TabStop = false;
            this.btnWaymarksExport.Text = "导出";
            this.btnWaymarksExport.UseVisualStyleBackColor = true;
            this.btnWaymarksExport.Click += new System.EventHandler(this.btnWaymarksExport_Click);
            // 
            // lstMessages
            // 
            this.mainTable.SetColumnSpan(this.lstMessages, 3);
            this.lstMessages.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstMessages.FormattingEnabled = true;
            this.lstMessages.HorizontalScrollbar = true;
            this.lstMessages.ItemHeight = 12;
            this.lstMessages.Location = new System.Drawing.Point(130, 15);
            this.lstMessages.Margin = new System.Windows.Forms.Padding(10, 5, 5, 3);
            this.lstMessages.Name = "lstMessages";
            this.lstMessages.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstMessages.Size = new System.Drawing.Size(462, 328);
            this.lstMessages.TabStop = false;
            this.lstMessages.MouseMove += new System.Windows.Forms.MouseEventHandler(this.lstMessages_MouseMove);
            // 
            // ButtonClearMessage
            // 
            this.ButtonClearMessage.AutoSize = true;
            this.ButtonClearMessage.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ButtonClearMessage.Location = new System.Drawing.Point(5, 323);
            this.ButtonClearMessage.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonClearMessage.Name = "ButtonClearMessage";
            this.ButtonClearMessage.Size = new System.Drawing.Size(116, 22);
            this.ButtonClearMessage.TabStop = false;
            this.ButtonClearMessage.Text = "清空全部日志";
            this.ButtonClearMessage.UseVisualStyleBackColor = true;
            this.ButtonClearMessage.Click += new System.EventHandler(this.cmdClearMessages_Click);
            // 
            // ButtonCopySelection
            // 
            this.ButtonCopySelection.AutoSize = true;
            this.ButtonCopySelection.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ButtonCopySelection.Location = new System.Drawing.Point(5, 276);
            this.ButtonCopySelection.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonCopySelection.Name = "ButtonCopySelection";
            this.ButtonCopySelection.Size = new System.Drawing.Size(116, 22);
            this.ButtonCopySelection.TabStop = false;
            this.ButtonCopySelection.Text = "复制所选日志";
            this.ButtonCopySelection.UseVisualStyleBackColor = true;
            this.ButtonCopySelection.Click += new System.EventHandler(this.cmdCopySelection_Click);
            // 
            // ButtonCopyProblematic
            // 
            this.ButtonCopyProblematic.AutoSize = true;
            this.ButtonCopyProblematic.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.ButtonCopyProblematic.Location = new System.Drawing.Point(5, 300);
            this.ButtonCopyProblematic.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonCopyProblematic.Name = "ButtonCopyProblematic";
            this.ButtonCopyProblematic.Size = new System.Drawing.Size(116, 22);
            this.ButtonCopyProblematic.TabStop = false;
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
            this.Controls.Add(this.mainPanel);
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DoubleBuffered = true;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "PostNamazuUi";
            this.Size = new System.Drawing.Size(625, 373);
            ((System.ComponentModel.ISupportInitialize)(this.TextPort)).EndInit();
            this.mainGroupBox.ResumeLayout(false);
            this.mainGroupBox.PerformLayout();
            this.flowLayoutActions.ResumeLayout(false);
            this.flowLayoutActions.PerformLayout();
            this.mainTable.ResumeLayout(false);
            this.mainTable.PerformLayout();
            this.tableHttp.ResumeLayout(false);
            this.tableHttp.PerformLayout();
            this.tableWaymarks.ResumeLayout(false);
            this.tableWaymarks.PerformLayout();
            this.grpWaymarks.ResumeLayout(false);
            this.grpWaymarks.PerformLayout();
            this.grpHttp.ResumeLayout(false);
            this.grpHttp.PerformLayout();
            this.grpEnabledCmd.ResumeLayout(false);
            this.grpEnabledCmd.PerformLayout();
            this.grpLang.ResumeLayout(false);
            this.grpLang.PerformLayout();
            this.mainPanel.ResumeLayout(false);
            this.mainPanel.PerformLayout();
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
        public System.Windows.Forms.GroupBox grpHttp;
        public System.Windows.Forms.GroupBox grpEnabledCmd;
        public System.Windows.Forms.GroupBox grpLang;
        public System.Windows.Forms.GroupBox grpWaymarks;
        private System.Windows.Forms.TableLayoutPanel tableWaymarks;
        public System.Windows.Forms.Button btnWaymarksImport;
        public System.Windows.Forms.Button btnWaymarksExport;
        private System.Windows.Forms.ToolTip logTip;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutActions;
        private System.Windows.Forms.Panel mainPanel;
        private System.Windows.Forms.TableLayoutPanel mainTable;
        private System.Windows.Forms.TableLayoutPanel leftTable;
        private System.Windows.Forms.TableLayoutPanel rightTable;
        private System.Windows.Forms.TableLayoutPanel tableHttp;
        private System.Windows.Forms.RadioButton radioButtonEN;
        private System.Windows.Forms.RadioButton radioButtonCN;
        public System.Windows.Forms.ListBox lstMessages;
    }
}