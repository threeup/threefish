using UnityEngine;
using BasicCommon;
using System.Collections;
using DG.Tweening;

namespace GoodFish
{
[System.Serializable]
    public class ActorMotor : MonoBehaviour
    {

        public Actor actor;

        public float friction;
        public float motorAcceleration;
        public float maxSpeed;

        private float turnSpeed = 150f;
        private float pitchAcceleration = -100f;

        float speed = 0f;
        Vector3 velocity = Vector3.zero;
        Vector3 motorForward = Vector3.one;
    
        public float Speed { get { return speed; } }
        public Vector3 Velocity { get { return velocity; } }
        public Vector3 MotorForward { get { return motorForward; } }
    
        public Transform thisTransform;


        private BasicTimer sequenceLock = new BasicTimer(0f, false);
        protected Sequence mySequence = null;
        public bool LockInput { get { return !sequenceLock.Paused; } }
        
        
        void Start()
        {
            RecomputeForward();
        }

        void OnEnable()
        {
            thisTransform = this.gameObject.transform;
        }

        void Update()
        {
            float deltaTime = Time.deltaTime;

            if( sequenceLock.Tick(deltaTime) )
            {
                mySequence = null;
            }
            RecomputeForward();
        }

        protected void RecomputeForward()
        {
            motorForward = thisTransform.forward;
            motorForward.x = 0;
        }

        public void SetPosition(Vector3 position)
        {
            this.transform.position = position;
        }

        public void SetPosition(float? x, float? y, float? z)
        {
            Vector3 newPos = thisTransform.position;
            if( x != null ) { newPos.x = x.Value; }
            if( y != null ) { newPos.y = y.Value; }
            if( z != null ) { newPos.z = z.Value; }
            this.transform.position = newPos;
        }

        public void SetSpeed(float val)
        {
            speed = val;
        }

        public void Throttle(float deltaTime, float amount)
        {
            if( thisTransform == null )
            {
                Debug.LogError("no transform"+this);
            }
            float appliedFriction = -friction*speed;
            float desiredSpeed = Mathf.Clamp(speed + amount*motorAcceleration*deltaTime + appliedFriction*deltaTime, -maxSpeed, maxSpeed);
            MotorMove(deltaTime, desiredSpeed);
        }

        public void MotorMove(float deltaTime, float desiredSpeed)
        {
            
            velocity = desiredSpeed*motorForward;
            SafeMove(thisTransform.position + velocity*deltaTime);
            speed = Vector3.Dot(velocity, motorForward);
            return;
        }

        public virtual void SafeMove(Vector3 desiredPos)
        {
            Bounds liveable = World.Instance.GetLiveableArea(actor.liveableArea);
            if( desiredPos.x < liveable.min.x )
            {
                desiredPos.x = thisTransform.position.x;
                velocity.x = 0;
                OnCollideMinX();
            }
            if( desiredPos.x > liveable.max.x )
            {
                desiredPos.x = liveable.max.x;
                velocity.x = 0;
                OnCollideMaxX();
            }
            if( desiredPos.y < liveable.min.y )
            {
                desiredPos.y = thisTransform.position.y;
                velocity.y = 0;
                OnCollideMinY();
            }
            if( desiredPos.y > liveable.max.y )
            {
                desiredPos.y = thisTransform.position.y;
                velocity.y = 0;
                OnCollideMaxY();
            }
            if( desiredPos.z < liveable.min.z )
            {
                desiredPos.z = thisTransform.position.z;
                velocity.z = 0;
                OnCollideMinZ();
            }
            if( desiredPos.z > liveable.max.z )
            {
                desiredPos.z = thisTransform.position.z;
                velocity.z = 0;
                OnCollideMaxZ();
            }
            this.transform.position = desiredPos;
        }

        public void Rotate(float deltaTime, float horizontalAxis, float verticalAxis)
        {

            bool isTurn = false;
            if( Mathf.Abs(verticalAxis) > 0.01f)
            {
                AddPitch(deltaTime, verticalAxis);
                RecomputeForward();
            }
            
            if( Mathf.Abs(horizontalAxis) > 0.8f)
            {
                isTurn = AddTurn(deltaTime, horizontalAxis);
            }


            if( isTurn )
            {
                speed = 0f;
                Vector3 backwards = Vector3.zero;
                if( this.transform.forward.z > 0f )
                {
                    backwards.z = -1f;
                }
                else
                {
                    backwards.z = 1f;
                }
                float duration = 0.25f;
                sequenceLock = new BasicTimer(duration, false);
                mySequence = DOTween.Sequence();
                mySequence.Append(transform.DOLookAt(thisTransform.position + backwards, duration, AxisConstraint.None));
            }
        }


        float GetYaw()
        {
            float yaw = thisTransform.rotation.eulerAngles.y;
            while( yaw > 90 )
            {
                yaw -= 180f;
            }
            while( yaw < -90 )
            {
                yaw += 180f;
            }
            return yaw;
        }

        bool AddTurn(float deltaTime, float axis)
        {
            this.transform.Rotate(Vector3.up, turnSpeed*axis*deltaTime);
            float yaw = GetYaw();
            float maxYaw = 35f;
            if( yaw > maxYaw )
            {
                return true;
            }
            if( yaw < -maxYaw )
            {
                return true;
            }
            return false;
        }


        void AddPitch(float deltaTime, float axis)
        {
            float pitch = thisTransform.rotation.eulerAngles.x;
            while( pitch > 180 )
            {
                pitch -= 360;
            }
            while( pitch < -180 )
            {
                pitch += 360;
            }
            if( pitch > 45 && axis < 0 )
            {
                return;
            }
            if( pitch < -45 && axis > 0 )
            {
                return;
            }
            this.transform.Rotate(Vector3.right, axis*pitchAcceleration*deltaTime);
        }

        protected virtual void OnCollideMinX() { }
        protected virtual void OnCollideMaxX() { }
        protected virtual void OnCollideMinY() { }
        protected virtual void OnCollideMaxY() { }
        protected virtual void OnCollideMinZ() { }
        protected virtual void OnCollideMaxZ() { }
    }
}