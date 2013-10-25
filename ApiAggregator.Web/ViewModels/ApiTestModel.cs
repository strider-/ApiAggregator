using System.Collections.Generic;

namespace ApiAggregator.Web.ViewModels
{
    public class ApiTestModel
    {
        public int Id { get; set; }
        public bool Debug { get; set; }
        public IDictionary<string, string> Pairs { get; set; }
    }
}