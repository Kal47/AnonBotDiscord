using System;
using System.Linq;
using System.Threading.Tasks;
using Discord.WebSocket;

namespace AnnonBotDiscord
{
    class MessageHandaler
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
        /* **************************************************************
         * Constructor
         * ****************************************************************/
        public MessageHandaler(char prefix, string catagoryName = "ANON")
        {
            Prefix = prefix;
            AnonCatagoryName = catagoryName;
        }
        public async Task Message(SocketMessage message, SocketCategoryChannel catagory)
        {
            if (message.Content[0] == Prefix)
                await Command(message, catagory);
            else if (catagory.Name == AnonCatagoryName)
            {
                await SendAnonMessage(message, catagory);
                await message.DeleteAsync();
            }
        }


       /* **************************************************************
        * Manages commands for the bot. 
        * ****************************************************************/
        private async Task Command(SocketMessage message, SocketCategoryChannel catagory)
        {
            String command = message.Content.ToLower();
            command = command.Remove(0,1);//removes prefix

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

            //commands that should only be used in anon catagory
            if (catagory.Name == AnonCatagoryName)
            {
                if (command.ElementAt(0) == 'a')
                {
                    //checks if user has manage roles permissions which is most likely the admin                    
                    if (catagory.Guild.Owner.Id == message.Id)
                        await AdminMessage(message, catagory);
                    else
                        await message.Channel.SendMessageAsync("Error: You do not have permision to use this command");
                    return;
                }
            }
            await message.Channel.SendMessageAsync("Error: Unknown Command");
        }

        //sends a message makred diffrently then other messages
        public async Task AdminMessage(SocketMessage message, SocketCategoryChannel catagory)
        {
            string anonMessage = message.Content;
            anonMessage = anonMessage.Remove(0, 3);//removes command prefix
            Console.WriteLine(anonMessage);
            await SendMsgToEveryChanInCatagory($":o2:**{DateTime.Now.TimeOfDay.Ticks % 42069}**\n{anonMessage}", catagory);
        }

        //sends message anon formated message to every channel in catagory
        public async Task SendAnonMessage(SocketMessage message, SocketCategoryChannel catagory)
        {
            string anonMessage = $":record_button:**{new DateTime(1970, 1, 1).Ticks % 42069}**\n{message.Content}";
            await SendMsgToEveryChanInCatagory(anonMessage, catagory);
        }

        //send a message to every channel in a catagory that the channel the message was sent in
        private async Task SendMsgToEveryChanInCatagory(string msg, SocketCategoryChannel catagory)
        {       
            //get all text channels in the guild the message was sent from
            var guildTextChannels = catagory.Guild.TextChannels.OfType<SocketTextChannel>();
            foreach (var txChannel in guildTextChannels)//go though all channels on server
            {
                if (txChannel.Category.Name == AnonCatagoryName)//check if channel is in the correct catagory name and send the message
                    await txChannel.SendMessageAsync(msg); //send message to all                            
            }                          
        }       
    }
}

