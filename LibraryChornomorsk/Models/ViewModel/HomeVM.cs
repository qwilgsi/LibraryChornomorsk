namespace LibraryChornomorsk.Models.ViewModel
{
    public class HomeVM
    {
        public IEnumerable<Book>? Books { get; set; }
        public IEnumerable<News>? News { get; set; }
        public IEnumerable<Annotation>? Annotations { get; set; }
        public IEnumerable<Category>? Categories { get; set; }
    }
}
