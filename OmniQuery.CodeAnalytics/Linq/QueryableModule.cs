using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using Mono.Cecil;

namespace OmniQuery.CodeAnalytics.Linq
{
    public class QueryableModule
    {
        private QueryableAssembly _assembly;
        private string _name;
        private string _fullPath;
        private ModuleType _moduleType;
        private string _targetRuntimeVersion;
        private TargetPlatform _targetPlatform;
        private bool _hasNativeCode;
        private bool _isStrongNameSigned;
        private QueryableMethod _entryPointMethod;
        private List<QueryableResource> _resources = new List<QueryableResource>();
        private List<QueryableType> _types = new List<QueryableType>();
        private List<QueryableMethod> _methods = new List<QueryableMethod>();

        public QueryableAssembly Assembly
        {
            get
            {
                return _assembly;
            }
        }

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string FullPath
        {
            get
            {
                return _fullPath;
            }
        }

        public ModuleType ModuleType
        {
            get
            {
                return _moduleType;
            }
        }

        public string TargetRuntimeVersion
        {
            get
            {
                return _targetRuntimeVersion;
            }
        }

        public TargetPlatform TargetPlatform
        {
            get
            {
                return _targetPlatform;
            }
        }

        public bool HasNativeCode
        {
            get
            {
                return _hasNativeCode;
            }
        }

        public bool IsStrongNameSigned
        {
            get
            {
                return _isStrongNameSigned;
            }
        }

        public QueryableMethod EntryPointMethod
        {
            get
            {
                return _entryPointMethod;
            }
            internal set
            {
                _entryPointMethod = value;
            }
        }

        public ReadOnlyCollection<QueryableResource> Resources
        {
            get
            {
                return _resources.AsReadOnly();
            }
        }

        public ReadOnlyCollection<QueryableType> Types
        {
            get
            {
                return _types.AsReadOnly();
            }
        }

        public ReadOnlyCollection<QueryableMethod> Methods
        {
            get
            {
                return _methods.AsReadOnly();
            }
        }

        internal QueryableModule(ModuleDefinition moduleDefinition, QueryableAssembly assembly)
        {
            _assembly = assembly;
            _name = Utility.ToString(_getName(moduleDefinition));
            _fullPath = Utility.ToString(moduleDefinition.FullyQualifiedName);
            _moduleType = _getModuleType(moduleDefinition);
            _targetRuntimeVersion = Utility.ToString(_getTargetRuntimeVersion(moduleDefinition));
            _targetPlatform = _getTargetPlatform(moduleDefinition);
            _hasNativeCode = _getHasNativeCode(moduleDefinition);
            _isStrongNameSigned = _getIsStrongNameSigned(moduleDefinition);

            if (moduleDefinition.Assembly != null && moduleDefinition == moduleDefinition.Assembly.MainModule)
            {
                _assembly.MainModule = this;
            }

            foreach (Resource resource in moduleDefinition.Resources)
            {
                _resources.Add(new QueryableResource(resource, this));
            }

            foreach (TypeDefinition typeDefinition in moduleDefinition.Types)
            {
                QueryableType type = new QueryableType(typeDefinition, this, null);
                _addTypesAndMethods(type);
            }
        }

        private string _getName(ModuleDefinition moduleDefinition)
        {
            string name = moduleDefinition.Name;
            if (name == null || name.Trim() == string.Empty)
            {
                name = Path.GetFileName(moduleDefinition.FullyQualifiedName);
            }
            return name;
        }

        private ModuleType _getModuleType(ModuleDefinition moduleDefinition)
        {
            ModuleType moduleType = ModuleType.Library;
            if (moduleDefinition.Kind != ModuleKind.Dll)
            {
                moduleType = Utility.ConvertEnum<ModuleType>(moduleDefinition.Kind);
            }
            return moduleType;
        }

        private string _getTargetRuntimeVersion(ModuleDefinition moduleDefinition)
        {
            return moduleDefinition.Runtime.ToString().Replace("Net_", "").Replace("_", ".");
        }

        private TargetPlatform _getTargetPlatform(ModuleDefinition moduleDefinition)
        {
            TargetPlatform targetPlatform = TargetPlatform.Unknown;
            if (moduleDefinition.Architecture == TargetArchitecture.I386 &&
                (moduleDefinition.Attributes & ModuleAttributes.ILOnly) == ModuleAttributes.ILOnly &&
                (moduleDefinition.Attributes & ModuleAttributes.Required32Bit) != ModuleAttributes.Required32Bit)
            {
                targetPlatform = TargetPlatform.AnyCPU;
            }
            else if (moduleDefinition.Architecture == TargetArchitecture.I386)
            {
                targetPlatform = TargetPlatform.x86;
            }
            else if (moduleDefinition.Architecture == TargetArchitecture.AMD64)
            {
                targetPlatform = TargetPlatform.x64;
            }
            else if (moduleDefinition.Architecture == TargetArchitecture.IA64)
            {
                targetPlatform = TargetPlatform.Itanium;
            }
            return targetPlatform;
        }

        private bool _getHasNativeCode(ModuleDefinition moduleDefinition)
        {
            return (moduleDefinition.Attributes & ModuleAttributes.ILOnly) != ModuleAttributes.ILOnly;
        }

        private bool _getIsStrongNameSigned(ModuleDefinition moduleDefinition)
        {
            return (moduleDefinition.Attributes & ModuleAttributes.StrongNameSigned) == ModuleAttributes.StrongNameSigned;
        }

        private void _addTypesAndMethods(QueryableType type)
        {
            _types.Add(type);
            _methods.AddRange(type.Methods);

            foreach (QueryableType nestedType in type.NestedTypes)
            {
                _addTypesAndMethods(nestedType);
            }
        }
    }
}
