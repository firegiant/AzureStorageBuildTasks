//---------------------------------------------------------------------------
// <copyright file="BaseStorageTask.cs" company="FireGiant">
//   Copyright (c) 2014, FireGiant.
//   This software is released under BSD License.
//   The license and further copyright text can be found in the file
//   LICENSE.txt at the root directory of the distribution.
// </copyright>
//---------------------------------------------------------------------------

using System;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;

namespace FireGiant.BuildTasks.AzureStorage
{
    /// <summary>
    /// Base class for all storage tasks.
    /// </summary>
    public abstract class BaseStorageTask : Task
    {
        /// <summary>
        /// Name of the Azure storage account to connect to.
        /// </summary>
        [Required]
        public string StorageAccount { get; set; }

        /// <summary>
        /// Authentication key for the Azure storage account.
        /// </summary>
        public string StorageAuthentication { get; set; }

        /// <summary>
        /// Indicates whether the task should connect to the Azure storage emulator.
        /// </summary>
        /// <remarks>If true then the storage account and authentication key values are ignored.</remarks>
        public bool StorageEmulator { get; set; }

        /// <summary>
        /// Attempts to connect to the Azure storage account.
        /// </summary>
        /// <param name="account">Azure storage account.</param>
        /// <returns>True if the Azure storage account could be determined otherwise false.</returns>
        protected bool TryConnect(out CloudStorageAccount account)
        {
            if (this.StorageEmulator)
            {
                return CloudStorageAccount.TryParse("UseDevelopmentStorage=true", out account);
            }

            if (String.IsNullOrEmpty(this.StorageAuthentication))
            {
                // TODO: Try to read authentication key from Windows credential manager.

                // TODO: bring back this log message when it can be tested correctly.
                //this.Log.LogError("Cannot locate authentication key in Windows credential manager for account: {0}", this.AccountString);

                account = null;
                return false;
            }

            StorageCredentials cred = new StorageCredentials(this.StorageAccount, this.StorageAuthentication);

            account = new CloudStorageAccount(cred, false);
            return true;
        }
    }
}
