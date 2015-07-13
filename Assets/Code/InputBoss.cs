using UnityEngine;
using System.Collections;

namespace GoodFish
{
	public class InputBoss : MonoBehaviour {

		public static InputBoss Instance;

		private bool hasButtonA = false;
		public bool ButtonA { get { return hasButtonA; } }

		private bool hasButtonX = false;
		public bool ButtonX { get { return hasButtonX; } }

		private float verticalAxis = 0f;
		public float VerticalAxis { get { return verticalAxis; } }

		private float horizontalAxis = 0f;
		public float HorizontalAxis { get { return horizontalAxis; } }


		private bool virtualButtonA;
		private bool virtualButtonX;
		private float virtualVerticalAxis;
		private float virtualHorizontalAxis;

	    public UserManager userMgr;
	    public AIManager aiMgr;

		// Use this for initialization
		void Start () {
			Instance = this;
		}
		
		// Update is called once per frame
		void Update () {

			float deltaTime = Time.deltaTime;
			hasButtonA = Input.GetKey(KeyCode.Space) || virtualButtonA  || Input.GetButton("Jump");
			hasButtonX = Input.GetKey(KeyCode.X) || virtualButtonX || Input.GetButton("Fire1");
			verticalAxis = Input.GetAxis("Vertical") + virtualVerticalAxis;
			horizontalAxis = Input.GetAxis("Horizontal") + virtualHorizontalAxis;


			if( userMgr.localUser != null )
			{
	        	if( userMgr.localUser.captain != null )
	            {
	                userMgr.localUser.captain.HandleInput(deltaTime, horizontalAxis, verticalAxis, hasButtonA, hasButtonX);
	            }    
	        }
		}

		public void VirtualSwimPress()
		{
			virtualButtonA = true;
		}
		public void VirtualSwimRelease()
		{
			virtualButtonA = false;
		}


		public void VirtualFlipPress()
		{
			virtualButtonX = true;
		}
		public void VirtualFlipRelease()
		{
			virtualButtonX = false;
		}

		public void VirtualHorizontalAxis(float val)
		{
			virtualHorizontalAxis = val;
		}

		public void VirtualVerticalAxis(float val)
		{
			virtualVerticalAxis = val;
		}
	}
}