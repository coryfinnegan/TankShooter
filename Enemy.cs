using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;



namespace Game1
{

    public class Enemy : Microsoft.Xna.Framework.DrawableGameComponent
    {

        VertexPositionNormalTexture[] pnverts;
        VertexPositionNormalTexture[] turretVerts;
        VertexPositionNormalTexture[] cannonVerts;

        Texture2D texture;
        Effect PointDiffSPecTextureEffect;

        Vector3[] pts;
        Matrix worldTrans = Matrix.Identity;
        Matrix worldRot = Matrix.Identity;
        Game1 myGame;
        public Vector3 position;
        public Vector3 direction = new Vector3(0, 0, 1);
        float gravity = -0.5f;
        Vector3 speed = new Vector3(0, 0, 0);
        Matrix view = Matrix.Identity;
        Matrix proj = Matrix.Identity;
        Vector4 lPos;
        Vector2[] tcs;
        Vector4 CamPos;
        Vector4 Ambient;
        Vector4 LightColor = new Vector4(1, 1, 1, 1);
        float rot = -10f;
        public float angle = 0.1f;
        Vector3 up;
        Vector3 right;
        Vector3 forward;
        Matrix world;
        float topTankAngle = 0.1f;
        float cannonAngleFloat = 0.1f;
        Matrix topAngle;
        Matrix topTrans;

        Matrix cannonAngle;
        Matrix cannonTrans;

        public Vector3 forwardVector;
        public Vector3 positionAhead;
        public Vector3 positionBehind;
        public float angleCheckFloat;
        public float tankSpeed = 15f;
        float TankTurnSpeed = 0.025f;
        


        public Enemy(Game game)
            : base(game)
        {
            

            myGame = (Game1)game;
            pnverts = new VertexPositionNormalTexture[36];
            turretVerts = new VertexPositionNormalTexture[36];
            cannonVerts = new VertexPositionNormalTexture[36];

            makeTankPart(2f, 0, 3f, 0, pnverts);
            makeTankPart(1.2f, 1f, 1.5f, 0, turretVerts);
            makeCanon(cannonVerts);

        }

        public void SetTranslation(Vector3 trns, float rot)
        {  // set sphere location
            position = trns;

            worldTrans = Matrix.CreateTranslation(position);
        }


        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>(@"Textures\rust");
            PointDiffSPecTextureEffect = Game.Content.Load<Effect>("Effects/DiffuseTexture");

            Ambient.X = 0.2f; // ambient light
            Ambient.Y = 10.0f; // specular exponent
            Ambient.Z = 1.0f; // specular intensity

            LightColor.X = 0.6f;
            LightColor.Y = 0.4f;
            LightColor.Z = 0.8f;
            LightColor.W = 0.0f;
        }


        public override void Update(GameTime gameTime)
        {
            //position += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            //direction.X = (float)Math.Sin(MathHelper.ToRadians(angle));
            //direction.Z = (float)Math.Cos(MathHelper.ToRadians(angle));
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (!myGame.endGame)
            {
                followPlayer(myGame.getTank());
                moveTankOrient();
                detectPlayer();
            }

            //Turret    
            topAngle = Matrix.CreateRotationY(topTankAngle);
            //topTrans = Matrix.CreateTranslation(position);
            Matrix temp = topAngle * world;
            topTrans = temp;

            //Cannon
            cannonAngle = Matrix.CreateRotationX(cannonAngleFloat);

            //cannonTrans = Matrix.CreateTranslation(position);
            Matrix temp_cannon = cannonAngle * topAngle * world;
            cannonTrans = temp_cannon;
            base.Update(gameTime);
        }
        public void setCamera(Matrix viewin, Matrix projin, Vector3 CPin)
        {
            view = viewin;
            proj = projin;
            CamPos.X = CPin.X;
            CamPos.Y = CPin.Y;
            CamPos.Z = CPin.Z;
            CamPos.W = 1.0f;
        }
        public void setLightPos(Vector4 pin)
        {
            lPos = pin;
        }
        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            PointDiffSPecTextureEffect.Parameters["World"].SetValue(world);

            PointDiffSPecTextureEffect.Parameters["View"].SetValue(myGame.view);
            PointDiffSPecTextureEffect.Parameters["Projection"].SetValue(myGame.camera.projection);
            PointDiffSPecTextureEffect.Parameters["LightDir"].SetValue(myGame.source);
            PointDiffSPecTextureEffect.Parameters["ModelTexture"].SetValue(texture);
            PointDiffSPecTextureEffect.Parameters["CameraPos"].SetValue(myGame.CameraPosition);
            PointDiffSPecTextureEffect.Parameters["Ambient"].SetValue(myGame.Ambient);
            PointDiffSPecTextureEffect.Parameters["LightColor"].SetValue(myGame.lightColor);

            //Draw the tank
            PointDiffSPecTextureEffect.Parameters["World"].SetValue(world);

            foreach (EffectPass pass in PointDiffSPecTextureEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                {
                    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>
                       (PrimitiveType.TriangleList, pnverts, 0, 12);
                }
            }

            PointDiffSPecTextureEffect.Parameters["World"].SetValue(topTrans);
            foreach (EffectPass pass in PointDiffSPecTextureEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                {
                    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>
                       (PrimitiveType.TriangleList, turretVerts, 0, 12);
                }
            }
            PointDiffSPecTextureEffect.Parameters["World"].SetValue(cannonTrans);
            foreach (EffectPass pass in PointDiffSPecTextureEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                {
                    GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>
                       (PrimitiveType.TriangleList, cannonVerts, 0, 12);
                }
            }
            base.Draw(gameTime);
        }
        public Vector3 getPosition()
        {
            return position;
        }
        public Vector3 getDirection()
        {
            return direction;
        }
        public void setAngle(float angleIn)
        {
            this.angle = angleIn + this.angle;
        }
        public float getAngle()
        {
            return angle;
        }
        public void setLeftAngle()
        {
            angle -= 0.5f;
        }
        public void setRightAngle()
        {
            angle += 0.5f;
        }
        public void setForward()
        {
                position.X += (direction.X / (tankSpeed));
                position.Z += (direction.Z / (tankSpeed));

        }
        public void setBackward()
        {

            position.X -= direction.X;
            position.Z -= direction.Z;


        }
        public void moveTankY()
        {
            if ((position.X > 0) && (position.X < myGame.gDim - 1) && (position.Z > 0) && (position.Z < myGame.gDim))
            {

                position.Y = myGame.calcXLerp(position.X, position.Z) + 2;

            }
        }
        public void moveTankOrient()
        {


            Matrix tempMatrix = Matrix.Identity;
            Vector3 newDirectionY = myGame.getNormals(position.X, position.Z);


            Vector3 newDirectionX = Vector3.Cross(newDirectionY, direction);
            newDirectionX.Normalize();

            Vector3 newDirectionZ = Vector3.Cross(newDirectionX, newDirectionY);
            newDirectionZ.Normalize();

            tempMatrix.Right = newDirectionX;
            tempMatrix.Up = newDirectionY;
            tempMatrix.Forward = newDirectionZ;
            forwardVector = newDirectionZ;

            world = tempMatrix * worldTrans;



        }
        public void makeTankPart(float x, float y, float z, float zAdd, VertexPositionNormalTexture[] inTextureArray)
        {
            int iv = 0;
            pts = new Vector3[8];
            Vector3[] nms = new Vector3[8];
            tcs = new Vector2[8];


            pts[0].X = -1.0f;
            pts[0].Y = -1.0f;
            pts[0].Z = -1.0f;
            pts[1].X = -1.0f;
            pts[1].Y = 1.0f;
            pts[1].Z = -1.0f;
            pts[2].X = -1.0f;
            pts[2].Y = -1.0f;
            pts[2].Z = 1.0f;
            pts[3].X = -1.0f;
            pts[3].Y = 1.0f;
            pts[3].Z = 1.0f;
            pts[4].X = 1.0f;
            pts[4].Y = -1.0f;
            pts[4].Z = -1.0f;
            pts[5].X = 1.0f;
            pts[5].Y = 1.0f;
            pts[5].Z = -1.0f;
            pts[6].X = 1.0f;
            pts[6].Y = -1.0f;
            pts[6].Z = 1.0f;
            pts[7].X = 1.0f;
            pts[7].Y = 1.0f;
            pts[7].Z = 1.0f;

            for (int i = 0; i < 8; i++)
            {
                pts[i].Z *= z;
                pts[i].Z += zAdd;
                pts[i].X *= x;
                pts[i].Y += y;

            }

            nms[0].X = 0.0f;
            nms[0].Y = 1.0f;
            nms[0].Z = 0.0f;
            nms[1].X = 0.0f;
            nms[1].Y = -1.0f;
            nms[1].Z = 0.0f;
            nms[2].X = 1.0f;
            nms[2].Y = 0.0f;
            nms[2].Z = 0.0f;
            nms[3].X = -1.0f;
            nms[3].Y = 0.0f;
            nms[3].Z = 0.0f;
            nms[4].X = 0.0f;
            nms[4].Y = 0.0f;
            nms[4].Z = 1.0f;
            nms[5].X = 0.0f;
            nms[5].Y = 0.0f;
            nms[5].Z = -1.0f;

            tcs[0].X = 0.0f;
            tcs[0].Y = 0.0f;
            tcs[1].X = 1.0f;
            tcs[1].Y = 0.0f;
            tcs[2].X = 0.0f;
            tcs[2].Y = 1.0f;
            tcs[3].X = 1.0f;
            tcs[3].Y = 1.0f;

            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[0], nms[3], tcs[1]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[1], nms[3], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[2], nms[3], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[2], nms[3], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[1], nms[3], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[3], nms[3], tcs[2]);

            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[4], nms[1], tcs[1]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[0], nms[1], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[6], nms[1], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[6], nms[1], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[0], nms[1], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[2], nms[1], tcs[2]);

            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[7], nms[0], tcs[1]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[3], nms[0], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[5], nms[0], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[5], nms[0], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[3], nms[0], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[1], nms[0], tcs[2]);

            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[4], nms[5], tcs[1]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[0], nms[5], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[5], nms[5], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[5], nms[5], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[0], nms[5], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[1], nms[5], tcs[2]);

            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[6], nms[4], tcs[1]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[7], nms[4], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[2], nms[4], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[2], nms[4], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[7], nms[4], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[3], nms[4], tcs[2]);

            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[4], nms[2], tcs[1]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[6], nms[2], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[5], nms[2], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[5], nms[2], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[6], nms[2], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[7], nms[2], tcs[2]);

        }
        public void makeCanon(VertexPositionNormalTexture[] inTextureArray)
        {
            int iv = 0;
            pts = new Vector3[8];
            Vector3[] nms = new Vector3[8];
            tcs = new Vector2[8];


            pts[0].X = -1.0f;
            pts[0].Y = -1.0f;
            pts[0].Z = -1.0f;
            pts[1].X = -1.0f;
            pts[1].Y = 1.0f;
            pts[1].Z = -1.0f;
            pts[2].X = -1.0f;
            pts[2].Y = -1.0f;
            pts[2].Z = 1.0f;
            pts[3].X = -1.0f;
            pts[3].Y = 1.0f;
            pts[3].Z = 1.0f;
            pts[4].X = 1.0f;
            pts[4].Y = -1.0f;
            pts[4].Z = -1.0f;
            pts[5].X = 1.0f;
            pts[5].Y = 1.0f;
            pts[5].Z = -1.0f;
            pts[6].X = 1.0f;
            pts[6].Y = -1.0f;
            pts[6].Z = 1.0f;
            pts[7].X = 1.0f;
            pts[7].Y = 1.0f;
            pts[7].Z = 1.0f;
            //makeTankPart(0.3fx, 2fy, 2fz, -2zadd, cannonVerts);
            for (int i = 0; i < 8; i++)
            {
                pts[i].Z *= 2f;
                pts[i].Z += -2f;
                pts[i].X *= 0.3f;
                pts[i].Y += 7;
                pts[i].Y *= 0.2f;

            }

            nms[0].X = 0.0f;
            nms[0].Y = 1.0f;
            nms[0].Z = 0.0f;
            nms[1].X = 0.0f;
            nms[1].Y = -1.0f;
            nms[1].Z = 0.0f;
            nms[2].X = 1.0f;
            nms[2].Y = 0.0f;
            nms[2].Z = 0.0f;
            nms[3].X = -1.0f;
            nms[3].Y = 0.0f;
            nms[3].Z = 0.0f;
            nms[4].X = 0.0f;
            nms[4].Y = 0.0f;
            nms[4].Z = 1.0f;
            nms[5].X = 0.0f;
            nms[5].Y = 0.0f;
            nms[5].Z = -1.0f;

            tcs[0].X = 0.0f;
            tcs[0].Y = 0.0f;
            tcs[1].X = 1.0f;
            tcs[1].Y = 0.0f;
            tcs[2].X = 0.0f;
            tcs[2].Y = 1.0f;
            tcs[3].X = 1.0f;
            tcs[3].Y = 1.0f;

            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[0], nms[3], tcs[1]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[1], nms[3], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[2], nms[3], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[2], nms[3], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[1], nms[3], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[3], nms[3], tcs[2]);

            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[4], nms[1], tcs[1]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[0], nms[1], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[6], nms[1], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[6], nms[1], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[0], nms[1], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[2], nms[1], tcs[2]);

            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[7], nms[0], tcs[1]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[3], nms[0], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[5], nms[0], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[5], nms[0], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[3], nms[0], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[1], nms[0], tcs[2]);

            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[4], nms[5], tcs[1]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[0], nms[5], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[5], nms[5], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[5], nms[5], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[0], nms[5], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[1], nms[5], tcs[2]);

            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[6], nms[4], tcs[1]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[7], nms[4], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[2], nms[4], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[2], nms[4], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[7], nms[4], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[3], nms[4], tcs[2]);

            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[4], nms[2], tcs[1]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[6], nms[2], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[5], nms[2], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[5], nms[2], tcs[0]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[6], nms[2], tcs[3]);
            inTextureArray[iv++] = new VertexPositionNormalTexture(pts[7], nms[2], tcs[2]);

        }
        public void setTankTopLeftAngle()
        {
            topTankAngle -= 0.1f;
        }
        public void setTankTopRightAngle()
        {
            topTankAngle += 0.1f;
        }
        public void setCanonForward()
        {
            if (cannonAngleFloat <= 0.3f && cannonAngleFloat >= -0.3f)
            {
                cannonAngleFloat += 0.01f;
                Console.Write(cannonAngleFloat);
            }
            else if (cannonAngleFloat > 0.3)
            {
                cannonAngleFloat = 0.3f;
            }
            else if (cannonAngleFloat < -0.3f)
            {
                cannonAngleFloat = -0.3f;
            }

        }
        public void setCanonBackward()
        {
            if (cannonAngleFloat <= 0.3f && cannonAngleFloat >= -0.3f)
            {
                cannonAngleFloat -= 0.01f;
                Console.Write(cannonAngleFloat);

            }
            else if (cannonAngleFloat > 0.3)
            {
                cannonAngleFloat = 0.3f;
            }
            else if (cannonAngleFloat < -0.3f)
            {
                cannonAngleFloat = -0.3f;
            }
        }
        public void checkAngle()
        {
            Vector3 flatVector = new Vector3(position.X, 0, position.Z);
            flatVector.Normalize();
            Vector3 terrainVector = myGame.getNormals(position.X, position.Z);
            float angleCheckFloat = Vector3.Dot(flatVector, terrainVector);
        }
        public void followPlayer(Tank tank)
        {
          
            Vector3 targetPosition = myGame.getTankPosition();
            angle = TurnToFace(position, targetPosition, angle, TankTurnSpeed);
            direction.X = (float)Math.Cos(angle);
            direction.Z = (float)Math.Sin(angle);
            position.X += (direction.X / (tankSpeed));
            position.Z += (direction.Z / (tankSpeed));
            moveTankY();
            SetTranslation(position, MathHelper.ToDegrees(angle));
            
        }

        public float TurnToFace(Vector3 currentPosition, Vector3 targetPosition,
            float currentAngle, float turnSpeed)
        {
            float x = targetPosition.X - currentPosition.X;
            float z = targetPosition.Z - currentPosition.Z;
            float desiredAngle = (float)Math.Atan2(z, x);
            float difference = WrapAngle(desiredAngle - currentAngle);
            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);
            float wrapAngleReturn = WrapAngle(currentAngle + difference);
            return WrapAngle(desiredAngle);
        }

        private static float WrapAngle(float radians)
        {
            while (radians < -MathHelper.Pi)
            {
                radians += MathHelper.TwoPi;
            }
            while (radians > MathHelper.Pi)
            {
                radians -= MathHelper.TwoPi;
            }
            return radians;
        }
        public void randomSpawn(int x, int z)
        {

            Vector3 randomSpawn = new Vector3(x, myGame.getHeights(x, z) + 1, z);
            this.SetTranslation(randomSpawn, 0);
            Console.Write(randomSpawn);

        }
        public void detectPlayer()
        {
            if (Vector3.Distance(myGame.getTankPosition(), position) < 2f)
            {
                this.Dispose();
            }
        }

    }
}
