using System.Collections.Generic;

namespace ApiAggregator.Web.ViewModels
{
    public class NavMenuModel
    {
        public IDictionary<int, string> Services { get; set; }
        public IDictionary<int, string> Mappings { get; set; }
    }
}