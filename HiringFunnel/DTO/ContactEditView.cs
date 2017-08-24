using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HiringFunnel.DTO
{
    public class ContactEditView
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; }

        public string Location { get; set; }

        public string LinkedIn { get; set; }

        public byte[] CVContent { get; set; }

        public string CVContentType { get; set; }


        public SeniorityLevel Seniority { get; set; }

        public Collection<TechnologyView> Technologies { get; set; }

        public Collection<InstProcessEditView> Instance = new Collection<InstProcessEditView>();

        
    }
}