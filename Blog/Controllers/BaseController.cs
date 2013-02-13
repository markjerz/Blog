using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Blog.Core.Infrastructure;

using Raven.Client;
using Raven.Client.Document;

namespace Blog.Controllers
{
    public abstract class BaseController : Controller
    {
        protected IDocumentSession session;

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            // set up the Raven DB store/session
            session = Blog.Core.Infrastructure.DocumentStore.Store.OpenSession();

            // fetch the tag cloud items
            ViewBag.TagCloudItems = GetTagCloudItems();
        }

        private IList<Models.TagCloudItem> GetTagCloudItems()
        {
            var result = session.Query<Core.Indexes.TagCount.ReduceResult, Core.Indexes.TagCount>()
                    .OrderByDescending(x => x.Count).ToArray();
            return result.Select(rr => new Models.TagCloudItem { Tag = rr.Tag, Weight = rr.Count }).ToList();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);

            session.Dispose();
        }
    }
}
