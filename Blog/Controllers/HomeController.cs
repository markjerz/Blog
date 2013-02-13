using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Blog.Core.Domain;
using Blog.Core.Indexes;

using Raven.Client;
using Raven.Client.Linq;
using Raven.Client.Util;

namespace Blog.Controllers
{
    public class HomeController : BaseController
    {
        public FileStreamResult Image(string id)
        {
            Guid gId = new Guid();
            if (Guid.TryParse(id, out gId)) {
                var image = session.Load<Image>(gId);
                if (image != null)
                {
                    // fetch out of attachments
                    var attachment = session.Advanced.DocumentStore.DatabaseCommands.GetAttachment(gId.ToString());
                    return new FileStreamResult(attachment.Data(), image.MimeType);
                }
            }
            throw new HttpException(404, "Image Not Found");
        }             

        public ActionResult Search(string q, int page = 1)
        {
            RavenQueryStatistics stats;
            var blogposts = session.Query<BlogPost, PostSearch>()
                .Where(b => b.IsPublished)
                .Search(b => b.Title, q)
                .Search(b => b.Content, q)
                .Statistics(out stats)
                .Skip((page - 1) * 10)
                .Take(10)
                .As<BlogPost>()
                .ToList();
            ViewBag.TotalResults = stats.TotalResults;
            ViewBag.Q = q;
            ViewBag.Page = page;
            return View(blogposts);
        }

        public ActionResult Blog(int id)
        {
            var blogPost = session.Load<BlogPost>(id);
            if (blogPost != null)
            {
                ViewBag.ShowComments = true;
                return View(blogPost);
            }
            throw new HttpException(404, "No idea where that's gone!");
        }

        public ActionResult Comment(int id, string name, string email, string url, string comment)
        {
            var blogPost = session.Load<BlogPost>(id);
            if (blogPost.Comments == null)
            {
                blogPost.Comments = new List<BlogComment>();
            }
            blogPost.Comments.Add(new BlogComment { Author = name, AuthorUrl = url, Content = comment, CommentedAt = DateTime.UtcNow, Email = email });
            session.SaveChanges();
            return RedirectToAction("Blog", new { id = id });
        }

        public ActionResult Index()
        {
            // get last 10 blog posts
            var blogPosts = session.Query<BlogPost>().Where(b => b.IsPublished).Take(10).OrderByDescending(b => b.PublishedAt).ToList();

            return View(blogPosts);
        }

        public ActionResult Tag(string tag, int pageSize = 10, int page = 1)
        {
            var blogPosts = session.Query<BlogPost>()
                .OrderByDescending(p => p.PublishedAt)
                .Where(p => p.Tags.Any(t => t == tag))
                .Where(p => p.IsPublished)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.Tag = tag;

            return View(blogPosts);
        }
    }
}
