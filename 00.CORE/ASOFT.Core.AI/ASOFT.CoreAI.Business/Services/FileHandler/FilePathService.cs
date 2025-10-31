using ASOFT.CoreAI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASOFT.CoreAI.Business
{
    public class FilePathService
    {
        public IReadOnlyList<AttachFileModel> NormalizeToPhysicalUnderWebRoot(
        string webRootPath, IEnumerable<AttachFileModel> files)
        {
            if (string.IsNullOrWhiteSpace(webRootPath)) return Array.Empty<AttachFileModel>();
            if (files == null) return Array.Empty<AttachFileModel>();

            var list = new List<AttachFileModel>();
            foreach (var f in files)
            {
                if (string.IsNullOrWhiteSpace(f?.AttachURL)) continue;

                var relative = f.AttachURL
                    .Replace("~\\", string.Empty)
                    .Replace("~", string.Empty)
                    .TrimStart('\\', '/')
                    .Replace("/", "\\");
                var abs = Path.GetFullPath(Path.Combine(webRootPath, relative));
                if (File.Exists(abs))
                {
                    list.Add(new AttachFileModel
                    {
                        AttachID = f.AttachID,
                        AttachName = f.AttachName,
                        AttachURL = abs
                    });
                }
            }
            return list;
        }
    }
}
