using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonBot.DataAccess
{
    /* 
    * GuildID bigint NOT NULL,
    * CatagoryID bigint DEFAULT 0, //
    * Prefix varchar(255) NULL, //if null only way to do cmd is ping bot
    * PublicChannelID bigInt DEFAULT 0,
    * AutoJoin bit DEFAULT 0
    *  
    */

    class GuildSettingsData
    {
        public ulong GuildID { get; set; }
        public ulong CatagoryID { get; set; }
        public string Prefix { get; set; }
        public ulong PublicChannelID { get; set; }
        public bool AutoJoin { get; set; }
    }
}
