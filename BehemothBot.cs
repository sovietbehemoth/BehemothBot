using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Entities;
using DSharpPlus.VoiceNext;
using DescriptionAttribute = DSharpPlus.CommandsNext.Attributes.DescriptionAttribute;

namespace DiscordBotAttempt3
{
    public static class Program
    {
        static DiscordClient discord;

        static void Main(string[] args)
        {
            MainAsync(args).ConfigureAwait(false).GetAwaiter().GetResult();
        }
//Configure client
        static async Task MainAsync(string[] args)
        {
            discord = new DiscordClient(new DiscordConfiguration
            {
                Token = TOKENHERE,
                TokenType = TokenType.Bot,
                LogTimestampFormat = "MMM dd yyyy - hh:mm:ss tt",
            });
            //Configure interactivity
            discord.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(30)
            });
            //define prefix
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { "-/" }
            });
            //messages a new user who joins the server
            discord.GuildMemberAdded += async (s, e) =>
            {
                await e.Member.SendMessageAsync($"Welcome to {e.Guild.Name}");
            };
            //error handling
            discord.SocketErrored += async (s, e) =>
            {
                await discord.GetChannelAsync(768501915104968755).Result.SendMessageAsync("Hey rock man, somethings wrong with me");
                await discord.GetChannelAsync(768501915104968755).Result.SendMessageAsync($"Error: {e.Exception}" +
                    $"Handling: {e.Handled}");
            };
            discord.UnknownEvent += async (s, e) =>
            {
                await discord.GetChannelAsync(768501915104968755).Result.SendMessageAsync("Hey rock man, somethings wrong with me");
                await discord.GetChannelAsync(768501915104968755).Result.SendMessageAsync($"{e.EventName} has happened" +
                    $"Handling: {e.Handled}" +
                    $"Info: {e.Json}");
            };
            //alert when client is resumed
            discord.Resumed += async (s, e) =>
            {
                await discord.GetChannelAsync(768501915104968755).Result.SendMessageAsync("Resumed client" +
                    $"Handling: {e.Handled}");
            };
           //command handling 
            commands.RegisterCommands<MyCommands>();

            await discord.ConnectAsync();
            await Task.Delay(-1);

            
        }

    }
    public class MyCommands : BaseCommandModule
    {
        [Command("ban"), Description("Bans specified user")]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task Ban(CommandContext ctx, DiscordMember member, string reason)
        {
        //try catch not necessary but it helps for errors
            try
            {
                await ctx.Guild.BanMemberAsync(member);
                await member.SendMessageAsync($"You have been banned from {ctx.Guild.Name} for {reason}");
                await ctx.RespondAsync($"Successfully banned {member.DisplayName} for {reason}");
            }
            catch
            {
                await ctx.RespondAsync("Error: Could not ban member");
            }
        }

        [Command("unban"), Description("Unbans specified user")]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task Unban(CommandContext ctx, DiscordUser user)
        {
            await ctx.Guild.UnbanMemberAsync(user.Id);
            await ctx.RespondAsync($"Successfully unbanned {user.Username}");
        }

        [Command("check"), Description("Checks specifics about whether BehemothBot is online")]
        public async Task Check(CommandContext ctx)
        {
            await ctx.RespondAsync("If you are only seeing this then BehemothBot is partially offline (JavaScript portion). If you are only seeing this then most commands will work because BehemothBot is mostly built upon a D# client");
        }

        [Command("kick"), Description("Kicks specified user")]
        [RequirePermissions(Permissions.KickMembers)]
        public async Task Kick(CommandContext ctx, DiscordMember member, string reason)
        {
            try
            {
                await member.SendMessageAsync($"You have been kicked from {ctx.Guild.Name}.");
                await ctx.Guild.BanMemberAsync(member);
                await ctx.Guild.UnbanMemberAsync(member.Id, reason);
                await ctx.RespondAsync($"Successfully kicked {member.DisplayName}");
            } catch
            {
                await ctx.RespondAsync("Error: Could not kick member");
            }
        }
//Will ping the JavaScript portion as well
        [Command("ping"), Description("Checks the latency of the client to the server, check if BehemothBot is online")]
        public async Task Ping(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = "BehemothBot",
                Description = $"{ctx.Client.Ping.ToString()} MS",
            };
            await ctx.RespondAsync(embed: embed);          
        }

        [Command("serverinfo"), Description("Provides basic information about the discord server")]
        public async Task Serverinfo(CommandContext ctx)
        {
            var embed = new DiscordEmbedBuilder
            {
                Title = ctx.Guild.Name,
                ImageUrl = ctx.Guild.IconUrl,
                Description = $"This server has {ctx.Guild.MemberCount} members, {ctx.Guild.Channels.Count} channels, and {ctx.Guild.Roles.Count} roles. {ctx.Guild.Name} was created on {ctx.Guild.Owner.JoinedAt.DateTime} and is owned by {ctx.Guild.Owner.DisplayName}",
                Color = DiscordColor.Gold,
            };
            await ctx.RespondAsync(embed: embed);
        }

        [Command("userinfo"), Description("Provides basic information about the discord user, this version of the command requires the users mention.")]
        public async Task Userinfo(CommandContext ctx, DiscordMember member)
        {
            var user = member.Equals(ctx.User);
            //switch not necessary but useful for context
            switch (user)
            {
                case true:
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = member.DisplayName,
                        ImageUrl = member.AvatarUrl,
                        Description = $"Your account was created on {member.CreationTimestamp.DateTime}. You joined this server on {member.JoinedAt.DateTime}",
                        Color = DiscordColor.CornflowerBlue,
                    };
                    await ctx.RespondAsync(embed: embed);
                    break;
                case false:
                    var embed2 = new DiscordEmbedBuilder
                    {
                        Title = member.DisplayName,
                        ImageUrl = member.AvatarUrl,
                        Description = $"This users account was created on {member.CreationTimestamp.DateTime}. They joined this server on {member.JoinedAt.DateTime}",
                        Color = DiscordColor.CornflowerBlue,
                    };
                    await ctx.RespondAsync(embed: embed2);
                    break;
            }
        }
//userinfo2 is used for user IDs
        [Command("userinfo2"), Description("Provides basic information about the discord user, this version of the command requires the users ID.")]
        public async Task Userinfo2(CommandContext ctx, long uid)
        {

            var member = ctx.Guild.GetMemberAsync(Convert.ToUInt64(uid)).Result;

            var self = member.Equals(ctx.User);
            switch (self)
            {
                case true:
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = member.DisplayName,
                        ImageUrl = member.AvatarUrl,
                        Description = $"Your account was created on {member.CreationTimestamp.DateTime}. You joined this server on {member.JoinedAt.DateTime}",
                        Color = DiscordColor.CornflowerBlue,
                    };
                    await ctx.RespondAsync(embed: embed);
                    break;
                case false:
                    var embed2 = new DiscordEmbedBuilder
                    {
                        Title = member.DisplayName,
                        ImageUrl = member.AvatarUrl,
                        Description = $"This users account was created on {member.CreationTimestamp.DateTime}. They joined this server on {member.JoinedAt.DateTime}",
                        Color = DiscordColor.CornflowerBlue,
                    };
                    await ctx.RespondAsync(embed: embed2);
                    break;
            }
        }

        [Command("rng"), Description("Picks a random number out of the parameters chosen. Syntax: -/randomnumber <lowest> <highest>")]
        public async Task Randomnum(CommandContext ctx, int x, int y)
        {
            if (x < y)
            {
                Random r = new Random();
                int randomval = r.Next(x, y);
                var embed = new DiscordEmbedBuilder
                {
                    Title = $"Random Number ({x} - {y})",
                    Description = randomval.ToString(),
                    Color = DiscordColor.Azure,
                };
                await ctx.RespondAsync(embed: embed);
            }
            //error handling
            else if (x == y)
            {
                await ctx.RespondAsync("Error: The two numbers cannot be equal");
            }
            else if (x > y)
            {
                await ctx.RespondAsync("Error: The first number must be less than the second");
            }
        }

        [Command("randomstring"), Description("Will return a completely random collection of letters, numbers, and other characters")]
        public async Task Randomstring(CommandContext ctx)
        {
            //generate capital letters
            string[] pwd1 = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            var pwd1cap = pwd1[new Random().Next(0, pwd1.Length)];
            var pwd1cap2 = pwd1[new Random().Next(0, pwd1.Length)];
            var pwd1cap3 = pwd1[new Random().Next(0, pwd1.Length)];

            var bpwd1cap = pwd1[new Random().Next(0, pwd1.Length)];
            var bpwd1cap2 = pwd1[new Random().Next(0, pwd1.Length)];
            var bpwd1cap3 = pwd1[new Random().Next(0, pwd1.Length)];

            var cpwd1cap = pwd1[new Random().Next(0, pwd1.Length)];
            var cpwd1cap2 = pwd1[new Random().Next(0, pwd1.Length)];
            var cpwd1cap3 = pwd1[new Random().Next(0, pwd1.Length)];

            //generate lowercase letters
            var pwd1low = pwd1[new Random().Next(0, pwd1.Length)];
            var pwd1low2 = pwd1[new Random().Next(0, pwd1.Length)];
            var pwd1low3 = pwd1[new Random().Next(0, pwd1.Length)];

            var bpwd1low = pwd1[new Random().Next(0, pwd1.Length)];
            var bpwd1low2 = pwd1[new Random().Next(0, pwd1.Length)];
            var bpwd1low3 = pwd1[new Random().Next(0, pwd1.Length)];

            var cpwd1low = pwd1[new Random().Next(0, pwd1.Length)];
            var cpwd1low2 = pwd1[new Random().Next(0, pwd1.Length)];
            var cpwd1low3 = pwd1[new Random().Next(0, pwd1.Length)];

            //generate numbers
            string[] pwd2 = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
            var pwd2int = pwd2[new Random().Next(0, pwd2.Length)];
            var pwd2int2 = pwd2[new Random().Next(0, pwd2.Length)];
            var pwd2int3 = pwd2[new Random().Next(0, pwd2.Length)];

            var bpwd2int = pwd2[new Random().Next(0, pwd2.Length)];
            var bpwd2int2 = pwd2[new Random().Next(0, pwd2.Length)];
            var bpwd2int3 = pwd2[new Random().Next(0, pwd2.Length)];

            var cpwd2int = pwd2[new Random().Next(0, pwd2.Length)];
            var cpwd2int2 = pwd2[new Random().Next(0, pwd2.Length)];
            var cpwd2int3 = pwd2[new Random().Next(0, pwd2.Length)];

            //generate special characters
            string[] pwd3 = { "-", "&", "%", "$" };
            var pwd3sp = pwd3[new Random().Next(0, pwd3.Length)];
            var pwd3sp2 = pwd3[new Random().Next(0, pwd3.Length)];
            var pwd3sp3 = pwd3[new Random().Next(0, pwd3.Length)];

            var bpwd3sp = pwd3[new Random().Next(0, pwd3.Length)];
            var bpwd3sp2 = pwd3[new Random().Next(0, pwd3.Length)];
            var bpwd3sp3 = pwd3[new Random().Next(0, pwd3.Length)];

            var cpwd3sp = pwd3[new Random().Next(0, pwd3.Length)];
            var cpwd3sp2 = pwd3[new Random().Next(0, pwd3.Length)];
            var cpwd3sp3 = pwd3[new Random().Next(0, pwd3.Length)];

            //generate random string
            var pwda = pwd1cap + pwd2int + pwd1low.ToLower() + pwd3sp;
            var pwdb = pwd1low2.ToLower() + pwd3sp2 + pwd1cap2 + pwd2int2;
            var pwdc = pwd2int3 + pwd1cap3 + pwd3sp3 + pwd1low3.ToLower();
            string[] pwdrand = { pwda, pwdb, pwdc };

            var pwdd = bpwd1cap + bpwd2int + bpwd1low.ToLower() + bpwd3sp;
            var pwde = bpwd1low2.ToLower() + bpwd3sp2 + bpwd1cap2 + bpwd2int2;
            var pwdf = bpwd2int3 + bpwd1cap3 + bpwd3sp3 + bpwd1low3.ToLower();
            string[] bpwdrand = { pwdd, pwde, pwdf };

            var pwdg = cpwd1cap + cpwd2int + cpwd1low.ToLower() + cpwd3sp;
            var pwdh = cpwd1low2.ToLower() + cpwd3sp2 + cpwd1cap2 + cpwd2int2;
            var pwdi = cpwd2int3 + cpwd1cap3 + cpwd3sp3 + cpwd1low3.ToLower();
            string[] cpwdrand = { pwdg, pwdh, pwdi };

            var pwdrand1 = pwdrand[new Random().Next(0, pwdrand.Length)];
            var pwdrand2 = bpwdrand[new Random().Next(0, bpwdrand.Length)];
            var pwdrand3 = cpwdrand[new Random().Next(0, cpwdrand.Length)];
            var pwd = pwdrand1 + pwdrand2 + pwdrand3;

            //return string
            var embed = new DiscordEmbedBuilder
            {
                Title = "Random String",
                Description = pwd.ToString(),
                Color = DiscordColor.IndianRed,
            };
            await ctx.RespondAsync(embed: embed);
        }

        [Command("math"), Description("Does simple maths, can do division (div), multiplication (mult), subtraction (sub), and addition (add). (Syntax: -/math (operation) (number1) (number2)")]
        public async Task Math(CommandContext ctx, int x, string op, int y)
        {
            switch (op)
            {
                case "*":
                    var product = x * y;
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "Product",
                        Description = product.ToString(),
                        Color = DiscordColor.Azure,
                    };
                    await ctx.RespondAsync(embed: embed);
                    break;
                case "/":
                    var product2 = x / y;
                        var embed2 = new DiscordEmbedBuilder
                        {
                            Title = "Quotient",
                            Description = product2.ToString(),
                            Color = DiscordColor.Azure,
                        };
                        await ctx.RespondAsync(embed: embed2);              
                    break;
                case "+":
                    var sum = x += y;
                    var embed3 = new DiscordEmbedBuilder
                    {
                        Title = "Sum",
                        Description = sum.ToString(),
                        Color = DiscordColor.Azure,
                    };
                    await ctx.RespondAsync(embed: embed3);
                    break;
                case "-":
                    var res = x -= y;
                    var embed4 = new DiscordEmbedBuilder
                    {
                        Title = "Difference",
                        Description = res.ToString(),
                        Color = DiscordColor.Azure,
                    };
                    await ctx.RespondAsync(embed: embed4);
                    break;
                case "^":
                    var pwr = MathF.Pow(x, y);
                    var embed5 = new DiscordEmbedBuilder
                    {
                        Title = "Exponential value",
                        Description = pwr.ToString(),
                        Color = DiscordColor.Azure,
                    };
                    await ctx.RespondAsync(embed: embed5);
                    break;
                default:
                    await ctx.RespondAsync("Error: Argument did not fall under specified operators");
                    break;
            }            
        }
//NOTICE: creates event only reset by restarting client
        [Command("autoresponse"), Description("Automatically sends response to specified message. Syntax: -/autoresponse (word to respond to) (automatic response)")]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task Autoresponse(CommandContext ctx, string target, [RemainingText] string autores)
        {
            await ctx.RespondAsync("Configuring automated response...");
            ctx.Client.MessageCreated += async (e, s) =>
            {
                if (s.Message.Channel.Guild.Equals(ctx.Guild))
                {
                    if (!s.Message.Author.IsBot)
                    {
                        if (s.Message.Content.Equals(target))
                            await s.Message.Channel.SendMessageAsync(autores.ToString());
                    }
                }
            };
        }

//NOTICE: creates event only reset by restarting client
        [Command("blacklist"), Description("Blacklist specified word, if word is entered in chat it will automatically get deleted. If blacklist level is high any words containing the blacklisted word will get deleted. Low level will only delete messages specifically containing that word. Syntax: -/blacklist (word) (level).)")]
        [RequirePermissions(Permissions.BanMembers)]
        public async Task Blacklist(CommandContext ctx, string black, string level)
        {
            switch (level)
            {
                case "low":
                    await ctx.RespondAsync($"{black} is now blacklisted at a low level");
                    ctx.Client.MessageCreated += async (e, s) =>
                    {
                        if (s.Message.Channel.Guild.Equals(ctx.Guild))
                        {
                            if (s.Message.Content.Equals(black))
                                await s.Message.DeleteAsync();
                        }
                    };
                    break;
                case "high":
                        await ctx.RespondAsync($"{black} is now blacklisted at a high level");
                        ctx.Client.MessageCreated += async (e, s) =>
                        {
                            if (s.Message.Channel.Guild.Equals(ctx.Guild))
                            {
                                if (s.Message.Content.Contains(black))
                                    await s.Message.DeleteAsync();
                            }
                        };
                    break;
                default:
                    await ctx.RespondAsync("Error: Level can either be high or low");
                    break;
            }
        }      
//Command used for debugging
        [Command("r"), Description("Send a message as the bot"), RequireOwner]
        [Hidden]
        public async Task Reminder(CommandContext ctx,[RemainingText]string sub)
        {
            await ctx.Client.GetChannelAsync(768501915104968755).Result.SendMessageAsync(sub);
        }

        [Command("snooze"), Description("Sets a 5 minute timer, used in conjunction with the alarm or timer command")]
        [Hidden]
        public async Task Snooze(CommandContext ctx, [RemainingText] string sub)
        {
            await Task.Delay(300000);
            var embed = new DiscordEmbedBuilder
            {
                Title = $"Snooze",
                Description = $"Snooze timer ended",
                Color = DiscordColor.Aquamarine,
            };
            await ctx.RespondAsync(embed: embed);

        }

        [Command("flip"), Description("Flips a coin")]
        public async Task Flip(CommandContext ctx)
        {
            Random r = new Random();
            int randomval = r.Next(0, 100);
            if (randomval <= 50)
            {
                var embed = new DiscordEmbedBuilder
                {
                    Title = $"Coin Flip",
                    Description = $"Heads",
                    Color = DiscordColor.Aquamarine,
                };
                await ctx.RespondAsync(embed: embed);
            }
            else if (randomval > 50 && randomval <= 100)
            {
                var embed2 = new DiscordEmbedBuilder
                {
                    Title = $"Coin Flip",
                    Description = $"Tails",
                    Color = DiscordColor.Aquamarine,
                };
                await ctx.RespondAsync(embed: embed2);
            }
        }

        [Command("timer"), Description("Starts a timer. Syntax: -/timer (time method (sec, min, hour)), time amount (ex; 45, 25)). ")]
        public async Task Ping(CommandContext ctx, string mth, int amt)
        {
            if (amt > 0)
            {
                switch (mth)
                {
                    case "sec":
                        int time = amt * 1000;
                        await ctx.RespondAsync($"Timer set for {amt} seconds");
                        await Task.Delay(time);
                        await ctx.RespondAsync(ctx.User.Mention);
                        var embed = new DiscordEmbedBuilder
                        {
                            Title = $"Timer",
                            Description = $"Timer ended for {amt} seconds",
                            Color = DiscordColor.Aquamarine,
                        };
                        await ctx.RespondAsync(embed: embed);
                        break;
                    case "min":
                        int time2 = amt * 60000;
                        await ctx.RespondAsync($"Timer set for {amt} minutes");
                        await Task.Delay(time2);
                        await ctx.RespondAsync(ctx.User.Mention);
                        var embed2 = new DiscordEmbedBuilder
                        {
                            Title = $"Timer - Minutes",
                            Description = $"Timer ended for {amt} minutes",
                        };
                        await ctx.RespondAsync(embed: embed2);
                        break;
                    case "hour":
                        int time3 = amt * 3600000;
                        await ctx.RespondAsync($"Timer set for {amt} hours");
                        await Task.Delay(time3);
                        await ctx.RespondAsync(ctx.User.Mention);
                        var embed3 = new DiscordEmbedBuilder
                        {
                            Title = $"Timer - Hours",
                            Description = $"Timer ended for {amt} Hours",
                        };
                        await ctx.RespondAsync(embed: embed3);
                        break;
                    default:
                        await ctx.RespondAsync("Error: Time measurement not recognized, use sec, min, or hour.");
                        break;
                }
            }
            //error handling
            else if (amt <= 0)
            {
                await ctx.RespondAsync("Error: Number cannot be negative");
            }
        }

        [Command("nick"), Description("Changes nickname of specified user")]
        public async Task Scrape(CommandContext ctx, DiscordMember member, string nick)
        {
            await ctx.RespondAsync($"Successfully changed {member.Username}s nickname");
        }

        [Command("hash"), Description("Gets a strings hash code. Syntax: -/hash (string)")]
        public async Task Hash(CommandContext ctx, string item)
        {
            await ctx.RespondAsync(item.GetHashCode().ToString());
        }
//handles math regarding one number
        [Command("math2"), Description("Calculates more complicated math. Syntax: -/math2 (operation: Square Root(sqr), Absolute Value(abs), Cosine(cos)) (Number)")]
        public async Task Math2(CommandContext ctx, string op, int x)
        {
           switch (op)
            {
                case "sqr":
                    var sqr = MathF.Sqrt(x);
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = "Square Root",
                        Description = sqr.ToString(),
                        Color = DiscordColor.Azure,
                    };
                    await ctx.RespondAsync(embed: embed);
                    break;
                case "abs":
                    var abs = MathF.Abs(x);
                    var embed2 = new DiscordEmbedBuilder
                    {
                        Title = "Absolute Value",
                        Description = abs.ToString(),
                        Color = DiscordColor.Azure,
                    };
                    await ctx.RespondAsync(embed: embed2);
                    break;
                case "cos":
                    var cos = MathF.Cos(x);
                    var embed3 = new DiscordEmbedBuilder
                    {
                        Title = "Cosine",
                        Description = cos.ToString(),
                        Color = DiscordColor.Azure,
                    };
                    await ctx.RespondAsync(embed: embed3);
                    break;
                default:
                    await ctx.RespondAsync("Error: Operation not recognized");
                    break;
            }
        }

        [Command("dis"), Description("Executes commands as specified user, (-/dis (user) (command)"), RequireOwner]
        public async Task Dis(CommandContext ctx, [Description("Member to execute as")] long uid, [RemainingText, Description("Command text to execute")] string command)
        {
            var member = ctx.Guild.GetMemberAsync(Convert.ToUInt64(uid)).Result;
            var cmds = ctx.CommandsNext;
            await ctx.RespondAsync("Error: This command no longer works");
        }
//used for debugging
        [Command("callsharp"), Description("Executes commands as specified user, (-/dis (user) (command)"), RequireOwner]
        [Hidden]
        public async Task Call(CommandContext ctx)
        {
            await ctx.RespondAsync($"{ctx.Client} is online and working");
        }

        [Command("info"), Description("Displays information about the bot")]
        public async Task Info(CommandContext ctx)
        {
            var embed3 = new DiscordEmbedBuilder
            {
                Title = "BehemothBot Information",
                Description = $"BehemothBot was created by MrBehemoth using C# and JavaScript",
                Color = DiscordColor.Goldenrod,
            };
            await ctx.RespondAsync(embed: embed3);
        }
//used for user ID input, task delays to prevent spam
        [Command("summon2"), Description("Sends a message to a specified user. Syntax: -/summon (USERID) (Method to summon (msg = bot will DM target), (mnt = bot will mention target in text channel)). Command has a delay of 10 seconds to prevent spam.")]
        public async Task Summon2(CommandContext ctx, long uid, string method)
        {
        //get user id
            var member = ctx.Guild.GetMemberAsync(Convert.ToUInt64(uid)).Result;
            //Make sure user isnt a bot and isnt the user who triggered command
            if (member != ctx.User && !member.IsBot)
            {
                switch (method)
                {
                    case "msg":
                        await member.SendMessageAsync($"You have been summoned by {ctx.User.Username} in {ctx.Guild.Name}");
                        await Task.Delay(10000);
                        break;
                    case "mnt":
                        await ctx.RespondAsync($"{member.Mention} You have been summoned by {ctx.User.Username}");
                        await Task.Delay(10000);
                        break;
                    default:
                        await ctx.RespondAsync("Error: Summon method does not exist, use either mnt which mentions the user or msg which messages the user");
                        break;
                }
            }
            else
            {
                await ctx.RespondAsync($"Error: You either tried to summon yourself or you tried to summon a bot.");
                await Task.Delay(3000);
            }
        }

        [Command("summon"), Description("Sends a message to a specified user. Refer to summon2 if you want to use a userID. Syntax: -/summon (User Mention) (Method to summon (msg = bot will DM target), (mnt = bot will mention target in text channel)). Command has a delay of 10 seconds to prevent spam.")]
        public async Task Summon(CommandContext ctx, DiscordMember member, string method)
        {
            if (member != ctx.User && !member.IsBot)
            {
                switch (method)
                {
                    case "msg":
                        await member.SendMessageAsync($"You have been summoned by {ctx.User.Username} in {ctx.Guild.Name}");
                        await Task.Delay(10000);
                        break;
                    case "mnt":
                        await ctx.RespondAsync($"{member.Mention} You have been summoned by {ctx.User.Username}");
                        await Task.Delay(10000);
                        break;
                    default:
                        await ctx.RespondAsync("Error: Summon method does not exist, use either mnt which mentions the user or msg which messages the user");
                        break;
                }
            }
            else
            {
                await ctx.RespondAsync($"Error: You either tried to summon yourself or you tried to summon a bot.");
                await Task.Delay(3000);
            }
        }
//Does not always work, needs more development
        [Command("alarm"), Description("Sets an alarm, only uses military time (24 hour format) Syntax: -/alarm (time), ex: -/alarm (23:43) will ping you at 11:43 PM")]
        public async Task Alarm(CommandContext ctx, string time, [RemainingText] string reason)
        {
        //handles the time input
            var val = time.Split(":");
            int hour = Convert.ToInt32(val[0]);
            var minute = Convert.ToInt32(val[1]);
//make sure time makes sense
            if (hour >= 0 && hour <= 23 && minute >= 0 && minute < 60)
            {
            //gets current time
                var chour = DateTime.Now.Hour;
                var cminute = DateTime.Now.Minute;
                if (minute > cminute)
                {
                //calculate time to wait
                    var waithourms = hour -= chour;
                    var waitminutems = minute -= cminute;
                    await ctx.RespondAsync($"Alarm set, I will ping you in {waithourms} hours and {waitminutems} minutes at {time}");
//convert time to milliseconds
                    var waithour = waithourms * 3600000;
                    var waitminute = waitminutems * 60000;
                    await Task.Delay(waithour + waitminute);
//when alarm sounds
                    await ctx.RespondAsync(ctx.Member.Mention);
                    var embed = new DiscordEmbedBuilder
                    {
                        Title = $"Alarm: {reason}",
                        Description = $"Alarm Sounding for {DateTime.Now.Hour.ToString()}:{DateTime.Now.Minute.ToString()}",
                        Color = DiscordColor.Aquamarine,
                    };
                    await ctx.RespondAsync(embed: embed);
                }
                else if (minute < cminute)
                {
                    var waithourms2 = hour -= chour;
                    var waitminutems2 = cminute -= minute;
                    await ctx.RespondAsync($"Alarm set, I will ping you in {waithourms2} hours and {waitminutems2} minutes at {time}");

                    var waithour2 = waithourms2 * 3600000;
                    var waitminute2 = waitminutems2 * 60000;
                    await Task.Delay(waithour2 + waitminute2);

                    await ctx.RespondAsync(ctx.Member.Mention);
                    var embed2 = new DiscordEmbedBuilder
                    {
                        Title = $"Alarm: {reason}",
                        Description = $"Alarm Sounding for {DateTime.Now.Hour.ToString()}:{DateTime.Now.Minute.ToString()}",
                        Color = DiscordColor.Aquamarine,
                    };
                    await ctx.RespondAsync(embed: embed2);
                }
            }
            //error handling
            else
            {
                await ctx.RespondAsync("Error: You either put a time higher than 23 or a negative time");
            }
        }


/* These remaining commands deal with the BehemothSharpInterpreter, an interpreter for my custom discord programming language. Read about how 
the language works before modifying the interpreter.
"outputon" Refers to whether the bot will output which lines of code it is running through, this is very usefull for debugging
and catching errors to find where exactly went wrong.
*/

//deals with arguments relating to math operations
        [Command("programop"), Description("Program the bot at a low level using operations.")]
        public async Task Prgrmop(CommandContext ctx, string output, string proc, int num1, string op, int num2, string thus, int answer, string then, string conc, [RemainingText] string concresult)
        {
            if (output == "outputon") await ctx.RespondAsync("Bot: Found parameters");          
            //identifies if then statement
            if (proc == "if" && then == "then")
            {
                if (output == "outputon") await ctx.RespondAsync("Bot: If then statement identified");
                //switches which operation is used
                switch (op)
                {
                    case "+":
                        if (output == "outputon") await ctx.RespondAsync("Bot: Operation identified");
                        //perform operation
                        var sum = num1 + num2;
                        //checks if statement is true and the state of the equal sign
                        if (sum == answer && thus == "=")
                        {
                            if (output == "outputon") await ctx.RespondAsync("Bot: Statement is true");
                            //defines what to do next
                            switch (conc)
                            {
                                case "respond":
                                    if (output == "outputon") await ctx.RespondAsync("Bot: Formulating response");
                                    //switches the equals sign
                                    switch (thus)
                                    {
                                        case "=":
                                            if (output == "outputon") await ctx.RespondAsync("Bot: Condition identified");
                                            await ctx.RespondAsync(concresult);
                                            break;
                                        default:
                                            if (output == "outputon") await ctx.RespondAsync("Bot: Condition does not exist");
                                            break;
                                    }
                                    break;
                                case "!respond":
                                    if (output == "outputon") await ctx.RespondAsync("Bot: Ended with order not to respond");
                                    break;
                                default:
                                    await ctx.RespondAsync("BotError: Then statement does not exist");
                                    break;
                            }
                        }
                        else if (thus == "=" && sum != answer && output == "outputon")
                        {
                            await ctx.RespondAsync("Bot: This statement is not true");
                        }
                        //if equals sign is switched
                        else if (thus == "!=")
                        {
                            if (output == "outputon") await ctx.RespondAsync("Bot: Condition identified");
                            if (sum != answer)
                            {
                                switch (conc)
                                {
                                    case "respond":
                                        await ctx.RespondAsync(concresult);
                                        break;
                                    case "!respond":
                                        if (output == "outputon") await ctx.RespondAsync("Bot: Ended with order not to respond");
                                        break;
                                    default:
                                        await ctx.RespondAsync("BotError: Then statement does not exist");
                                        break;
                                }
                            }
                            else
                            {
                                await ctx.RespondAsync("Bot: This statement is not true");
                            }
                        }
                        break;

                            case "*":
                        if (output == "outputon") await ctx.RespondAsync("Bot: Operation identified");
                        var product = num1 * num2;
                        if (product == answer && thus == "=")
                        {
                            if (output == "outputon") await ctx.RespondAsync("Bot: Statement is true");
                            switch (conc)
                            {
                                case "respond":
                                    if (output == "outputon") await ctx.RespondAsync("Bot: Formulating response");
                                    switch (thus)
                                    {
                                        case "=":
                                            if (output == "outputon") await ctx.RespondAsync("Bot: Condition identified");
                                            await ctx.RespondAsync(concresult);
                                            break;
                                        default:
                                            if (output == "outputon") await ctx.RespondAsync("Bot: Condition does not exist");
                                            break;
                                    }
                                    break;
                                case "!respond":
                                    if (output == "outputon") await ctx.RespondAsync("Bot: Ended with order not to respond");
                                    break;
                                default:
                                    await ctx.RespondAsync("BotError: Then statement does not exist");
                                    break;
                            }
                        }
                        else if (thus == "=" && product != answer && output == "outputon")
                        {
                            await ctx.RespondAsync("Bot: This statement is not true");
                        }
                        //if equals sign is inverted
                        else if (thus == "!=")
                        {
                            if (output == "outputon") await ctx.RespondAsync("Bot: Condition identified");
                            if (product != answer)
                            {
                                switch (conc)
                                {
                                    case "respond":
                                        await ctx.RespondAsync(concresult);
                                        break;
                                    case "!respond":
                                        if (output == "outputon") await ctx.RespondAsync("Bot: Ended with order not to respond");
                                        break;
                                    default:
                                        await ctx.RespondAsync("BotError: Then statement does not exist");
                                        break;
                                }
                            }
                            else
                            {
                                await ctx.RespondAsync("Bot: This statement is not true");
                            }
                        }
                        break;
                    default:
                    await ctx.RespondAsync("BotError: Operation does not exist");                      
                        break;
                }
            }
            else if (output == "outputon") await ctx.RespondAsync("BotError: Beginning statement does not exist");
        }
//deals with arguments regarding data types
        [Command("programvar")]
        public async Task Prgrminvar(CommandContext ctx, string output, string datatype, string dataname, string equals, string datatype2, [RemainingText]string dataname2)
        {
            int value;
            if (output == "outputon") await ctx.RespondAsync("Bot: Found parameters");
            if (equals == "=")
            {
                if (output == "outputon") await ctx.RespondAsync("Bot: Expression defined");              
                switch (datatype)
                {
                    case "var":
                        if (output == "outputon") await ctx.RespondAsync("Bot: Data type selected");
                        break;
                    case "int":
                        if (output == "outputon") await ctx.RespondAsync("Bot: Data type selected");
                        break;
                    default:
                        await ctx.RespondAsync("BotError: Data type not found");
                        break;
                }
                switch (datatype2)
                {
                    case "var":
                        if (output == "outputon") await ctx.RespondAsync("Bot: Variable selected");
                        if (!int.TryParse(dataname2, out value))
                        {
                            await ctx.RespondAsync($"Variable {dataname} is string {dataname2}");
                            var result = await ctx.Message.GetNextMessageAsync(m =>
                            {
                                return m.Content.ToLower() == "?" + dataname;
                            });

                            if (!result.TimedOut) await ctx.RespondAsync(dataname2);
                        }
                        var newvar = dataname2;
                        break;
                    case "int":                      
                            if (datatype == "var" && int.TryParse(dataname2, out value))
                        {
                            var newint = dataname2;
                            await ctx.RespondAsync($"Variable {dataname} is integer {dataname2}");
                            var result = await ctx.Message.GetNextMessageAsync(m =>
                            {
                                return m.Content.ToLower() == "?" + dataname;
                            });

                            if (!result.TimedOut) await ctx.RespondAsync(dataname2);
                        }
                        else if (!int.TryParse(dataname2, out value))
                        {
                            await ctx.RespondAsync("BotError: Cannot use a string as an integer");
                        }
                        else if (datatype == "int")
                        {
                            await ctx.RespondAsync("BotError: An integer cannot equal itself, unnecessary assignment of a variable to an integer");
                        }
                        break;
                    case "bool":
                        if (!int.TryParse(dataname, out value))
                        {
                            switch (dataname2)
                            {
                                case "true":
                                    await ctx.RespondAsync($"Variable {dataname} is true");
                                    bool datanameboolval = true;
                                    var result = await ctx.Message.GetNextMessageAsync(m =>
                                    {
                                        return m.Content.ToLower() == "?" + dataname;
                                    });

                                    if (!result.TimedOut) await ctx.RespondAsync(dataname2);
                                    break;
                                case "false":
                                    await ctx.RespondAsync($"Variable {dataname} is false");
                                    bool datanameboolval2 = false;
                                    var result2 = await ctx.Message.GetNextMessageAsync(m =>
                                    {
                                        return m.Content.ToLower() == "?" + dataname;
                                    });

                                    if (!result2.TimedOut) await ctx.RespondAsync(dataname2);
                                    break;
                                default:
                                    await ctx.RespondAsync("BotError: A bool value can only be true or false");
                                    break;
                            }
                        }
                        else if (int.TryParse(dataname, out value))
                        {
                            switch (dataname2)
                            {
                                case "true":
                                    await ctx.RespondAsync($"Integer {dataname} is true");
                                    bool datanameboolval = true;
                                    var result = await ctx.Message.GetNextMessageAsync(m =>
                                    {
                                        return m.Content.ToLower() == "?" + dataname;
                                    });

                                    if (!result.TimedOut) await ctx.RespondAsync(dataname2);
                                    break;
                                case "false":
                                    await ctx.RespondAsync($"Integer {dataname} is false");
                                    bool datanameboolval2 = false;
                                    var result2 = await ctx.Message.GetNextMessageAsync(m =>
                                    {
                                        return m.Content.ToLower() == "?" + dataname;
                                    });

                                    if (!result2.TimedOut) await ctx.RespondAsync(dataname2);
                                    break;
                                default:
                                    await ctx.RespondAsync("BotError: A bool value can only be true or false");
                                    break;
                            }
                        }
                        break;
                }
            }
            else
            {
                await ctx.RespondAsync($"BotError: Invalid expression term '{equals}'");
            }
        }

        [Command("programinq"), Description("Program the bot at a low level using inequalities.")]
        public async Task Prgrminq(CommandContext ctx, string output, string proc, int num1, string which, int num2, string then, string conc,[RemainingText] string concresult)
        {
            if (output == "outputon") await ctx.RespondAsync("Bot: Found parameters");
            
            if (proc == "if" && then == "then")
            {
                if (output == "outputon") await ctx.RespondAsync("Bot: If then statement defined");                
                switch (which)
                {
                    case ">":
                        if (output == "outputon") await ctx.RespondAsync("Bot: Greater than defined");                  
                        if (num1 > num2)
                        {
                            if (output == "outputon") await ctx.RespondAsync("Bot: Statement is true");                          
                            switch (conc)
                            {
                                case "respond":
                                    if (output == "outputon") await ctx.RespondAsync("Bot: Response defined");                                 
                                    await ctx.RespondAsync(concresult);
                                    break;
                                case "!respond":
                                    if (output == "outputon") await ctx.RespondAsync("Bot: Ended with order not to respond");                                   
                                    break;
                                default:
                                    await ctx.RespondAsync("BotError: Then statement does not exist");
                                    break;
                            }
                        }
                        else
                        {
                            if (output == "outputon") await ctx.RespondAsync("Bot: Statement is not true");                           
                        }
                        break;

                    case "<":
                        if (output == "outputon") await ctx.RespondAsync("Bot: Less than defined");
                        
                        if (num1 < num2)
                        {
                            if (output == "outputon") await ctx.RespondAsync("Bot: Statement is true");                       
                            switch (conc)
                            {
                                case "respond":
                                    if (output == "outputon") await ctx.RespondAsync("Bot: Response identified");                                  
                                    await ctx.RespondAsync(concresult);
                                    break;
                                case "!respond":
                                    if (output == "outputon") await ctx.RespondAsync("Bot: Ended with order not to respond");                                  
                                    break;
                                default:
                                    await ctx.RespondAsync("BotError: Then statement does not exist");
                                    break;
                            }
                        }
                        else
                        {
                            if (output == "outputon") await ctx.RespondAsync("Bot: Statement is not true");                           
                        }
                        break;

                    case "<=":
                        if (output == "outputon") await ctx.RespondAsync("Bot: Less than or equal to defined");                        
                        if (num1 <= num2)
                        {
                            if (output == "outputon") await ctx.RespondAsync("Bot: Statement is true");                           
                            switch (conc)
                            {
                                case "respond":
                                    if (output == "outputon") await ctx.RespondAsync("Bot: Response identified");
                                    await ctx.RespondAsync(concresult);
                                    break;
                                case "!respond":
                                    if (output == "outputon") await ctx.RespondAsync("Bot: Ended with order not to respond");
                                    break;
                                default:
                                    await ctx.RespondAsync("BotError: Then statement does not exist");
                                    break;
                            }
                        }
                        else
                        {
                            if (output == "outputon") await ctx.RespondAsync("Bot: Statement is not true");                           
                        }
                        break;

                    case ">=":
                        if (output == "outputon") await ctx.RespondAsync("Bot: Greater than or equal to defined");                       
                        if (num1 >= num2)
                        {
                            if (output == "outputon") await ctx.RespondAsync("Bot: Statement is true");
                            switch (conc)
                            {
                                case "respond":
                                    if (output == "outputon") await ctx.RespondAsync("Bot: Response identified");
                                    await ctx.RespondAsync(concresult);
                                    break;
                                case "!respond":
                                    if (output == "outputon") await ctx.RespondAsync("Bot: Ended with order not to respond");                                  
                                    break;
                                default:
                                    await ctx.RespondAsync("BotError: Then statement does not exist");
                                    break;
                            }
                        }
                        else
                        {
                            if (output == "outputon") await ctx.RespondAsync("Bot: Statement is not true");                           
                        }
                        break;

                    case "=":
                        if (output == "outputon") await ctx.RespondAsync("Bot: Less than defined");
                        if (num1 == num2)
                        {
                            if (output == "outputon") await ctx.RespondAsync("Bot: Statement is true");                         
                            switch (conc)
                            {
                                case "respond":
                                    if (output == "outputon") await ctx.RespondAsync("Bot: Response identified");                                 
                                    await ctx.RespondAsync(concresult);
                                    break;
                                case "!respond":
                                    if (output == "outputon") await ctx.RespondAsync("Bot: Ended with order not to respond");
                                    break;
                                default:
                                    await ctx.RespondAsync("BotError: Then statement does not exist");
                                    break;
                            }
                        }
                        else
                        {
                           if (output == "outputon") await ctx.RespondAsync("Bot: Statement is not true");                           
                        }
                        break;
                }
            }
            else
            {
             await ctx.RespondAsync("BotError: Beginning statement does not exist");                
            }
        }
         [Command("proga2")]
        public async Task Program1(CommandContext ctx, string output, string argument, [RemainingText]string argument2)
        {
            if (output == "1") await ctx.RespondAsync("Bot: Parameters found");
            switch (argument)
            {
                case "print":
                    if (output == "1") await ctx.RespondAsync("Bot: Print argument identified");
                    if (argument2.StartsWith("'") && argument2.EndsWith("'"))
                    {
                        var argument2count = argument2.Length;
                        if (argument2count != 0)
                        {
                            var realargcount2 = argument2count - 2;
                            var printargument = argument2.Substring(1, realargcount2);
                            if (printargument != "@everyone") await ctx.RespondAsync(printargument);
                        }
                        else await ctx.RespondAsync("BehemothSharpInterpreter: Cannot print empty string");
                    }
                    else if (!argument2.StartsWith("'") && !argument2.EndsWith("'"))
                    {
                        switch (argument2)
                        {
                            case var a when argument2.StartsWith("member"):
                                var secondArg = argument2.Split(".")[1];
                                switch (secondArg)
                                {
                                    case var b when secondArg.StartsWith("mention"):
                                        var thirdArg = secondArg.Split("(")[1];
                                        switch (thirdArg)
                                        {
                                            case var c when thirdArg.StartsWith("self") && thirdArg.EndsWith(");"):
                                                await ctx.RespondAsync(ctx.User.Mention);
                                                break;
                                            case var d when thirdArg.StartsWith("@") && thirdArg.EndsWith(");"):
                                                var splituponID = thirdArg.Split("@")[1];
                                                var splituponIDpar = splituponID.Split(")")[0];
                                                try
                                                {
                                                    var memberPrintMention = ctx.Guild.GetMemberAsync(Convert.ToUInt64(splituponIDpar)).Result;
                                                    await ctx.RespondAsync(memberPrintMention.Mention);
                                                }
                                                catch
                                                {
                                                    await ctx.RespondAsync("BehemothSharpInterpreter: Member not found");
                                                }
                                                break;
                                            default:
                                                await ctx.RespondAsync("BehemothSharpInterpreter: Argument not found // Unclosed parenthesis // Missing ;");
                                                break;
                                        }
                                        break;                                
                                }
                                break;
                            case var c when argument2.StartsWith("server"):
                                var secondArg2 = argument2.Split(".")[1];
                                switch (secondArg2)
                                {
                                    case var d when secondArg2.StartsWith("membercount;"):
                                        await ctx.RespondAsync(ctx.Guild.MemberCount.ToString());
                                        break;
                                    default:
                                        await ctx.RespondAsync($"BehemothBotInterpreter: '{secondArg2}' does not exist. Missing ;?");
                                        break;
                                    case var e when secondArg2.StartsWith("ownername;"):
                                        await ctx.RespondAsync(ctx.Guild.Owner.Username);
                                        break;
                                    case var e when secondArg2.StartsWith("ownermention;"):
                                        await ctx.RespondAsync(ctx.Guild.Owner.Mention);
                                        break;
                                    case var f when secondArg2.StartsWith("owner;"):
                                        await ctx.RespondAsync("BehemothBotInterpreter: Incorrect usage of 'owner'");
                                        break;
                                }
                                break;
                            default:
                                await ctx.RespondAsync($"BehemothBotInterpreter: '{argument2}' does not exist");
                                break;
                        }
                    }
                    break;
                default:
                    await ctx.RespondAsync($"BehemothSharpInterpreter: Error '{argument}' does not exist");
                    break;
                case "log":
                    if (argument2.StartsWith("'") && argument2.EndsWith("'"))
                    {
                        var argument2count = argument2.Length;
                        if (argument2count != 0)
                        {
                            var realargcount2 = argument2count - 2;
                            var printargument = argument2.Substring(1, realargcount2);
                            Console.WriteLine(printargument);
                            await ctx.RespondAsync("Logged in console");
                        }
                        else await ctx.RespondAsync("BehemothSharpInterpreter: Cannot print empty string");
                    }
                    break;
            }
        }
        
        [Command("program"), Description("Program the bot at a low level using variables.")]
        public async Task Prgrmvar(CommandContext ctx)
        {
            await ctx.RespondAsync("Initializing programming environment");
            ctx.Client.MessageCreated += async (e, s) =>
            {
                if (s.Message.Channel.Guild.Equals(ctx.Guild))
                {
                    var input = s.Message.Content;
                    var var = input.StartsWith("var");
                    switch (input)
                    {
                        case string n when (var):
                            var split1 = input.Split(" ");
                            var variablename = split1[1];
                            var split2 = variablename.Split(" ");
                            await ctx.RespondAsync($"Variable {variablename} selected");
                            var result = await ctx.Message.GetNextMessageAsync(m =>
                            {
                                var varnextmsg = m.Content.ToLower();

                                var varnextmsginteq = varnextmsg.StartsWith("eq");
                                switch (varnextmsg)
                                {
                                    case string n when (varnextmsginteq):
                                        var split1 = varnextmsg.Split(" ");
                                        var operatoreq = split1[1];
                                        m.Channel.SendMessageAsync(operatoreq);
                                        break;
                                }
                                return varnextmsginteq;
                            });
                            break;
                    }
                }
            };

        }
    }
}
