using Lemoo_pos.Services.Interfaces;
using Nest;

namespace Lemoo_pos.Services
{
    public class ElasticsearchService : IElasticsearchService
    {

        private readonly IElasticClient _client;

        public ElasticsearchService(IElasticClient client) {
            _client = client;
        }

        public void AddDocumentWithId<T>(T document, string id, string index) where T : class
        {
            EnsureIndexExists(index);

            var response = _client.Index(document, i => i
                .Index(index)
                .Id(id)
            );

            if (response.IsValid)
            {
                Console.WriteLine($"Document with ID {id} indexed successfully!");
            }
            else
            {
                Console.WriteLine($"Failed to index document: {response.OriginalException.Message}");
            }
        }


        public void DeleteDocumentById(string indexName, string id)
        {
            var response = _client.Delete(new DeleteRequest(indexName, id));

            if (response.IsValid)
            {
                Console.WriteLine($"Document with ID {id} deleted successfully!");
            }
            else
            {
                Console.WriteLine($"Failed to delete document with ID {id}: {response.OriginalException.Message}");
            }
        }

        public void EnsureIndexExists(string indexName)
        {
            var existsResponse = _client.Indices.Exists(indexName);

            if (!existsResponse.Exists)
            {
                var createIndexResponse = _client.Indices.Create(indexName, c => c
                    .Map(m => m.AutoMap())
                );

                if (createIndexResponse.IsValid)
                {
                    Console.WriteLine($"Index '{indexName}' created successfully.");
                }
                else
                {
                    throw new Exception($"Failed to create index: {createIndexResponse.OriginalException.Message}");
                }
            }
        }
    }
}
