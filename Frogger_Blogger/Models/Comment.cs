using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace Frogger_Blogger.Models
{
   
    public class Comment
    {
        public int Id { get; set; }
        //public string Email { get; set; }
        public string AuthorId { get; set; }
        public string Body { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset? Updated { get; set; }
        private string UpdateReason { get; set; }

        //Virtual Navigation section
        
        public int BlogPostId { get; set; }
        public virtual BlogPost BlogPost { get; set; }
        public virtual ApplicationUser Author { get; set; }


    }
}