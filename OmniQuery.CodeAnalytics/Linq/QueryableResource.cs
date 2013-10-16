using System;
using System.Collections.Generic;
using System.Text;
using Mono.Cecil;

namespace OmniQuery.CodeAnalytics.Linq
{
    public class QueryableResource
    {
        private QueryableAssembly _assembly;
        private QueryableModule _module;
        private string _name;
        private ResourceType _resourceType;
        private bool _isPublic;

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

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public ResourceType ResourceType
        {
            get
            {
                return _resourceType;
            }
        }

        public bool IsPublic
        {
            get
            {
                return _isPublic;
            }
        }

        internal QueryableResource(Resource resource, QueryableModule module)
        {
            _assembly = module.Assembly;
            _module = module;
            _name = Utility.ToString(resource.Name);
            _resourceType = Utility.ConvertEnum<ResourceType>(resource.ResourceType);
            _isPublic = resource.IsPublic;
        }
    }
}
