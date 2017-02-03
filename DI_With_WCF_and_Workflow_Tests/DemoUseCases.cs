using DI_With_WCF_and_Workflow;
using DI_With_WCF_and_Workflow.WCFServices;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace DI_With_WCF_and_Workflow_Tests
{
    [TestFixture(Explicit =true)]
    public class DemoUseCases
    {
        private Host _host;
        private List<IDisposable> _clients = new List<IDisposable>();

        [TearDown]
        public void Dispose()
        {
            if (_clients.Any())
            {
                foreach (var c in _clients)
                {
                    c.Dispose();
                }
                _clients.Clear();
            }

            if (_host != null)
            {
                _host.Dispose();
                _host = null;
            }
        }

        [Test]
        public async Task Call_simple_operation_using_async_API()
        {
            StartHost();

            var client = CreateClient();
            await client.ExecuteShortRunningOperationAsync(300);
        }

        [Test]
        public async Task Call_operation_using_transaction()
        {
            StartHost();

            var client = CreateClient();
            await client.ExecuteOperationUsingTransactionAsync(300);
        }

        #region Internals
        private ITestWCFService CreateClient()
        {
            var client = ChannelFactory<ITestWCFService>.CreateChannel(
                Host.DefaultBinding,
                new EndpointAddress(_host.Address));
            _clients.Add((IDisposable)client);

            return client;
        }

        private Host StartHost()
        {
            _host = new Host();
            _host.Initialize();

            return _host;
        }
        #endregion
    }
}
