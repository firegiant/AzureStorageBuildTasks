//---------------------------------------------------------------------------
// <copyright file="DownloadBlobs.cs" company="FireGiant">
//   Copyright (c) 2014, FireGiant.
//   This software is released under BSD License.
//   The license and further copyright text can be found in the file
//   LICENSE.txt at the root directory of the distribution.
// </copyright>
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Tasks = System.Threading.Tasks;

namespace FireGiant.BuildTasks.AzureStorage
{
    /// <summary>
    /// Task to download blobs to Azure storage account.
    /// </summary>
    public class DownloadBlobs : BaseStorageTask
    {
        /// <summary>
        /// Blobs to download. If this is empty, all blobs in the container are downloaded.
        /// </summary>
        public ITaskItem[] Blobs { get; set; }

        /// <summary>
        /// Container to download blobs from.
        /// </summary>
        [Required]
        public string Container { get; set; }

        /// <summary>
        /// Folder where blobs are downloaded by default.
        /// </summary>
        public string Folder { get; set; }

        /// <summary>
        /// Blobs downloaded to storage account.
        /// </summary>
        [Output]
        public ITaskItem[] Downloaded { get; set; }

        /// <summary>
        /// Executes the download of the provided blobs.
        /// </summary>
        /// <returns>True if the blobs were downloaded.</returns>
        public override bool Execute()
        {
            CloudStorageAccount account;
            if (!base.TryConnect(out account))
            {
                return false;
            }

            var client = account.CreateCloudBlobClient();

            var container = client.GetContainerReference(this.Container.ToLowerInvariant());

            if (!container.Exists())
            {
                // If the container does not exist, it's only okay if no blobs were specified.
                return this.Blobs == null || !this.Blobs.Any();
            }

            IEnumerable<ITaskItem> blobs;
            if (this.Blobs == null || !this.Blobs.Any())
            {
                var list = container.ListBlobs(useFlatBlobListing: true);

                blobs = list.Select(b => this.CreateBlobTaskItemFromBlobName(container, b.Uri.AbsoluteUri));
            }
            else
            {
                blobs = this.Blobs.Select(b => this.CreateBlobTaskItemFromBlobName(container, b.ItemSpec));
            }

            var downloads = new List<DownloadBlobTask>();
            foreach (var blobItem in blobs)
            {
                var blobFolder = blobItem.GetMetadata("RelativeDir");

                var blobName = String.IsNullOrEmpty(blobFolder) ? Path.GetFileName(blobItem.ItemSpec) : String.Concat(blobFolder, "/", Path.GetFileName(blobItem.ItemSpec));

                var fileName = blobItem.GetMetadata("LocalPath");

                if (String.IsNullOrEmpty(fileName))
                {
                    fileName = blobName.Replace('/', '\\');
                }

                var path = Path.Combine(this.Folder ?? String.Empty, fileName);

                Directory.CreateDirectory(Path.GetDirectoryName(path));

                var blob = container.GetBlockBlobReference(blobName);

                var download = new DownloadBlobTask();
                download.File = new TaskItem(path, new Dictionary<string, string>() { { "Blob", blob.Name }, { "BlobUrl", blob.Uri.AbsoluteUri }, { "Container", blob.Container.Name } });
                download.Task = blob.DownloadToFileAsync(path, FileMode.Create);

                downloads.Add(download);
            }

            Tasks.Task.WaitAll(downloads.Select(u => u.Task).ToArray());

            this.Downloaded = downloads.Select(u => u.File).ToArray();
            return true;
        }

        private ITaskItem CreateBlobTaskItemFromBlobName(CloudBlobContainer container, string blobUri)
        {
            var blob = container.GetBlockBlobReference(blobUri);

            var taskItem = new BlobTaskItem(blob.Uri, blob.Container.Name);
            return taskItem;
        }

        private class DownloadBlobTask
        {
            public ITaskItem File { get; set; }

            public Tasks.Task Task { get; set; }
        }
    }
}
