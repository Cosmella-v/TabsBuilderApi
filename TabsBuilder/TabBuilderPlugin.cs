
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System.Linq;
using TabsBuilderApi.Patches;

namespace TabsBuilderApi
{
    /// <summary>
    /// The main plugin class for tabsbuilder API.
    /// </summary>
    [BepInAutoPlugin("tabsbuilder.api", "TabsBuilderApi")]
    [BepInProcess("Among Us.exe")]
    public partial class TabBuilderPlugin : BasePlugin
    {
        public static ManualLogSource mls;
        internal Harmony Harmony { get; } = new(Id);
        /// <inheritdoc />
        public override void Load()
        {
            mls = BepInEx.Logging.Logger.CreateLogSource(Id);
            mls.LogDebug($"{Id} the api has Awaken");
            Harmony.PatchAll(typeof(TabsBuilderApi.Patches.PlayerCustomizationMenuPatched));
            TabsBuilderApi.Patches.PlayerCustomizationMenuPatched.registerClass();
        }
    };
}