﻿using System.ComponentModel.DataAnnotations;

namespace AsmApi_PH53971.Models
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        
        //public string FirstName { get; set; }
        //public string LastName { get; set; }
    }
}
