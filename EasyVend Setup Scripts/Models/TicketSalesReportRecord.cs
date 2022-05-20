using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyVend_Setup_Scripts
{
    internal class TicketSalesReportRecord
    {
        public int SiteId { get; set; }
        public string AgentNum { get; set; }
        public string SiteName { get; set; }
        public int DeviceId { get; set; }
        public int LotteryGameId { get; set; }
        public double TicketPrice { get; set; }
        public string GameName { get; set; }
        public int NumTicketsSold { get; set; }
        public double TotalAmount { get; set; }


        public void Display()
        {
            Console.WriteLine("SiteId: {0}, AgentNum: {1}, Site: {2}, Device: {3}, GameId: {4}, Price: {5}, Game Name: {6}, # Sold: {7}, Total Sold: {8}",
                SiteId, AgentNum, SiteName, DeviceId, LotteryGameId, TicketPrice, GameName, NumTicketsSold, TotalAmount);
        }
    }
}
