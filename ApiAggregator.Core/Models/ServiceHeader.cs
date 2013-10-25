
namespace ApiAggregator.Core.Models
{
    public class ServiceHeader : Root
    {
        public int ServiceId { get; set; }

        public Service Service { get; set; }

        public string Header { get; set; }

        public string Value { get; set; }
    }
}
