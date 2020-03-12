using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCore_Mentoring_Module1.Interfaces
{
    public interface IOptionsProvider
    {
        IOptions GetOptions();
    }
}
