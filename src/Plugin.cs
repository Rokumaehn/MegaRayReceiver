using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;

[assembly: AssemblyTitle(MegaRayReceiver.Plugin.NAME)]
[assembly: AssemblyVersion(MegaRayReceiver.Plugin.VERSION)]

namespace MegaRayReceiver
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "Rokumaehn.plugin.MegaRayReceiver";
        public const string NAME = "MegaRayReceiver";
        public const string VERSION = "1.0.0";

        public static ManualLogSource Log;
        static Harmony harmony;

        public void Awake()
        {
            Log = Logger;
            harmony = new Harmony(GUID);

            harmony.PatchAll(typeof(RayReceiverPatch));
        }

#if DEBUG
        public void OnDestroy()
        {
            harmony.UnpatchSelf();
            harmony = null;
        }
#endif
    }

    public class RayReceiverPatch
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerGeneratorComponent), nameof(PowerGeneratorComponent.MaxOutputCurrent_Gamma))]
        static void MegaRayStuff1(ref long __result /*ref PowerGeneratorComponent __instance*/)
        {
            __result = __result * 10L;
        }

        // [HarmonyPrefix]
        // [HarmonyPatch(typeof(PowerGeneratorComponent), nameof(PowerGeneratorComponent.EnergyCap_Gamma_Req))]
        // static bool MegaRayStuff2(ref PowerGeneratorComponent __instance, ref long __result, float sx, float sy, float sz, float increase, float eta)
        // {
        //     float num = (sx * __instance.x + sy * __instance.y + sz * __instance.z + increase * 0.8f + ((__instance.catalystPoint > 0) ? __instance.ionEnhance : 0f)) * 6f + 0.5f;
        //     num = (__instance.currentStrength = ((num > 1f) ? 1f : ((num < 0f) ? 0f : num)));
        //     float num2 = (float)Cargo.accTableMilli[__instance.catalystIncLevel];
        //     __instance.capacityCurrentTick = (long)(__instance.currentStrength * (1f + __instance.warmup * 1.5f) * ((__instance.catalystPoint > 0) ? (2f * (1f + num2)) : 1f) * ((__instance.productId > 0) ? 8f : 1f) * (float)__instance.genEnergyPerTick * 10.0f);
        //     eta = 1f - (1f - eta) * (1f - __instance.warmup * __instance.warmup * 0.4f);
        //     __instance.warmupSpeed = (num - 0.75f) * 4f * 1.3888889E-05f;
        //     __result = (long)((double)__instance.capacityCurrentTick / (double)eta + 0.49999999);
        //     return false;
        // }

        // [HarmonyPrefix]
        // [HarmonyPatch(typeof(PowerGeneratorComponent), nameof(PowerGeneratorComponent.RequiresCurrent_Gamma))]
        // static bool MegaRayStuff3(ref PowerGeneratorComponent __instance, ref long __result, float eta)
        // {
        //     float num = (float)Cargo.accTableMilli[__instance.catalystIncLevel];
        //     long num2 = (long)(__instance.currentStrength * (1f + __instance.warmup * 1.5f) * ((__instance.catalystPoint > 0) ? (2f * (1f + num)) : 1f) * ((__instance.productId > 0) ? 8f : 1f) * (float)__instance.genEnergyPerTick * 10.0f);
        //     eta = 1f - (1f - eta) * (1f - __instance.warmup * __instance.warmup * 0.4f);
        //     __result = (long)((double)num2 / (double)eta + 0.49999999);
        //     return false;
        // }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerGeneratorComponent), nameof(PowerGeneratorComponent.EnergyCap_Gamma_Req))]
        static void MegaRayStuff2post(ref PowerGeneratorComponent __instance, ref long __result, float sx, float sy, float sz, float increase, float eta)
        {
            __instance.capacityCurrentTick = __instance.capacityCurrentTick * 10L;
            __result = __result * 10L;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerGeneratorComponent), nameof(PowerGeneratorComponent.RequiresCurrent_Gamma))]
        static void MegaRayStuff3post(ref PowerGeneratorComponent __instance, ref long __result, float eta)
        {
            __result = __result * 10L;
        }
    }
}
