﻿#region description
//-----------------------------------------------------------------------------
// FallPiece.cs
//
// Written by bwrsandman(Sandy)
//-----------------------------------------------------------------------------
#endregion


#region using
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Cubic_The_Game
{
    class FallPiece : Piece
    {
        #region constants
        private const float offset = 50f;
        #endregion

        #region statics
        private readonly static Color inactiveColor = Color.White;
        #endregion

        #region members
        private int interactingPlayer = -1;
        private Color color;
        private Color interactedColor;
        #endregion

        public FallPiece(Vector3 origin)
        {
            center3 = origin;

            cubeFront = new VertexPositionColor[4];

            cubeBuffer = new VertexBuffer(GameObject.device, typeof(VertexPositionColor), cubeFront.Length, BufferUsage.None);
            cubeEffect = new BasicEffect(GameObject.device);
        }

        #region members
        public Matrix worldTranslation = Matrix.CreateTranslation(0, 0, 2);
        public Vector3 center3;
        public Vector2 center2 { get { return GameObject.GetScreenSpace(center3, worldTranslation); } }
        VertexPositionColor[] cubeFront;
        VertexBuffer cubeBuffer;
        BasicEffect cubeEffect;
        private bool isIntersected;
        #endregion

        #region update and draw
        public bool intersects(Vector2 cntr)
        {
            return ((center2 - cntr).Length() <= offset);
        }

        public bool intersects(Player[] players)
        {
            if (isIntersected && interactingPlayer >= 0 && intersects(players[interactingPlayer].center));
            else {
                isIntersected = false;
                interactingPlayer = -1;
                for (byte i = 0; i<players.Length; ++i) if (players[i]!=null)
                    if (intersects(players[i].center))
                    {
                        isIntersected = true;
                        interactingPlayer = i;
                        interactedColor = new Color(players[i].color.R/4+128, players[i].color.G/4+128, players[i].color.B/4+128);
                        break;
                    }
            }
            return isIntersected;
        }

        public void Update()
        {
            color = isIntersected? interactedColor : inactiveColor ;
            cubeFront[0] = new VertexPositionColor(new Vector3(-2, 2, 0) + center3, color);
            cubeFront[1] = new VertexPositionColor(new Vector3(2, 2, 0) + center3, color);
            cubeFront[2] = new VertexPositionColor(new Vector3(-2, -2, 0) + center3, color);
            cubeFront[3] = new VertexPositionColor(new Vector3(2, -2, 0) + center3, color);
        }

        public void Draw(Camera camera)
        {
            GameObject.device.SetVertexBuffer(cubeBuffer);

            cubeEffect.World = worldTranslation;
            cubeEffect.View = camera.view;
            cubeEffect.Projection = camera.projection;
            cubeEffect.DiffuseColor = color.ToVector3();


            foreach (EffectPass pass in cubeEffect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GameObject.device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, cubeFront, 0, 2);

            }


        }

        #endregion

    }
}
