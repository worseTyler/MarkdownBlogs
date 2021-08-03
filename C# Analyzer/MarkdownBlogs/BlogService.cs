using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownBlogs
{
    class BlogService
    {
        public List<Blog> Blogs;
        public BlogService(List<Blog> blogs)
        {
            Blogs = FormatBlogs(blogs);
        }
        public void BuildDataSheet(string filePath)
        {
            List<BlogData> dataList = AnalyzeBlogs();
            List<string> values = new();
            values.Add("Slug, Header One, Header Two, Header Three, Header Total, Image Count, Word Count, Code Count, Embeded Html");
            foreach(BlogData data in dataList)
            {
                values.Add($"{data.Slug}, {data.HeaderOne}, {data.HeaderTwo}, {data.HeaderThree}, {data.HeaderTotal}, {data.ImageTotal}, {data.WordsCount}, {data.CodeCount}, {data.EmbededHtml}");
            }
            File.WriteAllLines(filePath, values);
        }
        private List<BlogData> AnalyzeBlogs()
        {
            List<BlogData> dataList = new();
            Parallel.For(0, Blogs.Count(), i =>
            {
                BlogData data = BlogAnalyzer.Analyze(Blogs[i]);
                dataList.Add(data);
            });
            return dataList;
        }
        private List<Blog> FormatBlogs(List<Blog> blogs)
        {
            List<Blog> updatedBlogs = new();
            Parallel.For(0, blogs.Count(), i =>
            {
                Blog updatedBlog = BlogFormatter.Format(blogs[i]);
                updatedBlogs.Add(updatedBlog);
            });
            return updatedBlogs;
            //SecureLinks
        }
    }
}
