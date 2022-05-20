using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyVend_Setup_Scripts
{
    internal class DeviceTableRecord
    {
        public int DeviceId { get; set; }
        public int SiteId { get; set; }
        public string SerialNumber { get; set; }
        public bool IsActive { get; set; }

        public string Version { get; set; }


        public DeviceTableRecord()
        {

        }


        public DeviceTableRecord(int id, int siteId, string serial, bool active)
        {
            DeviceId = id;
            SiteId = siteId;
            SerialNumber = serial;
            IsActive = active;
        }


        public DeviceTableRecord(int id, int siteId)
        {
            DeviceId = id;
            SiteId = siteId;
        }


        public void Display()
        {
            Console.WriteLine("DeviceId: {0} SiteId: {1} Serial# {2} Active: {3} Version: {4}", DeviceId, SiteId, SerialNumber, IsActive, Version);
        }


        public static bool AreEqual(DeviceTableRecord A, DeviceTableRecord B)
        {
            return (
                A.DeviceId == B.DeviceId &&
                A.SiteId == B.SiteId
            );
        }
    }
}
