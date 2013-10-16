using System;
using System.Collections.Generic;
using System.Text;

namespace OmniQuery.CodeAnalytics
{
    /// <summary>
    /// Extend CodeAnalysis with enumerations
    /// </summary>
    public sealed partial class CodeAnalysisDb
    {
        #region Database Field Enumerations

        private enum AssemblyField
        {
            Name,
            FullName,
            Version,
            Culture,
            PublicKeyToken,
            MainModuleId,
            ModuleCount,
            ResourceCount,
            TypeCount,
            MethodCount
        }

        private enum ModuleField
        {
            AssemblyId,
            Name,
            FullPath,
            ModuleType,
            TargetRuntimeVersion,
            TargetPlatform,
            ContainsNativeCode,
            IsStrongNameSigned,
            EntryPointMethodId,
            ResourceCount,
            TypeCount,
            MethodCount
        }

        private enum TypeField
        {
            AssemblyId,
            ModuleId,
            Name,
            FullName,
            FullReferenceName,
            Namespace,
            MethodCount
        }

        private enum TypeReferenceField
        {
            ReferenceName,
            FullReferenceName,
            Namespace
        }

        private enum MethodField
        {
            AssemblyId,
            ModuleId,
            TypeId,
            Name,
            FullName,
            ReturnType,
            FullReturnType
        }

        private enum InstructionField
        {
            AssemblyId,
            ModuleId,
            TypeId,
            MethodId,
            Value
        }

        private enum ResourceField
        {
            AssemblyId,
            ModuleId,
            Name,
            ResourceType,
            IsPublic
        }

        private enum AttributeField
        {
            AssemblyId,
            ModuleId,
            Name,
            FullName,
            AttributeType
        }

        #endregion

        #region Publicly Exposed Enumerations

        public enum ModuleType
        {
            Library,
            Console,
            Windows,
            NetModule
        }

        public enum TargetPlatform
        {
            AnyCPU,
            x86,
            x64,
            Itanium
        }

        public enum ResourceType
        {
            Linked,
            Embedded,
            AssemblyLinked
        }

        public enum AttributeType
        {
            Assembly,
            Module,
            //Class,
            //Struct,
            //Enum,
            //Constructor,
            //Method,
            //Property,
            //Field,
            //Event,
            //Interface,
            //Parameter,
            //Delegate,
            //ReturnValue,
            //GenericParameter
        }

        #endregion
    }
}
