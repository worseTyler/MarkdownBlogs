using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownBlogs
{
    static class BlogFileService
    {
        // This is assuming a format of folder/year/month/slug/index.md
        public static List<Blog> PullBlogsFromDirectory(string directory)
        {
            ConcurrentBag<Blog> blogs = new();
            FileInfo[] files = new DirectoryInfo(directory).GetFiles("*md", SearchOption.AllDirectories);
            Parallel.ForEach(files, file =>
            {
                Blog blog = new();
                using StreamReader reader = new(file.OpenRead());
                blog.Content = reader.ReadToEnd();
                blog.Slug = file.FullName.Split("\\")[^2];
                blogs.Add(blog);
            });

            return blogs.ToList();
        }
    }
}
