using Lemoo_pos.Helper;
using Lemoo_pos.Models.Dto;
using Lemoo_pos.Services.Interfaces;
using Nest;

namespace Lemoo_pos.Services
{
    public class SearchService : ISearchService
    {
        private readonly IElasticClient _elasticClient;

        public SearchService (IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public List<ProductResponseDto> SearchProduct(long storeId, string query)
        {
            query = LanguageHelper.RemoveVietnameseTones(query.ToLower());

            var multiMatchQuery = new MultiMatchQuery
            {
                Query = query,
                Type = TextQueryType.MostFields,
                Fields = "keyword,keyword._3gram,name,skuCode"
            };

            var fuzzyQuery = new FuzzyQuery
            {
                Field = "keyword",
                Value = query,
                Fuzziness = Fuzziness.EditDistance(2)
            };


            var disMaxQuery = new DisMaxQuery
            {
                Queries = new List<QueryContainer>
            {
                multiMatchQuery,
                fuzzyQuery
            },
                Boost = 1.2,
                TieBreaker = 1.0
            };

            var termQuery = new TermQuery
            {
                Field = "storeId",
                Value = storeId 
            };

            var boolQuery = new BoolQuery
                {
                    Must = new List<QueryContainer>
                {
                    disMaxQuery
                },
                    Filter = new List<QueryContainer>
                {
                    termQuery
                }
            };


            var searchRequest = new SearchRequest<ProductSearchDto>("products")
            {
                Query = boolQuery,
                Explain = false,
            };



            var searchResponse = _elasticClient.Search<ProductSearchDto>(searchRequest);

            var productSearchResponses = searchResponse.Hits.Select(hit => hit.Source).ToList();

            List<ProductResponseDto> productResults = [];

            foreach (var productSearchRespnose in productSearchResponses)
            {
                productResults.Add(new()
                {
                    Id = productSearchRespnose.VariantId,
                    Name = productSearchRespnose.Name,
                    Price = productSearchRespnose.Price,
                    Quantity = productSearchRespnose.Quantity,
                    VariantName = productSearchRespnose.VariantName,
                    Image = productSearchRespnose.Image,
                });
            }

            return productResults;
        }
    }
}
