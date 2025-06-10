using System.Windows.Forms;

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
            this.mainPanel = new System.Windows.Forms.Panel();
            this.mainTable = new System.Windows.Forms.TableLayoutPanel();
            this.leftTable = new System.Windows.Forms.TableLayoutPanel();
            this.rightTable = new System.Windows.Forms.TableLayoutPanel();
            this.tableHttpRow1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableHttpRow3 = new System.Windows.Forms.TableLayoutPanel();
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
            this.tableHttpRow1.SuspendLayout();
            this.tableHttpRow3.SuspendLayout();
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
            this.mainPanel.Margin = new System.Windows.Forms.Padding(8);
            this.mainPanel.Name = "mainPanel";
            this.mainPanel.Padding = new System.Windows.Forms.Padding(8);
            // 
            // mainGroupBox
            // 
            this.mainGroupBox.Controls.Add(this.mainTable);
            this.mainGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;

            this.mainGroupBox.Margin = new System.Windows.Forms.Padding(6);
            this.mainGroupBox.Name = "mainGroupBox";
            this.mainGroupBox.Padding = new System.Windows.Forms.Padding(8);
            this.mainGroupBox.Text = "鲶鱼精邮差";
            // 
            // mainTable
            // 
            this.mainTable.AutoSize = true;
            this.mainTable.ColumnCount = 2;
            this.mainTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 200F));
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
            this.leftTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            
            this.leftTable.Controls.Add(this.grpEnabledCmd, 0, 0);
            this.leftTable.Controls.Add(this.grpHttp, 0, 1);
            this.leftTable.Controls.Add(this.grpLang, 0, 2);
            this.leftTable.Controls.Add(this.grpWaymarks, 0, 3);
            this.leftTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.leftTable.Name = "leftTable";
            this.leftTable.RowCount = 4;

            this.leftTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.leftTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.leftTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            this.leftTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            // 
            // rightTable
            // 
            this.rightTable.ColumnCount = 3;
            this.rightTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rightTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.rightTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
            this.rightTable.Controls.Add(this.lstMessages, 0, 0);
            this.rightTable.Controls.Add(this.ButtonCopySelection, 0, 1);
            this.rightTable.Controls.Add(this.ButtonCopyProblematic, 1, 1);
            this.rightTable.Controls.Add(this.ButtonClearMessage, 2, 1);
            this.rightTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightTable.Name = "rightTable";
            this.rightTable.RowCount = 2;
            this.rightTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.rightTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            // 
            // grpHttp
            // 
            this.grpHttp.AutoSize = true;
            //this.grpHttp.Controls.Add(this.tableHttp);
            this.grpHttp.Controls.Add(this.tableHttpRow1);
            this.grpHttp.Controls.Add(this.TextPort);
            this.grpHttp.Controls.Add(this.tableHttpRow3);
            this.grpHttp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpHttp.Margin = new System.Windows.Forms.Padding(0, 0, 4, 8);
            this.grpHttp.Name = "grpHttp";
            this.grpHttp.Padding = new System.Windows.Forms.Padding(6, 4, 6, 6);
            this.grpHttp.Text = "HTTP";
            // 
            // tableHttpRow1
            // 
            this.tableHttpRow1.AutoSize = true;
            this.tableHttpRow1.ColumnCount = 2;
            this.tableHttpRow1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33F));
            this.tableHttpRow1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 66.67F));
            this.tableHttpRow1.Controls.Add(this.lbPort, 0, 0);
            this.tableHttpRow1.Controls.Add(this.CheckAutoStart, 1, 0);
            this.tableHttpRow1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableHttpRow1.Name = "tableHttpRow1";
            this.tableHttpRow1.RowCount = 1;
            this.tableHttpRow1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            // 
            // tableHttpRow3
            // 
            this.tableHttpRow3.AutoSize = true;
            this.tableHttpRow3.ColumnCount = 2;
            this.tableHttpRow3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableHttpRow3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableHttpRow3.Controls.Add(this.ButtonStart, 0, 0);
            this.tableHttpRow3.Controls.Add(this.ButtonStop, 1, 0);
            this.tableHttpRow3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tableHttpRow3.Name = "tableHttpRow3";
            this.tableHttpRow3.RowCount = 1;
            this.tableHttpRow3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            // 
            // lbPort
            // 
            this.lbPort.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbPort.AutoSize = true;
            this.lbPort.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.lbPort.Name = "lbPort";
            this.lbPort.Text = "端口：";
            // 
            // TextPort
            // 
            this.TextPort.Dock = DockStyle.Bottom;
            this.TextPort.Maximum = new decimal(new int[] { 65535, 0, 0, 0});
            this.TextPort.Name = "TextPort";
            this.TextPort.Size = new System.Drawing.Size(80, 23);
            this.TextPort.TabIndex = 1;
            this.TextPort.Value = new decimal(new int[] { 2019, 0, 0, 0});
            // 
            // ButtonStart
            // 
            this.ButtonStart.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonStart.Location = new System.Drawing.Point(2, 2);
            this.ButtonStart.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonStart.Name = "ButtonStart";
            this.ButtonStart.Size = new System.Drawing.Size(80, 23);
            this.ButtonStart.TabIndex = 2;
            this.ButtonStart.Text = "开始";
            this.ButtonStart.UseVisualStyleBackColor = true;
            // 
            // ButtonStop
            // 
            this.ButtonStop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonStop.Enabled = false;
            this.ButtonStop.Location = new System.Drawing.Point(86, 2);
            this.ButtonStop.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonStop.Name = "ButtonStop";
            this.ButtonStop.Size = new System.Drawing.Size(80, 23);
            this.ButtonStop.TabIndex = 3;
            this.ButtonStop.Text = "停止";
            this.ButtonStop.UseVisualStyleBackColor = true;
            // 
            // CheckAutoStart
            // 
            this.CheckAutoStart.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.CheckAutoStart.AutoSize = true;
            this.CheckAutoStart.Location = new System.Drawing.Point(58, 2);
            this.CheckAutoStart.Margin = new System.Windows.Forms.Padding(0, 2, 2, 2);
            this.CheckAutoStart.Name = "CheckAutoStart";
            this.CheckAutoStart.Size = new System.Drawing.Size(96, 19);
            this.CheckAutoStart.TabIndex = 4;
            this.CheckAutoStart.Text = "自动启动监听";
            this.CheckAutoStart.UseVisualStyleBackColor = true;
            // 
            // grpEnabledCmd
            // 
            this.grpEnabledCmd.Controls.Add(this.flowLayoutActions);
            this.grpEnabledCmd.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpEnabledCmd.Margin = new System.Windows.Forms.Padding(0, 0, 4, 8);
            this.grpEnabledCmd.Name = "grpEnabledCmd";
            this.grpEnabledCmd.Padding = new System.Windows.Forms.Padding(6, 4, 6, 6);
            this.grpEnabledCmd.Text = "启用以下动作";
            // 
            // flowLayoutActions
            // 
            this.flowLayoutActions.AutoScroll = true;
            this.flowLayoutActions.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutActions.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutActions.Margin = new System.Windows.Forms.Padding(0);
            this.flowLayoutActions.Name = "flowLayoutActions";
            this.flowLayoutActions.WrapContents = false;
            // 
            // grpLang
            // 
            this.grpLang.AutoSize = true;
            this.grpLang.Controls.Add(this.radioButtonCN);
            this.grpLang.Controls.Add(this.radioButtonEN);
            this.grpLang.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpLang.Margin = new System.Windows.Forms.Padding(0, 0, 4, 8);
            this.grpLang.Name = "grpLang";
            this.grpLang.Padding = new System.Windows.Forms.Padding(6, 4, 6, 6);
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
            this.grpWaymarks.Controls.Add(this.tableWaymarks);
            this.grpWaymarks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpWaymarks.Margin = new System.Windows.Forms.Padding(0, 0, 4, 0);
            this.grpWaymarks.Name = "grpWaymarks";
            this.grpWaymarks.Padding = new System.Windows.Forms.Padding(6, 4, 6, 6);
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
            this.tableWaymarks.Margin = new System.Windows.Forms.Padding(0);
            this.tableWaymarks.Name = "tableWaymarks";
            this.tableWaymarks.RowCount = 1;
            this.tableWaymarks.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.AutoSize));
            // 
            // btnWaymarksImport
            // 
            this.btnWaymarksImport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnWaymarksImport.Location = new System.Drawing.Point(2, 2);
            this.btnWaymarksImport.Margin = new System.Windows.Forms.Padding(2);
            this.btnWaymarksImport.Name = "btnWaymarksImport";
            this.btnWaymarksImport.Size = new System.Drawing.Size(92, 23);
            this.btnWaymarksImport.TabStop = false;
            this.btnWaymarksImport.Text = "导入";
            this.btnWaymarksImport.UseVisualStyleBackColor = true;
            this.btnWaymarksImport.Click += new System.EventHandler(this.BtnWaymarksImport_Click);
            // 
            // btnWaymarksExport
            // 
            this.btnWaymarksExport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnWaymarksExport.Location = new System.Drawing.Point(98, 2);
            this.btnWaymarksExport.Margin = new System.Windows.Forms.Padding(2);
            this.btnWaymarksExport.Name = "btnWaymarksExport";
            this.btnWaymarksExport.Size = new System.Drawing.Size(92, 23);
            this.btnWaymarksExport.TabStop = false;
            this.btnWaymarksExport.Text = "导出";
            this.btnWaymarksExport.UseVisualStyleBackColor = true;
            this.btnWaymarksExport.Click += new System.EventHandler(this.BtnWaymarksExport_Click);
            // 
            // lstMessages
            // 
            this.rightTable.SetColumnSpan(this.lstMessages, 3);
            this.lstMessages.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lstMessages.Dock = System.Windows.Forms.DockStyle.Fill;

            this.lstMessages.FormattingEnabled = true;
            this.lstMessages.HorizontalScrollbar = true;
            this.lstMessages.ItemHeight = 13;
            this.lstMessages.Location = new System.Drawing.Point(6, 6);
            this.lstMessages.Margin = new System.Windows.Forms.Padding(6, 6, 6, 2);
            this.lstMessages.Name = "lstMessages";
            this.lstMessages.SelectionMode = System.Windows.Forms.SelectionMode.MultiExtended;
            this.lstMessages.Size = new System.Drawing.Size(408, 357);
            this.lstMessages.TabIndex = 0;
            this.lstMessages.TabStop = false;
            this.lstMessages.MouseMove += new System.Windows.Forms.MouseEventHandler(this.LstMessages_MouseMove);
            // 
            // ButtonClearMessage
            // 
            this.ButtonClearMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonClearMessage.Location = new System.Drawing.Point(8, 367);
            this.ButtonClearMessage.Margin = new System.Windows.Forms.Padding(2, 2, 4, 2);
            this.ButtonClearMessage.Name = "ButtonClearMessage";
            this.ButtonClearMessage.Size = new System.Drawing.Size(326, 31);
            this.ButtonClearMessage.TabIndex = 1;
            this.ButtonClearMessage.TabStop = false;
            this.ButtonClearMessage.Text = "清空全部日志";
            this.ButtonClearMessage.UseVisualStyleBackColor = true;
            this.ButtonClearMessage.Click += new System.EventHandler(this.CmdClearMessages_Click);
            // 
            // ButtonCopySelection
            // 
            this.ButtonCopySelection.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonCopySelection.Location = new System.Drawing.Point(340, 367);
            this.ButtonCopySelection.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonCopySelection.Name = "ButtonCopySelection";
            this.ButtonCopySelection.Size = new System.Drawing.Size(86, 31);
            this.ButtonCopySelection.TabIndex = 2;
            this.ButtonCopySelection.TabStop = false;
            this.ButtonCopySelection.Text = "复制选中日志";
            this.ButtonCopySelection.UseVisualStyleBackColor = true;
            this.ButtonCopySelection.Click += new System.EventHandler(this.CmdCopySelection_Click);
            // 
            // ButtonCopyProblematic
            // 
            this.ButtonCopyProblematic.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonCopyProblematic.Location = new System.Drawing.Point(430, 367);
            this.ButtonCopyProblematic.Margin = new System.Windows.Forms.Padding(2);
            this.ButtonCopyProblematic.Name = "ButtonCopyProblematic";
            this.ButtonCopyProblematic.Size = new System.Drawing.Size(86, 31);
            this.ButtonCopyProblematic.TabIndex = 3;
            this.ButtonCopyProblematic.TabStop = false;
            this.ButtonCopyProblematic.Text = "复制全部日志";
            this.ButtonCopyProblematic.UseVisualStyleBackColor = true;
            this.ButtonCopyProblematic.Click += new System.EventHandler(this.CmdCopyProblematic_Click);
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
            this.tableHttpRow1.ResumeLayout(false);
            this.tableHttpRow1.PerformLayout();
            this.tableHttpRow3.ResumeLayout(false);
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
        private System.Windows.Forms.TableLayoutPanel tableHttpRow1;
        private System.Windows.Forms.TableLayoutPanel tableHttpRow3;
        private System.Windows.Forms.RadioButton radioButtonEN;
        private System.Windows.Forms.RadioButton radioButtonCN;
        public System.Windows.Forms.ListBox lstMessages;
    }
}