using System;
using System.ComponentModel;

namespace elbbp_ui
{
    // https://stackoverflow.com/questions/1620847/how-can-i-get-visual-studio-2008-windows-forms-designer-to-render-a-form-that-im/17661276#17661276
    internal class AbstractFormTypeDescriptionProvider<TAbstract, TBase> : TypeDescriptionProvider
    {
        public AbstractFormTypeDescriptionProvider()
            : base(TypeDescriptor.GetProvider(typeof(TAbstract)))
        {

        }

        public override Type GetReflectionType(Type objectType, object instance)
        {
            if (objectType == typeof(TAbstract))
            {
                return typeof(TBase);
            }

            return base.GetReflectionType(objectType, instance);
        }

        public override object CreateInstance(IServiceProvider provider, Type objectType, Type[] argTypes, object[] args)
        {
            if (objectType == typeof(TAbstract))
            {
                objectType = typeof(TBase);
            }

            return base.CreateInstance(provider, objectType, argTypes, args);
        }
    }
}
