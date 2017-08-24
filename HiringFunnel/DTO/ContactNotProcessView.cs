using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace HiringFunnel.DTO
{
    public class ContactNotProcessView
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public SeniorityLevel Seniority { get; set; }

        public int ProcInstId { get; set; }


        public List<TechnologyView> Technologies { get; set; }

        public bool Contacted { get; set; }

        public ContactNotProcessView()
        {
            Technologies = new List<TechnologyView>();
            Contacted = false;

        }
    }
}