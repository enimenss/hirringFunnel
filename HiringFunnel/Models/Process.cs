using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HiringFunnel.Models
{
    public class Process
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Technologies { get; set; }

        [Required]
        public SeniorityLevel Seniority { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        
        public DateTime? EndDate { get; set; }

        public virtual ICollection<ProcessInstance> Instances { get; set; }

    }
}
