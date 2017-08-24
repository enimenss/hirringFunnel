using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiringFunnel.Models
{
    [DefaultValue(typeof(bool), "false")]
    public class ProcessInstance
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }

        public Phase? CurrentPhase { get; set; }

        //[DefaultValue(false)]
        //public bool Contacted { get; set; }

        public DateTime? ContactDate { get; set; }

        //[DefaultValue(false)]
        public bool InterviewScheduled { get; set; }

        //[DefaultValue(false)]
        public bool InterviewNoticed { get; set; }

        //[DefaultValue(false)]
        public bool InterviewHeld { get; set; }

        public DateTime? InterviewDate { get; set; }
        
        //[DefaultValue(false)]
        public bool TestScheduled { get; set; }

        //[DefaultValue(false)]
        public bool TestNoticed { get; set; }

        //[DefaultValue(false)]
        public bool TestHeld { get; set; }
        public DateTime? TestDate { get; set; }

        //[DefaultValue(false)]
        public bool TestDefenseScheduled { get; set; }

        //[DefaultValue(false)]
        public bool TestDefenseNoticed { get; set; }

        //[DefaultValue(false)]
        public bool TestDefenseHeld { get; set; }
        public DateTime? TestDefenseDate { get; set; }

        //[DefaultValue(false)]
        public bool OfferSent { get; set; }
        public DateTime? OfferDate { get; set; }

        //[DefaultValue(false)]
        public bool Accepted { get; set; }

        public DateTime? AcceptanceDate { get; set; }

        //[DefaultValue(false)]
        public bool Rejected { get; set; }
        public DateTime? RejectionDate { get; set; }

        //[DefaultValue(false)]
        public bool Quit { get; set; }
        public DateTime? QuitDate { get; set; }
        
        [Required]
        [ForeignKey("ContactInProcess")]
        public int ContactId { get; set; }

        public virtual Contact ContactInProcess { get; set; }

        [Required]
        [ForeignKey("InstanceOf")]
        public int ProcessId { get; set; }

        public virtual Process InstanceOf { get; set; }

        public virtual ICollection<Annotation> Annotations { get; set; }
        public virtual ICollection<ToDoItem> ToDoItems { get; set; }

        public virtual ICollection<Interviewer> Interviewers { get; set; }

    }
}
