using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using POS_System.Model;

namespace POS_System.Services
{
    public class TransactionService
    {
        private readonly StockRepository _stockRepository;

        public TransactionService()
        {
            _stockRepository = new StockRepository();
        }

        public double CalculateGrandTotal(List<CartItem> cart)
        {
            return cart.Sum(item => item.Total);
        }

        public void ProcessTransaction(List<CartItem> cart, string paymentType)
        {
            if (cart == null || cart.Count == 0)
            {
                throw new ArgumentException("Cart cannot be empty.");
            }

            double totalAmount = CalculateGrandTotal(cart);

            // 1. Decrease Stock for each item
            foreach (var item in cart)
            {
                _stockRepository.DecreaseStockQuantity(item.ItemCode, item.Quantity);
            }

            // 2. Record Sale History
            // 2. Record Sale History
            string cashier = UserSession.CurrentUserName ?? "Unknown";
            _stockRepository.RecordSale(totalAmount, paymentType, cashier);
        }
    }
}
