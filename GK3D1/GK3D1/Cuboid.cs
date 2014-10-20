using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GK3D1
{
    class Cuboid
    {
        public Game1.VertexPositionColorNormal[] Vertices { get; private set; }
        public int[] Indices { get; private set; }
        public Vector3 Center { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int Depth { get; set; }
        public Color Color { get; set; }

        public Cuboid(Vector3 center, int width, int height, int depth, bool isVisibleInside, bool isVisibleOutside, Color color)
        {
            Vertices = new Game1.VertexPositionColorNormal[8];
            Indices = new int[36];
            Center = center;
            Color = color;
            Width = width;
            Height = height;
            Depth = depth;
            SetUpVertices();
            if (isVisibleInside)
                SetUpInnerIndices();
            if (isVisibleOutside)
                SetUpOuterIndices();
        }

        public Cuboid(Game1.VertexPositionColorNormal[] vertices, int[] indices, Vector3 center)
        {
            Vertices = vertices;
            Indices = indices;
            Center = center;
        }

        public void SetUpVertices()
        {
            Vertices[0].Position = new Vector3(Center.X - Width / 2, Center.Y - Height / 2, Center.Z + Depth / 2);
            Vertices[0].Color = Color;
            Vertices[1].Position = new Vector3(Center.X + Width / 2, Center.Y - Height / 2, Center.Z + Depth / 2);
            Vertices[1].Color = Color;
            Vertices[2].Position = new Vector3(Center.X + Width / 2, Center.Y - Height / 2, Center.Z - Depth / 2);
            Vertices[2].Color = Color;
            Vertices[3].Position = new Vector3(Center.X - Width / 2, Center.Y - Height / 2, Center.Z - Depth / 2);
            Vertices[3].Color = Color;

            Vertices[4].Position = new Vector3(Center.X - Width / 2, Center.Y + Height / 2, Center.Z + Depth / 2);
            Vertices[4].Color = Color;
            Vertices[5].Position = new Vector3(Center.X + Width / 2, Center.Y + Height / 2, Center.Z + Depth / 2);
            Vertices[5].Color = Color;
            Vertices[6].Position = new Vector3(Center.X + Width / 2, Center.Y + Height / 2, Center.Z - Depth / 2);
            Vertices[6].Color = Color;
            Vertices[7].Position = new Vector3(Center.X - Width / 2, Center.Y + Height / 2, Center.Z - Depth / 2);
            Vertices[7].Color = Color;
        }

        private void SetUpOuterIndices()
        {
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
    }
}
