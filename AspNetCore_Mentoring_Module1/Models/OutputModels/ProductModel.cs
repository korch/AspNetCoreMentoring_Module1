using System.Collections.Generic;

namespace AspNetCore_Mentoring_Module1.Models.OutputModels
{
    public class ProductModel : Products
    {
        public string SupplierName { get; set; }
        public string CategoryName { get; set; }
   
        public virtual IEnumerable<Categories> Categories { get; set; }
        public virtual IEnumerable<Suppliers> Suppliers { get; set; }
    }
}
