using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Momotaro.Device
{
    /// <summary>
    /// リソース読み込み抽象クラス
    /// </summary>
    abstract class Loader
    {
        protected string[,] resources;//リソースアセット名群
        protected int counter;//現在登録しているカウンタ
        protected int maxNum;//最大登録数
        protected bool isEndFlag;//終了フラグ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="resources"></param>
        public Loader(string[,] resources)
        {
            this.resources = resources;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            counter = 0;
            isEndFlag = false;
            maxNum = 0;

            //条件がFalseの時に、エラー文を出す
            Debug.Assert(resources != null,
                "リソースデータ登録情報がおかしいです");
            //配列から、配列から
            maxNum = resources.GetLength(0);
        }

        /// <summary>
        /// 最大登録数
        /// </summary>
        /// <returns></returns>
        public int RegistMAXNum()
        {
            return maxNum;
        }

        /// <summary>
        /// 現在の登録している番号を取得
        /// </summary>
        /// <returns></returns>
        public int CurrentCount()
        {
            return counter;
        }

        /// <summary>
        /// 終了か？
        /// </summary>
        /// <returns></returns>
        public bool IsEnd()
        {
            return isEndFlag;
        }

        /// <summary>
        /// 抽象更新メソッド
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void Update(GameTime gameTime);
    }
}
