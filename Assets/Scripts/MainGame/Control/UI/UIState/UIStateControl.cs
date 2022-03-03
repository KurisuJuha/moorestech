﻿using System.Collections.Generic;
using MainGame.Control.UI.UIState.UIState;
using MainGame.Network.Receive;
using UnityEngine;
using VContainer;

namespace MainGame.Control.UI.UIState
{
    public class UIStateControl : MonoBehaviour
    {
        private  UIStateEnum _currentState;

        public UIStateEnum CurrentState => _currentState;

        private UIStateDictionary _uiStateDictionary;
        
        [Inject]
        public void Construct(UIStateDictionary uiStateDictionary,MoorestechInputSettings inputSettings)
        {
            inputSettings.Enable();
            _uiStateDictionary = uiStateDictionary;
        }
        
        
        //UIステート
        private void FixedUpdate()
        {
            //UIステートが変更されたら
            if (!_uiStateDictionary.GetState(_currentState).IsNext()) return;
            
            //現在のUIステートを終了し、次のステートを呼び出す
            _uiStateDictionary.GetState(_currentState).OnExit();
            _currentState = _uiStateDictionary.GetState(_currentState).GetNext();
            _uiStateDictionary.GetState(_currentState).OnEnter();
        }
    }
}