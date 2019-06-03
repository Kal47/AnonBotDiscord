using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace AnonBot.DataAccess
{
    class DatabaseEngine
    {
        public int WriteSettings(string quarryString, SqlParameterCollection parameterList)
        {
            using (var connection = SQL.GetNewConnection())
            {
                SqlCommand sqlCommand = new SqlCommand(quarryString, connection);

                foreach (var x in parameterList)
                {
                    sqlCommand.Parameters.Add(x);
                }
                int rowChanged = sqlCommand.ExecuteNonQuery();
                
                connection.Close();
                return rowChanged;
            }
        }

        public GuildSettingsData ReadSettings(ulong serverID)
        {
            using (var getConnection = SQL.GetPersistantConnection())
            {
                var data = getConnection.Query<GuildSettingsData>("SELECT TOP 1 FROM AnonBot WHERE ServerID = @ServerID;", new { ServerID = (ulong?)null});

                foreach (GuildSettingsData x in data)
                {
                    return x;
                }
            }
            return null;
        }
    }
}
