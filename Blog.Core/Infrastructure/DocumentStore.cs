using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Raven.Client;
using Raven.Client.Document;
using Raven.Client.Indexes;

namespace Blog.Core.Infrastructure
{
    public class DocumentStore
    {
        static Raven.Client.Document.DocumentStore store;

        public static Raven.Client.Document.DocumentStore Store
        {
            get
            {
                if (store != null && !store.WasDisposed)
                {
                    return store;
                }
                store = new Raven.Client.Embedded.EmbeddableDocumentStore { DataDirectory = "Data" };
                store.Initialize();

                // set up the indexes
                IndexCreation.CreateIndexes(typeof(Core.Indexes.TagCount).Assembly, store);

                return store;
            }
        }
    }
}
