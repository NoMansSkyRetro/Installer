using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NMSLegacyVersionInstaller
{
    public static class SteamID
    {

        // Version-based dummy Steam IDs (14 digits: major+minor of the version, zero padded).
        // Each legacy version gets its own id so their save folders never collide - no SmartSaveFolder needed.
        public static string DummySteamID(Enums.Update update)
        {
            switch (update)
            {
                case Enums.Update.Release: return "00000000000109";  // v1.09
                case Enums.Update.Foundation: return "00000000000113";  // v1.13
                case Enums.Update.PathFinder: return "00000000000124";  // v1.24
                case Enums.Update.AtlasRises: return "00000000000138";  // v1.38
                default: return "00000000000019";
            }
        }
    }
}
