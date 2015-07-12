using UnityEngine;
using System.Collections.Generic;


namespace GoodFish
{
    [System.Serializable]
    public class ActorMouth : MonoBehaviour
    {
    	public Actor actor;
    	void Awake()
        {
        }

        public void Update()
        {
            
        }

        

        public void OnTriggerEnter(Collider otherCollider)
        {
            GameObject other = otherCollider.gameObject;
        	ActorBody otherBody = other.GetComponent<ActorBody>();
            
            if( otherBody == null || otherBody.actor == actor )
            {
                return;
            }

            Vector3 offset = other.transform.position - this.transform.position;
            float offsetAngle = Mathf.Atan2(offset.y, offset.z);
            Vector3 forward = actor.transform.forward;
            float forwardAngle = Mathf.Atan2(forward.y, forward.z);
        	Actor otherActor = otherBody != null ? otherBody.actor : null;
            float angleDiff = 0;
            if( Mathf.Abs(forwardAngle) > Mathf.PI/2 )
            {
                angleDiff = forwardAngle - offsetAngle;
            }
            else
            {
                angleDiff = offsetAngle - forwardAngle;
            }

            while(angleDiff > Mathf.PI) { angleDiff -= Mathf.PI; }
            while(angleDiff < -Mathf.PI) { angleDiff += Mathf.PI; }
        	actor.HandleTrigger(otherActor, angleDiff);
        }
    }
}