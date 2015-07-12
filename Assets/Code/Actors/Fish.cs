using UnityEngine;
using BasicCommon;
using System.Collections;
using DG.Tweening;

namespace GoodFish
{
	public class Fish : Actor {

		private float swimDuration = 0.75f;
		private BasicTimer swimActionTimer;
		private float regainEnergyRate = 1f;
		private BasicTimer regainEnergyTimer;
		public int energy = 3;
		public int maxEnergy = 10;
		public bool isSwimming = false;

		private float eatSpeed = 0.25f;
		private float wiggleSpeed = 200f;

		void Awake()
		{
			swimActionTimer = new BasicTimer(swimDuration, false);
			swimActionTimer.Pause(true);
			regainEnergyTimer = new BasicTimer(regainEnergyRate, true);
		}

		protected override void Update()
		{
			float deltaTime = Time.deltaTime;
			if( swimActionTimer.Tick(deltaTime) )
			{
				isSwimming = false; 
			}
			if( !isSwimming && regainEnergyTimer.Tick(deltaTime) )
			{
				energy = Mathf.Min(energy + 1, maxEnergy);
			}
			for( int i=attached.Count-1; i>=0; --i )
			{
				if( !attached[i].IsAlive )
				{
					Consume(attached[i]);
				}
			}
			base.Update();
		}



		public override void FreeInput(float deltaTime, float horizontalAxis, float verticalAxis, bool btnA, bool btnX)
		{
			if( btnA && !isSwimming && energy > 0 )
			{
				energy--;
				isSwimming = true;
				swimActionTimer.Reset();
			}
			
			motor.Throttle(deltaTime, isSwimming ? 1f : 0f);

			motor.Rotate(deltaTime, horizontalAxis, verticalAxis);

			if( isSwimming && Mathf.Abs(horizontalAxis) < 0.01f)
			{
				AddWiggle(deltaTime);
			}
		}



		void AddWiggle(float deltaTime)
		{
			this.transform.Rotate(Vector3.up, wiggleSpeed*deltaTime);
			float yaw = this.transform.rotation.eulerAngles.y;
			while( yaw > 90 )
			{
				yaw -= 180f;
			}
			while( yaw < -90 )
			{
				yaw += 180f;
			}
			float maxYaw = 15f;
			if( yaw > maxYaw )
			{
				wiggleSpeed = -Mathf.Abs(wiggleSpeed);
			}
			if( yaw < -maxYaw )
			{
				wiggleSpeed = Mathf.Abs(wiggleSpeed);
			}
		}

		public override void Attach(Actor other, float offset)
		{

			other.eatenTimer.Duration = eatSpeed;
			other.eatenTimer.Reset();
			base.Attach(other, offset);
		}

		public override void Consume(Actor other)
		{
			int caughtWeight = other.weight;
			foreach(Actor child in other.attached)
			{
				caughtWeight += child.weight;
				SpawnBoss.Instance.Despawn(child);
			}
			health += caughtWeight;
			base.Consume(other);
		}
	}

}