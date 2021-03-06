﻿using Microsoft.Xna.Framework;
using Momotaro.Scene;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Momotaro.Device;
using Momotaro.Util;
using Momotaro.Actor.Characters;

namespace Momotaro.Actor.AI
{
    abstract class EnemyAI
    {
        protected IGameObjectMediator mediator;

        protected Vector2 position;
        protected bool isJump;

        //継承先で決める変数達
        protected float gravity;
        protected Vector2 velocity;


        protected Map map;
        protected List<Vector2> checkRList;
        protected List<Vector2> checkLList;
        protected List<Vector2> checkUList;
        protected List<Vector2> checkDList;

        protected Random rnd;


        public EnemyAI(Character character, IGameObjectMediator mediator)
        {
            position = character.Position;
            isJump = true;

            this.mediator = mediator;
            map = mediator.GetMap();
            rnd = GameDevice.Instance().GetRandom();

            //マップ当たり判定用リストの作成
            int width = character.Width;
            int height = character.Height;
            int widthMargin = character.WidthMargin;
            int heightMargin = character.HeightMargin;
            checkRList = new List<Vector2>()
            {
                new Vector2(width - widthMargin, heightMargin),
                new Vector2(width - widthMargin, height - heightMargin - 1)
            };
            checkLList = new List<Vector2>()
            {
                new Vector2(widthMargin - 1, heightMargin),
                new Vector2(widthMargin - 1, height - heightMargin - 1)
            };
            checkUList = new List<Vector2>()
            {
                new Vector2(widthMargin, heightMargin - 1),
                new Vector2(width - widthMargin - 1, heightMargin - 1)
            };
            checkDList = new List<Vector2>()
            {
                new Vector2(widthMargin, height - heightMargin),
                new Vector2(width - widthMargin - 1, height - heightMargin)
            };

        }

        /// <summary>
        /// 【追加】ジャンプ中か？
        /// </summary>
        /// <returns></returns>
        public bool IsJump()
        {
            return isJump;
        }

        /// <summary>
        /// 【追加】移動量の取得
        /// </summary>
        /// <returns></returns>
        public Vector2 Velocity()
        {
            return velocity;
        }

        public abstract void Attack();
        public abstract Vector2 Move();
    }
}
