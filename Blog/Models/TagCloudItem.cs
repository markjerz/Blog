using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog.Models
{
    public class TagCloudItem
    {
        public string Tag { get; set; }
        public int Weight { get; set; }
    }
}