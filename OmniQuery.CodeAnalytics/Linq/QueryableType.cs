using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Mono.Cecil;

namespace OmniQuery.CodeAnalytics.Linq
{
    public class QueryableType
    {
        private QueryableAssembly _assembly;
        private QueryableModule _module;
        private QueryableType _declaringType;
        private string _name;
        private string _fullName;
        private List<QueryableType> _nestedTypes = new List<QueryableType>();
        private List<QueryableMethod> _methods = new List<QueryableMethod>();

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

        public QueryableType DeclaringType
        {
            get
            {
                return _declaringType;
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

        public ReadOnlyCollection<QueryableType> NestedTypes
        {
            get
            {
                return _nestedTypes.AsReadOnly();
            }
        }

        public ReadOnlyCollection<QueryableMethod> Methods
        {
            get
            {
                return _methods.AsReadOnly();
            }
        }

        internal QueryableType(TypeDefinition typeDefinition, QueryableModule module, QueryableType declaringType)
        {
            _assembly = module.Assembly;
            _module = module;
            _declaringType = declaringType;
            _name = typeDefinition.Name;
            _fullName = typeDefinition.FullName;

            foreach (TypeDefinition nestedType in typeDefinition.NestedTypes)
            {
                _nestedTypes.Add(new QueryableType(nestedType, module, this));
            }

            foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
            {
                _methods.Add(new QueryableMethod(methodDefinition, this));
            }
        }
    }
}
