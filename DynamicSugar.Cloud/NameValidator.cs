// old Microsoft.WindowsAzure.Storage assembly

using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace DynamicSugar.Cloud
{
    //
    // Summary:
    //     Provides helpers to validate resource names across the Microsoft Azure Storage
    //     Services.
    public static class NameValidator
    {
        private const int BlobFileDirectoryMinLength = 1;

        private const int ContainerShareQueueTableMinLength = 3;

        private const int ContainerShareQueueTableMaxLength = 63;

        private const int FileDirectoryMaxLength = 255;

        private const int BlobMaxLength = 1024;

        private static readonly string[] ReservedFileNames = new string[25]
        {
            ".", "..", "LPT1", "LPT2", "LPT3", "LPT4", "LPT5", "LPT6", "LPT7", "LPT8",
            "LPT9", "COM1", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9",
            "PRN", "AUX", "NUL", "CON", "CLOCK$"
        };

        private static readonly RegexOptions RegexOptions = RegexOptions.ExplicitCapture | RegexOptions.Singleline | RegexOptions.CultureInvariant;

        private static readonly Regex FileDirectoryRegex = new Regex("^[^\"\\\\/:|<>*?]*\\/{0,1}$", RegexOptions);

        private static readonly Regex ShareContainerQueueRegex = new Regex("^[a-z0-9]+(-[a-z0-9]+)*$", RegexOptions);

        private static readonly Regex TableRegex = new Regex("^[A-Za-z][A-Za-z0-9]*$", RegexOptions);

        private static readonly Regex MetricsTableRegex = new Regex("^\\$Metrics(HourPrimary|MinutePrimary|HourSecondary|MinuteSecondary)?(Transactions)(Blob|Queue|Table)$", RegexOptions);

        //
        // Summary:
        //     Checks if a container name is valid.
        //
        // Parameters:
        //   containerName:
        //     A string representing the container name to validate.
        public static void ValidateContainerName(string containerName)
        {
            if (!"$root".Equals(containerName, StringComparison.Ordinal) && !"$logs".Equals(containerName, StringComparison.Ordinal))
            {
                ValidateShareContainerQueueHelper(containerName, "container");
            }
        }

        //
        // Summary:
        //     Checks if a queue name is valid.
        //
        // Parameters:
        //   queueName:
        //     A string representing the queue name to validate.
        public static void ValidateQueueName(string queueName)
        {
            ValidateShareContainerQueueHelper(queueName, "queue");
        }

        //
        // Summary:
        //     Checks if a share name is valid.
        //
        // Parameters:
        //   shareName:
        //     A string representing the share name to validate.
        public static void ValidateShareName(string shareName)
        {
            ValidateShareContainerQueueHelper(shareName, "share");
        }

        private static void ValidateShareContainerQueueHelper(string resourceName, string resourceType)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid {0} name. The {0} name may not be null, empty, or whitespace only.", new object[1] { resourceType }));
            }

            if (resourceName.Length < 3 || resourceName.Length > 63)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid {0} name length. The {0} name must be between {1} and {2} characters long.", new object[3] { resourceType, 3, 63 }));
            }

            if (!ShareContainerQueueRegex.IsMatch(resourceName))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid {0} name. Check MSDN for more information about valid {0} naming.", new object[1] { resourceType }));
            }
        }

        //
        // Summary:
        //     Checks if a blob name is valid.
        //
        // Parameters:
        //   blobName:
        //     A string representing the blob name to validate.
        public static void ValidateBlobName(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid {0} name. The {0} name may not be null, empty, or whitespace only.", new object[1] { "blob" }));
            }

            if (blobName.Length < 1 || blobName.Length > 1024)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid {0} name length. The {0} name must be between {1} and {2} characters long.", new object[3] { "blob", 1, 1024 }));
            }

            int num = 0;
            foreach (char c in blobName)
            {
                if (c == '/')
                {
                    num++;
                }
            }

            if (num >= 254)
            {
                throw new ArgumentException("The count of URL path segments (strings between '/' characters) as part of the blob name cannot exceed 254.");
            }
        }

        //
        // Summary:
        //     Checks if a file name is valid.
        //
        // Parameters:
        //   fileName:
        //     A string representing the file name to validate.
        public static void ValidateFileName(string fileName)
        {
            ValidateFileDirectoryHelper(fileName, "file");
            if (fileName.EndsWith("/", StringComparison.Ordinal))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid {0} name. Check MSDN for more information about valid {0} naming.", new object[1] { "file" }));
            }

            string[] reservedFileNames = ReservedFileNames;
            foreach (string text in reservedFileNames)
            {
                if (text.Equals(fileName, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid {0} name. This {0} name is reserved.", new object[1] { "file" }));
                }
            }
        }

        //
        // Summary:
        //     Checks if a directory name is valid.
        //
        // Parameters:
        //   directoryName:
        //     A string representing the directory name to validate.
        public static void ValidateDirectoryName(string directoryName)
        {
            ValidateFileDirectoryHelper(directoryName, "directory");
        }

        private static void ValidateFileDirectoryHelper(string resourceName, string resourceType)
        {
            if (string.IsNullOrWhiteSpace(resourceName))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid {0} name. The {0} name may not be null, empty, or whitespace only.", new object[1] { resourceType }));
            }

            if (resourceName.Length < 1 || resourceName.Length > 255)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid {0} name length. The {0} name must be between {1} and {2} characters long.", new object[3] { resourceType, 1, 255 }));
            }

            if (!FileDirectoryRegex.IsMatch(resourceName))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid {0} name. Check MSDN for more information about valid {0} naming.", new object[1] { resourceType }));
            }
        }

        //
        // Summary:
        //     Checks if a table name is valid.
        //
        // Parameters:
        //   tableName:
        //     A string representing the table name to validate.
        public static void ValidateTableName(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid {0} name. The {0} name may not be null, empty, or whitespace only.", new object[1] { "table" }));
            }

            if (tableName.Length < 3 || tableName.Length > 63)
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid {0} name length. The {0} name must be between {1} and {2} characters long.", new object[3] { "table", 3, 63 }));
            }

            if (!TableRegex.IsMatch(tableName) && !MetricsTableRegex.IsMatch(tableName) && !tableName.Equals("$MetricsCapacityBlob", StringComparison.OrdinalIgnoreCase))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "Invalid {0} name. Check MSDN for more information about valid {0} naming.", new object[1] { "table" }));
            }
        }
    }
}