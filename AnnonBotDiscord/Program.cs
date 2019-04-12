using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Linq;
using Blun.ConfigurationManager;
using AnnonBotDiscord;

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
        readonly private String CatagoryName = "ANON";
        readonly private char PrefixChar = '!';

        private DiscordSocketClient _client;
        
        AnonChannelManagement AnonChanMan = new AnonChannelManagement();
        CommandHandaler CmdHandiler = new CommandHandaler('!');

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
            _client.MessageReceived += MessageReceivedAsync;
            _client.UserLeft += UserLeftAsync;
        }

        public async Task MainAsync()
        {
            
            var _config = new DiscordSocketConfig { MessageCacheSize = 100 };
            // Tokens should be considered secret data, and never hard-coded.
            await _client.LoginAsync(TokenType.Bot, getToken());
            await _client.StartAsync();
            // Block the program until it is closed.
            await Task.Delay(-1);
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

        // This is not the recommended way to write a bot - consider
        // reading over the Commands Framework sample.
        private async Task MessageReceivedAsync(SocketMessage message)
        {
            // The bot should never respond to itself.
            if (message.Author.Id == _client.CurrentUser.Id)
                return;

            if (message.Content[0] == PrefixChar)
                await CmdHandiler.Command(message, _client);
            else
                await CmdHandiler.SendAnonMessage(message);
            await message.DeleteAsync();//deletes original message


        }


        // User Joined the Server        
        private async Task UserJoinedAsync(SocketGuildUser user)
        {
            Console.WriteLine($"User joined {user.Username}!");
            await AnonChanMan.CreateAnonChannel(user, CatagoryName);
        }

        // User Left the server        
        private async Task UserLeftAsync(SocketGuildUser user)
        {
            Console.WriteLine($"User left {user.Username}!");
            await AnonChanMan.RemoveAnnonChannel(user);
        }

        private string getToken()
        {
            String envName = "DiscordToken";

            // Determine whether the environment variable exists.
            if (Environment.GetEnvironmentVariable(envName) == null)
            {
                // If it doesn't exist, create it.
                Console.WriteLine("No DiscordToken Enviroment Varable! Please enter It now:");
                Environment.SetEnvironmentVariable(envName, Console.ReadLine());
            }
            return Environment.GetEnvironmentVariable(envName);
        }
    }
} 