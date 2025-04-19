using SlideCloud.Domain.Entities;

namespace SlideCloud.Application.DTO.Home;

public class HomeViewModel
{
    public required long UserId { get; set; }
    public required string UserName { get; set; }
    public required IEnumerable<Document> RecentFiles { get; set; }
    public required IEnumerable<Document> SharedDocuments { get; set; }
}