using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace SlideCloud.Domain.Entities;

public class User : IdentityUser<long>
{
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public ICollection<Document> Documents { get; set; }
} 