using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyVend_Setup_Scripts
{
    internal class DeviceTableRecord
    {
        public string SiteName { get; set; }

        public string SerialNumber { get; set; }
        public string [] ExternalTerminalIds { get; set; }
        public int DeviceId { get; set; }
        public int SiteId { get; set; }
        public bool IsActive { get; set; }
        public string Version { get; set; }

        
        public DeviceTableRecord()
        {

        }


        public DeviceTableRecord(string site, string serial, string [] terminalIds)
        {
            ExternalTerminalIds = new string[4];

            this.SiteName = site;
            this.SerialNumber = serial;
            this.ExternalTerminalIds = terminalIds;
        }

        public void Display()
        {
            Console.Write("Site: {0} SN: {1} ExternalTerminalIds: ", SiteName, SerialNumber);
            for (int i = 0; i < 4; i++)
            {
                Console.Write(ExternalTerminalIds[i] + ", ");
            }
            Console.WriteLine();
        }

    }
}
