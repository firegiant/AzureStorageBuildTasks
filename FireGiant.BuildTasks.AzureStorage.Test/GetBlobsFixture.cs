//---------------------------------------------------------------------------
// <copyright file="GetBlobsFixture.cs" company="FireGiant">
//   Copyright (c) 2014, FireGiant.
//   This software is released under BSD License.
//   The license and further copyright text can be found in the file
//   LICENSE.txt at the root directory of the distribution.
// </copyright>
//---------------------------------------------------------------------------

using System;
using Xunit;

namespace FireGiant.BuildTasks.AzureStorage.Test
{
    public class GetBlobsFixture
    {
        [Fact]
        public void CanListBlobs()
        {
            TestHelpers.EnsureOneContainerAsManyBlobs("test", "testblob.data");

            var task = new GetBlobs() { StorageEmulator = true };
            var result = task.Execute();

            Assert.True(result);
            Assert.Single(task.Blobs);

            var blob = task.Blobs[0];
            AssertExtensions.EndsWith("test/testblob.data", blob.ItemSpec);
            Assert.Equal(".data", blob.GetMetadata("Extension"));
            Assert.Equal("testblob", blob.GetMetadata("Filename"));
            Assert.Equal("/devstoreaccount1/test", blob.GetMetadata("Directory"));
            Assert.Equal(String.Empty, blob.GetMetadata("RelativeDir"));
            Assert.Equal("test", blob.GetMetadata("Container"));
        }
    }
}
