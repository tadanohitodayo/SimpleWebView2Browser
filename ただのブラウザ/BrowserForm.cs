
using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;

namespace MyTabBrowser
{
    public class BrowserForm : Form
    {
        // ==========================================================
        // Developer: tadanohito.dev
        // Passion: I absolutely love Microsoft! 
        // Among the Big 3, Microsoft is my top choice and my inspiration.
        // Thank you for the incredible .NET 10 & WebView2 ecosystem.
        // ==========================================================
        // --- Microsoft 365 Developer Program Support ---
        // Plan: Integrate Microsoft Graph to show Outlook Calendar in a side panel.
        // Scope: User.Read, Calendars.Read
        // [PLAN] E5 License Integration
        // I am eagerly waiting for the Microsoft 365 E5 Sandbox.
        // My goal is to build an AI-powered browser that empowers everyone 
        // using the Microsoft Graph API. I love building on this platform!

        private static string ClientId = "00000000-0000-0000-0000-000000000000";
        private IPublicClientApplication _pca;

        private TabControl tabs;
        private Button backBtn, forwardBtn, newTabBtn, aiSumBtn;
        private string historyFile = "history.txt";

        public BrowserForm()
        {
            Width = 1100;
            Height = 750;
            Text = "M365 Graph Explorer & AI Summarizer";

            // UIコントロール
            backBtn = new Button { Text = "←", Left = 10, Width = 40, Top = 5 };
            forwardBtn = new Button { Text = "→", Left = 55, Width = 40, Top = 5 };
            newTabBtn = new Button { Text = "+ New Tab", Left = 100, Width = 80, Top = 5 };
            aiSumBtn = new Button { Text = "✨ AI 要約", Left = 190, Width = 100, Top = 5, BackColor = System.Drawing.Color.AliceBlue };

            backBtn.Click += (s, e) => Current()?.GoBack();
            forwardBtn.Click += (s, e) => Current()?.GoForward();
            newTabBtn.Click += (s, e) => AddTab("https://www.Bing.com");
            aiSumBtn.Click += async (s, e) => await SummarizeCurrentPage();

            tabs = new TabControl
            {
                Top = 40,
                Width = ClientSize.Width,
                Height = ClientSize.Height - 40,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };

            Controls.Add(backBtn);
            Controls.Add(forwardBtn);
            Controls.Add(newTabBtn);
            Controls.Add(aiSumBtn);
            Controls.Add(tabs);

            AddTab("https://www.microsoft.com");
        }

        private WebView2 Current()
        {
            if (tabs.SelectedTab == null) return null;
            return tabs.SelectedTab.Controls[0] as WebView2;
        }

        // AIページ要約エンジン
        private async Task SummarizeCurrentPage()
        {
            var web = Current();
            if (web == null || web.CoreWebView2 == null) return;

            try
            {
                // JavaScriptでページ内のテキストを抽出
                string rawText = await web.CoreWebView2.ExecuteScriptAsync("document.body.innerText");

                // AI解析シミュレーション（E5付与後にAPIを本格実装）
                string summary = "AI: このページにはMicrosoftの革新的な技術が詰まっています。";
                string encouragement = "この情報を活かして、素晴らしい開発を続けましょう。";

                MessageBox.Show($"{summary}\n\n【励まし】\n{encouragement}", "AI Summarizer");
            }
            catch (Exception ex)
            {
                MessageBox.Show("AI要約の実行中にエラーが発生しました: " + ex.Message);
            }
        }

        private async void AddTab(string url)
        {
            var tab = new TabPage("Loading...");
            var web = new WebView2 { Dock = DockStyle.Fill };
            tab.Controls.Add(web);
            tabs.TabPages.Add(tab);
            tabs.SelectedTab = tab;

            try
            {
                await web.EnsureCoreWebView2Async();
                web.CoreWebView2.Navigate(url);

                web.CoreWebView2.NavigationCompleted += (s, e) =>
                {
                    tab.Text = web.CoreWebView2.DocumentTitle ?? "New Tab";
                    File.AppendAllText(historyFile, $"{web.Source} [{DateTime.Now}]{Environment.NewLine}");
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("WebView2の初期化に失敗しました: " + ex.Message);
            }
        }
    }
}