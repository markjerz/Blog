using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Blog.Core.Domain;

using Raven.Client.Indexes;

namespace Blog.Core.Indexes
{
    public class TagCount : AbstractIndexCreationTask<BlogPost, TagCount.ReduceResult>
    {
        public class ReduceResult
        {
            public string Tag { get; set; }
            public int Count { get; set; }
        }

        public TagCount()
        {
            Map = posts => from post in posts
                           from tag in post.Tags
                           select new { Tag = tag.ToLower().ToString(), Count = 1 };
            Reduce = results => from tagCount in results
                                group tagCount by tagCount.Tag
                                    into g
                                    select new { Tag = g.Key, Count = g.Sum(x => x.Count) };
            Sort(result => result.Count, Raven.Abstractions.Indexing.SortOptions.Int);
        }
    }
}
