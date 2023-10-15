using Microsoft.Azure.Cosmos;
using WebAPI.Domain.Interfaces;
using WebAPI.Domain.Models;

namespace WebAPI.Repository;

public interface IUserInfoRepository : IRepository<UserInfo>
{
    Task<UserInfo?> GetByCredentialsAsync(string username, string password);
    Task<UserInfo?> RegisterUserAsync(UserInfo userInfo);
}

public class UserInfoRepository : CosmosRepository<UserInfo>, IUserInfoRepository
{
    private readonly Container _containerConnection;
    public UserInfoRepository(CosmosConnectionManager connectionManager) : base(connectionManager, "UserInfo")
    {
        _containerConnection = connectionManager.CreateConnection("UserInfo");
    }

    public async Task<UserInfo?> GetByCredentialsAsync(string username, string password)
    {
        var results = await Query().Where(x => x.Username == username && x.Password == password).ToListAsync();
        return results.Count is 0 ? null : results[0];
    }

    public async Task<UserInfo?> RegisterUserAsync(UserInfo newUser)
    {
        if (string.IsNullOrWhiteSpace(newUser.id)) return null;
        var toBeUpdatedUser = await _containerConnection.FirstAsync<UserInfo?>(newUser.id);
        if (toBeUpdatedUser is null) return null;
        toBeUpdatedUser.Username = newUser.Username;
        toBeUpdatedUser.Password = newUser.Password;
        toBeUpdatedUser.Person = newUser.Person;
        var response = await _containerConnection.UpsertItemAsync(toBeUpdatedUser, new PartitionKey(newUser.id));
        return response.Resource;
    }
}
