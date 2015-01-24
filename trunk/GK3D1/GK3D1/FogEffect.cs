using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GK3D1
{
    class FogEffect
    {
        public static bool IsFogEnabled { get; set; }
        public static int FogStart { get; set; }
        public static int FogEnd { get; set; }
        public static float FogPower { get; set; }

        public FogEffect()
        {
            IsFogEnabled = false;
            FogStart = 800;
            FogEnd = 2000;
            FogPower = 0.5f;
        }

        public void SetParameters(Effect effect)
        {
            if (effect.Parameters["FogEnabled"] != null)
                effect.Parameters["FogEnabled"].SetValue(IsFogEnabled ? 1 : 0);
            if (effect.Parameters["FogStart"] != null)
                effect.Parameters["FogStart"].SetValue(FogStart);
            if (effect.Parameters["FogEnd"] != null)
                effect.Parameters["FogEnd"].SetValue(FogEnd);
            if (effect.Parameters["FogPower"] != null)
                effect.Parameters["FogPower"].SetValue(FogPower);
        }
    }
}
