
namespace ApiAggregator.Core.Models
{
    public class ServiceQueryString : Root
    {
        public int ServiceId { get; set; }

        public Service Service { get; set; }

        public string Name { get; set; }

        public string Value { get; set; }
    }
}
