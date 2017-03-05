using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;
using System.Threading.Tasks;

namespace DI_With_WCF_and_Workflow.DI.MEF
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", 
        Justification = "WCF does not use IDisposable but calls ReleaseInstance() to trigger freeing of resources.")]
    [Export(typeof(ComposedInstanceProvider<>))]
    internal class ComposedInstanceProvider<T> : IInstanceProvider
    {
        readonly ConcurrentDictionary<int,IDisposable> _disposables = new ConcurrentDictionary<int, IDisposable>();

        [Import]
        private ExportFactory<T> _exportFactory;

        [ImportingConstructor]
        public ComposedInstanceProvider()
        {
        }

        public object GetInstance(InstanceContext context)
        {
            ExportLifetimeContext<T> exportLifetimeContext = _exportFactory.CreateExport();
            _disposables.AddOrUpdate(context.GetHashCode(), _ => exportLifetimeContext, (a,b) => exportLifetimeContext);

            return exportLifetimeContext.Value;
        }

        public object GetInstance(InstanceContext context, Message message)
        {
            return GetInstance(context);
        }

        public void ReleaseInstance(InstanceContext context, object instance)
        {
            IDisposable d;
            if (_disposables.TryRemove(context.GetHashCode(), out d))
            {
                d.Dispose();
            }
        }
    }
}
