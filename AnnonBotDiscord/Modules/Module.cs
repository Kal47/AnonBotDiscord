using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using SQLDiscordBot.Static;

namespace SQLDiscordBot.Modules
{
    // Keep in mind your module **must** be public and inherit ModuleBase.
    // If it isn't, it will not be discovered by AddModulesAsync!
 
    public class BotModule : ModuleBase<SocketCommandContext>
    {
        private readonly DiscordSocketClient _client;
        public BotModule(DiscordSocketClient client)
        {
            _client = client;
        }

        // ~sample square 20 -> 400
        [Command("botlist")]
        [Summary("Gets list of bots from the check list.")]
        public async Task ListAsync()
        {
            // We can also access the channel from the Command Context.
            var SQLServer = SQL.GetDataConnection();
            string output = "List of users";
            UserList uList = new UserList();
            foreach (ulong x in uList.GetBots())
            {
                output = $"{output}, {x.ToString()}";
            }
            await Context.Channel.SendMessageAsync(output);
        }

        [Command("botadd")]
        [Summary ("Adds Bot to check list.")]
        public async Task AddAsync(
            [Summary("ID of bot")]
        ulong botID)
        {          
            var server = SQL.SendDataConnection();

            //check if user exeist and if is a bot or not
            var bot = _client.GetUser(botID);
            if (bot == null)
            {
                await ReplyAsync($"No bot with the ID {botID}");
                return;
            }

            if (!bot.IsBot)
            {
                await ReplyAsync($"ID {botID} is for a user not a bot.");
                return;
            }           

            new UserList().AddBot(botID);
            server.Close();          
            await ReplyAsync($"User with the ID {botID} was added to the check list!");
        }

        [Command("botremove")]
        [Summary("Removes a Bot from check list.")]
        public async Task RemoveAsync(
            [Summary("ID of bot")]
        ulong botID)
        {
            int rowsChanged = new UserList().RemoveBot(botID);
            if (rowsChanged > 0)
                await ReplyAsync($"Bot with the ID {botID} was removed from the check list");
            else
                await ReplyAsync($"No bot with ID {botID} on check list");
        }

        [Command("botcheck")]
        [Summary("One time check of bot")]
        public async Task CheckAsync(
            [Summary("ID of bot")]
        ulong botID)
        {
            //using a static class to get a varable in here..
            //I really really wish I knew of a batter way to do this.
            IDiscordClient disc = _client as IDiscordClient;

            Console.WriteLine("Updating " + botID.ToString());
            var user = disc.GetUserAsync(botID).Result;

            if (user == null)
            { 
                await ReplyAsync($"No bot with the ID {botID}");
                return;
            }

            string userName = $"{user.Username}#{user.Discriminator}, ";
            string status = user.Status.ToString();
            string avitarURL = user.GetAvatarUrl();
            string activity = "";
            if (user.Activity != null)
                activity = $"{user.Activity.Type.ToString()} {user.Activity.Name.ToString()}, ";
  
            await ReplyAsync($"{userName}{status}, {activity}{avitarURL}");
        }

        [Command("help")]
        [Summary("Help")]
        public async Task HelpAsync()
        {
           await ReplyAsync($"Oh look the help text works!");
        }

        [Command("botupdate")]
        [Summary("Updates all bots")]
        public async Task UpdateAsync()
        {
            new UserStatus().UpdateAll(_client);
            await ReplyAsync($"Oh cool it didnt crash!");
        }

    }
}
