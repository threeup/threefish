using UnityEngine;
using BasicCommon;
using System.Collections;

namespace GoodFish
{
	public class Boat : Actor {

		public Hook hook;
		private BasicTimer waitHookTimer = new BasicTimer(0.3f, false);

		public UnityEngine.UI.Toggle.ToggleEvent onToggleHook;


		protected override void Update () 
		{
			float deltaTime = Time.deltaTime;
			waitHookTimer.Tick(deltaTime);
			base.Update();
		}

		public override void FreeInput(float deltaTime, float horizontalAxis, float verticalAxis, bool btnA, bool btnX)
		{
			motor.Throttle(deltaTime, horizontalAxis);
			bool hasHook = hook != null && hook.IsAlive;
			if( verticalAxis < -0.01f && !hasHook && waitHookTimer.Paused )
			{
				hook = SpawnBoss.Instance.Spawn(ActorType.HOOK) as Hook;
				hook.motor.SetPosition(this.transform.position.x, this.transform.position.y - 3f, this.transform.position.z);
			}	
			if( hasHook )
			{
				hook.motor.Throttle(deltaTime, verticalAxis);
				hook.motor.SetPosition(this.transform.position.x, null, this.transform.position.z);
			}
		}


		public override void Attach(Actor other, float offset)
		{
			Consume(other);
		}

		public override void Consume(Actor other)
		{
			int caughtWeight = 0;
			foreach(Actor child in other.attached)
			{
				caughtWeight += child.weight;
				SpawnBoss.Instance.Despawn(child);
			}
			if( caughtWeight > 0 || other.lifeTime > 2f )
			{
				other.attached.Clear();
				SpawnBoss.Instance.Despawn(other);
				waitHookTimer.Reset();
				health += caughtWeight;
				Debug.Log("HOOK "+other+" ="+caughtWeight);
			}
			
		}
	}

}