using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using serial_monitor.Models;
using serial_monitor.Utils;

namespace serial_monitor.ViewModels
{
    class MainViewModel
    {
        /// <summary>
        /// Public singleton instance
        /// </summary>
        public static MainViewModel Instance = new MainViewModel();


        #region SerialPort

        #region Properties

        private SerialDevice serialDeviceInstance;
        private SerialDevice SerialDeviceInstance
        {
            get
            {
                if (serialDeviceInstance == null)
                {
                    Debugger.Log("SerialDeviceInstance not set.", Debugger.LogLevel.WARN);
                    throw new NullReferenceException("SerialDeviceInstance not set.");
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
                }
                SerialDeviceInstance.Recieved += value;
            }
            remove
            {
                if (SerialDeviceInstance == null)
                {
                    Debugger.Log("Cannot remove RecieveEventHandler: SerialDeviceInstance is null.", Debugger.LogLevel.WARN);
                }
                SerialDeviceInstance.Recieved -= value;
            }
        }

        public string PortName
        {
            get
            {
                if (SerialDeviceInstance == null)
                {
                    Debugger.Log("Cannot get PortName: SerialDeviceInstance is null.", Debugger.LogLevel.WARN);
                }
                return SerialDeviceInstance.PortName;
            }
            set
            {
                if (SerialDeviceInstance == null)
                {
                    Debugger.Log("Cannot set PortName: SerialDeviceInstance is null.", Debugger.LogLevel.WARN);
                }
                SerialDeviceInstance.PortName = value;

                SerialDeviceInstance.RecreateAndOpenPort();
            }
        }

        public int BaudRate
        {
            get
            {
                if (SerialDeviceInstance == null)
                {
                    Debugger.Log("Cannot get BaudRate: SerialDeviceInstance is null.", Debugger.LogLevel.WARN);
                }
                return SerialDeviceInstance.BaudRate;
            }
            set
            {
                if (SerialDeviceInstance == null)
                {
                    Debugger.Log("Cannot set BaudRate: SerialDeviceInstance is null.", Debugger.LogLevel.WARN);
                }
                SerialDeviceInstance.BaudRate = value;
            }
        }

        public string NewLine
        {
            get
            {
                if (SerialDeviceInstance == null)
                {
                    Debugger.Log("Cannot get NewLine: SerialDeviceInstance is null.", Debugger.LogLevel.WARN);
                }
                return SerialDeviceInstance.NewLine;
            }
            set
            {
                if (SerialDeviceInstance == null)
                {
                    Debugger.Log("Cannot set NewLine: SerialDeviceInstance is null.", Debugger.LogLevel.WARN);
                }
                SerialDeviceInstance.NewLine = value;
            }
        }


        public string[] AvailablePortNames
        {
            get
            {
                return SerialPort.GetPortNames();
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

        #endregion


        #region Methods

        public void RecreateAndOpenSerialPort()
        {
            SerialDeviceInstance.RecreateAndOpenPort();
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

    }
}
