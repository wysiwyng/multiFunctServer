using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace serverExternals
{
    public static class Commands
    {
        public const byte BroadcastMessage = 0;

        public const byte SpecificMessage = 1;

        public const byte ListClients = 2;
    }
}
