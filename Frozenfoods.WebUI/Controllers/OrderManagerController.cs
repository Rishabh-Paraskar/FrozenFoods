using FrozenFoods.Core.Contracts;
using FrozenFoods.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frozenfoods.WebUI.Controllers
{
   
    public class OrderManagerController : Controller
    {
        IOrderService OrderService;
        IRepository<Product> ProductContext;
        IRepository<Customer> CustomerContext;
        public OrderManagerController(IOrderService orderservice, IRepository<Product> productContext, IRepository<Customer> customerContext)
        {
            this.OrderService = orderservice;
            this.ProductContext = productContext;
            this.CustomerContext = customerContext;
        }

        // GET: OrderManager
        public ActionResult Index()
        {
            List<Order> orders = OrderService.GetOrderList().OrderByDescending(x => x.OrderNumber).Where(x => x.flag==true).ToList<Order>();
            return View(orders);
        }


        public ActionResult OrderAcceptReject()
        {
            List<Order> orders = OrderService.GetOrderList().OrderByDescending(x => x.OrderNumber).Where(x => x.flag == false).ToList<Order>(); ;

            return View(orders);
        }
        [AllowAnonymous]
        public ActionResult CustomerOrder()
        {
            string email = User.Identity.Name;
            Customer cus = CustomerContext.collection().FirstOrDefault(x => x.Email == email);
            List<Order> order = OrderService.GetOrderList().Where(x => x.CustomerId == cus.UserId).OrderByDescending(x => x.OrderNumber).ToList<Order>();
            return View(order);
        }



        public ActionResult updateOrder(string id)
        {
            ViewBag.statusList = new List<string>()
            {
                "Order Created",
                "Payment Processed",
                "Order Shipped",
                "Order Completed"
            };
            Order order = OrderService.GetOrder(id);
            return View(order);
        }

        [HttpPost]
        public ActionResult updateOrder(Order updatedOrder, string id)
        {
            Order order = OrderService.GetOrder(id);

            order.OrderStatus = updatedOrder.OrderStatus;
            OrderService.UpdateOrder(order);
            return RedirectToAction("Index");
        }
        [AllowAnonymous]
        public ActionResult ViewOrder(string id)
        {
            ViewBag.statusList = new List<string>()
            {
                "Order Created",
                "Payment Processed",
                "Order Shipped",
                "Order Completed"
            };
            Order order = OrderService.GetOrder(id);
            return View(order);
        }
        [AllowAnonymous]
        [HttpPost]
        public ActionResult ViewOrder(Order updatedOrder, string id)
        {
            Order order = OrderService.GetOrder(id);

            order.OrderStatus = updatedOrder.OrderStatus;
            OrderService.UpdateOrder(order);
            return RedirectToAction("Index");
        }

        public ActionResult Accept(string id)
        {

            Order orderTOEdit = OrderService.GetOrder(id);
            if (orderTOEdit == null)
            {
                return HttpNotFound();

            }
            else
            {
                orderTOEdit.AcceptOrReject = true;
                orderTOEdit.flag = true;

                OrderService.UpdateOrder(orderTOEdit);
                return RedirectToAction("Index");

            }
        }

        public ActionResult Reject(string id)
        {

            ViewBag.statusList = new List<string>()
            {
                "Order Created",
                "Payment Processed",
                "Order Shipped",
                "Order Completed",
                "Rejected"
            };

            Order OrderToDelete = OrderService.GetOrder(id);
            if (OrderToDelete == null)
            {
                return HttpNotFound();

            }
            else
            {
                return View(OrderToDelete);
            }

        }

        [HttpPost]
        [ActionName("Reject")]
        public ActionResult ConfirmReject(string id, Order updatedOrder)
        {
            Order OrderToDelete = OrderService.GetOrder(id);
            if (OrderToDelete == null)
            {
                return HttpNotFound();

            }
            else
            {
                Order order = OrderService.GetOrder(id);
                List<OrderItem> BasketProduct = order.OrderItems.ToList();
                foreach (var Product in BasketProduct)
                {
                    Product ProductQuantityTOEdit = ProductContext.find(Product.ProductId);
                    Product result = ProductContext.find(Product.ProductId);
                    ProductQuantityTOEdit.Category = result.Category;
                    ProductQuantityTOEdit.Description = result.Description;
                    ProductQuantityTOEdit.Name = result.Name;
                    ProductQuantityTOEdit.Price = (int)result.Price;
                    ProductQuantityTOEdit.Unit = result.Unit + Product.Quantity;
                    ProductContext.commit();
                }
                order.flag = true;
                OrderToDelete.Remark = updatedOrder.Remark;
                OrderToDelete.OrderStatus = updatedOrder.OrderStatus;
                OrderService.UpdateOrder(order);
                OrderService.UpdateOrder(OrderToDelete);

                return RedirectToAction("index");
            }
        }

    }
}