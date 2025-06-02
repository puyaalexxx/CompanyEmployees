using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Core.Domain.LinkModels
{
    public class Link
    {
        public string? Href { get; set; }
        public string? Rel { get; set; }
        public string? Method { get; set; }

        //needed for XML serialization
        public Link()
        {
        }

        public Link(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
    }
}
