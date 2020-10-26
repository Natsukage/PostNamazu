using Advanced_Combat_Tracker;
using FFXIV_ACT_Plugin.Common;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System;

namespace PostNamazu
{
    class PostNamazuUi : UserControl
    {
        private GroupBox _mainGroupBox;
        private Label _lbPort; //端口：
        public NumericUpDown TextPort; //端口
        public Button ButtonStart { get; private set; }
        public Button ButtonStop { get; private set; }
        private CheckBox _autoStart;

        private static ListBox lstMessages;
        private static Button cmdClearMessages;
        private static Button cmdCopyProblematic;

        public bool AutoStart => _autoStart.Checked;
        private static readonly string SettingsFile = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, "Config\\PostNamazu.config.xml");

        public void InitializeComponent(TabPage pluginScreenSpace) {
            _mainGroupBox = new GroupBox { Location = new Point(12, 12), Text = "监听设置", AutoSize = true, AutoSizeMode = AutoSizeMode.GrowAndShrink};
            _lbPort = new Label { AutoSize = true, Text = "端口:", Location = new Point(10, 33) };
            TextPort = new NumericUpDown { Location = new Point(85, 30), DecimalPlaces = 0, Maximum = 65535, Value = 2019, Size = new Size(100, 35) };
            ButtonStart = new Button { Text = "启动", Location = new Point(190, 30), Size = new Size(75, TextPort.Height) };
            ButtonStop = new Button { Text = "停止", Location = new Point(270, 30), Size = new Size(75, TextPort.Height), Enabled = false };
            _autoStart = new CheckBox { AutoSize = true, Text = "自动启动", Location = new Point(350, 33) };

            lstMessages = new ListBox { Location = new Point(10, TextPort.Height*2+20), FormattingEnabled = true, ScrollAlwaysVisible = true, HorizontalScrollbar = true, Size = new Size(440, 500), };
            cmdClearMessages = new Button { Location = new Point(10, lstMessages.Location.Y + lstMessages.Height + 10), Size = new Size(200, TextPort.Height), Text = "清空日志", UseVisualStyleBackColor = true };
            cmdCopyProblematic = new Button { Location = new Point(220, lstMessages.Location.Y + lstMessages.Height + 10), Size = new Size(200, TextPort.Height), Text = "复制到剪贴板", UseVisualStyleBackColor = true };

            LoadSettings();
            _mainGroupBox.Controls.Add(_lbPort);
            _mainGroupBox.Controls.Add(TextPort);
            _mainGroupBox.Controls.Add(ButtonStart);
            _mainGroupBox.Controls.Add(ButtonStop);
            _mainGroupBox.Controls.Add(_autoStart);

            _mainGroupBox.Controls.Add(lstMessages);
            _mainGroupBox.Controls.Add(cmdClearMessages);
            _mainGroupBox.Controls.Add(cmdCopyProblematic);
            

            pluginScreenSpace.Controls.Add(_mainGroupBox);
            pluginScreenSpace.AutoSize = true;

            cmdCopyProblematic.Click += cmdCopyProblematic_Click;
            cmdClearMessages.Click += cmdClearMessages_Click;

            _mainGroupBox.ResumeLayout(false);
            _mainGroupBox.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        public void Log(string log) {
            AddParserMessage(log);
        }

        public void Log(IntPtr log) {
            AddParserMessage($"{ log.ToInt64():X}");
        }

        public static void cmdCopyProblematic_Click(object sender, EventArgs e) {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (object item in lstMessages.Items) {
                stringBuilder.AppendLine((item ?? "").ToString());
            }
            if (stringBuilder.Length > 0) {
                Clipboard.SetText(stringBuilder.ToString());
            }
        }

        public static void cmdClearMessages_Click(object sender, EventArgs e) {
            lstMessages.Items.Clear();
        }

        public static void AddParserMessage(string message) {
            ACTWrapper.RunOnACTUIThread((System.Action)delegate {
                if (lstMessages.Items.Count > 500)
                    lstMessages.Items.RemoveAt(0);
                bool scroll = lstMessages.TopIndex == lstMessages.Items.Count - lstMessages.Height / lstMessages.ItemHeight;
                lstMessages.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
                if (scroll)
                    lstMessages.TopIndex = lstMessages.Items.Count - lstMessages.Height / lstMessages.ItemHeight;
            });
        }

        void LoadSettings() {

            if (File.Exists(SettingsFile)) {
                XmlDocument xdo = new XmlDocument();
                xdo.Load(SettingsFile);
                XmlNode head = xdo.SelectSingleNode("Config");
                TextPort.Text = head?.SelectSingleNode("Port")?.InnerText;
                _autoStart.Checked = bool.Parse(head?.SelectSingleNode("AutoStart")?.InnerText ?? "false");
            }
        }
        public void SaveSettings() {
            FileStream fs = new FileStream(SettingsFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            XmlTextWriter xWriter = new XmlTextWriter(fs, Encoding.UTF8) { Formatting = Formatting.Indented, Indentation = 1, IndentChar = '\t' };
            xWriter.WriteStartDocument(true);
            xWriter.WriteStartElement("Config");    // <Config>
            xWriter.WriteElementString("Port", TextPort.Text);
            xWriter.WriteElementString("AutoStart", _autoStart.Checked.ToString());
            xWriter.WriteEndElement();  // </Config>
            xWriter.WriteEndDocument(); // Tie up loose ends (shouldn't be any)
            xWriter.Flush();    // Flush the file buffer to disk
            xWriter.Close();
        }
    }
}

