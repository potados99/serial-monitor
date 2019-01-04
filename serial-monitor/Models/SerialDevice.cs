using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using serial_monitor.Utils;

namespace serial_monitor.Models
{
    public class SerialDevice
    {
        #region Constants

        protected const int READ_TIMEOUT = 1000;

        #endregion

        #region Variables

        #endregion

        #region Properties

        public SerialPort Port { get; set; }

        public bool IsOpen
        {
            get
            {
                return (this.Port != null && this.Port.IsOpen);
            }
        }

        #endregion

        public SerialDevice(string comport)
        {

            try
            {
                Port = new SerialPort(comport);

                Debugger.Log("Created SerialPort on " + comport + ".", Debugger.LogLevel.INFO);

                Port.Open();

                Debugger.Log("Opened SerialPort on " + comport + ".", Debugger.LogLevel.INFO);
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

                throw new System.ArgumentException("[처리됨] 포트 이름이 올바르지 않거나 사용 가능한 포트가 없습니다. portForUse: [" + (String.IsNullOrEmpty(PortName) ? "null" : PortName) + "]");
            }

            catch (System.UnauthorizedAccessException)
            {
                // Can't open port
                Debugger.Log("Port open failed due to UnauthorizedAccessException.", Debugger.LogLevel.FATAL);

                throw new System.UnauthorizedAccessException("[처리됨] 허가되지 않은 접근입니다.");
            }
        }

        /// <summary>
        /// 데이터를 보냅니다.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="read"></param>
        /// <returns></returns>
        protected string Send(string data, bool read = false)
        {

            long startTime = DateTime.Now.Millisecond;

            if (startTime - LastSentTime < 600)
            {
                Debugger.Log("Attempting too frequently!", Debugger.LogLevel.WARN);

                return null;
            }

            LastSentTime = DateTime.Now.Ticks;

            Debugger.Log("Assigned LastSentTime.", Debugger.LogLevel.TRACE);

            string recieved = null;

            try
            {
                Port.WriteLine(data);

                Debugger.Log("Successfully Sent " + data + " to port " + PortName + " successfully.", Debugger.LogLevel.INFO);

                if (read)
                {
                    recieved = Port.ReadLine(); /* read until NewLine arrives */

                    Debugger.Log("Successfully Read " + data + " from port " + PortName + " successfully", Debugger.LogLevel.INFO);

                    if (string.IsNullOrEmpty(recieved))
                    {
                        Debugger.Log("Recieved data is empty.", Debugger.LogLevel.WARN);

                        return null; /* if not recieved expected data */
                    }
                }

                Debugger.Log("Returning recieved data.", Debugger.LogLevel.TRACE);

                return recieved;
            }
            catch (System.NullReferenceException)
            {
                Debugger.Log("Send failed due to NullReferenceException.", Debugger.LogLevel.FATAL);

                throw new System.NullReferenceException("[처리됨] 객체 참조가 유효하지 않습니다.");
            }
            catch (System.InvalidOperationException)
            {
                // Port is not opened
                Debugger.Log("Send failed due to InvalidOperationException.", Debugger.LogLevel.FATAL);

                throw new System.InvalidOperationException("[처리됨] 포트가 닫혀있습니다.");
            }
            catch (System.TimeoutException)
            {
                // Read time out
                Debugger.Log("Send failed due to TimeoutException.", Debugger.LogLevel.FATAL);

                throw new System.TimeoutException("[처리됨] 장비가 응답하지 않습니다.");
            }
            catch (System.UnauthorizedAccessException)
            {
                Debugger.Log("Send failed due to UnauthorizedAccessException.", Debugger.LogLevel.FATAL);

                throw new System.UnauthorizedAccessException("[처리됨] 장비가 응답하지 않습니다.");
            }
            catch (System.IO.IOException)
            {
                Debugger.Log("Send failed due to IOException.", Debugger.LogLevel.FATAL);

                throw new System.IO.IOException("[처리됨] 부착된 장비가 응답하지 않습니다.");
            }
            catch (Exception e)
            {
                Debugger.Log("Send failed due to unhandled exception.", Debugger.LogLevel.FATAL);

                Console.WriteLine(e.ToString());

                throw new Exception("[처리되지 않음] 예외입니다.");
            }

        }

        /// <summary>
        /// 포트를 닫습니다. 불가능할 경우 false를 반환합니다.
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            if (this.Port != null && this.Port.IsOpen)
            {
                this.Port.Close();

                Debugger.Log("Port " + PortName + " closed.", Debugger.LogLevel.INFO);

                return true;
            }

            else
            {
                Debugger.Log("Failed to close port " + PortName + ".", Debugger.LogLevel.INFO);

                return false;
            }
        }
    }
