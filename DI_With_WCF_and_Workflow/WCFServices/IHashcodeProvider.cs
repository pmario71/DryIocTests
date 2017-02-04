using System;
using System.ComponentModel.Composition;

namespace DI_With_WCF_and_Workflow.WCFServices
{
    public interface IHashcodeProvider
    {
        int GetInstanceId();

        bool IsDisposed { get; }
    }

    [Export(typeof(IHashcodeProvider))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    class HashcodeProvider : IHashcodeProvider, IDisposable
    {
        public int GetInstanceId()
        {
            return this.GetHashCode();
        }

        public void Dispose()
        {
            this.IsDisposed = true;
        }

        public bool IsDisposed { get; private set; }
    }

}