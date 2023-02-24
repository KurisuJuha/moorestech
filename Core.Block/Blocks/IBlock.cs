﻿using System;

namespace Core.Block.Blocks
{
    public interface IBlock
    {
        public int EntityId { get;}
        public int BlockId { get;}
        public ulong BlockHash { get;}
        public string GetSaveState();
        
        /// <summary>
        /// ブロックで何らかのステートが変化したときに呼び出されます
        /// 例えば、動いている機械が止まったなど
        /// クライアント側で稼働アニメーションや稼働音を実行するときに使用します
        /// </summary>
        public event Action<ChangedBlockState> OnBlockStateChange;
    }

    public class ChangedBlockState
    {
        public readonly string CurrentState;
        public readonly string PreviousState;

        public readonly byte[] CurrentStateData;
        
        public ChangedBlockState(string currentState, string previousState, byte[] currentStateData)
        {
            CurrentState = currentState;
            PreviousState = previousState;
            CurrentStateData = currentStateData;
        }

    }
}