using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO.Ports;

namespace Balloon_Tracker
{
    public delegate void GotData(string s);

    class SimpleSerial
    {
        public event GotData RaiseGotData;

        SerialPort sp = new SerialPort();
        char[] reject = new char[2];
        string portname = "";

        List<string> messages = new List<string>();

        public override string ToString()
        {
            return portname;
        }

        static public string[] GetPortNames()
        {
            return SerialPort.GetPortNames();
        }

        public void Open(string name, int baud, bool input)
        {
            reject[0] = '\r';
            reject[1] = '\n';

            sp.PortName = name;
            sp.BaudRate = baud;

            sp.DataBits = 8;
            sp.Parity = Parity.None;
            sp.StopBits = StopBits.One;

            if (input)
            {
                sp.Handshake = Handshake.RequestToSend;
            }
            sp.ReceivedBytesThreshold = 10;

            sp.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);
            sp.Open();
            portname = name;
        }

        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            string s = null;

            while (sp.BytesToRead > 0)
            {
                try
                {
                    s = sp.ReadLine();
                }
                catch (Exception)
                {
                }
                if (s != null)
                {
                    if (s.Length > 0)
                    {
                        RaiseGotData(s);
                    }
                }
            }
        }

        public void Close()
        {
            sp.Close();
        }

        public void Write(string data)
        {
            sp.Write(data);
        }
    }
}
