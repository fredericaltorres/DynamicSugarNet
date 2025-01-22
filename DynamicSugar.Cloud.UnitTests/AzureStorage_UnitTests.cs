using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace DynamicSugar.Cloud.UnitTests
{
    [TestClass]
    public class AzureStorage_UnitTests
    {
        public string TestContainerName = "dynamic-sugar-cloud-unittests";
        public string TestFileName = "dynamic-sugar-cloud-unittests.txt";
        public string TestContainerName2 = "dynamic-sugar-cloud-unittests-2";

        private static AzureStorage GetAzureStorage()
        {
            return new AzureStorage(Environment.GetEnvironmentVariable("DynamicSugar.Cloud.UnitTests.Azure"));
        }

        [TestMethod]
        public void CreateContainer_ContainerExists_DeleteContainer()
        {
            var az = GetAzureStorage();
            az.CreateContainer(TestContainerName2);
            Assert.IsTrue(az.ContainerExists(TestContainerName2));
            az.DeleteContainer(TestContainerName2);
            Assert.IsFalse(az.ContainerExists(TestContainerName2));
        }

        [TestMethod]
        public void ContainerExists()
        {
            var az = GetAzureStorage();
            Assert.IsTrue(az.ContainerExists(TestContainerName));
            Assert.IsTrue(az.BlobExists(TestContainerName, TestFileName));
        }

        [TestMethod]
        public void DownloadBlob()
        {
            using (var tfh = new TestFileHelper())
            {
                var az = GetAzureStorage();
                var fileName = az.DownloadBlobAsync(TestContainerName, TestFileName).GetAwaiter().GetResult();
                Assert.IsTrue(File.Exists(fileName));
                tfh.TrackFile(fileName);
            }
        }

        [TestMethod]
        public void UploadBlobTextFile()
        {
            using (var tfh = new TestFileHelper())
            {
                var testFile = tfh.CreateTempFile("hello world", ".txt");
                var testFileNameOnly = Path.GetFileName(testFile);
                var az = GetAzureStorage();
            
                az.UploadBlobAsync(TestContainerName, testFileNameOnly, testFile, "plain/text").GetAwaiter().GetResult();

                var fileName = az.DownloadBlobAsync(TestContainerName, testFileNameOnly).GetAwaiter().GetResult();
                Assert.IsTrue(File.Exists(fileName));

                Assert.IsTrue(az.BlobExists(TestContainerName, testFileNameOnly));

                tfh.TrackFile(fileName);

                az.DeleteBlob(TestContainerName, testFileNameOnly);
            }
        }
    }
}
