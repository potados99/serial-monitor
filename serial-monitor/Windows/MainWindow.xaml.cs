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

        private void SendButton_Click(object sender, RoutedEventArgs e)
        {
            var msg = GetPrompt();
            VM.WriteLine(msg);
            AddLine(msg, Brushes.Orange);
            ClearPrompt();
        }

        #endregion

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

        private void PortNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Debugger.Log("HEY!!!!!!", Debugger.LogLevel.DEBUG);
            //VM.RecreateAndOpenSerialPort();
            
        }

        private void BaudRateComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        private void ScrollLockButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
