using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Actor.Characters;
using Momotaro.Device;
using Momotaro.Util;

namespace Momotaro.Actor.GameObjects
{

    /// <summary>
    /// 当たった時の方向
    /// </summary>
    enum Direction
    {
        Top,   //上
        Bottom,//下
        Left,  //左
        Right  //右
    }

    /// <summary>
    /// 抽象ゲームオブジェクトクラス
    /// </summary>
    abstract class GameObject : ICloneable //コピー機能を追加
    {
        protected string name; //アセット名
        protected Vector2 position; //位置
        protected int width; //幅
        protected int height; //高さ
        protected bool isDeadFlag = false; //死亡フラグ
        protected GameDevice gameDevice; //ゲームデバイス
        protected bool isChangeFlag = false;
        protected bool isClearFlag = false;
        protected GameObjectID id = GameObjectID.NONE;

        protected Dictionary<string, Motion> motionDict; //【追加】モーション管理用ディクショナリ
        protected Motion currentMotion; //【追加】現在使用中のモーション

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">アセット名</param>
        /// <param name="position">位置</param>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        /// <param name="gameDevice">ゲームデバイス</param>
        public GameObject(string name, Vector2 position,
            int width, int height, GameDevice gameDevice)
        {
            this.name = name;
            this.position = position;
            this.width = width;
            this.height = height;
            this.gameDevice = gameDevice;

            motionDict = null; //【追加】最初は空っぽで初期化
            currentMotion = null; //【追加】最初は空っぽで初期化
        }

        /// <summary>
        /// 位置の設定
        /// </summary>
        /// <param name="position">設定した位置</param>
        public void SetPosition(Vector2 position)
        {
            this.position = position;
        }

        /// <summary>
        /// 位置の取得
        /// </summary>
        /// <returns>ゲームオブジェクトの位置</returns>
        public Vector2 GetPosition()
        {
            return position;
        }

        /// <summary>
        /// オブジェクト幅の取得
        /// </summary>
        /// <returns></returns>
        public int GetWidth()
        {
            return width;
        }

        /// <summary>
        /// オブジェクトの高さの取得
        /// </summary>
        /// <returns></returns>
        public int GetHeight()
        {
            return height;
        }

        //抽象メソッド
        public abstract object Clone(); //ICloneableで必ず必要
        public abstract void Update(GameTime gameTime); //更新
        public abstract void Hit(GameObject gameObject); //ヒット通知  
        public abstract void HitChara(Character chara); //キャラクターとのヒット通知
        public abstract void Change(); //切り替え

        //仮想メソッド
        /// <summary>
        /// 描画
        /// </summary>
        /// <param name="renderer">描画オブジェクト</param>
        public virtual void Draw(Renderer renderer)
        {
            renderer.DrawTexture(name, position + gameDevice.GetDisplayModify());
        }

        /// <summary>
        /// 死んでいるか？
        /// </summary>
        /// <returns>死んでいたらtrue</returns>
        public bool IsDead()
        {
            return isDeadFlag;
        }

        /// <summary>
        /// 当たり判定用、矩形情報の取得
        /// </summary>
        /// <returns></returns>
        public Rectangle GetRectangle()
        {
            //矩形の生成
            Rectangle area = new Rectangle();

            //位置と幅、高さを設定
            area.X = (int)position.X;
            area.Y = (int)position.Y;
            area.Height = height;
            area.Width = width;

            return area;
        }

        /// <summary>
        /// 矩形同士の当たり判定
        /// </summary>
        /// <param name="otherObj">相手の矩形</param>
        /// <returns>当たってたら</returns>
        public bool IsCollision(GameObject otherObj)
        {
            //RectangleクラスのIntersectsメソッドで重なり判定
            return this.GetRectangle().Intersects(otherObj.GetRectangle());
        }


        public Direction CheckDirection(GameObject otherObj)
        {
            //中心位置の取得
            Point thisCenter = this.GetRectangle().Center; //自分の中心位置
            Point otherCenter = otherObj.GetRectangle().Center; //相手の中心位置

            //向きのベクトルを取得
            Vector2 dir =
                new Vector2(thisCenter.X, thisCenter.Y) -
                new Vector2(otherCenter.X, otherCenter.Y);

            //当たっている側面をリターンする
            //x成分とy成分でどちらの方が量が多いか
            if (Math.Abs(dir.X) > Math.Abs(dir.Y))
            {
                //xの向きが正のとき
                if (dir.X > 0)
                {
                    return Direction.Right;
                }
                return Direction.Left;
            }

            //y成分が大きく、正の値か？
            if (dir.Y > 0)
            {
                return Direction.Bottom;
            }

            //プレイヤーがブロックに乗った
            return Direction.Top;
        }


        public bool IsChange()
        {
            return isChangeFlag;
        }

        public void ChangeFlagTurn()
        {
            if (isChangeFlag)
            {
                isChangeFlag = !isChangeFlag;
            }
            else
            {
                return;
            }
        }

        public bool IsClear()
        {
            return isClearFlag;
        }

        public GameObjectID GetID()
        {
            return id;
        }

        public void SetID(GameObjectID id)
        {
            this.id = id;
        }
    }

}
