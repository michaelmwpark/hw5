using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Park_Michael_HW5.Models
{
    public class OrderDetail
    {
        public Int32 OrderDetailID { get; set; }

        [Range(1, Double.PositiveInfinity, ErrorMessage = "Quantity must be greater than 0")]
        [Display(Name = "Quantity:")]
        public Int32 Quantity { get; set; }

        [Display(Name = "Product Price")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal ProductPrice { get; set; }

        [Display(Name = "Extended Price")]
        [DisplayFormat(DataFormatString = "{0:C}")]
        public Decimal ExtendedPrice { get; set; }

        public Order Order { get; set; }
        public Product Product { get; set; }
    }
}
