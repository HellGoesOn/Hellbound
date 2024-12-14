using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Casull.Core.ECS.Components
{
    public class Tags : IComponent
    {
        public string[] tags;

        public Tags(params string[] tags) 
        {
            this.tags = tags;
        }

        public bool Has(string tag) => tags.Contains(tag);
    }
}
