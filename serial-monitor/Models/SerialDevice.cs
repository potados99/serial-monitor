using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        #region Properties

        private SerialPort port;
        // null-safe: 2018-1-8
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

        // null-safe & exception-safe: 2018-1-8
        private bool IsOpen
        {
            get
            {
                if (Port == null)
                {
                    Debugger.Log("Failed to get IsOpen: Port is null.", Debugger.LogLevel.WARN);

                    return false;
                }

                try
                {
                    return Port.IsOpen;
                }
                catch (Exception e)
                {
                    Debugger.Log("Failed to get IsOpen: \n" + e.ToString(), Debugger.LogLevel.WARN);

                    return false;
                }
            }
        }

        //null-safe & exception-safe: 2018-1-8
        private bool HasSomethingToRead
        {
            get
            {
                if (Port == null)
                {
                    Debugger.Log("Cannot preread: Port is null.", Debugger.LogLevel.WARN);

                    return false;
                }

                try
                {
                    bool exists = Port.BytesToRead > 0;

                    return exists;
                }
                catch (Exception e)
                {
                    Debugger.Log("Failed to get BytesToRead: \n" + e.ToString(), Debugger.LogLevel.WARN);
                    return false;
                }
            }
        }

        private bool Listening { get; set; } // bound to UI
        private Thread ReadThread { get; set; }

        public event RecieveEventHandler InvokeRecieved;
        // null-safe & exception-safe: 2018-1-8
        public event RecieveEventHandler Recieved
        {
            add
            {
                if (value == null)
                {
                    Debugger.Log("Putting Invalid null value to RecieveEventHandler.");

                    return;
                }
                InvokeRecieved += value;

                if (! StartListen())
                {
                    Debugger.Log("Failed to start listening.", Debugger.LogLevel.WARN);
                }
            }
            remove
            {
                if (value == null)
                {
                    Debugger.Log("Invalid null value to RecieveEventHandler.");

                    return;
                }
                if (InvokeRecieved == null)
                {
                    Debugger.Log("Cannot remove Recieved: InvokeRecieved is null.", Debugger.LogLevel.WARN);

                    return;
                }

                try
                {
                    InvokeRecieved -= value;
                }
                catch (Exception e)
                {
                    Debugger.Log("Failed to remove event handler: \n" + e.ToString(), Debugger.LogLevel.WARN);

                    return;
                }
            }
        }

        #endregion

        #region Event Delegate

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

                Debugger.Log("Created SerialPort at " + portname + ".", Debugger.LogLevel.INFO);

                Port.Open();

                Debugger.Log("Opened SerialPort at " + portname + ".", Debugger.LogLevel.INFO);

                ReadThread = new Thread(new ThreadStart(ListenLoop));
            }

            catch (System.IO.IOException)
            {
                // No port
                Debugger.Log("Port open failed due to IOException.", Debugger.LogLevel.FATAL);

                throw new System.IO.IOException("[처리됨] 포트를 열지 못했습니다.");
            }

            catch (System.ArgumentNullException)
            {
                // Wrong ThreadStart argument
                Debugger.Log("Failed to initialize ReadThread due to ArgumentNullException.", Debugger.LogLevel.FATAL);

                throw new System.ArgumentException("[처리됨] 스레드를 초기화하지 못했습니다.");
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
                Debugger.Log(all.ToString(), Debugger.LogLevel.FATAL);
            }
        }

        #endregion

        #region Write & Read

        // null-safe & exception-safe: 2019-1-8
        public bool WriteLine(string payload)
        {
            if (Port == null)
            {
                Debugger.Log("Failed to write to port: Port is null.", Debugger.LogLevel.WARN);
                return false;
            }

            try
            {
                Port.WriteLine(payload);
                return true;
            }
            catch (Exception e)
            {
                Debugger.Log("Failed to write to port: \n" + e.ToString(), Debugger.LogLevel.WARN);
                return false;
            }
        }

        // null-safe & exception-safe: 2019-1-8
        public string ReadLine()
        {
            if (Port == null)
            {
                Debugger.Log("Failed to read from port: Port is null.", Debugger.LogLevel.WARN);
                return null;
            }

            try
            {
                return Port.ReadLine();
            }
            catch (Exception e)
            {
                Debugger.Log("Failed to read from port: \n" + e.ToString(), Debugger.LogLevel.WARN);
                return null;
            }
        }

        // null-safe & exception-safe: 2019-1-8
        // safety dependent on: HasSomethingToRead, ReadLine
        private void ListenLoop()
        {
            while(true)
            {
                Thread.Sleep(1);

                if (HasSomethingToRead)
                {
                    string recieved = ReadLine();
                    string EscPresentation = recieved.Replace("\t", "\\t").Replace("\r", "\\r").Replace("\n", "\\n");

                    Debugger.Log("Recieved \"" + EscPresentation + "\".", Debugger.LogLevel.DEBUG);
                    InvokeRecieved?.Invoke(this, recieved);
                }
            }
        }

        #endregion

        #region Read Thread Control

        // null-safe & exception-safe: 2019-1-8
        private bool StartListen()
        {
            if (ReadThread == null)
            {
                Debugger.Log("ReadThread is null.", Debugger.LogLevel.WARN);
                return false;
            }
            if (Listening)
            {
                Debugger.Log("ReadThread already running.", Debugger.LogLevel.INFO);
                return false;
            }

            try
            {
                ReadThread.Start();

                Debugger.Log("Successfully started listening.", Debugger.LogLevel.INFO);

                Listening = true;
                return true;
            }
            catch (Exception e)
            {
                Debugger.Log("Exception while starting ReadThread: \n" + e.ToString(), Debugger.LogLevel.WARN);
                return false;
            }
        }

        // null-safe & exception-safe: 2019-1-8
        private bool EndListen()
        {
            // catch null here.
            if (ReadThread == null)
            {
                Debugger.Log("ReadThread is null.", Debugger.LogLevel.WARN);
                return false;
            }

            // catch other exceptions here.
            try
            {
                ReadThread.Abort();
                ReadThread = null;

                Debugger.Log("Successfully finished listening.", Debugger.LogLevel.INFO);

                Listening = false;
                return true;
            }
            catch (Exception e)
            {
                Debugger.Log("Exception while aborting thread: \n" + e.ToString(), Debugger.LogLevel.ERROR);
                return false;
            }
        }

        #endregion

        #region Port Control

        // null-safe & exception-safe: 2019-1-8
        // safety is dependent on: EndListen, Close
        public bool Dispose()
        {
            if (! EndListen())
            {
                Debugger.Log("Failed to end listening.", Debugger.LogLevel.WARN);
                return false;
            }
            Debugger.Log("Listen stopped", Debugger.LogLevel.INFO);

            if (! Close())
            {
                Debugger.Log("Failed to close port.", Debugger.LogLevel.WARN);
                return false;
            }
            Debugger.Log("Closed port", Debugger.LogLevel.INFO);

            Debugger.Log("Dispose successfull.", Debugger.LogLevel.INFO);

            return true;
         }

        // null-safe & exception-safe: 2019-1-8
        // safety dependent on: IsOpen, Port.Close
        public bool Close()
        {
            if (IsOpen)
            {
                try
                {
                    Port.Close();
                    Debugger.Log("Port closed at " + Port.PortName, Debugger.LogLevel.INFO);
                    return true;
                }
                catch (Exception e)
                {
                    Debugger.Log("Failed to close port: \n" + e.ToString(), Debugger.LogLevel.ERROR);
                    return false;
                }
            }

            else
            {
                Debugger.Log("Failed to close port.", Debugger.LogLevel.WARN);
                return false;
            }
        }

        #endregion
    }
}
