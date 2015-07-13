using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace GoodFish
{
	public class SpawnBoss : MonoBehaviour {


		public static SpawnBoss Instance;
		public List<GameObject> prototypes = new List<GameObject>();

		public Dictionary<ActorType, Stack<GameObject>> poolDictionary = new Dictionary<ActorType, Stack<GameObject>>();

		void Awake()
		{
			Instance = this;
			ActorType[] values = (ActorType[])System.Enum.GetValues(typeof(ActorType));
			foreach(ActorType val in values)
			{
				poolDictionary[val] = new Stack<GameObject>();
			}
			foreach(GameObject prototype in prototypes)
			{
				prototype.SetActive(false);
				prototype.transform.parent = this.transform;
				Actor actor = prototype.GetComponent<Actor>();
				ActorType actorType = actor.actorType;
				
				int count = 2;
				switch(actorType)
				{
					case ActorType.FISH_SMALL: 
					case ActorType.FISH_MEDIUM: 
					case ActorType.FISH_LARGE: 
						count = 30;
						break;
					case ActorType.FOOD:
						count = 100;
						break;
					case ActorType.FISH_SHARK:
						count = 3;
						break;
					case ActorType.BOAT:
					case ActorType.HOOK:
					default:
						count = 2;
						break;
				}
				for(int i=0; i<count; ++i)
				{
					GameObject entry = GameObject.Instantiate(actor.gameObject);
					entry.name = prototype.name.Replace("Proto","") + "-" + i.ToString();
					entry.transform.parent = this.transform;
					poolDictionary[actorType].Push(entry);
				}
			}

			
		}


		public Actor Spawn(Actor prototype)
		{
			return Spawn(prototype.actorType);
		}
		public Actor Spawn(ActorType actorType)
		{
			Stack<GameObject> pool = poolDictionary[actorType];
			if( pool.Count == 0 )
			{
				Debug.Log("empty pool"+actorType);
				return null;
			} 
			GameObject spawn = pool.Pop();
			spawn.transform.parent = null;
			Actor actor = spawn.GetComponent<Actor>();
			Bounds bounds = World.Instance.GetLiveableArea(actor.liveableArea);
			float xPos = bounds.center.x;
			float yPos = Random.Range(bounds.min.y, bounds.max.y);
			float zPos = Random.Range(bounds.min.z, bounds.max.z);
			Vector3 startPos = new Vector3(xPos, yPos, zPos);
			Vector3 midPos = startPos;
			midPos.z = 0;
			actor.transform.position = startPos;
			string actorTypeName = actorType.ToString();
			if( actorTypeName.StartsWith("FISH"))
			{
				actor.transform.LookAt(midPos);
			}
			spawn.SetActive(true);
			
			actor.AfterSpawn();

			return actor;
		}

		public void Despawn(Actor despawned)
		{
			Stack<GameObject> pool = poolDictionary[despawned.actorType];
			despawned.gameObject.SetActive(false);
			despawned.transform.parent = this.transform;
			pool.Push(despawned.gameObject);
			despawned.AfterDespawn();
		}
	}
}