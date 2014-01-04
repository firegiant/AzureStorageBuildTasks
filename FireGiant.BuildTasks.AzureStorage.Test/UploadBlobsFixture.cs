//---------------------------------------------------------------------------
// <copyright file="UploadBlobsFixture.cs" company="FireGiant">
//   Copyright (c) 2014, FireGiant.
//   This software is released under BSD License.
//   The license and further copyright text can be found in the file
//   LICENSE.txt at the root directory of the distribution.
// </copyright>
//---------------------------------------------------------------------------

using System.IO;
using Microsoft.Build.Utilities;
using Xunit;

namespace FireGiant.BuildTasks.AzureStorage.Test
{
    public class UploadBlobsFixture
    {
        private const string TestContainerName = "test-upload";

        [Fact]
        public void CanUploadSingleFileBlob()
        {
            TestHelpers.EnsureNoContainers();

            var root = TestHelpers.RootPath();
            var blob = new TaskItem(Path.Combine(root, @"TestData\upload.txt"));

            var task = new UploadBlobs() { Blobs = new TaskItem[] { blob }, Container = TestContainerName, StorageEmulator = true };
            var result = task.Execute();

            Assert.True(result);
            Assert.Equal(1, task.Uploaded.Length);
            AssertExtensions.EndsWith("/test-upload/upload.txt", task.Uploaded[0].ItemSpec);
        }

        [Fact]
        public void CanUploadSingleNestedFileBlob()
        {
            TestHelpers.EnsureNoContainers();

            var root = TestHelpers.RootPath();
            var blob = new TaskItem(Path.Combine(root, @"TestData\upload.txt"));
            blob.SetMetadata("BlobName", @"very\nested\renamed_upload.txt");

            var task = new UploadBlobs() { Blobs = new TaskItem[] { blob }, Container = TestContainerName, StorageEmulator = true };
            var result = task.Execute();

            Assert.True(result);
            Assert.Equal(1, task.Uploaded.Length);
            AssertExtensions.EndsWith("/test-upload/very/nested/renamed_upload.txt", task.Uploaded[0].ItemSpec);
        }

        [Fact]
        public void CanUploadMultipleFileBlobs()
        {
            TestHelpers.EnsureNoContainers();

            var root = TestHelpers.RootPath();
            var blobs = new[] {
                new TaskItem(Path.Combine(root, @"TestData\upload.txt")),
                new TaskItem(Path.Combine(root, @"TestData\upload2.txt")),
            };

            var task = new UploadBlobs() { Blobs = blobs , Container = TestContainerName, Public = true, StorageEmulator= true };
            var result = task.Execute();

            Assert.True(result);
            Assert.Equal(2, task.Uploaded.Length);
            AssertExtensions.EndsWith("/test-upload/upload.txt", task.Uploaded[0].ItemSpec);
            AssertExtensions.EndsWith("/test-upload/upload2.txt", task.Uploaded[1].ItemSpec);
        }
    }
}
