using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApiAggregator.Web.ViewModels
{
    public class SetupModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Display(Name="Confirm Password")]
        [Compare("Password", ErrorMessage="Passwords do not match!")]
        public string PasswordConfirm { get; set; }
    }
}