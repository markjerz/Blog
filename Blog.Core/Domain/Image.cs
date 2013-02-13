using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blog.Core.Domain
{
    public class Image
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string MimeType { get; set; }
    }
}
