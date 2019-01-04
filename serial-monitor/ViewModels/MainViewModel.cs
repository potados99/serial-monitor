using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using serial_monitor.Models;
using serial_monitor.Utils;

namespace serial_monitor.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Public singleton instance
        /// </summary>
        public static MainViewModel Instance = new MainViewModel();

        #region Interface

        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string propname)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propname));
            }
        }

        #endregion

        #region SerialPort

        #region Properties

        private SerialDevice serialDeviceInstance;
        private SerialDevice SerialDeviceInstance
        {
            get
            {
                if (serialDeviceInstance == null)
                {
                    Debugger.Log("Cannot get SerialDeviceInstance: serialDeviceInstance not set.", Debugger.LogLevel.ERROR);
                }
                return serialDeviceInstance;
            }
            set
            {
                if (value == null)
                {
                    Debugger.Log("Invalid null pointer to SerialDeviceInstance.", Debugger.LogLevel.WARN);
                    return;
                }
                serialDeviceInstance = value;
            }
        }

        public event SerialDevice.RecieveEventHandler Recieved
        {
            add
            {
                if (SerialDeviceInstance == null)
                {
                    Debugger.Log("Cannot add RecieveEventHandler: SerialDeviceInstance is null.", Debugger.LogLevel.WARN);
                    return;
                }
                SerialDeviceInstance.Recieved += value;
            }
            remove
            {
                if (SerialDeviceInstance == null)
                {
                    Debugger.Log("Cannot remove RecieveEventHandler: SerialDeviceInstance is null.", Debugger.LogLevel.WARN);
                    return;
                }
                SerialDeviceInstance.Recieved -= value;
            }
        }

        public string[] AvailablePortNames => SerialPort.GetPortNames();
        private string portName;
        public string PortName
        {
            get
            {
                return portName;
            }
            set
            {
                portName = value;
            }
        }

        public int[] AvailableBaudRates
        {
            get
            {
                int[] baudRateArray = { 9600, 115200};
                return baudRateArray;
            }
        }
        private int baudRate = 9600;
        public int BaudRate
        {
            get
            {
                return baudRate;
            }
            set
            {
                baudRate = value;
            }
        }

        public string[] AvailableNewLines
        {
            get
            {
                string[] array = { "CR", "LF", "CR+LF" };
                return array;
            }
        }
        private string[] newLinesValueArray = { "\r", "\n", "\r\n" };
        public int SelectedNewLineIndex { get; set; } = 1;
        public string NewLine => newLinesValueArray[SelectedNewLineIndex];

        #endregion

        #region Methods

        public void InitializeSerialPort()
        {
            SerialDeviceInstance = new SerialDevice(PortName, BaudRate, NewLine);
        }

        public void WriteLine(string message)
        {
            SerialDeviceInstance.WriteLine(message);
        }

        public void DisposePort()
        {
            SerialDeviceInstance.Dispose();
        }

        #endregion

        #endregion

        #region View

        private bool scrollLock = false;
        public bool ScrollLock
        {
            get
            {
                return scrollLock;
            }
            set
            {
                scrollLock = value;
            }
        }

        private bool opened = false;
        public bool Opened
        {
            get
            {
                return opened;
            }
            set
            {
                opened = value;
                Notify("Opened");
                Notify("Closed");
                Notify("PortNameComboBoxEnabled");
                Notify("BaudRateComboBoxEnabled");
                Notify("NewLineComboBoxEnabled");
            }
        }

        public bool Closed => !Opened;

        public bool PortNameComboBoxEnabled => (opened ? false : true); // enable only when port is closed.

        public bool BaudRateComboBoxEnabled => (opened ? false : true);

        public bool NewLineComboBoxEnabled => (opened ? false : true);

        #endregion
    }
}
