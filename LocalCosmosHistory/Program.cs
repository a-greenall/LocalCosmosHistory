// See https://aka.ms/new-console-template for more information

using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LocalCosmosHistory;
using Microsoft.Azure.Cosmos;

// Client options
CosmosClientOptions options = new()
{
    AllowBulkExecution = true,
    RequestTimeout = TimeSpan.FromMinutes(2),
    OpenTcpConnectionTimeout = TimeSpan.FromMinutes(2),
    MaxRetryAttemptsOnRateLimitedRequests = 50 
};

// Client
const string connectionString = ""; // Replace with your local Cosmos DB emulator connection string
CosmosClient client = new(connectionString, options);

// Create database and container
var container = await DatabaseCreator.CreateAssetBoxHistoryDatabaseAndContainer(client);

// Create the history items
var histories = AssetBoxHistoryGenerator.GetAssetBoxHistoryItems(10_000, DateTime.Today);

var successCount = 0;
var failureCount = 0;

async Task CreateHistoryDocument(AssetBoxHistoryDto assetBoxHistoryDto)
{
    try
    {
        await container.CreateItemAsync(assetBoxHistoryDto, new PartitionKey(assetBoxHistoryDto.PartitionKey));
        Interlocked.Increment(ref successCount);
    }
    catch (CosmosException)
    {
        Interlocked.Increment(ref failureCount);
    }
}

Console.WriteLine($"Inserting {histories.Count} records");
var stopwatch = Stopwatch.StartNew();

await Task.WhenAll(histories.Select(CreateHistoryDocument));

stopwatch.Stop();
Console.WriteLine($"Elapsed: {stopwatch.Elapsed.TotalSeconds} seconds with {successCount} successes, and {failureCount} failures");