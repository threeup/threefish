using UnityEngine;
using BasicCommon;
using System.Collections.Generic;
using Vectrosity;

namespace GoodFish
{
    [System.Serializable]
    public class Brain : MonoBehaviour
    {
        private delegate void TargetDelegate();
        private TargetDelegate Target;
        public Actor actor;

        public float innerRange = 10;
        public float outerRange = 30;

        private List<DirectionalData> choice = new List<DirectionalData>();
        private Dictionary<int, BrainRecord> records = new Dictionary<int, BrainRecord>();

        private BasicTimer scanTimer = new BasicTimer(2f);
        
        private bool hasTarget = false;
        private Vector3 worstPosition;
        private Vector3 bestPosition;
        private Vector3 targetPosition;

        public Material lineMaterial;
        private VectorLine worstLine;
        private VectorLine bestLine;
        private VectorLine goodSpiderLine;
        private VectorLine badSpiderLine;

        private bool canScan = true;
        public BrainBehaviour behaviour;

        private Direction bestDir = Direction.CENTER;
        private Direction worstDir = Direction.CENTER;
        private bool preferBest = true;

        void Awake()
        {
            int dirCount = System.Enum.GetValues(typeof(Direction)).Length;
            for(int i=0; i<dirCount; ++i)
            {
                choice.Add(new DirectionalData());
            }
            scanTimer.Randomize();

            worstLine = new VectorLine ("BrainAvoid", new Vector3[6], lineMaterial, 2f);
            worstLine.color = Color.red;
            bestLine = new VectorLine ("BrainTarget", new Vector3[6], lineMaterial, 2f);
            bestLine.color = Color.green;
            badSpiderLine = new VectorLine ("BadSpider", new Vector3[32], lineMaterial, 2f);
            badSpiderLine.color = Color.red;
            goodSpiderLine = new VectorLine ("GoodSpider", new Vector3[32], lineMaterial, 2f);
            goodSpiderLine.color = Color.blue;
        }


        public void SetAI(bool val)
        {
            AIManager.Instance.Register(this, val);
        }


        public void MinorTick(float deltaTime)
        {
            if( scanTimer.Tick(deltaTime) && canScan )
            {
                Scan();
                TargetBest();
            }
        }

        void Update()
        {
            if( bestDir != Direction.CENTER )
            {
                BrainRecord worstRecord = choice[(int)worstDir].worstRecord;
                if( worstRecord != null )
                {
                    worstPosition = worstRecord.position;
                }
                else
                {
                    worstPosition = actor.transform.position + Utils.GetVector(worstDir)*5f;  
                }
                BrainRecord bestRecord = choice[(int)bestDir].bestRecord;
                if( bestRecord != null )
                {
                    bestPosition = bestRecord.position;
                }
                else
                {
                    bestPosition = actor.transform.position + Utils.GetVector(bestDir)*5f;  
                }
                targetPosition = preferBest ? bestPosition : worstPosition;
                hasTarget = true;
            }
            if( hasTarget )
            {
                worstLine.MakeSpline(new Vector3[]{ worstPosition, this.transform.position }, 2);
                bestLine.MakeSpline(new Vector3[]{ this.transform.position, bestPosition }, 2);
                //worstLine.Draw();
                bestLine.Draw();
            }

            if( canScan )
            {
                List<Vector3> spiders = new List<Vector3>();
                for(int i=0; i<8; ++i)
                {
                    float desire = Mathf.Clamp(choice[i].GoodSum(behaviour), 10f, 100f);
                    spiders.Add(this.transform.position + Utils.GetVector(i)*desire*0.1f);
                }
                spiders.Add(spiders[0]);
                goodSpiderLine.MakeSpline(spiders.ToArray(), spiders.Count);
                goodSpiderLine.Draw();

                spiders.Clear();
                for(int i=0; i<8; ++i)
                {
                    float desire = Mathf.Clamp(choice[i].BadSum(behaviour), -100f, -11f);
                    spiders.Add(this.transform.position - Utils.GetVector(i)*desire*0.1f);
                }
                spiders.Add(spiders[0]);
                badSpiderLine.MakeSpline(spiders.ToArray(), spiders.Count);
                badSpiderLine.Draw();
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

        private void Scan()
        {
            Vector3 actorPos = actor.transform.position;
            Vector3 facing = actor.Forward;
            Bounds liveable = World.Instance.GetLiveableArea(actor.liveableArea);
            for(int i=0; i<choice.Count; ++i)
            {
                Direction dir = (Direction)i;
                Vector3 dirVector = Utils.GetVector(dir);
                float dotp = Vector3.Dot(facing, dirVector);
                DirectionalData dirData = choice[i];
                dirData.Clear();
                if( dotp > 0.8 )
                {
                    dirData.straight = 10;
                }
                else if( dotp > 0.6)
                {
                    dirData.straight = 6;
                }
                else
                {
                    dirData.straight = 0;
                }
                dirData.eastWest = EastWestDesire(dir);
                if( dir == Direction.CENTER )
                {
                    //temp
                    dirData.wall = -10;
                }
                else
                {
                    dirData.wall = WallDesire(dir, actorPos, liveable);
                }
                choice[i] = dirData;
            }

            List<Actor> actors = World.Instance.allActors;
            for(int i=0; i<actors.Count; ++i)
            {
                Actor other = actors[i];
                if( other == actor )
                {
                    continue;
                }
                float range = Range(actor, other);

                BrainRecord record = null;
                if( records.ContainsKey(other.uid) )
                {
                    record = records[other.uid];
                }
                else
                {
                    record = new BrainRecord(other.uid);
                    records.Add(other.uid, record);
                }

                Direction dir = Direction.CENTER;
                if( range > 3 )
                {
                    dir = Utils.GetDirection(actor, other);
                }
                int d = (int)dir;
                DirectionalData dirData = choice[d];
                record.position = other.transform.position;
                record.age = 0;
                record.range = range;
                record.fear = FearDesire(actor, other);
                record.friend = FriendDesire(actor, other);
                record.food = FoodDesire(actor, other);
                dirData.fear = Mathf.Max(dirData.fear + record.fear, -10f);
                dirData.friend = Mathf.Min(dirData.friend + record.friend, 10f);
                dirData.food = Mathf.Min(dirData.food + record.food, 10f);
                if( record.WorseThan(dirData.worstRecord, behaviour) )
                {
                    dirData.worstRecord = record;
                }
                if( record.BetterThan(dirData.bestRecord, behaviour) )
                {
                    dirData.bestRecord = record;

                }
                
                choice[d] = dirData;
            }
        }

        void TargetBest()
        {
            float bestDesire = -99999;
            bestDir = Direction.CENTER;
            float worstDesire = 99999;
            worstDir = Direction.CENTER;
            for(int i=0; i<8; ++i)
            {
                float desire = choice[i].TotalSum(behaviour);
                if( desire > bestDesire )
                {
                    bestDesire = desire;
                    bestDir = (Direction)i;
                }
                if( desire < worstDesire )
                {
                    worstDesire = desire;
                    worstDir = (Direction)i;
                }
            }
            preferBest = Mathf.Abs(bestDesire) > Mathf.Abs(worstDesire); 
        }

        public static float Range(Actor self, Actor other)
        {
            return (self.transform.position - other.transform.position).magnitude;
        }

        public static float EastWestDesire(Direction dir)
        {
            return Mathf.Round(Mathf.Abs(Utils.GetVector(dir).z)*10f);
        }

        public static float WallDesire(Direction dir, Vector3 pos, Bounds liveable)
        {
            float wallDistance = Utils.DistanceToBounds(dir, pos, liveable);
            if( wallDistance < 3f)
            {
                return 10f;
            }
            else if( wallDistance < 5f)
            {
                return 5f;
            }
            else if( wallDistance < 8f)
            {
                return 2f;
            }
            return 0f;
        }                

        public static float FearDesire(Actor self, Actor other)
        {
            return (other.weight - self.weight) > 2 ? 6f : 0f;
        }

        public static float FriendDesire(Actor self, Actor other)
        {
            return self.actorType == other.actorType ? 4f : 0f;
        }

        public static float FoodDesire(Actor self, Actor other)
        {
            return (self.weight - other.weight) > 2 ? 6f : 0f;
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

        
    }
}