using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Device;

namespace Momotaro.Actor
{
    class Space : GameObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="position">位置</param>
        /// <param name="gameDevice">ゲームデバイス</param>
        public Space(Vector2 position, GameDevice gameDevice)
            : base("", position, 64, 64, gameDevice)
        {

        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="other">コピー元のオブジェクト</param>
        public Space(Space other)
            : this(other.position, other.gameDevice) //自分のコンストラクタ呼び出し
        {

        }

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new Space(this);
        }

        /// <summary>
        /// ヒット通知
        /// </summary>
        /// <param name="gameObject">相手のオブジェクト</param>
        public override void Hit(GameObject gameObject)
        {

        }

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


        public override void Draw(Renderer renderer)
        {
            
        }

        public override void Change()
        {
            
        }
    }
}
