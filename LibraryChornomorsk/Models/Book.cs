using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryChornomorsk.Models
{
    public class Book
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Вкажіть назву книги")]
        [DisplayName("Назва книги")]
        public string? Title { get; set; }

        [DisplayName("Автор книги")]
        public string? Author { get; set; }

        [DisplayName("Опис книги")]
        public string? Description { get; set; }

        [DisplayName("Зображення книги")]
        public string? Image { get; set; }

        [DisplayName("Рік написання книги")]
        public int Year { get; set; }

        [Required(ErrorMessage = "Оберіть категорію")]
        [DisplayName("Категорія товару")]
        public int CategoryId { get; set; }

        [DisplayName("Категорія товару")]
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }
    }
}
