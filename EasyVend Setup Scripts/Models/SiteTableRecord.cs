using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyVend_Setup_Scripts
{
    internal class SiteTableRecord
    {
        public string SiteName { get; set; }
        public int Id { get; set; }
        public string Lottery { get; set; }
        public string AgentNumber { get; set; }
        public string ExternalStoreId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Zipcode { get; set; }
        public int DeviceCount { get; set; }


        public void display()
        {
            Console.WriteLine(
                "Name: {0} " +
                "Id: {1} " +
                "Lottery: {2} " +
                "Agent#: {3} " +
                "Devices: {4} " +
                "Phone: {5}" +
                "ExternalStoreId: {6}" +
                "FirstName: {7}" +
                "LastName: {8}" +
                "Email: {9}" +
                "Phone: {10}" +
                "Address: {11}" +
                "City: {12}" +
                "Zip: {13}"
                , SiteName, Id, Lottery, AgentNumber, DeviceCount
                , Phone,ExternalStoreId,FirstName,LastName, Email,
                Phone,Address,City,Zipcode);
        }


        public static bool AreEqual(SiteTableRecord site1, SiteTableRecord site2)
        {
            return (
                site1.SiteName == site2.SiteName &&
                site1.Id == site2.Id &&
                site1.AgentNumber == site2.AgentNumber &&
                site1.Phone == site2.Phone
            );
        }
    }
}
