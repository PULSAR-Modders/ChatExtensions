using HarmonyLib;

namespace ChatExtensions
{
    [HarmonyPatch(typeof(PLInGameUI), "ColoredMsg")]
    class ColoredMessage
    {
        static void Prefix(PLInGameUI __instance, ref string inMsg, bool isShadow, float alpha)
        {
            if (isShadow)
            {
                while (true)
                {
                    int index = inMsg.IndexOf("<color=#");
                    if (index < 0)
                        break;
                    int endIndex = index;
                    for (; true; endIndex++)
                    {
                        if (inMsg[endIndex] == '>')
                        {
                            break;
                        }
                    }
                    inMsg = inMsg.Remove(index, endIndex - index + 1);
                    int index2 = inMsg.IndexOf("</color>");
                    if (index2 < 0)
                        break;
                    inMsg = inMsg.Remove(index2, 8);
                }
            }
        }
    }
}
