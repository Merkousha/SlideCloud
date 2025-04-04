using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
namespace SlideCloud.Models.DTO.Home
{
    public class HomeDTO
    {
        public List<SlideCloud.Models.User> NewUsers { get; set; }
        public List<Document> LatestDocuments { get; set; }
        public List<Document> PopularDocuments { get; set; }
        public List<DocumentCategory> NewCategories { get; set; }
    }
}
