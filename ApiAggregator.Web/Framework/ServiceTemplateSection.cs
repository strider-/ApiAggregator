using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ApiAggregator.Web.Framework
{
    public class ServiceTemplateSection : ConfigurationSection
    {
        [ConfigurationProperty("services", IsRequired = true)]
        public ServiceTemplateCollection Templates
        {
            get { return this["services"] as ServiceTemplateCollection; }
        }
    }

    public class ServiceTemplateCollection : ConfigurationElementCollection
    {
        public ServiceTemplateElement this[int index]
        {
            get { return BaseGet(index) as ServiceTemplateElement; }
            set
            {
                if(BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceTemplateElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServiceTemplateElement)element).Name;
        }
    }

    public class ServiceTemplateElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return this["name"] as string; }
        }

        [ConfigurationProperty("rootUrl", IsRequired = true)]
        public string RootUrl
        {
            get { return this["rootUrl"] as string; }
        }

        [ConfigurationProperty("headers", IsRequired = false)]
        public ServiceTemplatePairCollection Headers
        {
            get { return this["headers"] as ServiceTemplatePairCollection; }
        }

        [ConfigurationProperty("queryStrings", IsRequired = false)]
        public ServiceTemplatePairCollection QueryStrings
        {
            get { return this["queryStrings"] as ServiceTemplatePairCollection; }
        }
    }

    public class ServiceTemplatePairCollection : ConfigurationElementCollection
    {
        public ServiceTemplatePairElement this[int index]
        {
            get { return BaseGet(index) as ServiceTemplatePairElement; }
            set
            {
                if(BaseGet(index) != null)
                {
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ServiceTemplatePairElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ServiceTemplatePairElement)element).Name;
        }
    }

    public class ServiceTemplatePairElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name { get { return this["name"] as string; } }

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value { get { return this["value"] as string; } }
    }
}