using System;

namespace POS_System.Model
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Contact { get; set; }

        public Supplier() { }

        public Supplier(string name, string address, string email, string contact)
        {
            Name = name;
            Address = address;
            Email = email;
            Contact = contact;
        }

        public Supplier(int id, string name, string address, string email, string contact)
        {
            Id = id;
            Name = name;
            Address = address;
            Email = email;
            Contact = contact;
        }
    }
}
