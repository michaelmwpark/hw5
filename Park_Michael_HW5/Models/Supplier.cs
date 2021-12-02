using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Park_Michael_HW5.Models
{
    public class Supplier
    {
        public Int32 SupplierID { get; set; }

        [Display(Name = "Supplier Name")]
        public String SupplierName { get; set; }

        [Display(Name = "Email")]
        [DataType(DataType.EmailAddress)]
        public String Email { get; set; }

        [Display(Name = "Phone Number")]
        [DataType(DataType.PhoneNumber)]
        public String PhoneNumber { get; set; }

        public List<Product> Products { get; set; }

        public Supplier()
        {
            if (Products == null)
            {
                Products = new List<Product>();
            }
        }

    }
}
