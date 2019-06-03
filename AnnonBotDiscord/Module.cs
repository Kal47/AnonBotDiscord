using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using 

namespace AnnonBotDiscord.Modules
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
        [Command("join")]
        [Summary("Join the Anon ")]
        public async Task JoinAsync(SocketMessage message)
        {
            await new AnonChannelManagement("ANON").CreateAnonChannel((SocketGuildUser)message.Author);
        }

        [Command("leave")]
        [Summary ("Adds Bot to check list.")]
        public async Task AddAsync(SocketMessage message)  
        {
            await new AnonChannelManagement("ANON").RemoveAnnonChannel((SocketGuildUser)message.Author);
        }

        [Command("autojoin")]
        [Summary("True/False if user auto joins anon when they join the server")]
        public async Task RemoveAsync(SocketMessage message,
            [Summary("Catagory For Annyoamus Channel")] string catagoryName)
        {
            
        }

        [Command("publicchannel")]
        [Summary("Enables a public channel so the people who are not joined can see the anon chat.")]
        public async Task HelpAsync()
        {
            
        }

        [Command("help")]
        [Summary("Help")]
        public async Task HelpAsync()
        {
           await ReplyAsync($"Oh look the help text works!");
        }


    }
}
