using AspNetCore_Mentoring_Module1.Interfaces;
using AspNetCore_Mentoring_Module1.Models.OutputModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore_Mentoring_Module1.Classes
{
    public class ProductModelProvider : IProductModelProvider
    {
        public ProductModel GetProducts(Products product = null)
        {
            if (product == null) {
                return new ProductModel();
            }

            return new ProductModel {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                SupplierId = product.SupplierId,
                CategoryId = product.CategoryId,
                QuantityPerUnit = product.QuantityPerUnit,
                UnitPrice = product.UnitPrice,
                UnitsInStock = product.UnitsInStock,
                UnitsOnOrder = product.UnitsOnOrder,
                ReorderLevel = product.ReorderLevel,
                Discontinued = product.Discontinued,
            };
        }
    }
}
