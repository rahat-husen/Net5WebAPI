using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.RequestFeatures
{
    public class CompanyParameters:RequestParameters
    {
        public CompanyParameters()
        {
            OrderBy = "Name";
        }
        public string OrderBy { get; set; }
    }
}
