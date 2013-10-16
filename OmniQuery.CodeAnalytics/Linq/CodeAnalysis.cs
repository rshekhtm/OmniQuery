using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace OmniQuery.CodeAnalytics.Linq
{
    public class CodeAnalysis
    {
        private List<QueryableAssembly> _assemblies = new List<QueryableAssembly>();
        private List<QueryableModule> _modules = new List<QueryableModule>();
        private List<QueryableResource> _resources = new List<QueryableResource>();
        private List<QueryableType> _types = new List<QueryableType>();
        private List<QueryableMethod> _methods = new List<QueryableMethod>();

        public ReadOnlyCollection<QueryableAssembly> Assemblies
        {
            get
            {
                return _assemblies.AsReadOnly();
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

        public CodeAnalysis(params string[] assemblyPaths)
        {
            if (assemblyPaths != null && assemblyPaths.Length > 0)
            {
                foreach (string assemblyPath in assemblyPaths)
                {
                    if (!File.Exists(assemblyPath))
                    {
                        throw new ArgumentException("Assembly not found at the specified path: " + assemblyPath);
                    }
                }

                foreach (string assemblyPath in assemblyPaths)
                {
                    QueryableAssembly assembly = new QueryableAssembly(assemblyPath);
                    _assemblies.Add(assembly);
                    _modules.AddRange(assembly.Modules);
                    _resources.AddRange(assembly.Resources);
                    _types.AddRange(assembly.Types);
                    _methods.AddRange(assembly.Methods);
                }
            }
            else
            {
                throw new ArgumentException("No assemblies specified for code analysis");
            }
        }
    }
}
