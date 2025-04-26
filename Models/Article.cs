using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace huan.Models;

public class Article
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(TypeName = "int")]
        public int Id { get; set; }
        
        [StringLength(100)]
        [Column(TypeName = "nvarchar(300)")]
        public string Title { get; set; }

        [StringLength(500)]
        [Column(TypeName = "nvarchar(500)")]
        public string Summary { get; set; }
        
        public string Content { get; set; }
        
        public DateTime SubmissionDate { get; set; }
        
        [StringLength(10)]
        [Column(TypeName = "nvarchar(50)")]
        public string Status { get; set; } // "Pending", "Approved", "Rejected"
        
        // Foreign keys
        public int AuthorId { get; set; }
  
        public int TopicId { get; set; }
        
        // Navigation properties
        [ForeignKey("AuthorId")]
        public virtual User Author { get; set; }
        
        [ForeignKey("TopicId")]
        public virtual Topic Topic  { get; set; }
    }
