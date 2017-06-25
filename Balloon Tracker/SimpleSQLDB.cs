using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MySql.Data.MySqlClient;

namespace Balloon_Tracker
{
    class SimpleSQLDB : SimpleDBInterface
    {
        MySqlConnection connection;

        public override bool Initialize(string initstring)
        {
            try
            {
                connection = new MySqlConnection("Server=127.0.0.1;Database=shc_tracking;Uid=shcrecorder;Pwd=39gejrn3t9ydf;");
                connection.Open();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override void LogPacket(APRSPacket p)
        {
            if (p.packetTime == null)
            {
                p.packetTime = "";
            }
            MySqlCommand command = connection.CreateCommand();
            command.CommandText = "INSERT INTO trackpoint (timetag, latitude, longitude, altitude, callsign, heading, speed, otherData, packettimetag, rawpacket) VALUES (?timetag, ?lat, ?lon, ?alt, ?callsign, ?heading, ?speed, ?otherdata, ?packettimetag, ?rawpacket)";
            command.Parameters.Add("?timetag", MySqlDbType.DateTime).Value = p.time;
            command.Parameters.Add("?lat", MySqlDbType.Double).Value = p.lat;
            command.Parameters.Add("?lon", MySqlDbType.Double).Value = p.lon;
            command.Parameters.Add("?alt", MySqlDbType.Double).Value = p.alt;
            command.Parameters.Add("?callsign", MySqlDbType.Text).Value = p.callsign;
            command.Parameters.Add("?heading", MySqlDbType.Double).Value = p.heading;
            command.Parameters.Add("?speed", MySqlDbType.Double).Value = p.speed;
            command.Parameters.Add("?otherdata", MySqlDbType.Text).Value = p.otherdata;
            command.Parameters.Add("?packettimetag", MySqlDbType.Text).Value = p.packetTime;
            command.Parameters.Add("?rawpacket", MySqlDbType.Text).Value = p.rawpacket;
            command.ExecuteNonQuery();
        }
    }
}
