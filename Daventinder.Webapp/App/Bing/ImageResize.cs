using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Daventinder.Webapp.App.Bing
{
    class ImageResize
    {
        public static void ResizeImageFixedWidth(string imgUrl, string path, int width)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(imgUrl);
            httpWebRequest.Timeout = 2000;
            HttpWebResponse httpWebReponse = (HttpWebResponse)httpWebRequest.GetResponse();
            Stream stream = httpWebReponse.GetResponseStream();

            try
            {
                Image imgToResize = Image.FromStream(stream);

                int sourceWidth = imgToResize.Width;
                int sourceHeight = imgToResize.Height;

                float nPercent = ((float)width / (float)sourceWidth);

                int destWidth = (int)(sourceWidth * nPercent);
                int destHeight = (int)(sourceHeight * nPercent);

                Bitmap b = new Bitmap(destWidth, destHeight);
                Graphics g = Graphics.FromImage((Image)b);
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
                g.Dispose();

                ((Image)b).Save(path, ImageFormat.Jpeg);
            }
            catch (Exception)
            {
                
            }
        }
    }
}
