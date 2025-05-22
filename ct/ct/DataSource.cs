using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ct
{
    public class DataSource
    {
        /// <summary>
        /// Specifies if the data source is live (data from an exchange) or from a offline file for back-testing.
        /// </summary>
        public virtual bool Live => false;
    }
}
