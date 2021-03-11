using PulsarPluginLoader;

namespace ChatExtensions
{
    public class Plugin : PulsarPlugin
    {
        public static readonly string harmonyIdentifier = "mod.id107.chatextensions";
        public override string Version => "0.0.1";

        public override string Author => "18107";

        public override string ShortDescription => "Adds command/chat history (up/down arrows), pasting in chat, and private messaging";

        public override string Name => "Chat Extensions";

        public override string HarmonyIdentifier()
        {
            return harmonyIdentifier;
        }
    }
}
