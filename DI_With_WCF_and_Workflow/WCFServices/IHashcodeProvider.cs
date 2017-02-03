using System;
using System.ComponentModel.Composition;

namespace DI_With_WCF_and_Workflow.WCFServices
{
    public interface IHashcodeProvider
    {
        int GetInstanceId();
    }

    [Export(typeof(IHashcodeProvider))]
    class MyClass : IHashcodeProvider
    {
        public int GetInstanceId()
        {
            return this.GetHashCode();
        }
    }

}