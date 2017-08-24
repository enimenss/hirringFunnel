using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HiringFunnel.DTO
{
    public class ProcessView
    {
        
        public int Id { get; set; }

        public int pInsContact { get; set; }

        
        public string Name { get; set; }


       
        public string Technologies { get; set; }

    
        public SeniorityLevel Seniority { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
}