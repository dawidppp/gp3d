using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GK3D1
{
    public class Camera
    {
        public Matrix View { get; set; }
        public Matrix Projection { get; set; }
        public Matrix World { get; set; }
        public Matrix Reflect { get; set; }
        public float Yaw { get; set; }
        public float Pitch { get; set; }
        public float Roll { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 VectorUp { get; private set; }
        public float RotationSpeed { get; set; }
        public float MoveSpeed { get; set; }
        public MouseState OriginalMouseState { get; private set; }
        public bool MouseEnable { get; set; }
        public static int ClipPlaneX { get; set; }

        private GraphicsDevice graphicsDevice;
        private GraphicsDeviceManager deviceManager;
        private Game1 game;
        private Vector3 bounds;

        public Vector3 Up { get; private set; }
        public Vector3 Right { get; private set; }

        public Camera(GraphicsDevice graphicsDevice, GraphicsDeviceManager deviceManager, Game1 game, Vector3 bounds)
        {
            this.graphicsDevice = graphicsDevice;
            this.deviceManager = deviceManager;
            this.game = game;
            this.bounds = bounds;
            Yaw = MathHelper.PiOver2;
            Pitch = 0;
            Roll = 0;
            ClipPlaneX = 100;
            MouseEnable = true;
            Position = new Vector3(1300, -500, 10);
            RotationSpeed = 0.3f;
            MoveSpeed = 1200.0f;
            Mouse.SetPosition(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
            OriginalMouseState = Mouse.GetState();
            // Vector3 lookAt = new Vector3(5, 10, -300);
            SetMatrices();
        }

        private void SetMatrices()
        {
            World = Matrix.Identity;
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, graphicsDevice.Viewport.AspectRatio, 1.0f, 20000.0f);
            View = Matrix.CreateLookAt(Position, Position + new Vector3(0, 0, -1), new Vector3(0, 1, 0));
            Reflect = Matrix.CreateScale(-1, 1, 1);
        }

        public void Update(float amount)
        {
            ProcessInput(amount);
        }

        bool isTabDown = false;
        bool isTildeDown = false;
        private void ProcessInput(float amount)
        {
            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState != OriginalMouseState && MouseEnable)
            {
                float xDifference = currentMouseState.X - OriginalMouseState.X;
                float yDifference = currentMouseState.Y - OriginalMouseState.Y;
                Yaw -= RotationSpeed * xDifference * amount;
                Pitch -= RotationSpeed * yDifference * amount;
                Mouse.SetPosition(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
                //UpdateView();
            }

            Vector3 moveVector = new Vector3(0, 0, 0);
            KeyboardState keyState = Keyboard.GetState();
            if (keyState.IsKeyDown(Keys.Up) || keyState.IsKeyDown(Keys.W))
                moveVector += new Vector3(0, 0, -1);
            if (keyState.IsKeyDown(Keys.Down) || keyState.IsKeyDown(Keys.S))
                moveVector += new Vector3(0, 0, 1);
            if (keyState.IsKeyDown(Keys.Right) || keyState.IsKeyDown(Keys.D))
                moveVector += new Vector3(1, 0, 0);
            if (keyState.IsKeyDown(Keys.Left) || keyState.IsKeyDown(Keys.A))
                moveVector += new Vector3(-1, 0, 0);
            if (keyState.IsKeyDown(Keys.PageUp) || keyState.IsKeyDown(Keys.Q))
                moveVector += new Vector3(0, 1, 0);
            if (keyState.IsKeyDown(Keys.PageDown) || keyState.IsKeyDown(Keys.Z))
                moveVector += new Vector3(0, -1, 0);
            if (keyState.IsKeyDown(Keys.X))
                Roll += RotationSpeed * amount;
            if (keyState.IsKeyDown(Keys.C))
                Roll -= RotationSpeed * amount;
            if (keyState.IsKeyDown(Keys.Tab) && !isTabDown)
            {
                MouseEnable = !MouseEnable;
                isTabDown = !isTabDown;
            }
            if (keyState.IsKeyUp(Keys.Tab) && isTabDown)
                isTabDown = false;
            if (keyState.IsKeyDown(Keys.OemTilde) && !isTildeDown)
            {
                deviceManager.PreferMultiSampling = !deviceManager.PreferMultiSampling;
                deviceManager.ApplyChanges();
                isTildeDown = !isTildeDown;
            }
            if (keyState.IsKeyUp(Keys.OemTilde) && isTildeDown)
                isTildeDown = false;
            if (keyState.IsKeyDown(Keys.Escape))
                game.Exit();

            AddToCameraPosition(moveVector * amount);
        }

        private void AddToCameraPosition(Vector3 vectorToAdd)
        {
            Matrix cameraRotation = Matrix.CreateRotationZ(Roll) * Matrix.CreateRotationX(Pitch) * Matrix.CreateRotationY(Yaw);
            Vector3 rotatedVector = Vector3.Transform(vectorToAdd, cameraRotation);
            var newPosition = Position + MoveSpeed * rotatedVector;

            //check if the camera is inside the arena bounds
            if (newPosition.Z < bounds.Z && newPosition.Z > -bounds.Z &&
                newPosition.X < bounds.X && newPosition.X > -bounds.X &&
                newPosition.Y < bounds.Y && newPosition.Y > -bounds.Y)
                Position += MoveSpeed * rotatedVector;
            UpdateView();
        }

        private void UpdateView()
        {
            var cameraRotationMatrix = Matrix.CreateRotationZ(Roll) * Matrix.CreateRotationX(Pitch) * Matrix.CreateRotationY(Yaw);

            var cameraOriginalTargetVector = new Vector3(0, 0, -1);
            var cameraOriginalUpVector = new Vector3(0, 1, 0);
            var cameraRotatedTargetVector = Vector3.Transform(cameraOriginalTargetVector, cameraRotationMatrix);
            var cameraFinalTargetVector = Position + cameraRotatedTargetVector;
            var cameraRotatedUpVector = Vector3.Transform(cameraOriginalUpVector, cameraRotationMatrix);

            View = Matrix.CreateLookAt(Position, cameraFinalTargetVector, cameraRotatedUpVector);

            //billboard
            this.Up = cameraRotatedUpVector;
            this.Right = Vector3.Cross(cameraRotatedTargetVector, cameraRotatedUpVector);
        }
    }
}
