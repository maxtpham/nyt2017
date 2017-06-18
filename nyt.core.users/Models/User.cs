using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace nyt.core.users.Models
{
    public class User
    {
        public Guid Id { get; set; }

        [MaxLength(256), Required]
        public string Name { get; set; }

        [MaxLength(Int32.MaxValue)]
        public string Description { get; set; }
    }
}
