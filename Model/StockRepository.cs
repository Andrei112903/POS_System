using System;
using System.Collections.Generic;
using System.Linq;

namespace POS_System.Model
{
    public class StockRepository
    {
        // Connection string name from App.config
        private readonly string connectionString;

        public StockRepository()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["POS_DB"].ConnectionString;
        }

        public void AddStock(Stock item)
        {
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                string query = "INSERT INTO Stocks (ItemCode, Description, Quantity, PurchasingPrice, SellingPrice, OrderNumber, Supplier) " +
                               "VALUES (@code, @desc, @qty, @pPrice, @sPrice, @order, @supplier)";
                
                using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@code", item.ItemCode);
                    cmd.Parameters.AddWithValue("@desc", item.Product);
                    cmd.Parameters.AddWithValue("@qty", item.Quantity);
                    cmd.Parameters.AddWithValue("@pPrice", item.PurchasingPrice);
                    cmd.Parameters.AddWithValue("@sPrice", item.SellingPrice);
                    cmd.Parameters.AddWithValue("@order", item.OrderNumber);
                    cmd.Parameters.AddWithValue("@supplier", item.Supplier);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Stock> GetAllItems()
        {
            List<Stock> stocks = new List<Stock>();

            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM Stocks";

                using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    conn.Open();
                    using (MySql.Data.MySqlClient.MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Map database row to Stock object
                            // Handle DBNull if necessary (assuming fields are Not Null based on setup script)
                            string code = reader["ItemCode"].ToString();
                            string desc = reader["Description"].ToString();
                            double qty = Convert.ToDouble(reader["Quantity"]);
                            double pPrice = Convert.ToDouble(reader["PurchasingPrice"]);
                            double sPrice = Convert.ToDouble(reader["SellingPrice"]);
                            string order = reader["OrderNumber"].ToString();
                            string supplier = reader["Supplier"].ToString();

                            stocks.Add(new Stock(code, desc, qty, pPrice, sPrice, order, supplier));
                        }
                    }
                }
            }
            return stocks;
        }

        public int GetItemCount()
        {
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                // Using COUNT query is more efficient than fetching all items
                string query = "SELECT COUNT(*) FROM Stocks";
                using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public double GetTotalPurchasingValue()
        {
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                // Use ISNULL or COALESCE to handle empty table case
                string query = "SELECT IFNULL(SUM(Quantity * PurchasingPrice), 0) FROM Stocks";
                using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    conn.Open();
                    return Convert.ToDouble(cmd.ExecuteScalar());
                }
            }
        }

        public void UpdateStock(string itemCode, Stock updatedItem)
        {
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                string query = "UPDATE Stocks SET Description=@desc, Quantity=@qty, PurchasingPrice=@pPrice, " +
                               "SellingPrice=@sPrice, OrderNumber=@order, Supplier=@supplier WHERE ItemCode=@code";

                using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@code", itemCode);
                    cmd.Parameters.AddWithValue("@desc", updatedItem.Product);
                    cmd.Parameters.AddWithValue("@qty", updatedItem.Quantity);
                    cmd.Parameters.AddWithValue("@pPrice", updatedItem.PurchasingPrice);
                    cmd.Parameters.AddWithValue("@sPrice", updatedItem.SellingPrice);
                    cmd.Parameters.AddWithValue("@order", updatedItem.OrderNumber);
                    cmd.Parameters.AddWithValue("@supplier", updatedItem.Supplier);

                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteStock(string itemCode)
        {
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                string query = "DELETE FROM Stocks WHERE ItemCode = @code";
                using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@code", itemCode);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Stock> SearchStocks(string query)
        {
            List<Stock> stocks = new List<Stock>();
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                string sql = "SELECT * FROM Stocks WHERE ItemCode LIKE @q OR Description LIKE @q OR Supplier LIKE @q";
                using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@q", "%" + query + "%");
                    conn.Open();
                    using (MySql.Data.MySqlClient.MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            stocks.Add(new Stock(
                                reader["ItemCode"].ToString(),
                                reader["Description"].ToString(),
                                Convert.ToDouble(reader["Quantity"]),
                                Convert.ToDouble(reader["PurchasingPrice"]),
                                Convert.ToDouble(reader["SellingPrice"]),
                                reader["OrderNumber"].ToString(),
                                reader["Supplier"].ToString()
                            ));
                        }
                    }
                }
            }
            return stocks;
        }

        public int GetLowStockCount(double threshold = 5.0)
        {
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Stocks WHERE Quantity <= @threshold";
                using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@threshold", threshold);
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public bool TestConnection(out string message)
        {
            try
            {
                using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
                {
                    conn.Open();
                    message = "Connection Successful!";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = "Connection Failed: " + ex.Message;
                return false;
            }
        }
        public Stock GetStockByCode(string itemCode)
        {
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM Stocks WHERE ItemCode = @code";
                using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@code", itemCode);
                    conn.Open();
                    using (MySql.Data.MySqlClient.MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Stock(
                                reader["ItemCode"].ToString(),
                                reader["Description"].ToString(),
                                Convert.ToDouble(reader["Quantity"]),
                                Convert.ToDouble(reader["PurchasingPrice"]),
                                Convert.ToDouble(reader["SellingPrice"]),
                                reader["OrderNumber"].ToString(),
                                reader["Supplier"].ToString()
                            );
                        }
                    }
                }
            }
            return null;
        }
        public List<Stock> GetTopStocks(int limit)
        {
            List<Stock> stocks = new List<Stock>();
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                // Fetch top items by quantity (descending)
                string query = "SELECT * FROM Stocks ORDER BY Quantity DESC LIMIT @limit";
                using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@limit", limit);
                    conn.Open();
                    using (MySql.Data.MySqlClient.MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            stocks.Add(new Stock(
                                reader["ItemCode"].ToString(),
                                reader["Description"].ToString(),
                                Convert.ToDouble(reader["Quantity"]),
                                Convert.ToDouble(reader["PurchasingPrice"]),
                                Convert.ToDouble(reader["SellingPrice"]),
                                reader["OrderNumber"].ToString(),
                                reader["Supplier"].ToString()
                            ));
                        }
                    }
                }
            }
            return stocks;
        }

        // Dashboard and Sales Methods

        public void CreateSalesTableIfNotExists()
        {
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                conn.Open();
                string query = @"CREATE TABLE IF NOT EXISTS Sales (
                                    Id INT AUTO_INCREMENT PRIMARY KEY,
                                    TransactionDate DATETIME NOT NULL,
                                    TotalAmount DECIMAL(18, 2) NOT NULL,
                                    PaymentType VARCHAR(50) NOT NULL,
                                    CashierName VARCHAR(100) DEFAULT 'Unknown'
                                 )";
                using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Temporary migration check for existing table without CashierName
                try
                {
                    string checkCol = "SELECT CashierName FROM Sales LIMIT 1";
                    using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(checkCol, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch
                {
                    string addCol = "ALTER TABLE Sales ADD COLUMN CashierName VARCHAR(100) DEFAULT 'Unknown'";
                    using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(addCol, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public void RecordSale(double amount, string type, string cashierName)
        {
            CreateSalesTableIfNotExists(); // Ensure table exists and has columns
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                string query = "INSERT INTO Sales (TransactionDate, TotalAmount, PaymentType, CashierName) VALUES (@date, @amount, @type, @cashier)";
                using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@date", DateTime.Now);
                    cmd.Parameters.AddWithValue("@amount", amount);
                    cmd.Parameters.AddWithValue("@type", type);
                    cmd.Parameters.AddWithValue("@cashier", cashierName);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Keep overload for backward compatibility if needed, or refactor all calls.
        // For now, I will assume we update the caller.

        public int GetDailySalesCount(DateTime date)
        {
            CreateSalesTableIfNotExists();
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                string query = "SELECT COUNT(*) FROM Sales WHERE DATE(TransactionDate) = @date";
                using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@date", date.Date);
                    conn.Open();
                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }
        }

        public double GetDailySalesTotal(DateTime date)
        {
            CreateSalesTableIfNotExists();
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                string query = "SELECT SUM(TotalAmount) FROM Sales WHERE DATE(TransactionDate) = @date";
                using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@date", date.Date);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != DBNull.Value ? Convert.ToDouble(result) : 0;
                }
            }
        }

        public System.Data.DataTable GetSales(DateTime start, DateTime end)
        {
            CreateSalesTableIfNotExists();
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                string query = "SELECT Id, TransactionDate, TotalAmount, PaymentType, CashierName FROM Sales WHERE DATE(TransactionDate) BETWEEN @start AND @end";
                using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@start", start.Date);
                    cmd.Parameters.AddWithValue("@end", end.Date);
                    
                    using (MySql.Data.MySqlClient.MySqlDataAdapter adapter = new MySql.Data.MySqlClient.MySqlDataAdapter(cmd))
                    {
                        System.Data.DataTable dt = new System.Data.DataTable();
                        adapter.Fill(dt);
                        return dt;
                    }
                }
            }
        }

        public double GetDailySalesTotalByType(DateTime date, string type)
        {
            CreateSalesTableIfNotExists();
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                string query = "SELECT SUM(TotalAmount) FROM Sales WHERE DATE(TransactionDate) = @date AND PaymentType = @type";
                using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@date", date.Date);
                    cmd.Parameters.AddWithValue("@type", type);
                    conn.Open();
                    object result = cmd.ExecuteScalar();
                    return result != DBNull.Value ? Convert.ToDouble(result) : 0;
                }
            }
        }

        public void DecreaseStockQuantity(string itemCode, int quantity)
        {
            using (MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection(connectionString))
            {
                string query = "UPDATE Stocks SET Quantity = Quantity - @qty WHERE ItemCode = @code";
                using (MySql.Data.MySqlClient.MySqlCommand cmd = new MySql.Data.MySqlClient.MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@qty", quantity);
                    cmd.Parameters.AddWithValue("@code", itemCode);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
