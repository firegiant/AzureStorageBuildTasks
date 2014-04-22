//---------------------------------------------------------------------------
// <copyright file="TestHelpers.cs" company="FireGiant">
//   Copyright (c) 2014, FireGiant.
//   This software is released under BSD License.
//   The license and further copyright text can be found in the file
//   LICENSE.txt at the root directory of the distribution.
// </copyright>
//---------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;

namespace FireGiant.BuildTasks.AzureStorage.Test
{
    public static class TestHelpers
    {
        public static string RootPath(string additional = null)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase.Substring(8)), additional ?? String.Empty);
        }

        public static void EnsureNoContainers()
        {
            var account = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var client = account.CreateCloudBlobClient();

            var deletes = client.ListContainers().Select(c => c.DeleteAsync()).ToArray();
            Task.WaitAll(deletes);
        }

        public static void EnsureOneContainerAsManyBlobs(string containerName, params string[] blobNames)
        {
            var account = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            var client = account.CreateCloudBlobClient();

            var deletes = client.ListContainers().Select(c => c.DeleteAsync()).ToArray();
            Task.WaitAll(deletes);

            var container = client.GetContainerReference(containerName);
            container.Create();

            var uploads = blobNames.Select(n =>
            {
                var blob = container.GetBlockBlobReference(n);
                return blob.UploadFromByteArrayAsync(new byte[] { 1, 2, 3, 4, }, 0, 4);
            }).ToArray();

            Task.WaitAll(uploads);
        }
    }
}
