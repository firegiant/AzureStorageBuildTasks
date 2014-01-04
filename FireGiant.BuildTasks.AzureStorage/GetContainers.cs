//---------------------------------------------------------------------------
// <copyright file="GetContainers.cs" company="FireGiant">
//   Copyright (c) 2014, FireGiant.
//   This software is released under BSD License.
//   The license and further copyright text can be found in the file
//   LICENSE.txt at the root directory of the distribution.
// </copyright>
//---------------------------------------------------------------------------

using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FireGiant.BuildTasks.AzureStorage
{
    /// <summary>
    /// Task to get the containers in an Azure storage account.
    /// </summary>
    public class GetContainers : BaseStorageTask
    {
        /// <summary>
        /// Containers found in the Azure storage account.
        /// </summary>
        [Output]
        public ITaskItem[] Containers { get; private set; }

        /// <summary>
        /// Executes the query for all the containers in the Azure storage account.
        /// </summary>
        /// <returns>True if the containers were queried</returns>
        public override bool Execute()
        {
            CloudStorageAccount account;
            if (!base.TryConnect(out account))
            {
                return false;
            }

            CloudBlobClient client = account.CreateCloudBlobClient();
            this.Containers = client.ListContainers().Select(c => this.CreateTaskItemFromBlobContainer(c)).ToArray();

            return true;
        }

        private TaskItem CreateTaskItemFromBlobContainer(CloudBlobContainer container)
        {
            TaskItem taskItem = new TaskItem(container.Name);
            return taskItem;
        }
    }
}
