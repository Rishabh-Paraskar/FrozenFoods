using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrozenFoods.Core.ViewModel
{
    public class BasketSummaryViewModel
    {
        public int BasketCount { get; set; }
        public decimal BasketTotal { get; set; }

        public BasketSummaryViewModel()
        {
        }
        public BasketSummaryViewModel(int basketcount, decimal baskettotal)
        {
            this.BasketCount = basketcount;
            this.BasketTotal = baskettotal;
        }
    }
}
