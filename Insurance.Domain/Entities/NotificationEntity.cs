using Insurance.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Insurance.Domain.Entities
{
    public class NotificationEntity : AuditedEntity
    {
        [Required(ErrorMessage = "The {0} is required"), MaxLength(100, ErrorMessage = "The {0} can't be exceed {1} character")]
        public string Title { get; set; }

        [Column(TypeName = "ntext")]
        public string Content { get; set; }
    }
}