using HarmonyLib;
using System.Collections.Generic;

namespace ChatExtensions
{
    [HarmonyPatch(typeof(PLNetworkManager), "ProcessCurrentChatText")]
    class HarmonyProcessCurrentChatText
    {
        private static readonly char[] newline = { '\n', '\r'};
        static void Prefix(PLNetworkManager __instance)
        {
            if (string.IsNullOrWhiteSpace(__instance.CurrentChatText))
            {
                return;
            }

            LinkedListNode<string> lastMessage = Update.chatHistory.FindLast(__instance.CurrentChatText.TrimEnd(newline));
            if (lastMessage != null)
            {
                Update.chatHistory.Remove(lastMessage);
            }
            Update.chatHistory.AddLast(__instance.CurrentChatText.TrimEnd(newline));
            if (Update.chatHistory.Count > 100)
            {
                Update.chatHistory.RemoveFirst();
            }
        }
    }
}
