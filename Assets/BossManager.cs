using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bloodborne
{
    public class BossManager : CharacterManager
    {
        BossLocomotionManager bossLocomotionManager;
        public bool isPerformingAction;
        
        [Header("A.I Settings")]
        public float detectionRadius;
        public float maximumDetectionAngle = 50;
        public float minimumDetectionAngle = -50;

        private void Start()
        {
            bossLocomotionManager= GetComponent<BossLocomotionManager>();
        }

        private void Update()
        {
            Debug.Log(bossLocomotionManager.currentTarget.transform.position);
        }

        public void HandleCurrentAction()
        {
            if(bossLocomotionManager.currentTarget == null)
            {
                bossLocomotionManager.HandleDetection();
            }
            else
            {
                bossLocomotionManager.HandleMoveToTarget();
            }
        }

        private void FixedUpdate()
        {
            HandleCurrentAction();
        }
    }
}
