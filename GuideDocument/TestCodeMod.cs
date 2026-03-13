using UnityEngine;

namespace TestCodeMod
{
    public class Main : SimpleModBehaviour
    {
        public override void OnModLoaded()
        {
            Log("Code Mod 已加载，开始监听家园初始化事件。");
            BattleObject.OnAfterHomeDataLoad += OnAfterHomeDataLoad;
        }

        public override void OnModUnloaded()
        {
            BattleObject.OnAfterHomeDataLoad -= OnAfterHomeDataLoad;
            Log("这个 Code Mod 被卸载了。");
        }

        private void OnAfterHomeDataLoad(BattleObject bo)
        {
            bo.maxStickerCarry = 3;
            Log("已把 maxStickerCarry 改成 3。");
        }
    }
}