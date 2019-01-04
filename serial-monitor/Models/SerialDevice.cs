using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using serial_monitor.Utils;

namespace serial_monitor.Models
{
    public class SerialDevice
    {
        #region Constants

        protected const int READ_TIMEOUT = 1000;

        #endregion

        #region Variables

        private Thread ReadThread;

        #endregion

        #region Properties

        private SerialPort Port { get; set; }

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
                Port.PortName = value;
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
                Port.BaudRate = value;
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
                Port.NewLine = value;
            }
        }

        private event RecieveEventHandler InvokeRecievedEventHandler;
        public event RecieveEventHandler Recieved
        {
            add
            {
                InvokeRecievedEventHandler += value;
                Debugger.Log("RecievedEventHandler added.", Debugger.LogLevel.INFO);
            }
            remove
            {
                InvokeRecievedEventHandler -= value;
                Debugger.Log("RecievedEventHandler removed.", Debugger.LogLevel.INFO);
            }
        }


        private bool IsOpen
        {
            get
            {
                return (this.Port != null && this.Port.IsOpen);
            }
        }

        private bool HasSomethingToRead
        {
            get
            {
                return Port.BytesToRead > 0;
            }
        }

        #endregion

        #region Event

        public delegate void RecieveEventHandler(object sender, string message);

        #endregion

        #region Constructor & Destructor

        public SerialDevice(string comport)
        {
            CreateAndOpenPort(comport);
        }

        ~SerialDevice() {
            Dispose();
        }

        #endregion

        #region Write & Read

        public void WriteLine(string payload)
        {
            Port.WriteLine(payload);
        }

        public string ReadLine()
        {
            return Port.ReadLine();
        }

        private void ListenLoop()
        {
            while(true)
            {
                if (HasSomethingToRead)
                {
                    InvokeRecievedEventHandler(this, ReadLine());
                }
            }
        }

        #endregion

        #region Read Thread Control

        private void StartListen()
        {
            if (ReadThread == null)
            {
                Debugger.Log("ReadThread is null.", Debugger.LogLevel.WARN);
                return;
            }

            ReadThread.Start();
        }

        private void EndListen()
        {
            if (ReadThread == null)
            {
                Debugger.Log("ReadThread is null.", Debugger.LogLevel.WARN);
                return;
            }

            ReadThread.Abort();
        }

        #endregion

        #region Port Control

        public void CreateAndOpenPort(string portname)
        {
            try
            {
                Port = new SerialPort()
                {
                    PortName = PortName,
                    BaudRate = BaudRate,
                    NewLine = NewLine
                };

                Debugger.Log("Created SerialPort on " + portname + ".", Debugger.LogLevel.INFO);

                Port.Open();

                Debugger.Log("Opened SerialPort on " + portname + ".", Debugger.LogLevel.INFO);

                ReadThread = new Thread(new ThreadStart(ListenLoop));

                StartListen();
            }

            catch (System.IO.IOException)
            {
                // No port
                Debugger.Log("Port open failed due to IOException.", Debugger.LogLevel.FATAL);

                throw new System.IO.IOException("[처리됨] 포트를 열지 못했습니다.");
            }

            catch (System.ArgumentException)
            {
                // Wrong port name
                Debugger.Log("Port open failed due to ArgumentException.", Debugger.LogLevel.FATAL);

                throw new System.ArgumentException("[처리됨] 포트 이름이 올바르지 않거나 사용 가능한 포트가 없습니다. portForUse: [" + (String.IsNullOrEmpty(Port.PortName) ? "null" : Port.PortName) + "]");
            }

            catch (System.UnauthorizedAccessException)
            {
                // Can't open port
                Debugger.Log("Port open failed due to UnauthorizedAccessException.", Debugger.LogLevel.FATAL);

                throw new System.UnauthorizedAccessException("[처리됨] 허가되지 않은 접근입니다.");
            }
        }

        public void RecreateAndOpenPort()
        {
            Debugger.Log("Will Dispose port " + PortName, Debugger.LogLevel.INFO);
            Dispose();
            Debugger.Log("Did Dispose port " + PortName, Debugger.LogLevel.INFO);

            CreateAndOpenPort(PortName);
        }

        public void Dispose()
        {
            EndListen();
            Debugger.Log("Listen stopped on " + PortName, Debugger.LogLevel.INFO);

            Close();
        }

        public bool Close()
        {
            if (IsOpen)
            {
                this.Port.Close();

                Debugger.Log("Port " + Port.PortName + " closed.", Debugger.LogLevel.INFO);

                return true;
            }

            else
            {
                Debugger.Log("Failed to close port " + Port.PortName + ".", Debugger.LogLevel.INFO);

                return false;
            }
        }

        #endregion
    }
}
