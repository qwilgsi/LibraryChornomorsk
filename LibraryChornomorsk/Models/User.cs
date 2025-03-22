using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
namespace LibraryChornomorsk.Models
{
    public class LibraryUser : IdentityUser
    {
        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Вкажіть ваше ПІБ")]
        [DisplayName("Ваше ПІБ")]
        public string? FullName { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Вкажіть ваш вік")]
        [DisplayName("Ваш вік")]
        public int Age { get; set; }
    }
}