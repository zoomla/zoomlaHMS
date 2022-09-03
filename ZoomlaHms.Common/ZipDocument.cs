using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoomlaHms.Common
{
    public class ZipDocument : IDisposable
    {
        private FileStream archiveStream;
        private ZipArchive archive;
        private bool disposedValue;

        public string SavePath { get; }

        public ZipDocument(string path)
        {
            SavePath = path;
            archiveStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            archive = new ZipArchive(archiveStream, ZipArchiveMode.Update, true);
        }

        public void Add(string path)
        {
            Add(path, null);
        }

        public void Add(string path, string packPath)
        {
            path = path
                    .Replace('\\', Path.DirectorySeparatorChar)
                    .Replace('/', Path.DirectorySeparatorChar);

            if (File.Exists(path))
            {
                AddFile(path, string.IsNullOrEmpty(packPath) ? Path.GetFileName(path) : packPath);
            }
            else if (Directory.Exists(path))
            {
                AddDirectory(path, string.IsNullOrEmpty(packPath) ? string.Empty : packPath);
                //AddDirectory(path, string.IsNullOrEmpty(packPath) ? path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1) : packPath);
            }
        }

        private void AddFile(string path, string packPath)
        {
            var entry = archive.GetEntry(packPath);
            if (entry != null)
            {
                entry.Delete();
            }
            entry = archive.CreateEntry(packPath);

            using Stream entryStream = entry.Open();
            using var fileStream = File.OpenRead(path);
            fileStream.CopyTo(entryStream);
            entryStream.Flush();
        }

        private void AddDirectory(string path, string packPath)
        {
            if (!string.IsNullOrEmpty(packPath) && !packPath.EndsWith("/"))
            { packPath += "/"; }

            var fileList = Directory.GetFiles(path);
            foreach (var file in fileList)
            {
                string packFilePath = Path.GetFileName(file);
                AddFile(file, packPath + packFilePath);
            }
            var dirList = Directory.GetDirectories(path);
            foreach (var dir in dirList)
            {
                string packDicPath = dir
                    .Replace('\\', Path.DirectorySeparatorChar)
                    .Replace('/', Path.DirectorySeparatorChar);

                AddDirectory(packDicPath, packPath + packDicPath.Substring(packDicPath.LastIndexOf(Path.DirectorySeparatorChar) + 1));
            }

            if (fileList.Length == 0 && dirList.Length == 0)
            {
                var entry = archive.GetEntry(packPath);
                if (entry != null)
                { entry.Delete(); }

                entry = archive.CreateEntry(packPath);
            }
        }

        public void Delete(string packPath)
        {
            if (packPath.EndsWith("/"))
            {
                var list = new List<ZipArchiveEntry>();
                foreach (var item in archive.Entries)
                {
                    if (item.FullName.StartsWith(packPath, StringComparison.OrdinalIgnoreCase))
                    { list.Add(item); }
                }

                foreach (var item in list)
                { item.Delete();  }
                return;
            }

            var entry = archive.GetEntry(packPath);
            if (entry == null)
            { return; }

            entry.Delete();
        }

        public void SaveChangesToFile()
        {
            archive.Dispose();
            archiveStream.Flush();
            archiveStream.Seek(0, SeekOrigin.Begin);
            archive = new ZipArchive(archiveStream, ZipArchiveMode.Update, true);
        }

        public void SaveZipFileAs(string inZipPath, string outputPath)
        {
            var entry = archive.GetEntry(inZipPath);
            if (entry == null)
            { return; }

            if (inZipPath.EndsWith("/"))
            { return; }


        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    archive.Dispose();
                    archiveStream.Dispose();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~ZipDocument()
        // {
        //     // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // 不要更改此代码。请将清理代码放入“Dispose(bool disposing)”方法中
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
