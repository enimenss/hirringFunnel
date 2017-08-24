using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HiringFunnel.DTO
{
    public class CommentView
    {
        public int Id { get; set; }

  
        public string Message { get; set; }

    
        public String Time { get; set; }

        public AnnotationType Type { get; set; }

        public string AuthorName { get; set; }

        public bool Hidden { get; set; }

        public CommentView() { }
    }
}