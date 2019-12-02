using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardColors
{
    static public Color COLORORANGE = new Color(0.522F, 0.690F, 0.592F),
        COLORRED = new Color(1.000F, 0.235F, 0.235F),
        COLORBLUE = new Color(0.176F, 0.365F, 0.376F),
        COLORGREEN = new Color(0.6800F, 0.8113F, 0.1415F),
        COLORPLAYER = new Color(0.2F,0.2F,0.2F);

    static public Color Adjust(Color c)
    {
        float hh, ss, vv;
        Color.RGBToHSV(c, out hh, out ss, out vv);
        ss = 1F - (1F - ss) * (1F - ss);
        return Color.HSVToRGB(hh, ss, vv);
    }

}
