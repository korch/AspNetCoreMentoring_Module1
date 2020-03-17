using AspNetCore_Mentoring_Module1.Models.OutputModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore_Mentoring_Module1.Interfaces
{
    public interface IProductModelProvider
    {
        ProductModel GetProducts(Products product);
    }
}
