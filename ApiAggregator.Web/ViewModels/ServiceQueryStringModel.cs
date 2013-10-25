using System.ComponentModel.DataAnnotations;

namespace ApiAggregator.Web.ViewModels
{
    public class ServiceQueryStringModel
    {
        public int Id { get; set; }

        public int ServiceId { get; set; }

        [Required(ErrorMessage = "The key name is required!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The key needs a value!")]
        public string Value { get; set; }
    }
}