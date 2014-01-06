//---------------------------------------------------------------------------
// <copyright file="UploadBlobsFixture.cs" company="FireGiant">
//   Copyright (c) 2014, FireGiant.
//   This software is released under BSD License.
//   The license and further copyright text can be found in the file
//   LICENSE.txt at the root directory of the distribution.
// </copyright>
//---------------------------------------------------------------------------

using System;
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
            Assert.Single(task.Uploaded);

            var uploaded = task.Uploaded[0];
            AssertExtensions.EndsWith("/test-upload/upload.txt", uploaded.ItemSpec);
            Assert.Equal(".txt", uploaded.GetMetadata("Extension"));
            Assert.Equal("upload", uploaded.GetMetadata("Filename"));
            Assert.Equal("/devstoreaccount1/test-upload", uploaded.GetMetadata("Directory"));
            Assert.Equal(String.Empty, uploaded.GetMetadata("RelativeDir"));
            Assert.Equal(UploadBlobsFixture.TestContainerName, uploaded.GetMetadata("Container"));
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
            Assert.Single(task.Uploaded);

            var uploaded = task.Uploaded[0];
            AssertExtensions.EndsWith("/test-upload/very/nested/renamed_upload.txt", uploaded.ItemSpec);
            Assert.Equal(".txt", uploaded.GetMetadata("Extension"));
            Assert.Equal("renamed_upload", uploaded.GetMetadata("Filename"));
            Assert.Equal("/devstoreaccount1/test-upload/very/nested", uploaded.GetMetadata("Directory"));
            Assert.Equal("very/nested", uploaded.GetMetadata("RelativeDir"));
            Assert.Equal(UploadBlobsFixture.TestContainerName, uploaded.GetMetadata("Container"));
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

            var uploaded = task.Uploaded[0];
            AssertExtensions.EndsWith("/test-upload/upload.txt", uploaded.ItemSpec);
            Assert.Equal(".txt", uploaded.GetMetadata("Extension"));
            Assert.Equal("upload", uploaded.GetMetadata("Filename"));
            Assert.Equal("/devstoreaccount1/test-upload", uploaded.GetMetadata("Directory"));
            Assert.Equal(String.Empty, uploaded.GetMetadata("RelativeDir"));
            Assert.Equal(UploadBlobsFixture.TestContainerName, uploaded.GetMetadata("Container"));

            uploaded = task.Uploaded[1];
            AssertExtensions.EndsWith("/test-upload/upload2.txt", uploaded.ItemSpec);
            Assert.Equal(".txt", uploaded.GetMetadata("Extension"));
            Assert.Equal("upload2", uploaded.GetMetadata("Filename"));
            Assert.Equal("/devstoreaccount1/test-upload", uploaded.GetMetadata("Directory"));
            Assert.Equal(String.Empty, uploaded.GetMetadata("RelativeDir"));
            Assert.Equal(UploadBlobsFixture.TestContainerName, uploaded.GetMetadata("Container"));
        }
    }
}
