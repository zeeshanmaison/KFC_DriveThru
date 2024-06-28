using DriveThru.Integration.Core.Model.Abstraction;
using System.ComponentModel.DataAnnotations;

namespace DriveThru.Integration.Core.Model
{
    public abstract class BasicEntity : Entity<long>, IBasicEntity
    {
        [Required]
        public bool IsActive { get; set; }

        [Required]
        public bool IsDeleted { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public long CreatedBy { get; set; }

        public DateTime? ModifiedOn { get; set; }

        public long? ModifiedBy { get; set; }
    }
}
