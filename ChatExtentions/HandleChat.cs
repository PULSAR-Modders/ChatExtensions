using HarmonyLib;
using PulsarPluginLoader.Utilities;
using System;
using UnityEngine;

namespace ChatExtensions
{
    [HarmonyPatch(typeof(PLInGameUI), "HandleChat")]
    class HandleChat
    {
        public static int cursorPos = 0;
        public static int cursorPos2 = -1;

        private static bool TagFound(string str, PLNetworkManager networkManager, int pos)
        {
            if (pos >= 0)
            {
                return false;
            }
            bool tagFound = false;
            for (int i = str.Length - pos; i < str.Length; i++)
            {
                if (networkManager.CurrentChatText[i] == '>')
                {
                    tagFound = true;
                    break;
                }
                else if (networkManager.CurrentChatText[i] == '<')
                {
                    break;
                }
            }
            if (tagFound)
            {
                tagFound = false;
                for (int i = str.Length - pos - 1; i >= 0; i--)
                {
                    if (networkManager.CurrentChatText[i] == '<')
                    {
                        tagFound = true;
                        break;
                    }
                    else if (networkManager.CurrentChatText[i] == '>')
                    {
                        break;
                    }
                }
            }
            return tagFound;
        }

        static void Prefix(PLInGameUI __instance, ref bool ___evenChatString, ref string __state)
        {
            PLNetworkManager networkManager = PLNetworkManager.Instance;
            if (networkManager.IsTyping)
            {
                ___evenChatString = false;
                __state = networkManager.CurrentChatText;
                
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    if (cursorPos2 == -1)
                    {
                        cursorPos2 = cursorPos;
                    }
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        if (cursorPos < __state.Length)
                        {
                            cursorPos++;
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        if (cursorPos > 0)
                        {
                            cursorPos--;
                        }
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        cursorPos2 = -1;
                        if (cursorPos < __state.Length)
                        {
                            cursorPos++;
                        }
                    }
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        cursorPos2 = -1;
                        if (cursorPos > 0)
                        {
                            cursorPos--;
                        }
                    }
                }
                
                if (networkManager.CurrentChatText != null)
                {
                    if (TagFound(__state, networkManager, cursorPos) || TagFound(__state, networkManager, cursorPos2))
                    {
                        __instance.ChatLabel.supportRichText = false;
                        __instance.ChatShadowLabel.supportRichText = false;
                        __instance.ChatShadow2Label.supportRichText = false;
                    }
                    else
                    {
                        __instance.ChatLabel.supportRichText = true;
                        __instance.ChatShadowLabel.supportRichText = true;
                        __instance.ChatShadow2Label.supportRichText = true;
                    }
                    networkManager.CurrentChatText = networkManager.CurrentChatText.Insert(__state.Length - cursorPos, DateTime.Now.Millisecond >= 500 ? "|" : " ");
                    if (cursorPos2 != -1 && cursorPos2 != cursorPos)
                    {
                        networkManager.CurrentChatText = networkManager.CurrentChatText.Insert(__state.Length - cursorPos2 + (cursorPos > cursorPos2 ? 1 : 0), DateTime.Now.Millisecond >= 500 ? "¦" : " ");
                    }
                }
            }
            else
            {
                cursorPos = 0;
                __instance.ChatLabel.supportRichText = true;
                __instance.ChatShadowLabel.supportRichText = true;
                __instance.ChatShadow2Label.supportRichText = true;
            }
        }

        static void Postfix(PLInGameUI __instance, ref string __state)
        {
            if (PLNetworkManager.Instance.IsTyping)
            {
                PLNetworkManager.Instance.CurrentChatText = __state;
            }
        }
    }
}
