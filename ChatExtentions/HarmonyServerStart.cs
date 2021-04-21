using HarmonyLib;

namespace ChatExtensions
{
    [HarmonyPatch(typeof(PLServer), "Start")]
    class HarmonyServerStart
    {
        static void Postfix()
        {
            Update.publicCached = false;
            HandlePublicCommands.RequestPublicCommands();
        }
    }
}
