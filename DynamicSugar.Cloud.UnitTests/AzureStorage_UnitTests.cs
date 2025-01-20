using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace DynamicSugar.Cloud.UnitTests
{
    [TestClass]
    public class AzureStorage_UnitTests
    {
        public string ContainerName1 = "asset-00a737b0-3bd0-4040-b0c1-e7b47896dc7d";

        [TestMethod]
        public void ContainerExists()
        {
            var az = GetAzureStorage();
            Assert.IsTrue(az.ContainerExists(ContainerName1));
            Assert.IsTrue(az.BlobExists(ContainerName1, "a_init.js"));
            // https://bskamsstorageqauswest.blob.core.windows.net/asset-00a737b0-3bd0-4040-b0c1-e7b47896dc7d/a_init.js
        }

        private static AzureStorage GetAzureStorage()
        {
            return new AzureStorage(Environment.GetEnvironmentVariable("DynamicSugar.Cloud.UnitTests.Azure"));
        }

        [TestMethod]
        public void DownloadBlob()
        {
            var az = GetAzureStorage();
            var fileName = az.DownloadBlobAsync(ContainerName1, "a_init.js").GetAwaiter().GetResult();
            Assert.IsTrue(File.Exists(fileName));
        }
    }
}
