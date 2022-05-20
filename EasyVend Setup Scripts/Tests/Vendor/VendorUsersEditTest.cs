using NUnit.Framework;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EasyVend_Setup_Scripts
{
    public class VendorUsersEditTest
    {

        string baseUrl;
        private LoginPage loginPage;
        private NavMenu navMenu;
        private VendorDetailsPage vendorDetials;
        private VendorUsersPage vendorUsersPage;

        [SetUp]
        public void Setup()
        {

            DriverFactory.InitDriver();
            baseUrl = ConfigurationManager.AppSettings["URL"];

            loginPage = new LoginPage(DriverFactory.Driver);
            navMenu = new NavMenu(DriverFactory.Driver);
            vendorDetials = new VendorDetailsPage(DriverFactory.Driver);
            vendorUsersPage = new VendorUsersPage(DriverFactory.Driver);
        }


        [TearDown]
        public void EndTest()
        {
            loginPage = null;
            navMenu = null;
            vendorDetials = null;
            vendorUsersPage = null;
            DriverFactory.CloseDriver();
        }


        //first add a user to have test data
        [Test, Description("Verify a new vendor user can be created and added to the list"), Order(1)]
        public void AddUser()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            vendorUsersPage.ClickAddUser();

            string username = NameGenerator.GenerateUsername(5);
            Assert.AreEqual(-1, vendorUsersPage.indexOfUser(username));

            vendorUsersPage.AddUserSuccess(
               username,
               NameGenerator.GenerateEntityName(5),
               NameGenerator.GenerateEntityName(5),
               "6612200748",
               (int)UserRoleSelect.ADMIN
            );

            Assert.AreNotEqual(-1, vendorUsersPage.indexOfUser(username));
        }


        [Test, Description("Verify fields are pre-populated with users' current information")]
        public void EditUser_Prepopulated_Data()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            UserTableRecord user = vendorUsersPage.GetUserByIndex(0);
            vendorUsersPage.ClickEditUserButton(0);

            Assert.IsTrue(vendorUsersPage.EditUserModal.GetFirstName().Length > 0);
            Assert.IsTrue(vendorUsersPage.EditUserModal.GetLastName().Length > 0);
            Assert.IsTrue(vendorUsersPage.EditUserModal.GetEmail().Length > 0);
            Assert.IsTrue(vendorUsersPage.EditUserModal.GetPhone().Length > 0);
            Assert.IsTrue(vendorUsersPage.EditUserModal.GetRole().Length > 0);

            Assert.IsTrue(vendorUsersPage.EditUserModal.UserMatches(user));
        }


        [Test, Description("Verify edit user modal opens when clicking a username from the list")]
        public void EditUser_Click_Username()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            vendorUsersPage.ClickUsername(0);

            Assert.IsTrue(vendorUsersPage.EditUserModal.IsOpen);
        }


        //Verify the edit user button is visible next to users in the list
        [Test, Description("Verify the edit user button is visible next to users in the list")]
        public void EditUser_Button()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);


            int count = vendorUsersPage.getRecordCount();

            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue(vendorUsersPage.EditUserButtonIsVisible(i));
            }
        }


        [Test, Description("Verify edit user modal opens when clicking the edit user button")]
        public void EditUser_Click_Button()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            Assert.IsTrue(vendorUsersPage.EditUserButtonIsVisible(0));

            vendorUsersPage.ClickEditUserButton(0);

            Assert.IsTrue(vendorUsersPage.EditUserModal.IsOpen);
        }


        [Test, Description("Verify edit user modal closes when the cancel button is pressed and user is unchanged")]
        public void EditUser_Cancel()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            UserTableRecord userBefore = vendorUsersPage.GetUserByIndex(0);
            vendorUsersPage.ClickEditUserButton(0);
            vendorUsersPage.EditUserModal.EnterForm(
                userBefore.Username,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.REPORT
            );

            vendorUsersPage.EditUserModal.ClickCancel();
            Assert.IsFalse(vendorUsersPage.EditUserModal.IsOpen);

            UserTableRecord userAfter = vendorUsersPage.GetUserByIndex(0);
            Assert.IsTrue(UserTableRecord.AreEqual(userBefore, userAfter));
        }


        [Test, Description("Verify a user can be edited successfully")]
        public void EditUser_Success()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            UserTableRecord userBefore = vendorUsersPage.GetUserByIndex(0);

            vendorUsersPage.ClickEditUserButton(0);
            vendorUsersPage.EditUserSuccess(
                userBefore.Username,
                NameGenerator.GenerateEntityName(5),
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.REPORT
            );

            int index = vendorUsersPage.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = vendorUsersPage.GetUserByIndex(index);

            Assert.IsFalse(UserTableRecord.AreEqual(userBefore, userAfter));
        }


        [Test, Description("Verify the user is not modified if invalid data is entered")]
        public void EditUser_Fail()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            UserTableRecord userBefore = vendorUsersPage.GetUserByIndex(0);

            //attempt to edit user - failure
            vendorUsersPage.ClickEditUserButton(0);
            vendorUsersPage.EditUserFail(
                userBefore.Username,
                "",
                NameGenerator.GenerateEntityName(5),
                "6612200748",
                (int)UserRoleSelect.REPORT
            );

            Assert.IsTrue(vendorUsersPage.EditUserModal.fNameErrorIsDisplayed());

            vendorUsersPage.EditUserModal.ClickCancel();

            int index = vendorUsersPage.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = vendorUsersPage.GetUserByIndex(index);

            Assert.IsTrue(UserTableRecord.AreEqual(userAfter, userBefore));

        }


        [Test, Description("Verify usere can edit another user's first name")]
        public void EditUser_Edit_FirstName()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            UserTableRecord userBefore = vendorUsersPage.GetUserByIndex(0);
            string newName = NameGenerator.GenerateEntityName(5);
            //eidt user
            vendorUsersPage.ClickEditUserButton(0);
            vendorUsersPage.EditUserSuccess(
                userBefore.Username,
                newName,
                userBefore.LastName,
                "6612200748",
                userBefore.Role == "Vendor Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            int index = vendorUsersPage.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = vendorUsersPage.GetUserByIndex(index);
            //verify users info matches the new user info
            Assert.IsFalse(UserTableRecord.AreEqual(userAfter, userBefore));
            Assert.AreEqual(newName, userAfter.FirstName);
        }


        //Verify error is displayed when first name field is empty
        [Test, Description("Verify error is displayed when first name field is empty")]
        public void EditUser_FirstName_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            UserTableRecord userBefore = vendorUsersPage.GetUserByIndex(0);

            //eidt user
            vendorUsersPage.ClickEditUserButton(0);
            vendorUsersPage.EditUserFail(
                userBefore.Username,
                "",
                userBefore.LastName,
                "6612200748",
                userBefore.Role == "Vendor Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            Assert.IsTrue(vendorUsersPage.EditUserModal.fNameErrorIsDisplayed());

            vendorUsersPage.EditUserModal.ClickCancel();
            int index = vendorUsersPage.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = vendorUsersPage.GetUserByIndex(index);

            Assert.IsTrue(UserTableRecord.AreEqual(userBefore, userAfter));
        }


        [Test, Description("Verify usere can edit another user's last name")]
        public void EditUser_Edit_LastName()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            UserTableRecord userBefore = vendorUsersPage.GetUserByIndex(0);
            string newName = NameGenerator.GenerateEntityName(5);
            //eidt user
            vendorUsersPage.ClickEditUserButton(0);
            vendorUsersPage.EditUserSuccess(
                userBefore.Username,
                userBefore.FirstName,
                newName,
                "6612200748",
                userBefore.Role == "Vendor Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            int index = vendorUsersPage.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = vendorUsersPage.GetUserByIndex(index);

            //verify users info matches the new user info
            Assert.IsFalse(UserTableRecord.AreEqual(userAfter, userBefore));
            Assert.AreEqual(newName, userAfter.LastName);
        }


        [Test, Description("Verify error is displayed when first name field is empty")]
        public void EditUser_LastName_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            UserTableRecord userBefore = vendorUsersPage.GetUserByIndex(0);

            //eidt user
            vendorUsersPage.ClickEditUserButton(0);
            vendorUsersPage.EditUserFail(
                userBefore.Username,
                userBefore.FirstName,
                "",
                "6612200748",
                userBefore.Role == "Vendor Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            Assert.IsTrue(vendorUsersPage.EditUserModal.lNameErrorIsDisplayed());

            vendorUsersPage.EditUserModal.ClickCancel();
            int index = vendorUsersPage.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = vendorUsersPage.GetUserByIndex(index);

            Assert.IsTrue(UserTableRecord.AreEqual(userBefore, userAfter));
        }


        //verify the email field is read only on the edit user modal
        [Test, Description("Verify the email field is read only on the edit user modal")]
        public void EditUser_Email_Readonly()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            //eidt user
            vendorUsersPage.ClickEditUserButton(0);

            Assert.IsTrue(vendorUsersPage.EditUserModal.EmailIsReadyOnly());
        }


        //verify user can edit a users role on edit user modal
        [Test, Description("verify user can edit a users role on edit user modal")]
        public void EditUser_Edit_Role()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            UserTableRecord userBefore = vendorUsersPage.GetUserByIndex(0);
            int roleBefore = userBefore.Role == "Vendor Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT;

            //eidt user
            vendorUsersPage.ClickEditUserButton(0);
            vendorUsersPage.EditUserSuccess(
                userBefore.Username,
                userBefore.FirstName,
                userBefore.LastName,
                "6612200748",
                userBefore.Role == "Vendor Admin" ? (int)UserRoleSelect.REPORT : (int)UserRoleSelect.ADMIN
            );

            int index = vendorUsersPage.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = vendorUsersPage.GetUserByIndex(index);
            int roleAfter = userAfter.Role == "Vendor Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT;

            //verify users info matches the new user info
            Assert.AreNotEqual(roleBefore, roleAfter);

        }


        [Test, Description("Verify error is displayed when first role is empty")]
        public void EditUser_Role_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            UserTableRecord userBefore = vendorUsersPage.GetUserByIndex(0);

            //eidt user
            vendorUsersPage.ClickEditUserButton(0);
            vendorUsersPage.EditUserFail(
                userBefore.Username,
                userBefore.FirstName,
                userBefore.LastName,
                "6612200748",
                (int)UserRoleSelect.NONE
            );

            Assert.IsTrue(vendorUsersPage.EditUserModal.RoleErrorIsDisplayed());

            vendorUsersPage.EditUserModal.ClickCancel();
            int index = vendorUsersPage.indexOfUser(userBefore.Username);
            UserTableRecord userAfter = vendorUsersPage.GetUserByIndex(index);

            Assert.AreEqual(userBefore.Role, userAfter.Role);
        }


        [Test, Description("Verify usere can edit another user's phone")]
        public void EditUser_Edit_Phone()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            //gets users current phone
            vendorUsersPage.ClickEditUserButton(0);
            UserTableRecord userBefore = vendorUsersPage.EditUserModal.GetUser();
            vendorUsersPage.EditUserModal.ClickCancel();

            //reverse phone number
            char[] c = userBefore.Phone.ToCharArray();
            Array.Reverse(c);
            string newPhone = new string(c);


            //eidt user
            vendorUsersPage.ClickEditUserButton(0);
            vendorUsersPage.EditUserSuccess(
                userBefore.Username,
                userBefore.FirstName,
                userBefore.LastName,
                newPhone,
                userBefore.Role == "Vendor Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            int index = vendorUsersPage.indexOfUser(userBefore.Username);
            vendorUsersPage.ClickEditUserButton(index);
            UserTableRecord userAfter = vendorUsersPage.EditUserModal.GetUser();

            //verify users info matches the new user info
            Assert.AreNotEqual(userBefore.Phone, userAfter.Phone);
            Assert.AreEqual(newPhone, userAfter.Phone);

        }



        //Verify error is displayed when phone field is empty
        [Test, Description("Verify error is displayed when first role is empty")]
        public void EditUser_Phone_Empty()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            //get users current phone
            vendorUsersPage.ClickEditUserButton(0);
            UserTableRecord userBefore = vendorUsersPage.EditUserModal.GetUser();
            vendorUsersPage.EditUserModal.ClickCancel();

            //edit user - phone blank
            vendorUsersPage.ClickEditUserButton(0);
            vendorUsersPage.EditUserFail(
                userBefore.Username,
                userBefore.FirstName,
                userBefore.LastName,
                "",
                userBefore.Role == "Vendor Admin" ? (int)UserRoleSelect.ADMIN : (int)UserRoleSelect.REPORT
            );

            Assert.IsTrue(vendorUsersPage.EditUserModal.PhoneErrorIsDisplayed());
            vendorUsersPage.EditUserModal.ClickCancel();

            //get users phone after
            int index = vendorUsersPage.indexOfUser(userBefore.Username);
            vendorUsersPage.ClickEditUserButton(0);
            UserTableRecord userAfter = vendorUsersPage.EditUserModal.GetUser();
            vendorUsersPage.EditUserModal.ClickCancel();

            //verify user was unchanged
            Assert.AreEqual(userBefore.Role, userAfter.Role);
            Assert.AreEqual(userBefore.Phone, userAfter.Phone);
        }


        //verify error is displayed when phone number is in invalid format
        [Test, Description("verify error is displayed when phone number is in invalid format")]
        public void EditUser_Phone_Invalid()
        {
            DriverFactory.GoToUrl(baseUrl);
            loginPage.PerformLogin(AppUsers.VENDOR_ADMIN.Username, AppUsers.VENDOR_ADMIN.Password);
            navMenu.ClickVendor();
            vendorDetials.EntityTabs.ClickUsersTab();
            vendorUsersPage.SetTableLength(100);

            UserTableRecord userBefore = vendorUsersPage.GetUserByIndex(0);
            //get users current phone
            vendorUsersPage.ClickEditUserButton(0);

            //more than 10 digits
            vendorUsersPage.EditUserModal.EnterPhone("12345678912");
            vendorUsersPage.EditUserModal.ClickSubmit();
            Assert.IsTrue(vendorUsersPage.EditUserModal.PhoneErrorIsDisplayed());
            vendorUsersPage.EditUserModal.ClickCancel();

            vendorUsersPage.ClickEditUserButton(0);

            //less than 10 numbers 123456789
            vendorUsersPage.EditUserModal.EnterPhone("123456789");
            vendorUsersPage.EditUserModal.ClickSubmit();
            Assert.IsTrue(vendorUsersPage.EditUserModal.PhoneErrorIsDisplayed());
            vendorUsersPage.EditUserModal.ClickCancel();

            vendorUsersPage.ClickEditUserButton(0);

            //letters 123456789a
            vendorUsersPage.EditUserModal.EnterPhone("123456789a");
            vendorUsersPage.EditUserModal.ClickSubmit();
            Assert.IsTrue(vendorUsersPage.EditUserModal.PhoneErrorIsDisplayed());
            vendorUsersPage.EditUserModal.ClickCancel();

            vendorUsersPage.ClickEditUserButton(0);

            //special characters 123456789!
            vendorUsersPage.EditUserModal.EnterPhone("123456789!");
            vendorUsersPage.EditUserModal.ClickSubmit();
            Assert.IsTrue(vendorUsersPage.EditUserModal.PhoneErrorIsDisplayed());

        }

    }
}