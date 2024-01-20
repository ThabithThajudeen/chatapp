using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DC_Assignment1
{
    public class ImageFile : SharedFile
    {
         
            public ImageFile(string fileName, byte[] content) : base(fileName, content) { }
    }


}

