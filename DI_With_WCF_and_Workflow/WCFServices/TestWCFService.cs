using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace DI_With_WCF_and_Workflow.WCFServices
{
    [ServiceContract]
    public interface ITestWCFService
    {

        [OperationContract(Name ="ExecuteShortRunningOperation")]
        Task<int> ExecuteShortRunningOperationAsync(int input);

        [OperationContract(Name = "ExecuteOperationUsingTransaction")]
        Task<int> ExecuteOperationUsingTransactionAsync(int input);
    }

    [Export(typeof(TestWCFService))]
    public class TestWCFService : ITestWCFService
    {
        /// <summary>
        /// Showcases async Transaction flow
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<int> ExecuteOperationUsingTransactionAsync(int input)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required, 
                TimeSpan.FromSeconds(5), 
                TransactionScopeAsyncFlowOption.Enabled))
            {

                await DBOperationsExtensions.UpdateDB(input);

                scope.Complete();
            }

            return input;
        }

        public async Task<int> ExecuteShortRunningOperationAsync(int input)
        {
            await Task.Delay(input);
            return 42;
        }
    }
}
