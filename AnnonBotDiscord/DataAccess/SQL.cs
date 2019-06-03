using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnonBot.DataAccess
{
    static class SQL
    {
         static private String UserName;
         static private String Password;
         static private String Server;
         static private SqlConnection Conection;

        public static void SetLoginCerdentials(string username, string password, string server)
        {
            UserName = username;
            Password = password;
            Server = server;
            Conection = new SqlConnection(GetConnectionString());
            try
            {
                Conection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        public static SqlConnection GetNewConnection()
        {
            var concetion = new SqlConnection(GetConnectionString());
            try
            {
                concetion.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return concetion;
        }

        public static SqlConnection GetPersistantConnection()
        {
            //check if connection is still working?

            return Conection;
        }

        public static void RestartConnection()
        {
            Conection = new SqlConnection(GetConnectionString());
            try
            {
                Conection.Open();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        private static string GetConnectionString()
        {
            return $"user={UserName}; password={Password}; server={Server};" +
           "Trusted_Connection=no; database=Stalker; connection timeout=30";
        }
    }
}
