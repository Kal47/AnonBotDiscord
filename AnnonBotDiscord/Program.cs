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
        //string connectionString = ConfigurationManager.ConnectionStrings["MyKey"].ConnectionString;
        private DiscordSocketClient _client;

        readonly string Token = System.IO.File.ReadAllText(@"token.txt");//reads token from txt file becouse all the config stuff I cant get to work

        AnonChannelManagement AnonChanMan = new AnonChannelManagement();

        readonly private String CatagoryName = "ANON";        

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
            await _client.LoginAsync(TokenType.Bot, Token);
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

            if (message.Content == "\\help")// help commands
                await message.Channel.SendMessageAsync("Bot Commands:\n" +
                    "something something idk");
            else if (message.Content == "\\join")

            await AnonChanMan.CreateAnonChannel((SocketGuildUser)message.Author, CatagoryName);
            else
            {
                //get all text channels in the guild the message was sent from
                SocketTextChannel messageTextChannel = message.Channel as SocketTextChannel;// convert to something i can work with this part hurts my brain
                if (messageTextChannel != null)
                {
                    var guildTextChannels = messageTextChannel.Guild.TextChannels.OfType<SocketTextChannel>();
                    foreach (var txChannel in guildTextChannels)//go though all channels on server
                    {
                        if (txChannel.Category.Name == CatagoryName)//check if channel is in the correct catagory name and send the message
                        {
                            await txChannel.SendMessageAsync("**" + message.Id % 42069 + "**\n" + message.Content); //send message to all 
                            await message.DeleteAsync();//deletes original message
                        }
                    }
                }
                else
                    Console.WriteLine("That casting is borked rip");
            }

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

             
    }
} 