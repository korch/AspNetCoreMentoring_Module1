using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore_Mentoring_Module1.Models.OutputModels;

namespace AspNetCore_Mentoring_Module1.Models
{
    public class ProductPagingViewModel
    {
        public IEnumerable<ProductModel> Products { get; set; }
        public PageViewModel PageViewModel { get; set; }
    }
}
