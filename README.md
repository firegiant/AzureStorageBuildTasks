# FireGiant.BuildTasks.AzureStorage

A set of MSBuild tasks for working with Azure storage blobs. The primary use case is to upload files to Azure storage as part of the build process. Ideal for posting the build results for distribution via Azure.

For example, to upload `foo.txt` to the `bar` container you could use the following in a project file.

    <PropertyGroup>
      <StorageAccount>put-storage-account-name-here</StorageAccount>
      <StorageAuthentication>put-storage-authentication-key-here</StorageAuthentication>

      <UploadContainer>test-uploads</UploadContainer>
    </PropertyGroup>

    <ItemGroup>
      <UploadBlob Include="foo.txt" />
    </ItemGroup>

    <Target Name="AfterBuild"
            DependsOnTargets="UploadBlobs">
      <Message Importance="high" Text="Uploaded blobs: @(UploadedBlob) to container: %(Container)" />
    </Target>

The `FireGiant.BuildTasks.AzureStorage.targets` provides the following ready-to-use targets in your own MSBuild projects.

## GetBlobs

Inputs:

`$(StorageAccount)` - name of storage account to query blobs from.

`$(StorageAuthentication)` - authorization key to access the storage account.

`$(StorageUseEmulator)` - set to true to use the local Azure storage emulator. This will override the `$(StorageAccount)`. Only useful for testing purposes.

`@(Container)` - optional Azure storage containers to get blobs from. If no containers are provided, blobs from all containers in the storage account are returned.

Outputs:

`@(Blob)` - full URL to blobs found in the storage account.

`%(Blob.Container)` - container of the found blob.

`%(Blob.Directory)` - directory (including the container name) of the found blob.

`%(Blob.Extension)` - extension (including dot) of the found blob.

`%(Blob.Filename)` - filename (without extension) of the found blob.

`%(Blob.Relative)` - directory (excluding the container name) of the found blob.

`%(Blob.RootDir)` - URL to the storage account.

## GetContainers

Inputs:

`$(StorageAccount)` - name of storage account to query containers from.

`$(StorageAuthentication)` - authorization key to access the storage account.

`$(StorageUseEmulator)` - set to true to use the local Azure storage emulator. This will override the `$(StorageAccount)`. Only useful for testing purposes.

Outputs:

`@(Container)` - full URL to container found in the storage account.

## UploadBlobs

Inputs:

`$(StorageAccount)` - name of storage account that houses the container to upload blobs to.

`$(StorageAuthentication)` - authorization key to access the storage account.

`$(StorageUseEmulator)` - set to true to use the local Azure storage emulator. This will override the `$(StorageAccount)`. Only useful for testing purposes.

`$(UploadContainer)` - container to upload blobs into.

`$(MakeUploadContainerPublic)` - marks the container as public such that all uploaded blobs are visible to everyone. By default the container state is not updated. If the container does not exist, by default it will be created private unless this is true.

`@(UploadBlob)` - local path to blob to upload. By default the blob will be uploaded into the root of the container using the same filename and extension.

`%(UploadBlob.BlobName)` - metadata on the `UploadBlob` item to override the default name of the blob.

Outputs:

`@(UploadedBlob)` - full URL to each uploaded blob.

`%(UploadedBlob.Container)` - container of the found blob.

`%(UploadedBlob.Directory)` - directory (including the container name) of the found blob.

`%(UploadedBlob.Extension)` - extension (including dot) of the found blob.

`%(UploadedBlob.Filename)` - filename (without extension) of the found blob.

`%(UploadedBlob.Relative)` - directory (excluding the container name) of the found blob.

`%(UploadedBlob.RootDir)` - URL to the storage account.
