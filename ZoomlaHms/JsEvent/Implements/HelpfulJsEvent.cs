using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
    }
}
