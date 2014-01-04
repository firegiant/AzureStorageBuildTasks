//---------------------------------------------------------------------------
// <copyright file="GetBlobsFixture.cs" company="FireGiant">
//   Copyright (c) 2014, FireGiant.
//   This software is released under BSD License.
//   The license and further copyright text can be found in the file
//   LICENSE.txt at the root directory of the distribution.
// </copyright>
//---------------------------------------------------------------------------

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
            AssertExtensions.EndsWith("test/testblob.data", task.Blobs[0].ItemSpec);
        }
    }
}
