using SampleElasticApi.Models;

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
