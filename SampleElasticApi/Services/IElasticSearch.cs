using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Options;
using SampleElasticApi.Configurations;
using SampleElasticApi.Models;
using System.Security.AccessControl;

namespace SampleElasticApi.Services;

public interface IElasticSearch
{
    Task CreateIndexNotExists(string name);
    Task<bool> AddOrUpdateUser(User user);
    Task<bool> AddOrUpdateBulk(IEnumerable<User> users, string indexName);
    Task<User> GetUser(string key);
    Task<List<User>?> GetUsers();
    Task<bool> RemoveUser(string key);
    Task<long?> RemoveAllUsers();
}

public class ElasticSearch : IElasticSearch
{
    private readonly ElasticsearchClient _client;
    private readonly ElasticSettings _elasticSettings;

    public ElasticSearch(IOptions<ElasticSettings> options, ElasticsearchClient client)
    {
        _elasticSettings = options.Value;
        var settings = new ElasticsearchClientSettings(new Uri(_elasticSettings.Url))
            //.Authentication()
            .DefaultIndex(_elasticSettings.DefaultIndex)
            ;
        _client = new ElasticsearchClient(settings);
    }

    public async Task<bool> AddOrUpdateBulk(IEnumerable<User> users, string indexName)
    {
        var response = await _client.BulkAsync(b => b.Index(_elasticSettings.DefaultIndex)
        .UpdateMany(users, (us, u) => us.Doc(u).DocAsUpsert(true)));
        return response.IsValidResponse;

    }

    public async Task<bool> AddOrUpdateUser(User user)
    {
        var response = await _client.IndexAsync(user, id => id.Index(_elasticSettings.DefaultIndex)
        .OpType(OpType.Index));
        return response.IsValidResponse;
    }

    public async Task CreateIndexNotExists(string name)
    {
        if (!_client.Indices.Exists(name).Exists)
            await _client.Indices.CreateAsync(name);
    }

    public async Task<User> GetUser(string key)
    {
        var response = await _client.GetAsync<User>(key, g => g.Index(_elasticSettings.DefaultIndex));
        return response.Source;
    }

    public async Task<List<User>?> GetUsers()
    {
        var response = await _client.SearchAsync<User>(e => e.Index(_elasticSettings.DefaultIndex));
        return response.IsValidResponse ? response.Documents.ToList() : default;

    }

    public async Task<long?> RemoveAllUsers()
    {
        var response = await _client.DeleteByQueryAsync<User>(d => d.Indices(_elasticSettings.DefaultIndex));
        return response.IsValidResponse ? response.Deleted : default;
    }

    public async Task<bool> RemoveUser(string key)
    {
        var response = await _client.DeleteAsync<User>(key, e => e.Index(_elasticSettings.DefaultIndex));
        return response.IsValidResponse;
    }
}
