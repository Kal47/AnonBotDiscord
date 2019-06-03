using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using System;
using Example.Settings;

namespace Example
{
    public class JoinLeaveService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;
        private readonly IServiceProvider _provider;

        // DiscordSocketClient, CommandService, IConfigurationRoot, and IServiceProvider are injected automatically from the IServiceProvider
        public JoinLeaveService(
            DiscordSocketClient discord,
            CommandService commands,
            IConfigurationRoot config,
            IServiceProvider provider)
        {
            _discord = discord;
            _commands = commands;
            _config = config;
            _provider = provider;

            _discord.UserJoined += OnUserJoinAsync;
            _discord.UserLeft += OnUserLeftAsync;
        }

        private async Task OnUserJoinAsync(SocketGuildUser user)
        {
            if (GuildSettings.GetGuildSettings(user.Guild.Id).AutoJoin)
            {
                await new AnonChannelManagement(GuildSettings.GetGuildSettings(user.Guild.Id).CatagoryID).CreateAnonChannel(user);
            }

            return;
        } 

        private async Task OnUserLeftAsync(SocketGuildUser user)
        {
            await new AnonChannelManagement(GuildSettings.GetGuildSettings(user.Guild.Id).CatagoryID).RemoveAnnonChannel(user);
        }


    }
}