using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace TabsBuilderApi.Utils
{
    public static class Prefabs
    {
        public static CubeChip CubeChip;
        public static NameplateChip NameplateChip;
        public static ColorChip BaseChip;
        // get all the chip types because among us was crashing for no reason
        internal static void findChips(PlayerCustomizationMenu menu)
        {
            if (CubeChip && NameplateChip && BaseChip) return;

            var chips = UnityEngine.Object.FindObjectsOfType<GameObject>(true);

            foreach (var chip in chips)
            {
                if (chip != null && chip.gameObject.scene.name == "HideAndDontSave")
                {
                    CubeChip CC = chip.gameObject.GetComponent<CubeChip>();
                    if (CC != null)
                    {
                        if (CubeChip) continue;
                        CubeChip = CC;
                        continue;
                    }
                    NameplateChip NC = chip.gameObject.GetComponent<NameplateChip>();
                    if (CC != null)
                    {
                        if (NameplateChip) continue;
                        NameplateChip = NC;
                        continue;
                    }
                    ColorChip BC = chip.gameObject.GetComponent<ColorChip>();
                    if (BC != null)
                    {
                        if (BaseChip) continue;
                        BaseChip = BC;
                        continue;
                    }
                }
            }

            //if (!NameplateChip) = 

        }
    };
     
};
