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

    public class Bullet : Microsoft.Xna.Framework.DrawableGameComponent
    {

        VertexPositionColor[] verts;
        int nlat, nlong;  // number of line lattitude and longitude
        BasicEffect ceffect;
        Vector3[] pts;
        float[] lts;
        float radius;
        Matrix worldTrans = Matrix.Identity;
        Matrix worldRot = Matrix.Identity;
        Game1 myGame;
        public Vector3 position;
        float gravity = -0.5f;
        public Vector3 speed = new Vector3(0, 0, 0);
        Vector3 normal = new Vector3(0, 0, 0);
        static public Boolean bounce = false; // static: one variable shared by all instances of sphere
        Boolean dropFlag;
        Vector3[,] normals;
        VertexPositionNormalTexture[] pnverts;
        Texture2D texture;
        Effect DiffuseTexture;
        Camera camera;
        Matrix world;
        Matrix view;
        Vector3 LightDir = new Vector3(2, 2, 2);
        Vector4 Ambient;
        float ambientMult = 0.5f;
        float angle = 0f;
        Vector3 bulletVelocity;




        public Bullet(Game game)
            : base(game)
        {

            myGame = (Game1)game;
            nlat = 10;
            nlong = 20;
            radius = 1.0f;
            Ambient = new Vector4(ambientMult, ambientMult, ambientMult, 1);


            verts = new VertexPositionColor[nlat * nlong * 6];
            pnverts = new VertexPositionNormalTexture[nlat * nlong * 6];

            int iv = 0;

            pts = new Vector3[4];
            lts = new float[4];
            Color[] clrs = new Color[4];


            for (int il = 0; il < 4; il++)
            {  // grey for now
                clrs[il] = new Color(.5f, .5f, .5f);
            }

            int ind = 0;
            float uvTimes = 0.1f;
            for (int i = 0; i < nlat; i++)
            {
                for (int j = 0; j < nlong; j++)
                {

                    pts[0].X = (float)(Math.Cos(MathHelper.TwoPi * ((float)i / (float)nlong)) * Math.Sin(MathHelper.Pi * ((float)j / (float)nlat)));
                    pts[0].Y = (float)(Math.Cos(MathHelper.Pi * ((float)j / (float)nlat)));
                    pts[0].Z = (float)(Math.Sin(MathHelper.TwoPi * ((float)i / (float)nlong)) * Math.Sin(MathHelper.Pi * ((float)j / (float)nlat)));

                    pts[1].X = (float)(Math.Cos(MathHelper.TwoPi * ((float)(i + 1) / (float)nlong)) * Math.Sin(MathHelper.Pi * ((float)j / (float)nlat)));
                    pts[1].Y = (float)(Math.Cos(MathHelper.Pi * ((float)j / (float)nlat)));
                    pts[1].Z = (float)(Math.Sin(MathHelper.TwoPi * ((float)(i + 1) / (float)nlong)) * Math.Sin(MathHelper.Pi * ((float)j / (float)nlat)));

                    pts[2].X = (float)(Math.Cos(MathHelper.TwoPi * ((float)i / (float)nlong)) * Math.Sin(MathHelper.Pi * ((float)(j + 1) / (float)nlat)));
                    pts[2].Y = (float)(Math.Cos(MathHelper.Pi * ((float)(j + 1) / (float)nlat)));
                    pts[2].Z = (float)(Math.Sin(MathHelper.TwoPi * ((float)i / (float)nlong)) * Math.Sin(MathHelper.Pi * ((float)(j + 1) / (float)nlat)));

                    pts[3].X = (float)(Math.Cos(MathHelper.TwoPi * ((float)(i + 1) / (float)nlong)) * Math.Sin(MathHelper.Pi * ((float)(j + 1) / (float)nlat)));
                    pts[3].Y = (float)(Math.Cos(MathHelper.Pi * ((float)(j + 1) / (float)nlat)));
                    pts[3].Z = (float)(Math.Sin(MathHelper.TwoPi * ((float)(i + 1) / (float)nlong)) * Math.Sin(MathHelper.Pi * ((float)(j + 1) / (float)nlat)));

                    for (int k = 0; k < 4; k++)
                    {
                        pts[k].Z *= 0.5f;
                        pts[k].X *= 0.5f;
                        pts[k].Y *= 0.5f;

                    }

                    pnverts[ind].TextureCoordinate = new Vector2(i * uvTimes, j * uvTimes);
                    pnverts[ind].Position = new Vector3(pts[0].X, pts[0].Y, pts[0].Z);
                    pnverts[ind++].Normal = new Vector3(pts[0].X, pts[0].Y, pts[0].Z);

                    pnverts[ind].TextureCoordinate = new Vector2((i + 1) * uvTimes, (j) * uvTimes);
                    pnverts[ind].Position = new Vector3(pts[1].X, pts[1].Y, pts[1].Z);
                    pnverts[ind++].Normal = new Vector3(pts[1].X, pts[1].Y, pts[1].Z);

                    pnverts[ind].TextureCoordinate = new Vector2((i) * uvTimes, (j + 1) * uvTimes);
                    pnverts[ind].Position = new Vector3(pts[2].X, pts[2].Y, pts[2].Z);
                    pnverts[ind++].Normal = new Vector3(pts[2].X, pts[2].Y, pts[2].Z);

                    pnverts[ind].TextureCoordinate = new Vector2((i) * uvTimes, (j + 1) * uvTimes);
                    pnverts[ind].Position = new Vector3(pts[2].X, pts[2].Y, pts[2].Z);
                    pnverts[ind++].Normal = new Vector3(pts[2].X, pts[2].Y, pts[2].Z);

                    pnverts[ind].TextureCoordinate = new Vector2((i + 1) * uvTimes, (j) * uvTimes);
                    pnverts[ind].Position = new Vector3(pts[1].X, pts[1].Y, pts[1].Z);
                    pnverts[ind++].Normal = new Vector3(pts[1].X, pts[1].Y, pts[1].Z);

                    pnverts[ind].TextureCoordinate = new Vector2((i + 1) * uvTimes, (j + 1) * uvTimes);
                    pnverts[ind].Position = new Vector3(pts[3].X, pts[3].Y, pts[3].Z);
                    pnverts[ind++].Normal = new Vector3(pts[3].X, pts[3].Y, pts[3].Z);

                    /*
                   verts[iv++] = new VertexPositionColor(pts[0], clrs[0]);
                   verts[iv++] = new VertexPositionColor(pts[1], clrs[1]);
                   verts[iv++] = new VertexPositionColor(pts[2], clrs[2]);
                   verts[iv++] = new VertexPositionColor(pts[2], clrs[2]);
                   verts[iv++] = new VertexPositionColor(pts[1], clrs[1]);
                   verts[iv++] = new VertexPositionColor(pts[3], clrs[3]);
                   */


                }
            }
        }


        public void SetLightDir(Vector3 ldir)
        {  // calculate lightmaps - call this only once, not every frame

            Color ncol = new Color(0, 0, 0, 255);
            float lt;

            for (int il = 0; il < nlat * nlong * 6; il++)
            {  // asssumes radius is one
                lt = Vector3.Dot(ldir, verts[il].Position);
                if (lt < 0.0f) lt = 0.0f;
                ncol.R = ncol.G = ncol.B = (byte)(lt * 255);

                verts[il].Color = ncol;
            }

        }


        public override void Initialize()
        {
            //base.Initialize();
            // TODO: Add your initialization code here
            ceffect = new BasicEffect(GraphicsDevice);
            ceffect.VertexColorEnabled = true;
            ceffect.World = worldRot * worldTrans;
            //position += bulletVelocity;

            texture = myGame.Content.Load<Texture2D>(@"Textures\sphere");
            DiffuseTexture = myGame.Content.Load<Effect>("Effects/DiffuseTexture");
            base.Initialize();
        }
        public void SetTranslation(Vector3 trns)
        {  // set sphere location
            position = trns;
            worldTrans = Matrix.CreateTranslation(position);
            ceffect.World = worldRot * worldTrans;

        }

        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here
            speed.Y += gravity;
            position += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            position += bulletVelocity * 0.4f;
            speed = bounceBall(speed, position);

            worldTrans = Matrix.CreateTranslation(position);
            ceffect.World = worldRot * worldTrans;


            base.Update(gameTime);
        }



        public override void Draw(GameTime gameTime)
        {
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            myGame.setCamera(ceffect);


            /*
             foreach (EffectPass pass in ceffect.CurrentTechnique.Passes)
             {
                 pass.Apply();
                  {
                     GraphicsDevice.DrawUserPrimitives<VertexPositionColor>
                         (PrimitiveType.TriangleList, verts, 0, nlat * nlong * 2);
                 }
             }
             * */


            DiffuseTexture.Parameters["World"].SetValue(worldTrans);
            DiffuseTexture.Parameters["View"].SetValue(myGame.view);
            DiffuseTexture.Parameters["Projection"].SetValue(myGame.camera.projection);
            DiffuseTexture.Parameters["LightDir"].SetValue(myGame.source); // need this later
            DiffuseTexture.Parameters["ModelTexture"].SetValue(texture);
            DiffuseTexture.Parameters["Ambient"].SetValue(myGame.Ambient);
            DiffuseTexture.Parameters["CameraPos"].SetValue(myGame.CameraPosition);
            DiffuseTexture.Parameters["LightColor"].SetValue(myGame.lightColor);

            foreach (EffectPass pass in DiffuseTexture.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionNormalTexture>
                                                        (PrimitiveType.TriangleList, pnverts, 0, nlat * nlong * 2);
            }

            base.Draw(gameTime);
        }

        public float calcSphereLerp(Vector3 position)
        {
            float XPosition = position.X;
            float Zposition = position.Z;
            float Yposition = myGame.calcXLerp(XPosition, Zposition);
            return Yposition;
        }
        public Vector3 bounceBall(Vector3 speed, Vector3 position)
        {

            Vector3 reflection;
            float Yposition;
            if ((position.X > 0) && (position.X < myGame.gDim - 1) && (position.Z > 0) && (position.Z < myGame.gDim))
            {
                Yposition = calcSphereLerp(position);
                normal = myGame.getNormals(position.X, position.Z);
                if (Yposition >= position.Y)
                {
                    Vector3 sdr = -speed;
                    sdr.Normalize();
                    reflection = 2f * Vector3.Dot(normal, sdr) * normal - sdr;
                    float smag = speed.Length();
                    speed = reflection * smag;
                    return speed;
                }
                else return speed;
            }
            else
            {
                return speed;
            }
        }
        public void setGraphicsVar(Camera camera, Matrix world, Matrix view)
        {
            this.camera = camera;
            this.world = world;
            this.view = view;
        }
        public void setVelocity(Vector3 incBulletVelocity)
        {
            this.bulletVelocity = incBulletVelocity;
            bulletVelocity.Normalize();
            //speed *= bulletVelocity;
        }
        public Vector3 getPosition()
        {
            return position;
        }

    }
}
