using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GK3D1
{
    class Bench
    {
        public Model BenchModel { get; set; }
        public Texture2D[] BenchTextures { get; set; }
        private Texture2D[] benchTextures;
        private ContentManager content;
        private Effect effect;

        public Bench(ContentManager content, Effect effect)
        {
            this.content = content;
            this.effect = effect;
            //BenchModel = LoadModel("Bench", out benchTextures);
            BenchModel = content.Load<Model>("refereechair");
            BenchTextures = new Texture2D[50];
            int i = 0;
            foreach (ModelMesh mesh in BenchModel.Meshes)
                foreach (BasicEffect currentEffect in mesh.Effects)
                    BenchTextures[i++] = currentEffect.Texture;
        }

        private Model LoadModel(string assetName, out Texture2D[] textures)
        {

            Model newModel = content.Load<Model>(assetName);
            textures = new Texture2D[7];
            int i = 0;
            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (BasicEffect currentEffect in mesh.Effects)
                    textures[i++] = currentEffect.Texture;

            foreach (ModelMesh mesh in newModel.Meshes)
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                    meshPart.Effect = effect.Clone();

            return newModel;
        }
    }
}
