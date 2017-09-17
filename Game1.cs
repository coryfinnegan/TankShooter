using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


namespace Game3
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Camera camera;
        VertexPositionColor[] verts;
        VertexBuffer vertexBuffer;
        Matrix worldTranslation = Matrix.Identity;
        Matrix worldRotation = Matrix.Identity;
        VertexPositionColor[] cverts;
        VertexBuffer cvb;
        BasicEffect ceffect;
        int gDim = 200;
        //int y_draw_pos = -5;
        //Color orange = new Color(255, 165, 0);
        Matrix world = Matrix.Identity;

        //new Code
        public Vector3 CameraPosition = new Vector3(0, 20, 5);
        public Vector3 CameraDirection;
        public Vector3 CameraUp = Vector3.Up;
        float CamXAngle = 0;
        float CamYAngle = MathHelper.Pi;
        public Matrix view { get; protected set; }
        MouseState mouseState;
        MouseState prevMouseState;
        float mx, my, dmx, dmy, mScale;
        float CamXAngleSin;
        float CamXAngleCos;
        float CamYAngleSin;
        float CamYAngleCos;

        float cameraSpeed = 0.1f;
        Vector3 cameraRightAngle;
        Matrix cameraTranslation = Matrix.Identity;
       

        //Game 5 Fractal Terrain
        float[,] hts;
        Random rng;
        
        //Game 6 
        Vector3[,] normals;
        float[,] lights;
        Vector3 source = new Vector3(2, 2, 2);




        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 1200;
            graphics.PreferredBackBufferWidth = 1600;
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
            camera = new Camera(this, new Vector3(0, 0, 5),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);



            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            
            //Game6 

            
            //Initialize vertices
            verts = new VertexPositionColor[3];
            verts[0] = new VertexPositionColor(new Vector3(0, 1, 0), Color.Blue);
            verts[1] = new VertexPositionColor(new Vector3(1, -1, 0), Color.Red);
            verts[2] = new VertexPositionColor(new Vector3(-1, -1, 0), Color.Green);

            //set vertex data in VertexBuffer
            vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor),
                verts.Length, BufferUsage.None);
            vertexBuffer.SetData(verts);

            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            GraphicsDevice.RasterizerState = rs;

            //new Code Homework4
            dmx = dmy = 0;
            mScale = 0.01f;

            //Allocate Verts etc
            cverts = new VertexPositionColor[gDim * gDim * 6];
            cvb = new VertexBuffer(GraphicsDevice, typeof(VertexPositionColor), cverts.Length, BufferUsage.None);
            ceffect = new BasicEffect(GraphicsDevice);
            ceffect.VertexColorEnabled = true;

            //Homework 5 - Fill up the 2d array
            hts = new float[gDim+1 , gDim +1];
            rng = new Random(37);
            /*for (int i = 0; i <= gDim; i++)
            {
                for (int j = 0; j <= gDim; j++)
                {
                    hts[i, j] = (float)rng.NextDouble() - 4.0f;
                    //Console.WriteLine("Place in index i = " + i + " j = " + j + " value " + hts[i, j]);
                }
            }
            */
            otherCalcHeights();


            Vector3 A1;
            Vector3 B1;
            Vector3 crossProd1;
            Vector3 A2;
            Vector3 B2;
            Vector3 crossProd2;
            Vector3 A3;
            Vector3 B3;
            Vector3 crossProd3;
            Vector3 A4;
            Vector3 B4;
            Vector3 crossProd4;

            for (int i = 0; i < gDim; i++)
            {
                for (int j = 0; j < gDim; i++)
                {
                    if ((i != 0) && (j != 0))
                    {
                        A1 = new Vector3(1, hts[i, j] - hts[i - 1, j], 0);
                        B1 = new Vector3(0, hts[i, j + 1] - hts[i, j], -1);
                        crossProd1 = Vector3.Cross(A1, B1);
                        crossProd1.Normalize();

                        A2 = new Vector3(1, hts[i + 1, j] - hts[i, j], 0);
                        B2 = new Vector3(0, hts[i, j] - hts[i, j - 1], 1);
                        crossProd2 = Vector3.Cross(A2, B2);
                        crossProd2.Normalize();

                        A3 = new Vector3(-1, hts[i, j] - hts[i + 1, j], 0);
                        B3 = new Vector3(0, hts[i, j + 1] - hts[i, j], 1);
                        crossProd3 = Vector3.Cross(A3, B3);
                        crossProd3.Normalize();

                        A4 = new Vector3(1, hts[i, j] - hts[i - 1, j], 0);
                        B4 = new Vector3(0, hts[i, j] - hts[i, j + 1], -1);
                        crossProd4 = Vector3.Cross(A4, B4);
                        crossProd4.Normalize();

                        normals[i, j] = (crossProd1 + crossProd2 + crossProd3 + crossProd4) / 4;

                        lights[i, j] = Vector3.Dot(normals[i, j], source);
                    }
                }
            }

            //Homework 3 - Make the Square
            int ind = 0;
            for (int x = 0; x < gDim; x++)
            {

                for (int z = 0; z < gDim; z++)
                {
                   
                    cverts[ind++] = new VertexPositionColor(new Vector3(x, hts[x,z], z), new Color(255, 0, 0) * lights[x,z]);
                    cverts[ind++] = new VertexPositionColor(new Vector3(x + 1, hts[x + 1, z], z), new Color(255, 0, 0) * lights[x, z]);
                    cverts[ind++] = new VertexPositionColor(new Vector3(x + 1, hts[x + 1, z + 1], z + 1), new Color(0, 0, 255) * lights[x, z]);
                    cverts[ind++] = new VertexPositionColor(new Vector3(x + 1, hts[x + 1, z + 1], z + 1), new Color(0, 165, 255) * lights[x, z]);
                    cverts[ind++] = new VertexPositionColor(new Vector3(x, hts[x, z + 1], z + 1), new Color(255, 165, 0) * lights[x, z]);
                    cverts[ind++] = new VertexPositionColor(new Vector3(x, hts[x, z], z), new Color(255, 165, 0) * lights[x, z]);
                    //Console.WriteLine("Triangle Made at " + x + " " + z);
                }
            
            }
            //homework6 - Normalize 

            
            

    
        }


        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Translation
            KeyboardState keyboardState = Keyboard.GetState();
            KeyboardState prekeyboardState = keyboardState;


            //CameraDirection.Y = hts[(int)CameraPosition.X, (int)CameraPosition.Y];
            cameraRightAngle = Vector3.Cross(CameraUp, CameraDirection);
             //Rotation Homework 3 - 
            if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
            {
                CameraPosition.X += (cameraRightAngle.X * cameraSpeed);
                CameraPosition.Z += (cameraRightAngle.Z * cameraSpeed);
            }

            if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
            {
                CameraPosition.X -= (cameraRightAngle.X * cameraSpeed);
                CameraPosition.Z -= (cameraRightAngle.Z * cameraSpeed);
            }

            if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
            {
                CameraPosition.X += (CameraDirection.X * cameraSpeed);
                CameraPosition.Z += (CameraDirection.Z * cameraSpeed);
                Console.WriteLine(CameraPosition.ToString());
            }

            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {

                CameraPosition.X -= (CameraDirection.X * cameraSpeed);
                CameraPosition.Y -= (CameraDirection.Y * cameraSpeed);
            }
                
            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();
            
            if ((CameraPosition.X > 0) && (CameraPosition.X < gDim -1) && (CameraPosition.Z > 0) && (CameraPosition.Z < gDim)) {
                
                //CameraPosition.Y = hts[(int)CameraPosition.X,(int)CameraPosition.Z] + 5;
                CameraPosition.Y = calcXLerp(CameraPosition.X, CameraPosition.Z);

            }

            //New Code Homework 4 -- Mouse Input in Update
            mouseState = Mouse.GetState();
            if (mouseState.MiddleButton == ButtonState.Pressed)
            {
                if (prevMouseState.MiddleButton == ButtonState.Pressed)
                {
                    dmx = mouseState.X - mx;
                    dmy = mouseState.Y - my;
                    CamXAngle -= dmy * mScale;
                    CamYAngle -= dmx * mScale;
                    //X Angle Calculations
                    CamXAngleSin = (float)Math.Sin(CamXAngle);
                    CamXAngleCos = (float)Math.Cos(CamXAngle);
                    //Y Angle Calculations
                    CamYAngleSin = (float)Math.Sin(CamYAngle);
                    CamYAngleCos = (float)Math.Cos(CamYAngle);


                    CameraDirection = new Vector3(CamXAngleCos * CamYAngleSin, CamXAngleSin, CamXAngleCos * CamYAngleCos);

                    // Debug.WriteLine(" x " + CamXAngle + "    y " + CamYAngle);
                }
                mx = mouseState.X;
                my = mouseState.Y;



            }
            prevMouseState = mouseState;

            //Creating view matrix from position direction and up
            view = Matrix.CreateLookAt(CameraPosition, CameraPosition + CameraDirection, CameraUp);
            //

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            ceffect.World = world;
            ceffect.View = view;
            ceffect.Projection = camera.projection;
            foreach (EffectPass pass in ceffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>
                (PrimitiveType.TriangleList, cverts, 0, gDim * gDim * 2);
            }
            base.Draw(gameTime);
        }

        public void otherCalcHeights()
        {
            Texture2D heightMap = Content.Load<Texture2D>(@"Textures\shatteredworlds3");
            // can be .bmp, .dds, .dib, .hdr, .jpg, .pfm, .png, .ppm, .tga

            int terrainWidth = heightMap.Width;
            int terrainHeight = heightMap.Height;

            Color[] heightMapColors = new Color[terrainWidth * terrainHeight];
            heightMap.GetData(heightMapColors);

            for (int x = 0; x <= gDim; x++)
                for (int y = 0; y <= gDim; y++)
                {
                    if ((x < terrainWidth) && (y < terrainHeight))
                    {
                        hts[x, y] = heightMapColors[x + y * terrainWidth].R / 5.0f;
                    }
                    else
                    {
                        hts[x, y] = 0.0f;
                    }
                }
        }

        public float calcXLerp(float xPosition, float zPosition)
        {
            //Variables
            float yPosition;
            float findex_X = xPosition;
            int lowIndex_X = (int)findex_X;
            int highIndex_X = lowIndex_X + 1;
            float findex_Z = zPosition;
            int lowIndex_Z = (int)findex_Z;
            int highIndex_Z = (int)findex_Z+1;

            //Percentages
            float pctg_X = findex_X - (float)(int)findex_X;
            float pctg_Z = findex_Z - (float)(int)findex_Z;

            //Calculate Lerps
            float xlerp = (1.0f - pctg_X) * hts[lowIndex_X, highIndex_Z] + pctg_X * hts[highIndex_X,highIndex_Z];
            float zlerp = (1.0f - pctg_X) * hts[lowIndex_X, lowIndex_Z] + pctg_X * hts[highIndex_X, lowIndex_Z];
            float ylerp = (1.0f - pctg_Z) * zlerp + pctg_Z * xlerp;
            yPosition = ylerp + 10;
            //Calculate Blerp
           
            return yPosition;
        }
        

    }


}
