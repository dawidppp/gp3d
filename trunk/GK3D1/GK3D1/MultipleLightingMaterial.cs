using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GK3D1
{
    public class MultiLightingMaterial : Material
    {
        public Vector3 AmbientLightColor { get; set; }
        public Vector3[] LightPosition { get; set; }
        public Vector3[] LightDirection { get; set; }
        public Vector3[] LightColor { get; set; }
        public float ConeAngle { get; set; }
        public float LightFalloff { get; set; }

        public MultiLightingMaterial()
        {
            AmbientLightColor = new Vector3(.1f, .1f, .1f);
            LightDirection = new Vector3[2];
            LightPosition = new Vector3[2];
            LightColor = new Vector3[] { Vector3.One, Vector3.One};
            ConeAngle = 30;
            LightFalloff = 20;
        }
        public override void SetEffectParameters(Effect effect)
        {
            if (effect.Parameters["LightDirection"] != null)
                effect.Parameters["LightDirection"].SetValue(LightDirection);
            if (effect.Parameters["LightColor"] != null)
                effect.Parameters["LightColor"].SetValue(LightColor);
            if (effect.Parameters["AmbientLightColor"] != null)
                effect.Parameters["AmbientLightColor"].SetValue(
                AmbientLightColor);
            if (effect.Parameters["LightPosition"] != null)
                effect.Parameters["LightPosition"].SetValue(LightPosition);
            if (effect.Parameters["LightFalloff"] != null)
                effect.Parameters["LightFalloff"].SetValue(LightFalloff);
            if (effect.Parameters["ConeAngle"] != null)
                effect.Parameters["ConeAngle"].SetValue(
                MathHelper.ToRadians(ConeAngle / 2));
        }
    }
}
