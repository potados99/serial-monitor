using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Linq;
using System.Text;
using serial_monitor.Models;
using serial_monitor.Utils;
using System.Windows;

namespace serial_monitor.ViewModels
{
    class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Public singleton instance
        /// </summary>
        public static MainViewModel Instance = new MainViewModel();

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string propname)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propname));
        }

        #endregion

        #region SerialPort

        #region Constants

        private const int DEFAULT_BAUDRATE = 9600;

        #endregion

        #region Properties

        private SerialDevice serialDeviceInstance;
        private SerialDevice SerialDeviceInstance
        {
            get
            {
                if (serialDeviceInstance == null)
                {
                    Debugger.Log("Cannot get SerialDeviceInstance: serialDeviceInstance not set.", Debugger.LogLevel.WARN);
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

        // Port name
        public string[] AvailablePortNames => SerialPort.GetPortNames();
        public string PortName { get; set; } = SerialPort.GetPortNames().Last();

        // Baudrate
        public int[] AvailableBaudRates { get; } = { 9600, 115200 };
        public int BaudRate { get; set; } = DEFAULT_BAUDRATE;

        // New Line
        public string[] AvailableNewLines { get; } = { "CR", "LF", "CR+LF" };
        private string[] NewLinesValueArray { get; } = { "\r", "\n", "\r\n" };
        public int SelectedNewLineIndex { get; set; } = 1; // LF: \n
        public string NewLine => NewLinesValueArray[SelectedNewLineIndex];

        #endregion

        #region Methods

        // exception-safe: 2019-1-8
        // safety dependent on: SerialDevice
        public bool InitializeSerialPort()
        {
            if (SerialDeviceInstance != null)
            {
                Debugger.Log("SerialDeviceInstance already exists. Creating new one.", Debugger.LogLevel.INFO);
            }
            if (String.IsNullOrEmpty(PortName) || String.IsNullOrEmpty(NewLine))
            {
                MessageBox.Show("Fill options.");
                return false;
            }

            try
            {
                SerialDeviceInstance = new SerialDevice(PortName, BaudRate, NewLine);
                return true;
            }
            catch (ArgumentException e)
            {
                Debugger.Log("Failed initializing SerialDevice due to ArgumentException: \n" + e.ToString(), Debugger.LogLevel.ERROR);
                MessageBox.Show("Wrong argument.");
                return false;
            }
            catch (Exception others)
            {
                Debugger.Log("Failed initializing SerialDevice: \n" + others.ToString(), Debugger.LogLevel.ERROR);
                MessageBox.Show("Failed creating SerialDevice instance.");
                return false;
            }
        }

        // null-safe & exception-safe: 2019-1-8
        // safety depent on: SerialDevice.WriteLine
        public bool WriteLine(string message)
        {
            if (SerialDeviceInstance == null)
            {
                Debugger.Log("Failed to write: SerialDeviceInstance in null.", Debugger.LogLevel.WARN);
                return false;
            }

            return SerialDeviceInstance.WriteLine(message);
        }

        // null-safe & exception-safe: 2019-1-8
        // safety depent on: SerialDevice.Dispose
        public bool DisposePort()
        {
            if (SerialDeviceInstance == null)
            {
                Debugger.Log("Failed to dispose port: SerialDeviceInstance in null.", Debugger.LogLevel.WARN);
                return false;
            }

            return SerialDeviceInstance.Dispose();
        }

        #endregion

        #endregion

        #region View

        public bool ScrollLock { get; set; } = false;

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
