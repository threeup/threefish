using UnityEngine;
using System.Collections.Generic;


namespace GoodFish
{
    public class User : MonoBehaviour
    {
        public bool isControllable = false;
        public bool isAI = true;
        public List<ActorType> actorTypes = new List<ActorType>(); 
    	public Actor captain; 
        public List<Actor> grunts;
        public int lives = 3;
        public int captainLives = 3;
        public int actorCount = 1;

        void Awake()
        {
            
        }

        public void StartGame()
        {
            for(int i=0; i<actorCount; ++i)
            {
                int rangeVal = Random.Range(0,actorTypes.Count);
                ActorType actorType = actorTypes[rangeVal];
                Actor spawn = SpawnBoss.Instance.Spawn(actorType);
                spawn.OnDead += HandleDeadActor;
                if( captain == null && isControllable )
                {
                    captain = spawn;
                    if( captain.brain != null )
                    {
                        captain.brain.SetAI(isAI);
                    }
                }
                else 
                {
                    grunts.Add(spawn);
                    if( spawn.brain != null )
                    {
                        spawn.brain.SetAI(true);
                    }
                }
                
            }
        }

        public void SetAI(bool val)
        {
            isAI = val;
            if( captain != null && captain.brain != null )
            {
                captain.brain.SetAI(val);
            }
        }


        void HandleDeadActor(Actor actor)
        {
            if( actor == captain )
            {
                captainLives--;
                if( captainLives == 0 )
                {
                    Debug.Log("Game over");
                }
                else
                {
                    captain = SpawnBoss.Instance.Spawn(actor.actorType);
                    if( captain.brain != null )
                    {
                        captain.brain.SetAI(isAI);
                    }
                }
            }
            else
            {
                grunts.Remove(actor);
                lives--;
                if( lives == 0 )
                {
                    Debug.Log("Dry");
                }
                else
                {
                    Actor spawn = SpawnBoss.Instance.Spawn(actor.actorType);
                    grunts.Add(spawn);
                    if( spawn.brain != null )
                    {
                        spawn.brain.SetAI(true);
                    }
                }
            }
            
        }

        public void Activate()
        {
            if( captain == null )
            {
                Debug.Log("no captain");
                return;
            }
            switch( captain.liveableArea )
            {
                case World.LiveableArea.SURFACE: World.Instance.GotoSurface(captain); break;
                case World.LiveableArea.WATER: World.Instance.GotoWater(captain); break;
                default: break;
            }
        }

    }
}