using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LibraryChornomorsk.Models
{
    public class Annotation
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Вкажіть назву анотації")]
        [DisplayName("Назва анотації")]
        public string? Name { get; set; }

        [DisplayName("Опис анотації")]
        public string? Description { get; set; }

        [DisplayName("Зображення анотації")]
        public string? Image { get; set; }

        [DisplayName("Дата анотації")]
        public DateTime date { get; set; }
    }
}
