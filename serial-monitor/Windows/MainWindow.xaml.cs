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
using System.Windows.Navigation;
using System.Windows.Shapes;
using serial_monitor.Utils;
using serial_monitor.ViewModels;

namespace serial_monitor.Windows
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Variables

        MainViewModel VM = MainViewModel.Instance;

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
        }

        #endregion

        #region Button Click

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            VM.Opened = true;
            VM.InitializeSerialPort();
            VM.Recieved += (s, m) =>
            {
                Application.Current.Dispatcher.Invoke(new Action(() => { AddLine(m, Brushes.Green); }));
            };
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            VM.Opened = false;
            VM.DisposePort();
        }

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var msg = GetPrompt();
            VM.WriteLine(msg);
            AddLine(msg, Brushes.Orange);
            ClearPrompt();
        }

        #endregion

        #region Helper

        public void AddLine(string line, SolidColorBrush colorBrush)
        {
            var tp = ContentTextBox.Document.ContentEnd;
            var tr = new TextRange(tp, tp)
            {
                Text = line + VM.NewLine
            };

            tr.ApplyPropertyValue(TextElement.ForegroundProperty, colorBrush);

            ContentTextBox.ScrollToEnd();
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
