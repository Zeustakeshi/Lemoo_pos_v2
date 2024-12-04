namespace Lemoo_pos.Services.Interfaces
{
    public interface IElasticsearchService
    {

        void DeleteDocumentById(string indexName, string id);

        void EnsureIndexExists(string indexName);

        Task SaveDocumentById<T>(T document, string id, string index) where T : class;
    }
}
