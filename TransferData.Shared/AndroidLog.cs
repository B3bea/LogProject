using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TransferData.Shared;

[Table("AndroidLog")]
public class AndroidLog
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [Column(TypeName ="varchar(100)")]
    public string LogDate { get; set; } = string.Empty;

    public int Pid { get; set; }
    public int Tid { get; set; }

    [Required]
    [Column(TypeName = "char(1)")]
    public string Level { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string Component { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;
}   

