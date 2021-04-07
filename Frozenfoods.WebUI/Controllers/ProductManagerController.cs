using FrozenFoods.Core.Contracts;
using FrozenFoods.Core.Models;
using FrozenFoods.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frozenfoods.WebUI.Controllers
{
    // this class is using to handle all the product related work homepage, addproduct, edit, delete, list of product
    //  [Authorize(Roles = "Admin")]
    public class ProductManagerController : Controller
    {
        IRepository<Product> ProductContext;
        IRepository<AddProductHistory> AddProductHistoryContext;
        IRepository<ProductCategory> ProductCategoryContext;
        public ProductManagerController(IRepository<Product> productcontext, IRepository<ProductCategory> productcategorycontext, IRepository<AddProductHistory> addproducthistorycontext)
        {
            this.ProductContext = productcontext;
            this.ProductCategoryContext = productcategorycontext;
            this.AddProductHistoryContext = addproducthistorycontext;
        }

        // GET: ProductManager
        // this method returns the list of products
        public ActionResult Index()
        {
            List<Product> product = ProductContext.collection().ToList();
            return View(product);
        }




        public ActionResult Detailsss(String Id)
        {
            Product produts = ProductContext.find(Id);

            return PartialView("_Details", produts);

        }


        // this method retuns creates new product page
        public ActionResult Create()
        {
            ProductManagerViewModel ProductManagerViewMode = new ProductManagerViewModel();
            ProductManagerViewMode.Product = new Product();
            ProductManagerViewMode.ProductCategory = ProductCategoryContext.collection().Where(x => x.IsActive == true).ToList();
            return View(ProductManagerViewMode);
        }
        [HttpPost]
        public ActionResult Create(Product product, HttpPostedFileBase file)
        {
            DatabaseFirstEntities dataContext = new DatabaseFirstEntities();
            var resultt = dataContext.Products.FirstOrDefault(x => x.Name.ToString().Equals(product.Name, StringComparison.OrdinalIgnoreCase) && x.Id != product.Id && x.Category == product.Category);
            ProductManagerViewModel ProductManagerViewModel = new ProductManagerViewModel();
            ProductManagerViewModel.Product = product;
            ProductManagerViewModel.ProductCategory = ProductCategoryContext.collection();
            if (!ModelState.IsValid)
            {
                return View(ProductManagerViewModel);
            }
            else if (resultt != null)
            {

                ViewBag.Message = "Product Already Exists !!";
                return View(ProductManagerViewModel);

            }
            else
            {
                if (file != null)
                {
                    product.Image = product.Id + Path.GetFileName(file.FileName);

                    var fileName = product.Id + Path.GetFileName(file.FileName);
                    // store the file inside ~/App_Data/uploads folder
                    var path = Path.Combine(Server.MapPath("~/Content/ProductImages"), fileName);

                    //     file.SaveAs(Server.MapPath(@"~/Content/CustomerImages/") + model.Image);
                    file.SaveAs(path);
                }
                product.Unit = product.TotalQuantity;
                ProductContext.insert(product);
                ProductContext.commit();

                return RedirectToAction("Index");
            }
        }

        public ActionResult Edit(string Id)
        {
            Product product = ProductContext.find(Id);
            if (product == null)
            {
                return HttpNotFound();

            }
            else
            {
                ProductManagerViewModel viewModel = new ProductManagerViewModel();
                viewModel.Product = product;
                viewModel.ProductCategory = ProductCategoryContext.collection().Where(x => x.IsActive == true).ToList();
                return View(viewModel);
            }
        }

        [HttpPost]
        public ActionResult Edit(Product product, string Id, HttpPostedFileBase file, string Image)
        {
            DatabaseFirstEntities dataContext = new DatabaseFirstEntities();
            var resultt = dataContext.Products.FirstOrDefault(x => x.Name.ToString().Equals(product.Name, StringComparison.OrdinalIgnoreCase) && x.Id != product.Id && x.Category == product.Category);
            ProductManagerViewModel ProductManagerViewModel = new ProductManagerViewModel();
            ProductManagerViewModel.Product = product;
            ProductManagerViewModel.Product.Image = Image;
            ProductManagerViewModel.ProductCategory = ProductCategoryContext.collection();

            Product productTOEdit = ProductContext.find(Id);
            if (productTOEdit == null)
            {
                return HttpNotFound();

            }
            else if (resultt != null)
            {

                ViewBag.Message = "Product Already Exists !!";
                return View(ProductManagerViewModel);
            }
            else
            {
                if (!ModelState.IsValid)
                {
                    return View(ProductManagerViewModel);
                }
                else
                {
                    if (file != null)
                    {
                        productTOEdit.Image = product.Id + Path.GetFileName(file.FileName);
                        var fileName = product.Id + Path.GetFileName(file.FileName);
                        // store the file inside ~/App_Data/uploads folder
                        var path = Path.Combine(Server.MapPath("~/Content/ProductImages"), fileName);

                        //     file.SaveAs(Server.MapPath(@"~/Content/CustomerImages/") + model.Image);
                        file.SaveAs(path);


                    }
                    productTOEdit.Category = product.Category;
                    productTOEdit.Description = product.Description;
                    productTOEdit.Name = product.Name;
                    productTOEdit.Price = product.Price;
                    productTOEdit.Unit = product.Unit;

                    ProductContext.commit();
                    return RedirectToAction("Index");
                }
            }

        }


        public ActionResult AddMoreQuantity(string Id)
        {
            Product product = ProductContext.find(Id);
            if (product == null)
            {
                return HttpNotFound();

            }
            else
            {
                ProductManagerViewModel viewModel = new ProductManagerViewModel();
                viewModel.Product = product;
                viewModel.ProductCategory = ProductCategoryContext.collection();
                return View(viewModel);
            }
        }




        [HttpPost]
        public ActionResult AddMoreQuantity(Product product, string Id, int NewQuantity)
        {
            Product OldProduct = ProductContext.find(Id);
            Product productTOEdit = ProductContext.find(Id);
            ProductManagerViewModel ProductManagerViewModel = new ProductManagerViewModel();
            ProductManagerViewModel.Product = OldProduct;
            ProductManagerViewModel.ProductCategory = ProductCategoryContext.collection();
            if (productTOEdit == null)
            {
                return HttpNotFound();

            }
            else
            {



                int moreQ = NewQuantity;
                productTOEdit.Unit = moreQ + OldProduct.Unit;
                productTOEdit.Category = OldProduct.Category;
                productTOEdit.Description = OldProduct.Description;
                productTOEdit.Name = OldProduct.Name;
                productTOEdit.Price = (int)OldProduct.Price;
                productTOEdit.Unit = OldProduct.Unit;


                AddProductHistory addProductHistory = new AddProductHistory()
                {
                    ProductId = product.Id,
                    ProductName = OldProduct.Name,
                    AddedBy = User.Identity.Name,
                    AddedQuantity = moreQ


                };

                AddProductHistoryContext.insert(addProductHistory);
                AddProductHistoryContext.commit();
                ProductContext.commit();
                return RedirectToAction("Index");

            }

        }

        public ActionResult ProductAdd(String Id)
        {
            AddProductHistory ProductDetails = AddProductHistoryContext.find(Id);
            if (ProductDetails == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(ProductDetails);
            }
        }

        //public ActionResult History()
        //{
        // List <  AddProductHistory> ProductAddDetails = AddProductHistoryContext.collection().ToList();
        //    return View(ProductAddDetails);

        //}


        public ActionResult Delete(string Id)
        {
            Product productToDelete = ProductContext.find(Id);
            if (productToDelete == null)
            {
                return HttpNotFound();

            }
            else
            {
                return View(productToDelete);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public ActionResult ConfirmDelete(string Id)
        {
            Product productToDelete = ProductContext.find(Id);
            if (productToDelete == null)
            {
                return HttpNotFound();

            }
            else
            {
                ProductContext.delete(Id);
                ProductContext.commit();
                return RedirectToAction("index");
            }
        }

        public ActionResult Details(String Id)
        {
            Product ProductDetails = ProductContext.find(Id);
            if (ProductDetails == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(ProductDetails);
            }
        }


        public ActionResult AddHistory(string Id)
        {
            List<AddProductHistory> AddHistory = AddProductHistoryContext.collection().OrderByDescending(x => x.CreatedAt).Where(x => x.ProductId == Id).ToList();

            if (AddHistory == null)
            {
                return HttpNotFound();
            }
            else
            {
                return View(AddHistory);
            }

        }

    }
}