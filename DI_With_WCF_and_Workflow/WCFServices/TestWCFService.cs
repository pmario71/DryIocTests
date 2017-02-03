using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.Serialization;
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

        [OperationContract(Name = "ResolveSharedResourceAsync")]
        Task<DependencyReport> ResolveSharedResourceAsync();
    }

    [ServiceContract]
    public interface ITestWCFService2
    {
        [OperationContract(Name = "ResolveSharedResourceAsync")]
        Task<DependencyReport> ResolveSharedResourceAsync();
    }

    [Export(typeof(TestWCFService))]
    public class TestWCFService : ITestWCFService
    {
        private IHashcodeProvider _provider;

        [ImportingConstructor]
        public TestWCFService(IHashcodeProvider provider)
        {
            _provider = provider;
        }

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

        public Task<DependencyReport> ResolveSharedResourceAsync()
        {
            var r = new DependencyReport();
            r.ServiceInstanceId = this.GetHashCode();
            r.DependencyInstanceId = _provider.GetInstanceId();

            return Task.FromResult(r);
        }
    }

    [Export(typeof(TestWCFService2))]
    public class TestWCFService2 : ITestWCFService2
    {
        private IHashcodeProvider _provider;

        [ImportingConstructor]
        public TestWCFService2(IHashcodeProvider provider)
        {
            _provider = provider;
        }

        public Task<DependencyReport> ResolveSharedResourceAsync()
        {
            var r = new DependencyReport();
            r.ServiceInstanceId = this.GetHashCode();
            r.DependencyInstanceId = _provider.GetInstanceId();

            return Task.FromResult(r);
        }
    }

    [DataContract]
    public class DependencyReport
    {
        [DataMember]
        public int ServiceInstanceId { get; set; }

        [DataMember]
        public int DependencyInstanceId { get; set; }
    }
}
