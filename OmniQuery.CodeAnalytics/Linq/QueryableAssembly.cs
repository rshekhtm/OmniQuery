using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Mono.Cecil;

namespace OmniQuery.CodeAnalytics.Linq
{
    public class QueryableAssembly
    {
        private string _name;
        private string _fullName;
        private string _version;
        private string _culture;
        private string _publicKeyToken;
        private QueryableModule _mainModule;
        private List<QueryableModule> _modules = new List<QueryableModule>();
        private List<QueryableResource> _resources = new List<QueryableResource>();
        private List<QueryableType> _types = new List<QueryableType>();
        private List<QueryableMethod> _methods = new List<QueryableMethod>();

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public string FullName
        {
            get
            {
                return _fullName;
            }
        }

        public string Version
        {
            get
            {
                return _version;
            }
        }

        public string Culture
        {
            get
            {
                return _culture;
            }
        }

        public string PublicKeyToken
        {
            get
            {
                return _publicKeyToken;
            }
        }

        public QueryableModule MainModule
        {
            get
            {
                return _mainModule;
            }
            internal set
            {
                _mainModule = value;
            }
        }

        public ReadOnlyCollection<QueryableModule> Modules
        {
            get
            {
                return _modules.AsReadOnly();
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

        internal QueryableAssembly(string assemblyPath)
        {
            AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(assemblyPath);

            _name = Utility.ToString(assemblyDefinition.Name.Name);
            _fullName = Utility.ToString(assemblyDefinition.Name.FullName);
            _version = Utility.ToString(assemblyDefinition.Name.Version);
            _culture = Utility.ToString(assemblyDefinition.Name.Culture);
            _publicKeyToken = Utility.ToString(assemblyDefinition.Name.PublicKeyToken);

            foreach (ModuleDefinition moduleDefinition in assemblyDefinition.Modules)
            {
                QueryableModule module = new QueryableModule(moduleDefinition, this);
                _modules.Add(module);
                _resources.AddRange(module.Resources);
                _types.AddRange(module.Types);
                _methods.AddRange(module.Methods);
            }
        }
    }
}
