using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASMHT_PH53971.Models.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }  
        public string? Address { get; set; }
        public string? Gender { get; set; }
        public string? Nationality { get; set; }
        public bool Role { get; set; }

        public IList<SelectListItem> AvailableRoles { get; set; } = new List<SelectListItem>(); 
        public IList<string> SelectedRoles { get; set; } = new List<string>(); 
    }
}
