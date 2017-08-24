using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HiringFunnel.Models
{
    public class Contact
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        
        public DateTime DateOfBirth { get; set; }
        public string Location { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string LinkedIn { get; set; }

        public virtual byte[] CVContent { get; set; }

        public string CVContentType { get; set; }

        [Required]
        public SeniorityLevel Seniority { get; set; }

        [DefaultValue(false)]
        public bool Deleted { get; set; }

        public virtual ICollection<Technology> Technologies { get; set; }
        public virtual ICollection<ProcessInstance> Instances { get; set; }
        public virtual ICollection<Annotation> Comments { get; set; }

    }
}
