//---------------------------------------------------------------------------
// <copyright file="DownloadBlobsFixture.cs" company="FireGiant">
//   Copyright (c) 2014, FireGiant.
//   This software is released under BSD License.
//   The license and further copyright text can be found in the file
//   LICENSE.txt at the root directory of the distribution.
// </copyright>
//---------------------------------------------------------------------------

using System.IO;
using Xunit;

namespace FireGiant.BuildTasks.AzureStorage.Test
{
    public class DownloadBlobsFixture
    {
        private const string TestContainerName = "test-download";

        [Fact]
        public void CanDownloadSingleFileBlob()
        {
            TestHelpers.EnsureOneContainerAsManyBlobs(DownloadBlobsFixture.TestContainerName, "test.txt");

            var root = TestHelpers.RootPath("test_results");

            if (Directory.Exists(root))
            {
                Directory.Delete(root, true);
            }

            Assert.False(File.Exists(Path.Combine(root, @"test.txt")));

            var task = new DownloadBlobs() { Container = DownloadBlobsFixture.TestContainerName, Folder = root, StorageEmulator = true };
            var result = task.Execute();

            Assert.True(result);
            Assert.Single(task.Downloaded);

            var downloaded = task.Downloaded[0];
            Assert.Equal(Path.Combine(root, @"test.txt"), downloaded.ItemSpec);
            Assert.Equal(".txt", downloaded.GetMetadata("Extension"));
            Assert.Equal("test", downloaded.GetMetadata("Filename"));
            Assert.Equal("test.txt", downloaded.GetMetadata("Blob"));
            Assert.Equal(DownloadBlobsFixture.TestContainerName, downloaded.GetMetadata("Container"));
            Assert.True(File.Exists(Path.Combine(root, @"test.txt")));

            Directory.Delete(root, true);
        }

        [Fact]
        public void CanDownloadSingleNestedFileBlob()
        {
            TestHelpers.EnsureOneContainerAsManyBlobs(DownloadBlobsFixture.TestContainerName, "test/download.txt");

            var root = TestHelpers.RootPath("test_results");

            if (Directory.Exists(root))
            {
                Directory.Delete(root, true);
            }

            Assert.False(File.Exists(Path.Combine(root, @"test\download.txt")));

            var task = new DownloadBlobs() { Container = DownloadBlobsFixture.TestContainerName, Folder = root, StorageEmulator = true };
            var result = task.Execute();

            Assert.True(result);
            Assert.Single(task.Downloaded);

            var downloaded = task.Downloaded[0];
            Assert.Equal(Path.Combine(root, @"test\download.txt"), downloaded.ItemSpec);
            Assert.Equal(".txt", downloaded.GetMetadata("Extension"));
            Assert.Equal("download", downloaded.GetMetadata("Filename"));
            Assert.Equal("test/download.txt", downloaded.GetMetadata("Blob"));
            Assert.Equal(DownloadBlobsFixture.TestContainerName, downloaded.GetMetadata("Container"));
            Assert.True(File.Exists(Path.Combine(root, @"test\download.txt")));

            Directory.Delete(root, true);
        }

        [Fact]
        public void CanDownloadMultipleFileBlobs()
        {
            TestHelpers.EnsureOneContainerAsManyBlobs(DownloadBlobsFixture.TestContainerName, "foo/download1.txt", "download2.txt");

            var root = TestHelpers.RootPath("test_results");

            if (Directory.Exists(root))
            {
                Directory.Delete(root, true);
            }

            var task = new DownloadBlobs() { Container = TestContainerName, Folder = root, StorageEmulator = true };
            var result = task.Execute();

            Assert.True(result);
            Assert.Equal(2, task.Downloaded.Length);

            foreach (var downloaded in task.Downloaded)
            {
                Assert.True(File.Exists(downloaded.ItemSpec));
            }
        }
    }
}
