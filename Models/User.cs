using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace huan.Models;

public class User
    {   
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required, UniqueAttribute(ErrorMessage = "Username already exists.")]
        [StringLength(50)]
        [Column(TypeName = "varchar(50)")]
        public string Username { get; set; }
        
        [Required]
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string Password { get; set; }
        
        [Required]
        [StringLength(100)]
        [Column(TypeName = "nvarchar(100)")]
        public string FullName { get; set; }
        
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string Email { get; set; }
        
        [Required]
        [StringLength(15)]
        [Column(TypeName = "nvarchar(30)")]
        public string Role { get; set; } // "Author" or "Admin"
        [Required]
        [StringLength(10)]
        [Column(TypeName = "nvarchar(10)")]
        public String Status { get; set; } // "Active" or "Inactive"


        [DataType(DataType.Date)]
        public DateTime CreatedAt { get; set; } // Date of account creation
        // Navigation property
        public virtual ICollection<Article> Articles { get; set; }
    }

internal class UniqueAttribute : Attribute
{
    public string ErrorMessage { get; set; }
}

