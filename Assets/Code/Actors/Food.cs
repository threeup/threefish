using UnityEngine;
using BasicCommon;
using System.Collections;
using DG.Tweening;

namespace GoodFish
{
	public class Food : Actor {


		public Renderer foodRenderer; 
		Vector3 axis;
		Vector3 forward;
		float spinRate = 30f;
		float moveRate = 0.5f;

		public BasicTimer changeTimer = new BasicTimer(10f);

		void Start()
		{
			foodRenderer.material = new Material(foodRenderer.material);
			foodRenderer.material.color = new Color(Random.value, Random.value, Random.value, 1f);
			weight = Random.Range(1,4);
			switch(weight)
			{
				case 1: this.transform.localScale = 0.7f*Vector3.one; break;
				case 2: this.transform.localScale = 1.0f*Vector3.one; break;
				case 3: this.transform.localScale = 1.3f*Vector3.one; break;
			}
			this.transform.rotation = Random.rotation;
			ChangeDirections();
			changeTimer.TimeVal = Random.Range(0f, 10f);
		}

		protected override void Update()
		{
			if( IsAttached )
			{
				base.Update();
				return;
			}
			float deltaTime = Time.deltaTime;
			transform.Rotate(axis, spinRate*deltaTime);

			motor.SafeMove(transform.position + moveRate*forward*deltaTime);

			if( changeTimer.Tick(deltaTime) )
			{
				ChangeDirections();
			}
			base.Update();
		}

		void ChangeDirections()
		{
			Quaternion forwardQuat = Random.rotation;
			forward = forwardQuat*Vector3.forward;
			forward.x = 0f;
			Quaternion axisQuat = Random.rotation;
			axis = axisQuat*Vector3.up;
		}
	}

}