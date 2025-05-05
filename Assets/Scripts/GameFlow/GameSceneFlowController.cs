using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameFlow
{
    [RequireComponent(typeof(GameTimer))]
    public class GameSceneFlowController : MonoBehaviour
    {
        private GameSceneFlowContext _gameSceneFlowContext;

        [Header("Box Transforms")] 
        public List<Transform> boxTransforms;
        
        [Header("Box Objects")] 
        public List<GameObject> pfBoxObjects;
        
        private void Awake()
        {
            _gameSceneFlowContext = new GameSceneFlowContext();
            _gameSceneFlowContext.Initialize(this);
        }

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        private void OnBoxGenCalled()
        {
            
        }
    }
}
