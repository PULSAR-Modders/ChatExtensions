using PulsarPluginLoader.Chat.Commands.CommandRouter;
using PulsarPluginLoader.Utilities;

namespace ChatExtensions
{
    class Command : ChatCommand
    {
        public override string[] CommandAliases()
        {
            return new string[] { "pm", "whisper" };
        }

        public override string Description()
        {
            return "Sends a direct message to the specified player";
        }

        public override void Execute(string arguments)
        {
            string arg1 = arguments.Split(' ')[0].ToLower();
            string message = arguments.Substring(arg1.Length);
            PLPlayer player = null;
            switch (arg1)
            {
                case "c":
                case "captain":
                    player = PLServer.Instance.GetCachedFriendlyPlayerOfClass(0);
                    break;
                case "p":
                case "pilot":
                    player = PLServer.Instance.GetCachedFriendlyPlayerOfClass(1);
                    break;
                case "s":
                case "scientist":
                    player = PLServer.Instance.GetCachedFriendlyPlayerOfClass(2);
                    break;
                case "w":
                case "weapons":
                    player = PLServer.Instance.GetCachedFriendlyPlayerOfClass(3);
                    break;
                case "e":
                case "engineer":
                    player = PLServer.Instance.GetCachedFriendlyPlayerOfClass(4);
                    break;
                default:
                    foreach (PLPlayer p in PLServer.Instance.AllPlayers)
                    {
                        if (p != null && p.GetPlayerName().ToLower().StartsWith(arg1))
                        {
                            player = p;
                            break;
                        }
                    }
                    break;
            }

            if (player != null)
            {
                if (!player.IsBot)
                {
                    Messaging.Echo(PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer(), $"<color=#a0a0a0>You whisper to</color> [&%~[C{player.GetClassID()} {player.GetPlayerName()} ]&%~]<color=#a0a0a0>: {message}</color>");
                    PrivateMessage.SendMessage(player.GetPhotonPlayer(), message);
                }
                else
                {
                    Messaging.Notification("Can't send messages to bots");
                }
            }
            else
            {
                Messaging.Notification("Could not find the specified player");
            }
        }

        public override string[] UsageExamples()
        {
            return new string[] {$"/{CommandAliases()[0]} pilot <message>",
                $"/{CommandAliases()[0]} w <message>",
                $"/{CommandAliases()[0]} <name> <message>" };
        }
    }
}
