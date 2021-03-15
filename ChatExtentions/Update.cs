using HarmonyLib;
using PulsarPluginLoader.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChatExtensions
{
    [HarmonyPatch(typeof(PLNetworkManager), "Update")]
    class Update
    {
        private static LinkedListNode<string> curentHistory = null;

        private static string currentChatText;

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

        static void Prefix(PLNetworkManager __instance)
        {
            if (__instance.IsTyping && HandleChat.cursorPos > 0)
            {
                currentChatText = __instance.CurrentChatText;
                foreach (char c in Input.inputString)
                {
                    if (c == "\b"[0])
                    {
                        currentChatText = currentChatText.Remove(currentChatText.Length - HandleChat.cursorPos - 1, 1);
                    }
                    else if (c == Environment.NewLine[0] || c == "\r"[0])
                    {
                        //Do nothing
                    }
                    else
                    {
                        currentChatText = currentChatText.Insert(currentChatText.Length - HandleChat.cursorPos, c.ToString());
                    }
                }
            }
        }

        static void Postfix(PLNetworkManager __instance)
        {
            if (!__instance.IsTyping)
            {
                curentHistory = null;
                return;
            }

            if (HandleChat.cursorPos > 0)
            {
                __instance.CurrentChatText = currentChatText;
            }

            if (Input.GetKey(KeyCode.LeftControl) &&
                Input.GetKeyDown(KeyCode.V))
            {
                __instance.CurrentChatText += Clipboard.Paste();
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                HandleChat.cursorPos = 0;
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
                HandleChat.cursorPos = 0;
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
