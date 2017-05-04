using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ANAConversationPlatform.Helpers
{
    public static class FileSaveHelper
    {
        public static bool Save(string fullFileName, Stream fileStream)
        {
            using (FileStream fs = new FileStream(fullFileName, FileMode.OpenOrCreate))
            {
                fileStream.CopyTo(fs);
                fs.Flush();
            }
            return true;
        }
    }
}
