using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Balloon_Tracker
{
    class PacketLogger
    {
        SimpleDBInterface myDB;

        public PacketLogger()
        {
            // try to user mysql db engine
            myDB = new SimpleSQLDB();
            if (!myDB.Initialize(@""))
            {
                // fail over to memory/filesystem kml
                myDB = new SimpleFileDB();
                myDB.Initialize(@"");
            }
        }

        public void LogPacket(APRSPacket p)
        {
            myDB.LogPacket(p);
        }
    }
}
