using UnityEngine;
using System.Collections.Generic;


namespace GoodFish
{
    [System.Serializable]
    public class ActorHUD : MonoBehaviour
    {
        public Actor actor;

        public GameObject health;
        private Material healthMaterial;
        public GameObject food;
        private Material foodMaterial;
        public GameObject score;
        private Material scoreMaterial;
        public GameObject desire;
    	private Material desireMaterial;


        void Awake()
        {
            Renderer healthRenderer = health.GetComponent<Renderer>();
            healthMaterial = new Material(healthRenderer.material);
            healthRenderer.material = healthMaterial;

            Renderer foodRenderer = food.GetComponent<Renderer>();
            foodMaterial = new Material(foodRenderer.material);
            foodRenderer.material = foodMaterial;

            Renderer desireRenderer = desire.GetComponent<Renderer>();
            desireMaterial = new Material(desireRenderer.material);
            desireRenderer.material = desireMaterial;

            actor.OnFacingChange += HandleFacingChange;    
        }

        void Start()
        {
            SetHealth(10);
            SetFood(10);
        }

        public void SetHealth(int val)
        {
            SpriteBoss sb = SpriteBoss.Instance;
            switch(val)
            {
                case 0:
                    healthMaterial.mainTexture = sb.NoHealth;
                    break;
                case 1:
                case 2:
                case 3:
                    healthMaterial.mainTexture = sb.LowHealth;
                    break;
                case 4:
                case 5:
                case 6:
                    healthMaterial.mainTexture = sb.MedHealth;
                    break;
                case 7:
                case 8:
                case 9:
                default:
                    healthMaterial.mainTexture = sb.FullHealth;
                    break;
            }
        }

        public void SetFood(int val)
        {
            SpriteBoss sb = SpriteBoss.Instance;
            switch(val)
            {
                case 0:
                    foodMaterial.mainTexture = sb.NoFood;
                    break;
                case 1:
                case 2:
                case 3:
                    foodMaterial.mainTexture = sb.LowFood;
                    break;
                case 4:
                case 5:
                case 6:
                    foodMaterial.mainTexture = sb.MedFood;
                    break;
                case 7:
                case 8:
                case 9:
                default:
                    foodMaterial.mainTexture = sb.FullFood;
                    break;
            }
        }

        public void SetDesire(int val)
        {
            SpriteBoss sb = SpriteBoss.Instance;
            switch(val)
            {
                case 0:
                    desireMaterial.mainTexture = sb.GoFood;
                    break;
                case 1:
                    desireMaterial.mainTexture = sb.GoFish;
                    break;
                case 2:
                    desireMaterial.mainTexture = sb.GoFear;
                    break;
                case 3:
                    desireMaterial.mainTexture = sb.GoTired;
                    break;
                default:
                case 4:
                    desireMaterial.mainTexture = sb.GoHappy;
                    break;
            }
        }


        public void HandleFacingChange(bool val)
        {
            health.transform.Rotate(new Vector3(0f, 180f, 0f)); 
            food.transform.Rotate(new Vector3(0f, 180f, 0f)); 
            score.transform.Rotate(new Vector3(0f, 180f, 0f)); 
            desire.transform.Rotate(new Vector3(0f, 180f, 0f)); 
        }
    }
}