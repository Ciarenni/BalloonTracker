using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Text.RegularExpressions;

namespace Balloon_Tracker
{
    class APRSPacket
    {
        public string callsign;
        public double lat;
        public double lon;
        public double alt;
        public double heading;
        public double speed;
        public DateTime time;
        public string packetTime;
        public string otherdata;
        public string rawpacket;

        public bool Parse(string s)
        {
            MatchCollection mc = Regex.Matches(s, @"^([^>]+)>[^:]+:/([0-9]+)h([0-9.]+)([NS])/([0-9.]+)([EW])[Oo]([0-9]+)/([0-9]+)/A=([0-9]+)(.*)");
            if (mc.Count == 1)
            {
                foreach(Match m in mc)
                {
                    time = DateTime.Now;
                    callsign = m.Groups[1].Value;
                    packetTime = ParseTime(m.Groups[2].Value);
                    lat = ParseLatLon(m.Groups[3].Value, m.Groups[4].Value);
                    lon = ParseLatLon(m.Groups[5].Value, m.Groups[6].Value);
                    heading = ParseHeading(m.Groups[7].Value);
                    speed = ParseSpeed(m.Groups[8].Value);
                    alt = ParseAlt(m.Groups[9].Value);
                    otherdata = m.Groups[10].Value;
                    rawpacket = s;

                    return true;
                }
            }
            mc = Regex.Matches(s, @"^([^>]+)>[^:]+:!([0-9.]+)([NS])/([0-9.]+)([EW])[Oo]([0-9]+)/([0-9]+)/A=([0-9]+)(.*)");
            if (mc.Count == 1)
            {
                foreach (Match m in mc)
                {
                    time = DateTime.Now;
                    callsign = m.Groups[1].Value;
//                    packetTime = ParseTime(m.Groups[2].Value);
                    lat = ParseLatLon(m.Groups[2].Value, m.Groups[3].Value);
                    lon = ParseLatLon(m.Groups[4].Value, m.Groups[5].Value);
                    heading = ParseHeading(m.Groups[6].Value);
                    speed = ParseSpeed(m.Groups[7].Value);

                    // hack to fix broken alt string
                    //string astring = m.Groups[8].Value;
                    //astring = astring.Substring(0, astring.Length - 2);
                    //alt = ParseAlt(astring);

                    alt = ParseAlt(m.Groups[8].Value);
                    otherdata = m.Groups[9].Value;
                    rawpacket = s;

                    return true;
                }
            }
            return false;
        }

        private string ParseTime(string s)
        {
            int hour = 0;
            int minute = 0;
            int second = 0;

            if(s.Length == 6)
            {
                hour = int.Parse(s.Substring(0,2));
                minute = int.Parse(s.Substring(2, 2));
                second = int.Parse(s.Substring(4, 2));
            }

            return hour + ":" + minute + ":" + second;
        }

        private double ParseLatLon(string s, string dir)
        {
            double ret = 0;

            if (s.Length > 6)
            {
                string a;
                string b;

                int i = s.Length - 5;

                a = s.Substring(0, i);
                b = s.Substring(i, 5);

                double sec;
                double whole;

                if (double.TryParse(a, out whole))
                {
                    if (double.TryParse(b, out sec))
                    {
                        ret = whole + sec / 60.0;
                    }
                }
            }

            if ((dir == "S") || (dir == "W"))
            {
                ret = -ret;
            }

            return ret;
        }

        private double ParseHeading(string s)
        {
            double ret = 0;
            double.TryParse(s, out ret);
            return ret;
        }

        private double ParseSpeed(string s)
        {
            double ret = 0;
            double.TryParse(s, out ret);
            return ret * 0.51444444; // convert from knots to m/s
        }

        private double ParseAlt(string s)
        {
            double ret = 0;
            double.TryParse(s, out ret);
            return ret * 0.3048; // convert from feet to meters
        }

        public override string ToString()
        {
            return time.ToString() + " " + callsign + " " + lat.ToString() + " " + lon.ToString() + " " + alt.ToString() + " " + heading.ToString() + " " + speed.ToString() + " " + otherdata;
        }
    }
}
