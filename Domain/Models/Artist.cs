using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models;

[Table("artists")]
public class Artist
{
    [Key]
    [Column("pk_artist_id", TypeName = "uuid")]
    public Guid ArtistId { get; set; }

    [Required]
    [MaxLength(255)]
    [Column("name", TypeName = "varchar(255)")]
    public string Name { get; set; } = string.Empty;

    [Column("description", TypeName = "text")]
    public string? Description { get; set; }

    [MaxLength(255)]
    [Column("genre", TypeName = "varchar(255)")]
    public string? Genre { get; set; }
}