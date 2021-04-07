using FrozenFoods.Core.Contracts;
using FrozenFoods.Core.Models;
using FrozenFoods.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frozenfoods.WebUI.Controllers
{
    public class HomeController : Controller
    {

        IRepository<Product> context;
        IRepository<ProductCategory> productcategories;
        public HomeController(IRepository<Product> productcontext, IRepository<ProductCategory> productCategoryContext)
        {
            context = productcontext;
            productcategories = productCategoryContext;
        }


        public ActionResult Index(string name, string category = null)
        {
           
            if (name != null)
            {
                if (name.Length != 0)
                {
                    List<Product> SearchProduct;
                    List<ProductCategory> SearchCategories = productcategories.collection().Where(x => x.IsActive == true).ToList();

                    if (name == null)
                    {
                        ViewBag.Message = "Not Found";
                        SearchProduct = context.collection().ToList();

                    }
                    else
                    {
                        SearchProduct = context.collection().Where(p => p.Name == name).ToList();

                    }
                    ProductListViewModel SearchModel = new ProductListViewModel();
                    SearchModel.Product = SearchProduct;
                    SearchModel.ProductCategories = SearchCategories;
                   
                    return View(SearchModel);
                }
            }
            List<Product> products;
            List<ProductCategory> categories = productcategories.collection().Where(x => x.IsActive == true).ToList();

            if (category == null)
            {
                products = context.collection().ToList();

            }
            else
            {
                products = context.collection().Where(p => p.Category == category).ToList();

            }
            ProductListViewModel model = new ProductListViewModel();
            model.Product = products;
            model.ProductCategories = categories;
            return View(model);

        }

        public ActionResult Details(String Id)
        {
            Product produts = context.find(Id);
            if (produts == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(produts);
            }
        }

        public ActionResult DetailsPartial(String Id)
        {
            Product produts = context.find(Id);
            if (produts == null)
            {
                return HttpNotFound();
            }
            else
            {
                return PartialView();
            }

        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public ActionResult Search(string name)
        {
            List<Product> products;
            List<ProductCategory> categories = productcategories.collection().Where(x => x.IsActive == true).ToList();

            if (name == null)
            {
                ViewBag.Message = "Not Found";
                products = context.collection().ToList();

            }
            else
            {
                products = context.collection().Where(p => p.Name == name).ToList();

            }
            ProductListViewModel model = new ProductListViewModel();
            model.Product = products;
            model.ProductCategories = categories;
            return View(model);
        }
    }
}