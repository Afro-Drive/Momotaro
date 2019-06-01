using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Momotaro.Actor.GameObjects;
using Momotaro.Device;
using Momotaro.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Momotaro.Actor.Characters
{
    /// <summary>
    /// 近藤　追加
    /// </summary>
    abstract partial class Character
    {
        protected string name; //アセット名
        protected bool isDeadFlag = false; //死亡フラグ
        protected GameDevice gameDevice; //ゲームデバイス
        protected bool isChangeFlag = false;
        protected bool isClearFlag = false;
        protected bool invincible = false;
        protected Timer invincibleTime;
        protected Color color;
        protected float alpha;

        //座標・当たり判定関連のプロパティ
        public Vector2 Position { get; set; }//座標
        public int Width { get; }//全幅
        public int Height { get; }//全長
        public int HitW { get; }//実際の当たり判定幅
        public int HitH { get; }//実際の当たり判定高さ
        public int WidthMargin { get; }//全幅と判定幅の空白
        public int HeightMargin { get; }//全高さと判定高さの空白

        protected Map map;
        protected List<Vector2> checkRList;
        protected List<Vector2> checkLList;
        protected List<Vector2> checkUList;
        protected List<Vector2> checkDList;

        //【追加】
        //動作別のモーションディクショナリ（切り取り個数や切り替え時間が異なる場合に使用）
        protected Dictionary<MotionName, Motion> motionDict;
        //現在使用中のモーション(更新するモーションの選定用)
        protected Motion currentMotion;
        protected Direction myDirectionX, myDirectionY; //自分の向いてる方向
        protected Vector2 velocity; //移動量(自分の向いている方向の更新に使用)
        protected bool isJump; //ジャンプ中か？


        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="name">アセット名</param>
        /// <param name="position">位置</param>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        /// <param name="gameDevice">ゲームデバイス</param>
        public Character(string name, Vector2 position,
            int width, int height, int hitW, int hitH, GameDevice gameDevice)
        {
            this.name = name;
            this.Position = position;
            this.Width = width;
            this.Height = height;
            this.HitW = hitW;
            this.HitH = hitH;
            color = Color.White;
            alpha = 1f;
            // あたり判定の余白（左右の空き部分）
            WidthMargin = (Width - HitW) / 2;
            HeightMargin = (Height - HitH) / 2;

            // リストの生成
            checkRList = new List<Vector2>()
                { new Vector2(Width - WidthMargin, HeightMargin),
                new Vector2(Width - WidthMargin, Height - HeightMargin - 1) };
            checkLList = new List<Vector2>()
                { new Vector2(WidthMargin - 1, HeightMargin),
                new Vector2(WidthMargin - 1, Height - HeightMargin - 1) };
            checkUList = new List<Vector2>()
                { new Vector2(WidthMargin, HeightMargin - 1),
                new Vector2(Width - WidthMargin - 1, HeightMargin - 1) };
            checkDList = new List<Vector2>()
                { new Vector2(WidthMargin, Height - HeightMargin),
                new Vector2(Width - WidthMargin - 1, Height - HeightMargin) };

            this.gameDevice = gameDevice;

            //【追加】
            motionDict = null;
            myDirectionX = Direction.Right; //右側で初期化→当たり判定リストに変更予定
            myDirectionY = Direction.Top; //上側で初期化
            velocity = Vector2.Zero;
            isJump = true;
        }


        public abstract void Update(GameTime gameTime); //更新
        public abstract void HitChara(Character character); //キャラクターとのヒット通知
        public abstract void HitObj(GameObject obj);//オブジェクトとのヒット通知

        //仮想メソッド
        /// <summary>
        /// 描画
        /// </summary>
        /// <param name="renderer">描画オブジェクト</param>
        public virtual void Draw(Renderer renderer)
        {
            renderer.DrawTexture(name, Position + gameDevice.GetDisplayModify(), color * alpha);
        }

        /// <summary>
        /// 【追加】自分自身の向いている方向の更新
        /// </summary>
        public void UpdateMyDirection()
        {
            //X方向
            if (velocity.X > 0)//右移動時
            {
                myDirectionX = Direction.Right;
            }
            else if (velocity.X < 0)//左移動時
            {
                myDirectionX = Direction.Left;
            }
            else if (velocity.X == 0) return; //移動してない場合は最後の向きのまま

            //Y方向
            if (velocity.Y > 0)//下移動時
            {
                myDirectionY = Direction.Bottom;
            }
            else if (velocity.Y < 0)//上移動時
            {
                myDirectionY = Direction.Top;
            }
            else if (velocity.Y == 0) return; //移動してない場合は最後の向きのまま
        }

        #region 【追加】アニメーション用の描画
        /// <summary>
        /// 【追加】非ジャンプ中の方向に合わせた描画
        /// </summary>
        /// <param name="assetOfDirR">右に向いているときの使用画像</param>
        /// <param name="assetOfDirL">左に向いているときの使用画像</param>
        /// <param name="moveMotion">移動用のモーション</param>
        public void DrawDirMotion(Renderer renderer, string assetOfDirL, Motion moveMotion)
        {
            //左向き
            if (myDirectionX == Direction.Left /*checkPos == checkLList*/)
            {
                renderer.DrawTexture(
                    assetOfDirL,
                    Position + gameDevice.GetDisplayModify(),
                    moveMotion.DrawingRange(),
                    color);
            }
            //右向き
            else if (myDirectionX == Direction.Right)
            {
                //左向きの画像を反転させて描画する
                renderer.DrawAntiAxisTexture(
                    assetOfDirL,
                    Position + gameDevice.GetDisplayModify(),
                    moveMotion.DrawingRange(),
                    new Vector2(0, 0),
                    1,
                    SpriteEffects.FlipHorizontally,
                    color);
            }
        }

        /// <summary>
        /// 【追加】非ジャンプ中の方向に合わせた描画
        /// (左右反転画像を用意する必要があるもの)
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="assetOfDirR"></param>
        /// <param name="assetOfDirL"></param>
        /// <param name="moveMotion"></param>
        public void DrawDirMotion(Renderer renderer, string assetOfDirR, string assetOfDirL, Motion moveMotion)
        {
            string asset = "";　//実際に描画に使うアセット名の変数を用意
            //左向き
            if (myDirectionX == Direction.Left /*checkPos == checkLList*/)
                //左向き用のアセットを代入
                asset = assetOfDirL;
            //右向き
            else if (myDirectionX == Direction.Right)
                //左向きを反転させると描画状態が正しく行かないため、
                //左専用の画像を描画する
                asset = assetOfDirR;

            //向きに応じたアセットを描画する
            renderer.DrawTexture(
                asset,
                Position + gameDevice.GetDisplayModify(),
                moveMotion.DrawingRange(),
                color);

        }

        /// <summary>
        /// 【追加】ジャンプ中の方向に合わせた描画処理
        /// （右左両方の画像を用意する必要がある場合に使用）
        /// </summary>
        /// <param name="assetOfDirR">ジャンプ中かつ右向き時の使用アセット</param>
        /// <param name="assetOfDirL">ジャンプ中かつ左向き時の使用アセット</param>
        /// <param name="jumpMotion">ジャンプ用のモーション</param>
        public void DrawJumping(Renderer renderer, string assetOfDirR, string assetOfDirL, Motion jumpMotion)
        {
            string useAsset = "";
            //右方向へジャンプ
            if (isJump && velocity.X >= 0)
                useAsset = assetOfDirR; //使用アセットは右用のものにする
            //左方向へジャンプ
            else if (isJump && velocity.X <= 0)
                useAsset = assetOfDirL; //使用アセットは左用のモノにする

            //方向に合わせたアセットを描画する
            renderer.DrawTexture(
                useAsset,
                Position + gameDevice.GetDisplayModify(),
                jumpMotion.DrawingRange(), //ディクショナリから該当するモーションを取り出す
                color);
        }

        /// <summary>
        /// 【追加】ジャンプ中の方向に合わせた描画処理
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="assetOfDirL">左向きにジャンプ中に使用するアセット名</param>
        /// <param name="jumpMotion">ジャンプ中のモーション</param>
        public void DrawJumping(Renderer renderer, string assetOfDirL, Motion jumpMotion)
        {
            //左方向へジャンプ
            if (isJump && velocity.X <= 0)
            {
                renderer.DrawTexture(
                    assetOfDirL,
                    Position + gameDevice.GetDisplayModify(),
                    jumpMotion.DrawingRange(),
                    color);
            }
            //右方向へジャンプ
            else if (isJump && velocity.X >= 0)
            {
                //左方向の画像を反転させて描画
                renderer.DrawAntiAxisTexture(
                    assetOfDirL,
                    Position + gameDevice.GetDisplayModify(),
                    jumpMotion.DrawingRange(), //ディクショナリから該当するモーションを取り出す
                    Vector2.Zero,
                    1f,
                    SpriteEffects.FlipHorizontally,
                    color);
            }
        }

        /// <summary>
        /// 【追加】停止中の描画処理
        /// (左右別のアセットを用意する必要がある場合に使用する)
        /// </summary>
        /// <param name="assetOfIdlingR">右方向で停止時に使用する画像</param>
        /// <param name="assetOfIdlingL">左方向で停止時に使用する画像</param>
        /// <param name="idlingMotion">停止用のモーション</param>
        public void DrawIdling(Renderer renderer, string assetOfIdlingR, string assetOfIdlingL, Motion idlingMotion)
        {
            string useAsset = "";
            //右方向を向いている
            if (myDirectionX == Direction.Right)
                useAsset = assetOfIdlingR;
            //左方向を向いている
            else if (myDirectionX == Direction.Left)
                useAsset = assetOfIdlingL;

            //方向に合わせたアセットで描画
            renderer.DrawTexture(
                useAsset,
                Position + gameDevice.GetDisplayModify(),
                idlingMotion.DrawingRange(),
                color);

        }

        /// <summary>
        /// 【追加】停止中の描画処理
        /// </summary>
        /// <param name="renderer"></param>
        /// <param name="assetOfIdlingL">左向き時のアセット</param>
        /// <param name="idlingMotion">アイドリング状態のモーション</param>
        public void DrawIdling(Renderer renderer, string assetOfIdlingL, Motion idlingMotion)
        {
            //左方向を向いている
            if (myDirectionX == Direction.Left)
            {
                renderer.DrawTexture(
            assetOfIdlingL,
            Position + gameDevice.GetDisplayModify(),
            idlingMotion.DrawingRange(),
                    color);

            }
            //右方向を向いている
            else if (myDirectionX == Direction.Right)
            {
                //左方向のアセットを反転させて描画
                renderer.DrawAntiAxisTexture(
                    assetOfIdlingL,
                    Position + gameDevice.GetDisplayModify(),
                    idlingMotion.DrawingRange(),
                    Vector2.Zero,
                    1f,
                    SpriteEffects.FlipHorizontally,
                    color);
            }
        }
        #endregion 【追加】アニメーション用の描画


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
            area.X = (int)Position.X + WidthMargin;
            area.Y = (int)Position.Y + HeightMargin;
            area.Height = HitH;
            area.Width = HitW;

            return area;
        }

        /// <summary>
        /// 矩形同士の当たり判定
        /// </summary>
        /// <param name="otherObj">相手の矩形</param>
        /// <returns>当たってたら</returns>
        public bool IsCollision(Character other)
        {
            //RectangleクラスのIntersectsメソッドで重なり判定
            return GetRectangle().Intersects(other.GetRectangle());
        }


        public Direction CheckDirection(Character other)
        {
            //中心位置の取得
            Point thisCenter = this.GetRectangle().Center; //自分の中心位置
            Point otherCenter = other.GetRectangle().Center; //相手の中心位置

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
    }
}
