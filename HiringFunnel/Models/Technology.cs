using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HiringFunnel.Models
{
    public class Technology
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public virtual ICollection<Contact> ContactsInTech { get; set; }
        public virtual ICollection<User> UsersInTech { get; set; }

    }
}
