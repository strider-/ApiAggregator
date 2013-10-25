using System;

namespace ApiAggregator.Core.Models
{
    public class ApiMapping : Root
    {
        public int ServiceId { get; set; }

        public string Name { get; set; }

        public string Method { get; set; }
        
        public string Endpoint { get; set; }

        public Service Service { get; set; }

        public bool Enabled { get; set; }

        public DateTime Created { get; set; }

        public string Api { get; set; }
    }
}