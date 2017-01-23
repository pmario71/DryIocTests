using System.ComponentModel.Composition.Hosting;

namespace DI_With_WCF_and_Workflow.DI.MEF
{
    public interface IContainerProvider
    {
        CompositionContainer GetContainer();
    }
}