using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyVend_Setup_Scripts
{
    public class MachineActivityRecord
    {
        public DateTime Date { get; set; }
        public int SiteId { get; set; }
        public int AgentNumber { get; set; }
        public string SiteName { get; set; }
        public int DeviceId { get; set; }
        public string Activity { get; set; }
        public int LotteryGameId { get; set; }
        public double TicketPrice { get; set; }
        public string GameName { get; set; }
        public string DrawerNumber { get; set; }


        public void Display()
        {
            Console.WriteLine("Date: {0}", Date);
            Console.WriteLine("SiteId: {0}", SiteId);
            Console.WriteLine("AgentNum: {0}", AgentNumber);
            Console.WriteLine("Site Name: {0}", SiteName);
            Console.WriteLine("DeviceId: {0}", DeviceId);
            Console.WriteLine("Activity: {0}", Activity);
            Console.WriteLine("Game Id: {0}", LotteryGameId);
            Console.WriteLine("Price: {0}", TicketPrice);
            Console.WriteLine("Game Name: {0}", GameName);
            Console.WriteLine("Drawer: {0}", DrawerNumber);
        }
    }
}
