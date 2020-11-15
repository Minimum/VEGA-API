using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VEGA_API.DAL;

namespace VEGA_API.Files
{
    public sealed class FileService
    {
        private FileDal dal { get; set; }

        public FileService()
        {
            dal = new FileDal();
        }

        public void CreateFile(File file)
        {
            if(file.Id != 0)
                throw new Exception("Cannot create a file that already exists.");


            // insert file
        }

        public void CreateFile(File file, FileInstance instance)
        {
            CreateFile(file);
            CreateFileInstance(instance);
        }

        public void CreateFileInstance(FileInstance instance)
        {
            // insert instance
        }

        public void SaveData(File file, Volume volume, Stream data)
        {
            dal.SaveFile(GetFilePath(file, volume), data);
        }

        public Stream LoadData(File file, Volume volume)
        {
            return dal.LoadFile(GetFilePath(file, volume));
        }

        public String GetFilePath(File file, Volume volume)
        {
            String id = file.Id.ToString();
            String filePath = "" + id[0];

            for (int x = 1; x < id.Length; x++)
            {
                if (x % 2 == 0)
                {
                    filePath += "/" + id[x];
                }
                else
                {
                    filePath += id[x];
                }
            }

            return volume.Path + '/' + filePath;
        }
    }
}
