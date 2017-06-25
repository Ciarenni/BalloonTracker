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
    public partial class Form1 : Form
    {
        int linecounter = 0;
        int packetcounter = 0;
        int packetsAfterFilter = 0;

        SerialPortConfig spconf = new SerialPortConfig();

        PacketLogger pl = new PacketLogger();
        AzElFrom2Points aef2p = new AzElFrom2Points();
//        SimpleSerial spIn = new SimpleSerial();
        SimpleSerial spOut = new SimpleSerial();

        //List<string> balloonCallsigns = new List<string>();
        //List<string> groundCrewCallsigns = new List<string>();

        APRSPacket p = new APRSPacket();

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            trackingTargets.Items.Add("KJ4NKE-11");

            recordingTargets.Items.Add("KG4WSV");
            recordingTargets.Items.Add("KG4WSV-11");
            recordingTargets.Items.Add("KG4WSV-12");
            recordingTargets.Items.Add("KG4WSV-13");
            recordingTargets.Items.Add("KG4WSV-14");
            recordingTargets.Items.Add("KJ4NKE-12");
            recordingTargets.Items.Add("KJ4NKE-11");

//            spIn.RaiseGotData += new GotData(spIn_RaiseGotData);

            aef2p.SetBase(34.722445, -86.640799, 183);
//            spIn.Open("COM3", 9600, true);
            spOut.Open("COM4", 9600, false);
            spconf.RaiseGotData += new GotData(spconf_RaiseGotData);
            spconf.Show(this);
            this.FormClosing += new FormClosingEventHandler(Form1_FormClosing);
        }

        void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            spconf.Dispose();
            this.Dispose();
        }

        void spconf_RaiseGotData(string s)
        {
            this.Invoke(new DoProcessData(this.ProcessData), new object[] { s });
        }

        public delegate void DoProcessData(string s);

        /*
        void spIn_RaiseGotData(string s)
        {
            this.Invoke(new DoProcessData(this.ProcessData), new object[]{s});
        }
        */

        void ProcessData(string s)
        {
            Console.WriteLine(s);
            linecounter++;
            label3.Text = linecounter.ToString();
            textBox1.Text = s + "\n" + textBox1.Text;

            if (p.Parse(s))
            {
                label4.Text = packetcounter.ToString();

                // filter for our callsigns
                if (trackingTargets.Items.Contains(p.callsign))
                {
                    packetsAfterFilter++;
                    LogPacket(p);
                    HandleBalloonPacket(p);
                }
                else if (recordingTargets.Items.Contains(p.callsign))
                {
                    packetsAfterFilter++;
                    // got a valid packet, so process it
                    LogPacket(p);
                }
                label5.Text = packetsAfterFilter.ToString();
            }
        }

        private void HandleBalloonPacket(APRSPacket p)
        {
            label7.Text = Math.Round(p.lat, 2).ToString() + ", " + Math.Round(p.lon, 2).ToString() + ", " + Math.Round(p.alt, 0).ToString();
            label11.Text = Math.Round(p.speed, 0).ToString() + ", " + Math.Round(p.heading, 0).ToString();

            if (p.alt > 0)
            {
                // command rotator
                List<double> l = aef2p.CalcForTarget(p.lat, p.lon, p.alt);

                string az = Math.Round(l[0], 0).ToString();
                string el = Math.Round(l[1], 0).ToString();
                string dist = Math.Round(l[2], 0).ToString();

                label9.Text = az + ", " + el + ", " + dist;

                while (az.Length < 3)
                {
                    az = "0" + az;
                }

                while (el.Length < 3)
                {
                    el = "0" + el;
                }

                string cmd = "W" + az + " " + el + "\r\n";
                spOut.Write(cmd);
            }
        }

        private void LogPacket(APRSPacket p)
        {
            pl.LogPacket(p);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            APRSPacket p = new APRSPacket();
            p.Parse(@"KJ4NKE-11b>APT311,W4GPS-7*,WIDE2-1:/151842h3434.82N/08527.22WO093/059/A=024978/ UAH SHC 1 Balloon");
            HandleBalloonPacket(p);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            textBox1.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (trackingTargets.SelectedIndex >= 0)
            {
                trackingTargets.Items.RemoveAt(trackingTargets.SelectedIndex);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (trackingToAdd.Text.Length > 0)
            {
                if(!trackingTargets.Items.Contains(trackingToAdd.Text))
                {
                    trackingTargets.Items.Add(trackingToAdd.Text);
                }
                trackingToAdd.Text = "";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (recordingToAdd.Text.Length > 0)
            {
                if (!recordingTargets.Items.Contains(recordingToAdd.Text))
                {
                    recordingTargets.Items.Add(recordingToAdd.Text);
                }
                recordingToAdd.Text = "";
            }
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
            if (recordingTargets.SelectedIndex >= 0)
            {
                recordingTargets.Items.RemoveAt(recordingTargets.SelectedIndex);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            spconf.Show(this);
        }
    }
}
