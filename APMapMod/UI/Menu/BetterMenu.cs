using System;
using Modding;
using Satchel.BetterMenus;
using UnityEngine;

namespace APMapMod.UI;

internal static class BetterMenu
{
    private static Menu _menuRef;

    public static MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
    {
        if (_menuRef == null)
            _menuRef = PrepareMenu();
        return _menuRef.GetMenuScreen(modListMenu);
    }

    private static Menu PrepareMenu(){
        return new Menu("Archipelago Map Mod", new Element[]
        {
            new TextPanel("Enter the Red Blue and Green values for your icon color", 800f),
            new CustomSlider(
                "Red",
                r=> APMapMod.GS.IconColor.r = Mathf.RoundToInt(r),
                () => APMapMod.GS.IconColor.r
            ) { minValue = 0, maxValue = 255, wholeNumbers = true},
            new CustomSlider(
                "Green",
                g=> APMapMod.GS.IconColor.g = Mathf.RoundToInt(g) ,
                () => APMapMod.GS.IconColor.g
            ) { minValue = 0, maxValue = 255, wholeNumbers = true},
            new CustomSlider(
            "Blue",
            b=> APMapMod.GS.IconColor.b = Mathf.RoundToInt(b),
            () => APMapMod.GS.IconColor.b
            ) { minValue = 0, maxValue = 255, wholeNumbers = true},
        });
    }
}