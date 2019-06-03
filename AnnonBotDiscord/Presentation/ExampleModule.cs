using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Example.Settings;
using System.Threading.Tasks;

namespace Example.Modules
{
    [Name("Example")]
    public class ExampleModule : ModuleBase<SocketCommandContext>
    {
        [Command("say"), Alias("s")]
        [Summary("Make the bot say something")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public Task Say([Remainder]string text)
            => ReplyAsync(text);
        
        [Group("set"), Name("Example")]
        [RequireContext(ContextType.Guild)]
        public class Set : ModuleBase
        {
            [Command("nick"), Priority(1)]
            [Summary("Change your nickname to the specified text")]
            [RequireUserPermission(GuildPermission.ChangeNickname)]
            public Task Nick([Remainder]string name)
                => Nick(Context.User as SocketGuildUser, name);

            [Command("nick"), Priority(0)]
            [Summary("Change another user's nickname to the specified text")]
            [RequireUserPermission(GuildPermission.ManageNicknames)]
            public async Task Nick(SocketGuildUser user, [Remainder]string name)
            {
                await user.ModifyAsync(x => x.Nickname = name);
                await ReplyAsync($"{user.Mention} I changed your name to **{name}**");
            }
        }
    }

    public class PublicModule : ModuleBase<SocketCommandContext>
    {
        private readonly DiscordSocketClient _client;
        public PublicModule(DiscordSocketClient client)
        {
            _client = client;
        }

        // ~sample square 20 -> 400
        [Command("join")]
        [Summary("Join the Anon ")]
        public async Task JoinAsync(SocketMessage message)
        {
            if (!(message.Author is SocketGuildUser user))
                return;
            ulong catagoryID = GuildSettings.GetGuildSettings(user.Guild.Id).CatagoryID;

            await new AnonChannelManagement(catagoryID).CreateAnonChannel(user);
        }

        [Command("leave")]
        [Summary("Adds Bot to check list.")]
        public async Task AddAsync(SocketMessage message)
        {
            if (!(message.Author is SocketGuildUser user))
                return;
            ulong catagoryID = GuildSettings.GetGuildSettings(user.Guild.Id).CatagoryID;

            await new AnonChannelManagement(catagoryID).RemoveAnnonChannel(user);
        }

        [Command("autojoin")]
        [Summary("True/False if user auto joins anon when they join the server")]
        public void AutoJoinAsync(SocketMessage message,
            [Summary("Catagory For Annyoamus Channel")] bool enable)
        {
            if (!(message.Author is SocketGuildUser user))
                return;
            GuildSettings.SetAutoJoin(user.Guild.Id, enable);
        }

        [Command("publicchannel")]
        [Summary("Enables a public channel so the people who are not joined can see the anon chat.")]
        public async Task PublicChannelAsync(SocketMessage message,
            [Summary("10")] bool enableChannel)
        {
            if (!(message.Author is SocketGuildUser user))
                return;
            ulong catagoryID = GuildSettings.GetGuildSettings(user.Guild.Id).CatagoryID;

            if (enableChannel)
            {
                if (true)//do an sql if channel is already made)               


                    await new AnonChannelManagement(catagoryID).CreatePublicAnonChannel(user.Guild);
            }
            else
            {
                await new AnonChannelManagement(catagoryID).RemovePublicAnonChannel(user.Guild);
            }
            //sets some sql doodad to 
        }

        [Command("a")]
        [RequireUserPermission(GuildPermission.Administrator)]
        public Task ListAsync(params string[] objects)
            


        [Command("prefix")]
        [Summary("Set Prefix for bot ")]
        public async Task HelpAsync()
        {
            await ReplyAsync($"Oh look the help text works!");
        }


    }
}
