using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GK3D1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        Camera camera;
        VertexBuffer myVertexBuffer;
        IndexBuffer myIndexBuffer;
        Arena arena;
        VertexPositionColorNormal[] axis;
        int[] axisIndices;
        MultiLightingMaterial manySpotLightMat;
        Effect spotLightEffect;
        Effect spotLightEffectTextureless;
        Effect spotLightEffectBench;
        

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 700;
            graphics.PreferredBackBufferHeight = 700;
            graphics.IsFullScreen = false;
            graphics.PreferMultiSampling = true;
            graphics.ApplyChanges();
            Window.Title = "Volleyball Arena";
            //IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            device = graphics.GraphicsDevice;
            arena = new Arena(Content, device);
            //ShowAxis();
            camera = new Camera(device, this, new Vector3(arena.Width / 2 - 50, arena.Height / 2 - 100, arena.Depth / 2 - 50));
            camera.Position = new Vector3(arena.Width/2.3f, arena.Height/5, 400);
            CalculateNormals(arena.Vertices, arena.Indices);
            //arena.CalculateNormals(arena.FieldVertices, arena.FieldIndices);
            //arena.CalculateNormals(arena.FloorVertices, arena.FloorIndices);
            CalculateNormals(arena.Net.Vertices, arena.Net.Indices);
            CalculateNormals(arena.LeftPost.Vertices, arena.LeftPost.Indices);
            CalculateNormals(arena.RightPost.Vertices, arena.RightPost.Indices);
            foreach (var b in arena.Bleachers)
            {
                CalculateNormals(b.Vertices, b.Indices);
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            camera.Update(timeDifference);

            base.Update(gameTime);
        }

        private void setEffectMatrices(Effect effect)
        {
            setEffectParameter(effect, "World", camera.World);
            setEffectParameter(effect, "View", camera.View);
            setEffectParameter(effect, "Projection", camera.Projection);
            setEffectParameter(effect, "CameraPosition", camera.Position);
        }


        private int component = 1;
        private void updatePointLight()
        {
            if (component == 1)
            {
                component = 2;
                arena.LightMaterialToClone.PointLightColor = new Vector3(0.8f, 2f, 0);
                arena.LightMaterialToClone.PointLightSpecularColor = new Vector3(0.8f, 2, 0);
            }
            else if (component == 2)
            {
                component = 3;
                arena.LightMaterialToClone.PointLightColor = new Vector3(0.3f, 0, 2.5f);
                arena.LightMaterialToClone.PointLightSpecularColor = new Vector3(0.3f, 0, 2.5f);
            }
            else
            {
                component = 1;
                arena.LightMaterialToClone.PointLightColor = new Vector3(1.5f, 0, 0);
                arena.LightMaterialToClone.PointLightSpecularColor = new Vector3(1.5f, 0, 0);
            }

            arena.LightMaterialToClone.SetEffectParameters(arena.FieldLightEffect);
            arena.LightMaterialToClone.SetEffectParameters(arena.FloorLightEffect);
            arena.LightMaterialToClone.SetEffectParameters(arena.MultipleLightEffectToClone);
            arena.LightMaterialToClone.SetEffectParameters(arena.TexturelessLightEffect);
        }

        float time = 2;
        float timer = 2;

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            //RasterizerState rs = new RasterizerState();
            time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time < 0)
            {
                time = timer;
                updatePointLight();
            }

            foreach (EffectPass pass in arena.TexturelessLightEffect.CurrentTechnique.Passes)
            {
                setEffectMatrices(arena.TexturelessLightEffect);
                pass.Apply();
                // arena
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, arena.Vertices, 0, arena.Vertices.Length,
                    arena.Indices, 0, arena.Indices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
                //net
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, arena.Net.Vertices, 0, arena.Net.Vertices.Length,
                    arena.Net.Indices, 0, arena.Net.Indices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
                //left post
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, arena.LeftPost.Vertices, 0, arena.LeftPost.Vertices.Length,
                    arena.LeftPost.Indices, 0, arena.LeftPost.Indices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
                //right post
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, arena.RightPost.Vertices, 0, arena.RightPost.Vertices.Length,
                    arena.RightPost.Indices, 0, arena.RightPost.Indices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
                //axis
                //device.DrawUserIndexedPrimitives(PrimitiveType.LineList, axis, 0, axis.Length, axisIndices, 0, 3,
                //    VertexPositionColorNormal.VertexDeclaration);
                foreach (var b in arena.Bleachers)
                {
                    device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, b.Vertices, 0, b.Vertices.Length,
                    b.Indices, 0, b.Indices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
                }
            }

            foreach (EffectPass pass in arena.FieldLightEffect.CurrentTechnique.Passes)
            {
                setEffectMatrices(arena.FieldLightEffect);
                pass.Apply();
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, arena.FieldVertices, 0, arena.FieldVertices.Length, arena.FieldIndices, 0, arena.FieldIndices.Length / 3, VertexPositionNormalTexture.VertexDeclaration);
            }

            //foreach (EffectPass pass in arena.FloorLightEffect.CurrentTechnique.Passes)
            //{
            //    setEffectMatrices(arena.FloorLightEffect);
            //    pass.Apply();
            //    device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, arena.FloorVertices, 0, arena.FloorVertices.Length, arena.FloorIndices, 0, arena.FloorIndices.Length / 3, VertexPositionNormalTexture.VertexDeclaration);
            //}

            foreach (CModel model in arena.Models)
                model.Draw(camera.View, camera.Projection, camera.Position);

            foreach (CModel model in arena.TexturelessModels)
                model.Draw(camera.View, camera.Projection, camera.Position);

            base.Draw(gameTime);
        }

        private void CalculateNormals(VertexPositionColorNormal[] vertices, int[] indices)
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

        private void ShowAxis()
        {
            axis = new VertexPositionColorNormal[6];
            axis[0].Position = new Vector3(-1000, 0, 0);
            axis[0].Color = Color.Green;
            axis[1].Position = new Vector3(1000, 0, 0);
            axis[1].Color = Color.Green;
            axis[2].Position = new Vector3(0, -1000, 0);
            axis[2].Color = Color.Blue;
            axis[3].Position = new Vector3(0, 1000, 0);
            axis[3].Color = Color.Blue;
            axis[4].Position = new Vector3(0, 0, -1000);
            axis[4].Color = Color.Orange;
            axis[5].Position = new Vector3(0, 0, 1000);
            axis[5].Color = Color.Orange;

            axisIndices = new int[6];
            axisIndices[0] = 0;
            axisIndices[1] = 1;
            axisIndices[2] = 2;
            axisIndices[3] = 3;
            axisIndices[4] = 4;
            axisIndices[5] = 5;
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
