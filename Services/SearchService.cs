using Lemoo_pos.Helper;
using Lemoo_pos.Models.Dto;
using Lemoo_pos.Services.Interfaces;
using Nest;

namespace Lemoo_pos.Services
{
    public class SearchService : ISearchService
    {
        private readonly IElasticClient _elasticClient;

        public SearchService(IElasticClient elasticClient)
        {
            _elasticClient = elasticClient;
        }

        public List<ProductResponseDto> SearchProduct(long storeId, long branchId, string query)
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

            var termQueryStore = new TermQuery
            {
                Field = "storeId",
                Value = storeId
            };

            var termQueryBranch = new TermsQuery
            {

                Field = "branches",
                Terms = [branchId]
            };

            var boolQuery = new BoolQuery
            {
                Must = new List<QueryContainer>
                {
                    disMaxQuery
                },
                Filter = new List<QueryContainer>
                {
                    termQueryStore,
                    termQueryBranch
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

            foreach (var productSearchResponse in productSearchResponses)
            {
                productResults.Add(new()
                {
                    Id = productSearchResponse.VariantId,
                    Name = productSearchResponse.Name,
                    Price = productSearchResponse.Price,
                    Quantity = productSearchResponse.Quantity,
                    VariantName = productSearchResponse.VariantName,
                    Image = productSearchResponse.Image,
                    BranchId = branchId,
                    AllowNegativeInventory = productSearchResponse.AllowNegativeInventory,
                });
            }

            return productResults;
        }

        public List<CustomerSearchResponseDto> SearchCustomer(long storeId, string query)
        {
            query = LanguageHelper.RemoveVietnameseTones(query.ToLower());

            var multiMatchQuery = new MultiMatchQuery
            {
                Query = query,
                Type = TextQueryType.MostFields,
                Fields = "keyword,keyword._3gram,name,phone,email"
            };

            var fuzzyQuery = new FuzzyQuery
            {
                Field = "keyword",
                Value = query,
                Fuzziness = Fuzziness.EditDistance(2)
            };


            var disMaxQuery = new DisMaxQuery
            {
                Queries =
            [
                multiMatchQuery,
                fuzzyQuery
            ],
                Boost = 1.2,
                TieBreaker = 1.0
            };

            var termQueryStore = new TermQuery
            {
                Field = "storeId",
                Value = storeId
            };


            var boolQuery = new BoolQuery
            {
                Must = [disMaxQuery],
                Filter = [termQueryStore]
            };

            var searchRequest = new SearchRequest<CustomerSearchDto>("customers")
            {
                Query = boolQuery,
                Explain = false,
            };

            var searchResponse = _elasticClient.Search<CustomerSearchDto>(searchRequest);

            var customerSearches = searchResponse.Hits.Select(hit => hit.Source).ToList();

            List<CustomerSearchResponseDto> customerSearchResponses = [];

            foreach (var customerSearch in customerSearches)
            {
                customerSearchResponses.Add(new()
                {
                    Id = customerSearch.Id,
                    Name = customerSearch.Name,
                    Phone = customerSearch.Phone,
                    Email = customerSearch.Email
                });
            }

            return customerSearchResponses;
        }
    }
}
