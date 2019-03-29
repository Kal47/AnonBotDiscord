using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace AnonBot
{
    // This is a minimal, bare-bones example of using Discord.Net
    //
    // If writing a bot with commands, we recommend using the Discord.Net.Commands
    // framework, rather than handling commands yourself, like we do in this sample.
    //
    // You can find samples of using the command framework:
    // - Here, under the 02_commands_framework sample
    // - https://github.com/foxbot/DiscordBotBase - a bare-bones bot template
    // - https://github.com/foxbot/patek - a more feature-filled bot, utilizing more aspects of the library
    class Program
    {
        private DiscordSocketClient _client;
        string Token = "token";
        private List<ulong> channelList = new List<ulong>();
        private String CatagoryName = "Anon";


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
            //load jason file


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
                    "");
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
                            await txChannel.SendMessageAsync("**" + message.Id % 42069 + "**\n" + message.Content); //send message to all 
                    }
                }
                else
                    Console.WriteLine("That casting is borked rip");    
            }

        }

        private async Task UserJoinedAsync(SocketGuildUser user)
        {
            await CreateAnonChannel(user);
        }

        private async Task UserLeftAsync(SocketGuildUser user)
        {
            await RemoveAnnonChannel(user);       
        }


        private async Task RemoveAnnonChannel(SocketGuildUser user)
        {
            Console.WriteLine("Delteing thing dosnt work yet");
        }

        /* User Joined
         * 1. Creates a new channgel named after the user
         * 2. Sets permissions so the user can only see that channel
         * 3. Adds the channel ID to a list 
         */
        private async Task CreateAnonChannel(SocketGuildUser user)
        {
            //sees if catagory exists and creates one if there is none
            foreach (var cat in user.Guild.CategoryChannels)
            {
                if (cat.Name != CatagoryName)
                {  
                    var catagory = await user.Guild.CreateCategoryChannelAsync(CatagoryName);
                   // _client.Guilds.ElementAt(1).EveryoneRole;   ///find out how to get @everyone role
                   // catagory.AddPermissionOverwriteAsync(IRole,)
                    continue;
                }

            }


                //creates a new channel that only the new user can acsess. 
                var channel = await user.Guild.CreateTextChannelAsync(user.Username, x =>
            {                
                x.Topic = $"{user.Username}'s Channel";                
                x.IsNsfw = true;   
               
                foreach (var cat in user.Guild.CategoryChannels)
                {
                    if (cat.Name == CatagoryName)
                    {
                        x.CategoryId = cat.Id;
                        continue;
                    }
                }
                
            });

            //Modify permersions on users channel
            OverwritePermissions channelPerms = new OverwritePermissions(
                 PermValue.Deny,    //PermValue createInstantInvite
                 PermValue.Deny,    //PermValue manageChannel
                 PermValue.Deny,    //PermValue addReactions
                 PermValue.Allow,   //PermValue viewChannel
                 PermValue.Allow,   //PermValue sendMessages
                 PermValue.Deny,    //PermValue sendTTSMessages
                 PermValue.Deny,    //PermValue manageMessages
                 PermValue.Allow,   //PermValue embedLinks
                 PermValue.Inherit, //PermValue attachFiles  !!!I wish I can attach files idk, will leave this inherit and change it if it works
                 PermValue.Allow,   //PermValue readMessageHistory
                 PermValue.Deny,    //PermValue mentionEveryone
                 PermValue.Deny,    //PermValue useExternalEmojis
                 PermValue.Deny,    //PermValue connect
                 PermValue.Deny,    //PermValue speak
                 PermValue.Deny,    //PermValue muteMembers
                 PermValue.Deny,    //PermValue deafenMembers
                 PermValue.Deny,    //PermValue moveMembers
                 PermValue.Deny,    //PermValue useVoiceActivation
                 PermValue.Deny,    //PermValue manageRoles
                 PermValue.Deny);   //PermValue manageWebhooks
            await channel.AddPermissionOverwriteAsync(user, channelPerms);

            //add channel ID to the list

            channelList.Add(channel.Id);
            //write new channel to xml fle

            //Send Welcome Message
            var msg = await channel.SendMessageAsync($"**Welcome {user.Mention} to {user.Guild.Name}!**\n\n" +
                "Everyone on this server is in a channel by themselves and the bot. " +
                "The bot grabs all messages sent by users and sends them to everyone elses channel. " +
                "Becouse the bot posts the message you don't know who sent the message.\n\n" +
                "This bot does not log anything. You can view the source code at https://github.com/doc543/AnonBot \n" +
                "Use \"\\help\" - Brings up help text."
                );
            //await user.Guild.AddPinAsync(channel.Id, msg.Id); //pin msg
        }



    }
} 