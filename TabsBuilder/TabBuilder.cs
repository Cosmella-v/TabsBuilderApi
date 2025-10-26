
using System;
using System.Linq;
using TMPro;
using UnityEngine;
namespace TabsBuilderApi
{
    namespace backend
    {
        /// <summary>
        /// Component attached to API-object for tabs to allow external callbacks.
        /// </summary>
        public class ExpandedTabButton : MonoBehaviour
        {
            public Action<PlayerCustomizationMenu>? action;
            public void InvokeAction(PlayerCustomizationMenu menu)
            {
                action?.Invoke(menu);
            }
        }
    }

    namespace Utils
    {
        /// <summary>
        /// TabBuilder class
        /// </summary>
        public class TabBuilder
        {
        /// <summary>
        /// patching "PlayerCustomizationMenu.Start" Checks if it has already ran before
        /// </summary>
        public static bool StartCheck(PlayerCustomizationMenu __instance)
            {
                if (PlayerCustomizationMenu.Instance && PlayerCustomizationMenu.Instance != __instance)
                {
                    return true;
                }
                return false;
            }
            private PlayerCustomizationMenu menu;
            private InventoryTab? tab;
            private Transform? parent;
            private Transform? buttonParent;
            private PassiveButton? topButton;
            private Action<PlayerCustomizationMenu>? action;
            public int? insertIndex = null;

            /// <summary>
            /// API for creating custom PlayerCustomizationMenu tabs
            /// </summary>
            public TabBuilder(PlayerCustomizationMenu menu)
            {
                this.menu = menu;
                parent = menu.Tabs[0].Tab.transform.parent;
                buttonParent = menu.Tabs[0].Button.transform.parent.parent.parent;
            }

            /// <summary>
            /// Creates a new tab.
            /// </summary>
            public TabBuilder CreateTab(string tabName)
            {
                tab = UnityEngine.Object.Instantiate(menu.Tabs[2].Tab, parent).TryCast<InventoryTab>();
                if (tab == null) return this;
                tab.name = tabName;
                tab.gameObject.SetActive(false);
                return this;
            }

            /// <summary>
            /// Replace the InventoryTab script with a custom one.
            /// </summary>
            public TabBuilder ReplaceScript(Il2CppSystem.Type type)
            {
                if (tab == null) return this;

                var name = tab.name;
                Scroller scroller = tab.scroller;
                ColorChip ColorTabPrefab = tab.ColorTabPrefab;
                PoolablePlayer PlayerPreview = tab.PlayerPreview;

                var gObject = tab.gameObject;

                UnityEngine.Object.Destroy(tab);

                tab = gObject.AddComponent(type).TryCast<InventoryTab>();
                if (tab == null) return this;
                tab.scroller = scroller;
                tab.PlayerPreview = PlayerPreview;
                tab.ColorTabPrefab = ColorTabPrefab;

                tab.YStart = tab.YStart;
                tab.YOffset = tab.YOffset;
                tab.NumPerRow = tab.NumPerRow;
                tab.XRange = tab.XRange;


                tab.name = name;

                return this;
            }
            /// <summary>
            /// Creates the top button for the tab.
            /// </summary>
            public TabBuilder CreateTop(Sprite assetImage)
            {
                if (tab == null) return this;
                var topObj = UnityEngine.Object.Instantiate(menu.Tabs[1].Button.transform.parent.parent, buttonParent);
                topObj.position = menu.Tabs[1].Button.transform.parent.parent.position;
                topObj.position = new Vector3(-10000000000000000000f, topObj.position.y, topObj.position.z);
                topObj.name = tab.name;

                var gchild = topObj.GetChild(0);
                var bg = gchild.Find("Tab Background").GetComponent<PassiveButton>();
                bg.OnClick = new();
                Action actionOpen = () => OpenTab();
                bg.OnClick.AddListener(actionOpen);
                topButton = bg;

                if (assetImage)
                {
                    var image = gchild.Find("Icon").GetComponent<SpriteRenderer>();
                    image.sprite = assetImage;
                }

                return this;
            }
            /// <summary>
            /// Insert the tab before a specific existing tab.
            /// </summary>
            public TabBuilder Before(string existingTabName)
            {
                insertIndex = null;
                for (int i = 0; i < menu.Tabs.Length; i++)
                {
                    if (menu.Tabs[i].Tab.name == existingTabName)
                    {
                        insertIndex = i;
                        break;
                    }
                }
                return this;
            }
            /// <summary>
            /// Insert the tab after a specific existing tab.
            /// </summary>
            public TabBuilder After(string existingTabName)
            {
                insertIndex = null;
                for (int i = 0; i < menu.Tabs.Length; i++)
                {
                    if (menu.Tabs[i].Tab.name == existingTabName)
                    {
                        insertIndex = i + 1;
                        break;
                    }

                }
                return this;
            }

            /// <summary>
            /// Set a custom action to run when the tab is opened.
            /// </summary>
            public TabBuilder SetAction(Action<PlayerCustomizationMenu> callback)
            {
                action = callback;
                return this;
            }

            /// <summary>
            /// Build the tab and add it to the menu.
            /// </summary>
            public InventoryTab? Build()
            {
                if (tab == null) return null;
                if (topButton == null) return null;
                var tabs = menu.Tabs.ToList();
                GameObject ApiTag = new GameObject("viper.cosmella.tabAPI");
                ApiTag.transform.parent = tab.transform;
                var newTabButton = new TabButton();
                newTabButton.Tab = tab;
                newTabButton.Button = topButton.GetComponent<SpriteRenderer>();
                newTabButton.tabText = tab.transform.Find("Text").GetComponent<TextMeshPro>();
                if (action != null)
                {
                    var x = ApiTag.AddComponent<backend.ExpandedTabButton>();
                    x.action = action;
                }

                if (insertIndex.HasValue)
                    tabs.Insert(insertIndex.Value, newTabButton);
                else
                    tabs.Add(newTabButton);

                menu.Tabs = tabs.ToArray();

                RepositionTabs();

                return tab;
            }

            private void OpenTab()
            {
                if (menu.Tabs[menu.selectedTab].Tab != tab)
                {
                    menu.OpenTab(tab);
                };
            }

            protected void RepositionTabs() {
                float width = (float)Screen.width / 1920f;
                var tabs = menu.Tabs.ToList();
                int count = tabs.Count;
                for (int i = 0; i < count; i++)
                {
                    var buttonObj = tabs[i].Button.transform.parent.parent;
                    if (buttonObj != null)
                    {
                        buttonObj.position = new Vector3( (width * -4.5f) + (1 * (i+1)), buttonObj.position.y, buttonObj.position.z);
                    }
                }
                var GlyR = menu.glyphR;
                if (GlyR)
                {
                    GlyR.transform.position = new Vector3((width * -4.5f) + (1 * (count + 1)), GlyR.transform.position.y, GlyR.transform.position.z);
                };
            }
        }
    }
};