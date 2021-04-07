using FrozenFoods.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenFoods.Core.ViewModel
{
    public class ProductManagerViewModel
    {
        public Product Product { get; set; }
        public IEnumerable<ProductCategory> ProductCategory { get; set; }

        public static explicit operator ProductManagerViewModel(Product v)
        {
            throw new NotImplementedException();
        }
    }
}
