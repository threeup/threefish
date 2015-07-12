using UnityEngine;
using BasicCommon;
using System.Collections.Generic;


namespace GoodFish
{
    [System.Serializable]
    public class Brain : MonoBehaviour
    {
        private delegate void TargetDelegate();
        private TargetDelegate Target;
        public Actor actor;
        public float scanRange = 20;

        private bool hasTarget = false;
        private bool hasScanned = false;
        private Transform targetTransform;
        private Vector3 targetPosition;

        private Dictionary<int, BrainRecord> memory = new Dictionary<int, BrainRecord>();

        private List<DirectionalData> choice = new List<DirectionalData>();

        private BasicTimer scanTimer = new BasicTimer(2f);
        private BasicTimer followTimer = new BasicTimer(2f);
        private BasicTimer retargetTimer = new BasicTimer(15f);
        void Awake()
        {
            for(int i=0; i<8; ++i)
            {
                choice.Add(new DirectionalData(null));
            }
            scanTimer.Randomize();
            followTimer.Randomize();
            retargetTimer.Randomize();
            //Target = TargetBest;
            Target = TargetFar;
        }

        public void SetAI(bool val)
        {
            AIManager.Instance.Register(this, val);
        }

        public void MinorTick(float deltaTime)
        {

            if( scanTimer.Tick(deltaTime) )
            {
                Scan();
            }
            if( hasScanned && retargetTimer.Tick(deltaTime) )
            {
                Target();
            }
            if( followTimer.Tick(deltaTime) )
            {
                if ( targetTransform != null )
                {
                    targetPosition = targetTransform.position;
                }
            }
        }

        public void Tick(float deltaTime)
        {
            if( actor.LockInput )
            {
                return;
            }
            float horizontalAxis = 0;
            float verticalAxis = 0;
            bool shouldSwim = false;
            if( !hasTarget )
            {
                shouldSwim = Random.Range(0, 10) > 2;
                verticalAxis = Random.Range(-1f, 1f);
                verticalAxis *= Mathf.Abs(verticalAxis);
                actor.HandleInput(deltaTime, horizontalAxis, verticalAxis, shouldSwim, false);
                return;    
            }
            Vector3 target = targetPosition;
            Vector3 forward = actor.Forward;
            Vector3 diff = target - actor.transform.position;

			bool wrongWay = Mathf.Sign(actor.Forward.z) != Mathf.Sign(diff.z);
			
			if( wrongWay )
			{
				horizontalAxis = 1f;
			}
			else
			{
				shouldSwim = diff.sqrMagnitude > 1;
				Vector3 desiredForward = diff.normalized;
				//float angle = Vector3.Angle(forward, desiredForward);
				float diffY = desiredForward.y - forward.y;
				verticalAxis = diffY;
			}

            
            actor.HandleInput(deltaTime, horizontalAxis, verticalAxis, shouldSwim, false);
        }

        public void Scan()
        {
            List<Actor> actors = World.Instance.allActors;
            for(int i=0; i<actors.Count; ++i)
            {
                Actor other = actors[i];
                if( other == actor )
                {
                    continue;
                }
                float range = Range(actor, other);
                if( !memory.ContainsKey(other.uid) )
                {
                    memory[other.uid] = new BrainRecord(other);
                }
                BrainRecord record = memory[other.uid];
                record.position = other.transform.position;
                record.age = 0;
                record.range = range;
                int desire = WeightDesire(actor, other) * RangeDesire(range, scanRange);
                record.desire = desire;

                int dir = Direction(actor, other);
                DirectionalData dirData = choice[dir];
                if( dirData.bestRecord == null)
                {
                    dirData.bestRecord = record;
                }
                else if ( dirData.bestRecord.desire < desire )
                {
                    dirData.bestRecord = record;
                }
                dirData.desireSum += desire;
                choice[dir] = dirData;
            }
            hasScanned = true;
        }

        public static Vector3 GetVector(int dir)
        {
            switch(dir)
            {
                case 0: return new Vector3(0,-1,0);
                case 1: return new Vector3(0,-1,1);
                case 2: return new Vector3(0,0,1);
                case 3: return new Vector3(0,1,1);
                case 4: return new Vector3(0,1,0);
                case 5: return new Vector3(0,1,-1);
                case 6: return new Vector3(0,0,-1);
                case 7: return new Vector3(0,-1,-1);

            }
            return Vector3.zero;
        }

        public void SetTargetRecord(BrainRecord record, int dir)
        {
            Vector3 generalTarget = actor.transform.position + GetVector(dir)*10f;
			if( record == null )
			{
				targetPosition = generalTarget;
                Debug.DrawLine(actor.transform.position, generalTarget, Color.green, 4f);
                return;
			}
            if( record.target != null )
            {
                targetTransform = record.target.transform;
            }
            else
            {
                targetTransform = null;
            }
            targetPosition = record.position;
            Debug.DrawLine(actor.transform.position, record.position, Color.magenta, 4f);
			Debug.DrawLine(actor.transform.position, generalTarget, Color.blue, 4f);
        }

        public static int Direction(Actor self, Actor other)
        {
            Vector3 diff = self.transform.position - other.transform.position;
            float angle = Vector3.Angle(diff, Vector3.up);
            if( angle < 0 ) 
            {
                angle += 360;
            }
            if( angle < 45f) { return 0; }
            if( angle < 90f) { return 1; }
            if( angle < 135f) { return 2; }
            if( angle < 180f) { return 3; }
            if( angle < 215f) { return 4; }
            if( angle < 270f) { return 5; }
            if( angle < 315f) { return 6; }
            if( angle < 360f) { return 7; }
            return 0;

        }

        public static float Range(Actor self, Actor other)
        {
            return (self.transform.position - other.transform.position).magnitude;
        }

        public static int WeightDesire(Actor self, Actor other)
        {
            return self.weight - other.weight;
        }

        public static int RangeDesire(float range, float scanRange)
        {
            if( range < scanRange/4 )
            {
                return 10;
            }
            else if( range < scanRange/2 )
            {
                return 4;
            }
            else if( range < scanRange )
            {
                return 1;
            }
            return 0;
        }


        public void TargetBest()
        {
            BrainRecord bestRecord = null;
            int bestDesire = 0;
            int worstDesire = 0;
            int bestDir = -1;
            //int worstDir = -1;
            for(int i=0; i<8; ++i)
            {
                if( choice[i].desireSum > bestDesire )
                {
                    bestDesire = choice[i].desireSum;
                    bestRecord = choice[i].bestRecord;
                    bestDir = i;
                }
                if( choice[i].desireSum < worstDesire )
                {
                    worstDesire = choice[i].desireSum;
                    //worstDir = i;
                }
            }
            /*if( bestDir == -1 )
            {
                bestDir = ( worstDir + 4 ) % 8;
            }*/
            if( bestDir != -1 )
            {
                SetTargetRecord(bestRecord, bestDir);
                hasTarget = true;
            }
            else
            {
                TargetFar();
            }
            
            
        }

        public void TargetFar()
        {
            targetTransform = null;
            //Vector3 selfPos = actor.transform.position;

            Bounds liveable = World.Instance.GetLiveableArea(actor.liveableArea);

            if( actor.transform.position.z < 0 )
            {
                targetPosition = new Vector3(0, Random.Range(liveable.max.y+5, liveable.min.y-5), liveable.max.z);
            }
            else
            {
                targetPosition = new Vector3(0, Random.Range(liveable.max.y+5, liveable.min.y-5), liveable.min.z);
            }
            hasTarget = true;
            
        }

    }
}