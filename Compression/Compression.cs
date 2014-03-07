using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Text.RegularExpressions;
//using System.Dynamic;
using System.IO;
using System.IO.Compression;

namespace DynamicSugar.Compression {

    public static class GZip {

        public static void GZipFolder(string path, string wildCard) {

            var files = System.IO.Directory.GetFiles(path, wildCard);

            foreach(var file in files)
                GZipFile(file);
        }

        public static string UnGZipFile(string fileName) {

            var buffer          = System.IO.File.ReadAllBytes(fileName);
            var bufferNotZipped = Unzip(buffer);
            var newName         = Path.Combine(Path.GetDirectoryName(fileName), Path.GetFileNameWithoutExtension(fileName));
            System.IO.File.WriteAllBytes(newName, bufferNotZipped);
            return newName;
        }

        /// <summary>
        /// Gzip a file. 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="extension"></param>
        /// <returns></returns>
        public static string GZipFile(string fileName, string extension = "gzip") {

            var bufferNotGZipped = System.IO.File.ReadAllBytes(fileName);
            var buffer           = Zip(bufferNotGZipped);
            var newName          = fileName + "." + extension;
            System.IO.File.WriteAllBytes(newName, buffer);
            return newName;
        }

        private static void CopyTo(Stream src, Stream dest) {

            byte[] bytes = new byte[4096];
            int cnt;
            while ((cnt = src.Read(bytes, 0, bytes.Length)) != 0) {
                dest.Write(bytes, 0, cnt);
            }
        }

        public static byte[] Zip(string str) {

             return Zip(Encoding.UTF8.GetBytes(str));
        }

        public static byte[] Zip(byte[] bytes) {
           
            using (var msi = new MemoryStream(bytes)) {
                using (var mso = new MemoryStream()) {
                    using (var gs = new GZipStream(mso, CompressionMode.Compress)) {
                        #if NET4
                            msi.CopyTo(gs);
                        #else
                            CopyTo(msi, gs);
                        #endif
                    }
                    return mso.ToArray();
                }
            }
        }

        public static string UnzipAsString(byte[] bytes) {

            return Encoding.UTF8.GetString(Unzip(bytes));
        }

        public static byte[] Unzip(byte[] bytes) {

            using (var msi = new MemoryStream(bytes)) {
                using (var mso = new MemoryStream()) {
                    using (var gs = new GZipStream(msi, CompressionMode.Decompress)) {
                        #if NET4
                            gs.CopyTo(mso);
                        #else
                            CopyTo(gs, mso);
                        #endif
                    }
                    return mso.ToArray();
                }
            }
        }
    }
}
