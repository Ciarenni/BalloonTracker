using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Balloon_Tracker
{
    public partial class SerialPortConfig : Form
    {
        public event GotData RaiseGotData;
        List<SimpleSerial> ports = new List<SimpleSerial>();

        public SerialPortConfig()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comports.SelectedIndex >= 0)
            {
                if(!activePorts.Items.Contains(comports.SelectedItem.ToString()))
                {
                    SimpleSerial ss = new SimpleSerial();

                    int br;
                    if (int.TryParse(baudRate.Text, out br))
                    {
                        ss.RaiseGotData += new GotData(ss_RaiseGotData);
                        ss.Open(comports.SelectedItem.ToString(), br, true);
                        activePorts.Items.Add(ss);
                    }
                }
            }
        }

        void ss_RaiseGotData(string s)
        {
            RaiseGotData(s);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (activePorts.SelectedIndex >= 0)
            {
                SimpleSerial ss = activePorts.SelectedItem as SimpleSerial;
                ss.Close();
                activePorts.Items.RemoveAt(activePorts.SelectedIndex);
            }
        }

        private void SerialPortConfig_Load(object sender, EventArgs e)
        {
            baudRate.Text = "9600";
            string[] ports = SimpleSerial.GetPortNames();
            foreach (string port in ports)
            {
                comports.Items.Add(port);
            }

            this.FormClosing += new FormClosingEventHandler(SerialPortConfig_FormClosing);
        }

        void SerialPortConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}
