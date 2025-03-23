using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SlideCloud.Models;

public class TagDocument
{

    // کلید اصلی
    [Key]
    public int Id { get; set; }
    // کلید خارجی به Tag
    public int TagId { get; set; }
    [ForeignKey("TagId")]
    public Tag Tag { get; set; }

    // کلید خارجی به Document
    public int DocumentId { get; set; }
    [ForeignKey("DocumentId")]
    public Document Document { get; set; }
}
