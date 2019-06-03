using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace Example
{
    class AnonChannelManagement
    {        
        private readonly OverwritePermissions PermissionsDenyAll = new OverwritePermissions(
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

        private readonly OverwritePermissions PermissionsReadOnly = new OverwritePermissions(
            PermValue.Deny,   //PermValue createInstantInvite
            PermValue.Deny,   //PermValue manageChannel
            PermValue.Deny,   //PermValue addReactions
            PermValue.Allow,   //PermValue viewChannel
            PermValue.Deny,   //PermValue sendMessages
            PermValue.Deny,   //PermValue sendTTSMessages
            PermValue.Deny,   //PermValue manageMessages
            PermValue.Deny,   //PermValue embedLinks
            PermValue.Deny,   //PermValue attachFiles  !!!I wish I can attach files idk, will leave this inherit and change it if it works
            PermValue.Allow,   //PermValue readMessageHistory
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

        private readonly ulong CatagoryID = 0;

        public AnonChannelManagement(ulong catagoryID)
        {
            CatagoryID = catagoryID;
        }

        /* **************************************************************
         * Create Anon Channel
         * 1. Creates a new channgel named after the user
         * 2. Checks if a catagory is made if not creates one and sets permissions
         * 4. Sets permissions on channel so the user can only see that channel
         * 5. Send Welcome message and pins it. 
         * ****************************************************************/
        public async Task CreateAnonChannel(SocketGuildUser user)
        {
            var everyoneRole = user.Guild.Roles.FirstOrDefault(x => x.IsEveryone);

            Console.WriteLine($"Creating Channel for {user.Username}! for  Guild {user.Guild.Name}");
            //create text channel
            var text = await user.Guild.CreateTextChannelAsync(user.Id.ToString());

            //find catagory named whatever
            ICategoryChannel category = user.Guild.CategoryChannels.FirstOrDefault(x => x.Id == CatagoryID);
            //if catagory exits make a new one
            if (category == null)
            {
                category = await user.Guild.CreateCategoryChannelAsync("Annoyamus Channels");
                await category.AddPermissionOverwriteAsync(everyoneRole, PermissionsDenyAll);
                
            }

            await text.ModifyAsync(x => x.CategoryId = category.Id);            
            await text.AddPermissionOverwriteAsync(everyoneRole, PermissionsDenyAll);
            await text.AddPermissionOverwriteAsync(user, PermissionsAnnChannel);

            var msg = await text.SendMessageAsync($"**Welcome {user.Mention} to {user.Guild.Name}!**\n\n" +
            "Everyone on this server is in a channel by themselves and the bot. " +
            "The bot grabs all messages sent by users and sends them to everyone elses channel. " +
            "Becouse the bot posts the message you don't know who sent the message.\n\n" +
            "This bot does not log anything. You can view the source code at https://github.com/doc543/AnonBot \n" +
            "Use \"\\help\" - Brings up help text.");
            await msg.PinAsync();
            Console.WriteLine("Created Text Channel: " + CatagoryID.ToString());
        }
        /********************************************************
        * Creates Public Anon Channel
        * ********************************************************/
        public async Task CreatePublicAnonChannel(SocketGuild guild)
        {
            var everyoneRole = guild.Roles.FirstOrDefault(x => x.IsEveryone);

            Console.WriteLine($"Creating PublicChannel for Guild {guild.Name}");
            var text = await guild.CreateTextChannelAsync("Anon");
            ICategoryChannel category = guild.CategoryChannels.FirstOrDefault(x => x.Id == CatagoryID);
            //if catagory exits make a new one
            if (category == null)
            {
                category = await guild.CreateCategoryChannelAsync("Anonyomus Channels");
                await category.AddPermissionOverwriteAsync(everyoneRole, PermissionsDenyAll);
            }

            await text.AddPermissionOverwriteAsync(everyoneRole, PermissionsReadOnly);
            await text.ModifyAsync(x => x.CategoryId = category.Id);

            var message = await text.SendMessageAsync($"**Welcome to {guild.Name}'s Anonamus Channel!**\n\n" +
            "This channel allows all the users on the server to see the anonamous chat without being in it." +
            "The bot grabs all messages sent by users and sends them to everyone elses channel. " +
            "Becouse the bot posts the message you don't know who sent the message.\n\n" +
            "You can view the source code at https://github.com/doc543/AnonBot \n");
            await message.PinAsync();

            Console.WriteLine("Created Text Channel: " + CatagoryID.ToString());
        }

        /********************************************************
        * Remove Public Anon Channel
        * ********************************************************/
        public async Task RemovePublicAnonChannel(SocketGuild guild)
        {
            ulong channelID = 0; //get sql to get channel id of that server

            await guild.GetChannel(channelID).DeleteAsync();

        }

        /********************************************************
        * Removes an Anon channel
        * ********************************************************/
        public async Task RemoveAnnonChannel(SocketGuildUser user)
        {
            foreach (var chan in user.Guild.Channels)
            {
                if (chan.Name == user.Id.ToString())//looks for channel the same name as user ID
                {
                    await chan.DeleteAsync();
                }
            }
        }  
    }
}
