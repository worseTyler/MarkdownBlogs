using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MarkdownBlogs
{
    class DatabaseService
    {
        private string Server;
        private string Database;
        private string UserName;
        private string Password;
        private MySqlConnection Connection;
        public DatabaseService(string server, string database, string userName, string password)
        {
            Server = server;
            Database = database;
            UserName = userName;
            Password = password;
            Connection = InitializeConnection();
        }
        private MySqlConnection InitializeConnection()
        {
            return new($"Server={Server}; database={Database}; UID={UserName}; password={Password}");
        }

        public void PushUpBlogs(List<Blog> blogs)
        {
            Connection.Open();
            foreach(Blog blog in blogs)
            {
                string updateBlogQuery = $"update {Database}.wp_posts set post_content = '{blog.Content}' where post_name = '{blog.Slug}' and post_status = 'publish' and post_type = 'post'";
                MySqlCommand updateBlog = new(updateBlogQuery, Connection);
                updateBlog.ExecuteNonQuery();
            }
            Connection.Close();
        }

        public List<Blog> PullDownBlogs()
        {
            Connection.Open();
            List<Blog> blogs = new();
            // Gets Markdown Content
            string getBlogsQuery = $"select post_name, post_content_filtered from {Database}.wp_posts where post_status = 'publish' and post_type = 'post'";
            MySqlCommand getBlogs = new(getBlogsQuery, Connection);
            MySqlDataReader reader = getBlogs.ExecuteReader();
            while (reader.Read())
            {
                Blog blog = new();
                blog.Slug = (string)reader["post_name"];
                blog.Content = (string)reader["post_content_filtered"];
                blogs.Add(blog);
            }
            reader.Close();
            Connection.Close();
            return blogs;
        }

        public void WriteBlogsToFile(string destinationFolder)
        {
            List<Blog> blogs = PullDownBlogs();
            if (Directory.Exists(destinationFolder))
            {
                foreach (Blog blog in blogs)
                {
                    File.WriteAllText(destinationFolder + blog.Slug + ".md", blog.Content);
                }
            }
            else
            {
                throw new DirectoryNotFoundException();
            }
        }

        public List<int> GetPublishedIds()
        {
            Connection.Open();
            List<int> ids = new();
            // Gets Markdown Content
            string getIdsQuery = "select id where post_status = 'publish' and post_type = 'post'";
            MySqlCommand getIds = new(getIdsQuery, Connection);
            MySqlDataReader reader = getIds.ExecuteReader();
            while (reader.Read())
            {
                int id = (int)reader["id"];
                ids.Add(id);
            }
            reader.Close();
            Connection.Close();
            return ids;
        }
    }
}
