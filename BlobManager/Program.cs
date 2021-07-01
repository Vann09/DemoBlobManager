using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System;
using System.Threading.Tasks;

public class Program 
{
    private const string blobServiceEndpoint = <Url-blob-service>;
    private const string storageAccountName = <nombre-cuenta>;
    private const string storageAccountKey = <key-cuenta>;

    public static async Task Main (string[] args)
    {
        StorageSharedKeyCredential accountCredentials = new StorageSharedKeyCredential ( storageAccountName, storageAccountKey);
       
        BlobServiceClient serviceClient = new BlobServiceClient (new Uri(blobServiceEndpoint), accountCredentials);
       
        AccountInfo info = await serviceClient.GetAccountInfoAsync();
       
        await Console.Out.WriteLineAsync($"Conectado a la cuenta de almacenamiento de Azure");
        await Console.Out.WriteLineAsync($"Nombre de la cuenta:\t{storageAccountName}");
        await Console.Out.WriteLineAsync($"Rendimiento de la cuenta:\t{info?.AccountKind}");
        await Console.Out.WriteLineAsync($"Redundancia de la cuenta:\t{info?.SkuName}");

        await EnumerateContainerAsync(serviceClient);

        string existingContainerName = "raster-graphics";
        await EnumerateBlobAsync(serviceClient, existingContainerName);

        string newContainerName = "vector-graphics";
        BlobContainerClient containerClient = await GetBlobContainerAsync(serviceClient, newContainerName);

        string uploadedBlobName = "graph.svg";
        BlobClient blobClient = await GetBlobAsync(containerClient, uploadedBlobName);
        await Console.Out.WriteLineAsync($"Blob Url:\t{blobClient.Uri}");
    
    }
    private static async Task EnumerateContainerAsync(BlobServiceClient client)
    {
        await foreach (BlobContainerItem container in client.GetBlobContainersAsync())
        {
            await Console.Out.WriteLineAsync($"Contenedor:\t{container.Name}");
        }
    }
    private static async Task EnumerateBlobAsync(BlobServiceClient client, string containerName)
    {
        BlobContainerClient container = client.GetBlobContainerClient(containerName);
        await Console.Out.WriteLineAsync ($"Buscando:\t{container.Name}");
        await foreach (BlobItem blob in container.GetBlobsAsync())
        {
            await Console.Out.WriteLineAsync($"Blob existente:\t{blob.Name}");
        }
    }

    private static async Task<BlobContainerClient> GetBlobContainerAsync(BlobServiceClient client, string containerName)
    {
        BlobContainerClient container = client.GetBlobContainerClient(containerName);
        await container.CreateIfNotExistsAsync(PublicAccessType.Blob);
        await Console.Out.WriteLineAsync($"Nuevo contenedor:\t{container.Name}");
        return container;
    }

    private static async Task<BlobClient> GetBlobAsync (BlobContainerClient client, string blobName)
    {
        BlobClient blob = client.GetBlobClient(blobName);
        await Console.Out.WriteLineAsync($"Blob encontrado:\t{blob.Name}");
        return blob;
    }
}
