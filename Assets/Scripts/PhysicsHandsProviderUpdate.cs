using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Leap.PhysicalHands;
using Leap;

namespace ProjectMayday
{
    public class PhysicsHandsProviderUpdate : MonoBehaviour
    {
        // Start is called before the first frame update
        [SerializeField] XRLeapProviderManager leapProviderManager;
        [SerializeField] GameObject physicsManagerGameObject;
        PhysicalHandsManager physicsHandsManager;

        bool hasAssigned = false;
        float timer = 0;
        const float duration = 2;

		private void Awake()
		{
            physicsHandsManager = physicsManagerGameObject.GetComponent<PhysicalHandsManager>();
		}


		void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(!hasAssigned)
            {
                timer += Time.deltaTime;
                if(timer > duration)
                {
                    physicsHandsManager.InputProvider = leapProviderManager.LeapProvider;
                    hasAssigned = true;
                }
            }
        }
    }
}