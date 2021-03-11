using HarmonyLib;
using System.Collections.Generic;

namespace ChatExtentions
{
    [HarmonyPatch(typeof(PLNetworkManager), "ProcessCurrentChatText")]
    class ProcessCurrentChatText
    {
        static void Prefix(PLNetworkManager __instance)
        {
            if (string.IsNullOrWhiteSpace(__instance.CurrentChatText))
            {
                return;
            }

            LinkedListNode<string> lastMessage = Global.chatHistory.FindLast(__instance.CurrentChatText);
            if (lastMessage != null)
            {
                Global.chatHistory.Remove(lastMessage);
            }
            Global.chatHistory.AddLast(__instance.CurrentChatText);
            if (Global.chatHistory.Count > 100)
            {
                Global.chatHistory.RemoveFirst();
            }
        }
    }
}
