using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyEmployees.Core.Domain.LinkModels
{
    public class LinkCollectionWrapper<T> : LinkResourceBase
    {
        public List<T> Values { get; set; } = new List<T>();

        public LinkCollectionWrapper()
        {

        }

        public LinkCollectionWrapper(List<T> value) => Values = value ?? new List<T>();
    }
}
