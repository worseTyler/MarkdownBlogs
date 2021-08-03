using ImageMagick;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdownBlogs
{
    static class ImageService
    {
        public static void ProcessImages(List<string> links, string folderDestination)
        {
            Parallel.For(0, links.Count, i =>
            {
                if (!File.Exists($"{folderDestination}\\{ links[i].Split("/")[^1]}")){
                    using (var image = new MagickImage(links[i].Trim()))
                    {
                        int maxHeight = 800;
                        int maxWidth = 800;
                        int height = image.Height <= maxHeight ? image.Height : maxHeight;
                        int width = image.Width <= maxWidth ? image.Width : maxWidth;
                        var size = new MagickGeometry(width, height);
                        size.IgnoreAspectRatio = false;
                        image.Resize(size);
                        string[] split = links[i].Split("/");
                        if (!Directory.Exists($"{folderDestination}\\{ split[^3]}\\{ split[^2]}"))
                        {
                            Directory.CreateDirectory($"{folderDestination}\\{ split[^3]}\\{ split[^2]}");
                        }
                        image.Write($"{folderDestination}\\{split[^3]}\\{split[^2]}\\{split[^1].Split(".")[^2] + ".webp"}");
                    }
                }
            });
        }
    }
}

/*
 * After having all images into the correct destinations in the correct folders 
 * it might make the most sense to connect to the app service and use FTP to put the
 * new images into their correct spots, however at the time of writing this it doesn't
 * seem like wordpress is currently supporting webp images so this isn't currently
 * being used 
 */
// example link: https://intellitect.com/wp-content/uploads/2021/06/Intell-Mobile-20210701-000826.png
