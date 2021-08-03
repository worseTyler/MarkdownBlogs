using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MarkdownBlogs
{
    static class BlogAnalyzer
    {
        public static BlogData Analyze(Blog blog)
        {
            string content = blog.Content;
            return new BlogData()
            {
                Slug = blog.Slug,
                HeaderOne = HeaderOneCount(content),
                HeaderTwo = HeaderTwoCount(content),
                HeaderThree = HeaderThreeCount(content),
                HeaderTotal = TotalHeaderCount(content),
                ImageTotal = ImageCount(content),
                WordsCount = WordCount(content),
                CodeCount = CodeWordCount(content),
                EmbededHtml = FindEmbededHtml(content),
                DeadLinks = TryLinks(content) 
            };
        }
        private static int HeaderOneCount(string content)
        {
            Regex removeCode = new(@"(?S)```[^`]+```");
            content = removeCode.Replace(content, "");
            Regex headerOnes = new Regex("\n#[^#\n]+\\w.+[^\n]");
            return headerOnes.Matches(content).Count();
        }
        private static int HeaderTwoCount(string content)
        {
            Regex headerTwos = new Regex("\n##[^#\n]+\\w.+[^\n]");
            int count = headerTwos.Matches(content).Count();
            return count;
        }
        private static int HeaderThreeCount(string content)
        {
            Regex headerTwos = new Regex("\n###[^#\n]+\\w.+[^\n]");
            return headerTwos.Matches(content).Count();
        }
        private static int TotalHeaderCount(string content)
        {
            Regex headers = new Regex(@"\n#+.+");
            return headers.Matches(content).Count();
        }
        private static int ImageCount(string content)
        {
            Regex images = new Regex(@"!\[.+\]\(.+\)");
            return images.Matches(content).Count();
        }
        private static int WordCount(string content)
        {
            return content.Split(" ").Length;
        }
        private static int CodeWordCount(string content)
        {
            int total = 0;
            Regex code = new Regex(@"(?S)```[^`]+```");
            MatchCollection matches = code.Matches(content);
            foreach (Match match in matches)
            {
                string codeInstance = match.Value;
                total += codeInstance.Split(" ").Length;
                total += codeInstance.Split(".").Length;
            }
            return total;
        }
        private static bool FindEmbededHtml(string content)
        {
            Regex embed = new(@"<.+>.+</.+>");
            return embed.IsMatch(content);
        }
        private static List<string> TryLinks(string content)
        {
            List<string> deadLinks = new();
            Regex links = new Regex(@"https*://[^\)]+");
            IEnumerable<string> matches = links.Matches(content).Select(item => item.Value);
            Parallel.For(0, matches.Count(), i =>
            {
                string link = matches.ElementAt(i);
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(link);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    if(response.StatusCode != HttpStatusCode.OK)
                    {
                        Console.WriteLine(link);
                        deadLinks.Add(link);
                    }
                }
                catch (System.Net.WebException exception)
                {
                    Console.WriteLine(exception);
                    Console.WriteLine(link);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            });
            return deadLinks;
        }
    }
}
