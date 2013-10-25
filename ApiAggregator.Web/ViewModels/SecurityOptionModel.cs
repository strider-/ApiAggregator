using System.ComponentModel.DataAnnotations;
using ApiAggregator.Core.Models;

namespace ApiAggregator.Web.ViewModels
{
    public class SecurityOptionModel
    {
        public SecurityOption Option { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}