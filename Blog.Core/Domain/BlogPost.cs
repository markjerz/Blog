using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blog.Core.Domain
{
    public class BlogPost
    {
        public BlogPost()
        {
            Comments = new List<BlogComment>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ContentSynopsis { get; set; }
        public bool IsPublished { get; set; }
        public DateTime PublishedAt { get; set; }
        public string[] Tags { get; set; }
        public IList<BlogComment> Comments { get; set; }
    }

    public class BlogComment
    {
        public string Author { get; set; }
        public string Content { get; set; }
        public DateTime CommentedAt { get; set; }
        public string AuthorUrl { get; set; }
        public string Email { get; set; }
    }
}
