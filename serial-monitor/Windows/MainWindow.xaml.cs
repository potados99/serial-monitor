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
using serial_monitor.ViewModels;

namespace serial_monitor.Windows
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var vm = SerialDeviceViewModel.Instance;

            vm.CreateSerialDevice("COM10");
            vm.Recieved += (s, e) =>
            {
                Console.WriteLine("HEY YA");
                Console.WriteLine(e);
            };
        }
    }
}
