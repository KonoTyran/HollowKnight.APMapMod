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
            new TextPanel("Enter the Red Green and Blue values for your icon color", 800f),
            new CustomSlider(
                "Red",
                r=> APMapMod.GS.IconColorR = Mathf.RoundToInt(r),
                () => APMapMod.GS.IconColorR,
                0,
                255,
                true
                ),
            new CustomSlider(
                "Green",
                g=> APMapMod.GS.IconColorG = Mathf.RoundToInt(g) ,
                () => APMapMod.GS.IconColorG,
                0,
                255,
                true
                ),
            new CustomSlider(
                "Blue", 
                b=> APMapMod.GS.IconColorB = Mathf.RoundToInt(b), 
                () => APMapMod.GS.IconColorB ,
                0,
                255,
                true
            )
        });
    }
}