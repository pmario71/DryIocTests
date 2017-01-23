using SimpleInjector;
using SimpleInjector.Advanced;
using System;
using System.Reflection;

namespace SimpleInjectorTests
{

    public class ImplicitPropertyInjectionBehavior : IPropertySelectionBehavior
    {
        private readonly Container container;
        internal ImplicitPropertyInjectionBehavior(Container container)
        {
            this.container = container;
        }

        public bool SelectProperty(Type type, PropertyInfo property)
        {
            return this.IsImplicitInjectable(property);
        }

        private bool IsImplicitInjectable(PropertyInfo property)
        {
            return IsInjectableProperty(property) && this.IsRegistered(property);
        }

        private static bool IsInjectableProperty(PropertyInfo prop)
        {
            MethodInfo setMethod = prop.GetSetMethod(nonPublic: false);
            return setMethod != null && !setMethod.IsStatic && prop.CanWrite;
        }

        private bool IsRegistered(PropertyInfo property)
        {
            return this.container.GetRegistration(property.PropertyType) != null;
        }
    }
}
