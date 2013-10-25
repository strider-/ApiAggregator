using System.ComponentModel.DataAnnotations;
using ApiAggregator.Core.Models;
using ApiAggregator.Web.Framework;

namespace ApiAggregator.Web.ViewModels
{
    public class ConfigurationModel
    {
        public string ApiKey { get; set; }

        [Required]
        public SecurityOption SecurityOption { get; set; }

        [Required]
        public bool RequireLogin { get; set; }

        [Required]
        public bool RequireAuthenticator { get; set; }

        [Required]
        [Display(Name="API Index")]
        public bool DescribeAtRoot { get; set; }

        [Required]
        public string Username { get; set; }

        [Display(Name = "New Password")]
        public string Password { get; set; }

        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match!")]
        public string PasswordConfirm { get; set; }
    }
}