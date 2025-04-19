using SlideCloud.Domain.Entities;

namespace SlideCloud.Application.DTO.Home;

public class HomeDTO
{
    public IEnumerable<Domain.Entities.User> NewUsers { get; set; } = new List<Domain.Entities.User>();
    public IEnumerable<Document> LatestDocuments { get; set; } = new List<Document>();
    public IEnumerable<Document> PopularDocuments { get; set; } = new List<Document>();
    public IEnumerable<DocumentCategory> NewCategories { get; set; } = new List<DocumentCategory>();
} 