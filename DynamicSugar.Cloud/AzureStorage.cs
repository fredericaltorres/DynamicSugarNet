using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using System.IO;

namespace DynamicSugar.Cloud
{
    public class AzureStorage 
    {
        private BlobClientOptions _clientOptions;
        private BlobServiceClient _blobServiceClient;

        private AzureStorage()
        {
        }

        public AzureStorage(string storageConnectionString)
        {
            Initialize(storageConnectionString);
        }

        public void Initialize(string storageConnectionString)
        {
            _clientOptions = new BlobClientOptions
            {
                Retry = { Delay = TimeSpan.FromSeconds(30), MaxRetries = 3, Mode = RetryMode.Exponential, MaxDelay = TimeSpan.FromSeconds(30) }
            };

            _blobServiceClient = new BlobServiceClient(storageConnectionString, _clientOptions);
        }

        public void CreateContainer(string containerName, string encryptionScope = null)
        {
            BlobContainerEncryptionScopeOptions scope = null;
            if (encryptionScope != null)
            {
                scope = new BlobContainerEncryptionScopeOptions()
                {
                    DefaultEncryptionScope = encryptionScope,
                    PreventEncryptionScopeOverride = true
                };
            }

            BlobContainerClient container = _blobServiceClient.GetBlobContainerClient(containerName);
            container.CreateIfNotExistsAsync(PublicAccessType.None, null, scope).GetAwaiter().GetResult();
        }

        private static Uri GetURI(BlobClient blobClient, bool writeMode, bool createMode, string storedPolicyName = null)
        {
            // Check whether this BlobClient object has been authorized with Shared Key.
            if (blobClient.CanGenerateSasUri)
            {
                // Create a SAS token that's valid for one hundred years.
                BlobSasBuilder sasBuilder = new BlobSasBuilder()
                {
                    BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                    BlobName = blobClient.Name,
                    Resource = "b"
                };

                if (storedPolicyName == null)
                {
                    sasBuilder.ExpiresOn = DateTimeOffset.UtcNow.AddHours(12);
                    if (createMode)
                    {
                        sasBuilder.SetPermissions(BlobSasPermissions.All);
                    }
                    else if (writeMode)
                    {
                        sasBuilder.SetPermissions(BlobSasPermissions.Read | BlobSasPermissions.Write);
                    }
                    else
                    {
                        sasBuilder.SetPermissions(BlobSasPermissions.Read);
                    }
                }
                else
                {
                    sasBuilder.Identifier = storedPolicyName;
                }

                Uri r = blobClient.GenerateSasUri(sasBuilder);
                return r;
            }
            else
            {
                throw new Exception(@"BlobClient not authorized to generate SAS key");
            }
        }

        public List<string> ListBlobsAsync(string containerName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            var r = new List<string>();
            foreach (BlobItem blobItem in blobContainerClient.GetBlobs())
                r.Add(blobItem.Name);

            return r;
        }

        public bool BlobExists(string containerName, string blobName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
            var r = blobClient.ExistsAsync().GetAwaiter().GetResult();
            return r;
        }

        public bool DeleteBlob(string containerName, string blobName)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);
            var r = blobClient.ExistsAsync().GetAwaiter().GetResult();
            if (r)
                blobClient.DeleteAsync().GetAwaiter().GetResult();
            return true;
        }

        public Uri GetBlobURI(string containerName, string blobName, bool writeMode = false, bool createMode = false)
        {
            var blobContainerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            BlobClient blobClient = blobContainerClient.GetBlobClient(blobName);

            return GetURI(blobClient, writeMode, createMode);
        }

        public BlobContainerClient GetContainer(string containerName)
        {
            return _blobServiceClient.GetBlobContainerClient(containerName);
        }

        public async Task DeleteContainer(string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.DeleteAsync();
        }

        public int CopyContainer(string sourceName, string destName, string encryptionScope = null)
        {
            int copyCount = 0;
            if (!ContainerExists(destName))
            {
                this.CreateContainer(destName, encryptionScope);
            }

            if (IsContainerEmpty(destName))
            {
                var destContainerClient = _blobServiceClient.GetBlobContainerClient(destName);
                var sourceContainerClient = _blobServiceClient.GetBlobContainerClient(sourceName);

                var blobs = sourceContainerClient.GetBlobs();
                var tasks = new List<Task>();
                foreach (BlobItem blobItem in blobs)
                {
                    BlobClient sourceBlob = sourceContainerClient.GetBlobClient(blobItem.Name);
                    tasks.Add(CopyBlobAsync(sourceBlob, destContainerClient));
                    copyCount++;
                }

                Task.WaitAll(tasks.ToArray());
            }
            else
            {
                throw new ApplicationException($"Target container of copy target {destName} already exists");
            }

            return copyCount;
        }

        public int GetContainerFileCount(string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var blobs = containerClient.GetBlobs().ToList(); // Force loading of all files
            return blobs.Count;
        }

        private bool IsContainerEmpty(string containerName)
        {
            return GetContainerFileCount(containerName) == 0;
        }

        public bool ContainerExists(string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            var response = containerClient.ExistsAsync().GetAwaiter().GetResult();
            return response;
        }

        public bool IsValidContainerName(string containerName)
        {
            bool ret = true;
            try
            {
                NameValidator.ValidateContainerName(containerName);
            }
            catch (ArgumentException a)
            {
                ret = false;
            }
            return ret;
        }

        public bool IsContainerEncrypted(string containerName, string scope)
        {
            return GetEncryptionScope(containerName) == scope ? true : false;
        }

        public string GetEncryptionScope(string containerName)
        {
            var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            Azure.Storage.Blobs.Models.BlobContainerProperties props = containerClient.GetProperties();
            return props.DefaultEncryptionScope;
        }

        private async Task CopyBlobAsync(BlobClient sourceBlob, BlobContainerClient destContainer)
        {
            // Ensure that the source blob exists.
            if (await sourceBlob.ExistsAsync())
            {
                Azure.Storage.Blobs.Models.BlobProperties sourceProperties = await sourceBlob.GetPropertiesAsync();

                BlobLeaseClient lease = sourceBlob.GetBlobLeaseClient();
                if (sourceProperties.LeaseState == Azure.Storage.Blobs.Models.LeaseState.Leased)
                {
                    Console.WriteLine($"Breaking lease on blob {sourceBlob.Name}");
                    await lease.BreakAsync();
                }

                await lease.AcquireAsync(TimeSpan.FromSeconds(-1));

                try
                {
                    BlobClient destBlob = destContainer.GetBlobClient(sourceBlob.Name);
                    await destBlob.StartCopyFromUriAsync(sourceBlob.Uri);
                    Azure.Storage.Blobs.Models.BlobProperties destProperties = await destBlob.GetPropertiesAsync();
                    sourceProperties = await sourceBlob.GetPropertiesAsync();

                    if (!(await VerifyCopy(destContainer.Name, sourceBlob, destBlob)))
                    {
                        throw new ApplicationException($"Container copy failed validation for {destContainer.Name} and blob {destBlob.Name}.");
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    if (sourceProperties.LeaseState == Azure.Storage.Blobs.Models.LeaseState.Leased)
                    {
                        await lease.BreakAsync();
                    }
                }
            }
        }

        private async Task<bool> VerifyCopy(string containerName, BlobClient sourceClient, BlobClient destClient, int depth = 0)
        {
            bool valid = false;
            Azure.Storage.Blobs.Models.BlobProperties sourceProperties = sourceClient.GetProperties();
            Azure.Storage.Blobs.Models.BlobProperties destProperties = destClient.GetProperties();

            //Make sure copy is completed
            if (destProperties.CopyStatus == Azure.Storage.Blobs.Models.CopyStatus.Pending)
            {
                int delay = depth >= 5 ? 5 * 1000 : 1000 * (1 + depth); //delay in milliseconds (max 5 seconds)
                await Task.Delay(delay);
                return await VerifyCopy(containerName, sourceClient, destClient, ++depth);
            }
            else if (destProperties.CopyStatus != Azure.Storage.Blobs.Models.CopyStatus.Success)
            {
                throw new ApplicationException($"Could not copy blob {sourceClient.Name} in container {containerName}. Copy status was {destProperties.CopyStatus}.");
            }

            if (sourceProperties.ContentHash == null || destProperties.ContentHash == null)
            {
                //check blob size
                if (sourceProperties.ContentLength == destProperties.ContentLength)
                {
                    valid = true;
                }
            }
            else
            {
                //check hash
                if (sourceProperties.ContentHash.SequenceEqual(destProperties.ContentHash))
                {
                    valid = true;
                }
            }

            return valid;
        }

        public async Task<string> DownloadBlobAsync(string containerName, string blobName)
        {
            try
            {
                var localFileName = Path.Combine(Path.GetTempPath(), Path.GetFileName(blobName));
                var container = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = container.GetBlobClient(blobName);
                await blobClient.DownloadToAsync(localFileName);

                return localFileName;
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to download file:{blobName}, from container:{containerName}, message:{ex.Message}", ex);
            }
        }

        public async Task UploadBlobAsync(string containerName, string blobName, string localFileName, string contentType, bool overwrite = true)
        {
            try
            {
                var container = _blobServiceClient.GetBlobContainerClient(containerName);
                var blobClient = container.GetBlobClient(blobName);

                using (var uploadFileStream = File.OpenRead(localFileName))
                {
                    await blobClient.UploadAsync(uploadFileStream, new BlobHttpHeaders { ContentType = contentType });
                }
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to upload file:{blobName}, to container:{containerName}, message:{ex.Message}", ex);
            }
        }
    }
}
