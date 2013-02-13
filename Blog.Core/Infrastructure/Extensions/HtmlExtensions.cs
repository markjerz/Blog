using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Blog.Core.Infrastructure.Extensions
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString Pager(this HtmlHelper helper, int currentPage, int pageSize, int totalRecords, string urlPrefix)
        {
            StringBuilder sb1 = new StringBuilder();

            if (currentPage > 4)
                sb1.AppendLine(String.Format("<a href=\"{0}&page={1}\">&lt;&lt;</a>", urlPrefix, 1));

            if (currentPage > 1)
                sb1.AppendLine(String.Format("<a href=\"{0}&page={1}\">&lt;</a>", urlPrefix, currentPage - 1));

            int startPage = currentPage > 4 ? currentPage - 3 : 1;
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
            for (int i = startPage; i < currentPage + 4 && i <= totalPages; i++)
            {
                if (i != currentPage)
                {
                    sb1.AppendLine(String.Format("<a href=\"{0}&page={1}\">{1}</a>", urlPrefix, i));
                }
                else
                {
                    sb1.AppendLine(i.ToString());
                }
            }

            if (currentPage < totalPages)
                sb1.AppendLine(String.Format("<a href=\"{0}&page={1}\">&gt;</a>", urlPrefix, currentPage + 1));

            if (currentPage < totalPages - 4)
                sb1.AppendLine(String.Format("<a href=\"{0}&page={1}\">&gt;&gt;</a>", urlPrefix, totalPages));

            return MvcHtmlString.Create(sb1.ToString());
        }
    }
}
