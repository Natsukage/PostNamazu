﻿using System;
 using System.Drawing;
 using System.Windows.Forms;
using Newtonsoft.Json;
using PostNamazu.Actions;
using PostNamazu.Common.Localization;
using PostNamazu.Models;


namespace PostNamazu
{
    public class ImportWaymarksForm : Form
    {
        private static string _prevData;
        private static string _defaultData = @"{
    ""A"": {""X"": 100, ""Z"":  90, ""Y"": 0, ""Active"": true},
    ""B"": {""X"": 110, ""Z"": 100, ""Y"": 0, ""Active"": true},
    ""C"": {""X"": 100, ""Z"": 110, ""Y"": 0, ""Active"": true},
    ""D"": {""X"":  90, ""Z"": 100, ""Y"": 0, ""Active"": true},
    ""One"":   {},
    ""Two"":   {},
    ""Three"": {},
    ""Four"":  {}
}";
        private Panel mainPanel;
        private GroupBox grpMain;
        private TableLayoutPanel mainTable;
        public TextBox TxtWaymarksData;
        public Button btnDefault;
        public Button btnPlace;
        public Button btnPublic;

        internal WayMark WaymarkModule = PostNamazu.Plugin.GetModuleInstance<WayMark>();

        public ImportWaymarksForm()
        {
            Text = L.Get("PostNamazu/importWaymarksForm");
            // Font = PostNamazu.Plugin.PluginUI.Font;              // System.Drawing.Font?

            mainPanel = new() { AutoSize = true, Dock = DockStyle.Fill, Padding = new Padding(20) };
            grpMain = new() { AutoSize = true, Dock = DockStyle.Fill, Padding = new Padding(20) };
            grpMain.Text = L.Get("PostNamazu/importWaymarksFormGrpMain");
            mainTable = new() { AutoSize = true, Dock = DockStyle.Fill };
            TxtWaymarksData = new() { AutoSize = true, Dock = DockStyle.Fill, Multiline = true };
            try
            {
                var clipboardData = Clipboard.GetText() ?? "";
                JsonConvert.DeserializeObject<WayMarks>(Clipboard.GetText());
                TxtWaymarksData.Text = clipboardData;
            }
            catch
            {
                TxtWaymarksData.Text = string.IsNullOrEmpty(_prevData) ? _defaultData : _prevData;
            }
            TxtWaymarksData.Select(TxtWaymarksData.TextLength, 0);

            btnDefault = MyButton(nameof(btnDefault));
            btnDefault.Click += new EventHandler(btnDefault_Click);
            btnPlace = MyButton(nameof(btnPlace));
            btnPlace.Click += new EventHandler(btnPlace_Click);
            btnPublic = MyButton(nameof(btnPublic));
            btnPublic.Click += new EventHandler(btnPublic_Click);

            this.Controls.Add(mainPanel);
            mainPanel.Controls.Add(grpMain);
            grpMain.Controls.Add(mainTable);

            mainTable.RowCount = 2;
            mainTable.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            mainTable.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            mainTable.ColumnCount = 3;
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            mainTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            mainTable.Controls.Add(TxtWaymarksData, 0, 0);
            mainTable.SetColumnSpan(TxtWaymarksData, 3);
            mainTable.Controls.Add(btnDefault, 0, 1);
            mainTable.Controls.Add(btnPlace, 1, 1);
            mainTable.Controls.Add(btnPublic, 2, 1);

            this.FormClosed += (_, __) =>
            {
                if (!string.IsNullOrWhiteSpace(TxtWaymarksData.Text))
                {
                    _prevData = TxtWaymarksData.Text;
                }
            };

            MinimumSize = new Size(PostNamazu.Plugin.PluginUi.Width / 2, PostNamazu.Plugin.PluginUi.Height / 2);
            StartPosition = FormStartPosition.CenterScreen;
        }

        private static Button MyButton(string name)
        {
            Button btn = new()
            {
                Name = name,
                AutoSize = true,
                Anchor = AnchorStyles.None,
                Margin = new Padding(5, 10, 5, 5),
                TabStop = false,
                UseVisualStyleBackColor = true
            };
            // 将按钮名转换为合适的本地化键名格式
            var key = $"PostNamazu/importWaymarksForm{char.ToUpper(name[0])}{name.Substring(1)}";
            btn.Text = L.Get(key);
            return btn;
        }

        private void btnDefault_Click(object sender, EventArgs e)
        {
            TxtWaymarksData.Text = _defaultData;
        }

        private void btnPlace_Click(object sender, EventArgs e)
        {
            try
            {
                var waymarks = JsonConvert.DeserializeObject<WayMarks>(TxtWaymarksData.Text);
                WaymarkModule.DoWaymarks(waymarks);
                MessageBox.Show(L.Get("PostNamazu/importWaymarksFormLocal"), "PostNamazu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(L.Get("PostNamazu/importWaymarksFormFail", ex.ToString()), "PostNamazu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnPublic_Click(object sender, EventArgs e)
        {
            try
            {
                if (WaymarkModule.GetInCombat() == true)
                {
                    MessageBox.Show(L.Get("PostNamazu/importWaymarksFormInCombat"), "PostNamazu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                var waymarks = JsonConvert.DeserializeObject<WayMarks>(TxtWaymarksData.Text);
                WaymarkModule.DoWaymarks(waymarks);
                WaymarkModule.Public(waymarks);
                MessageBox.Show(L.Get("PostNamazu/importWaymarksFormPublic"), "PostNamazu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(L.Get("PostNamazu/importWaymarksFormFail", ex.ToString()), "PostNamazu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}