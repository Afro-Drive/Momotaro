using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Actor.Characters;
using Momotaro.Device;

namespace Momotaro.Actor.GameObjects
{
    class ClearBlock : GameObject
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="position"></param>
        /// <param name="gameDevice"></param>
        public ClearBlock(Vector2 position, GameDevice gameDevice) 
            : base("goalpoint", position, 64, 64, gameDevice)
        {
        }

        /// <summary>
        /// コピーコンストラクタ
        /// </summary>
        /// <param name="other"></param>
        public ClearBlock(ClearBlock other)
            :this(other.Position, other.gameDevice)
        { }

        /// <summary>
        /// 複製
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new ClearBlock(this);
        }

        /// <summary>
        /// オブジェクトとのヒット通知
        /// </summary>
        /// <param name="gameObject"></param>
        public override void Hit(GameObject gameObject)
        {
            
        }

        /// <summary>
        /// プレイヤーとのヒット通知
        /// </summary>
        /// <param name="chara"></param>
        public override void HitChara(Character chara)
        {

        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            
        }

        /// <summary>
        /// 切り替え
        /// </summary>
        public override void Change()
        {
            
        }

        public override void Draw(Renderer renderer)
        {
            renderer.DrawTexture(name, Position + new Vector2(0, -64)+gameDevice.GetDisplayModify());
        }
    }
}
