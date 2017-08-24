using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HiringFunnel.Models
{
    public class ToDoItem
    {

        [Key]
        public int Id { get; set; }

        [Required]
        public string Message { get; set; }

        [DefaultValue(false)]
        public bool Completed { get; set; }

        [Required]
        public DateTime Time { get; set; }

        [ForeignKey("InstanceOfProcess")]
        public int PInsId { get; set; }

        public virtual ProcessInstance InstanceOfProcess { get; set; }

    }
}
