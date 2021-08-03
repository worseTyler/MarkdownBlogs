using OpenQA.Selenium;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using OpenQA.Selenium.Chrome;

namespace UpdateBlogScript
{
    class Program
    {
        public static string WebsiteUrl = ""; // example: https://itltcweb-clone.azurewebsites.net/wp-admin
        public static string Username = "";
        public static string Password = "";
        public static int Count = 0;
        static void Main(string[] args)
        {
            // List of all blog id's that need to be updated
            List<int> list = new();
            int numWorkers = 1;

            Parallel.For(0, numWorkers, new ParallelOptions { MaxDegreeOfParallelism = numWorkers }, i =>
            {
                DoWork(i, numWorkers, list);
            });
        }
        public static void GoToUrl(IWebDriver driver, int id)
        {
            driver.Navigate().GoToUrl($"{WebsiteUrl}/post.php?post={id}&action=edit");
            try
            {
                var possibleBox = driver.FindElement(By.Id("markdown-switch"));
                possibleBox.Click();
                Console.WriteLine($"Weird Page: {id}");

            }
            catch
            {
                var updateBox = driver.FindElement(By.Id("publish"));
                updateBox.Click();
            }
        }
        public static void DoWork(int worker, int numWorkers, List<int> list)
        {
            int totalWork = list.Count();
            int start = totalWork * worker / numWorkers;
            int end = totalWork * (worker + 1) / numWorkers;
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("--incognito");
            //options.AddArgument("headless");
            IWebDriver driver = new OpenQA.Selenium.Chrome.ChromeDriver(options);
            driver.Navigate().GoToUrl(WebsiteUrl);

            var userNameBox = driver.FindElement(By.Id("user_login"));
            var passwordBox = driver.FindElement(By.Id("user_pass"));
            var loginBox = driver.FindElement(By.Id("wp-submit"));
            userNameBox.SendKeys(Username);
            passwordBox.SendKeys(Password);
            loginBox.Click();
            Thread.Sleep(5000);
            for (int i = start; i < end; i++)
            {
                int index = i % totalWork;  
                try
                {
                    GoToUrl(driver, list[index]);
                    Interlocked.Increment(ref Count);
                    Console.WriteLine($"Success {Count} of {totalWork}: {list[index]}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine($"Error: {list[index]}");
                }
            }
            driver.Close();
        }
    }
}
