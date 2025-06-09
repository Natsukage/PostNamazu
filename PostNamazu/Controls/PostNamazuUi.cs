﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Advanced_Combat_Tracker;
using PostNamazu.Actions;
using PostNamazu.Common;
using PostNamazu.Models;

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

        public void cmdCopyProblematic_Click(object sender, EventArgs e) => CopyLog(copyAll: true);

        public void cmdCopySelection_Click(object sender, EventArgs e) => CopyLog(copyAll: false);

        private void CopyLog(bool copyAll)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var source = copyAll ? lstMessages.Items.Cast<object>() : lstMessages.SelectedItems.Cast<object>();
            foreach (object item in source)
            {
                stringBuilder.AppendLine((item ?? "").ToString());
            }
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Remove(stringBuilder.Length - Environment.NewLine.Length, Environment.NewLine.Length);
                Clipboard.SetText(stringBuilder.ToString());
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.C))
            {
                CopyLog(copyAll: false);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void cmdClearMessages_Click(object sender, EventArgs e)
        {
            lstMessages.Items.Clear();
        }

        private int prevTipIdx = -1;
        private void lstMessages_MouseMove(object sender, MouseEventArgs e)
        {
            ListBox lb = sender as ListBox;
            int index = lb.IndexFromPoint(e.Location);
            if (index != prevTipIdx)
            {
                if (index != -1)
                    logTip.SetToolTip(lb, lb.Items[index].ToString());
                else
                    logTip.RemoveAll();
                prevTipIdx = index;
            }
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

                    string language = head?.SelectSingleNode("Language")?.InnerText;
                    if (string.IsNullOrEmpty(language) || !Enum.TryParse(language, out I18n.CurrentLanguage))
                    {
                        I18n.CurrentLanguage = CultureInfo.CurrentCulture.TwoLetterISOLanguageName == "zh" ? I18n.Language.CN : I18n.Language.EN;
                    }
                    if (I18n.CurrentLanguage == I18n.Language.EN)
                    {
                        radioButtonEN.Checked = true;
                    }
                    else
                    {
                        radioButtonCN.Checked = true;
                    }
                    TranslateUi();

                    var actionList = head?.SelectSingleNode("Actions")?.ChildNodes;
                    if (actionList == null) return;
                    foreach (XmlNode action in actionList)
                        ActionEnabled[action.Name] = bool.Parse(action.InnerText);
                }
                catch (Exception ex)
                {
                    Log(I18n.Translate("PostNamazuUi/CfgLoadException", "配置文件载入异常：\n{0}", ex.ToString()));
                    File.Delete(SettingsFile);
                    Log(I18n.Translate("PostNamazuUi/CfgReset", "已重置配置文件。"));
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
            xWriter.WriteElementString("Language", I18n.CurrentLanguage.ToString());
            xWriter.WriteStartElement("Actions");    // <Actions>
            foreach (var action in ActionEnabled)
                xWriter.WriteElementString(action.Key,action.Value.ToString());
            xWriter.WriteEndElement();  // </Actions>
            xWriter.WriteEndElement();  // </Config>
            xWriter.WriteEndDocument(); // Tie up loose ends (shouldn't be any)
            xWriter.Flush();    // Flush the file buffer to disk
            xWriter.Close();
        }

        private void LanguageRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonEN.Checked)
            {
                I18n.CurrentLanguage = I18n.Language.EN;
            }
            else if (radioButtonCN.Checked)
            {
                I18n.CurrentLanguage = I18n.Language.CN;
            }
            TranslateUi();
        }

        /// <summary> 根据模组状态更新对应动作的颜色。</summary>
        /// <param name="actionName">模组的类名（Type.Name）。</param>
        internal void UpdateActionColorByState(string actionName, PostNamazu.StateEnum state)
        {
            if (flowLayoutActions.InvokeRequired)
            {
                flowLayoutActions.Invoke(new Action(() => UpdateActionColorByState(actionName, state)));
                return;
            }
            var checkBox = flowLayoutActions.Controls.OfType<CheckBox>().FirstOrDefault(chk => chk.Text == actionName);
            if (checkBox == null) return;
            switch (state)
            {
                case PostNamazu.StateEnum.Failure:
                    checkBox.ForeColor = Color.FromArgb(180, 45, 30); // red
                    break;
                case PostNamazu.StateEnum.Waiting:
                    checkBox.ForeColor = Color.FromArgb(150, 105, 0); // yellow
                    break;
                case PostNamazu.StateEnum.Ready:
                    checkBox.ForeColor = Color.FromArgb(15, 90, 60); // green
                    break;
                case PostNamazu.StateEnum.NotReady:
                    checkBox.ForeColor = Color.Black;
                    break;
            }
        }

        internal void TranslateUi()
        {
            this.SuspendLayout();
            if (Parent != null) Parent.Text = I18n.Translate("PostNamazu", "鲶鱼精邮差");
            RecursiveTranslateControls(this);
            this.ResumeLayout(true);
        }

        private void RecursiveTranslateControls(Control control)
        {
            if (!string.IsNullOrEmpty(control.Text))
            {
                string text = I18n.TranslateUi(control.Name);
                if (text != null)
                {
                    control.Text = text;
                }
            }
            foreach (Control child in control.Controls)
            {
                RecursiveTranslateControls(child);
            }
        }

        public void btnWaymarksImport_Click(object sender, EventArgs e)
        {
            ImportWaymarksForm importForm = new ImportWaymarksForm();
            importForm.Show(this);
            importForm.BringToFront();
        }

        public void btnWaymarksExport_Click(object sender, EventArgs e)
        {
            string data;
            try
            {
                data = GetCurrentWaymarksString();
                Clipboard.SetText(data);
                MessageBox.Show(I18n.Translate("PostNamazuUi/ExportWaymarks", "已将标点文本存入剪贴板。"), "PostNamazu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(I18n.Translate("PostNamazuUi/ExportWaymarksFail", "读取现有标点失败：\n{0}", ex.ToString()), "PostNamazu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }

        public static string GetCurrentWaymarksString()
        {
            WayMarks waymarks = PostNamazu.Plugin.GetModuleInstance<WayMark>().ReadCurrentWaymarks();
            return waymarks.ToJsonString();
        }

    }
}