using FrozenFoods.Core.Contracts;
using FrozenFoods.Core.Models;
using FrozenFoods.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Frozenfoods.WebUI.Controllers
{
    public class AccountController : Controller
    {

       
        IRepository<Customer> CustomerContext;
     
        public AccountController( IRepository<Customer> context)
        {
                this.CustomerContext = context;
        }





        // GET: Account
        public ActionResult Index()
        {
            return View();
        }


       
        public ActionResult Register()
        {
            return View();
        }

       
        [HttpPost]
        public ActionResult SaveRegisterDetails(Customer model, HttpPostedFileBase file)
        {
           List< Customer> cus = CustomerContext.collection().Where(x => x.Email==model.Email).ToList();
            if (cus.Count == 0) {

            
            if (ModelState.IsValid)
            {
               
                using (var databaseContext = new DatabaseFirstEntities())
                {
                    

                    Customer customer = new Customer()
                    {
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        Email = model.Email,
                        PasswordHash=model.PasswordHash,
                        DateOfBirth = model.DateOfBirth,
                        SecurityQuestion = model.SecurityQuestion,
                        Answer = model.Answer,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber,
                        ZipCode = model.ZipCode,
                        UserId = model.Email,
                        


                    };
                    if (file != null)
                    {
                        customer.Image = model.UserId + Path.GetExtension(file.FileName);
                        var fileName = model.UserId + Path.GetExtension(file.FileName);
                        // store the file inside ~/App_Data/uploads folder
                        var path = Path.Combine(Server.MapPath("~/Content/uploads"), fileName);

                        //     file.SaveAs(Server.MapPath(@"~/Content/CustomerImages/") + model.Image);
                        file.SaveAs(path);
                    }

                    //Calling the SaveDetails method which saves the details.
                    databaseContext.Customers.Add(customer);
                    databaseContext.SaveChanges();
                }

                ViewBag.Message = "User Details Saved";
                return RedirectToAction("Index","Home");
            }
            else
            {

                //If the validation fails, we are returning the model object with errors to the view, which will display the error messages.
                return View("Register", model);
            }
            }
            else
            {
                ViewBag.Messages = "Email Already Exists";
                return View("Register", model);
            }
        }


        public ActionResult Login()
        {
            return View();
        }

        //The login form is posted to this method.
        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            //Checking the state of model passed as parameter.
            if (ModelState.IsValid)
            {

                //Validating the user, whether the user is valid or not.
                var isValidUser = IsValidUser(model);

                //If user is valid & present in database, we are redirecting it to Welcome page.
                if (isValidUser != null)
                {
                    var Email = model.Email;
                    DatabaseFirstEntities DataContext = new DatabaseFirstEntities();
                    var customer = DataContext.Customers.FirstOrDefault(x => x.Email == model.Email);
                    var admin = DataContext.UserRoles.FirstOrDefault(x => x.UserId == customer.Id);
                    if (admin == null)
                    {
                        UserRole ur = new UserRole();
                        ur.RoleId = "fgdsgs";
                        var adminYes = DataContext.Roles.FirstOrDefault(x => x.Id == ur.RoleId);
                        ViewBag.Admin = "adminNo";
                    }
                    else
                    {
                        TempData["name"] = customer.Email;
                        var adminYes = DataContext.Roles.FirstOrDefault(x => x.Id == admin.RoleId);
                        TempData.Keep();

                       
                    }
                    FormsAuthentication.SetAuthCookie(model.Email, false);
                    return RedirectToAction("Index","Home");
                }
                else
                {
                    //If the username and password combination is not present in DB then error message is shown.
                    ModelState.AddModelError("Failure", "Wrong Username and password combination !");
                    return View();
                }
            }
            else
            {
                //If model state is not valid, the model with error message is returned to the View.
                return View(model);
            }
        }

        //function to check if User is valid or not
        public Customer IsValidUser(LoginViewModel model)
        {
            using (var dataContext = new DatabaseFirstEntities())
            {
                //Retireving the user details from DB based on username and password enetered by user.
                Customer user = dataContext.Customers.Where(query => query.Email.Equals(model.Email) && query.PasswordHash.Equals(model.Password)).SingleOrDefault();
                //If user is present, then true is returned.
                if (user == null)
                    return null;
                //If user is not present false is returned.
                else
                    return user;
            }
        }


        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Abandon(); // it will clear the session at the end of request
            return RedirectToAction("Index","Home");
        }
    }

}
