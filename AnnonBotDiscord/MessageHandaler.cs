using System;
using System.Linq;
using System.Threading.Tasks;
using AnonBot.DataAccess;
using Discord.WebSocket;

namespace AnonBot.Business
{
    class AnonMessageHandaler
    {
        public async Task Message(SocketMessage message, SocketCategoryChannel catagory)
        {

            var guildData = new AnonServer().GetGuildSettings(catagory.Guild.Id);

            if (catagory.Id == guildData.CatagoryID)
            {
                await SendAnonMessage(message, catagory);
                await message.DeleteAsync();
            }
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
                if (txChannel.Category.Id == 1)//check if channel is in the correct catagory name and send the message
                    await txChannel.SendMessageAsync(msg); //send message to all                            
            }                          
        }       
    }
}

