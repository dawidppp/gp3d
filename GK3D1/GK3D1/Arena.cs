using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GK3D1
{
    class Arena
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int Depth { get; private set; }

        public Game1.VertexPositionColorNormal[] Vertices { get; private set; }
        public int[] Indices { get; private set; }

        public Game1.VertexPositionColorNormal[] FieldVertices { get; private set; }
        public int[] FieldIndices { get; private set; }

        public Arena()
        {
            Width = 600;
            Depth = 400;
            Height = 400;
            SetUpVertices();
            //SetUpOuterIndices();
            SetUpInnerIndices();
            SetField();
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
            FieldVertices = new Game1.VertexPositionColorNormal[4];
            FieldVertices[0].Position = new Vector3(-120, -Height / 2 + 1, 75);
            FieldVertices[0].Color = Color.Yellow;
            FieldVertices[1].Position = new Vector3(120, -Height / 2 + 1, 75);
            FieldVertices[1].Color = Color.Yellow;
            FieldVertices[2].Position = new Vector3(120, -Height / 2 + 1, -75);
            FieldVertices[2].Color = Color.Yellow;
            FieldVertices[3].Position = new Vector3(-120, -Height / 2 + 1, -75);
            FieldVertices[3].Color = Color.Yellow;

            FieldIndices = new int[6];
            FieldIndices[0] = 0;
            FieldIndices[1] = 2;
            FieldIndices[2] = 1;
            FieldIndices[3] = 0;
            FieldIndices[4] = 3;
            FieldIndices[5] = 2;
        }
    }
}
