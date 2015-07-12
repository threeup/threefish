using UnityEngine;
using System.Collections.Generic;


namespace GoodFish
{
    [System.Serializable]
    public class World : MonoBehaviour
    {
        public static World Instance;

        public enum LiveableArea
		{
			SURFACE,
			HOOK,
			WATER,
		}

        public Bounds waterBounds;
        public Bounds hookBounds;
        public Bounds surfaceBounds;

		public UserManager userMgr;
		public GameControls controls;
		public BoatCameraControl boatCamera;
		public FishCameraControl fishCamera;
        public float SurfaceY { get { return surfaceBounds.min.y; } }


        public List<Actor> allActors = new List<Actor>();
        private int nextActorUID = 100;

        void Awake()
        {
            Instance = this;

        }

        public void StartGame()
        {
        	controls.StartGame();
        	userMgr.StartGame();
        }


        public Bounds GetLiveableArea(LiveableArea areaType)
        {
        	switch(areaType)
        	{
        		case LiveableArea.SURFACE: return surfaceBounds;
        		case LiveableArea.HOOK: return hookBounds;
        		case LiveableArea.WATER: return waterBounds;
        	}
        	return new Bounds(Vector3.zero, Vector3.zero);
        }

        public void GotoSurface(Actor focalActor)
        {
        	controls.SetModeSurface(true);
        	boatCamera.SetActiveFocus(true, focalActor);
        	fishCamera.SetActiveFocus(false, focalActor);
        }

        public void GotoWater(Actor focalActor)
        {
        	controls.SetModeSurface(false);	
        	boatCamera.SetActiveFocus(false, focalActor);
        	fishCamera.SetActiveFocus(true, focalActor);
        }

        public int AddActor(Actor actor)
        {
        	allActors.Add(actor);
        	return nextActorUID++;
        }

        public void RemoveActor(Actor actor)
        {
        	allActors.Remove(actor);
        }

    }
}