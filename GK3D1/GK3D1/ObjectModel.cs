using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace GK3D1
{
    class ObjectModel
    {
        public Model Model { get; set; }
        public Texture2D[] Textures { get; set; }
        private ContentManager content;
        private Effect effect;

        public ObjectModel(ContentManager content, Effect effect)
        {
            this.content = content;
            this.effect = effect;
            Model = content.Load<Model>("umpire");
            Textures = new Texture2D[30];
            int i = 0;
            foreach (ModelMesh mesh in Model.Meshes)
                foreach (BasicEffect currentEffect in mesh.Effects)
                    Textures[i++] = currentEffect.Texture;
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
