using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DC_Assignment1
{
    public abstract class SharedFile
    {
        public string FileName { get; set; }
        public object Content { get; set; }

        public SharedFile(string fileName, object content)
        {
            FileName = fileName;
            Content = content;
        }
    }
}
