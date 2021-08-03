using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MarkdownBlogs
{
    static class BlogFormatter
    {
        public static Blog Format(Blog blog)
        {
            blog.Content = SecureLinks(blog.Content);
            return blog;
        }

        private static string SecureLinks(string content)
        {
            string updatedContent = content;
            Regex unsecureLinks = new(@"http://[^\)]+");
            IEnumerable<string> matches = unsecureLinks.Matches(content).Select(item => item.Value);
            Parallel.For(0, matches.Count(), i =>
            {
                string unsecureLink = matches.ElementAt(i);
                string secureLink = unsecureLink.Insert(4, "s");
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(secureLink);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    lock (updatedContent)
                    {
                        updatedContent = updatedContent.Replace(unsecureLink, secureLink);
                    }
                }
                catch(System.Net.WebException exception)
                {
                    Console.WriteLine($"A secure version of this link doesn't exist: {unsecureLink}");
                }
                catch(Exception exception)
                {
                    Console.WriteLine(exception);
                }
            });
            return updatedContent;
        }

        private static string ImagesToWebp(string content)
        {
            // Not currently being used
            string updatedContent = content;
            Regex media = new("https://intellitect.com/wp-content/[^\"\\)]+");
            List<string> links = media.Matches(updatedContent).Select(item => item.Value).Where(item => item.Contains(".png") || item.Contains(".jpeg") || item.Contains(".jpg")).ToList();
            foreach (string link in links)
            {
                string updatedLink = link.Replace(".png", ".webp");
                updatedLink = updatedLink.Replace(".jpg", ".webp");
                updatedLink = updatedLink.Replace(".jpeg", ".webp");
                updatedContent = updatedContent.Replace(link, updatedLink);
            }
            ImageService.ProcessImages(links, "");
            return updatedContent;
        }
    }
}
