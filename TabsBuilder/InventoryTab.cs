using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TabsBuilderApi
{
    namespace Utils
    {
        public class TabScript : InventoryTab
        {
            public virtual void componentEnabledFinished(PlayerCustomizationMenu instance)
            { }
        };
        /// <summary>
        /// Stub methods namespace. Needed because of Il2Cpp limitations on injected classes.
        /// </summary>
        namespace Stubs
        {
            public static class StubInventoryTab
            {
                [HarmonyReversePatch]
                [HarmonyPatch(typeof(InventoryTab), nameof(InventoryTab.HasLocalPlayer))]
                [MethodImpl(MethodImplOptions.NoInlining)]
                public static bool HasLocalPlayer(InventoryTab instance)
                {
                    return false;
                }

                [HarmonyReversePatch]
                [HarmonyPatch(typeof(InventoryTab), nameof(InventoryTab.GetDisplayColor))]
                [MethodImpl(MethodImplOptions.NoInlining)]
                public static int GetDisplayColor(InventoryTab instance)
                {
                    return 0;
                }

                [HarmonyReversePatch]
                [HarmonyPatch(
                    typeof(InventoryTab),
                    nameof(InventoryTab.UpdateMaterials),
                    typeof(SpriteRenderer),
                    typeof(CosmeticData)
                )]
                [MethodImpl(MethodImplOptions.NoInlining)]
                public static void UpdateMaterials(
                    InventoryTab instance,
                    SpriteRenderer spriteRenderer,
                    CosmeticData data)
                {
                }

                [HarmonyReversePatch]
                [HarmonyPatch(typeof(InventoryTab), nameof(InventoryTab.SetScrollerBounds))]
                [MethodImpl(MethodImplOptions.NoInlining)]
                public static void SetScrollerBounds(InventoryTab instance)
                {
                }

                [HarmonyReversePatch]
                [HarmonyPatch(typeof(InventoryTab), nameof(InventoryTab.ClickEquip))]
                [MethodImpl(MethodImplOptions.NoInlining)]
                public static void ClickEquip(InventoryTab instance)
                {
                }

                [HarmonyReversePatch]
                [HarmonyPatch(typeof(InventoryTab), nameof(InventoryTab.OnEnable))]
                [MethodImpl(MethodImplOptions.NoInlining)]
                public static void OnEnable(InventoryTab instance)
                {
                }

                [HarmonyReversePatch]
                [HarmonyPatch(typeof(InventoryTab), nameof(InventoryTab.OnDisable))]
                [MethodImpl(MethodImplOptions.NoInlining)]
                public static void OnDisable(InventoryTab instance)
                {
                }

                [HarmonyReversePatch]
                [HarmonyPatch(typeof(InventoryTab), nameof(InventoryTab.GetDefaultSelectable))]
                [MethodImpl(MethodImplOptions.NoInlining)]
                public static ColorChip GetDefaultSelectable(InventoryTab instance)
                {
                    return null!;
                }

                [HarmonyReversePatch]
                [HarmonyPatch(typeof(InventoryTab), nameof(InventoryTab.IsSelectedItemEquipped))]
                [MethodImpl(MethodImplOptions.NoInlining)]
                public static bool IsSelectedItemEquipped(InventoryTab instance)
                {
                    return false;
                }

                [HarmonyReversePatch]
                [HarmonyPatch(typeof(InventoryTab), nameof(InventoryTab.GetCurrentProdID))]
                [MethodImpl(MethodImplOptions.NoInlining)]
                public static string GetCurrentProdID(InventoryTab instance)
                {
                    return string.Empty;
                }

                [HarmonyReversePatch]
                [HarmonyPatch(typeof(InventoryTab), nameof(InventoryTab.GetDisplayColor))]
                [MethodImpl(MethodImplOptions.NoInlining)]
                public static int GetDisplayColor()
                {
                    return 0;
                }
            }
        }

        }
}
