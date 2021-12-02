using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace Park_Michael_HW5.Models
{
    public enum Type { hat, pants, sweatshirt, tank, tshirt, other  }

    public class Product
    {
        public Int32 ProductID { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [Display(Name = "Product Name")]
        public String ProductName { get; set; }

        public String Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Display(Name = "Price")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal Price { get; set; }

        [Display(Name = "Product Type")]
        public Type ProductType { get; set; }

        public List<Supplier> Suppliers { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }

        public Product()
        {
            if (Suppliers == null)
            {
                Suppliers = new List<Supplier>();
            }

            if (OrderDetails == null)
            {
                OrderDetails = new List<OrderDetail>();
            }
        }
    }
}
