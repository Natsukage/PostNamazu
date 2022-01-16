using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Advanced_Combat_Tracker;

namespace PostNamazu
{
    public partial class PostNamazuUi : UserControl
    {
        public PostNamazuUi()
        {
            InitializeComponent();
            LoadSettings();
            ButtonCopyProblematic.Click += cmdCopyProblematic_Click;
            ButtonClearMessage.Click += cmdClearMessages_Click;
        }
        public bool AutoStart => CheckAutoStart.Checked;
        private static readonly string SettingsFile = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, "Config\\PostNamazu.config.xml");

        public void Log(string log)
        {
            AddParserMessage(log);
        }

        public void Log(IntPtr log)
        {
            AddParserMessage($"{ log.ToInt64():X}");
        }

        public void cmdCopyProblematic_Click(object sender, EventArgs e)
        {
            StringBuilder stringBuilder = new StringBuilder();
            foreach (object item in lstMessages.Items)
            {
                stringBuilder.AppendLine((item ?? "").ToString());
            }
            if (stringBuilder.Length > 0)
            {
                Clipboard.SetText(stringBuilder.ToString());
            }
        }

        public void cmdClearMessages_Click(object sender, EventArgs e)
        {
            lstMessages.Items.Clear();
        }

        internal static void RunOnACTUIThread(Action code)
        {
            if (ActGlobals.oFormActMain.InvokeRequired && !ActGlobals.oFormActMain.IsDisposed && !ActGlobals.oFormActMain.Disposing)
            {
                ActGlobals.oFormActMain.Invoke(code);
            }
            else
            {
                code();
            }
        }

        public void AddParserMessage(string message)
        {
            RunOnACTUIThread(delegate
            {
                if (lstMessages.Items.Count > 500)
                    lstMessages.Items.RemoveAt(0);
                bool scroll = lstMessages.TopIndex == lstMessages.Items.Count - lstMessages.Height / lstMessages.ItemHeight;
                lstMessages.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
                if (scroll)
                    lstMessages.TopIndex = lstMessages.Items.Count - lstMessages.Height / lstMessages.ItemHeight;
            });
        }

        void LoadSettings()
        {

            if (File.Exists(SettingsFile))
            {
                XmlDocument xdo = new XmlDocument();
                try
                {
                    xdo.Load(SettingsFile);
                    XmlNode head = xdo.SelectSingleNode("Config");
                    TextPort.Text = head?.SelectSingleNode("Port")?.InnerText;
                    CheckAutoStart.Checked = bool.Parse(head?.SelectSingleNode("AutoStart")?.InnerText ?? "false");
                }
                catch (Exception)
                {
                    Log("配置文件载入异常");
                    File.Delete(SettingsFile);
                    Log("已清除错误的配置文件");
                    Log("设置已被重置");
                }

            }
        }
        public void SaveSettings()
        {
            FileStream fs = new FileStream(SettingsFile, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);
            XmlTextWriter xWriter = new XmlTextWriter(fs, Encoding.UTF8) { Formatting = Formatting.Indented, Indentation = 1, IndentChar = '\t' };
            xWriter.WriteStartDocument(true);
            xWriter.WriteStartElement("Config");    // <Config>
            xWriter.WriteElementString("Port", TextPort.Text);
            xWriter.WriteElementString("AutoStart", CheckAutoStart.Checked.ToString());
            xWriter.WriteEndElement();  // </Config>
            xWriter.WriteEndDocument(); // Tie up loose ends (shouldn't be any)
            xWriter.Flush();    // Flush the file buffer to disk
            xWriter.Close();
        }
    }
}
