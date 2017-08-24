using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiringFunnel.Models
{
    public class Annotation
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [Required]
        public AnnotationType Type { get; set; }

        [DefaultValue(false)]
        public bool Hidden { get; set; }
        
        [ForeignKey("InstanceOfProcess")]
        public int? PInsId { get; set; }

        public virtual ProcessInstance InstanceOfProcess { get; set; }

        [ForeignKey("Receiver")]
        public int? ContactId { get; set; }

        public virtual Contact Receiver { get; set; }

        [Required]
        [ForeignKey("Author")]
        public int AuthorId { get; set; }
        
        public virtual User Author { get; set; }

    }
}
