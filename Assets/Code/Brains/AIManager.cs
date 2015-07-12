using UnityEngine;
using System.Collections.Generic;


namespace GoodFish
{
    [System.Serializable]
    public class AIManager : MonoBehaviour
    {
        public static AIManager Instance;
    	public List<Brain> controlledBrains;

        void Awake()
        {
            Instance = this;
        }

        public void Update()
        {
            float deltaTime = Time.deltaTime;
            foreach(Brain brain in controlledBrains)
            {
                brain.MinorTick(deltaTime);
                brain.Tick(deltaTime);
            }
        }


        public void Register(Brain brain, bool val)
        {
            if( val )
            {
                if( !controlledBrains.Contains(brain ))
                {
                    controlledBrains.Add(brain);
                }
            }
            else
            {
                if( controlledBrains.Contains(brain ))
                {
                    controlledBrains.Remove(brain);
                }
            }
        }

    }
}