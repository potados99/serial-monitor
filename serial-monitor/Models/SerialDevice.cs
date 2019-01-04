using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using serial_monitor.Utils;

namespace serial_monitor.Models
{
    /// <summary>
    /// Usage: initialize and add recieve event handler
    /// </summary>
    public class SerialDevice
    {
        #region Constants

        protected const int READ_TIMEOUT = 1000;

        #endregion

        #region Variables

        private Thread ReadThread;

        #endregion

        #region Properties

        private SerialPort port;
        private SerialPort Port
        {
            get
            {
                if (port == null)
                {
                    Debugger.Log("Cannot get Port: port is null.", Debugger.LogLevel.ERROR);
                }
                return port;
            }
            set
            {
                if (value == null)
                {
                    Debugger.Log("Invalid null pointer to Port.", Debugger.LogLevel.WARN);
                    return;
                }
                port = value;
            }
        }

        private bool Listening { get; set; }

        public event RecieveEventHandler InvokeRecieved;
        public event RecieveEventHandler Recieved
        {
            add
            {
                if (value == null)
                {
                    Debugger.Log("Invalid null value to RecieveEventHandler.");
                    return;
                }
                InvokeRecieved += value;
                StartListen();
            }
            remove
            {
                if (value == null)
                {
                    Debugger.Log("Invalid null value to RecieveEventHandler.");
                    return;
                }
                InvokeRecieved -= value;
            }
        }

        private bool IsOpen
        {
            get
            {
                return (Port != null && Port.IsOpen);
            }
        }

        private bool HasSomethingToRead
        {
            get
            {
                if (Port == null)
                {
                    Debugger.Log("Cannot preread: Port is null.", Debugger.LogLevel.WARN);
                    return false;
                }
                return Port.BytesToRead > 0;
            }
        }

        #endregion

        #region Event

        public delegate void RecieveEventHandler(object sender, string message);

        #endregion

        #region Constructor & Destructor

        public SerialDevice(string portname, int baudrate, string newLine)
        {
            try
            {
                Port = new SerialPort()
                {
                    PortName = portname,
                    BaudRate = baudrate,
                    NewLine = newLine
                };

                Debugger.Log("Created SerialPort on " + portname + ".", Debugger.LogLevel.INFO);

                Port.Open();

                Debugger.Log("Opened SerialPort on " + portname + ".", Debugger.LogLevel.INFO);

                ReadThread = new Thread(new ThreadStart(ListenLoop));
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

                throw new System.ArgumentException("[처리됨] 포트 이름이 올바르지 않거나 사용 가능한 포트가 없습니다. portForUse: [" + (String.IsNullOrEmpty(portname) ? "null" : portname) + "]");
            }

            catch (System.UnauthorizedAccessException)
            {
                // Can't open port
                Debugger.Log("Port open failed due to UnauthorizedAccessException.", Debugger.LogLevel.FATAL);

                throw new System.UnauthorizedAccessException("[처리됨] 허가되지 않은 접근입니다.");
            }
            catch (Exception all)
            {
                Debugger.Log("Unhandled error", Debugger.LogLevel.FATAL);
            }
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
                    InvokeRecieved?.Invoke(this, ReadLine());
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
            if (Listening)
            {
                Debugger.Log("ReadThread already running.", Debugger.LogLevel.INFO);
                return;
            }

            ReadThread.Start();
            Listening = true;
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

        public void Dispose()
        {
            EndListen();
            Debugger.Log("Listen stopped", Debugger.LogLevel.INFO);

            Close();
        }

        public bool Close()
        {
            if (IsOpen)
            {
                Port.Close();

                Debugger.Log("Port " + Port.PortName + " closed.", Debugger.LogLevel.INFO);

                return true;
            }

            else
            {
                Debugger.Log("Failed to close port.", Debugger.LogLevel.INFO);

                return false;
            }
        }

        #endregion
    }
}
