using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ApiAggregator.Web.ViewModels
{
    public class ServiceModel
    {
        public ServiceModel()
        {
            Headers = new List<ServiceHeaderModel>();
            QueryStringAppends = new List<ServiceQueryStringModel>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage = "You have to give this service a name!")]
        [Display(Name = "Service Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "I kinda need to know where the service is!")]
        [Display(Name = "Root Url")]
        public string RootUrl { get; set; }

        [Required]
        public bool Enabled { get; set; }

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

        public List<ServiceHeaderModel> Headers { get; set; }

        public List<ServiceQueryStringModel> QueryStringAppends { get; set; }
    }
}