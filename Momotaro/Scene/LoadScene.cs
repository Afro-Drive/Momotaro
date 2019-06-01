using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Momotaro.Device;
using Momotaro.Util;
using Momotaro.Def;

namespace Momotaro.Scene
{
    /// <summary>
    /// 画像データ、音データ読み込み用シーン
    /// </summary>
    class LoadScene : IScene
    {
        private Renderer renderer;//描画オブジェクト

        //読み込み用オブジェクト
        private TextureLoader textureLoader;
        private BGMLoader bgmLoader;
        private SELoader seLoader;

        private int totalResouceNum;//全リソース数
        private bool isEndFlag;//終了フラグ
        private Timer timer;//演出用タイマー


        #region テクスチャ用
        /// <summary>
        /// テクスチャ読み込み用２次元配列の取得
        /// </summary>
        /// <returns></returns>
        private string[,] textureMatrix()
        {
            //テクスチャディレクトリのデフォルトパス
            string path = "./Texture/";

            //読み込み対象データ
            string[,] data = new string[,]
            {

                //シーンごとの背景
                {"clear", path },
                {"gameover", path },
                {"title", path },
                {"logo", path },
                {"trueEnding",path },
                {"stagebg",path },
                {"bossbg",path },
                {"pausebg", path },

                { "door", path},
                {"cursor",path },
                {"stage", path },
                {"flame", path },
                {"thorn", path },
                {"momo", path },
                {"tubo", path },
                {"dango", path },
                {"goalpoint", path },
                {"fire",path },
                {"shootObj_dot",path },
                {"blk_chip", path },
                {"smoke_effect", path },
                {"hp",path },
                {"hp_null",path },
                {"switch_CanPut",path },
                {"switch_TimeDown",path },

                {"boss_particle", path },


                //↓アニメーション用画像
                //桃太郎
                {"momotaro_attackR", path },
                {"momotaro_attackL", path },
                {"momotaro_jumpR", path },
                {"momotaro_jumpL", path },
                {"momotaro_moveR", path },
                {"momotaro_moveL", path },
                {"momotaro_idlingR", path },
                {"momotaro_idlingL", path },

               //犬
                {"inu_moveR", path },
                {"inu_moveL", path },
                {"inu_idlingL", path },
                {"inu_jumpL", path }, 

                //キジ
                {"kiji_jumpmotion", path }, 
                {"kiji_movemotionL", path },

                //猿
                {"saru_idlingL", path },
                {"saru_moveL", path },
                {"saru_jumpL", path },
                {"saru_climbingL", path },
                {"saru_idlingclimbingL", path },

                //小鬼
                {"Oni_Attack", path }, 
                {"kooni_moveL", path },
                {"kooni_damage", path },

                //フクロウ
                {"owl_motion",path },

                //ラスボス「鬼」
                {"oni_damage_face", path },
                {"oni_idling_face", path },
                {"oni_attack_face", path },
                //↑アニメーション画像
                
                // 不要になったリソース
                //{ "black", path},
                //{ "block", path},
                //{ "block2", path},
                //{ "button1", path},
                //{ "button2", path},
                //{ "door1", path},
                //{ "door2", path},
                //{ "player", path},
                //{ "puddle", path},

                //簡易画像
                //{ "inu", path},
                //{ "momotaro", path},
                //{ "saru", path},
                //{ "tori", path},
                //{"hito",path },
                //{"teki", path },
                //{"goal", path },
                //{"kabe", path },
                //{"switch",path },
                //{"momotaro_R",path },
                //{"momotaro_L",path },
                //{"inu_idlingR", path },
                //{"kiji_movemotionR", path },
                //{"saru_moveR", path },
                //{"saru_idlingR", path },
                //{"saru_jumpR", path },
                //{"kiji_movemotion",path },
                //{"kiji_jumpmotion",path },
                //{"saru_climbingR", path },
                //{"kooni_moveR", path },
                //{ "number", path } 

                //必要に応じて自分で追加
            };

            return data;
        }
        #endregion テクスチャ用

        #region BGM用
        /// <summary>
        /// BGM読み込み用２次元配列の取得
        /// </summary>
        /// <returns></returns>
        private string[,] BGMMatrix()
        {
            //テクスチャディレクトリのデフォルトパス
            string path = "./Sound/";

            //BGM(MP3)読み込み対象データ
            string[,] data = new string[,]
            {
                //{ "titlebgm", path},
                {"bgm_boss",path },
                {"bgm_clear",path },
                {"bgm_over",path },
                {"bgm_stage",path },
                {"bgm_title",path },
                {"bgm_stage4",path },

                //必要に応じて自分で追加
            };

            return data;
        }
        #endregion BGM用

        #region SE用
        /// <summary>
        /// SE読み込み用２次元配列の取得
        /// </summary>
        /// <returns></returns>
        private string[,] SEMatrix()
        {
            //テクスチャディレクトリのデフォルトパス
            string path = "./Sound/";

            //BGM(MP3)読み込み対象データ
            string[,] data = new string[,]
            {
                //{ "titlese", path },
                {"boss_atk",path },
                {"boss_rush",path },
                {"button",path },
                {"dango",path },
                {"dmg",path },
                {"e_atk",path },
                {"e_move",path },
                {"momo",path },
                {"momo_atk",path },
                {"over",path },
                {"p_change",path },
                {"p_jump",path },
                {"p_move",path },

                //必要に応じて自分で追加
            };

            return data;
        }
        #endregion SE用

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public LoadScene()
        {
            //描画オブジェクトを取得
            renderer = GameDevice.Instance().GetRenderer();

            //読み込む対象を取得し、実体生成
            textureLoader = new TextureLoader(textureMatrix());
            bgmLoader = new BGMLoader(BGMMatrix());
            seLoader = new SELoader(SEMatrix());
            isEndFlag = false;

            timer = new CountDownTimer(0.1f);
        }

        /// <summary>
        /// 描画
        /// </summary>
        /// <param name="renderer"></param>
        public void Draw(Renderer renderer)
        {
            //描画開始
            renderer.Begin();

            renderer.DrawTexture("load", new Vector2(20, 20));

            //現在読み込んでいる数を取得
            int currentCount =
                textureLoader.CurrentCount() +
                bgmLoader.CurrentCount() +
                seLoader.CurrentCount();

            //読み込むモノがあれば描画
            if (totalResouceNum != 0)
            {
                //読み込んだ割合
                float rate = (float)currentCount / totalResouceNum;
                //数字で描画
                renderer.DrawNumber(
                    "number",
                    new Vector2(20, 100),
                    (int)(rate * 100.0f));

                //バーで描画
                renderer.DrawTexture(
                    "fade",
                    new Vector2(0, 500),
                    null,
                    0.0f,
                    Vector2.Zero,
                    new Vector2(rate * Screen.WIDTH, 20));
            }

            //終了
            //すべてのデータを読み込んだか？
            if (textureLoader.IsEnd() && bgmLoader.IsEnd() && seLoader.IsEnd())
            {
                isEndFlag = true;
            }

            //描画終了
            renderer.End();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize(Scene lastSceneName)
        {
            //終了フラグを継続に設定
            isEndFlag = false;
            //テクスチャ読み込みオブジェクトを初期化
            textureLoader.Initialize();
            //BGM読み込みオブジェクトを初期化
            bgmLoader.Initialize();
            //SE読み込みオブジェクトを初期化
            seLoader.Initialize();
            //全リソース数を計算
            totalResouceNum =
                textureLoader.RegistMAXNum() +
                bgmLoader.RegistMAXNum() +
                seLoader.RegistMAXNum();
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
        /// 次のシーン
        /// </summary>
        /// <returns></returns>
        public Scene Next()
        {
            return Scene.Logo;
        }

        /// <summary>
        /// 終了
        /// </summary>
        public void Shutdown()
        {

        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            //演出確認用（大量のデータがあるときは設定時間を０に）
            //一定時間ごとに読み込み
            timer.Update(gameTime);
            if (timer.IsTime() == false)
            {
                return;
            }
            timer.Initialize();

            //テクスチャから順々に読み込みを行う
            if (textureLoader.IsEnd() == false)
            {
                textureLoader.Update(gameTime);
            }
            else if (bgmLoader.IsEnd() == false)
            {
                bgmLoader.Update(gameTime);
            }
            else if (seLoader.IsEnd() == false)
            {
                seLoader.Update(gameTime);
            }
        }
    }
}
