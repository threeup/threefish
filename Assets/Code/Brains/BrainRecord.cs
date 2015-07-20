using UnityEngine;
using System.Collections.Generic;


namespace GoodFish
{
    [System.Serializable]
    public class BrainRecord
    {
        public Vector3 position;
    	public int actorUID;
        public float age;
        public float range;
        public float fear;
        public float friend;
        public float food;

        public BrainRecord(int actorUID)
        {
            this.actorUID = actorUID;
        }

        public bool BetterThan(BrainRecord other, BrainBehaviour behaviour)
        {
            float lhs = GoodSum(behaviour);
            if( other == null )
            {
                return lhs > 0;
            }
            float rhs = other.GoodSum(behaviour);
            return lhs > rhs;
        }

        public bool WorseThan(BrainRecord other, BrainBehaviour behaviour)
        {
            float lhs = BadSum(behaviour);
            if( other == null  )
            {
                return lhs < 0; 
            }
            float rhs = other.BadSum(behaviour);
            return lhs < rhs;
        }

        public float GoodSum(BrainBehaviour behaviour)
        {
            float sum = 0;
            if( behaviour.fear > 0 ) { sum += fear*behaviour.fear; }
            if( behaviour.friend > 0 ) { sum += friend*behaviour.friend; }
            if( behaviour.food > 0 ) { sum += food*behaviour.food; }
            return sum;
        }
        public float BadSum(BrainBehaviour behaviour)
        {
            float sum = 0;
            if( behaviour.fear < 0 ) { sum += fear*behaviour.fear; }
            if( behaviour.friend < 0 ) { sum += friend*behaviour.friend; }
            if( behaviour.food < 0 ) { sum += food*behaviour.food; }
            return sum;
        }

    }

    public struct DirectionalData
    {
        public BrainRecord bestRecord;
        public BrainRecord worstRecord;
        public float fear;
        public float friend;
        public float food;
        public float straight;
        public float eastWest;
        public float wall;

        public void Clear()
        {
            bestRecord = null;
            worstRecord = null;
            fear = 0; 
            friend = 0; 
            food = 0; 
            straight = 0; 
            eastWest = 0; 
            wall = 0; 
        }

        public float GoodSum(BrainBehaviour behaviour)
        {
            float sum = 0;
            if( behaviour.fear > 0 ) { sum += fear*behaviour.friend; }
            if( behaviour.friend > 0 ) { sum += friend*behaviour.friend; }
            if( behaviour.food > 0 ) { sum += food*behaviour.friend; }
            if( behaviour.straight > 0 ) { sum += straight*behaviour.straight; }
            if( behaviour.eastWest > 0 ) { sum += eastWest*behaviour.eastWest; }
            if( behaviour.wall > 0 ) { sum += wall*behaviour.wall; }
            return sum;
        }
        public float BadSum(BrainBehaviour behaviour)
        {
            float sum = 0;
            if( behaviour.fear < 0 ) { sum += fear*behaviour.friend; }
            if( behaviour.friend < 0 ) { sum += friend*behaviour.friend; }
            if( behaviour.food < 0 ) { sum += food*behaviour.friend; }
            if( behaviour.straight < 0 ) { sum += straight*behaviour.straight; }
            if( behaviour.eastWest < 0 ) { sum += eastWest*behaviour.eastWest; }
            if( behaviour.wall < 0 ) { sum += wall*behaviour.wall; }
            return sum;
        }

        public float TotalSum(BrainBehaviour behaviour)
        {
            return  fear*behaviour.fear +
                    friend*behaviour.friend +
                    food*behaviour.food +
                    straight*behaviour.straight +
                    eastWest*behaviour.eastWest +
                    wall*behaviour.wall;
        }

        public string StringSum()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("Fear ");
            sb.Append(fear.ToString());
            sb.Append("friend ");
            sb.Append(friend.ToString());
            sb.Append("food ");
            sb.Append(food.ToString());
            sb.Append("straight ");
            sb.Append(straight.ToString());
            sb.Append("eastWest ");
            sb.Append(eastWest.ToString());
            sb.Append("wall ");
            sb.Append(wall.ToString());
            return sb.ToString();
        }
    }

    [System.Serializable]
    public struct BrainBehaviour
    {
        public float fear;
        public float friend;
        public float food;
        public float straight;
        public float eastWest;
        public float wall;

        public BrainBehaviour(float fear, float friend, float food, float straight, float eastWest, float wall)
        {
            this.fear = fear;
            this.friend = friend;
            this.food = food;
            this.straight = straight;
            this.eastWest = eastWest;
            this.wall = wall;
        }

        public static BrainBehaviour BackForth = new BrainBehaviour(0,0,0,5,10,10);
        public static BrainBehaviour Hungry = new BrainBehaviour(0,0,5,5,10,10);
    }

}