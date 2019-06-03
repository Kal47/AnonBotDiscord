using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using AnnonBotDiscord;
using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;

namespace AnonBot
{
    // This is a minimal, bare-bones example of using Discord.Net
    //
    // If writing a bot with commands, we recommend using the Discord.Net.Commands
    // framework, rather than handling commands yourself, like we do in this sample.
    //
    // You can find samples of using the command framework:    // - Here, under the 02_commands_framework sample
    // - https://github.com/foxbot/DiscordBotBase - a bare-bones bot template
    // - https://github.com/foxbot/patek - a more feature-filled bot, utilizing more aspects of the library
    class Program
    {
        //changable vars
        readonly static private String CatagoryName = "ANON";
        readonly static private char PrefixChar = '!';

        private DiscordSocketClient _client;

        AnonChannelManagement AnonChanMan = new AnonChannelManagement(CatagoryName);
        MessageHandaler MagHandeler = new MessageHandaler(PrefixChar);

        // Discord.Net heavily utilizes TAP for async, so we create
        // an asynchronous context from the beginning.
        static void Main(string[] args)
        {
            new Program().MainAsync().GetAwaiter().GetResult();
        }

        public Program()
        {
            // It is recommended to Dispose of a client when you are finished
            // using it, at the end of your app's lifetime.
            _client = new DiscordSocketClient();

            _client.Log += LogAsync;
            _client.Ready += ReadyAsync;
            _client.UserJoined += UserJoinedAsync;
            _client.UserLeft += UserLeftAsync;
        }

        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();

                client.Log += LogAsync;
                client.Ready += ReadyAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                // Tokens should be considered secret data and never hard-coded.
                // We can read from the environment variable to avoid hardcoding.
                await client.LoginAsync(TokenType.Bot, GetToken());
                await client.StartAsync();

                // Here we initialize the logic required to register our commands.
                await services.GetRequiredService<CommandHandler>().InitializeAsync();

                await Task.Delay(-1);
            }
        }

        private ServiceProvider ConfigureServices()
        {
            return new ServiceCollection()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }

        //Logs are sent to the console
        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        // The Ready event indicates that the client has opened a
        // connection and it is now safe to access the cache.
        private Task ReadyAsync()
        {
            Console.WriteLine($"{_client.CurrentUser} is connected!");

            return Task.CompletedTask;
        }

        // User Joined the Server        
        private async Task UserJoinedAsync(SocketGuildUser user)
        {
            Console.WriteLine($"User joined {user.Username}!");
            await AnonChanMan.CreateAnonChannel(user);
        }

        // User Left the server        
        private async Task UserLeftAsync(SocketGuildUser user)
        {
            Console.WriteLine($"User left {user.Username}!");
            await AnonChanMan.RemoveAnnonChannel(user);
        }

        private string GetToken()
        {
            string token = "";
            string path = @"token.txt";
            try
            {
                token = System.IO.File.ReadAllText(path);
            }
            catch
            { 
                // If it doesn't exist, create it.
                Console.WriteLine("No Token file found! Please enter It now:");
                token = Console.ReadLine();
                System.IO.File.WriteAllText(path, token);
            }
            return token;
        }
    }
}
/* serverID bigint
 * CatagoryID bigint
 * Prefix varchar(255)
 * AutoJoin bool
 * PublicChannelID bigint //if null no public channel
 * 
 * /
