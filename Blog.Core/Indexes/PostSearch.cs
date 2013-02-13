using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;

using Blog.Core.Domain;

using Raven.Client.Indexes;
using Raven.Abstractions.Indexing;

namespace Blog.Core.Indexes
{
    public class PostSearch : AbstractIndexCreationTask<BlogPost, BlogPost>
    {
        public PostSearch()
        {
            Map = posts => 
                from doc in posts select new { doc.Title, doc.Content, doc.IsPublished };
            Analyzers = new Dictionary<Expression<Func<BlogPost, object>>, string>
            {
             { b => b.Title, "SimpleAnalyzer" },
             { b => b.Content, "SimpleAnalyzer" }
            };
            Analyzers.Add(b => b.IsPublished, "");
            Indexes = new Dictionary<Expression<Func<BlogPost, object>>, FieldIndexing>
            {
                { b => b.Title, FieldIndexing.Analyzed },
                { b => b.Content, FieldIndexing.Analyzed },
                { b => b.IsPublished, FieldIndexing.NotAnalyzed }
            };
        }
    }
}
