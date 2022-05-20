using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyVend_Setup_Scripts
{
    public enum UserPermission
    {
        VENDOR_ADMIN,
        VENDOR_REPORT,
        LOTTERY_ADMIN,
        LOTTERY_REPORT,
        SITE_ADMIN,
        SITE_REPORT,
    }

    internal class UserRole
    {


        public static UserPermission parseUserRole(string role)
        {
            switch (role)
            {
                case "Vendor Admin":
                    return UserPermission.VENDOR_ADMIN;
                    break;
                case "Vendor Report":
                    return UserPermission.VENDOR_REPORT;
                    break;
                case "Lottery Admin":
                    return UserPermission.LOTTERY_ADMIN;
                    break;
                case "Lottery Report":
                    return UserPermission.LOTTERY_REPORT;
                    break;
                case "Site Admin":
                    return UserPermission.SITE_ADMIN;
                    break;
                default:
                    return UserPermission.SITE_REPORT;
                    break;
            }
        }
    }
}
