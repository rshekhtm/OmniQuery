using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;

namespace OmniQuery.CodeAnalytics.Linq
{
    public class QueryableMethod
    {
        private QueryableAssembly _assembly;
        private QueryableModule _module;
        private QueryableType _type;
        private string _name;
        private string _fullName;
        private List<QueryableMethodReference> _referencedMethods = new List<QueryableMethodReference>();
        private List<QueryableMethod> _referencingMethods = new List<QueryableMethod>();

        public QueryableAssembly Assembly
        {
            get
            {
                return _assembly;
            }
        }

        public QueryableModule Module
        {
            get
            {
                return _module;
            }
        }

        public QueryableType Type
        {
            get
            {
                return _type;
            }
        }

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

        internal QueryableMethod(MethodDefinition methodDefinition, QueryableType type)
        {
            _assembly = type.Assembly;
            _module = type.Module;
            _type = type;
            _name = methodDefinition.Name;
            _fullName = methodDefinition.FullName;

            if (methodDefinition == methodDefinition.Module.EntryPoint)
            {
                _module.EntryPointMethod = this;
            }
        }
    }
}
