using UnityEngine;
using BasicCommon;
using System.Collections;

namespace GoodFish
{
	public class Boat : Actor {

		public Hook hook;
		public bool hasHook = false; 
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
			hasHook = hook != null && hook.IsAlive;
			motor.Throttle(deltaTime, horizontalAxis * (hasHook ? 0.5f : 1f));
			if( verticalAxis < -0.01f && !hasHook && waitHookTimer.Paused )
			{
				hook = SpawnBoss.Instance.Spawn(ActorType.HOOK) as Hook;
				hook.motor.SetPosition(this.transform.position.x, this.transform.position.y - 3f, this.transform.position.z);
			}	
			if( hasHook )
			{
				hook.motor.Throttle(deltaTime, verticalAxis * (btnA ? 1f : 0.5f) );
				hook.motor.SetPosition(this.transform.position.x, null, this.transform.position.z);
			}
		}


		public override void Attach(Actor other, float offset)
		{
			Consume(other);
		}

		public override void Consume(Actor other)
		{
			if( other != hook )
			{
				return;
			}
			int caughtWeight = 0;
			for(int i=other.attached.Count-1; i>=0; --i)
			{
				Actor child = other.attached[i];
				caughtWeight += child.weight;
				base.Consume(child);
			}
			if( caughtWeight > 0 || other.lifeTime > 2f )
			{
				waitHookTimer.Reset();
				ModifyFood(caughtWeight);
				hook = null;
				base.Consume(other);
			}
			
		}
	}

}