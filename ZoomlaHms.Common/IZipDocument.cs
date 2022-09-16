using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomlaHms.Common
{
    public interface IZipDocument : IDisposable
    {
        public string SavePath { get; }
        public void Add(string path);
        public void Add(string path, string packPath);
        public void Delete(string packPath);
        public byte[] Get(string packPath);
        public void Save();
    }
}
