using System.Collections.ObjectModel;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;

namespace LocalCosmosHistory
{
    public static class DatabaseCreator
    {
        public static async Task<Container> CreateAssetBoxHistoryDatabaseAndContainer(CosmosClient client, int? throughput = 10_000, bool recreateContainer = false)
        {
            const string dbName = "history";
            const string containerName = "asset-box-history";
            
            var databaseResponse = await client.CreateDatabaseIfNotExistsAsync(dbName);
            var containerReference = databaseResponse.Database.GetContainer(containerName);

            if (recreateContainer)
            {
                try
                {
                    var containerResponse = await containerReference.ReadContainerAsync();
                    await containerResponse.Container.DeleteContainerAsync();
                }
                catch (CosmosException e) when (e.StatusCode == HttpStatusCode.NotFound)
                {
                    // Ignore - container doesn't exist so will be created below
                }
            }
         
            var containerProperties = new ContainerProperties
            {
                Id = containerName,
                PartitionKeyPath = "/PartitionKey",
                IndexingPolicy = new IndexingPolicy
                {
                    Automatic = true,
                    IndexingMode = IndexingMode.Consistent,
                    IncludedPaths =
                    {
                        new IncludedPath
                        {
                            Path = "/PartitionKey/?"
                        },
                        new IncludedPath
                        {
                            Path = "/AssetBoxId/?"
                        },
                        new IncludedPath
                        {
                            Path = "/AssetBoxGlobalId/?"
                        },
                        new IncludedPath
                        {
                            Path = "/WrapperGlobalId/?"
                        },
                        new IncludedPath
                        {
                            Path = "/AllAssetPricesConfirmed/?"
                        },
                        new IncludedPath
                        {
                            Path = "/HistoryFor/?"
                        },
                        new IncludedPath
                        {
                            Path = "/Source/?"
                        },
                    },
                    CompositeIndexes =
                    {
                        new Collection<CompositePath>
                        {
                            new()
                            {
                                Order = CompositePathSortOrder.Ascending,
                                Path = "/AssetBoxId"
                            },
                            new()
                            {
                                Order = CompositePathSortOrder.Ascending,
                                Path = "/HistoryFor"
                            }
                        }
                    },
                    ExcludedPaths =
                    {
                        new ExcludedPath
                        {
                            Path = "/*"
                        },
                        new ExcludedPath
                        {
                            Path = "/\"_etag\"/?"
                        }
                    }
                }
            };

            await databaseResponse.Database.CreateContainerIfNotExistsAsync(
                containerProperties, ThroughputProperties.CreateManualThroughput(throughput.Value));

            return client.GetContainer(dbName, containerName);
        }
    }
}