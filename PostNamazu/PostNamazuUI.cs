using Advanced_Combat_Tracker;
using System;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using PostNamazu.Controls;

namespace PostNamazu
{
    class PostNamazuUi
    {
        public UIControl ui = new UIControl();
        public bool AutoStart => ui.CheckAutoStart.Checked;
        private static readonly string SettingsFile = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, "Config\\PostNamazu.config.xml");

        public PostNamazuUi(TabPage pluginScreenSpace)
        {
            pluginScreenSpace.Controls.Add(ui);
            LoadSettings();
            ui.ButtonCopyProblematic.Click += cmdCopyProblematic_Click;
            ui.ButtonClearMessage.Click += cmdClearMessages_Click;
        }

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
            foreach (object item in ui.lstMessages.Items)
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
            ui.lstMessages.Items.Clear();
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
                if (ui.lstMessages.Items.Count > 500)
                    ui.lstMessages.Items.RemoveAt(0);
                bool scroll = ui.lstMessages.TopIndex == ui.lstMessages.Items.Count - ui.lstMessages.Height / ui.lstMessages.ItemHeight;
                ui.lstMessages.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
                if (scroll)
                    ui.lstMessages.TopIndex = ui.lstMessages.Items.Count - ui.lstMessages.Height / ui.lstMessages.ItemHeight;
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
                    ui.TextPort.Text = head?.SelectSingleNode("Port")?.InnerText;
                    ui.CheckAutoStart.Checked = bool.Parse(head?.SelectSingleNode("AutoStart")?.InnerText ?? "false");
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
            xWriter.WriteElementString("Port", ui.TextPort.Text);
            xWriter.WriteElementString("AutoStart", ui.CheckAutoStart.Checked.ToString());
            xWriter.WriteEndElement();  // </Config>
            xWriter.WriteEndDocument(); // Tie up loose ends (shouldn't be any)
            xWriter.Flush();    // Flush the file buffer to disk
            xWriter.Close();
        }
    }
}
