using SlideCloud.Models;

namespace SlideCloud.Areas.User.Models.Slides
{
    public class ListSlideVM
    {
        public IEnumerable<Document> Documents { get; set; }
        public IEnumerable<DocumentCategory> DocumentCategories { get; set; }
    }
}
