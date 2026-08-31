using HarmonyLib;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.Runtime.Remoting.Messaging;
using TabsBuilderApi.backend;
using TabsBuilderApi.Utils;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.RemoteConfigSettingsHelper;

namespace TabsBuilderApi.Patches
{
    public class PlayerCustomizationMenuBehaviourPatched : MonoBehaviour {
        public Scroller anotherScroller;
        private AspectSpacer aspectSpacer;
        private PlayerCustomizationMenu instance;
        private int orderSize = 0;
        protected Il2CppSystem.Collections.Generic.List<Transform> funSpacingSort()
        {
            var orderedChildren = new Il2CppSystem.Collections.Generic.List<Transform>();

            if (instance)
            {
                if (instance.BackButton != null)
                {
                    orderedChildren.Add(instance.BackButton.transform);
                }

                foreach (var tab in instance.Tabs)
                {
                    if (tab == null)
                        continue;

                    Transform tabTransform = tab.Button.transform?.parent?.parent;

                    if (tabTransform != null && !orderedChildren.Contains(tabTransform))
                    {
                        orderedChildren.Add(tabTransform);
                    }
                }
            };
            // add the rest
            for (int i = 0; i < anotherScroller.Inner.childCount; i++)
            {
                Transform child = anotherScroller.Inner.GetChild(i);

                if (!orderedChildren.Contains(child))
                {
                    orderedChildren.Add(child);
                }
            }

            orderedChildren.Remove(instance.glyphR.transform);
            orderedChildren.Add(instance.glyphR.transform);

            orderedChildren.Remove(instance.glyphL.transform);
            return orderedChildren;
        }
        public void triggerSpacing()
        {
            if (!aspectSpacer) return;
            if (!anotherScroller || !anotherScroller.Inner) return;
            Camera main = Camera.main;
            if (!main) return;
            if (main.aspect > aspectSpacer.defaultAspectRatio &&!aspectSpacer.spaceWiderAspectRatios)return;
            float num = aspectSpacer.xSpacing *(main.aspect / aspectSpacer.defaultAspectRatio);
            var orderedChildren = funSpacingSort();
            if (orderedChildren.Count == 0) return;
            float num2 = Mathf.Ceil(-orderedChildren.Count / 2f) * num;
            for (int i = 0; i < orderedChildren.Count; i++)
            {
                Transform child = orderedChildren[i];
                child.localPosition = new Vector3(
                    num2 + i * num,
                    child.localPosition.y,
                    child.localPosition.z
                );
            }
            orderSize = orderedChildren.Count;
            float maxScroll = Mathf.Max(
                0f,
                (orderSize - 1) * num
            );

            anotherScroller.SetBoundsMin(0, -maxScroll / 2);
            anotherScroller.SetBoundsMax(0, maxScroll / 2);

            if (instance?.glyphL && instance?.BackButton)
            {
                instance.glyphL.transform.position = new Vector3(
                    instance.BackButton.transform.position.x,
                    instance.glyphL.transform.position.y,
                    instance.glyphL.transform.position.z
                );
            }
        
        }
        public void OpenTab(PlayerCustomizationMenu __instance, InventoryTab tab)
        {
            
            var tabAPIData = tab.transform?.Find("viper.cosmella.tabAPI");
            if (anotherScroller != null &&
                anotherScroller.gameObject != null
                && (ActiveInputManager.currentControlType == ActiveInputManager.InputType.Joystick)) anotherScroller.ScrollPercentX(1f - ((float)__instance.selectedTab / (float)(orderSize)));
            TabsBuilderApi.TabBuilderPlugin.mls.LogMessage($"{(float)__instance.selectedTab / (float)__instance.Tabs.Count} is the %");
            if (tabAPIData != null)
            {
                TabsBuilderApi.backend.ExpandedTabButton ExpandedTabButtonStuffer = tabAPIData.GetComponent<TabsBuilderApi.backend.ExpandedTabButton>();
                if (ExpandedTabButtonStuffer)
                {

                    ExpandedTabButtonStuffer.InvokeAction(__instance);
                }
            }
            ;
            
            var script = tab.TryCast<TabsBuilderApi.Utils.TabScript>();
            if (script)
            {
                script.componentEnabledFinished(__instance);
            }
        }
        public void onSpawn(PlayerCustomizationMenu __instance)
        {
            if (!__instance) return;
            instance = __instance;
    
            TabsBuilderApi.backend.TabRegistry.BuildAll(__instance);

            var header = __instance.transform.FindChild("Header");

            if (header)
            {
                header = header.FindChild("Tabs");
            }

            if (header == null)
            {
                header = __instance.Tabs[1].Button.transform.parent.parent;
            }
            ;
            if (header == null)
            {
                TabsBuilderApi.TabBuilderPlugin.mls.LogFatal("unable to find tabs? 3:");
                return;
            }

            /*this script is the AspectSpacer handler*/
            aspectSpacer = header.gameObject.GetComponent<AspectSpacer>();
            if (aspectSpacer)
            {
                aspectSpacer.enabled = false;
            }

            var scrollObj = header.parent.gameObject;

            BoxCollider2D collider = scrollObj.GetComponent<BoxCollider2D>();

            if (collider == null)
                collider = scrollObj.AddComponent<BoxCollider2D>();


            collider.size = new Vector2(30f, 1f);
            collider.offset = Vector2.zero;

            anotherScroller = scrollObj.AddComponent<Scroller>();

            anotherScroller.allowX = true;
            anotherScroller.allowY = false;
            anotherScroller.DragScrollSpeed = 1f;

            anotherScroller.Colliders = new Il2CppReferenceArray<Collider2D>(new Collider2D[] { collider });
            anotherScroller.Inner = header;

            __instance.BackButton.transform.SetParent(header);
            triggerSpacing();
            anotherScroller.ScrollPercentX(1f);
        }
    }

    [HarmonyPatch(typeof(PlayerCustomizationMenu))]
    public class PlayerCustomizationMenuPatched 
    {

        public static void registerClass()
        {
            Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<TabsBuilderApi.backend.ExpandedTabButton>();
            Il2CppInterop.Runtime.Injection.ClassInjector.RegisterTypeInIl2Cpp<TabsBuilderApi.Patches.PlayerCustomizationMenuBehaviourPatched>();
        }

        [HarmonyPatch(nameof(PlayerCustomizationMenu.Start))]
        [HarmonyPrefix]
        public static bool Start_Prefix(PlayerCustomizationMenu __instance)
        {
            /*
                A better version of TabBuilder.StartCheck
                if (TabBuilder.StartCheck(__instance)) return true;
            */
            if (__instance.GetComponent<PlayerCustomizationMenuBehaviourPatched>())
            {
                return true;
            }
            var PlayerCustomizationMenuBehaviourPatched = __instance?.gameObject?.AddComponent<PlayerCustomizationMenuBehaviourPatched>();
            Prefabs.findChips(__instance);
            PlayerCustomizationMenuBehaviourPatched.onSpawn(__instance);
            return true;
        }
        
 [HarmonyPatch(nameof(PlayerCustomizationMenu.OpenTab))]
 [HarmonyPostfix]
 public static void OpenTab_Postfix(PlayerCustomizationMenu __instance, InventoryTab tab)
 {
     if (!tab) return;
     var PlayerCustomizationMenuBehaviourPatched = __instance?.gameObject?.GetComponent<PlayerCustomizationMenuBehaviourPatched>();
            if (PlayerCustomizationMenuBehaviourPatched)
            {
                PlayerCustomizationMenuBehaviourPatched.OpenTab(__instance, tab);
            }
 }
}
}
