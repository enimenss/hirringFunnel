using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HiringFunnel.Models
{
    public class User
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public SeniorityLevel Seniority { get; set; }

        [Required]
        public UserLevel Role { get; set; }

        [DefaultValue(false)]
        public bool Deleted { get; set; }
        
        public virtual ICollection<Technology> Technologies { get; set; }
        public virtual ICollection<Annotation> Annotations { get; set; }

        public virtual ICollection<Interviewer> Interviewers { get; set; }

    }
}
