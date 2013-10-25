using System.ComponentModel.DataAnnotations;

namespace ApiAggregator.Web.ViewModels
{
    public class MappingModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "You have to give your mapping a name!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "I need a local enpoint to map to!")]
        public string Endpoint { get; set; }

        [Required(ErrorMessage = "I need to know how to send data to the remote server!")]
        public string Method { get; set; }

        [Required(ErrorMessage = "You need to enter a relative endpoint for the service to call!")]
        [Display(Name = "API Url")]
        public string Api { get; set; }

        public string ServiceName { get; set; }

        [Required]
        public bool Enabled { get; set; }

        [Required(ErrorMessage = "You must select a service for the mapping!")]
        public int? ServiceId { get; set; }

        public string RowClass
        {
            get
            {
                if(!Enabled)
                {
                    return "warning";
                }

                return "";
            }
        }
    }
}