using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownBlogs
{
    class BlogData
    {
        public string Slug { get; init; }
        public int HeaderOne { get; init; }
        public int HeaderTwo { get; init; }
        public int HeaderThree { get; init; }
        public int HeaderTotal { get; init; }
        public int ImageTotal { get; init; }
        public int WordsCount { get; init; }
        public int CodeCount { get; init; }
        public bool EmbededHtml { get; init; }

        public List<string> DeadLinks = new();
    }
}
