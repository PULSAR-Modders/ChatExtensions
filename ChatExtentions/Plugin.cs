using PulsarPluginLoader;

namespace ChatExtentions
{
    public class Plugin : PulsarPlugin
    {
        public static readonly string harmonyIdentifier = "mod.id107.chatextentions";
        public override string Version => "0.0.0";

        public override string Author => "18107";

        public override string ShortDescription => "Adds command/chat history (up/down arrows), pasting in chat, and private messaging";

        public override string Name => "Chat Extentions";

        public override string HarmonyIdentifier()
        {
            return harmonyIdentifier;
        }
    }
}
