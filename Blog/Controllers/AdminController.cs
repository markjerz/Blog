using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Blog.Core.Domain;

using HtmlAgilityPack;

namespace Blog.Controllers {
    [Authorize]
    public class AdminController : BaseController {
        public ActionResult Index() {
            return View();
        }

        //
        // GET: /Admin/
        public ActionResult UploadImage() {
            return View();
        }

        [HttpPost]
        public ActionResult UploadImage(HttpPostedFileBase image, string rename) {
            if (image.ContentLength > 0) {
                var name = System.IO.Path.GetFileNameWithoutExtension(image.FileName);
                var ext = System.IO.Path.GetExtension(image.FileName);
                if (rename != null && rename.Trim().Length > 0) {
                    name = rename.Trim();
                }

                // save the image file
                var ravenImage = new Image { MimeType = image.ContentType, Name = name + ext };
                session.Store(ravenImage);
                session.SaveChanges();

                session.Advanced.DocumentStore.DatabaseCommands.PutAttachment(ravenImage.Id.ToString(), null, image.InputStream, null);
                return RedirectToAction("UploadImage");
            }
            return View();
        }

        public ActionResult ImageSearch(string q) {
            var images = session.Query<Image>().Where(i => i.Name.StartsWith(q)).Take(10).ToList();
            return Json(images.Select(i => new { i.Id, i.Name }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult Post(int? id) {
            var blogPost = id.HasValue ? session.Load<BlogPost>(id.Value) : new BlogPost();
            if (!id.HasValue) {
                session.Store(blogPost);
                session.SaveChanges();
            }
            return View(blogPost);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Post(string title, string tags, int? id, string content = "", string action = "") {
            if (tags == null || tags.Trim().Length == 0) {
                tags = null;
            }
            var blogPost = id.HasValue ? session.Load<BlogPost>(id.Value) : new BlogPost();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(content);
            var text = String.Join(" ", doc.DocumentNode.Descendants().Where(n => n is HtmlTextNode).Select(n => n.InnerText));

            blogPost.Title = title;
            blogPost.Tags = tags != null ? tags.Split(',').Select(t => t.Trim().ToLower()).ToArray() : null;
            blogPost.Content = content;
            blogPost.ContentSynopsis = text.Length > 100 ? text.Substring(0, 100) + "..." : text;

            if (action == "Publish") {
                blogPost.PublishedAt = DateTime.UtcNow;
                blogPost.IsPublished = true;
            }

            session.Store(blogPost);
            session.SaveChanges();

            if (this.Request.IsAjaxRequest()) {
                return Content("Success");
            } else {
                return RedirectToAction("Index", "Admin");
            }
        }

        public ActionResult Delete(int? id) {
            if (id.HasValue && this.Request.UrlReferrer.Host == this.Request.Url.Host) {
                var blogpost = session.Load<BlogPost>(id.Value);
                session.Delete(blogpost);
                session.SaveChanges();
            }
            return RedirectToAction("EditPosts");
        }

        public ActionResult DeleteImage(Guid id) {
            if (this.Request.UrlReferrer.Host == this.Request.Url.Host) {
                var image = session.Load<Image>(id);
                session.Delete(image);
                session.SaveChanges();
            }
            return RedirectToAction("EditImages");
        }

        public ActionResult EditPosts() {
            var posts = session.Query<BlogPost>().ToList();
            return View(posts);
        }

        public ActionResult EditImages(int page = 1) {
            Raven.Client.RavenQueryStatistics stats;
            var images = session.Query<Image>().Statistics(out stats).Skip((page - 1) * 15).Take(15).ToList();
            ViewBag.Page = page;
            ViewBag.TotalResults = stats.TotalResults;
            return View(images);
        }
    }
}
