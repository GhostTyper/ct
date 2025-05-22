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

        /// <summary>
        /// Retrieves messages from the data source. Implementations typically
        /// yield messages endlessly from an exchange or a file.
        /// </summary>
        /// <returns>An asynchronous stream of <see cref="Message"/> objects.</returns>
        public virtual async IAsyncEnumerable<Message> GetMessagesAsync()
        {
            await Task.CompletedTask;
            yield break;
        }
    }
}
