using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GK3D1
{
    class Arena
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Depth { get; private set; }

        public Texture2D CourtTexture { get; set; }
        public Texture2D FloorTexture { get; set; }

        public VertexPositionColorNormal[] Vertices { get; private set; }
        public int[] Indices { get; private set; }
        public Color ArenaWallsColor { get; set; }

        public Cuboid Net { get; set; }
        public Cuboid LeftPost { get; set; }
        public Cuboid RightPost { get; set; }
        public List<Cuboid> Bleachers { get; set; }
        public List<CModel> Models { get; set; }
        public List<CModel> TexturelessModels { get; set; }

        public VertexPositionNormalTexture[] FieldVertices { get; private set; }
        public int[] FieldIndices { get; private set; }

        public VertexPositionNormalTexture[] FloorVertices { get; private set; }
        public int[] FloorIndices { get; private set; }

        public Effect MultipleLightEffectToClone { get; set; }
        public Effect FieldLightEffect { get; set; }
        public Effect FloorLightEffect { get; set; }
        public Effect TexturelessLightEffect { get; set; }
        public MultiLightingMaterial LightMaterialToClone { get; set; }
        public MultiLightingMaterial FloorMaterial { get; set; }

        private ContentManager contentManager;
        private GraphicsDevice device;

        public Arena(ContentManager contentManager, GraphicsDevice device)
        {
            Width = 12000;
            Depth = 10000;
            Height = 4000;
            this.device = device;
            ArenaWallsColor = Color.DimGray;
            SetUpVertices(ArenaWallsColor);
            //SetUpOuterIndices();
            SetUpInnerIndices();
            this.contentManager = contentManager;
            CourtTexture = contentManager.Load<Texture2D>("court");
            FloorTexture = contentManager.Load<Texture2D>("floor");
            SetField();
            SetFloor();
            Models = new List<CModel>();
            TexturelessModels = new List<CModel>();
            MultipleLightEffectToClone = contentManager.Load<Effect>("ManySpotLightEffect");

            Net = new Cuboid(new Vector3(0, -Height / 2 + 480, 0), 5, 200, 1600, false, true, Color.LightGray);
            LeftPost = new Cuboid(new Vector3(0, -Height/2 + 300, 800), 10, 600, 10, false, true, Color.LightGray);
            RightPost = new Cuboid(new Vector3(0, -Height / 2 + 300, -800), 10, 600, 10, false, true, Color.LightGray);

            setEffects();
            setModels();
            Bleachers = new List<Cuboid>();
            setBleachers();
        }

        private void setBleachers()
        {
            int height = 75;
            int width = (int)(Width/1.5f);
            int baseDepth = Depth/4;
            int depthIndent = 200;
            Color color = Color.DarkRed;

            for(int i=0; i<15; i++)
                Bleachers.Add(new Cuboid(new Vector3(0, -Height / 2 - height/2 + i*height, Depth / 3 + i*depthIndent), width, height, baseDepth + i * depthIndent, false, true,
                    color));

            for (int i = 0; i < 15; i++)
                Bleachers.Add(new Cuboid(new Vector3(0, -Height / 2 - height / 2 + i * height, -Depth / 3 - i * depthIndent), width, height, baseDepth + i * depthIndent, false, true,
                    color));

            for (int i = 0; i < 15; i++)
                Bleachers.Add(new Cuboid(new Vector3(-Width / 3 - i * depthIndent, -Height / 5 + i * height, 0), baseDepth / 6 + i * depthIndent, height, width / 2, false, true,
                    color));

            for (int i = 0; i < 15; i++)
                Bleachers.Add(new Cuboid(new Vector3(Width / 3 + i * depthIndent, -Height / 5 + i * height, 0), baseDepth / 6 + i * depthIndent, height, width / 2, false, true,
                    color));
            
        }

        private void setModels()
        {
            var parkbench = contentManager.Load<Model>("parkbench");
            var volleyball = contentManager.Load<Model>("volleyball");
            var spotlight = contentManager.Load<Model>("spotlight");
            var detector = contentManager.Load<Model>("detector_3ds");
            var ship = contentManager.Load<Model>("Models\\p1_wedge");
            var umpire = contentManager.Load<Model>("umpire");
            Models.Add(new CModel(parkbench, new Vector3(0, -Height / 2 + 50, -1600), Vector3.Zero, new Vector3(3f), device));
            Models.Add(new CModel(volleyball, new Vector3(300, -Height / 2, 0), Vector3.Zero, new Vector3(2f), device));
            Models.Add(new CModel(parkbench, new Vector3(0, -Height / 2 + 50, 1600), Vector3.Zero, new Vector3(3f), device));
            Models.Add(new CModel(spotlight, LightMaterialToClone.LightPosition[0] + new Vector3(0, 10, 0), LightMaterialToClone.LightDirection[0], new Vector3(1), device));
            Models.Add(new CModel(spotlight, LightMaterialToClone.LightPosition[1] + new Vector3(0, 10, 0), LightMaterialToClone.LightDirection[0], new Vector3(1), device));
            Models.Add(new CModel(detector, LightMaterialToClone.PointLightPosition + new Vector3(-40, 0, -100), new Vector3(300, 0, 0), new Vector3(4), device));
            Models.Add(new CModel(ship, LightMaterialToClone.PointLightPosition + new Vector3(40, 100, 900), new Vector3(0, 0, 0), new Vector3(0.4f), device));
            //TexturelessModels.Add(new CModel(umpire, new Vector3(0), new Vector3(0, 0, 0), new Vector3(0.2f), device));


            foreach (CModel m in Models)
            {
                m.SetModelEffect(MultipleLightEffectToClone, true);
                m.Material = LightMaterialToClone;
            }

            //foreach (CModel m in TexturelessModels)
            //{
            //    m.SetModelEffect(TexturelessLightEffect, true);
            //    m.Material = LightMaterialToClone;
            //}

            Models[0].Scale = new Vector3(20);
            Models[1].Scale = new Vector3(0.2f);
            Models[2].Scale = new Vector3(20);

            Models[0].Rotation = new Vector3(0, -300, 0);
            Models[2].Rotation = new Vector3(0, 300, 0);
        }

        private void setEffects()
        {
            // base effect and material to be cloned TODO
            MultipleLightEffectToClone.CurrentTechnique = MultipleLightEffectToClone.Techniques["Technique1"];
            LightMaterialToClone = new MultiLightingMaterial();
            LightMaterialToClone.LightDirection[0] = new Vector3(1f, -1, 0);
            LightMaterialToClone.LightDirection[1] = new Vector3(-1f, -10, 0);
            LightMaterialToClone.LightPosition[0] = new Vector3(400, Height / 2 - 200, 0);
            LightMaterialToClone.LightPosition[1] = new Vector3(-400, Height / 2 - 200, 0);
            LightMaterialToClone.PointLightPosition = new Vector3(-Width/7, 0, -Depth/2 + 180);
            LightMaterialToClone.PointLightColor = new Vector3(1, 0, 0);
            LightMaterialToClone.PointLightAttenuation = 1500;
            LightMaterialToClone.SpecularPower = 300;
            LightMaterialToClone.PointLightSpecularColor = new Vector3(1, 0, 0);
            LightMaterialToClone.LightFalloff = 20;
            LightMaterialToClone.ConeAngle = 90f;
            LightMaterialToClone.SetEffectParameters(MultipleLightEffectToClone);

            //field
            FieldLightEffect = MultipleLightEffectToClone.Clone();
            setEffectParameter(FieldLightEffect, "BasicTexture", CourtTexture);
            setEffectParameter(FieldLightEffect, "TextureEnabled", true);

            //floor
            FloorLightEffect = MultipleLightEffectToClone.Clone();
            setEffectParameter(FloorLightEffect, "BasicTexture", FloorTexture);
            setEffectParameter(FloorLightEffect, "TextureEnabled", true);

            //textureless
            TexturelessLightEffect = MultipleLightEffectToClone.Clone();
            TexturelessLightEffect.CurrentTechnique = TexturelessLightEffect.Techniques["Color"];
            setEffectParameter(TexturelessLightEffect, "TextureEnabled", false);
        }

        public void SetUpVertices(Color color)
        {
            Vertices = new VertexPositionColorNormal[8];
            var minus = 100;
            Vertices[0].Position = new Vector3(-Width / 2, -Height / 2 - minus, Depth / 2);
            Vertices[0].Color = color;
            Vertices[1].Position = new Vector3(Width / 2, -Height / 2 - minus, Depth / 2);
            Vertices[1].Color = color;
            Vertices[2].Position = new Vector3(Width / 2, -Height / 2 - minus, -Depth / 2);
            Vertices[2].Color = color;
            Vertices[3].Position = new Vector3(-Width / 2, -Height / 2 - minus, -Depth / 2);
            Vertices[3].Color = color;

            Vertices[4].Position = new Vector3(-Width / 2, Height / 2 - minus, Depth / 2);
            Vertices[4].Color = color;
            Vertices[5].Position = new Vector3(Width / 2, Height / 2 - minus, Depth / 2);
            Vertices[5].Color = color;
            Vertices[6].Position = new Vector3(Width / 2, Height / 2 - minus, -Depth / 2);
            Vertices[6].Color = color;
            Vertices[7].Position = new Vector3(-Width / 2, Height / 2 - minus, -Depth / 2);
            Vertices[7].Color = color;
        }

        private void SetUpOuterIndices()
        {
            Indices = new int[36];
            //bottom
            var side = new int[] { 0, 1, 2, 3, 0 };
            var firstIndex = CreateSideTriangles(side, 0);
            //top
            side = new int[] { 4, 7, 6, 5, 4 };
            firstIndex = CreateSideTriangles(side, firstIndex);
            //left
            side = new int[] { 3, 7, 4, 0, 3 };
            firstIndex = CreateSideTriangles(side, firstIndex);
            //right
            side = new int[] { 1, 5, 6, 2, 1 };
            firstIndex = CreateSideTriangles(side, firstIndex);
            //front
            side = new int[] { 0, 4, 5, 1, 0 };
            firstIndex = CreateSideTriangles(side, firstIndex);
            //back
            side = new int[] { 2, 6, 7, 3, 2 };
            CreateSideTriangles(side, firstIndex);
        }

        private void SetUpInnerIndices()
        {
            Indices = new int[36];
            //bottom
            var side = new int[] { 0, 3, 2, 1, 0 };
            var firstIndex = CreateSideTriangles(side, 0);
            //top
            side = new int[] { 4, 5, 6, 7, 4 };
            firstIndex = CreateSideTriangles(side, firstIndex);
            //left
            side = new int[] { 3, 0, 4, 7, 3 };
            firstIndex = CreateSideTriangles(side, firstIndex);
            //right
            side = new int[] { 1, 2, 6, 5, 1 };
            firstIndex = CreateSideTriangles(side, firstIndex);
            //front
            side = new int[] { 0, 1, 5, 4, 0 };
            firstIndex = CreateSideTriangles(side, firstIndex);
            //back
            side = new int[] { 2, 3, 7, 6, 2 };
            CreateSideTriangles(side, firstIndex);
        }

        private int CreateSideTriangles(int[] side, int firstIndex)
        {
            //int sideIndex = 0;
            int sideIndex = 0;
            for (int i = 0; i < 2; i++)
            {
                Indices[firstIndex++] = side[sideIndex++];
                Indices[firstIndex++] = side[sideIndex++];
                Indices[firstIndex++] = side[sideIndex];
            }
            return firstIndex;
        }

        private void SetField()
        {
            FieldVertices = new VertexPositionNormalTexture[4];
            FieldVertices[0].Position = new Vector3(-Width/5, -Height / 2 + 2, Depth/8);
            FieldVertices[0].TextureCoordinate = new Vector2(0, 0);
            //FieldVertices[0].Color = Color.Yellow;
            FieldVertices[1].Position = new Vector3(Width / 5, -Height / 2 + 2, Depth / 8);
            FieldVertices[1].TextureCoordinate = new Vector2(0, 1);
            //FieldVertices[1].Color = Color.Yellow;
            FieldVertices[2].Position = new Vector3(Width / 5, -Height / 2 + 2, -Depth / 8);
            FieldVertices[2].TextureCoordinate = new Vector2(1, 1);
            //FieldVertices[2].Color = Color.Yellow;
            FieldVertices[3].Position = new Vector3(-Width / 5, -Height / 2 + 2, -Depth / 8);
            FieldVertices[3].TextureCoordinate = new Vector2(1, 0);
            //FieldVertices[3].Color = Color.Yellow;

            FieldIndices = new int[6];
            FieldIndices[0] = 0;
            FieldIndices[1] = 2;
            FieldIndices[2] = 1;
            FieldIndices[3] = 0;
            FieldIndices[4] = 3;
            FieldIndices[5] = 2;

            CalculateNormals(FieldVertices, FieldIndices);
        }

        private void SetFloor()
        {
            FloorVertices = new VertexPositionNormalTexture[4];
            FloorVertices[0].Position = Vertices[0].Position + new Vector3(0, 60, 0);
            FloorVertices[0].TextureCoordinate = new Vector2(0, 0);
            FloorVertices[1].Position = Vertices[1].Position + new Vector3(0, 60, 0);
            FloorVertices[1].TextureCoordinate = new Vector2(0, 1);
            FloorVertices[2].Position = Vertices[2].Position + new Vector3(0, 60, 0);
            FloorVertices[2].TextureCoordinate = new Vector2(1, 1);
            FloorVertices[3].Position = Vertices[3].Position + new Vector3(0, 60, 0);
            FloorVertices[3].TextureCoordinate = new Vector2(1, 0);

            FloorIndices = new int[6];
            FloorIndices[0] = 0;
            FloorIndices[1] = 2;
            FloorIndices[2] = 1;
            FloorIndices[3] = 0;
            FloorIndices[4] = 3;
            FloorIndices[5] = 2;

            CalculateNormals(FloorVertices, FloorIndices);
        }

        public void CalculateNormals(VertexPositionNormalTexture[] vertices, int[] indices)
        {
            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal = new Vector3(0, 0, 0);

            for (int i = 0; i < indices.Length / 3; i++)
            {
                int index1 = indices[i * 3];
                int index2 = indices[i * 3 + 1];
                int index3 = indices[i * 3 + 2];

                Vector3 side1 = vertices[index1].Position - vertices[index3].Position;
                Vector3 side2 = vertices[index1].Position - vertices[index2].Position;
                Vector3 normal = Vector3.Cross(side1, side2);

                vertices[index1].Normal += normal;
                vertices[index2].Normal += normal;
                vertices[index3].Normal += normal;
            }

            for (int i = 0; i < vertices.Length; i++)
                vertices[i].Normal.Normalize();
        }

        void setEffectParameter(Effect effect, string paramName, object val)
        {
            if (effect.Parameters[paramName] == null)
                return;
            if (val is Vector3)
                effect.Parameters[paramName].SetValue((Vector3)val);
            else if (val is bool)
                effect.Parameters[paramName].SetValue((bool)val);
            else if (val is Matrix)
                effect.Parameters[paramName].SetValue((Matrix)val);
            else if (val is Texture2D)
                effect.Parameters[paramName].SetValue((Texture2D)val);
        }
    }
}
