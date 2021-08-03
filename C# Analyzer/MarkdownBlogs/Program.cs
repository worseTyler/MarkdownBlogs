using System;
using System.Collections.Generic;
using System.Net;

namespace MarkdownBlogs
{
    class Program
    {
        static void Main(string[] args)
        {
            string exampleDirectory = "C:\\temp";
            List<Blog> blogs = BlogFileService.PullBlogsFromDirectory(exampleDirectory);
            foreach(Blog blog in blogs)
            {
                Console.WriteLine(blog.Slug);
            }
            Console.WriteLine($"Total Count: {blogs.Count}");
        }
    }
}
