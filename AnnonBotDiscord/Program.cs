using System;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Discord.Rest;

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
        string Token = "";
        
        private String CatagoryName = "ANON";
        private readonly OverwritePermissions PermissionsAnonCatagory = new OverwritePermissions(
            PermValue.Deny,   //PermValue createInstantInvite
            PermValue.Deny,   //PermValue manageChannel
            PermValue.Deny,   //PermValue addReactions
            PermValue.Deny,   //PermValue viewChannel
            PermValue.Deny,   //PermValue sendMessages
            PermValue.Deny,   //PermValue sendTTSMessages
            PermValue.Deny,   //PermValue manageMessages
            PermValue.Deny,   //PermValue embedLinks
            PermValue.Deny,   //PermValue attachFiles  !!!I wish I can attach files idk, will leave this inherit and change it if it works
            PermValue.Deny,   //PermValue readMessageHistory
            PermValue.Deny,   //PermValue mentionEveryone
            PermValue.Deny,   //PermValue useExternalEmojis
            PermValue.Deny,   //PermValue connect
            PermValue.Deny,   //PermValue speak
            PermValue.Deny,   //PermValue muteMembers
            PermValue.Deny,   //PermValue deafenMembers
            PermValue.Deny,   //PermValue moveMembers
            PermValue.Deny,   //PermValue useVoiceActivation
            PermValue.Deny,   //PermValue manageRoles
            PermValue.Deny);  //PermValue manageWebhooks

        private readonly OverwritePermissions PermissionsAnnChannel = new OverwritePermissions(
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
                    "something something idk");
            else if (message.Content == "\\join")
                await CreateAnonChannel((SocketGuildUser)message.Author);                
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
        // Links to create annon channel method
        private async Task UserJoinedAsync(SocketGuildUser user)
        {
            Console.WriteLine($"User joined {user.Username}!");
            await CreateAnonChannel(user);
        }

        // User Left the server
        // links to remove channel method
        //this is needed so there are not exes channels around being used.
        private async Task UserLeftAsync(SocketGuildUser user)
        {
            Console.WriteLine($"User left {user.Username}!");
            await RemoveAnnonChannel(user);       
        }

        //
        private async Task RemoveAnnonChannel(SocketGuildUser user)
        { 
            foreach (var chan in user.Guild.Channels)
            {
                if (chan.Name == user.Id.ToString())//looks for channel the same name as user ID
                {
                    await chan.DeleteAsync();
                }
            }
            
        }

        /********************************************************
         * Creates a new channel, for refrence only not used
         * 
         * ********************************************************/
        public async Task CreateTextChat(string name, SocketGuild guild, string cName,  string startMessage = "")
        {            
            var text = await guild.CreateTextChannelAsync(name);
            if (!(cName == ""))
            {
                ICategoryChannel category = guild.CategoryChannels.FirstOrDefault(x => x.Name == cName);

                if (category == null)
                {
                    category = await guild.CreateCategoryChannelAsync(cName);
                }

                await text.ModifyAsync(x => x.CategoryId = category.Id);
                
            }

             Console.WriteLine("Created Text Channel: " + name);
        }

        /* **************************************************************
         * Create Anon Channel
         * 1. Creates a new channgel named after the user
         * 2. Checks if a catagory is made if not creates one and sets permissions
         * 4. Sets permissions on channel so the user can only see that channel
         * 5. Send Welcome message and pins it. 
         * ****************************************************************/
        private async Task CreateAnonChannel(SocketGuildUser user)
        {           
            var everyoneRole = user.Guild.Roles.FirstOrDefault(x => x.IsEveryone);

            Console.WriteLine($"Creating Channel for {user.Username}! for  Guild {user.Guild.Name}");
            var text = await user.Guild.CreateTextChannelAsync(user.Id.ToString());
            if (string.IsNullOrEmpty(CatagoryName))//this works somehow
            {
                ICategoryChannel category = user.Guild.CategoryChannels.FirstOrDefault(x => x.Name == CatagoryName);

                if (category == null)
                {
                    category = await user.Guild.CreateCategoryChannelAsync(CatagoryName);                    
                }
                await text.ModifyAsync(x => x.CategoryId = category.Id);
                await category.AddPermissionOverwriteAsync(everyoneRole, PermissionsAnonCatagory);
            }
           
            await text.AddPermissionOverwriteAsync(everyoneRole, PermissionsAnonCatagory);
            await text.AddPermissionOverwriteAsync(user, PermissionsAnnChannel);
            //await user.Guild.AddPinAsync(channel.Id, msg.Id); //pin msg
            await text.SendMessageAsync($"**Welcome {user.Mention} to {user.Guild.Name}!**\n\n" +
            "Everyone on this server is in a channel by themselves and the bot. " +
            "The bot grabs all messages sent by users and sends them to everyone elses channel. " +
            "Becouse the bot posts the message you don't know who sent the message.\n\n" +
            "This bot does not log anything. You can view the source code at https://github.com/doc543/AnonBot \n" +
            "Use \"\\help\" - Brings up help text."
            );
            Console.WriteLine("Created Text Channel: " + CatagoryName);


            //         |
            //old code V
            /*
            //sees if catagory exists and creates one if there is none
            var anonCatagory = user.Guild.CategoryChannels.FirstOrDefault(x => x.Name == CatagoryName); 
            if (anonCatagory == null)
            {
                Console.WriteLine($"Catagory not found");
                await user.Guild.CreateCategoryChannelAsync(CatagoryName);
                anonCatagory = user.Guild.CategoryChannels.FirstOrDefault(x => x.Name == CatagoryName);
                SocketRole everyoneRole;
                foreach (var role in user.Guild.Roles)
                {
                    if (role.IsEveryone)
                    {
                        everyoneRole = role;                        
                        await anonCatagory.AddPermissionOverwriteAsync(everyoneRole, PermissionsAnonCatagory);
                        Console.WriteLine($"New Catagory, ID {anonCatagory.Id}");                        
                        continue;
                    }
                }
            }
            Console.WriteLine($"Catagory, ID {anonCatagory.Id}");

            
            //creates a new channel that only the new user can acsess. 

            var channel = await user.Guild.CreateTextChannelAsync(user.Id.ToString(), x =>//channel name is set to user ID so the delete method knows what channel to delete
            {
                x.Topic = "\\help for help text, bot commands do not get sent";
                x.IsNsfw = true;
                x.CategoryId = anonCatagory.Id; 
            });

            
            await user.Guild.GetChannel(channel.Id).ModifyAsync(x =>
            {
                x.CategoryId = anonCatagory.Id;
            });
            
            





            Console.WriteLine($"Channel Name, {channel.Name}, ID {channel.Id}");

            
            //Modify permersions on users channel
           
            await channel.AddPermissionOverwriteAsync(user, PermissionsAnnChannel);

            //Send Welcome Message
            var msg = await channel.SendMessageAsync($"**Welcome {user.Mention} to {user.Guild.Name}!**\n\n" +
                "Everyone on this server is in a channel by themselves and the bot. " +
                "The bot grabs all messages sent by users and sends them to everyone elses channel. " +
                "Becouse the bot posts the message you don't know who sent the message.\n\n" +
                "This bot does not log anything. You can view the source code at https://github.com/doc543/AnonBot \n" +
                "Use \"\\help\" - Brings up help text."
                );
            //await user.Guild.AddPinAsync(channel.Id, msg.Id); //pin msg
            */
            }



    }
} 