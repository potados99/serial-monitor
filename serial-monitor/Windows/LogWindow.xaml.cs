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
        private static List<string> LogList = new List<string>();

        public LogWindow()
        {
            Instances += 1;
            InitializeComponent();

            FillLogListFromQueue();
            RestoreLogs();

            Debugger.LogRaised += Log_Raised;
        }



        #region Event Handler

        private void Log_Raised(string log)
        {
            Application.Current.Dispatcher.Invoke(new Action(() => { AddLog(); }));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Instances -= 1;
            Debugger.LogRaised -= Log_Raised;
        }

        #endregion

        #region Helper

        private void FillLogListFromQueue()
        {
            LogList.AddRange(Debugger.LogQueue);
        }

        private void RestoreLogs()
        {
            foreach (var log in LogList)
            {
                LogTextBox.AppendText(log + Environment.NewLine);
            }

            LogTextBox.ScrollToEnd();
        }

        private void AddLog()
        {
            var Q = Debugger.LogQueue;
            while (Q.Count > 0)
            {
                var log = Q.Dequeue();

                LogList.Add(log);
                LogTextBox.AppendText(log + Environment.NewLine);
            }

            LogTextBox.ScrollToEnd();
        }

        public static int GetNumberOfInstances()
        {
            return Instances;
        }

        #endregion
    }
}
