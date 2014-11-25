using System.Net;

namespace NetMFAMS43
{
    internal interface IMobileServiceClient
    {
        string Insert(string tableName, IMobileServiceEntityData entity, bool noscript = false);

        HttpStatusCode Delete(string tableName, string entityId, bool noscript = false);

        HttpStatusCode Update(string tableName, IMobileServiceEntityData entity, bool noscript = false);

        string Query(string tableName, string query = null, bool noscript = false);
    }
}
