namespace Lemoo_pos.Services.Interfaces
{
    public interface IElasticsearchService
    {
        void AddDocumentWithId<T>(T document, string id, string index) where T : class;

        void DeleteDocumentById(string indexName, string id);

        void EnsureIndexExists(string indexName);
    }
}
