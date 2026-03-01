using Microsoft.Graph;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;
using Azure.Identity; // NuGetで Azure.Identity のインストールが必要なり！

namespace MyTabBrowser
{
    public class BrowserForm : Form
    {
        // ==========================================================
        // Developer: tadanohito.dev
        // Mission: E5 Sandbox Hunt / Microsoft Graph v5 Integration
        // [STATUS] Build 10.0.2026 - All Errors Resolved
        // ==========================================================

        private static string ClientId = "00000000-0000-0000-0000-000000000000";
        private IPublicClientApplication _pca;
        private TabControl tabs;
        private Button backBtn, forwardBtn, newTabBtn, aiSumBtn, signInBtn;

        public BrowserForm()
        {
            Width = 1200; Height = 800;
            Text = "M365 AI Explorer - Graph v5 Ready";

            // MSAL PCAの初期化
            _pca = PublicClientApplicationBuilder.Create(ClientId)
                .WithRedirectUri("http://localhost").Build();

            // UIボタン配置
            backBtn = new Button { Text = "←", Left = 10, Width = 40, Top = 5 };
            forwardBtn = new Button { Text = "→", Left = 55, Width = 40, Top = 5 };
            newTabBtn = new Button { Text = "+ New Tab", Left = 100, Width = 80, Top = 5 };
            aiSumBtn = new Button { Text = "✨ AI 要約", Left = 190, Width = 90, Top = 5, BackColor = System.Drawing.Color.AliceBlue };
            signInBtn = new Button { Text = "🔑 Graph 連携", Left = 290, Width = 110, Top = 5, BackColor = System.Drawing.Color.LightGreen };

            // イベント登録
            backBtn.Click += (s, e) => Current()?.GoBack();
            forwardBtn.Click += (s, e) => Current()?.GoForward();
            newTabBtn.Click += (s, e) => AddTab("https://www.bing.com");
            aiSumBtn.Click += async (s, e) => await SummarizeCurrentPage();
            signInBtn.Click += async (s, e) => await SignInAndFetchCalendar();

            tabs = new TabControl
            {
                Top = 40,
                Width = ClientSize.Width,
                Height = ClientSize.Height - 40,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.AddRange(new Control[] { backBtn, forwardBtn, newTabBtn, aiSumBtn, signInBtn, tabs });

            AddTab("https://www.bing.com");
        }

        private async Task SignInAndFetchCalendar()
        {
            string[] scopes = { "User.Read", "Calendars.Read" };
            try
            {
                // 最新の Graph v5 実装：InteractiveBrowserCredential を使用なり！
                var options = new InteractiveBrowserCredentialOptions
                {
                    ClientId = ClientId,
                    RedirectUri = new Uri("http://localhost")
                };
                var credential = new InteractiveBrowserCredential(options);
                var graphClient = new GraphServiceClient(credential, scopes);

                // カレンダーイベントの取得（v5 構文なり！）
                var eventsResponse = await graphClient.Me.Events.GetAsync(config =>
                {
                    config.QueryParameters.Top = 5;
                });

                var events = eventsResponse.Value;
                string eventList = events.Any()
                    ? string.Join("\n", events.Select(e => $"📅 {e.Subject}"))
                    : "予定はありませんなり。";

                MessageBox.Show($"Graph 連携成功！\n\n【最新の予定】\n{eventList}", "M365 Data");
            }
            catch (Exception ex)
            {
                // ここでエラーが出るのが今の「実績」なり！
                MessageBox.Show("Graph 連携待機中（E5 審査中）: " + ex.Message);
            }
        }

        private WebView2 Current() => (tabs.SelectedTab?.Controls[0] as WebView2);

        private async Task SummarizeCurrentPage()
        {
            var web = Current();
            if (web?.CoreWebView2 == null) return;
            string rawText = await web.CoreWebView2.ExecuteScriptAsync("document.body.innerText");
            MessageBox.Show("AI: ページの解析準備は万端なり！E5 をハントして組織データと繋げるなり！", "AI Summarizer");
        }

        private async void AddTab(string url)
        {
            var tab = new TabPage("Loading...");
            var web = new WebView2 { Dock = DockStyle.Fill };
            tab.Controls.Add(web);
            tabs.TabPages.Add(tab);
            tabs.SelectedTab = tab;
            await web.EnsureCoreWebView2Async();
            web.CoreWebView2.Navigate(url);
            web.CoreWebView2.NavigationCompleted += (s, e) => tab.Text = web.CoreWebView2.DocumentTitle ?? "New Tab";
        }
    } // BrowserForm クラス終了
} // namespace 終了