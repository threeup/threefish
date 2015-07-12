using UnityEngine;
using System.Collections.Generic;


namespace GoodFish
{
    [System.Serializable]
    public class BrainRecord
    {
        public Vector3 position;
    	public Actor target;
        public float age;
        public float range;
        public int desire;

        public BrainRecord(Actor actor)
        {
            target = actor;
        }
    }

    public struct DirectionalData
    {
        public int desireSum;
        public BrainRecord bestRecord;

        public DirectionalData(BrainRecord record)
        {
            bestRecord = record;
            desireSum = record == null ? 0 : record.desire;
        }
    }


}