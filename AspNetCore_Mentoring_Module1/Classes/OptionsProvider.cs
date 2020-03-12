using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore_Mentoring_Module1.Interfaces;
using Microsoft.Extensions.Configuration;

namespace AspNetCore_Mentoring_Module1.Classes
{
    public class OptionsProvider : IOptionsProvider
    {
        private readonly IOptions _options;

        public OptionsProvider(IOptions options)
        {
            _options = options;
        }

        public IOptions GetOptions()
        {
            return _options;
        }
    }
}
