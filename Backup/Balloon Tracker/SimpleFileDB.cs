using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace Balloon_Tracker
{
    class SimpleFileDB : SimpleDBInterface
    {
        List<APRSPacket> log = new List<APRSPacket>();

        public override bool Initialize(string initstring)
        {
            // generate the loader kml file
            TextWriter tw = new StreamWriter("Loader.kml");
            tw.Write(@"<?xml version='1.0' encoding='UTF-8'?>
<kml xmlns = 'http://earth.google.com/kml/2.1'>
  <Folder>
    <NetworkLink>
      <Link>
        <href>data.kml</href>
        <refreshMode>onInterval</refreshMode>
        <refreshInterval>60</refreshInterval>
      </Link>
    </NetworkLink>
  </Folder>
</kml>");
            tw.Close();

            return true;
        }

        public override void LogPacket(APRSPacket p)
        {
            log.Add(p);
            // regenerate the kml file

            // generate the loader kml file
            TextWriter tw = new StreamWriter("data.kml");
            tw.Write(@"<?xml version=""1.0"" encoding=""UTF-8""?>
<kml xmlns=""http://www.opengis.net/kml/2.2"">
  <Document>
    <name>Paths</name>
    <description>Examples of paths. Note that the tessellate tag is by default
      set to 0. If you want to create tessellated lines, they must be authored
      (or edited) directly in KML.
      </description>
    <Style id=""yellowLineGreenPoly"">
      <LineStyle>
        <color>7f00ffff</color>
        <width>4</width>
      </LineStyle>
      <PolyStyle>
        <color>7f00ff00</color>
      </PolyStyle>
    </Style>
    <Placemark>
      <name>Absolute Extruded</name>
      <description>Transparent green wall with yellow outlines</description>
      <styleUrl>#yellowLineGreenPoly</styleUrl>
      <LineString>
        <extrude>1</extrude>
        <tessellate>1</tessellate>
        <altitudeMode>absolute</altitudeMode>
        <coordinates>");

            foreach (APRSPacket p2 in log)
            {
                tw.WriteLine(p2.lon.ToString() + "," + p2.lat.ToString() + "," + p2.alt.ToString());
            }

            tw.Write(@"</coordinates>
      </LineString>
    </Placemark>
  </Document>
</kml>");

            tw.Close();
        }
    }
}
