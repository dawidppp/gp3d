using System;
using System.Linq.Expressions;
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
        FogEffect fogEffect;
        Effect simpleColorEffect;
        RasterizerState originalState;
        RasterizerState wireFrameRasterizerState;
        ParticleSystem ps;
        Random r = new Random();

        //billboard system
        BillboardSystem spectators;


        #region Create Mirror VertexBuffer

        VertexBuffer verticesM;

        void CreateMirrorVertexBuffer()
        {
            VertexPositionColor[] vertices = new VertexPositionColor[4];

            vertices[0].Position = new Vector3(0, 1, -3) * 400;// - new Vector3(arena.Width/3,0,0);
            vertices[1].Position = new Vector3(0, 1, 3) * 400;// - new Vector3(arena.Width / 3, 0, 0);
            vertices[2].Position = new Vector3(0, 5, 3) * 400;// - new Vector3(arena.Width / 3, 0, 0);
            vertices[3].Position = new Vector3(0, 5, -3) * 400;// - new Vector3(arena.Width / 3, 0, 0);
            vertices[0].Color = Color.White;
            vertices[1].Color = Color.White;
            vertices[2].Color = Color.White;
            vertices[3].Color = Color.White;

            verticesM = new VertexBuffer(device, VertexPositionColor.VertexDeclaration, 4, BufferUsage.WriteOnly);
            verticesM.SetData<VertexPositionColor>(vertices);
        }
        #endregion

        #region Create Mirror IndexBuffer

        IndexBuffer indicesM;

        void CreateMirrorIndexBuffer()
        {
            UInt16[] indices = new UInt16[6];

            indices[0] = 0;
            indices[1] = 2;
            indices[2] = 3;
            indices[3] = 0;
            indices[4] = 1;
            indices[5] = 2;

            indicesM = new IndexBuffer(device, IndexElementSize.SixteenBits, 6, BufferUsage.WriteOnly);
            indicesM.SetData<UInt16>(indices);

        }
        #endregion

        DepthStencilState addIfMirror = new DepthStencilState()
        {
            StencilEnable = true,
            StencilFunction = CompareFunction.Always,
            StencilPass = StencilOperation.Increment

        };
        DepthStencilState checkMirror = new DepthStencilState()
        {
            StencilEnable = true,
            StencilFunction = CompareFunction.Equal,
            ReferenceStencil = 1,
            StencilPass = StencilOperation.Keep
        };


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
            //graphics.PreferredDepthStencilFormat = DepthFormat.Depth24Stencil8;
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
            originalState = device.RasterizerState;
            //wireFrameRasterizerState = new RasterizerState();
            //wireFrameRasterizerState.FillMode = FillMode.WireFrame;
            simpleColorEffect = Content.Load<Effect>("SimpleColor");
            fogEffect = new FogEffect();
            arena = new Arena(Content, device);
            //ShowAxis();
            camera = new Camera(device, graphics, this, new Vector3(arena.Width / 2 - 50, arena.Height / 2 - 100, arena.Depth / 2 - 50));
            camera.Position = new Vector3(arena.Width / 2.3f, arena.Height / 5, 400);

            ps = new ParticleSystem(GraphicsDevice, Content, Content.Load<Texture2D>("fire"),
800, new Vector2(80), 0.5f, Vector3.Zero, 0.1f);
            CalculateNormals(arena.Vertices, arena.Indices);
            //arena.CalculateNormals(arena.FieldVertices, arena.FieldIndices);
            //arena.CalculateNormals(arena.FloorVertices, arena.FloorIndices);
            //CalculateNormals(arena.Net.Vertices, arena.Net.Indices);
            CalculateNormals(arena.LeftPost.Vertices, arena.LeftPost.Indices);
            CalculateNormals(arena.RightPost.Vertices, arena.RightPost.Indices);

            foreach (var b in arena.Bleachers)
            {
                CalculateNormals(b.Vertices, b.Indices);
            }

            //billboards
            // Generate random tree positions
            Random r = new Random();
            Vector3[] positions = new Vector3[30];
            for (int i = 0; i < positions.Length / 3; i++)
            {
                positions[i] = new Vector3(-arena.Width / 4 + i * 300,
                    -arena.Height / 3 - 100, -arena.Depth / 4 - 250);
                positions[i + positions.Length / 3] = new Vector3(-arena.Width / 4 + i * 300,
                    -arena.Height / 3 - 270, -arena.Depth / 4 - 50);
                positions[i + 2 * positions.Length / 3] = new Vector3(-arena.Width / 4 + i * 300,
                    -arena.Height / 3 - 400, -arena.Depth / 4 + 150);
            }
            spectators = new BillboardSystem(GraphicsDevice, Content,
            Content.Load<Texture2D>("person"), new Vector2(150, 250), positions);

            CreateMirrorVertexBuffer();
            CreateMirrorIndexBuffer();

            Camera.ClipPlaneX = arena.Width / 2;
            setEffectToAllFiles("ClipPlane", (float)arena.Width / 2 + 100f);
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
            ProcessInput();

           

            for (int i = 0; i < 10; i++)
            {
                // Generate a direction within 15 degrees of (0, 1, 0)
                Vector3 offset = new Vector3(MathHelper.ToRadians(20.0f));
                Vector3 randAngle = Vector3.Up + randVec3(offset/2, offset);
                // Generate a position between (-400, 0, -400) and (400, 0, 400)
                Vector3 randPosition = randVec3(new Vector3(-20) + arena.Models[5].Position + new Vector3(0, 50, 50), new Vector3(20) + arena.Models[5].Position + new Vector3(0, 50, 50));
                // Generate a speed between 600 and 900
                float randSpeed = (float) r.NextDouble()*300 + 200;
                ps.AddParticle(randPosition, randAngle, randSpeed);
                ps.Update();
            }


            base.Update(gameTime);
        }

        // Returns a random Vector3 between min and max
        Vector3 randVec3(Vector3 min, Vector3 max)
        {
            return new Vector3(
            min.X + (float)r.NextDouble() * (max.X - min.X),
            min.Y + (float)r.NextDouble() * (max.Y - min.Y),
            min.Z + (float)r.NextDouble() * (max.Z - min.Z));
        }

        private bool isFDown = false;
        private bool isGDown = false;
        private bool isHDown = false;
        private bool isTDown = false;
        private void ProcessInput()
        {
            KeyboardState keyState = Keyboard.GetState();

            // fog enabled
            if (keyState.IsKeyDown(Keys.F) && !isFDown)
            {
                FogEffect.IsFogEnabled = !FogEffect.IsFogEnabled;
                setFogParameters();
                isFDown = !isFDown;
            }
            if (keyState.IsKeyUp(Keys.F) && isFDown)
                isFDown = false;

            // fog power increment
            if (keyState.IsKeyDown(Keys.G))
            {
                if (FogEffect.FogPower < 0.8f)
                    FogEffect.FogPower += 0.01f;
                setFogParameters();
            }

            // fog power decrement
            if (keyState.IsKeyDown(Keys.H))
            {
                if (FogEffect.FogPower > 0)
                    FogEffect.FogPower -= 0.01f;
                setFogParameters();
            }

            // clip plane increment
            if (keyState.IsKeyDown(Keys.O))
            {
                Camera.ClipPlaneX -= 20;
                Console.WriteLine(Camera.ClipPlaneX);
                setEffectToAllFiles("ClipPlane", (float)Camera.ClipPlaneX);
            }

            // clip plane decrement
            if (keyState.IsKeyDown(Keys.P))
            {
                Camera.ClipPlaneX += 20;
                setEffectToAllFiles("ClipPlane", (float)Camera.ClipPlaneX);
            }

            // court texture change
            if (keyState.IsKeyDown(Keys.T) && !isTDown)
            {
                arena.ActiveCourtTexture = arena.ActiveCourtTexture >= arena.CourtTextures.Count - 1
                    ? 0
                    : arena.ActiveCourtTexture + 1;
                setEffectParameter(arena.FieldLightEffect, "BasicTexture", arena.CourtTextures[arena.ActiveCourtTexture]);
                isTDown = true;
            }
            if (keyState.IsKeyUp(Keys.T) && isTDown)
                isTDown = false;
        }

        private void setFogParameters()
        {
            fogEffect.SetParameters(arena.FieldLightEffect);
            fogEffect.SetParameters(arena.FloorLightEffect);
            fogEffect.SetParameters(arena.MultipleLightEffectToClone);
            fogEffect.SetParameters(arena.NetLightEffect);
            fogEffect.SetParameters(arena.TexturelessLightEffect);
            fogEffect.SetParameters(spectators.effect);
        }

        private void setEffectMatrices(Effect effect, Matrix? worldReflection = null)
        {
            if (worldReflection != null)
                setEffectParameter(effect, "World", worldReflection);
            else
                setEffectParameter(effect, "World", camera.World);
            setEffectParameter(effect, "View", camera.View);
            setEffectParameter(effect, "Projection", camera.Projection);
            setEffectParameter(effect, "CameraPosition", camera.Position);
        }

        private void setEffectToAllFiles(string parameter, object value)
        {
            setEffectParameter(arena.FieldLightEffect, parameter, value);
            setEffectParameter(arena.FloorLightEffect, parameter, value);
            setEffectParameter(arena.MultipleLightEffectToClone, parameter, value);
            setEffectParameter(arena.NetLightEffect, parameter, value);
            setEffectParameter(arena.TexturelessLightEffect, parameter, value);
            setEffectParameter(spectators.effect, parameter, value);
        }


        private int component = 1;
        private void updatePointLight()
        {
            if (component == 1)
            {
                component = 2;
                arena.LightMaterialToClone.PointLightColor = new Vector4(0.8f, 2f, 0, 1);
                arena.LightMaterialToClone.PointLightSpecularColor = new Vector4(0.8f, 2, 0, 1);
            }
            else if (component == 2)
            {
                component = 3;
                arena.LightMaterialToClone.PointLightColor = new Vector4(0.3f, 0, 2.5f, 1);
                arena.LightMaterialToClone.PointLightSpecularColor = new Vector4(0.3f, 0, 2.5f, 1);
            }
            else
            {
                component = 1;
                arena.LightMaterialToClone.PointLightColor = new Vector4(1.5f, 0, 0, 1);
                arena.LightMaterialToClone.PointLightSpecularColor = new Vector4(1.5f, 0, 0, 1);
            }

            arena.LightMaterialToClone.SetEffectParameters(arena.FieldLightEffect);
            //arena.LightMaterialToClone.SetEffectParameters(arena.FloorLightEffect);
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
            //GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, Color.CornflowerBlue, 1, 0);

            //GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            //GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;

            DrawScene(gameTime);

            
            ps.Draw(camera.View, camera.Projection, camera.Up, camera.Right);

            //device.RasterizerState = wireFrameRasterizerState;
            //setEffectToAllFiles("ClipFront", true);
            //DrawScene(gameTime);

            //device.RasterizerState = originalState;
            

            base.Draw(gameTime);
        }


        private void DrawScene(GameTime gameTime)
        {
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);
            //RasterizerState rs = new RasterizerState();
            time -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (time < 0)
            {
                time = timer;
                updatePointLight();
            }

            int i = 0;
            setEffectToAllFiles("ClipFront", false);
            foreach (EffectPass pass in arena.TexturelessLightEffect.CurrentTechnique.Passes)
            {
                if (i == 1)
                    setEffectToAllFiles("ClipFront", true);
                setEffectMatrices(arena.TexturelessLightEffect);
                pass.Apply();
                // arena
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, arena.Vertices, 0, arena.Vertices.Length,
                    arena.Indices, 0, arena.Indices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
                //net
                //device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, arena.Net.Vertices, 0, arena.Net.Vertices.Length,
                //    arena.Net.Indices, 0, arena.Net.Indices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
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
                i++;
            }


            setEffectToAllFiles("ClipFront", false);
            foreach (EffectPass pass in arena.FloorLightEffect.CurrentTechnique.Passes)
            {
                setEffectMatrices(arena.FloorLightEffect);
                pass.Apply();
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, arena.FloorVertices, 0, arena.FloorVertices.Length, arena.FloorIndices, 0, arena.FloorIndices.Length / 3, VertexPositionNormalTexture.VertexDeclaration);
            }

            foreach (EffectPass pass in arena.FieldLightEffect.CurrentTechnique.Passes)
            {
                setEffectMatrices(arena.FieldLightEffect);
                pass.Apply();
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, arena.FieldVertices, 0, arena.FieldVertices.Length, arena.FieldIndices, 0, arena.FieldIndices.Length / 3, VertexPositionNormalTexture.VertexDeclaration);
            }

            spectators.Draw(camera.View, camera.Projection, camera.World, camera.Up, camera.Right, camera.Position);
            i = 0;
            foreach (EffectPass pass in arena.NetLightEffect.CurrentTechnique.Passes)
            {
                if (i == 1 && Camera.ClipPlaneX > 0)
                    break;
                setEffectMatrices(arena.NetLightEffect);
                pass.Apply();
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, arena.Net.Vertices, 0, arena.Net.Vertices.Length, arena.Net.Indices, 0, arena.Net.Indices.Length / 3, VertexPositionNormalTexture.VertexDeclaration);
                i++;
            }


            setEffectToAllFiles("ClipFront", false);
            foreach (CModel model in arena.Models)
                model.Draw(camera.View, camera.Projection, camera.Position);



            ////set and draw the mirror, remember we are drawing to both color and stencil buffer
            //device.SetVertexBuffer(verticesM);
            //device.Indices = indicesM;
            //simpleColorEffect.Parameters["WVP"].SetValue(camera.World * camera.View * camera.Projection);

            ////set the stencil buffer to check if we are drawing on the surface of the mirror
            //device.DepthStencilState = addIfMirror;
            //simpleColorEffect.CurrentTechnique.Passes[0].Apply();
            //device.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, 4, 0, 2);

            ////set stencil for draw on mirror surface only
            //device.DepthStencilState = checkMirror;
            //device.Clear(ClearOptions.DepthBuffer, Color.CornflowerBlue, 1, 0);
            //device.RasterizerState = RasterizerState.CullClockwise;

            //foreach (EffectPass pass in arena.TexturelessLightEffect.CurrentTechnique.Passes)
            //{
            //    setEffectMatrices(arena.TexturelessLightEffect, camera.Reflect);
            //    pass.Apply();
            //    // arena
            //    device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, arena.Vertices, 0, arena.Vertices.Length,
            //        arena.Indices, 0, arena.Indices.Length / 3, VertexPositionColorNormal.VertexDeclaration);

            //    foreach (var b in arena.Bleachers)
            //    {
            //        device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, b.Vertices, 0, b.Vertices.Length,
            //        b.Indices, 0, b.Indices.Length / 3, VertexPositionColorNormal.VertexDeclaration);
            //    }
            //}

            

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
            if (val is Vector4)
                effect.Parameters[paramName].SetValue((Vector4)val);
            if (val is float)
                effect.Parameters[paramName].SetValue((float)val);
            else if (val is bool)
                effect.Parameters[paramName].SetValue((bool)val);
            else if (val is Matrix)
                effect.Parameters[paramName].SetValue((Matrix)val);
            else if (val is Texture2D)
                effect.Parameters[paramName].SetValue((Texture2D)val);
        }
    }
}
