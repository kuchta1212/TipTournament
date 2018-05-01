using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TipTournament.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Uživatelské jméno")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        [Display(Name = "Pamatovat si mě?")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Hele sorry ale bez uživatelského jména ta neklapne.")]
        [StringLength(16, ErrorMessage = "Chce to alespoň 3 ale maximálně 16 znaků, tak se do toho zkus vejít ;-)",
            MinimumLength = 3)]
        [Display(Name = "Uživatelské jméno")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Hele nechci moc, ale ty {2} znaky tam prostě musíš dát.", MinimumLength = 3)]
        [DataType(DataType.Password)]
        [Display(Name = "Heslo")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Potvrdit heslo")]
        [Compare("Password", ErrorMessage = "Hele asi překlep, tak si to zopakuj alespoň se procvičíš")]
        public string ConfirmPassword { get; set; }
    }

}