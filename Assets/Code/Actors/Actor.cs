using UnityEngine;
using BasicCommon;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

namespace GoodFish
{

	public class Actor : MonoBehaviour {

		public ActorType actorType;
		public int uid = -1;

		public int weight = 1;
		public int food = 10;
		public int maxFood = 10;
		public int health = 10;
		public int maxHealth = 10;
		public float lifeTime = 0f;

		public World.LiveableArea liveableArea = World.LiveableArea.SURFACE;

		public delegate void OnDeadDelegate(Actor actor);
		public OnDeadDelegate OnDead;
		public delegate void OnFacingChangeDelegate(bool val);
		public OnFacingChangeDelegate OnFacingChange;

		public Brain brain;
		public ActorMotor motor;
		public ActorBody body;
		public ActorHUD hud;


		private bool facePositive = true;
		
		public Transform attachTransform;
		public List<Actor> attached = new List<Actor>();
		public Actor attachParent = null;
		public float attachOffset = 0f;

		public BasicTimer hungryTimer = new BasicTimer(1f);
		public BasicTimer eatenTimer = new BasicTimer(0f);

		public Vector3 Forward { get { return motor.MotorForward; } }
		public bool LockInput { get { return motor.LockInput; } }
		public bool IsAttached { get { return attachParent != null; } }
		public bool IsAlive { get { return health > 0; } }

		
		public void Reset()
		{
			Detach();
			eatenTimer = new BasicTimer(0f);
			lifeTime = 0f;
			health = maxHealth;
		}

		// Update is called once per frame
		protected virtual void Update () 
		{
			float deltaTime = Time.deltaTime;
			lifeTime += deltaTime;
			CheckFacing();

			if( attached.Count > 0)
			{
				Vector3 forwardOffset = this.transform.forward;
				float forwardAngle = Mathf.Atan2(forwardOffset.y, forwardOffset.z);
				foreach(Actor child in attached)
				{
					float angle = Mathf.PI/2 - forwardAngle;
					if( Mathf.Abs(forwardAngle) > Mathf.PI/2 )
					{
						angle += child.attachOffset;
					}
					else
					{
						angle -= child.attachOffset;
					}
					Vector3 offsetPosition = new Vector3(0f, Mathf.Cos(angle), Mathf.Sin(angle));
					child.motor.SetPosition(attachTransform.position + offsetPosition*this.transform.localScale.x);
				}
			}

			if( maxFood > 0 && hungryTimer.Tick(deltaTime) )
			{
				if( food > 0 )
				{ 
					ModifyFood(-1);
				}
				else
				{
					ModifyHealth(-1);
				}
			}
			if( eatenTimer.Tick(deltaTime) )
			{
				ModifyHealth(-2);
			}
		}

		public virtual void HandleInput(float deltaTime, float horizontalAxis, float verticalAxis, bool btnA, bool btnX)
		{
			if( motor.LockInput )
			{
				return;
			}
			
			if( IsAttached )
			{
				AttachedInput(deltaTime, horizontalAxis, verticalAxis, btnA, btnX);
			}
			else
			{
				FreeInput(deltaTime, horizontalAxis, verticalAxis, btnA, btnX);
			}
		}

		public virtual void FreeInput(float deltaTime, float horizontalAxis, float verticalAxis, bool btnA, bool btnX)
		{

		}
		public virtual void AttachedInput(float deltaTime, float horizontalAxis, float verticalAxis, bool btnA, bool btnX)
		{

		}

		public virtual void HandleTrigger(Actor other, float offset)
		{
			if( other == null || !Utils.CanInteract(this, other))
			{
				return;
			}
			if( weight > other.weight )
			{
				Attach(other, offset);
			}
		}

		public virtual void Attach(Actor other, float offset)
		{
			if( attached.Contains(other) )
			{
				Debug.Log("double attach");
				return;
			}
			attached.Add(other);
			other.attachParent = this;
			other.attachOffset = offset;
			other.body.SetCollidable( !other.IsAttached );
		}

		public void Detach()
		{
			if( attachParent != null )
			{
				attachParent.attached.Remove(this);
				this.attachParent = null;
				this.body.SetCollidable( !this.IsAttached );
			}
		}

		public virtual void Consume(Actor other)
		{
			other.attached.Clear();
			SpawnBoss.Instance.Despawn(other);
		}

		public void AfterSpawn()
		{
			uid = World.Instance.AddActor(this);
			Reset();
		}

		public void AfterDespawn()
		{
			World.Instance.RemoveActor(this);
			if( brain != null )
			{
				brain.SetAI(false);
			}
			if( OnDead != null )
			{
				OnDead(this);
			}
		}

		public void ModifyFood(int val)
		{
			food = Mathf.Min(maxFood, food + val);
			if( hud != null )
			{
				hud.SetFood( (int)Mathf.Round((float)food/maxFood * 10) );
			}
		}

		public void ModifyHealth(int val)
		{
			health = Mathf.Min(maxHealth, health + val);
			if( hud != null )
			{
				hud.SetHealth( (int)Mathf.Round((float)health/maxHealth * 10) );
			}
		}


		public void CheckFacing()
		{
			bool shouldFacePositive = motor.MotorForward.z > 0;
			if( facePositive == shouldFacePositive )
			{
				return;
			}
			facePositive = shouldFacePositive;
			if( OnFacingChange != null )
			{
				OnFacingChange(facePositive);
			}
		}

		
	}

}