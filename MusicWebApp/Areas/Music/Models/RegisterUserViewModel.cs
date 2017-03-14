using MusicWebApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MusicWebApp.Areas.Music.Models
{
    public class RegisterUserViewModel : Login
    {
        public string Email { get; set; }

        // Unneccessary
        [Display(Name = "First name")]
        public string Firstname { get; set; }
        [Display(Name = "Last name")]
        public string Lastname { get; set; }
        [Display(Name = "Phone")]
        public string Phone { get; set; }
        [Display(Name = "Are you a male?")]
        public bool Gender { get; set; }
        
        // Image
        public HttpPostedFileBase ImageBase { get; set; }
    }
}