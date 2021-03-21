using PulsarPluginLoader;
using PulsarPluginLoader.Chat.Commands;
using System.Collections.Generic;

namespace ChatExtensions
{
    class HandlePublicCommands : ModMessage
    {
        private static readonly string harmonyIdentifier = Plugin.harmonyIdentifier;
        private static readonly string handlerIdentifier = "ChatExtensions.HandlePublicCommands";

        private static bool publicCommandsCached = false;
        private static string[] publicCommands = null;

        private static string[] GetPublicCommands()
        {
            if (!publicCommandsCached)
            {
                IEnumerable<IChatCommand> commands = ChatCommandRouter.Instance.GetCommands();
                List<string> aliases = new List<string>();
                foreach (IChatCommand command in commands)
                {
                    if (command.PublicCommand())
                    {
                        foreach (string alias in command.CommandAliases())
                        {
                            aliases.Add(alias);
                        }
                    }
                }
                publicCommands = aliases.ToArray();
                publicCommandsCached = true;
            }
            return publicCommands;
        }

        public static void RequestPublicCommands()
        {
            SendRPC(harmonyIdentifier, handlerIdentifier, PhotonTargets.MasterClient, new object[] { true });
        }

        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            if ((bool)arguments[0])
            {
                if (PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer().IsMasterClient)
                {
                    string[] aliases = GetPublicCommands();
                    if (aliases.Length > 2)
                    {
                        SendRPC(harmonyIdentifier, handlerIdentifier, sender.sender, new object[] { false, aliases });
                    }
                }
            }
            else
            {
                if (PLNetworkManager.Instance.IsTyping && PLNetworkManager.Instance.CurrentChatText.StartsWith("!"))
                {
                    PLNetworkManager.Instance.CurrentChatText = Update.AutoComplete(PLNetworkManager.Instance.CurrentChatText, (string[])arguments[1]);
                }
            }
        }
    }
}
