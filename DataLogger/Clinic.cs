using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLogger
{
    public class Clinic
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string PostalCode { get; set; }
        public string Contact { get; set; }

        public Clinic()
        {
        }

        public Clinic(string name, string city, string street, string postalCode, string contact)
        {
            Name = name;
            City = city;
            Street = street;
            PostalCode = postalCode;
            Contact = contact;
        }
    }
}
