namespace Momotaro.Actor.Characters.BossObj
{
    partial class Boss
    {
        /// <summary>
        /// ボスの戦闘態勢の列挙型
        /// </summary>
        private enum BossState
        {
            RunAway,　//プレイヤーターゲットからの逃走
            SprintAttack, 　//気玉の急上昇放出
            JumpAttack, 　//高速突進
            ShotAttack,　//ターゲットに向けて気玉を1発放つ
            CircleAttack,　//気玉の弾幕放出
            Wait, 　//待機
        }
    }
}
