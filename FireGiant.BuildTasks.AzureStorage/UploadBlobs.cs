//---------------------------------------------------------------------------
// <copyright file="UploadBlobs.cs" company="FireGiant">
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
    /// Task to upload blobs to Azure storage account.
    /// </summary>
    public class UploadBlobs : BaseStorageTask
    {
        /// <summary>
        /// Blobs to upload.
        /// </summary>
        [Required]
        public ITaskItem[] Blobs { get; set; }

        /// <summary>
        /// Container to upload blobs to.
        /// </summary>
        [Required]
        public string Container { get; set; }

        /// <summary>
        /// Indicates whether the container should be marked public.
        /// </summary>
        public bool Public { get; set; }

        /// <summary>
        /// Blobs uploaded to storage account.
        /// </summary>
        [Output]
        public ITaskItem[] Uploaded { get; set; }

        /// <summary>
        /// Executes the uploaded of the provided blobs.
        /// </summary>
        /// <returns>True if the blobs were uploaded.</returns>
        public override bool Execute()
        {
            CloudStorageAccount account;
            if (!base.TryConnect(out account))
            {
                return false;
            }

            CloudBlobClient client = account.CreateCloudBlobClient();

            CloudBlobContainer container = client.GetContainerReference(this.Container.ToLowerInvariant());
            container.CreateIfNotExists();

            if (this.Public)
            {
                container.SetPermissions(new BlobContainerPermissions() { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            List<UploadBlobTask> uploads = new List<UploadBlobTask>();
            foreach (ITaskItem blobItem in this.Blobs)
            {
                string blobName = blobItem.GetMetadata("BlobName");
                if (String.IsNullOrEmpty(blobName))
                {
                    blobName = Path.GetFileName(blobItem.ItemSpec);
                }

                blobName = blobName.ToLowerInvariant().Replace('\\', '/');

                UploadBlobTask upload = new UploadBlobTask();
                upload.Blob = container.GetBlockBlobReference(blobName);
                upload.Task = upload.Blob.UploadFromFileAsync(blobItem.ItemSpec, FileMode.Open);

                uploads.Add(upload);
            }

            Tasks.Task.WaitAll(uploads.Select(u => u.Task).ToArray());

            this.Uploaded = uploads.Select(u => this.CreateTaskItemFromBlob(u.Blob)).ToArray();
            return true;
        }

        private TaskItem CreateTaskItemFromBlob(CloudBlockBlob blob)
        {
            TaskItem taskItem = new TaskItem(blob.Uri.AbsoluteUri);
            taskItem.SetMetadata("Container", blob.Container.Name);

            return taskItem;
        }

        private class UploadBlobTask
        {
            public CloudBlockBlob Blob { get; set; }

            public Tasks.Task Task { get; set; }
        }
    }
}
