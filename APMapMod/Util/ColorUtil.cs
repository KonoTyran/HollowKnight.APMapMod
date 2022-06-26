using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace APMapMod.Util;

public class ColorUtil
{
    public static Color GetRandomLightColor()
    {
        // this guarantees that at least ONE value is more than .5 so that we dont generate a color
        // that is very dark.
        var random = new Random(DateTime.Now.Second);
        var colorList = new List<float>
        {
            (float) (random.NextDouble() * .5f + .5f),
            (float) random.NextDouble() * .5f,
            (float) random.NextDouble()
        };
        var randomColorList = colorList.OrderBy(_ => random.Next()).ToList();
        return new Color(randomColorList[0], randomColorList[1], randomColorList[2]);
    }
}