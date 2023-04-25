using MissionApp.Entities.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MissionApp.Entities.ViewModels
{
    public class AdminUserDetailsVM
    {
        public int UserId { get; set; }
        public string? Avatar { get; set; }

        [Required(ErrorMessage = " Required!")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = " Required!")]
        public string? LastName { get; set; }

        [Required(ErrorMessage = " Required!")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = " Required!")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required(ErrorMessage = " Required!")]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = null!;

        [Required(ErrorMessage = " Required!")]
        public long PhoneNumber { get; set; }

        public string? EmployeeId { get; set; }

        public string? Department { get; set; }

        public string? ProfileText { get; set; }

        [Required(ErrorMessage = " Required!")]
        public long CityId { get; set; }

        [Required(ErrorMessage = " Required!")]
        public long CountryId { get; set; }

        //Password change
        [Required(ErrorMessage = "Required!")]
        public string OldPassword { get; set; } = null!;

        [Required(ErrorMessage = "Required!")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = null!;

        [Required(ErrorMessage = "Required!")]
        [Compare("NewPassword")]
        public string ConfirmPasswordEdit { get; set; } = null!;
        public IEnumerable<User> UserLists { get; set; } = new List<User>();
        public IEnumerable<Country> CountryList { get; set; } = new List<Country>();
    }
}
