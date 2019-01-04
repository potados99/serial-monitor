using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using serial_monitor.Models;
using serial_monitor.Utils;

namespace serial_monitor.ViewModels
{
    class SerialDeviceViewModel
    {
        public static SerialDeviceViewModel Instance = new SerialDeviceViewModel();

        public bool CreateSerialDevice(string comport)
        {
            try
            {
                SerialDeviceInstance = new SerialDevice(comport);
                PortName = comport;
            }
            catch (Exception)
            {
                Debugger.Log("Failed creating serial device.", Debugger.LogLevel.ERROR);
                return false;
            }

            return true;
        }

        private SerialDevice serialDeviceInstance;
        public SerialDevice SerialDeviceInstance
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

        public int BaudRate
        {
            get
            {
                return SerialDeviceInstance.Port.BaudRate;
            }
            set
            {
                SerialDeviceInstance.Port.BaudRate = value;
            }
        }

        public string NewLine
        {
            get
            {
                return SerialDeviceInstance.Port.NewLine;
            }
            set
            {
                SerialDeviceInstance.Port.NewLine = value;
            }
        }

        public int ReadTimeOut
        {
            get
            {
                return SerialDeviceInstance.Port.ReadTimeout;
            }
            set
            {
                SerialDeviceInstance.Port.ReadTimeout = value;
            }
        }

        public event SerialDevice.RecieveEventHandler Recieved
        {
            add
            {
                SerialDeviceInstance.Recieved += value;
            }
            remove
            {
                SerialDeviceInstance.Recieved -= value;
            }
        }
    }
}
