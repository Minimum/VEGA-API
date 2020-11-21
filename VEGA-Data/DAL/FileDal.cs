using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace VEGA_Data.DAL
{
    public class FileDal
    {
        public void SaveFile(String path, Stream data)
        {
            using (FileStream stream = new FileStream(path, FileMode.CreateNew))
            {
                data.CopyTo(stream);
            }
        }

        public FileStream LoadFile(String path)
        {
            return new FileStream(path, FileMode.Open);
        }
    }
}
