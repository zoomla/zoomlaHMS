using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ZoomlaHms.Common;

namespace ZoomlaHms.JsEvent.Implements
{
    public class HelpfulJsEvent : IJsEvent
    {
        public void OpenLogView()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new LogViewer().Show();
            });
        }

        public void ShowPolicy()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                new Policy().Show();
            });
        }

        public string GetSay()
        {
            var file = Path.Combine(SystemPath.PathBase, "says.txt");
            if (!File.Exists(file))
            { return string.Empty; }

            var lines = File.ReadAllLines(file);
            if (lines == null || lines.Length == 0)
            { return string.Empty; }

            string content = string.Empty;
            int rc = 100;
            while (string.IsNullOrEmpty(content) && rc > 0)
            {
                int index = new Random().Next(0, lines.Length);
                content = lines[index];
                rc--;
            }

            return content;
        }
    }
}
