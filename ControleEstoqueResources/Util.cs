using System;
using Windows.UI;

namespace ControleEstoqueResources;

public static class Util
{
    public static Color CreateColorFromHex(string pHexColorCode)
    {
        pHexColorCode = pHexColorCode.Replace("#", string.Empty);
        pHexColorCode = pHexColorCode.Replace("0x", string.Empty);
        byte a = (byte)(Convert.ToUInt32(pHexColorCode.Substring(0, 2), 16));
        byte r = (byte)(Convert.ToUInt32(pHexColorCode.Substring(2, 2), 16));
        byte g = (byte)(Convert.ToUInt32(pHexColorCode.Substring(4, 2), 16));
        byte b = (byte)(Convert.ToUInt32(pHexColorCode.Substring(6, 2), 16));

        return Windows.UI.Color.FromArgb(a, r, g, b);
    }
}
