using System;
using System.Collections.Generic;
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
        }
        public bool AutoStart => CheckAutoStart.Checked;
        private static readonly string SettingsFile = Path.Combine(ActGlobals.oFormActMain.AppDataFolder.FullName, "Config\\PostNamazu.config.xml");
        public Dictionary<string, bool> ActionEnabled = new();

        public void RegisterAction(string name)
        {
            if (!ActionEnabled.ContainsKey(name))
                ActionEnabled[name] = true;
            CheckBox checkAction = new() { Text = name, Checked = ActionEnabled[name] ,AutoSize = true};
            checkAction.CheckedChanged += CheckBoxActions_CheckedChanged;
            flowLayoutActions.Controls.Add(checkAction);
        }

        private void CheckBoxActions_CheckedChanged(object sender, System.EventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            ActionEnabled[checkbox.Text]=checkbox.Checked;
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
                XmlDocument xdo = new();
                try
                {
                    xdo.Load(SettingsFile);
                    XmlNode head = xdo.SelectSingleNode("Config");
                    TextPort.Text = head?.SelectSingleNode("Port")?.InnerText;
                    if (string.IsNullOrEmpty(TextPort.Text)) 
                        TextPort.Text = "2019";
                    CheckAutoStart.Checked = bool.Parse(head?.SelectSingleNode("AutoStart")?.InnerText ?? "false");

                    var actionList = head?.SelectSingleNode("Actions")?.ChildNodes;
                    if (actionList == null) return;
                    foreach (XmlNode action in actionList)
                        ActionEnabled[action.Name] = bool.Parse(action.InnerText);
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
            xWriter.WriteStartElement("Actions");    // <Actions>
            foreach (var action in ActionEnabled)
                xWriter.WriteElementString(action.Key,action.Value.ToString());
            xWriter.WriteEndElement();  // </Actions>
            xWriter.WriteEndElement();  // </Config>
            xWriter.WriteEndDocument(); // Tie up loose ends (shouldn't be any)
            xWriter.Flush();    // Flush the file buffer to disk
            xWriter.Close();
        }
    }
}
