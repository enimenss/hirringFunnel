using HiringFunnel.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HiringFunnel.DTO
{
    public class ContactView
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string Email { get; set; }

        public string Phone { get; set; }

        public DateTime DateOfBirth { get; set; }

        
        public string Location { get; set; }


        public string LinkedIn { get; set; }

        public byte[] CVContent { get; set; }

        public string CVContentType { get; set; }


        public SeniorityLevel Seniority { get; set; }

        public Collection<TechnologyView> Technologies { get; set; }

        public ContactView()
        {
            Technologies = new Collection<TechnologyView>();
        }

        public ContactView( Contact c)
        {
            Id = c.Id;
            Email = c.Email;
            FirstName = c.FirstName;
            LastName = c.LastName;
            Phone = c.Phone;
            Seniority = c.Seniority;
            Technologies = new Collection<TechnologyView>();

            foreach (var t in c.Technologies)
            {
                Technologies.Add(new TechnologyView(t));
            }
        }

      

         public void BaseData(Contact c)
        {
            Id = c.Id;
            Email = c.Email;
            FirstName = c.FirstName;
            LastName = c.LastName;
            Phone = c.Phone;
            Seniority = c.Seniority;

        }

        public void Tehnology(Collection<Technology> list)
        {
            foreach( var t in list)
            {
                Technologies.Add(new TechnologyView(t));
            }
        }
    
    }

}