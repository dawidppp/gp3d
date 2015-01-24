using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GK3D1
{
    class TexturedRectangle
    {
        public VertexPositionNormalTexture[] Vertices { get; private set; }
        public int[] Indices { get; private set; }
        public Vector3 Center { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public bool IsTextureBothSides { get; set; }

        private Texture2D texture; 

        public TexturedRectangle( Vector3 center, int width, int height, Texture2D texture, bool isTextureBothSides) 
        {
            
            

            if (!isTextureBothSides)
            {
                Vertices = new VertexPositionNormalTexture[4];
                Indices = new int[6];
            }
            else
            {
                Vertices = new VertexPositionNormalTexture[8];
                Indices = new int[12];
            }
            Center = center;
            Width = width;
            Height = height;
            this.texture = texture;
            IsTextureBothSides = isTextureBothSides;
            SetRectangle();

            if (isTextureBothSides)
            {
                Vertices[4].Position = new Vector3(Center.X, Center.Y + Height / 2, Center.Z + Width / 2);
                Vertices[4].TextureCoordinate = new Vector2(0, 0);
                Vertices[5].Position = new Vector3(Center.X, Center.Y - Height / 2, Center.Z + Width / 2);
                Vertices[5].TextureCoordinate = new Vector2(0, 1);
                Vertices[6].Position = new Vector3(Center.X, Center.Y - Height / 2, Center.Z - Width / 2);
                Vertices[6].TextureCoordinate = new Vector2(1, 1);
                Vertices[7].Position = new Vector3(Center.X, Center.Y + Height / 2, Center.Z - Width / 2);
                Vertices[7].TextureCoordinate = new Vector2(1, 0);

                Indices[6] = 4;
                Indices[7] = 5;
                Indices[8] = 6;
                Indices[9] = 4;
                Indices[10] = 6;
                Indices[11] = 7;
            }
            CalculateNormals(Vertices, Indices);
        }

        private void SetRectangle()
        {
            Vertices[0].Position = new Vector3(Center.X, Center.Y + Height / 2, Center.Z + Width/2);
            Vertices[0].TextureCoordinate = new Vector2(0, 0);
            Vertices[1].Position = new Vector3(Center.X, Center.Y - Height / 2, Center.Z + Width / 2);
            Vertices[1].TextureCoordinate = new Vector2(0, 1);
            Vertices[2].Position = new Vector3(Center.X, Center.Y - Height / 2, Center.Z - Width / 2);
            Vertices[2].TextureCoordinate = new Vector2(1, 1);
            Vertices[3].Position = new Vector3(Center.X, Center.Y + Height / 2, Center.Z - Width / 2);
            Vertices[3].TextureCoordinate = new Vector2(1, 0);

            Indices[0] = 0;
            Indices[1] = 2;
            Indices[2] = 1;
            Indices[3] = 0;
            Indices[4] = 3;
            Indices[5] = 2;
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
    }
}
