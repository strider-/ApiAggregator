using System;

namespace ApiAggregator.Core.Models
{
    public class RecentItem
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Type { get; set; }

        public bool Enabled { get; set; }

        public DateTime Created { get; set; }

        public string Url { get; set; }
    }
}
