﻿using MainGame.Control.Game.MouseKeyboard;
using MainGame.Control.UI.UIState.UIState;
using MainGame.Network.Receive;
using MainGame.Network.Send;
using UnityEngine;
using VContainer;

namespace MainGame.Control.UI.UIState
{
    public class UIStateControl : MonoBehaviour
    {
        [SerializeField] private GameObject pauseMenu;
        [SerializeField] private GameObject playerInventory;
        [SerializeField] private GameObject blockInventory;

        
        private  IUIState _currentState;
        private MoorestechInputSettings _inputSettings;

        
        [Inject]
        public void Construct(IBlockClickDetect blockClickDetect,RequestPlayerInventoryProtocol inventoryProtocol)
        {
            _inputSettings = new MoorestechInputSettings();
            _inputSettings.Enable();

            //ステートマシンの設定
            var gameScreen = new GameScreenState();
            var inventory = new PlayerInventoryState(gameScreen,_inputSettings,playerInventory,inventoryProtocol);
            var blockInventoryState = new BlockInventoryState(gameScreen,_inputSettings,blockInventory);
            var pause = new PauseMenuState(gameScreen,_inputSettings,pauseMenu);
            
            gameScreen.Construct(inventory,pause,blockInventoryState,_inputSettings,blockClickDetect);
            _currentState = gameScreen;
        }
        
        
        //UIステート
        private void FixedUpdate()
        {
            //UIステートが変更されたら
            if (!_currentState.IsNext()) return;
            
            //現在のUIステートを終了し、次のステートを呼び出す
            _currentState.OnExit();
            _currentState = _currentState.GetNext();
            _currentState.OnEnter();
        }
    }
}