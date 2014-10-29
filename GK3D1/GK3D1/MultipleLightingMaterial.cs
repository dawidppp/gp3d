using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GK3D1
{
    public class MultiLightingMaterial : Material, ICloneable
    {
        public Vector3 AmbientLightColor { get; set; }
        public Vector3[] LightPosition { get; set; }
        public Vector3[] LightDirection { get; set; }
        public Vector3[] LightColor { get; set; }
        public float ConeAngle { get; set; }
        public float LightFalloff { get; set; }
        public float SpecularPower { get; set; }
        public Vector3 SpecularColor { get; set; }

        public Vector3 PointLightPosition { get; set; }
        public Vector3 PointLightColor { get; set; }
        public float PointLightAttenuation { get; set; }
        public float PointLightFalloff { get; set; }
        public float PointLightSpecularPower { get; set; }
        public Vector3 PointLightSpecularColor { get; set; }

        public MultiLightingMaterial()
        {
            AmbientLightColor = new Vector3(.2f, .2f, .2f);
            LightDirection = new Vector3[2];
            LightPosition = new Vector3[2];
            LightColor = new Vector3[] { Vector3.One, Vector3.One };
            ConeAngle = 30;
            LightFalloff = 20;
            SpecularPower = 300;
            SpecularColor = new Vector3(1, 1, 1);
            PointLightPosition = new Vector3(200, -300, 0);
            PointLightColor = new Vector3(1, 0, 0);
            PointLightAttenuation = 900;
            PointLightFalloff = 10;
            PointLightSpecularPower = 300;
            PointLightSpecularColor = new Vector3(1, 1, 1);
        }
        public override void SetEffectParameters(Effect effect)
        {
            if (effect.Parameters["PointLightSpecularColor"] != null)
                effect.Parameters["PointLightSpecularColor"].SetValue(PointLightSpecularColor);
            if (effect.Parameters["PointLightSpecularPower"] != null)
                effect.Parameters["PointLightSpecularPower"].SetValue(PointLightSpecularPower);
            if (effect.Parameters["PointLightFalloff"] != null)
                effect.Parameters["PointLightFalloff"].SetValue(PointLightFalloff);
            if (effect.Parameters["PointLightAttenuation"] != null)
                effect.Parameters["PointLightAttenuation"].SetValue(PointLightAttenuation);
            if (effect.Parameters["PointLightColor"] != null)
                effect.Parameters["PointLightColor"].SetValue(PointLightColor);
            if (effect.Parameters["PointLightPosition"] != null)
                effect.Parameters["PointLightPosition"].SetValue(PointLightPosition);
            if (effect.Parameters["SpecularColor"] != null)
                effect.Parameters["SpecularColor"].SetValue(SpecularColor);
            if (effect.Parameters["SpecularPower"] != null)
                effect.Parameters["SpecularPower"].SetValue(SpecularPower);
            if (effect.Parameters["LightDirection"] != null)
                effect.Parameters["LightDirection"].SetValue(LightDirection);
            if (effect.Parameters["LightColor"] != null)
                effect.Parameters["LightColor"].SetValue(LightColor);
            if (effect.Parameters["AmbientLightColor"] != null)
                effect.Parameters["AmbientLightColor"].SetValue(AmbientLightColor);
            if (effect.Parameters["LightPosition"] != null)
                effect.Parameters["LightPosition"].SetValue(LightPosition);
            if (effect.Parameters["LightFalloff"] != null)
                effect.Parameters["LightFalloff"].SetValue(LightFalloff);
            if (effect.Parameters["ConeAngle"] != null)
                effect.Parameters["ConeAngle"].SetValue(MathHelper.ToRadians(ConeAngle / 2));
        }

        public object Clone()
        {
            var item = new MultiLightingMaterial
                         {
                             AmbientLightColor = AmbientLightColor,
                             ConeAngle = ConeAngle,
                             LightColor = LightColor,
                             LightDirection = LightDirection,
                             LightFalloff = LightFalloff,
                             LightPosition = LightPosition,
                             PointLightAttenuation = PointLightAttenuation,
                             PointLightColor = PointLightColor,
                             PointLightFalloff = PointLightFalloff,
                             PointLightPosition = PointLightPosition,
                             PointLightSpecularColor = PointLightSpecularColor,
                             PointLightSpecularPower = PointLightSpecularPower,
                             SpecularColor = SpecularColor,
                             SpecularPower = SpecularPower
                         };
            return item;
        }
    }
}
