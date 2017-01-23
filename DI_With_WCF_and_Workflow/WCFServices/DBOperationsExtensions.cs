using System;
using System.Threading.Tasks;
using System.Transactions;

namespace DI_With_WCF_and_Workflow.WCFServices
{

    public static class DBOperationsExtensions
    {
        public static Task<T> Query<T>()
        {
            return Task.FromResult(default(T));
        }

        public async static Task UpdateDB<T>(T entity)
        {
            await Task.Delay(100);

            if (Transaction.Current == null)
            {
                throw new Exception("Failed to acquire ambient transaction!");
            }

        }
    }
}
