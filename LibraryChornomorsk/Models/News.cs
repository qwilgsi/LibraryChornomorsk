using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace LibraryChornomorsk.Models
{
    public class News
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Вкажіть назву новини")]
        [DisplayName("Назва новини")]
        public string? Name { get; set; }

        [DisplayName("Опис новини")]
        public string? Description { get; set; }

        [DisplayName("Зображення новини")]
        public string? Image {  get; set; }
    }
}
