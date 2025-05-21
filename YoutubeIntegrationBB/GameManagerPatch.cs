using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
namespace YoutubeIntegrationBB
{
    [HarmonyPatch]
    internal class GameManagerPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(BaseGameManager), "BeginPlay")]
        static void BaseGMPatch(BaseGameManager __instance)
        {

            if (__instance.GetType() != typeof(PitstopGameManager))
            {
                BasePlugin.Instance.CPH.Play();
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(PitstopGameManager), "LoadNextLevel")]
        static void CANTYOUDOONETHINGGMDePatch(BaseGameManager __instance)
        {

            

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(PitstopGameManager), "Initialize")]
        static void CANTYOUDOONETHINGGMDePatch2(BaseGameManager __instance)
        {

            BasePlugin.Instance.CPH.Stop();

        }
    }
}


