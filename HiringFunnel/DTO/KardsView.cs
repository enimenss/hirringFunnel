using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HiringFunnel.DTO
{
    public class KardsView
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string Technologies { get; set; }

        public string Seniority { get; set; }

        public DateTime? ContactDate { get; set; }

    }
}