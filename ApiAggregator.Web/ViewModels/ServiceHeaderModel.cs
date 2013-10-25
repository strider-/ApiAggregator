using System.ComponentModel.DataAnnotations;

namespace ApiAggregator.Web.ViewModels
{
    public class ServiceHeaderModel
    {
        public int Id { get; set; }

        public int ServiceId { get; set; }

        [Required(ErrorMessage="A header name is required!")]
        public string Header { get; set; }

        [Required(ErrorMessage="I need a value for the header!")]
        public string Value { get; set; }
    }
}