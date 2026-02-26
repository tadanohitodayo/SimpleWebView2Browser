using System;
using System.Windows.Forms;
// ↓ ここに BrowserForm がいる名前空間を指定する
using MyTabBrowser;

namespace MyApp
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // MyTabBrowser の中の BrowserForm を呼び出す
            Application.Run(new BrowserForm());
        }
    }
}