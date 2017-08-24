using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HiringFunnel.DTO
{
    public class ProcessInstanceInfo
    {
        public int Id { get; set; }

        public string ProcessName { get; set; }

        public string ContactName { get; set; }

        public string ContactSeniority { get; set; }

        public IEnumerable<string> ContactTechnologies { get; set; }

        public string ProcessSeniority { get; set; }

        public string ProcessTechnologies { get; set; }

        public bool isEnd { get; set; }

        public DateTime? Date { get; set; }
    }
}