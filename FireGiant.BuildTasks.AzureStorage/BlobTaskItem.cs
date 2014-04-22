//---------------------------------------------------------------------------
// <copyright file="BlobTaskItem.cs" company="FireGiant">
//   Copyright (c) 2014, FireGiant.
//   This software is released under BSD License.
//   The license and further copyright text can be found in the file
//   LICENSE.txt at the root directory of the distribution.
// </copyright>
//---------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Microsoft.Build.Framework;

namespace FireGiant.BuildTasks.AzureStorage
{
    /// <summary>
    /// Custom task item that provides the appropriate metadata for Azure storage blobs.
    /// </summary>
    public class BlobTaskItem : MarshalByRefObject, ITaskItem
    {
        private Dictionary<string, string> metadata = new Dictionary<string, string>();

        /// <summary>
        /// Creates a new blob task item.
        /// </summary>
        /// <param name="uri">Full URL to the blob in the Azure storage account.</param>
        /// <param name="container">Container the blob is a member of.</param>
        public BlobTaskItem(Uri uri, string container)
        {
            var absolutePath = uri.AbsolutePath;
            var directory = Path.GetDirectoryName(absolutePath).Replace('\\', '/');
            var relativeTo = directory.StartsWith("/devstoreaccount1/") ? "/devstoreaccount1/" + container : "/" + container;

            this.ItemSpec = uri.AbsoluteUri;
            this.SetMetadata("Directory", directory);
            this.SetMetadata("RelativeDir", directory.Substring(relativeTo.Length).TrimStart('/'));
            this.SetMetadata("Extension", Path.GetExtension(absolutePath));
            this.SetMetadata("Filename", Path.GetFileNameWithoutExtension(absolutePath));
            this.SetMetadata("RootDir", uri.GetLeftPart(UriPartial.Authority));
            this.SetMetadata("Container", container);
        }

        public IDictionary CloneCustomMetadata()
        {
            return new Dictionary<string, string>(this.metadata);
        }

        public void CopyMetadataTo(ITaskItem destinationItem)
        {
            foreach (var data in this.metadata)
            {
                destinationItem.SetMetadata(data.Key, data.Value);
            }
        }

        public string GetMetadata(string metadataName)
        {
            var value = String.Empty;

            this.metadata.TryGetValue(metadataName, out value);
            return value;
        }

        public string ItemSpec { get; set; }

        public int MetadataCount
        {
            get { return this.metadata.Keys.Count; }
        }

        public ICollection MetadataNames
        {
            get { return this.metadata.Keys; }
        }

        public void RemoveMetadata(string metadataName)
        {
            this.metadata.Remove(metadataName);
        }

        public void SetMetadata(string metadataName, string metadataValue)
        {
            this.metadata[metadataName] = metadataValue;
        }
    }
}
