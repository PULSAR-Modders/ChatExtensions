using HarmonyLib;
using PulsarPluginLoader.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace ChatExtentions
{
    [HarmonyPatch(typeof(PLNetworkManager), "Update")]
    class Update
    {
        private static LinkedListNode<string> curentHistory = null;

        private static void SetChat(PLNetworkManager instance)
        {
            if (curentHistory == null)
            {
                instance.CurrentChatText = "";
            }
            else
            {
                instance.CurrentChatText = curentHistory.Value;
            }
        }

        static void Postfix(PLNetworkManager __instance)
        {
            if (!__instance.IsTyping)
            {
                curentHistory = null;
                return;
            }

            if (Input.GetKey(KeyCode.LeftControl) &&
                Input.GetKeyDown(KeyCode.V))
            {
                __instance.CurrentChatText += Clipboard.Paste();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (curentHistory == null)
                {
                    curentHistory = Global.chatHistory.Last;
                }
                else
                {
                    curentHistory = curentHistory.Previous;
                }
                SetChat(__instance);
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (curentHistory == null)
                {
                    curentHistory = Global.chatHistory.First;
                }
                else
                {
                    curentHistory = curentHistory.Next;
                }
                SetChat(__instance);
            }
        }
    }
}
