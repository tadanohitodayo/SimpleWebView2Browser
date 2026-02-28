using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;
using System;
using System.IO;
using System.Windows.Forms;
using Microsoft.Web.WebView2.WinForms;
using Microsoft.Web.WebView2.Core;

namespace MyTabBrowser
{
    public class BrowserForm : Form
    { // ==========================================================
      // Developer: tadanohitodayo (tadanohito.dev)
      // Passion: I absolutely love Microsoft! 
      // Among the Big 3, Microsoft is my top choice and my inspiration.
      // Thank you for the incredible .NET 10 & WebView2 ecosystem.
      // ==========================================================
      // --- Microsoft 365 Developer Program Support ---
      // Plan: Integrate Microsoft Graph to show Outlook Calendar in a side panel.
      // Scope: User.Read, Calendars.Read
      // Shifted to Dev Account
      // [PLAN] E5 License Integration
      // I am eagerly waiting for the Microsoft 365 E5 Sandbox.
      // My goal is to build an AI-powered browser that empowers everyone 
      // using the Microsoft Graph API. I love building on this platform!
        private static string ClientId = "00000000-0000-0000-0000-000000000000";
        private IPublicClientApplication _pca;

        private TabControl tabs;
        private Button backBtn, forwardBtn, newTabBtn;
        private string historyFile = "history.txt";

        public BrowserForm()
        {
            Width = 1000;
            Height = 700;
            Text = "M365 Graph Explorer Browser";


            // UIコントロールの作成
            backBtn = new Button { Text = "←", Left = 10, Width = 40, Top = 5 };
            forwardBtn = new Button { Text = "→", Left = 55, Width = 40, Top = 5 };
            newTabBtn = new Button { Text = "+ New Tab", Left = 100, Width = 80, Top = 5 };

            backBtn.Click += (s, e) => Current()?.GoBack();
            forwardBtn.Click += (s, e) => Current()?.GoForward();
            newTabBtn.Click += (s, e) => AddTab("https://www.google.com");

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
            Controls.Add(tabs);

            AddTab("https://www.google.com");
        }

        private WebView2 Current()
        {
            if (tabs.SelectedTab == null) return null;
            return tabs.SelectedTab.Controls[0] as WebView2;
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

                web.CoreWebView2.HistoryChanged += (s, e) =>
                {
                    backBtn.Enabled = web.CoreWebView2.CanGoBack;
                    forwardBtn.Enabled = web.CoreWebView2.CanGoForward;
                };

                web.CoreWebView2.NavigationCompleted += (s, e) =>
                {
                    string pageTitle = web.CoreWebView2.DocumentTitle;
                    tab.Text = string.IsNullOrEmpty(pageTitle) ? "New Tab" : pageTitle;

                    try
                    {
                        File.AppendAllText(historyFile, web.Source + " [" + DateTime.Now + "]" + Environment.NewLine);
                    }
                    catch (IOException)
                    {
                        // ログ出力失敗時のハンドリング（コンソールやデバッグ出力など）
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show("WebView2の初期化に失敗しました: " + ex.Message);
            }
        }
    }
}