using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace POS_System.Model
{
    public class Stock
    {
        public string ItemCode { get; set; }
        public string Product { get; set; }
        public double Quantity { get; set; }
        public double PurchasingPrice { get; set; }
        public double PurchasingValue => Quantity * PurchasingPrice;
        public double SellingPrice { get; set; }
        public string OrderNumber { get; set; }
        public string Supplier { get; set; }

        public Stock(string itemCode, string product, double quantity, double purchasingPrice, double sellingPrice, string orderNumber, string supplier)
        {
            ItemCode = itemCode;
            Product = product;
            Quantity = quantity;
            PurchasingPrice = purchasingPrice;
            SellingPrice = sellingPrice;
            OrderNumber = orderNumber;
            Supplier = supplier;
        }

        public Stock() { }
    }
}
