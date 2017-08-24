using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HiringFunnel.DTO
{
    public class SessionLogin
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public UserLevel Role { get; set; }

    }
}