using PulsarPluginLoader.Chat.Commands;
using PulsarPluginLoader.Utilities;

namespace ChatExtentions
{
    class Command : IChatCommand
    {
        public string[] CommandAliases()
        {
            return new string[] { "pm", "whisper" };
        }

        public string Description()
        {
            return "Sends a direct message to the specified player";
        }

        public bool Execute(string arguments, int SenderID)
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
                Messaging.Echo(PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer(), $"[&%~[C5  You whisper to {player.GetPlayerName()}: {message} ]&%~]");
                PrivateMessage.SendMessage(player.GetPhotonPlayer(), message);
            }
            else
            {
                Messaging.Notification("Could not find the specified player");
            }
            return false;
        }

        public bool PublicCommand()
        {
            return false;
        }

        public string UsageExample()
        {
            return $"/{CommandAliases()[0]} pilot <message>\n" +
                $"/{CommandAliases()[0]} w <message>\n" +
                $"/{CommandAliases()[0]} <name> <message>";
        }
    }
}
