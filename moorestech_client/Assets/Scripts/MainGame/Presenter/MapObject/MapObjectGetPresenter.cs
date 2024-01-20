﻿using System.Threading;
using Cysharp.Threading.Tasks;
using Game.MapObject.Interface;
using Constant;
using Game.PlayerInventory.Interface;
using MainGame.Network.Send;
using MainGame.UnityView.Control;
using MainGame.UnityView.Game;
using MainGame.UnityView.MapObject;
using MainGame.UnityView.SoundEffect;
using MainGame.UnityView.UI.Inventory;
using MainGame.UnityView.UI.Inventory.Main;
using MainGame.UnityView.UI.UIState;
using MainGame.UnityView.Util;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace MainGame.Presenter.MapObject
{
    /// <summary>
    ///     マップオブジェクトのUIの表示や削除の判定を担当する
    /// </summary>
    public class MapObjectGetPresenter : MonoBehaviour
    {
        [SerializeField] private MiningObjectProgressbarPresenter miningObjectProgressbarPresenter;
        [SerializeField] private HotBarView hotBarView;
        [SerializeField] private float miningDistance = 1.5f;

        private CancellationToken _gameObjectCancellationToken;
        private CancellationTokenSource _miningCancellationTokenSource = new();

        private ILocalPlayerInventory _localPlayerInventory;
        private SendGetMapObjectProtocolProtocol _sendGetMapObjectProtocolProtocol;
        private UIStateControl _uiStateControl;
        private IPlayerPosition _playerPosition;

        [Inject]
        public void Constructor(UIStateControl uiStateControl, SendGetMapObjectProtocolProtocol sendGetMapObjectProtocolProtocol, ILocalPlayerInventory localPlayerInventory, IPlayerPosition playerPosition)
        {
            _uiStateControl = uiStateControl;
            _sendGetMapObjectProtocolProtocol = sendGetMapObjectProtocolProtocol;
            _localPlayerInventory = localPlayerInventory;
            _playerPosition = playerPosition;
            _gameObjectCancellationToken = this.GetCancellationTokenOnDestroy();
        }
        
        private MapObjectGameObject _currentMapObjectGameObject = null;

        private async UniTask Update()
        {
            if (!IsStartMining())
            {
                return;
            }

            await Mining();
            

            #region Internal

            bool IsStartMining()
            {
                if (_uiStateControl.CurrentState != UIStateEnum.GameScreen) return false;

                UpdateCurrentMapObject();

                if (_currentMapObjectGameObject == null) return false;
            
                var (_,mineable) = GetMiningData(_currentMapObjectGameObject.MapObjectType);

                if (!mineable) return false;
                if (miningObjectProgressbarPresenter.IsMining || !InputManager.Playable.ScreenLeftClick.GetKey) return false;

                return true;
            }

            async UniTask Mining()
            {
                _miningCancellationTokenSource.Cancel();
                _miningCancellationTokenSource = new CancellationTokenSource();

                //マイニングバーのUIを表示するやつを設定
                var (miningTime,_) = GetMiningData(_currentMapObjectGameObject.MapObjectType);
                miningObjectProgressbarPresenter.StartMining(miningTime, _miningCancellationTokenSource.Token).Forget();

                var isMiningFinish = await IsMiningFinishWait(miningTime);

                //マイニングをキャンセルせずに終わったので、マイニング完了をサーバーに送信する
                if (isMiningFinish)
                {
                    _sendGetMapObjectProtocolProtocol.Send(_currentMapObjectGameObject.InstanceId);
                    PlaySoundEffect();
                }

                _miningCancellationTokenSource.Cancel();
            }
            
            

            void UpdateCurrentMapObject()
            {
                var mapObject = GetOnMouseMapObject();
                if (mapObject == null)
                {
                    if (_currentMapObjectGameObject != null)
                    {
                        _currentMapObjectGameObject.OutlineEnable(false);
                    }
                    _currentMapObjectGameObject = null;
                    return;
                }

                if (_currentMapObjectGameObject == mapObject) return;
                
                if (_currentMapObjectGameObject != null)
                {
                    _currentMapObjectGameObject.OutlineEnable(false);
                }
                _currentMapObjectGameObject = mapObject;
                _currentMapObjectGameObject.OutlineEnable(true);
            }

            (float miningTime, bool mineable) GetMiningData(string mapObjectType)
            {
                var slotIndex = PlayerInventoryConst.HotBarSlotToInventorySlot(hotBarView.SelectIndex);

                //TODO 採掘するためのアイテムはコンフィグに移す（mapObject.jsonとか作る？）
                var isStoneTool = _localPlayerInventory.IsItemExist(AlphaMod.ModId, "stone tool", slotIndex);
                var isStoneAx = _localPlayerInventory.IsItemExist(AlphaMod.ModId, "stone ax", slotIndex);
                var isIronAx = _localPlayerInventory.IsItemExist(AlphaMod.ModId, "iron ax", slotIndex);
                var isIronPickaxe = _localPlayerInventory.IsItemExist(AlphaMod.ModId, "iron pickaxe", slotIndex);

                switch (mapObjectType)
                {
                    #region 木

                    case VanillaMapObjectType.VanillaTree when isIronAx:
                        return (4, true);
                    case VanillaMapObjectType.VanillaTree when isStoneAx:
                        return (4, true);
                    case VanillaMapObjectType.VanillaTree when isStoneTool:
                        return (10, true);
                    case VanillaMapObjectType.VanillaTree:
                        return (10000, true);

                    case VanillaMapObjectType.VanillaBigTree when isIronAx:
                        return (10, true);
                    case VanillaMapObjectType.VanillaBigTree:
                        return (10000, true);

                    #endregion

                    #region 石

                    case VanillaMapObjectType.VanillaStone:
                        return (5, true);


                    case VanillaMapObjectType.VanillaCoal when isIronPickaxe:
                        return (5, true);
                    case VanillaMapObjectType.VanillaCoal:
                        return (10000, true);
                    case VanillaMapObjectType.VanillaIronOre when isIronPickaxe:
                        return (10, true);
                    case VanillaMapObjectType.VanillaIronOre:
                        return (10000, true);

                    case VanillaMapObjectType.VanillaCray when isStoneAx:
                        return (3, true);
                    case VanillaMapObjectType.VanillaCray:
                        return (10000, true);

                    #endregion

                    #region ブッシュ

                    case VanillaMapObjectType.VanillaBush:
                        return (3, true);

                    #endregion
                }

                return (5, false);
            }
            
            async UniTask<bool> IsMiningFinishWait(float miningTime)
            {
                //map objectがフォーカスされ、クリックされているので採掘を行う
                //採掘中はこのループの中にいる
                //採掘時間分ループする
                var nowTime = 0f;
                while (nowTime < miningTime)
                {
                    await UniTask.Yield(PlayerLoopTiming.Update, _gameObjectCancellationToken);
                    nowTime += Time.deltaTime;

                    //クリックが離されたら採掘を終了する か map objectが変わったら採掘を終了する
                    if (InputManager.Playable.ScreenLeftClick.GetKeyUp || _currentMapObjectGameObject != GetOnMouseMapObject())
                    {
                        return false;
                    }
                }

                return true;
            }

            void PlaySoundEffect()
            {
                SoundEffectType soundEffectType;
                switch (_currentMapObjectGameObject.MapObjectType)
                {
                    case VanillaMapObjectType.VanillaStone:
                    case VanillaMapObjectType.VanillaCray:
                    case VanillaMapObjectType.VanillaCoal:
                    case VanillaMapObjectType.VanillaIronOre:
                        soundEffectType = SoundEffectType.DestroyStone;
                        break;
                    case VanillaMapObjectType.VanillaTree:
                    case VanillaMapObjectType.VanillaBigTree:
                        soundEffectType = SoundEffectType.DestroyTree;
                        break;
                    case VanillaMapObjectType.VanillaBush:
                        soundEffectType = SoundEffectType.DestroyBush;
                        break;
                    default:
                        soundEffectType = SoundEffectType.DestroyStone;
                        Debug.LogError("採掘音が設定されていません");
                        break;
                }

                SoundEffectManager.Instance.PlaySoundEffect(soundEffectType);
            }
            
            #endregion
        }

        private MapObjectGameObject GetOnMouseMapObject()
        {
            //スクリーンからマウスの位置にRayを飛ばす
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, 10)) return null;
            if (EventSystem.current.IsPointerOverGameObject()) return null;
            if (!hit.collider.gameObject.TryGetComponent(out MapObjectGameObject mapObject)) return null;
                
            var playerPos = _playerPosition.GetPlayerPosition3D();
            var mapObjectPos = mapObject.transform.position;
            if (miningDistance < Vector3.Distance(playerPos, mapObjectPos)) return null;

            return mapObject;
        }



    }
}