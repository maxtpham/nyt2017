using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace nyt.core.tasks.Models
{
    public class task
    {
        [Key]
        public Guid id { get; set; }

        [Required, MaxLength(50)]
        public string code { get; set; }

        [Required, MaxLength(256)]
        public string title { get; set; }

        [MaxLength(Int32.MaxValue)]
        public string content { get; set; }

        public DateTime created { get; set; }
        public DateTime updated { get; set; }
        public Guid? creator { get; set; }
        public Guid? assignee { get; set; }
    }
}
