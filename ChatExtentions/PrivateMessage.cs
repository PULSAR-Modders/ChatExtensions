using PulsarPluginLoader;
using PulsarPluginLoader.Utilities;

namespace ChatExtensions
{
    class PrivateMessage : ModMessage
    {
        private static readonly string harmonyIdentifier = Plugin.harmonyIdentifier;
        private static readonly string handlerIdentifier = "ChatExtensions.PrivateMessage";

        public static void SendMessage(PhotonPlayer player, string message)
        {
            SendRPC(harmonyIdentifier, handlerIdentifier, player, new object[] { message });
        }

        public override void HandleRPC(object[] arguments, PhotonMessageInfo sender)
        {
            string name = PLServer.GetPlayerForPhotonPlayer(sender.sender).GetPlayerName();
            string message = (string)arguments[0];
            Messaging.Echo(PLNetworkManager.Instance.LocalPlayer.GetPhotonPlayer(), $"[&%~[C5  {name} whispers to you: {message} ]&%~]");
        }
    }
}
