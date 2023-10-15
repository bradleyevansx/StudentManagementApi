using Microsoft.Azure.Cosmos;
using System;

namespace WebAPI.Repository
{
    public class CosmosConnectionManager
    {
        private readonly CosmosClient ClientConnection;

        public CosmosConnectionManager()
        {
            string apiurl = "https://obito-rip.documents.azure.com:443/";
            //string apiurl = Environment.GetEnvironmentVariable("APIURL");
            //string connectionString = Environment.GetEnvironmentVariable("CONNECTIONSTRING");
            string connectionString = "mZm47VxAmbhvNjRgT8CDx03kLmdMfotNW4gyptOfTB4Rl7kxBnzCwXTtcDjh6Cl8tgTuoI56HXKzACDb7QFGjw==";
            
            if (string.IsNullOrEmpty(apiurl) || string.IsNullOrEmpty(connectionString))
            {
                throw new ApplicationException("API URL or connection string not provided.");
            }

            ClientConnection = new CosmosClient(apiurl, connectionString);
        }

        public Container CreateConnection(string containerId)
        {
            return ClientConnection.GetContainer("Music", containerId);
        }
    }
}