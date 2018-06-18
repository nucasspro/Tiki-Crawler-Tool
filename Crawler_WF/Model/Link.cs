using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler_WF.Model
{
    public class Link
    {
        private string _productLink;

        public string ProductLink { get { return _productLink; } set { _productLink = value; } }

        public Link(string link)
        {
            this._productLink = link;
        }
    }
}
