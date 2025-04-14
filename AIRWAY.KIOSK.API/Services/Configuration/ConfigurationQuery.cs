using System.Data;
using Dapper;

namespace AIRWAY.KIOSK.API.Services.Configuration;

public sealed partial class ConfigurationService
{
    private static (string sql, DynamicParameters param) GetByKeyQuery(string key)
    {
        // Param component
        var param = new DynamicParameters();
        param.Add(":key", key, dbType: DbType.String, direction: ParameterDirection.Input);

        // SQL component
        string query = @"SELECT t.id AS ""Id"", t.key AS ""Key"", t.value AS ""Value""
                        FROM tbl_configuration AS t
                        WHERE t.active AND t.key = :key";

        return (query, param);
    }

    private static (string sql, DynamicParameters param) GetByKeysQuery(string[] keys)
    {
        // Param component
        var param = new DynamicParameters();
        param.Add(":keys", keys, direction: ParameterDirection.Input);

        // SQL component
        string query = @"SELECT t.id AS ""Id"", t.key AS ""Key"", t.value AS ""Value""
                        FROM tbl_configuration AS t
                        WHERE t.active AND t.key = ANY (:keys)";

        return (query, param);
    }

    private static (string sql, DynamicParameters param) GetAllQuery(bool excludeInternal)
    {
        // Param component
        var param = new DynamicParameters();

        // SQL component
        string query = @"SELECT t.id AS ""Id"", t.key AS ""Key"", t.value AS ""Value""
                        FROM tbl_configuration AS t
                        WHERE t.active";

        if (excludeInternal)
            query = $"{query} AND NOT (t.internal)";

        return (query, param);
    }
}