using Microsoft.Extensions.DependencyInjection;
using WebAPI.Domain.Interfaces;
using WebAPI.Domain.Models;
using WebAPI.Repository;

namespace WebAPITest.Repository;

public static class DependencyInjection
{
    public static IServiceCollection AddRepository(this IServiceCollection services)
    {
        services.AddTransient<IRepository<UserInfo>, UserInfoRepository>();
        services.AddTransient<IUserInfoRepository, UserInfoRepository>();
        services.AddTransient<IRepository<PracticeSession>, PracticeSessionRepository>();
        services.AddTransient<ITokenRepository, TokenRepository>();
        services.AddTransient<IRepository<RefreshToken>, RefreshTokenRepository>();

        services.AddSingleton<CosmosConnectionManager>();

        /*services.AddSingleton<Database>(x =>
        {
            var client =
                new CosmosClient(
                    "AccountEndpoint=https://obito-rip.documents.azure.com:443/;AccountKey=mZm47VxAmbhvNjRgT8CDx03kLmdMfotNW4gyptOfTB4Rl7kxBnzCwXTtcDjh6Cl8tgTuoI56HXKzACDb7QFGjw==;");
            return client.GetDatabase("Music");
        });*/

        

        

        return services;
        
    }
}