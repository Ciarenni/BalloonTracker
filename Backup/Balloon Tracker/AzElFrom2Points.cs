using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Balloon_Tracker
{
    class AzElFrom2Points
    {
        // alt in meters
        public void SetBase(double lat, double lon, double alt)
        {
            baseLat = lat;
            baseLon = lon;
            baseAlt = alt;
        }

        // alt in meters
        public List<double> CalcForTarget(double lat, double lon, double alt)
        {
            List<double> ret = new List<double>();

            double z = alt - baseAlt;
            double y = 112171.277 * (lat - baseLat);
            double x = 112171.277 * (lon - baseLon) * Math.Cos(baseLat/57.3);

            double heading = 90 - Math.Atan2(y, x) * 180 / Math.PI;

            double dist = Math.Sqrt(x * x + y * y);
            //double ele = 90 + Math.Atan2(z, dist) * 180 / Math.PI;
            double ele = Math.Atan2(z, dist) * 180 / Math.PI;

            if (heading < 0)
            {
                heading += 360;
            }

            if (ele < 0)
            {
                ele = 0;
            }

            ret.Add(heading);
            ret.Add(ele);
            ret.Add(dist);

            return ret;
        }

        private double baseLat;
        private double baseLon;
        private double baseAlt;
    }
}
