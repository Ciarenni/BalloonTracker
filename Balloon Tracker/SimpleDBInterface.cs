using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Balloon_Tracker
{
    abstract class SimpleDBInterface
    {
        abstract public bool Initialize(string initstring);
        abstract public void LogPacket(APRSPacket p);
    }
}
