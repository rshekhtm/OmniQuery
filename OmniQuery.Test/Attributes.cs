using System;
using System.Collections.Generic;
using System.Text;

namespace OmniQuery.Test
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class AssemblyAttribute : Attribute
    {
        public string InnerField;
        public string InnerProperty { get; set; }

        public AssemblyAttribute(string stringParam, int intParam, bool boolParam)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ClassAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Constructor)]
    public class ConstructorAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Delegate)]
    public class DelegateAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Enum)]
    public class EnumAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Event)]
    public class EventAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class FieldAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.GenericParameter)]
    public class GenericParameterAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Interface)]
    public class InterfaceAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class MethodAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Module)]
    public class ModuleAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Parameter)]
    public class ParameterAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.ReturnValue)]
    public class ReturnValueAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Struct)]
    public class StructAttribute : Attribute
    {
    }
}
