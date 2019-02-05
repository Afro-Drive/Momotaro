using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Device;

namespace Momotaro.Actor
{
    class Block : GameObject
    {
        //フィールド
        private int chipNum; //【追加】マップチップの指定番号

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="gameDevice">ゲームデバイス</param>
        public Block(Vector2 position, GameDevice gameDevice, int chipNum)
            : base("blk_chip", position, 64, 64, gameDevice)
        {
            this.chipNum = chipNum;
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="other"></param>
        public Block(Block other)
            : this(other.position, other.gameDevice, other.chipNum)
        {

        }

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new Block(this);
        }

        /// <summary>
        /// ヒット通知
        /// </summary>
        /// <param name="gameObject">相手のオブジェクト</param>
        public override void Hit(GameObject gameObject)
        {

        }

        /// <summary>
        /// ヒット通知
        /// </summary>
        /// <param name="chara">キャラクター</param>
        public override void HitChara(Character chara)
        {
            
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="gameTime">ゲーム時間</param>
        public override void Update(GameTime gameTime)
        {

        }

        public override void Change()
        {
            
        }

        //描画処理
        public override void Draw(Renderer renderer)
        {
            renderer.DrawTexture(
                name,
                position + gameDevice.GetDisplayModify(),
                //マップチップの画像の切り取り位置を指定
                new Rectangle(64 * ((chipNum - 1) % 3), 64 * ((chipNum - 1) / 3), 64, 64),
                Color.White);
        }
    }
}
