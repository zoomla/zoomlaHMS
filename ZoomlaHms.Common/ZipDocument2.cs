using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ZoomlaHms.Common
{
    public class ZipDocument2 : IZipDocument
    {
        private bool disposedValue;
        private ZipFile archive;
        private bool hasTrans = false;

        public string SavePath { get; }

        public ZipDocument2(string path)
        {
            SavePath = path;

            if (File.Exists(path))
            { archive = new ZipFile(path); }
            else
            { archive = ZipFile.Create(path); }
        }

        public void Add(string path)
        {
            Add(path, null);
        }

        public void Add(string path, string packPath)
        {
            CheckDispose();

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
                //AddDirectory(path, string.IsNullOrEmpty(packPath) ? path.Substring(path.LastIndexOf(Path.DirectorySeparatorChar) + 1).TrimEnd(Path.DirectorySeparatorChar) : packPath);
            }
        }

        private void AddFile(string path, string packPath)
        {
            bool localTrans = !hasTrans;
            if (localTrans)
            {
                archive.BeginUpdate();
                hasTrans = true;
            }

            Delete(packPath);
            archive.Add(path, packPath);

            if (localTrans)
            {
                archive.CommitUpdate();
                hasTrans = false;
            }
        }

        private void AddDirectory(string path, string packPath)
        {
            bool localTrans = !hasTrans;
            if (localTrans)
            {
                archive.BeginUpdate();
                hasTrans = true;
            }

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

                string dicName = packPath + packDicPath.Substring(packDicPath.LastIndexOf(Path.DirectorySeparatorChar) + 1);
                if (archive.GetEntry(dicName) == null)
                {
                    archive.AddDirectory(dicName);
                }

                AddDirectory(packDicPath, dicName);
            }

            if (localTrans)
            {
                archive.CommitUpdate();
                hasTrans = false;
            }
        }

        public void Delete(string packPath)
        {
            CheckDispose();

            var entry = archive.GetEntry(packPath);
            if (entry == null)
            { return; }

            if (hasTrans)
            {
                archive.Delete(entry);
            }
            else
            {
                archive.BeginUpdate();
                archive.Delete(entry);
                archive.CommitUpdate();
            }
        }

        public byte[] Get(string packPath)
        {
            CheckDispose();

            var entry = archive.GetEntry(packPath);
            if (entry == null || entry.IsDirectory)
            { return null; }

            using var stream = archive.GetInputStream(entry.ZipFileIndex);
            byte[] buffer = new byte[stream.Length];
            stream.Read(buffer);
            return buffer;
        }

        public void Save()
        {
            CheckDispose();

            //...
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)
                    archive.Close();
                }

                // TODO: 释放未托管的资源(未托管的对象)并重写终结器
                // TODO: 将大型字段设置为 null
                disposedValue = true;
            }
        }

        // // TODO: 仅当“Dispose(bool disposing)”拥有用于释放未托管资源的代码时才替代终结器
        // ~ZipDocument2()
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

        private void CheckDispose()
        {
            if (disposedValue)
                throw new InvalidOperationException("Object destroyed");
        }
    }
}
