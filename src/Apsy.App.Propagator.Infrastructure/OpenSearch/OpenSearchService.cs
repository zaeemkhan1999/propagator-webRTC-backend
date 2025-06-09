using OpenSearch.Net;
using OpenSearch.Client;
using Microsoft.Extensions.Configuration;
using System;
public interface IOpenSearchService
{
    void CreateIndex(string indexName);
    void IndexDocument<T>(string indexName, T document) where T : class;
    ISearchResponse<T> Search<T>(string indexName, string query) where T : class;
}

public class OpenSearchService : IOpenSearchService
{
    private readonly IOpenSearchClient _client;
    public OpenSearchService(IConfiguration configuration)
    {
        var settings = new ConnectionSettings(new Uri(configuration["OpenSearch:Url"]))
            .BasicAuthentication(configuration["OpenSearch:Username"], configuration["OpenSearch:Password"])
            .DefaultIndex("my-index");

        _client = new OpenSearchClient(settings);
    }
    public void CreateIndex(string indexName)
    {
        var response = _client.Indices.Create(indexName);
        if (!response.IsValid)
        {
            throw new Exception($"Error creating index: {response.DebugInformation}");
        }
    }
    public void IndexDocument<T>(string indexName, T document) where T : class
    {
        var response = _client.Index(document, idx => idx.Index(indexName));
        if (!response.IsValid)
        {
            throw new Exception($"Error indexing document: {response.DebugInformation}");
        }
    }
    public ISearchResponse<T> Search<T>(string indexName, string query) where T : class
    {
        var response = _client.Search<T>(s => s
            .Index(indexName)
            .Query(q => string.IsNullOrEmpty(query)
                ? q.MatchAll()
                : q.Bool(b => b
                    .Must(m => m.MultiMatch(mm => mm
                        .Fields(f => f
                            .Field("legalName")
                            .Field("displayName")
                            .Field("username")
                            .Field("email")
                        )
                        .Query(query)
                    ))
                )
            )
        );

        return response;
    }


}
