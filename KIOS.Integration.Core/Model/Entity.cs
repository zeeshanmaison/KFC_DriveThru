using DriveThru.Integration.Core.Model.Abstraction;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveThru.Integration.Core.Model
{
    public abstract class Entity<TKey> : IBase<TKey>
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public TKey Id { get; set; }

        [Required]
        public Guid GlobalId { get; set; } = Guid.NewGuid();
    }
}
