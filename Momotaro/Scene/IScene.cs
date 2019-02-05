using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Device;

namespace Momotaro.Scene
{
    /// <summary>
    /// シーンインタフェース
    /// </summary>
    interface IScene
    {
        void Initialize(Scene lastSceneName);//初期化
        void Update(GameTime gameTime);//更新
        void Draw(Renderer renderer);//描画
        void Shutdown();//終了

        //シーン管理用
        bool IsEnd();//終了チェック
        Scene Next();//次のシーンへ
    }
}
