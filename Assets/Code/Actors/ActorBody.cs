using UnityEngine;
using System.Collections.Generic;


namespace GoodFish
{
    [System.Serializable]
    public class ActorBody : MonoBehaviour
    {
    	public Actor actor;
    	public ActorMotor motor;
        public Vector3 desiredScale = Vector3.one;
        public Collider bodyCollider;
        public Collider mouthCollider;

        private Transform thisTransform;
        void Awake()
        {
            thisTransform = this.transform;
            thisTransform.localScale = desiredScale;
        }

        public void Update()
        {
            
        }

        public void SetFacing(bool right)
        {
            Vector3 realScale = desiredScale;
            realScale.y *= right ? 1f : -1f;
            thisTransform.localScale = realScale;
        }

        public void SetCollidable(bool val)
        {
            if( mouthCollider != null )
            {
                mouthCollider.enabled = val;
            }
        	bodyCollider.enabled = val;
        }
    }
}