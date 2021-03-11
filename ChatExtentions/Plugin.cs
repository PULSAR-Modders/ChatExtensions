using PulsarPluginLoader;

namespace ChatExtentions
{
    public class Plugin : PulsarPlugin
    {
        public override string Version => "0.0.0";

        public override string Author => "18107";

        public override string ShortDescription => base.ShortDescription; //TODO

        public override string Name => "Chat Extentions";

        public override string HarmonyIdentifier()
        {
            return "mod.id107.chatextentions";
        }
    }
}
