using FrozenFoods.Core.Contracts;
using FrozenFoods.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frozenfoods.WebUI.Controllers
{
    
    public class ProductCategoryManagerController : Controller
    {
        IRepository<ProductCategory> ProductCategoryContext;

        public ProductCategoryManagerController(IRepository<ProductCategory> productcategorycontext)
        {
            this.ProductCategoryContext = productcategorycontext;
        }

        // GET: ProductCategoryManager
        public ActionResult Index()
        {
            List<ProductCategory> productCategory = ProductCategoryContext.collection().ToList();
            return View(productCategory);
        }

        public ActionResult Create()
        {
            ProductCategory productCategory = new ProductCategory();

            return View(productCategory);
        }

        [HttpPost]
        public ActionResult Create(ProductCategory productCategory)
        {
            DatabaseFirstEntities dataContext = new DatabaseFirstEntities();
            //     var result= dataContext.ProductCategoryDB.FirstOrDefault(x => x.Category.ToString(). == productCategory.Category && x.Id != productCategory.Id);

            var resultt = dataContext.ProductCategories.FirstOrDefault(x => x.Category.ToString().Equals(productCategory.Category.ToString(), StringComparison.OrdinalIgnoreCase) && x.Id != productCategory.Id);

            //var validateName = db.Products.FirstOrDefault
            //                   (x => x.ProductName == ProductName && x.Id != Id);
            if (!ModelState.IsValid)
            {
                return View(productCategory);
            }
            else if (resultt != null)
            {
                ViewBag.Message = "Category Already Exists !!";
                return View(productCategory);
            }
            else
            {
                ProductCategoryContext.insert(productCategory);
                ProductCategoryContext.commit();

                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(string Id)
        {
            ProductCategory productCategory = ProductCategoryContext.find(Id);
            if (productCategory == null)
            {
                return HttpNotFound();

            }
            else
            {
                return View(productCategory);
            }
        }

        [HttpPost]
        public ActionResult Edit(ProductCategory productCategory, string Id)
        {
            DatabaseFirstEntities dataContext = new DatabaseFirstEntities();
            var resultt = dataContext.ProductCategories.FirstOrDefault(x => x.Category.ToString().Equals(productCategory.Category.ToString(), StringComparison.OrdinalIgnoreCase) && x.Id != productCategory.Id);

            ProductCategory productCategoryTOEdit = ProductCategoryContext.find(Id);
            if (productCategoryTOEdit == null)
            {
                return HttpNotFound();

            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return View(productCategory);
                }
                else if (resultt != null)
                {
                    ViewBag.Message = "Category Already Exists !!";
                    return View(productCategory);



                }
                else
                {
                    productCategoryTOEdit.Category = productCategory.Category;
                    productCategoryTOEdit.IsActive = productCategory.IsActive;
                    ProductCategoryContext.commit();
                    return RedirectToAction("Index");
                }
            }

        }

        public ActionResult Delete(string Id)
        {
            ProductCategory productCategoryToDelete = ProductCategoryContext.find(Id);
            if (productCategoryToDelete == null)
            {
                return HttpNotFound();

            }
            else
            {
                return View(productCategoryToDelete);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(string Id)
        {
            ProductCategory productCategoryToDelete = ProductCategoryContext.find(Id);
            if (productCategoryToDelete == null)
            {
                return HttpNotFound();

            }
            else
            {
                ProductCategoryContext.delete(Id);
                ProductCategoryContext.commit();
                return RedirectToAction("index");
            }
        }
    }
}