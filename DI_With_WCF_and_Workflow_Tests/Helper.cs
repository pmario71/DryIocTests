using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI_With_WCF_and_Workflow_Tests
{
    class Helper
    {

        /// <summary>
        /// Returns a local, unique service uri, which contains the name of the contract and a guid.
        /// In case <see cref="id"/> is specified, the id is used instead of the guid.
        /// </summary>
        /// <typeparam name="TContract"></typeparam>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Uri UriFromContract<TContract>(string id=null)
        {
            string rand = id ?? Guid.NewGuid().ToString();

            return new Uri($"net.pipe://localhost/{typeof(TContract).Name}_{rand}");
        }

    }
}
