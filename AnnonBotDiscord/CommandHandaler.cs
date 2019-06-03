using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace AnnonBotDiscord
{
    class CommandHandaler2
    {
        /****************************************
        * Suported Commands
        * 
        * Admin Only Commands
        * /a {message} 
        *  sends an messaged tagged as admin
        * 
        * 
        * Debug Commands 
        * /join 
        * *************************************************/
        public char Prefix { get; set; }
        public string AnonCatagoryName { get; set; }


        AnonChannelManagement AnonChanMan = new AnonChannelManagement("ANON");

        public CommandHandaler(char prefix, string catagoryName = "ANON")
        {
            Prefix = prefix;
            AnonCatagoryName = catagoryName;
        }
        
        //Manages commands for the bot. 
        private async Task Command(SocketMessage message, SocketCategoryChannel catagory)
        {
            String command = message.Content.ToLower();
            command.Remove(1);//removes prefix

            //commands that can be used anywhere
            if (command == "join")
            {
                await AnonChanMan.CreateAnonChannel((SocketGuildUser)message.Author);
                return;
            }
            else if (command == "leave")
            {
                await AnonChanMan.RemoveAnnonChannel((SocketGuildUser)message.Author);
                return;
            }
            else if (command == "help")
            {
                await message.Channel.SendMessageAsync("Bot Commands:\n" +
                   "something something idk");
                return;
            }  
        }
    }
}

