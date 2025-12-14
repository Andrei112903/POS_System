using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace POS_System.Model
{
    public class SupplierRepository
    {
        private readonly string connectionString;

        public SupplierRepository()
        {
            connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["POS_DB"].ConnectionString;
            CreateSupplierTableIfNotExists();
        }

        public void CreateSupplierTableIfNotExists()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = @"CREATE TABLE IF NOT EXISTS Suppliers (
                                    Id INT AUTO_INCREMENT PRIMARY KEY,
                                    Name VARCHAR(255) NOT NULL,
                                    Address VARCHAR(255),
                                    Email VARCHAR(255),
                                    Contact VARCHAR(50)
                                 )";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void AddSupplier(Supplier supplier)
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "INSERT INTO Suppliers (Name, Address, Email, Contact) VALUES (@name, @address, @email, @contact)";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", supplier.Name);
                    cmd.Parameters.AddWithValue("@address", supplier.Address);
                    cmd.Parameters.AddWithValue("@email", supplier.Email);
                    cmd.Parameters.AddWithValue("@contact", supplier.Contact);
                    
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Supplier> GetAllSuppliers()
        {
            List<Supplier> list = new List<Supplier>();
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                string query = "SELECT * FROM Suppliers";
                using (MySqlCommand cmd = new MySqlCommand(query, conn))
                {
                    conn.Open();
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Supplier(
                                Convert.ToInt32(reader["Id"]),
                                reader["Name"].ToString(),
                                reader["Address"].ToString(),
                                reader["Email"].ToString(),
                                reader["Contact"].ToString()
                            ));
                        }
                    }
                }
            }
            return list;
        }
    }
}
