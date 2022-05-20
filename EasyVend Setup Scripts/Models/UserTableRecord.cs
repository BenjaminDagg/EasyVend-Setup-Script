using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyVend_Setup_Scripts
{
    internal class UserTableRecord
    {
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Phone { get; set; }

        public string Role { get; set; }
        public bool Locked { get; set; }
        public bool Active { get; set; }

        public UserTableRecord(string username, string fName, string lName, string role, bool locked, bool active)
        {
            Username = username;
            FirstName = fName;
            LastName = lName;
            Role = "";
            Locked = locked;
            Active = active;

        }

        public UserTableRecord()
        {

        }


        public void display()
        {
            Console.WriteLine("Email: {0}, First Name: {1}, Last Name: {2},  Role: {3}", Username, FirstName, LastName, Role);
        }


        public static bool AreEqual(UserTableRecord A, UserTableRecord B)
        {
            return (
                A.Username == B.Username &&
                A.FirstName == B.FirstName &&
                A.LastName == B.LastName
            );
        }

    }
}
