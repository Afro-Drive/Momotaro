using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Momotaro.Util
{
    /// <summary>
    /// 範囲クラス
    /// </summary>
    class Range
    {
        private int first;//範囲の最初の番号
        private int end;//終端番号

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="first">範囲の最初</param>
        /// <param name="end">終端番号</param>
        public Range(int first, int end)
        {
            this.first = first;
            this.end = end;
        }

        /// <summary>
        /// 範囲の最初の番号を取得
        /// </summary>
        /// <returns>範囲の最初</returns>
        public int First()
        {
            return first;
        }

        /// <summary>
        /// 範囲の終端番号を取得
        /// </summary>
        /// <returns>終端番号</returns>
        public int End()
        {
            return end;
        }

        /// <summary>
        /// 範囲内にはいいているか？
        /// </summary>
        /// <param name="num">調べたい数</param>
        /// <returns></returns>
        public bool IsWithin(int num)
        {
            //最初の番号より小さい
            if (num < first)
            {
                return false;
            }
            //終端番号より大きい
            if (num > end)
            {
                return false;
            }
            //範囲内
            return true;
        }

        /// <summary>
        /// （設定した開始・終端が）範囲外か？
        /// </summary>
        /// <returns>範囲外ならtrue</returns>
        public bool IsOutOfRange()
        {
            return first >= end;
        }

        /// <summary>
        /// 指定番号が範囲外か？
        /// </summary>
        /// <param name="num">調べたい番号</param>
        /// <returns>範囲外ならtrue</returns>
        public bool IsOutOfRange(int num)
        {
            //範囲内でなければtrue
            return !IsWithin(num);
        }
    }
}
