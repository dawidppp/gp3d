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


        public Game1.VertexPositionColorNormal[] Vertices { get; private set; }
        public int[] Indices { get; private set; }

        public Cuboid Net { get; set; }
        public Cuboid LeftPost { get; set; }
        public Cuboid RightPost { get; set; }
        public Bench Bench { get; set; }
        public ObjectModel UmpireChairs { get; set; }

        public VertexPositionNormalTexture[] FieldVertices { get; private set; }
        public int[] FieldIndices { get; private set; }

        public VertexPositionNormalTexture[] FloorVertices { get; private set; }
        public int[] FloorIndices { get; private set; }

        private ContentManager contentManager;

        public Arena(ContentManager contentManager)
        {
            Width = 7200;
            Depth = 4800;
            Height = 2000;
            SetUpVertices();
            //SetUpOuterIndices();
            SetUpInnerIndices();
            this.contentManager = contentManager;
            CourtTexture = contentManager.Load<Texture2D>("court");
            FloorTexture = contentManager.Load<Texture2D>("floor");
            SetField();
            SetFloor();
            Net = new Cuboid(new Vector3(0, -780, 0), 5, 100, 700, false, true, Color.LightGray);
            LeftPost = new Cuboid(new Vector3(0, -850, 350), 10, 300, 10, false, true, Color.LightGray);
            RightPost = new Cuboid(new Vector3(0, -850, -350), 10, 300, 10, false, true, Color.LightGray);
        }

        public void SetUpVertices()
        {
            Vertices = new Game1.VertexPositionColorNormal[8];
            Vertices[0].Position = new Vector3(-Width / 2, -Height / 2, Depth / 2);
            Vertices[0].Color = Color.DarkOrange;
            Vertices[1].Position = new Vector3(Width / 2, -Height / 2, Depth / 2);
            Vertices[1].Color = Color.DarkOrange;
            Vertices[2].Position = new Vector3(Width / 2, -Height / 2, -Depth / 2);
            Vertices[2].Color = Color.DarkOrange;
            Vertices[3].Position = new Vector3(-Width / 2, -Height / 2, -Depth / 2);
            Vertices[3].Color = Color.DarkOrange;

            Vertices[4].Position = new Vector3(-Width / 2, Height / 2, Depth / 2);
            Vertices[4].Color = Color.DarkOrange;
            Vertices[5].Position = new Vector3(Width / 2, Height / 2, Depth / 2);
            Vertices[5].Color = Color.DarkOrange;
            Vertices[6].Position = new Vector3(Width / 2, Height / 2, -Depth / 2);
            Vertices[6].Color = Color.DarkOrange;
            Vertices[7].Position = new Vector3(-Width / 2, Height / 2, -Depth / 2);
            Vertices[7].Color = Color.DarkOrange;
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
            FieldVertices[0].Position = new Vector3(-1000, -Height / 2 + 2, 500);
            FieldVertices[0].TextureCoordinate = new Vector2(0,0);
            //FieldVertices[0].Color = Color.Yellow;
            FieldVertices[1].Position = new Vector3(1000, -Height / 2 + 2, 500);
            FieldVertices[1].TextureCoordinate = new Vector2(0,1);
            //FieldVertices[1].Color = Color.Yellow;
            FieldVertices[2].Position = new Vector3(1000, -Height / 2 + 2, -500);
            FieldVertices[2].TextureCoordinate = new Vector2(1,1);
            //FieldVertices[2].Color = Color.Yellow;
            FieldVertices[3].Position = new Vector3(-1000, -Height / 2 + 2, -500);
            FieldVertices[3].TextureCoordinate = new Vector2(1,0);
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
            FloorVertices[0].Position = Vertices[0].Position + new Vector3(0,1,0);
            FloorVertices[0].TextureCoordinate = new Vector2(0, 0);
            FloorVertices[1].Position = Vertices[1].Position + new Vector3(0, 1, 0);
            FloorVertices[1].TextureCoordinate = new Vector2(0, 1);
            FloorVertices[2].Position = Vertices[2].Position + new Vector3(0, 1, 0);
            FloorVertices[2].TextureCoordinate = new Vector2(1, 1);
            FloorVertices[3].Position = Vertices[3].Position + new Vector3(0, 1, 0);
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

        private void CalculateNormals(VertexPositionNormalTexture[] vertices, int[] indices)
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
