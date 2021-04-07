using FrozenFoods.Core.Contracts;
using FrozenFoods.Core.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frozenfoods.WebUI.Controllers
{
    public class BasketController : Controller
    {
        IBasketService BasketService;
        IOrderService OrderService;
        IRepository<Customer> CustomerContext;
        IRepository<Product> ProductContext;

        public BasketController(IBasketService basketservice, IOrderService orderservice, IRepository<Customer> context, IRepository<Product> productContext)
        {
            this.BasketService = basketservice;
            this.OrderService = orderservice;
            this.CustomerContext = context;
            this.ProductContext = productContext;
        }

        // GET: Basket
        public ActionResult Index()
        {

            var model = BasketService.GetBasketItem(this.HttpContext);
            List<Product> produts = ProductContext.collection().ToList();

            dynamic mymodel = new ExpandoObject();
            mymodel.Teachers = model;
            mymodel.Students = produts;


            return View(mymodel);
        }


        public ActionResult _showParticalView(String Id)
        {
            //var model = BasketService.GetBasketItem(this.HttpContext);
            //dynamic mymodel = new ExpandoObject();
            //mymodel.Teachers = model;       
            //Product produts = ProductContext.find(Id);           
            //mymodel.Students = produts;
            //if (produts == null)
            //{
            //    return HttpNotFound();
            //}
            //else
            //{
            //    return(mymodel);
            //}
            Product produts = ProductContext.find(Id);
            return PartialView(produts);

        }


        public ActionResult AddToBasket(String id)
        {
            BasketService.AddToBasket(this.HttpContext, id);
            return RedirectToAction("Index");
        }

        public ActionResult Remove(String id)
        {
            BasketService.Remove(this.HttpContext, id);
            return RedirectToAction("Index");
        }


        public ActionResult RemoveFromBasket(String id)
        {
            BasketService.RemoveFromBasket(this.HttpContext, id);
            return RedirectToAction("Index");
        }

        public PartialViewResult BasketsSummary()
        {
            var BasketSummary = BasketService.GetBasketSummary(this.HttpContext);
            return PartialView(BasketSummary);
        }

        [Authorize]
        public ActionResult CheckOut()
        {
            var basketItems = BasketService.GetBasketItem(this.HttpContext);

            
            foreach (var pro in basketItems)
            {
                if (pro.Quantity > 0 )
                {

                }
                else if (basketItems.Count == 0)
                {
                    ViewBag.Message = "Please Add valid Quantity";
                    return RedirectToAction("Index");
                }
                else {
                    ViewBag.Message = "Please Add valid Quantity";
                    return RedirectToAction("Index");

                }
            }

            if (basketItems.Count == 0) {

                ViewBag.Message = "Please Add valid Quantity";
                return RedirectToAction("Index");
            }
            
            Customer cus = CustomerContext.collection().FirstOrDefault(c => c.Email == User.Identity.Name);
            TempData["CustomerId"] = cus.UserId;
            if (cus != null)
            {

                Order order = new Order()
                {
                    Email = cus.Email,
                    Address = cus.Address,
                    FirstName = cus.FirstName,
                    SurName = cus.LastName,
                    ZipCode = cus.ZipCode
                };
                return View(order);
            }
            else
            {
                return RedirectToAction("Thankyou");
            }

        }

        [HttpPost]
        [Authorize]
        public ActionResult CheckOut(Order order)
        {
            if (!ModelState.IsValid)
            {
                return View(order);
            }
            else
            {

                var basketItems = BasketService.GetBasketItem(this.HttpContext);

                order.OrderStatus = "Order created";
                order.Email = User.Identity.Name;
                order.CustomerId = TempData["CustomerId"] as string;
                order.OrderStatus = "Payment Processed";

                OrderService.CreateOrder(order, basketItems);
                BasketService.ClearBasket(this.HttpContext);
                List<OrderItem> BasketProduct = order.OrderItems.ToList();
                foreach (var Product in BasketProduct)
                {
                    Product ProductQuantityTOEdit = ProductContext.find(Product.ProductId);
                    Product result = ProductContext.find(Product.ProductId);

                    ProductQuantityTOEdit.Category = result.Category;
                    ProductQuantityTOEdit.Description = result.Description;
                    ProductQuantityTOEdit.Name = result.Name;
                    ProductQuantityTOEdit.Price = (int)result.Price;

                    ProductQuantityTOEdit.Unit = (int)result.Unit - Product.Quantity;
                    ProductContext.commit();
                }



                return RedirectToAction("Thankyou", new { orderId = order.OrderNumber });
            }
        }

        public ActionResult Thankyou(string orderId)
        {

            ViewBag.orderId = orderId;
            return View();
        }

    }
}