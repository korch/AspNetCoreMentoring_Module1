using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore_Mentoring_Module1.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AspNetCore_Mentoring_Module1.Classes
{
    public class Options : IOptions
    {
        public int NumberOfItemsForPaging { get; private set; }

        public Options(IConfiguration configuration)
        {
            NumberOfItemsForPaging = int.Parse(configuration.GetSection(Constants.PagingSection).GetSection(Constants.PagingValueSection).Value);
        }
    }
}
