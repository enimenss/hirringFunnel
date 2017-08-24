using HiringFunnel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HiringFunnel.DTO
{
    public class TechnologyView
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public TechnologyView() { }
      
        public TechnologyView( Technology t):base()
        {
            Id = t.Id;
            Name = t.Name;

        }
    }
}