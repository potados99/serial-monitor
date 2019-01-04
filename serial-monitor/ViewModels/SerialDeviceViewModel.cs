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

            var p = SerialDeviceInstance.Port;

            p.BaudRate = BaudRate;
            p.NewLine = NewLine;
            p.ReadTimeout = ReadTimeOut;

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

        private string newLine = "\n";
        public string NewLine
        {
            get
            {
                return newLine;
            }
            set
            {
                newLine = value;
            }
        }

        private int readTimeOut = 1000;
        public int ReadTimeOut
        {
            get
            {
                return readTimeOut;
            }
            set
            {
                readTimeOut = value;
            }
        }
    }
}
