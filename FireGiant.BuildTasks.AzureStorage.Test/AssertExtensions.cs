//---------------------------------------------------------------------------
// <copyright file="AssertExtensions.cs" company="FireGiant">
//   Copyright (c) 2014, FireGiant.
//   This software is released under BSD License.
//   The license and further copyright text can be found in the file
//   LICENSE.txt at the root directory of the distribution.
// </copyright>
//---------------------------------------------------------------------------

using Xunit;

namespace FireGiant.BuildTasks.AzureStorage.Test
{
    public static class AssertExtensions
    {
        public static void EndsWith(string expected, string actual)
        {
            Assert.True(expected.Length <= actual.Length);

            string actualActual = actual.Substring(actual.Length - expected.Length);
            Assert.Equal(expected, actualActual);
        }
    }
}
