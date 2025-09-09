using ImageMagick;
using System.IO;

namespace ASOFT.Core.Business.Files
{
    public class ImageHelper
    {
        public static void CompressImage(string path)
        {
            var file = new FileInfo(path);

            var optimizer = new ImageOptimizer();
            optimizer.Compress(file);
            file.Refresh();
            //file.CopyTo(path, true);
        }
    }
}
