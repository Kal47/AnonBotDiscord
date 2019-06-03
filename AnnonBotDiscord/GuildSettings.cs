using AnonBot.DataAccess;
using Dapper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AnonBot.Business
{ 
    class AnonServer
    {        
        //static private IEnumerable<GuildSettingsData> GuildDataTable;

        GuildSettingsData ServerSettings;

        private void UpdateSettings()
        {
            //pulls settings from sql and stores them in memoery, is called after every change to the database
            //new DatabaseEngine().ReadSettings();


        }
        public int NewGuild(ulong guildID)
        {
            SqlParameterCollection sqlParams = null;
            sqlParams.Add(new SqlParameter("@Param", SqlDbType.BigInt) { Value = guildID} );

            int rowChanged = new DatabaseEngine().WriteSettings("INSERT INTO AnonBot VALUES(@Param, NULL, NULL, false);", sqlParams);
            return rowChanged;
        }

        public GuildSettingsData GetGuildSettings(ulong guildID)
        {
            return new DatabaseEngine().ReadSettings(guildID);
        }

        public int SetAutoJoin(ulong guildID, bool enable)
        {
            SqlParameterCollection sqlParams = null;
            sqlParams.Add(new SqlParameter("@GID", SqlDbType.Text) { Value = guildID });
            sqlParams.Add(new SqlParameter("@AutoJoin", SqlDbType.Bit){Value = enable});

            int rowChanged = new DatabaseEngine().WriteSettings("UPDATE AnonBot SET AutoJoin = @AutoJoin WHERE GuildID = @GID;", sqlParams);
                                 
            return rowChanged;
        }


        public int SetPublicChannelID(ulong guildID, ulong publicChannelID)
        {
            SqlParameterCollection sqlParams = null;
            sqlParams.Add(new SqlParameter("@GID", SqlDbType.Text) { Value = guildID });
            sqlParams.Add(new SqlParameter("@PublicChannelID", SqlDbType.BigInt){Value = publicChannelID});
            

            int rowChanged = new DatabaseEngine().WriteSettings("UPDATE AnonBot SET PublicChannelID = @PublicChannelID WHERE GuildID = @GID;", sqlParams);

            return rowChanged;
        }
    }
}
