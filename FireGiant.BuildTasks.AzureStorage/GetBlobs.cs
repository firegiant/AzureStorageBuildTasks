//---------------------------------------------------------------------------
// <copyright file="GetBlobs.cs" company="FireGiant">
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

namespace FireGiant.BuildTasks.AzureStorage
{
    /// <summary>
    /// Task to get the blobs in an Azure storage account.
    /// </summary>
    public class GetBlobs : BaseStorageTask
    {
        /// <summary>
        /// Optional set of containers to narrow the query for blobs.
        /// </summary>
        public ITaskItem[] Containers { get; set; }

        /// <summary>
        /// Blobs found in the Azure storage account.
        /// </summary>
        [Output]
        public ITaskItem[] Blobs { get; private set; }

        /// <summary>
        /// Executes the query for all the blobs in the Azure storage account.
        /// </summary>
        /// <returns>True if the blobs were queried.</returns>
        public override bool Execute()
        {
            CloudStorageAccount account;
            if (!base.TryConnect(out account))
            {
                return false;
            }

            CloudBlobClient client = account.CreateCloudBlobClient();

            var blobs = this.GetBlobsFilteredByContainers(client);
            this.Blobs = blobs.Select(b => CreateTaskItemFromBlobItem(b)).ToArray();

            return true;
        }

        private IEnumerable<IListBlobItem> GetBlobsFilteredByContainers(CloudBlobClient client)
        {
            var containerNames = this.Containers == null ? null :
                new HashSet<string>(this.Containers.Select(c => c.ItemSpec.ToLowerInvariant())
                                                   .Where(n => !String.IsNullOrEmpty(n)));

            return client.ListContainers()
                         .Where(c => containerNames == null || containerNames.Contains(c.Name))
                         .SelectMany(c => c.ListBlobs(null, true));
        }

        private ITaskItem CreateTaskItemFromBlobItem(IListBlobItem b)
        {
            ITaskItem taskItem = new BlobTaskItem(b.Uri, b.Container.Name);
            return taskItem;
        }
    }
}
