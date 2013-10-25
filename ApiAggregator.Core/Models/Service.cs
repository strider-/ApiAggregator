using System;
using System.Collections.Generic;

namespace ApiAggregator.Core.Models
{
    public class Service : Root
    {
        public Service()
        {
            Headers = new List<ServiceHeader>();
            QueryStringAppends = new List<ServiceQueryString>();
        }

        public string Name { get; set; }

        public string RootUrl { get; set; }

        public bool Enabled { get; set; }

        public DateTime Created { get; set; }

        public IEnumerable<ServiceHeader> Headers { get; set; }

        public IEnumerable<ServiceQueryString> QueryStringAppends { get; set; }
    }
}
