using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using AnnonBotDiscord;

namespace AnnonBotDiscord
{
    class CommandHandaler
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
        public string CatagoryName { get; set; }

        AnonChannelManagement AnonChanMan = new AnonChannelManagement();

        public CommandHandaler(char prefix, string catagoryName = "ANON")
        {
            Prefix = prefix;
            CatagoryName = catagoryName;
        }

        public async Task Command(SocketMessage message, DiscordSocketClient _client)
        {
            String command = message.Content.ToLower();
            command.Remove(1);//removes prefix
            if (command == "join")
                await AnonChanMan.CreateAnonChannel((SocketGuildUser)message.Author, CatagoryName);
            else if (command == "leave")
                await AnonChanMan.RemoveAnnonChannel((SocketGuildUser)message.Author);
            else if (command == "help")
                await message.Channel.SendMessageAsync("Bot Commands:\n" +
                   "something something idk");
            else if (command.StartsWith("a "))
                await adminMessage(message);
            else
                await message.Channel.SendMessageAsync("Error: Unknown Command");
        }    

        private async Task adminMessage(SocketMessage message)
        {

            string anonMessage = message.Content;
            anonMessage.Remove(1);//removes prefix
            await SendMsgToEveryChanInCatagory($":02:**{message.Id % 42069}**\n{anonMessage}", message);
        }

        public async Task SendAnonMessage(SocketMessage message)
        {
            string anonMessage = $":record_button:**{message.Id % 42069}**\n{message}";
            await SendMsgToEveryChanInCatagory(anonMessage, message);
        }

        private async Task SendMsgToEveryChanInCatagory(string msg, SocketMessage message)
        {
            //get all text channels in the guild the message was sent from
            SocketTextChannel messageTextChannel = message.Channel as SocketTextChannel;// convert to something i can work with this part hurts my brain
            if (messageTextChannel != null)
            {
                if (messageTextChannel.Category.Name == CatagoryName)//make sure messages that are sent from outside the catagory are not sent
                {
                    var guildTextChannels = messageTextChannel.Guild.TextChannels.OfType<SocketTextChannel>();
                    foreach (var txChannel in guildTextChannels)//go though all channels on server
                    {
                        if (txChannel.Category.Name == CatagoryName)//check if channel is in the correct catagory name and send the message
                            await txChannel.SendMessageAsync(msg); //send message to all                            
                    }
                }
            }
        }

       
    }
}

