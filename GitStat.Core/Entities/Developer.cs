using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GitStat.Core.Entities
{
    public class Developer : EntityObject
    {
        [Required]
        public string Name { get; set; }
        public ICollection<Commit> Commits { get; set; }

    }
}
