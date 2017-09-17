using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;


namespace Game1
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        public Camera camera;
        VertexPositionColor[] verts;
        VertexBuffer vertexBuffer;
        Matrix worldTranslation = Matrix.Identity;
        Matrix worldRotation = Matrix.Identity;
        VertexPositionColor[] cverts;
        VertexBuffer cvb;
        BasicEffect ceffect;
        public int gDim = 100;
        public int numPrim;
        //int y_draw_pos = -5;
        //Color orange = new Color(255, 165, 0);
        public Matrix world = Matrix.Identity;

        //new Code
        public Vector3 CameraPosition = new Vector3(100,100, 100);
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

        float cameraSpeed = 1f;
        Vector3 cameraRightAngle;
        Matrix cameraTranslation = Matrix.Identity;
       
        //Game 5 Fractal Terrain
        float[,] hts;
        Random rng;
        
        //Game 6 
        public Vector3[,] normals;
        float[,] lights;
        public Vector3 source = new Vector3(0, 1, 0);
        Vector3 lDir = new Vector3(0, 1, 0);
        
        //Vector4 lightPos = new Vector4(2, 2, 2, 2);

        Boolean flying;
        Sphere aSphere;
        Sphere goalSphere;
        Tank tank;
        Boolean tankBool = true;
        //Sphere[] spheres;
        
        float ta;

        VertexPositionNormalTexture[] pnverts;
        Texture2D groundTexture;
        Effect DiffuseTextureEffect;
        //float texScale = 1f;
        float tTime;

        
        public Vector4 Ambient;
        float ambientMult = 0.01f;
        Vector4 LightColor;
        Vector4 camPos;
        public Vector3 lightColor = new Vector3( 0.2f ,0.2f, 0.2f);

        Vector3 cameraReference = new Vector3(0, 0, 1);
        SpriteFont font;
        float windowDistance;
        float xCamera;
        float yCamera;
        float zCamera;
        Vector3 mouseClickVector3;
        Matrix inverseView;
        Vector3 mouseView;
        float oppositeSide;

        Vector3 vDir = new Vector3();
        Vector3 sphereToClickVector3 = new Vector3();
        Vector3 sphereToClickNormalVector3 = new Vector3();

        Vector3 mouseClickNormalVector3 = new Vector3();
        float cosBetweenMouseAndSphere;
        float angle;
        Vector3 vDirNormal;

        Vector3 rP = new Vector3();
        Vector3 Q = new Vector3();
        float rQP;
        float distanceBetween;
        
        //Enemies
        int totalEnemies = 5;
        List<Enemy> enemies;

        //Bullets
        public List<Bullet> bullets;

        //EndGame
        public Boolean endGame = false;
        KeyboardState prekeyboardState;
        KeyboardState keyboardState = Keyboard.GetState();

        //random
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();



        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.IsFullScreen = false;
            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 800;
            Content.RootDirectory = "Content";
            goalSphere = new Sphere(this);
            bullets = new List<Bullet>();
            enemies = new List<Enemy>(totalEnemies);



            tank = new Tank(this);
            //Enemy
            for (int i = 0; i < totalEnemies; i++)
            {
                Enemy enemy = new Enemy(this);
                enemies.Add(enemy);
          
            }
            //Bullets
            foreach (Enemy enemy in enemies)
            {
                Components.Add(enemy);
            }
            foreach (Bullet bullet in bullets)
            {
                Components.Add(bullet);
            }

             
            Components.Add(goalSphere);
            Components.Add(tank);
            goalSphere.SetLightDir(source);
            //tank.setLightPos(lightPos);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            camera = new Camera(this, new Vector3(50, 40, 50),
                Vector3.Zero, Vector3.Up);
            Components.Add(camera);
            groundTexture = Content.Load<Texture2D>(@"Textures\grass3");
            DiffuseTextureEffect = Content.Load<Effect>("Effects/DiffuseTexture");
            Ambient = new Vector4(ambientMult, ambientMult, ambientMult, 1f);
            this.IsMouseVisible = true;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            font = Content.Load<SpriteFont>("MyFont");
            
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            
          
            
 
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
            pnverts = new VertexPositionNormalTexture[gDim * gDim * 6];      
            otherCalcHeights();
            source.Normalize();

            normals = new Vector3[gDim+1, gDim+1];
            lights = new float[gDim+1, gDim+1];

            for (int i = 0; i < gDim; i++)
            {
                for (int j = 0; j < gDim; j++)
                {
                    if ((i != 0) && (j != 0))
                    {
                      
                        normals[i,j] = Vector3.Cross(new Vector3(0,hts[i,j+1] - hts[i,j], 1), new Vector3(1, hts[i,j] - hts[i -1, j], 0));
                        //Console.WriteLine(normals.ToString());

                        lights[i, j] = Vector3.Dot(normals[i, j], source);
                    }
                }
            }

            //Spawn Tank
            tank.SetTranslation(new Vector3(2,hts[2,2],2),0);

            

            //Spawn Enemies
            foreach (Enemy enemy in enemies)
            {
                enemy.randomSpawn(RandomNumber(0, gDim), RandomNumber(0, gDim));
                Console.Write("Enemy spawned");


            }
            
             //Make the Square
            float uvTimes = 1.0f / gDim;
            int ind = 0;
            for (int x = 0; x < gDim; x++)
            {

                for (int z = 0; z < gDim; z++)
                {
                    pnverts[ind].TextureCoordinate = new Vector2(x * uvTimes, z * uvTimes);
                    pnverts[ind].Position = new Vector3(x, hts[x, z], z);
                    pnverts[ind++].Normal = normals[x, z];

                    pnverts[ind].TextureCoordinate = new Vector2((x + 1) * uvTimes, z * uvTimes);
                    pnverts[ind].Position = new Vector3(x +1, hts[x + 1, z], z);
                    pnverts[ind++].Normal = normals[x+1, z];

                    pnverts[ind].TextureCoordinate = new Vector2((x + 1) * uvTimes, (z + 1) * uvTimes);
                    pnverts[ind].Position = new Vector3(x + 1, hts[x + 1, z + 1], z + 1);
                    pnverts[ind++].Normal = normals[x+1, z+1];

                    pnverts[ind].TextureCoordinate = new Vector2((x + 1) * uvTimes, (z + 1) * uvTimes);
                    pnverts[ind].Position = new Vector3(x + 1, hts[x + 1, z + 1], z + 1);
                    pnverts[ind++].Normal = normals[x+1, z+1];

                    pnverts[ind].TextureCoordinate = new Vector2(x * uvTimes, (z + 1) * uvTimes);
                    pnverts[ind].Position = new Vector3(x, hts[x, z + 1], z + 1);
                    pnverts[ind++].Normal = normals[x, z+1];

                    pnverts[ind].TextureCoordinate = new Vector2(x * uvTimes, z * uvTimes);
                    pnverts[ind].Position = new Vector3(x, hts[x, z], z);
                    pnverts[ind++].Normal = normals[x, z];
                 
                }
            }
            setGoalLocation();

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
            //moveKeyboardCamera();
           
            //moveMouseCamera();

            endGameFunc();
            positionCamera();
            if (!endGame)
            {
                moveTank();
                tank.moveTankY();
                checkSphere();
                detectBulletCollision();
                //detectPlayerTankCollision();

            }
            
            //Creating view matrix from position direction and up
            view = Matrix.CreateLookAt(CameraPosition, CameraPosition + CameraDirection, CameraUp);
            //cameraClick();
            /*
            tTime += (float)(gameTime.ElapsedGameTime.Milliseconds) / 1000.0f;

            source.Y = (float)(Math.Cos(MathHelper.TwoPi * (tTime / 20.0f)));
            if (source.Y < 0) lDir.Y *= -1.0f;
            source.Z = (float)(Math.Cos(MathHelper.TwoPi * (tTime / 40.0f)));
            source.X = (float)(Math.Sin(MathHelper.TwoPi * (tTime / 40.0f)));
            source.Normalize();
             */
            
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Graphics
            GraphicsDevice.Clear(Color.Black);
            GraphicsDevice.SetVertexBuffer(vertexBuffer);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;    
 
            //Objects
            goalSphere.setGraphicsVar(camera, world, view);

            foreach (Enemy enemy in enemies)
            {
                enemy.setCamera(view, camera.projection, CameraPosition);
            }
            
            
            foreach (Bullet bullet in bullets){
                bullet.setGraphicsVar(camera, world, view);
            }
            tank.setCamera(view, camera.projection, CameraPosition);

            foreach (EffectPass pass in ceffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionColor>
                (PrimitiveType.TriangleList, cverts, 0, gDim * gDim * 2);
            }
            DiffuseTextureEffect.Parameters["World"].SetValue(world);
            DiffuseTextureEffect.Parameters["View"].SetValue(view);
            DiffuseTextureEffect.Parameters["Projection"].SetValue(camera.projection);
            DiffuseTextureEffect.Parameters["LightDir"].SetValue(source); // need this later
            DiffuseTextureEffect.Parameters["ModelTexture"].SetValue(groundTexture);
            DiffuseTextureEffect.Parameters["Ambient"].SetValue(Ambient);
            DiffuseTextureEffect.Parameters["CameraPos"].SetValue(CameraPosition);
            DiffuseTextureEffect.Parameters["LightColor"].SetValue(lightColor);




            foreach (EffectPass pass in DiffuseTextureEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>
                (PrimitiveType.TriangleList, pnverts, 0, gDim * gDim * 2);
            }


            DrawText();
            base.Draw(gameTime);
        }

        public void otherCalcHeights()
        {
            Texture2D heightMap = Content.Load<Texture2D>(@"Textures\heightmap3");
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
            float findex_X = xPosition;
            int lowIndex_X = (int)findex_X;
            int highIndex_X = lowIndex_X + 1;
            float findex_Z = zPosition;
            int lowIndex_Z = (int)findex_Z;
            int highIndex_Z = (int)findex_Z+1;
            float pctg_X = findex_X - (float)(int)findex_X;
            float pctg_Z = findex_Z - (float)(int)findex_Z;
            float xlerp = (1.0f - pctg_X) * hts[lowIndex_X, highIndex_Z] + pctg_X * hts[highIndex_X,highIndex_Z];
            float zlerp = (1.0f - pctg_X) * hts[lowIndex_X, lowIndex_Z] + pctg_X * hts[highIndex_X, lowIndex_Z];
            float ylerp = (1.0f - pctg_Z) * zlerp + pctg_Z * xlerp;
            return ylerp;
        }

        public void setCamera(BasicEffect effect)
        {
            effect.View = view;
            effect.Projection = camera.projection;
        }

        public Vector3 getNormals(float xPosition, float zPosition)
        {
            //Beta Code
            //float findex_X = ((xPosition - ))

            //Code that works
            float findex_X = xPosition;
            int lowIndex_X = (int)findex_X;
            int highIndex_X = lowIndex_X + 1;
            float findex_Z = zPosition;
            int lowIndex_Z = (int)findex_Z;
            int highIndex_Z = (int)findex_Z + 1;
            float pctg_X = findex_X - (float)(int)findex_X;
            float pctg_Z = findex_Z - (float)(int)findex_Z;
            Vector3 normalA, normalB, normalC;
            normalA = (1.0f - pctg_X) * normals[lowIndex_X, highIndex_Z] + (pctg_X * normals[highIndex_X, highIndex_Z]);
            normalB = (1.0f - pctg_X) * normals[lowIndex_X, lowIndex_Z] + (pctg_X * normals[highIndex_X, lowIndex_Z]);  
            normalC = (1.0f - pctg_Z) * normalB + pctg_Z * normalA;
            
            normalC.Normalize();
            
            return normalC;
        }
        public void moveKeyboardCamera()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            KeyboardState prekeyboardState = keyboardState;
            cameraRightAngle = Vector3.Cross(CameraUp, CameraDirection);

            if (keyboardState.IsKeyDown(Keys.F))
            {
                //prekeyboardState = keyboardState;
                flying = true;
                tankBool = false;
            }
            if (keyboardState.IsKeyDown(Keys.Space))
            {
                //prekeyboardState = keyboardState;
                flying = false;
                tankBool = false;
            }

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
                CameraPosition.Y += (CameraDirection.Y * cameraSpeed);

            }

            if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
            {
                CameraPosition.X -= (CameraDirection.X * cameraSpeed);
                CameraPosition.Y -= (CameraDirection.Y * cameraSpeed);
                //CameraPosition.Z += (CameraDirection.Z * cameraSpeed); 
            }
            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            if (flying == false)
            {
                if ((CameraPosition.X > 0) && (CameraPosition.X < gDim - 1) && (CameraPosition.Z > 0) && (CameraPosition.Z < gDim))
                {

                    CameraPosition.Y = calcXLerp(CameraPosition.X, CameraPosition.Z) + 10;
                }
            }
            
        }
        public void moveMouseCamera()
        {
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
                }
                mx = mouseState.X;
                my = mouseState.Y;
            }
            prevMouseState = mouseState;
        }
        public void moveTank()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            
                  
            if (keyboardState.IsKeyDown(Keys.A))
            {
                
                tank.setRightAngle();
            }
            if (keyboardState.IsKeyDown(Keys.D))
            {
                tank.setLeftAngle();
            }

            if (keyboardState.IsKeyDown(Keys.W))
            {
                tank.setForward();

            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                tank.setBackward();
                
            }
            if (keyboardState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }
              if (keyboardState.IsKeyDown(Keys.Left))
            {
                tank.setTankTopLeftAngle();
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                tank.setTankTopRightAngle();
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                tank.setCanonForward();
            }

            if (keyboardState.IsKeyDown(Keys.Down))
            {
                tank.setCanonBackward();
            }
            if (keyboardState.IsKeyDown(Keys.Space) && prekeyboardState.IsKeyUp(Keys.Space))
            {
                // && !endGame
                shoot();
            }
            prekeyboardState = keyboardState;

            
        }
        public void positionCamera()
        {
            cameraRightAngle = Vector3.Cross(CameraUp, CameraDirection);
            CameraPosition = tank.getPosition();
            CameraPosition.X -= tank.getDirection().X * 50.0f;
            CameraPosition.Z -= tank.getDirection().Z * 50.0f;
            CameraPosition.Y += 50.0f;
            CameraDirection = Vector3.Normalize(tank.getPosition() - CameraPosition);
            CameraPosition.Y += 50.0f;
        }
        private void DrawText()
        {    
            mouseState = Mouse.GetState();
            mx = mouseState.X;
            my = mouseState.Y;
            prevMouseState = mouseState;
            spriteBatch.Begin();
            //spriteBatch.DrawString(font, "Mx My:" + mx + " " + my + "\n", new Vector2(10, 30), Color.Black);
            //spriteBatch.DrawString(font, "\nCamera Vector: X: " + mouseView.X + " Y: " + mouseView.Y + " Z: " + mouseView.Z + " \n", new Vector2(10, 30), Color.Black);
            //spriteBatch.DrawString(font, "\n\nMouseClickVector Vector: X " + mouseClickVector3.X + " Y: " + mouseClickVector3.Y + " Z: " + mouseClickVector3.Z + " \n\n\n", new Vector2(10, 30), Color.Black);
            //spriteBatch.DrawString(font, "\n\n\nTank Angle: X " + tank.angle + " \n", new Vector2(10, 30), Color.Black);
            if (endGame)
            {
                spriteBatch.DrawString(font, "           Game Over!\nPress Space to Restart", new Vector2(300, 100), Color.White);
            }


            spriteBatch.End();
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        private void cameraClick()
        {
            //Calculate Window Distance
            /*
            mouseState = Mouse.GetState();
            prevMouseState = mouseState;
            mx = mouseState.X;
            my = mouseState.Y;
            windowDistance = 1.0f * (float)Math.Tan(MathHelper.PiOver4 / 2);
            //calculate y Camera
            yCamera = (my / 600) * 2f;
            yCamera = 1.0f - yCamera;
            //Calculate X Camera
            xCamera = (mx / 800) * (2f * 3 / 4);
            xCamera = 1.0f - xCamera;
            inverseView = Matrix.Invert(view);
            mouseView = new Vector3(xCamera, yCamera, windowDistance);
            mouseClickVector3 = Vector3.Transform(mouseView, inverseView);
            mouseClickNormalVector3 = mouseClickVector3;
            mouseClickNormalVector3.Normalize();
            Vector3 QPoint = new Vector3();
            vDir = mouseClickVector3 - CameraPosition;
            vDirNormal = vDir;
            vDirNormal.Normalize();            
            for (int i = 0; i < totalSpheres; i++)
            {
                sphereToClickVector3 = spheres[i].position - mouseClickVector3;
                sphereToClickNormalVector3 = sphereToClickVector3;
                sphereToClickNormalVector3.Normalize();
                cosBetweenMouseAndSphere = Vector3.Dot(sphereToClickNormalVector3, vDirNormal);
                rP = sphereToClickVector3;
                Q = CameraPosition - mouseClickVector3;
                Q.Normalize();
                rQP = Vector3.Dot(rP, Q);
                QPoint = rQP * Q;
                distanceBetween = Vector3.Distance(rP, QPoint);


                //angle = (float)Math.Acos(cosBetweenMouseAndSphere);
                //oppositeSide = sphereToClickVector3.Length() * (float)Math.Sin(angle);
                

                //debug

                if (distanceBetween < 1f)
                {
                    spheres[i].speed.Y += 100;
                    Console.Write("Sphere touched");
                }//end if
            }//end For
             * */
        }//end cameraClick
        public float getHeights(float xPos, float zPos)
        {
            return hts[(int)xPos, (int)zPos];
        }
        public void setGoalLocation(){
            Random random = new Random();
            int randomX = random.Next(gDim);
            int randomZ = random.Next(gDim);
            goalSphere.SetTranslation(new Vector3(randomX, getHeights(randomX, randomZ) +3, randomZ));

        }
        public void checkSphere()
        {
            float distance = Vector3.Distance(tank.position, goalSphere.position);
            if (distance <= 2f)
            {
                endGame = true;
                Console.Write("Goal Touched");
            }
            float newDistance = distance;
 
        }
        public Tank getTank()
        {
            return tank;
        }
        public Vector3 getTankPosition()
        {
            return tank.getPosition();
        }
        public void endGameFunc()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            KeyboardState prekeyboardState = keyboardState;
            if (endGame == true)
            {
                if (keyboardState.IsKeyDown(Keys.Space)){
                    tank.SetTranslation(new Vector3(2, hts[2, 2], 2), 0);
                    foreach (Enemy enemy in enemies) {
                        enemy.randomSpawn(RandomNumber(0, gDim), RandomNumber(0, gDim));
                        endGame = false;
                    } 
                }
            }
        }
        public void shoot()
        {

            Bullet bullet = new Bullet(this);
            Components.Add(bullet);
            bullet.SetLightDir(source);
            bullet.setGraphicsVar(camera, world, view);
            bullets.Add(bullet);
            bullet.SetTranslation(new Vector3(tank.position.X, tank.position.Y + 7, tank.position.Z));
            bullet.setVelocity(tank.getTopTrans().Forward * 0.002f);  

        }
        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            {
                return random.Next(min, max);
            }
        }

        public void detectPlayerTankCollision()
        {
            List<Enemy> expiredEnemies = new List<Enemy>();

            for (int i = 0; i < totalEnemies; i++)
            {
                if (Vector3.Distance(enemies[i].getPosition(), tank.getPosition()) < 2f)
                {
                    
                    endGame = true;
                }
            }
            
        }
        public void detectBulletCollision()
        {
            List<Bullet> expiredBullets = new List<Bullet>();
            List<Enemy> expiredEnemies = new List<Enemy>();
            foreach (Enemy enemy in enemies)
            {
                foreach (Bullet bullet in bullets)
                {
                    if (Vector3.Distance(enemy.getPosition(), bullet.getPosition()) < 2f){
                        expiredBullets.Add(bullet);
                        expiredEnemies.Add(enemy);
                    }
                }
            }
            foreach (Enemy expiredEnemy in expiredEnemies)
            {
                foreach (Bullet expiredBullet in expiredBullets)
                {

                    bullets.Remove(expiredBullet);
                     enemies.Remove(expiredEnemy);
                     Console.Write("Tank Hit");
                }
            }
        }
    }//end Game
}//end nameSpace
