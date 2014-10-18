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
        SpriteBatch spriteBatch;
        GraphicsDevice device;
        Effect effect;
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
        float leftrightRot = MathHelper.PiOver2;
        float updownRot = 0;
        float thirdRot = 0;
        const float rotationSpeed = 0.3f;
        const float moveSpeed = 30.0f;
        MouseState originalMouseState;
        Matrix worldMatrix;
        bool MouseEnable = true;
        Vector3 clipVectorNormal = new Vector3(0, 0, 0);

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
            Window.Title = "GK 3D";
            IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            device = graphics.GraphicsDevice;
            effect = Content.Load<Effect>("effects"); ;
            arena = new Arena();
            //SetUpCamera();
            //UpdateViewMatrix();
            Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
            originalMouseState = Mouse.GetState();
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

            SetUpCamera();
            CalculateNormals(arena.Vertices, arena.Indices);
            CalculateNormals(arena.FieldVertices, arena.FieldIndices);

            // TODO: use this.Content to load your game content here
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
            //MouseState mouseState = Mouse.GetState();
            //KeyboardState keyState = Keyboard.GetState();
            //if (mouseState.Y < graphics.PreferredBackBufferHeight && mouseState.Y > 0)
            //{
            //    if (mouseState.X > graphics.PreferredBackBufferWidth * 3 / 4 && mouseState.X < graphics.PreferredBackBufferWidth)
            //        horizontalAngle = 0.01f;
            //    else if (mouseState.X < graphics.PreferredBackBufferWidth / 4 && mouseState.X > 0)
            //        horizontalAngle = -0.01f;
            //    else
            //        horizontalAngle = 0;
            //}
            //if (mouseState.X < graphics.PreferredBackBufferWidth && mouseState.X > 0)
            //{
            //    if (mouseState.Y > graphics.PreferredBackBufferHeight * 3 / 4 && mouseState.Y < graphics.PreferredBackBufferHeight)
            //        verticalAngle = 0.01f;
            //    else if (mouseState.Y < graphics.PreferredBackBufferHeight / 4 && mouseState.Y > 0)
            //        verticalAngle = -0.01f;
            //    else verticalAngle = 0;
            //}
            //if (keyState.IsKeyDown(Keys.Delete))
            //    angle += 0.05f;
            //if (keyState.IsKeyDown(Keys.PageDown))
            //    angle -= 0.05f;
            float timeDifference = (float)gameTime.ElapsedGameTime.TotalMilliseconds / 1000.0f;
            ProcessInput(timeDifference);

            base.Update(gameTime);
        }
        

        private void ProcessInput(float amount)
        {
            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState != originalMouseState && MouseEnable)
            {
                float xDifference = currentMouseState.X - originalMouseState.X;
                float yDifference = currentMouseState.Y - originalMouseState.Y;
                leftrightRot -= rotationSpeed * xDifference * amount;
                updownRot -= rotationSpeed * yDifference * amount;
                Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
                UpdateViewMatrix();
            }
            Vector3 moveVector = new Vector3(0, 0, 0);
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Up))
                moveVector += new Vector3(0, 0, -2);
            if (keyState.IsKeyDown(Keys.Down))
                moveVector += new Vector3(0, 0, 2);
            if (keyState.IsKeyDown(Keys.Right))
                moveVector += new Vector3(2, 0, 0);
            if (keyState.IsKeyDown(Keys.Left))
                moveVector += new Vector3(-2, 0, 0);
            if (keyState.IsKeyDown(Keys.PageUp))
                moveVector += new Vector3(0, 2, 0);
            if (keyState.IsKeyDown(Keys.PageDown))
                moveVector += new Vector3(0, -2, 0);
            if (keyState.IsKeyDown(Keys.P))
                thirdRot -= rotationSpeed * 1f * amount;
            if (keyState.IsKeyDown(Keys.O))
                thirdRot += rotationSpeed * 1f * amount;
            if (keyState.IsKeyDown(Keys.Tab))
                MouseEnable = !MouseEnable;
            if (keyState.IsKeyDown(Keys.I))
                updownRot -= rotationSpeed * 1f * amount;
            if (keyState.IsKeyDown(Keys.U))
                updownRot += rotationSpeed * 1f * amount;
            if (keyState.IsKeyDown(Keys.Y))
                leftrightRot -= rotationSpeed * 1f * amount;
            if (keyState.IsKeyDown(Keys.T))
                leftrightRot += rotationSpeed * 1f * amount;


            if (keyState.IsKeyDown(Keys.Escape))
                this.Exit();
            AddToCameraPosition(moveVector * amount);
        }

        private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            position += moveSpeed * rotatedVector;
            UpdateViewMatrix();
        }

        private void UpdateViewMatrix()
        {
            Matrix cameraRotation = Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
            Matrix cameraRotation2 = Matrix.CreateRotationZ(thirdRot) * Matrix.CreateRotationX(updownRot) * Matrix.CreateRotationY(leftrightRot);
            Vector3 cameraOriginalTarget = new Vector3(0, 0, -1);
            Vector3 cameraOriginalUpVector = new Vector3(0, 1, 0);
            Vector3 cameraOriginalRightVector = new Vector3(1, 0, 0);

            Vector3 cameraRotatedTarget = Vector3.Transform(cameraOriginalTarget, cameraRotation2);
            Vector3 cameraFinalTarget = position + cameraRotatedTarget;


            Vector3 cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotation2);


            viewMatrix = Matrix.CreateLookAt(position, cameraFinalTarget, cameraRotatedUpVector);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightBlue);
            RasterizerState rs = new RasterizerState();
            //rs.CullMode = CullMode.None;
            //rs.FillMode = FillMode.WireFrame;
            //device.RasterizerState = rs;
            
            //viewMatrix = viewMatrix * Matrix.CreateRotationY(horizontalAngle) * Matrix.CreateRotationX(verticalAngle);
            Matrix worldMatrix = Matrix.Identity;
            //Matrix worldMatrix = Matrix.CreateTranslation(position) * Matrix.CreateRotationY(horizontalAngle) * Matrix.CreateTranslation(-position); // * Matrix.CreateRotationX(verticalAngle);
            //Matrix worldMatrix = Matrix.CreateTranslation(new Vector3(0, -300, -1000)) * Matrix.CreateRotationY(horizontalAngle) * Matrix.CreateRotationX(verticalAngle);
            effect.CurrentTechnique = effect.Techniques["Colored"];
            effect.Parameters["xView"].SetValue(viewMatrix);
            effect.Parameters["xProjection"].SetValue(projectionMatrix);
            effect.Parameters["xWorld"].SetValue(worldMatrix);
            Vector3 lightDirection = new Vector3(1.0f, -1.0f, -1.0f);
            lightDirection.Normalize();
            effect.Parameters["xLightDirection"].SetValue(lightDirection);
            effect.Parameters["xAmbient"].SetValue(0.4f);
            effect.Parameters["xEnableLighting"].SetValue(true);
            //effect.VertexColorEnabled = true;
            //effect.View = viewMatrix;
            //effect.World = Matrix.CreateTranslation(new Vector3(0, 0, -10)) * Matrix.CreateRotationY(horizontalAngle) * Matrix.CreateRotationX(verticalAngle);
            //effect.Projection = projectionMatrix;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, arena.Vertices, 0, arena.Vertices.Length, arena.Indices, 0, arena.Indices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, arena.FieldVertices, 0, arena.FieldVertices.Length, arena.FieldIndices, 0, arena.FieldIndices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
                device.DrawUserIndexedPrimitives(PrimitiveType.LineList, axis, 0, axis.Length, axisIndices, 0, 3, VertexPositionColorNormal.VertexDeclaration);
            }
            base.Draw(gameTime);
        }

        private Vector3 toRotateV = new Vector3(0, 10, 100);
        private Vector3 position = new Vector3(100, 0, 0);
        private Vector3 lookAt = new Vector3(5, 10, 0);
        private void SetUpCamera()
        {
            viewMatrix = Matrix.CreateLookAt(position, lookAt, new Vector3(0, 1, 0));
            //viewMatrix = Matrix.CreateLookAt(new Vector3(0, 300, 1000), new Vector3(0, 0, 0), new Vector3(0, 1, 0));
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, device.Viewport.AspectRatio, 1.0f, 10000.0f);
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
    }
}
