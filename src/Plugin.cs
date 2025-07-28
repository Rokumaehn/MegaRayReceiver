using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;

[assembly: AssemblyTitle(MegaRayReceiver.Plugin.NAME)]
[assembly: AssemblyVersion(MegaRayReceiver.Plugin.VERSION)]

namespace MegaRayReceiver
{
    [BepInPlugin(GUID, NAME, VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public const string GUID = "Rokumaehn.plugin.MegaRayReceiver";
        public const string NAME = "MegaRayReceiver";
        public const string VERSION = "1.1.3";

        public static ConfigEntry<int> EnergyCapMultiplier;
        public static ConfigEntry<bool> AlwaysOn;
        public static ManualLogSource Log;
        static Harmony harmony;

        public void Awake()
        {
            Log = Logger;
            UIPowerPanelPatch.Log = Logger;
            harmony = new Harmony(GUID);

            EnergyCapMultiplier = Config.Bind("RayReceiver", "EnergyCapMultiplier", 10, 
                new ConfigDescription("Requestable Energy Multiplier", new AcceptableValueRange<int>(1, 100)));
            AlwaysOn = Config.Bind("RayReceiver", "AlwaysOn", false,
                new ConfigDescription("Always use full power, ignoring the current strength of the ray receiver", new AcceptableValueList<bool>(false, true)));
            UIPowerPanelPatch.Multiplier = EnergyCapMultiplier.Value;
            UIPowerPanelPatch.AlwaysOn = AlwaysOn.Value;

            harmony.PatchAll(typeof(UIPowerPanelPatch));
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
        //public static long EnergyCapMultiplier = 10L;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerGeneratorComponent), nameof(PowerGeneratorComponent.MaxOutputCurrent_Gamma))]
        static void MegaRayStuff1(ref long __result /*ref PowerGeneratorComponent __instance*/)
        {
            __result = __result * UIPowerPanelPatch.Multiplier;
        }

        // [HarmonyPrefix]
        // [HarmonyPatch(typeof(PowerGeneratorComponent), nameof(PowerGeneratorComponent.EnergyCap_Gamma_Req))]
        // static bool MegaRayStuff2(ref PowerGeneratorComponent __instance, ref long __result, float sx, float sy, float sz, float increase, float eta)
        // {
        //     float num = (sx * __instance.x + sy * __instance.y + sz * __instance.z + increase * 0.8f + ((__instance.catalystPoint > 0) ? __instance.ionEnhance : 0f)) * 6f + 0.5f;
        //     num = (__instance.currentStrength = ((num > 1f) ? 1f : ((num < 0f) ? 0f : num)));
        //     float num2 = (float)Cargo.accTableMilli[__instance.catalystIncLevel];
        //     __instance.capacityCurrentTick = (long)(__instance.currentStrength * (1f + __instance.warmup * 1.5f) * ((__instance.catalystPoint > 0) ? (2f * (1f + num2)) : 1f) * ((__instance.productId > 0) ? 8f : 1f) * (float)__instance.genEnergyPerTick * EnergyCapMultiplier);
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
        //     long num2 = (long)(__instance.currentStrength * (1f + __instance.warmup * 1.5f) * ((__instance.catalystPoint > 0) ? (2f * (1f + num)) : 1f) * ((__instance.productId > 0) ? 8f : 1f) * (float)__instance.genEnergyPerTick * EnergyCapMultiplier);
        //     eta = 1f - (1f - eta) * (1f - __instance.warmup * __instance.warmup * 0.4f);
        //     __result = (long)((double)num2 / (double)eta + 0.49999999);
        //     return false;
        // }

        // [HarmonyPostfix]
        // [HarmonyPatch(typeof(PowerGeneratorComponent), nameof(PowerGeneratorComponent.EnergyCap_Gamma_Req))]
        // static void MegaRayStuff2post(ref PowerGeneratorComponent __instance, ref long __result, float sx, float sy, float sz, float increase, float eta)
        // {
        //     //__instance.capacityCurrentTick = __instance.capacityCurrentTick * UIPowerPanelPatch.Multiplier;
        //     //__result = __result * UIPowerPanelPatch.Multiplier;

        //     float num = __instance.currentStrength = 1.0f;
        //     float num2 = (float)Cargo.accTableMilli[__instance.catalystIncLevel];
        //     __instance.capacityCurrentTick = (long)(__instance.currentStrength * (1f + __instance.warmup * 1.5f) * ((__instance.catalystPoint > 0) ? (2f * (1f + num2)) : 1f) * ((__instance.productId > 0) ? 8f : 1f) * (float)__instance.genEnergyPerTick);
        //     eta = 1f - (1f - eta) * (1f - __instance.warmup * __instance.warmup * 0.4f);
        //     __instance.warmupSpeed = (num - 0.75f) * 4f * 1.3888889E-05f;
        //     __result = (long)((double)__instance.capacityCurrentTick / (double)eta + 0.49999999);

        //     __instance.capacityCurrentTick = __instance.capacityCurrentTick * UIPowerPanelPatch.Multiplier;
        //     __result = __result * UIPowerPanelPatch.Multiplier;
        // }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PowerGeneratorComponent), nameof(PowerGeneratorComponent.EnergyCap_Gamma_Req))]
        static bool MegaRayStuff2pre(ref PowerGeneratorComponent __instance, ref long __result, float sx, float sy, float sz, float increase, float eta)
        {
            //__instance.capacityCurrentTick = __instance.capacityCurrentTick * UIPowerPanelPatch.Multiplier;
            //__result = __result * UIPowerPanelPatch.Multiplier;

            float num;
            if (UIPowerPanelPatch.AlwaysOn)
                num = __instance.currentStrength = 1.0f;
            else
            {
                num = (sx * __instance.x + sy * __instance.y + sz * __instance.z + increase * 0.8f + ((__instance.catalystPoint > 0) ? __instance.ionEnhance : 0f)) * 6f + 0.5f;
                num = (__instance.currentStrength = ((num > 1f) ? 1f : ((num < 0f) ? 0f : num)));
            }

            float num2 = (float)Cargo.accTableMilli[__instance.catalystIncLevel];
            __instance.capacityCurrentTick = (long)(__instance.currentStrength * (1f + __instance.warmup * 1.5f) * ((__instance.catalystPoint > 0) ? (2f * (1f + num2)) : 1f) * ((__instance.productId > 0) ? 8f : 1f) * (float)__instance.genEnergyPerTick);
            eta = 1f - (1f - eta) * (1f - __instance.warmup * __instance.warmup * 0.4f);
            __instance.warmupSpeed = (num - 0.75f) * 4f * 1.3888889E-05f;
            __result = (long)((double)__instance.capacityCurrentTick / (double)eta + 0.49999999);

            __instance.capacityCurrentTick = __instance.capacityCurrentTick * UIPowerPanelPatch.Multiplier;
            __result = __result * UIPowerPanelPatch.Multiplier;
            return false;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(PowerGeneratorComponent), nameof(PowerGeneratorComponent.RequiresCurrent_Gamma))]
        static void MegaRayStuff3post(ref PowerGeneratorComponent __instance, ref long __result, float eta)
        {
            __result = __result * UIPowerPanelPatch.Multiplier;
        }

        // [HarmonyPostfix]
        // [HarmonyPatch(typeof(PowerGeneratorComponent), nameof(PowerGeneratorComponent.EtaCurrent_Gamma))]
        // static void MegaRayStuff4post(ref PowerGeneratorComponent __instance, ref float __result, float eta)
        // {
        //     __result = 1.0f;
        // }

        // [HarmonyPrefix]
        // [HarmonyPatch(typeof(PowerGeneratorComponent), nameof(PowerGeneratorComponent.EnergyCap_Gamma))]
        // static bool MegaRayStuff5pre(ref PowerGeneratorComponent __instance, ref long __result, ref float response)
        // {
        //     response = 1.0f;
        //     return false;
        // }

        // [HarmonyPostfix]
        // [HarmonyPatch(typeof(PowerGeneratorComponent), nameof(PowerGeneratorComponent.GameTick_Gamma))]
        // static void MegaRayStuff6post(ref PowerGeneratorComponent __instance, ref bool useIon, ref bool useCata, ref bool keyFrame, ref PlanetFactory factory, ref int[] productRegister, ref int[] consumeRegister)
        // {
        //     __instance.warmup = 1.0f;
        // }
    }

}
