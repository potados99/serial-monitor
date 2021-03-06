﻿using serial_monitor.Utils;
using serial_monitor.ViewModels;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace serial_monitor.Windows
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Fields

        private MainViewModel VM = MainViewModel.Instance;

        #endregion

        #region Constructor

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = VM;
        }

        #endregion

        #region Event

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            VM.DisposePort();
            App.Current.Shutdown();
        }

        private void PromptTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendToPort();
            }
        }

        #endregion

        #region Button Click

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenPort();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            ClosePort();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            SendToPort();
        }

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            ClearLogBox();
        }

        private void ShowLogItem_Click(object sender, RoutedEventArgs e)
        {
            if (LogWindow.GetNumberOfInstances() == 0)
            {
                new LogWindow().Show();
            }
        }

        private void AppInfoItem_Click(object sender, RoutedEventArgs e)
        {
            var ver = Assembly.GetExecutingAssembly().GetName().Version.ToString();

            MessageBox.Show(
                "Serial Monitor\n" +
                "Version " + ver.ToString() + "\n\n" +
                "Copyright © 2019 Potados All Rights Reserved.",
                "Application Information"
                );
        }

        #endregion

        #region Helper

        // safe: 2019-1-8
        private void OpenPort()
        {
            if (! VM.InitializeSerialPort())
            {
                Debugger.Log("Failed to open: Failed to initialize SerialDevice.", Debugger.LogLevel.ERROR);
                return;
            }

            VM.Opened = true;
            VM.Recieved += (s, m) =>
            {
                string EscPresentation = m.Replace("\t", "\\t").Replace("\r", "\\r").Replace("\n", "\\n");

                Debugger.Log("Recieved \"" + EscPresentation + "\" and newline.", Debugger.LogLevel.DEBUG);
                Application.Current.Dispatcher.Invoke(new Action(() => { AddLine(m, Brushes.Green); }));
            };
        }

        // safe: 2019-1-8
        private void ClosePort()
        {
            if (! VM.DisposePort())
            {
                Debugger.Log("Failed to open: Failed to initialize SerialDevice.", Debugger.LogLevel.ERROR);
                return;
            }

            VM.Opened = false;
        }

        private void SendToPort()
        {
            var message = GetPrompt();

            if (! VM.WriteLine(message))
            {
                Debugger.Log("Failed to write to port.", Debugger.LogLevel.WARN);

                return;
            }
            Debugger.Log("Sent \"" + message + "\" and newline.", Debugger.LogLevel.DEBUG);

            AddLine(message, Brushes.Orange);
            ClearPrompt();
        }

        public void AddLine(string line, SolidColorBrush colorBrush)
        {
            var tp = ContentTextBox.Document.ContentEnd;

            var tr = new TextRange(tp, tp)
            {
                Text = line + '\r'
            };
 
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, colorBrush);

            if (! VM.ScrollLock)
            {
                ContentTextBox.ScrollToEnd();
            }
        }

        private void ClearLogBox()
        {
            ContentTextBox.Document.Blocks.Clear();
        }

        private string GetPrompt()
        {
            return PromptTextBox.Text;
        }

        private void ClearPrompt()
        {
            PromptTextBox.Text = "";
        }

        #endregion
    }
}
