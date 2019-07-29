using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Frogger_Blogger.Models
{

    public class LandingPage
    {
        public BlogPost CarouselPost { get; set; }
        public BlogPost SmallPost { get; set; }
        public BlogPost SidePost { get; set; }
        public BlogPost LargePost { get; set; }

    }
}