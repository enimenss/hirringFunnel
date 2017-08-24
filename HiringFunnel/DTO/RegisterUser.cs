using HiringFunnel.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HiringFunnel.DTO
{
    public class RegisterUser
    {
        public int Id { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public SeniorityLevel Seniority { get; set; }

        [Required]
        public string Email { get; set; }


        public string Password { get; set; }

        [Required]
        public UserLevel Role { get; set; }

        public bool Deleted { get; set; }

        public Collection<TechnologyView> Technologies = new Collection<TechnologyView>();

        public RegisterUser() { }

        public RegisterUser(User c):base()
        {
            Id = c.Id;
            Email = c.Email;
            FirstName = c.FirstName;
            LastName = c.LastName;
            Role = c.Role;
            Seniority = c.Seniority;
            Password = c.PasswordHash;
            Deleted = c.Deleted;
            //Technologies = new Collection<TechnologyView>();

            foreach (var t in c.Technologies)
            {
                Technologies.Add(new TechnologyView(t));
            }
        }

    }
}