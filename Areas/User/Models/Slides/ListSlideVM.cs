using SlideCloud.Models;

namespace SlideCloud.Areas.User.Models.Slides
{
    public class ListSlideVM
    {
        public PaginationModel<Document> Pagination { get; set; }
        public IEnumerable<DocumentCategory> DocumentCategories { get; set; }
        
    }
}
