using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using serial_monitor.Utils;

namespace serial_monitor.Windows
{
    /// <summary>
    /// LogWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LogWindow : Window
    {
        private static int Instances = 0;

        public LogWindow()
        {
            Instances += 1;
            InitializeComponent();

            // flush all
            while (Debugger.LogQueue.Count > 0)
            {
                AddLog();
            }

            Debugger.LogRaised += (log) =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { AddLog(); }));
            };
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Instances -= 1;
        }

        private void AddLog()
        {
            if (Debugger.LogQueue.Count > 0)
            {
                LogTextBox.AppendText(Debugger.LogQueue.Dequeue() + Environment.NewLine);
                LogTextBox.ScrollToEnd();
            }
        }

        public static int GetNumberOfInstances()
        {
            return Instances;
        }
    }
}
