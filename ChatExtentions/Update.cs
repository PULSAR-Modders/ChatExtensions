using HarmonyLib;
using PulsarPluginLoader.Chat.Commands;
using PulsarPluginLoader.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ChatExtensions
{
    [HarmonyPatch(typeof(PLNetworkManager), "Update")]
    class Update
    {
        public static LinkedList<string> chatHistory = new LinkedList<string>();

        private static LinkedListNode<string> curentHistory = null;

        private static string currentChatText;

        private static bool textModified = false;

        private static bool commandsCached = false;
        private static string[] aliases = null;

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

        private static void DeleteSelected()
        {
            int pos;
            int length;
            if (HandleChat.cursorPos < HandleChat.cursorPos2)
            {
                pos = currentChatText.Length - HandleChat.cursorPos2;
                length = HandleChat.cursorPos2 - HandleChat.cursorPos;
            }
            else
            {
                pos = currentChatText.Length - HandleChat.cursorPos;
                length = HandleChat.cursorPos - HandleChat.cursorPos2;
                HandleChat.cursorPos = HandleChat.cursorPos2;
            }
            HandleChat.cursorPos2 = -1;
            currentChatText = currentChatText.Remove(pos, length);
        }

        private static string[] GetCommands()
        {
            if (!commandsCached)
            {
                IEnumerable<IChatCommand> commands = ChatCommandRouter.Instance.GetCommands();
                List<string> listAliases = new List<string>();
                foreach (IChatCommand command in commands)
                {
                    if (!command.PublicCommand())
                    {
                        foreach (string alias in command.CommandAliases())
                        {
                            listAliases.Add(alias);
                        }
                    }
                }
                aliases = listAliases.ToArray();
                commandsCached = true;
            }
            return aliases;
        }

        public static string AutoComplete(string text, string[] aliases)
        {
            string chat = text.Substring(1);
            List<string> matchingCommands = new List<string>();
            int matchCount = 0;
            foreach (string alias in aliases)
            {
                if (alias.ToLower().StartsWith(chat.ToLower()))
                {
                    matchCount++;
                    matchingCommands.Add(alias);
                }
            }
            if (matchCount == 1)
            {
                text = text[0] + matchingCommands[0];
            }
            else if (matchCount > 1)
            {
                foreach (string alias in matchingCommands)
                {
                    Messaging.Notification(text[0] + alias);
                }
            }
            return text;
        }

        static void Prefix(PLNetworkManager __instance)
        {
            currentChatText = __instance.CurrentChatText;
            if (__instance.IsTyping && (HandleChat.cursorPos > 0 || HandleChat.cursorPos2 > 0))
            {
                foreach (char c in Input.inputString)
                {
                    if (c == "\b"[0])
                    {
                        if (HandleChat.cursorPos2 != -1 && HandleChat.cursorPos2 != HandleChat.cursorPos)
                        {
                            DeleteSelected();
                        }
                        else
                        {
                            currentChatText = currentChatText.Remove(currentChatText.Length - HandleChat.cursorPos - 1, 1);
                        }
                        textModified = true;
                    }
                    else if (c == Environment.NewLine[0] || c == "\r"[0])
                    {
                        //Do nothing
                    }
                    else
                    {
                        if (HandleChat.cursorPos2 != -1 && HandleChat.cursorPos2 != HandleChat.cursorPos)
                        {
                            DeleteSelected();
                        }
                        currentChatText = currentChatText.Insert(currentChatText.Length - HandleChat.cursorPos, c.ToString());
                        textModified = true;
                    }
                }
                if (Input.GetKeyDown(KeyCode.Delete))
                {
                    if (HandleChat.cursorPos2 != -1 && HandleChat.cursorPos2 != HandleChat.cursorPos)
                    {
                        DeleteSelected();
                    }
                    else
                    {
                        currentChatText = currentChatText.Remove(currentChatText.Length - HandleChat.cursorPos, 1);
                        HandleChat.cursorPos--;
                    }
                    textModified = true;
                }
            }
            else if (__instance.IsTyping)
            {
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    if (currentChatText.StartsWith("/"))
                    {
                        currentChatText = AutoComplete(currentChatText, GetCommands());
                        textModified = true;
                    }
                    else if (currentChatText.StartsWith("!"))
                    {
                        HandlePublicCommands.RequestPublicCommands();
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

            if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
            {
                if (Input.GetKeyDown(KeyCode.V))
                {
                    if (HandleChat.cursorPos2 != -1 && HandleChat.cursorPos2 != HandleChat.cursorPos)
                    {
                        DeleteSelected();
                    }
                    currentChatText = currentChatText.Insert(currentChatText.Length - HandleChat.cursorPos, GUIUtility.systemCopyBuffer);
                    textModified = true;
                }
                if (Input.GetKeyDown(KeyCode.C) && HandleChat.cursorPos2 != -1 && HandleChat.cursorPos2 != HandleChat.cursorPos)
                {
                    int pos;
                    int length;
                    if (HandleChat.cursorPos < HandleChat.cursorPos2)
                    {
                        pos = currentChatText.Length - HandleChat.cursorPos2;
                        length = HandleChat.cursorPos2 - HandleChat.cursorPos;
                    }
                    else
                    {
                        pos = currentChatText.Length - HandleChat.cursorPos;
                        length = HandleChat.cursorPos - HandleChat.cursorPos2;
                    }
                    GUIUtility.systemCopyBuffer = currentChatText.Substring(pos, length);
                }
                if (Input.GetKeyDown(KeyCode.X) && HandleChat.cursorPos2 != -1 && HandleChat.cursorPos2 != HandleChat.cursorPos)
                {
                    int pos;
                    int length;
                    if (HandleChat.cursorPos < HandleChat.cursorPos2)
                    {
                        pos = currentChatText.Length - HandleChat.cursorPos2;
                        length = HandleChat.cursorPos2 - HandleChat.cursorPos;
                    }
                    else
                    {
                        pos = currentChatText.Length - HandleChat.cursorPos;
                        length = HandleChat.cursorPos - HandleChat.cursorPos2;
                    }
                    GUIUtility.systemCopyBuffer = currentChatText.Substring(pos, length);
                    DeleteSelected();
                    textModified = true;
                }
                if (Input.GetKeyDown(KeyCode.A))
                {
                    HandleChat.cursorPos = 0;
                    HandleChat.cursorPos2 = currentChatText.Length;
                }
            }

            if (textModified)
            {
                textModified = false;
                __instance.CurrentChatText = currentChatText;
            }

            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                HandleChat.cursorPos = 0;
                HandleChat.cursorPos2 = -1;
                if (curentHistory == null)
                {
                    curentHistory = chatHistory.Last;
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
                HandleChat.cursorPos2 = -1;
                if (curentHistory == null)
                {
                    curentHistory = chatHistory.First;
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
