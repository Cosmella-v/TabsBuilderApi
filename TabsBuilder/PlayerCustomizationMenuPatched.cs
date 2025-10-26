using HarmonyLib;
using TabsBuilderApi.Utils;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RemoteConfigSettingsHelper;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using TabsBuilderApi.backend;

namespace TabsBuilderApi.Patches
{
    [HarmonyPatch(typeof(PlayerCustomizationMenu))]
    public static class PlayerCustomizationMenuPatched
    {
        public static TabsBuilderApi.backend.AnotherScroller anotherScroller;

        public static void registerClass()
        {
            Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<TabsBuilderApi.backend.ExpandedTabButton>();
            Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<TabsBuilderApi.backend.AnotherScroller>();
        }

        [HarmonyPatch(nameof(PlayerCustomizationMenu.Start))]
        [HarmonyPrefix]
        public static bool Start_Prefix(PlayerCustomizationMenu __instance)
        {
            if (TabBuilder.StartCheck(__instance)) return true;

            TabsBuilderApi.backend.TabRegistry.BuildAll(__instance);

            return true;
        }

        [HarmonyPatch(nameof(PlayerCustomizationMenu.Start))]
        [HarmonyPostfix]
        public static void Start_Postfix(PlayerCustomizationMenu __instance)
        {
            if (TabBuilder.StartCheck(__instance)) { return; }


            var header = __instance.transform.FindChild("Header");

            if (header)
            {
                header = header.FindChild("Tabs");
            }

            if (header == null)
            {
                header = __instance.Tabs[1].Button.transform.parent.parent;
            };
            if (header == null)
            {
                TabsBuilderApi.TabBuilderPlugin.mls.LogFatal("unable to find tabs? 3:");
                return;
            }

            var AspectSpacer = header.gameObject.GetComponent<AspectSpacer>();
            if (AspectSpacer)
            {
                UnityEngine.Object.Destroy(AspectSpacer);
            }
            var AspectSize = header.gameObject.GetComponent<AspectSize>();
            if (AspectSize)
            {
                UnityEngine.Object.Destroy(AspectSize);
            }
            var scrollObj = header.parent.gameObject;

            BoxCollider2D collider = scrollObj.GetComponent<BoxCollider2D>();
            
            if (collider == null)
                collider = scrollObj.AddComponent<BoxCollider2D>();

            collider.size = new Vector2(30f,1f);
            collider.offset = Vector2.zero;

            anotherScroller = scrollObj.AddComponent<TabsBuilderApi.backend.AnotherScroller>();

            float width = (float)Screen.width / 1920f;

            anotherScroller.allowX = true;
            anotherScroller.allowY = false;
            anotherScroller.DragScrollSpeed = 1f;

            anotherScroller.Colliders = new Il2CppReferenceArray<Collider2D>(new Collider2D[] { collider });
            anotherScroller.Inner = header;

            __instance.BackButton.transform.SetParent(header);
            __instance.glyphL.gameObject.transform.position = new Vector3(width * -4.5f, __instance.glyphL.gameObject.transform.position.y, __instance.glyphL.gameObject.transform.position.z);
            __instance.BackButton.transform.position = new Vector3(__instance.glyphL.gameObject.transform.position.x, __instance.BackButton.transform.position.y, __instance.BackButton.transform.position.z);
            anotherScroller.ScrollPostX(width * -2f);
            anotherScroller.SetBoundsMax(0, 5);
            anotherScroller.SetBoundsMin(0, width * -2f);
            return;
        }

        [HarmonyPatch(nameof(PlayerCustomizationMenu.Update))]
        [HarmonyPostfix]
        public static void Update_Postfix(PlayerCustomizationMenu __instance)
        {
           if (anotherScroller)
            {
               float width = (float)Screen.width / 1920f;
               anotherScroller.SetBoundsMax(0, (width * 4) + (__instance.Tabs.Count - 3) );
               __instance.BackButton.transform.position = new Vector3(__instance.glyphL.gameObject.transform.position.x, __instance.BackButton.transform.position.y, __instance.BackButton.transform.position.z);
            }
        }

        [HarmonyPatch(nameof(PlayerCustomizationMenu.OpenTab))]
        [HarmonyPostfix]
        public static void OpenTab_Postfix(PlayerCustomizationMenu __instance, InventoryTab tab)
        {
            if (!tab) return;
            var tabAPIData = tab.transform.Find("viper.cosmella.tabAPI");
            if (anotherScroller.gameObject && (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Joystick)) anotherScroller.ScrollPostX( (Screen.width / 1920f) + (1 * (__instance.selectedTab - 2) ) );
            if (tabAPIData != null)
            {
                TabsBuilderApi.backend.ExpandedTabButton ExpandedTabButtonStuffer = tabAPIData.GetComponent<TabsBuilderApi.backend.ExpandedTabButton>();
                if (ExpandedTabButtonStuffer)
                {
                    var inner = tab.scroller.Inner;

                    foreach (var child in inner)
                    {
                        var childTransform = child.TryCast<Transform>();
                        if (childTransform != null)
                        {
                            UnityEngine.Object.Destroy(childTransform.gameObject);
                        }

                    }
                    tab.ColorChips.Clear();

                    ExpandedTabButtonStuffer.InvokeAction(__instance);
                }
            };
        }
    }
}
