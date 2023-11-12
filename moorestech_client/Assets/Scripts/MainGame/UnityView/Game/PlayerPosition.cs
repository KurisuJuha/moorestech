﻿using Game.World.Interface.DataStore;
using MainGame.Basic;
using MainGame.UnityView.Block;
using StarterAssets;
using UnityEngine;

namespace MainGame.UnityView.Game
{
    public class PlayerPosition : MonoBehaviour, IPlayerPosition
    {
        [SerializeField] private ThirdPersonController controller;

        public Vector2 GetPlayerPosition()
        {
            var position = transform.position;
            return new Vector2(position.x, position.z);
        }

        /// <summary>
        ///     注意：アップデートのタイミングによってはThirdPersonController.csによる戻しが発生する可能性がある
        ///     セットしても位置が変わらなかった時はThirdPersonController.csをオフにして位置がセットできているか試してください
        /// </summary>
        /// <param name="vector2"></param>
        public void SetPlayerPosition(Vector2 vector2)
        {
            //サーバー側は2次元なのでx,yだが、unityはy upなのでzにyを入れる
            var firstPlayerPos = new Vector3(vector2.x, transform.position.y, vector2.y);
            var (position, rotation, scale) = SlopeBlockPlaceSystem.GetSlopeBeltConveyorTransform(firstPlayerPos.AddBlockPlaceOffset(), BlockDirection.North);
            controller.Warp(position);
        }

        public void SetActive(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}