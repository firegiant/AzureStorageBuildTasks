//---------------------------------------------------------------------------
// <copyright file="GetContainersFixture.cs" company="FireGiant">
//   Copyright (c) 2014, FireGiant.
//   This software is released under BSD License.
//   The license and further copyright text can be found in the file
//   LICENSE.txt at the root directory of the distribution.
// </copyright>
//---------------------------------------------------------------------------

using Xunit;

namespace FireGiant.BuildTasks.AzureStorage.Test
{
    public class GetContainersFixture
    {
        [Fact]
        public void InvalidAccount()
        {
            var task = new GetContainers() { StorageAccount = "invalid" };
            var result = task.Execute();

            Assert.False(result);
            Assert.Null(task.Containers);
        }

        [Fact]
        public void CanListContainers()
        {
            TestHelpers.EnsureOneContainerAsManyBlobs("test");

            var task = new GetContainers() { StorageEmulator = true };
            var result = task.Execute();

            Assert.True(result);
            Assert.Single(task.Containers);
            Assert.Equal("test", task.Containers[0].ItemSpec);
        }
    }
}
