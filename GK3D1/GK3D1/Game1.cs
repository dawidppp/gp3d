using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace GK3D1
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        GraphicsDevice device;
        Effect effect;
        BasicEffect basicEffectCourt;
        BasicEffect basicEffectFloor;
        Texture2D heightMap;
        Matrix viewMatrix;
        Matrix projectionMatrix;
        VertexBuffer myVertexBuffer;
        IndexBuffer myIndexBuffer;
        private Arena arena;
        private float angle = 0f;
        private float horizontalAngle = 0f;
        private float verticalAngle = 0f;
        private VertexPositionColorNormal[] axis;
        private int[] axisIndices;
        float yaw = MathHelper.PiOver2;
        float pitch = 0;
        const float rotationSpeed = 0.3f;
        const float moveSpeed = 800.0f;
        MouseState originalMouseState;
        Matrix worldMatrix;
        bool MouseEnable = true;
        private Effect simpleEffect;

        public struct VertexPositionColorNormal
        {
            public Vector3 Position;
            public Color Color;
            public Vector3 Normal;

            public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
            (
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(sizeof(float) * 3, VertexElementFormat.Color, VertexElementUsage.Color, 0),
                new VertexElement(sizeof(float) * 3 + 4, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0)
            );
        }

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
            graphics.PreferredBackBufferWidth = 500;
            graphics.PreferredBackBufferHeight = 500;
            graphics.IsFullScreen = false;
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
            effect = Content.Load<Effect>("effects");
            basicEffectCourt = new BasicEffect(device);
            basicEffectFloor = new BasicEffect(device);
            simpleEffect = Content.Load<Effect>("PointLightEffect");
            arena = new Arena(Content);
            graphics.PreferMultiSampling = true;
            graphics.ApplyChanges();
            arena.Bench = new Bench(Content, new BasicEffect(device));
            Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
            originalMouseState = Mouse.GetState();
            ShowAxis();

            SetUpCamera();
            CalculateNormals(arena.Vertices, arena.Indices);
            arena.CalculateNormals(arena.FieldVertices, arena.FieldIndices);
            arena.CalculateNormals(arena.FloorVertices, arena.FloorIndices);
            CalculateNormals(arena.Net.Vertices, arena.Net.Indices);
            CalculateNormals(arena.LeftPost.Vertices, arena.LeftPost.Indices);
            CalculateNormals(arena.RightPost.Vertices, arena.RightPost.Indices);
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
            ProcessInput(timeDifference);
            basicEffectFloor.World = Matrix.Identity;
            basicEffectFloor.View = viewMatrix;
            basicEffectFloor.Projection = projectionMatrix;
            basicEffectFloor.TextureEnabled = true;
            basicEffectFloor.Texture = arena.FloorTexture;

            basicEffectCourt.World = Matrix.Identity;
            basicEffectCourt.View = viewMatrix;
            basicEffectCourt.Projection = projectionMatrix;
            basicEffectCourt.TextureEnabled = true;
            basicEffectCourt.Texture = arena.CourtTexture;

            base.Update(gameTime);
        }


        private void ProcessInput(float amount)
        {
            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState != originalMouseState && MouseEnable)
            {
                float xDifference = currentMouseState.X - originalMouseState.X;
                float yDifference = currentMouseState.Y - originalMouseState.Y;
                yaw -= rotationSpeed * xDifference * amount;
                pitch -= rotationSpeed * yDifference * amount;
                Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
                UpdateViewMatrix();
            }
            Vector3 moveVector = new Vector3(0, 0, 0);
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Up))
                moveVector += new Vector3(0, 0, -1);
            if (keyState.IsKeyDown(Keys.Down))
                moveVector += new Vector3(0, 0, 1);
            if (keyState.IsKeyDown(Keys.Right))
                moveVector += new Vector3(1, 0, 0);
            if (keyState.IsKeyDown(Keys.Left))
                moveVector += new Vector3(-1, 0, 0);
            if (keyState.IsKeyDown(Keys.PageUp))
                moveVector += new Vector3(0, 1, 0);
            if (keyState.IsKeyDown(Keys.PageDown))
                moveVector += new Vector3(0, -1, 0);
            if (keyState.IsKeyDown(Keys.Tab))
                MouseEnable = !MouseEnable;
            if (keyState.IsKeyDown(Keys.Escape))
                this.Exit();

            AddToCameraPosition(moveVector * amount);
        }

        private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(pitch) * Matrix.CreateRotationY(yaw);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            position += moveSpeed * rotatedVector;
            UpdateViewMatrix();
        }

        private void UpdateViewMatrix()
        {
            var cameraRotationMatrix = Matrix.CreateRotationX(pitch) * Matrix.CreateRotationY(yaw);

            var cameraOriginalTargetVector = new Vector3(0, 0, -1);
            var cameraOriginalUpVector = new Vector3(0, 1, 0);
            var cameraRotatedTargetVector = Vector3.Transform(cameraOriginalTargetVector, cameraRotationMatrix);
            var cameraFinalTargetVector = position + cameraRotatedTargetVector;
            var cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotationMatrix);

            viewMatrix = Matrix.CreateLookAt(position, cameraFinalTargetVector, cameraRotatedUpVector);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);
            //RasterizerState rs = new RasterizerState();

            worldMatrix = Matrix.Identity;
            effect.CurrentTechnique = effect.Techniques["Colored"];
            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);
            effect.Parameters["xWorld"].SetValue(worldMatrix);
            Vector3 lightDirection = new Vector3(1.0f, -1.0f, -1.0f);
            lightDirection.Normalize();
            effect.Parameters["xLightDirection"].SetValue(lightDirection);
            effect.Parameters["xAmbient"].SetValue(0.4f);
            effect.Parameters["xEnableLighting"].SetValue(true);
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, arena.Vertices, 0, arena.Vertices.Length,
                    arena.Indices, 0, arena.Indices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, arena.Net.Vertices, 0, arena.Net.Vertices.Length,
                    arena.Net.Indices, 0, arena.Net.Indices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, arena.LeftPost.Vertices, 0, arena.LeftPost.Vertices.Length,
                    arena.LeftPost.Indices, 0, arena.LeftPost.Indices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, arena.RightPost.Vertices, 0, arena.RightPost.Vertices.Length,
                    arena.RightPost.Indices, 0, arena.RightPost.Indices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
                device.DrawUserIndexedPrimitives(PrimitiveType.LineList, axis, 0, axis.Length, axisIndices, 0, 3,
                    VertexPositionColorNormal.VertexDeclaration);
            }

            DrawModelFbx(viewMatrix, new Vector3(-600, -970, 500), arena.Bench.BenchModel, 1f, new Vector3(0, 300, 0));
            DrawModelFbx(viewMatrix, new Vector3(-150, -970, 500), arena.Bench.BenchModel, 1f, new Vector3(0, 300, 0));
            DrawModelFbx(viewMatrix, new Vector3(250, -970, 500), arena.Bench.BenchModel, 1f, new Vector3(0, 300, 0));
            DrawModelFbx(viewMatrix, new Vector3(700, -970, 500), arena.Bench.BenchModel, 1f, new Vector3(0, 300, 0));
            DrawModelFbx(viewMatrix, new Vector3(-600, -970, -500), arena.Bench.BenchModel, 1f, new Vector3(0, -300, 0));
            DrawModelFbx(viewMatrix, new Vector3(-150, -970, -500), arena.Bench.BenchModel, 1f, new Vector3(0, -300, 0));
            DrawModelFbx(viewMatrix, new Vector3(250, -970, -500), arena.Bench.BenchModel, 1f, new Vector3(0, -300, 0));
            DrawModelFbx(viewMatrix, new Vector3(700, -970, -500), arena.Bench.BenchModel, 1f, new Vector3(0, -300, 0));

            foreach (EffectPass pass2 in basicEffectFloor.CurrentTechnique.Passes)
            {
                pass2.Apply();
                basicEffectFloor.LightingEnabled = true;
                basicEffectFloor.PreferPerPixelLighting = true;
                basicEffectFloor.DirectionalLight0.Direction = new Vector3(-1, -1, 1);
                basicEffectFloor.DirectionalLight0.Enabled = true;
                basicEffectFloor.DirectionalLight0.SpecularColor = new Vector3(0.5f, 0.5f, 0.5f);
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, arena.FloorVertices, 0, arena.FloorVertices.Length, arena.FloorIndices, 0, arena.FloorIndices.Length / 3, VertexPositionNormalTexture.VertexDeclaration);
            }
            foreach (EffectPass pass2 in basicEffectCourt.CurrentTechnique.Passes)
            {
                pass2.Apply();
                basicEffectCourt.LightingEnabled = true;
                basicEffectCourt.PreferPerPixelLighting = true;
                basicEffectCourt.DirectionalLight0.Direction = new Vector3(-1, -1, 1);
                basicEffectCourt.DirectionalLight0.Enabled = true;
                basicEffectCourt.DirectionalLight0.SpecularColor = new Vector3(1f, 1f, 1f);
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, arena.FieldVertices, 0, arena.FieldVertices.Length, arena.FieldIndices, 0, arena.FieldIndices.Length / 3, VertexPositionNormalTexture.VertexDeclaration);
            }
            base.Draw(gameTime);
        }

        private void DrawModelFbx(Matrix currentViewMatrix, Vector3 Translation, Model model, float f, Vector3 rotation = new Vector3())
        {
            Matrix worldMatrix = Matrix.CreateScale(f, f, f) *
                                 Matrix.Identity * Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y) * Matrix.CreateRotationZ(rotation.Z);
            worldMatrix = worldMatrix * Matrix.CreateTranslation(Translation);
            // pos,rot,scl
            Matrix[] shipTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(shipTransforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect currentEffect in mesh.Effects)
                {
                    currentEffect.World = worldMatrix;
                    currentEffect.View = currentViewMatrix;
                    currentEffect.Projection = projectionMatrix;
                    currentEffect.LightingEnabled = true;
                }
                mesh.Draw();
            }
        }

        private Vector3 position = new Vector3(1300, -500, 10);
        private Vector3 lookAt = new Vector3(5, 10, -300);
        private void SetUpCamera()
        {
            viewMatrix = Matrix.CreateLookAt(position, lookAt, new Vector3(0, 1, 0));
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 1.0f, 10000.0f);
        }

        private void CalculateNormals(Game1.VertexPositionColorNormal[] vertices, int[] indices)
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
    }
}
