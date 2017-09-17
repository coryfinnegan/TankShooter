using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;


namespace Game1
{
    public class Camera : GameComponent
    {
        public Matrix view { get; protected set; }
        public Matrix projection { get; protected set; }

        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up)
            : base(game)
        {
            view = Matrix.CreateLookAt(pos, target, up);

            projection = Matrix.CreatePerspectiveFieldOfView(
        MathHelper.PiOver4,
        800/ 600, 1, 1000);

        }
        
    }
}
