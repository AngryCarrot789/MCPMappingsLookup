using MCPMappingsLookup.Views.Main;
using System.ComponentModel;
using System.IO;
using System.Text;
using System.Windows;
using TheRFramework.Utilities.String;

namespace MCPMappingsLookup.Views
{
    public static class WindowManager
    {
        public static App Application { get; private set; }

        public static MainView MainWindow { get; private set; }

        public static bool IsInitialised { get; set; }

        // this is just how i prefer to start an application...
        // and i like it :)))))))
        // you can change it if you want...

        public static void Initialise(App app, string[] args)
        {
            if (IsInitialised)
            {
                return;
            }

            //string path = @"C:\Users\kettl\Desktop\aaa.txt";
            //
            //StringBuilder sb = new StringBuilder(5000);
            //
            //foreach(string line in File.ReadAllLines(path))
            //{
            //    if (line.Extract(0, 2) == "CL")
            //    {
            //        string pair = line.Substring(4).Trim();
            //        int splitter = pair.IndexOf(' ');
            //        string obfuscated = pair.Substring(0, splitter);
            //        string mcpFull = pair.Substring(splitter + 1);
            //        int lastPath = mcpFull.LastIndexOf('/');
            //        string mcp = mcpFull.Substring(lastPath + 1);
            //        sb.AppendLine($"{obfuscated} {mcp}");
            //    }
            //}
            //
            //File.WriteAllText(@"C:\Users\kettl\Desktop\mcpToObfuscated.txt", sb.ToString());

            Application = app;
            
            MainViewModel model = new MainViewModel();
            MainWindow = new MainView()
            {
                Model = model
            };
            
            MainWindow.Closing += OnWindowClosing;

            IsInitialised = true;
        }

        public static void ShutdownApplication()
        {
            ShutdownWindows();
            Application.Shutdown();
        }

        #region Showing and closing/hiding windows

        public static void ShowMain() => ShowWindow(MainWindow);
        public static void HideMain() => HideWindow(MainWindow);

        public static void ShowWindow(Window window)
        {
            window.Show();
        }

        public static void HideWindow(Window window)
        {
            window.Hide();
        }

        private static void ShutdownWindows()
        {
            MainWindow.Closing -= OnWindowClosing;
            //MainWindow.Close();
        }

        private static void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (sender is MainView)
            {
                ShutdownApplication();
            }
            else
            {
                e.Cancel = true;
                (sender as Window)?.Hide();
            }
        }

        #endregion
    }
}
