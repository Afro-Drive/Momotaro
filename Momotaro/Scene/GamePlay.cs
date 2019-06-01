using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Momotaro.Device;
using Momotaro.Actor;
using Momotaro.Util;
using Momotaro.Actor.Characters.Player;
using Momotaro.Actor.Characters.BossObj;

namespace Momotaro.Scene
{
    /// <summary>
    /// ゲームプレイクラス
    /// </summary>
    class GamePlay : IScene
    {
        private bool isEndFlag;//終了フラグ

        private Map map; //マップ
        private Sound sound;

        private GameObjectManager gameObjectManager; //ゲームオブジェクトマネージャ
        private PlayerManager playerManager; //切り替え用のプレイヤーマネージャー
        private Boss boss;

        private Timer countUpTimer; //カウントアップタイマー
        private Score score; //スコア管理

        private Scene nextScene;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GamePlay(Score score, Timer timer)
        {
            //シーン終了フラグ
            isEndFlag = false;

            this.score = score;
            countUpTimer = timer;
            sound = GameDevice.Instance().GetSound();

            //オブジェクトマネージャーの実体生成
            gameObjectManager = new GameObjectManager(score);
        }



        /// <summary>
        /// 描画
        /// </summary>
        /// <param name="renderer">描画オブジェクト</param>
        public void Draw(Renderer renderer)
        {
            renderer.Begin();

            DrawBackGround(renderer);

            //マップの描画
            map.Draw(renderer);

            //オブジェクトマネージャーの描画
            gameObjectManager.Draw(renderer);

            //タイマーの描画
            //renderer.DrawNumber("number", new Vector2(580, 13), countUpTimer.Now());

            renderer.End();
        }



        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize(Scene lastSceneName)
        {
            //シーン終了フラグを初期化
            isEndFlag = false;

            //前のシーンがポーズだったら初期化しない
            if (lastSceneName == Scene.Pause)
                return;

            //Mapの実体生成
            map = new Map(GameDevice.Instance(), gameObjectManager);

            //キャラの制限をステージ番号で設定
            int playerLeng = GameData.stageNum - 1;
            GameData.pCount = MathHelper.Clamp(playerLeng, 0, 3);

            //オブジェクトマネージャーの初期化
            gameObjectManager.Initialize();
            //マネージャに追加（当たり判定等のため）
            gameObjectManager.Add(map);

            //マップのロード
            map.Load("stage" + GameData.stageNum.ToString().PadLeft(2, '0') + ".csv", "./csv/");

            //前ステージから続くスコア加算を止める
            score.Shutdown();

            //プレイヤーマネージャーの実体生成
            playerManager = new PlayerManager(gameObjectManager);
            playerManager.Initialize();

            PutPlayerOn();

            //ボスの設定 
            //ステージ05になったらボスを追加する
            if (GameData.stageNum == 5)
            {
                boss = new Boss(new Vector2(40, 23) * 64, GameDevice.Instance(), gameObjectManager);
                gameObjectManager.Add(boss);
            }
        }

        /// <summary>
        /// シーン終了か？
        /// </summary>
        /// <returns></returns>
        public bool IsEnd()
        {
            return isEndFlag;
        }

        /// <summary>
        /// 次のシーンは
        /// </summary>
        /// <returns>次のシーン名</returns>
        public Scene Next()
        {
            return nextScene;
        }

        /// <summary>
        /// 終了処理
        /// </summary>
        public void Shutdown()
        {
        }

        /// <summary>
        /// 更新処理
        /// </summary>
        /// <param name="gameTime">ゲーム時間</param>
        public void Update(GameTime gameTime)
        {
            map.Update(gameTime);
            gameObjectManager.Update(gameTime);
            countUpTimer.Update(gameTime);

            PlayBGM();

            playerManager.AcceptInput();
            playerManager.Update(gameTime);

            IsClear();
            IsBossDead();
            IsPlayerDead();

            GoPause();
        }

        private void PutPlayerOn()
        {
            //最初のキャラを設定（引数はポジション）
            if (GameData.stageNum == 3)
            {
                playerManager.SetStartPlayer(new Vector2(4, 67) * 64);
            }
            else if (GameData.stageNum == 4)
            {
                playerManager.SetStartPlayer(new Vector2(3, 10) * 64);
            }
            else if (GameData.stageNum == 5)
            {
                playerManager.SetStartPlayer(new Vector2(30, 20) * 64);
            }
            else
            {
                playerManager.SetStartPlayer(new Vector2(3, 12) * 64);
            }
        }

        private void DrawBackGround(Renderer renderer)
        {
            if (GameData.stageNum >= 4)　//【変更】ステージ4も背景をまがまがしく
            {
                renderer.DrawTexture("bossbg", Vector2.Zero);
            }
            else
            {
                renderer.DrawTexture("stagebg", Vector2.Zero);
            }
        }

        private void PlayBGM()
        {
            if (GameData.stageNum == 5)
            {
                sound.PlayBGM("bgm_boss");
            }
            else if (GameData.stageNum == 4)
            {
                sound.PlayBGM("bgm_stage4");
            }
            else
            {
                sound.PlayBGM("bgm_stage");
            }
        }

        private void GoPause()
        {
            if (Input.GetKeyTrigger(Keys.P) || 
                Input.GetKeyTrigger(PlayerIndex.One, Buttons.Start))
            {
                isEndFlag = true;
                nextScene = Scene.Pause;
            }
        }

        private void IsPlayerDead()
        {
            if (gameObjectManager.IsPlayerDead() || 
                gameObjectManager.GetHp() <= 0)
            {
                sound.StopBGM();
                isEndFlag = true;
                nextScene = Scene.GameOver;
            }
        }

        private void IsBossDead()
        {
            if (boss != null && boss.IsDead())
            {
                nextScene = Scene.TrueEnding;
                sound.StopBGM();
                isEndFlag = true;
            }
        }

        private void IsClear()
        {
            if (playerManager.IsClear())
            {
                sound.StopBGM();
                nextScene = Scene.Ending;

                isEndFlag = true;
            }
        }
    }
}
