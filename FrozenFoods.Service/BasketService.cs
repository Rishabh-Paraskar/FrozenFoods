using FrozenFoods.Core.Contracts;
using FrozenFoods.Core.Models;
using FrozenFoods.Core.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace FrozenFoods.Service
{
    public class BasketService : IBasketService
    {
        IRepository<Product> ProductContext;
        IRepository<Basket> BasketContext;

        public const string BasketSessionName = "eCommerceBasket";

        public BasketService(IRepository<Product> productcontext, IRepository<Basket> basketcontext)
        {
            this.ProductContext = productcontext;
            this.BasketContext = basketcontext;
        }

        private Basket GetBasket(HttpContextBase httpContext, bool createIfNull)
        {
            HttpCookie cookie = httpContext.Request.Cookies.Get(BasketSessionName);

            Basket basket = new Basket();

            if (cookie != null)
            {
                string basketId = cookie.Value;
                if (!string.IsNullOrEmpty(basketId))
                {
                    basket = BasketContext.find(basketId);
                    return basket;
                }
                else
                {
                    if (createIfNull)
                    {
                        basket = createNewBasket(httpContext);
                    }
                }
            }
            else
            {
                if (createIfNull)
                {
                    basket = createNewBasket(httpContext);
                }
            }

            return basket;

        }

        //private void checkOutFull(HttpContextBase httpContext) {
        //    HttpCookie cookie = httpContext.Request.Cookies.Get(BasketSessionName);
        //    Basket basket = new Basket();
        //    if (cookie != null)
        //    {
        //        string basketId = cookie.Value;
        //        if (!string.IsNullOrEmpty(basketId))
        //        {
        //            basket = BasketContext.find(basketId);
        //            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.ProductId == productid);

        //            item = new BasketItem()
        //            {
        //                BasketId = basket.Id,
        //                ProductId = productid,
        //                Quantity = 1
        //            };
        //            basket.BasketItems.Add(item);
        //            BasketContext.commit();
        //        }
        //        else
        //        {
                  
        //        }
        //    }
        //    else
        //    {
                
        //    }
        //}




        private Basket createNewBasket(HttpContextBase httpContext)
        {
            Basket basket = new Basket();
            BasketContext.insert(basket);
            BasketContext.commit();

            HttpCookie cookie = new HttpCookie(BasketSessionName);
            cookie.Value = basket.Id;
            cookie.Expires = DateTime.Now.AddDays(1);
            httpContext.Response.Cookies.Add(cookie);

            return basket;
        }

        public void AddToBasket(HttpContextBase httpcontext, string productid)
        {
            Basket basket = GetBasket(httpcontext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.ProductId == productid);

            if (item == null)
            {

                item = new BasketItem()
                {
                    BasketId = basket.Id,
                    ProductId = productid,
                    Quantity = 1
                };
                basket.BasketItems.Add(item);
            }
            else
            {
                item.Quantity = item.Quantity + 1;
            }
            BasketContext.commit();
            //return karwa skta hu product Id
        }


        public void AddInOneTimeToBasket(HttpContextBase httpcontext, string productid)
        {
            Basket basket = GetBasket(httpcontext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.ProductId == productid);

            if (item == null)
            {

                item = new BasketItem()
                {
                    BasketId = basket.Id,
                    ProductId = productid,
                    Quantity = 1
                };
                basket.BasketItems.Add(item);
            }
            else
            {
                item.Quantity = item.Quantity + 1;
            }
            BasketContext.commit();
            //return karwa skta hu product Id
        }


        public void Remove(HttpContextBase httpcontext, string productid)
        {
            Basket basket = GetBasket(httpcontext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.ProductId == productid);
            if (item == null)
            {


            }
            else if (item.Quantity > 0)
            {
                item.Quantity = item.Quantity - 1;
            }
            else
            {

            }
            BasketContext.commit();
        }




        public void ClearBasket(HttpContextBase httpcontext)
        {
            Basket basket = GetBasket(httpcontext, false);
            basket.BasketItems.Clear();
            BasketContext.commit();
        }

        public List<BasketItemViewModel> GetBasketItem(HttpContextBase httpcontext)
        {
            Basket basket = GetBasket(httpcontext, false);

            if (basket != null)
            {
                var result = (from b in basket.BasketItems
                              join p in ProductContext.collection() on b.ProductId equals p.Id
                              select new BasketItemViewModel()
                              {
                                  Id = b.ProductId,
                                  Quantity = b.Quantity,
                                  ProductName = p.Name,
                                  Image = p.Image,
                                  Price = p.Price

                              }
                              ).ToList();
                return result;
            }
            else
            {
                return new List<BasketItemViewModel>();
            }
        }

        public BasketSummaryViewModel GetBasketSummary(HttpContextBase httpcontext)
        {
            Basket basket = GetBasket(httpcontext, false);
            BasketSummaryViewModel model = new BasketSummaryViewModel(0, 0);
            if (basket != null)
            {
                int? basketCount = (from item in basket.BasketItems
                                    select item.Quantity).Sum();
                decimal? basketTotal = (from item in basket.BasketItems
                                        join p in ProductContext.collection() on item.ProductId equals p.Id
                                        select item.Quantity * p.Price).Sum();
                model.BasketCount = basketCount ?? 0;
                model.BasketTotal = basketTotal ?? decimal.Zero;
                return model;
            }
            else
            {
                return model;
            }
        }

        public void RemoveFromBasket(HttpContextBase httpcontext, string itemid)
        {
            Basket basket = GetBasket(httpcontext, true);
            BasketItem item = basket.BasketItems.FirstOrDefault(i => i.ProductId == itemid);

            if (item != null)
            {
                basket.BasketItems.Remove(item);
                BasketContext.commit();
            }
        }


    }
}
