using FrozenFoods.Core.Contracts;
using FrozenFoods.Core.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frozenfoods.WebUI.Controllers
{
    public class CustomerController : Controller
    {
       
        IRepository<Customer> CustomerContext;

        public CustomerController(IRepository<Customer> customercontext)
        {
            this.CustomerContext = customercontext;
        }

        // GET: Customer
        public ActionResult Index()
        {
            List<Customer> CustomerList = CustomerContext.collection().ToList();
            return View(CustomerList);
        }

        public ActionResult Edit(string Id)
        {
            Customer Customer = CustomerContext.find(Id);
            if (Customer == null)
            {
                return HttpNotFound();

            }
            else
            {
                Customer viewModel = new Customer();
                viewModel = Customer;
                return View(viewModel);
            }
        }

        [HttpPost]
        public ActionResult Edit(Customer Customer, string Id, HttpPostedFileBase File, string Password)
        {

            Customer CustomerTOEdit = CustomerContext.find(Id);
            if (CustomerTOEdit == null)
            {
                return HttpNotFound();

            }
            else
            {
                List<Customer> cus = CustomerContext.collection().Where(x => x.Email == Customer.Email && x.Id != CustomerTOEdit.Id).ToList();
                if (cus.Count != 0)
                {
                    return View(Customer);
                }
                else
                { 
                if (!ModelState.IsValid)
                {
                    return View(Customer);
                }
                else
                {
                    if (File != null)
                    {
                        CustomerTOEdit.Image = Customer.Id + Path.GetFileName(File.FileName);
                        //    File.SaveAs(Server.MapPath("//Content//CustomerImages//") + CustomerTOEdit.Image);
                        var fileName = Customer.Id + Path.GetFileName(File.FileName);
                        // store the file inside ~/App_Data/uploads folder
                        var path = Path.Combine(Server.MapPath("~/Content/uploads"), fileName);
                        File.SaveAs(path);

                    }
                    CustomerTOEdit.FirstName = Customer.FirstName;
                    CustomerTOEdit.LastName = Customer.LastName;
                    CustomerTOEdit.Email = Customer.Email;
                    CustomerTOEdit.DateOfBirth = Customer.DateOfBirth;
                    CustomerTOEdit.Disable = Customer.Disable;
                    CustomerTOEdit.SecurityQuestion = Customer.SecurityQuestion;
                    CustomerTOEdit.PhoneNumber = Customer.PhoneNumber;
                    CustomerTOEdit.Answer = Customer.Answer;
                    CustomerTOEdit.Address = Customer.Address;
                    CustomerTOEdit.ZipCode = Customer.ZipCode;

                    // CustomerTOEdit.p=

                    UserManager<IdentityUser> userManager =
                    new UserManager<IdentityUser>(new UserStore<IdentityUser>());
                    if (Password.Length != 0)
                    {
                        userManager.RemovePassword(Customer.UserId);
                        String newPassword = Password;
                        userManager.AddPassword(Customer.UserId, newPassword);
                    }
                    else
                    {

                    }
                    CustomerContext.commit();

                    return RedirectToAction("Index");
                }
             }
            }

        }

        public ActionResult ForgotPassword()
        {

            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword(string Email, string SecurityQuestion, string Answer)
        {
            Customer cus = CustomerContext.collection().FirstOrDefault(c => c.Email == Email);
            if (cus != null)
            {
                if (cus.SecurityQuestion == SecurityQuestion && cus.Answer == Answer)
                {
                    TempData.Add("MyTempData", cus.Id);
                    return RedirectToAction("UpdatePassword");
                }
                else
                {
                    ViewBag.Message = "Invalid Email or Security Question/Answer";
                    return View();
                }
            }
            else
            {
                ViewBag.Message = "Invalid Email or Security Question/Answer";
                return View();
            }

        }

        public ActionResult UpdatePassword()
        {
            string id = TempData["MyTempData"].ToString();
            Customer Customer = CustomerContext.find(id);
            if (Customer == null)
            {
                return HttpNotFound();

            }
            else
            {
                Customer viewModel = new Customer();
                viewModel = Customer;
                return View(viewModel);
            }

        }

        [HttpPost]
        public ActionResult UpdatePassword(Customer Customer, string Id, HttpPostedFileBase File, string Password)
        {
            Customer CustomerTOEdit = CustomerContext.find(Id);
            Customer CustomerToStore = CustomerContext.find(Id);
            if (CustomerTOEdit == null)
            {
                return HttpNotFound();

            }
            else
            {



                CustomerTOEdit.SecurityQuestion = CustomerToStore.SecurityQuestion;
                CustomerTOEdit.FirstName = CustomerToStore.FirstName;
                CustomerTOEdit.LastName = CustomerToStore.LastName;
                CustomerTOEdit.Email = CustomerToStore.Email;
                CustomerTOEdit.DateOfBirth = CustomerToStore.DateOfBirth;
                CustomerTOEdit.Disable = CustomerToStore.Disable;
                CustomerTOEdit.PhoneNumber = CustomerToStore.PhoneNumber;
                CustomerTOEdit.Answer = CustomerToStore.Answer;
                CustomerTOEdit.Address = CustomerToStore.Address;
                CustomerTOEdit.ZipCode = CustomerToStore.ZipCode;


                Microsoft.AspNet.Identity.UserManager<IdentityUser> userManager =
                new Microsoft.AspNet.Identity.UserManager<IdentityUser>(new UserStore<IdentityUser>());
                if (Password.Length != 0)
                {
                    userManager.RemovePassword(CustomerTOEdit.UserId);
                    String newPassword = Password;
                    userManager.AddPassword(CustomerTOEdit.UserId, newPassword);
                }
                else
                {

                }
                CustomerContext.commit();

                return RedirectToAction("Index", "Home");

            }
        }





        public ActionResult Details(String Id)
        {
            Customer CustomerDetails = CustomerContext.find(Id);
            if (CustomerDetails == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(CustomerDetails);
            }
        }


        public ActionResult MyProfile()
        {



            Customer cus = CustomerContext.collection().FirstOrDefault(c => c.Email == User.Identity.Name);

            if (cus == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(cus);
            }
        }



        public ActionResult UpdateProfileByUser(string Id)
        {
            Customer Customer = CustomerContext.find(Id);
            if (Customer == null)
            {
                return HttpNotFound();

            }
            else
            {
                Customer viewModel = new Customer();
                viewModel = Customer;
                return View(viewModel);
            }
        }


        [HttpPost]
        public ActionResult UpdateProfileByUser(Customer Customer, string Id, HttpPostedFileBase File, string PasswordHash)
        {

            Customer CustomerTOEdit = CustomerContext.find(Id);
            Customer CustomerTO = CustomerContext.find(Id);
            Customer.Image = File.FileName;
            Customer.PasswordHash = PasswordHash;
            //Customer.SecurityQuestion = CustomerTO.SecurityQuestion;
            //Customer.Answer = CustomerTO.Answer;
            //Customer.Email = CustomerTO.Email;
            if (CustomerTOEdit == null)
            {
                return HttpNotFound();

            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return View(Customer);
                }
                else
                {
                    if (File != null)
                    {
                        CustomerTOEdit.Image = Customer.Id + Path.GetFileName(File.FileName);
                        //    File.SaveAs(Server.MapPath("//Content//CustomerImages//") + CustomerTOEdit.Image);
                        var fileName = Customer.Id + Path.GetFileName(File.FileName);
                        // store the file inside ~/App_Data/uploads folder
                        var path = Path.Combine(Server.MapPath("~/Content/uploads"), fileName);
                        File.SaveAs(path);

                    }
                    CustomerTOEdit.FirstName = Customer.FirstName;
                    CustomerTOEdit.LastName = Customer.LastName;
                    CustomerTOEdit.Email = CustomerTO.Email;
                    CustomerTOEdit.DateOfBirth = Customer.DateOfBirth;
                    CustomerTOEdit.Disable = Customer.Disable;
                    CustomerTOEdit.SecurityQuestion = CustomerTO.SecurityQuestion;
                    CustomerTOEdit.PhoneNumber = Customer.PhoneNumber;
                    CustomerTOEdit.Answer = CustomerTO.Answer;
                    CustomerTOEdit.Address = Customer.Address;
                    CustomerTOEdit.ZipCode = Customer.ZipCode;

                    // CustomerTOEdit.p=

                    CustomerContext.commit();

                    return RedirectToAction("MyProfile");
                }
            }

        }

    }
}