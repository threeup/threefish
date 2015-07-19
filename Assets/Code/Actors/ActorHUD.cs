using UnityEngine;
using System.Collections.Generic;
using GumboLib;
using DG.Tweening;


namespace GoodFish
{
    [System.Serializable]
    public class HUDMachine : BasicMachine<HUDState> 
    {
    }

    [System.Serializable]
    public enum HUDState
    {
        NONE,
        SINGLE,
        DOUBLE,
    }

    [System.Serializable]
    public class ActorHUD : MonoBehaviour
    {

        private struct HUDSlotData
        {
            public int slot;
            public float ttl;
            public Texture tex;
        }

        public Actor actor;

        public GameObject imageAlpha;
        private Material imageAlphaMaterial;
        public GameObject imageBravo;
        private Material imageBravoMaterial;
        public GameObject imageCharlie;
        private Material imageCharlieMaterial;
        public GameObject bubble;
        public Renderer[] bubbleRenderers;
        private Material bubbleMaterial;

        public Transform facingLeft;
        public Transform facingRight;

        private List<HUDSlotData> slotQueue = new List<HUDSlotData>();
        private bool slotDirty = false;

        public HUDMachine machine;

        public float defaultTTL = 1f;

        void Awake()
        {
            

            Renderer imageAlphaRenderer = imageAlpha.GetComponent<Renderer>();
            imageAlphaMaterial = new Material(imageAlphaRenderer.material);
            imageAlphaRenderer.material = imageAlphaMaterial;

            Renderer imageBravoRenderer = imageBravo.GetComponent<Renderer>();
            imageBravoMaterial = new Material(imageBravoRenderer.material);
            imageBravoRenderer.material = imageBravoMaterial;

            Renderer imageCharlieRenderer = imageCharlie.GetComponent<Renderer>();
            imageCharlieMaterial = new Material(imageCharlieRenderer.material);
            imageCharlieRenderer.material = imageCharlieMaterial;

            
            bubbleMaterial = new Material(bubbleRenderers[0].material);
            for(int i=0; i<bubbleRenderers.Length; ++i)
            {
                bubbleRenderers[i].material = bubbleMaterial;
            }
              
            imageAlphaMaterial.DOFade(0f, 0.2f);
            imageBravoMaterial.DOFade(0f, 0.2f);
            imageCharlieMaterial.DOFade(0f, 0.2f);
            
            machine = new HUDMachine();
            machine.Initialize(typeof(HUDState)); 
            machine.AddEnterListener((int)HUDState.NONE, OnNoneEnter);
            machine.AddExitListener((int)HUDState.NONE, OnNoneExit);
            machine.AddEnterListener((int)HUDState.SINGLE, OnSingleEnter);
            machine.AddSameListener((int)HUDState.SINGLE, OnSingleSame);
            machine.AddExitListener((int)HUDState.SINGLE, OnSingleExit);
            machine.AddEnterListener((int)HUDState.DOUBLE, OnDoubleEnter);
            machine.AddSameListener((int)HUDState.DOUBLE, OnDoubleSame);
            machine.AddExitListener((int)HUDState.DOUBLE, OnDoubleExit);

            machine.SetState(HUDState.NONE);
            
        }

        public void Init()
        {
            actor.OnFacingChange += HandleFacingChange; 
            SetHealth(10);
            SetFood(10);
        }

        

        void Update()
        {
            float deltaTime = Time.deltaTime;
            for(int i=slotQueue.Count-1; i>=0; --i)
            {
                HUDSlotData slotData = slotQueue[i];
                slotData.ttl -= deltaTime;
                if( slotData.ttl < 0f )
                {
                    slotQueue.RemoveAt(i);
                    slotDirty = true;
                }
                else
                {
                    slotQueue[i] = slotData;
                }
            }   
            if( slotDirty )
            {
                ProcessSlots();
            }
        }

        void ProcessSlots()
        {
            if( slotQueue.Count >= 2 )
            {
                machine.SetState(HUDState.DOUBLE);
            }
            else if( slotQueue.Count == 1)
            {
                machine.SetState(HUDState.SINGLE);
            }
            else
            {
                machine.SetState(HUDState.NONE);
            }
        }

        public void OnNoneEnter()
        {
            bubbleMaterial.DOFade(0f, 0.2f);
            //temp
            for(int i=0; i<bubbleRenderers.Length; ++i)
            {
                bubbleRenderers[i].enabled = false;
            }
        }

        public void OnNoneExit()
        {
            bubbleMaterial.DOFade(0.8f, 0.2f);
            //temp
            for(int i=0; i<bubbleRenderers.Length; ++i)
            {
                bubbleRenderers[i].enabled = true;
            }
        }

        public void OnSingleEnter()
        {
            imageAlphaMaterial.mainTexture = slotQueue[0].tex;
            imageAlphaMaterial.DOFade(1f, 0.2f);
        }

        public void OnSingleSame()
        {
            imageAlphaMaterial.mainTexture = slotQueue[0].tex;
        }

        public void OnSingleExit()
        {
            imageAlphaMaterial.DOFade(0f, 0.2f);
        }

        public void OnDoubleEnter()
        {
            imageBravoMaterial.mainTexture = slotQueue[0].tex;
            imageCharlieMaterial.mainTexture = slotQueue[1].tex;
            imageBravoMaterial.DOFade(1f, 0.2f);
            imageCharlieMaterial.DOFade(1f, 0.2f);
        }

        public void OnDoubleSame()
        {
            imageBravoMaterial.mainTexture = slotQueue[0].tex;
            imageCharlieMaterial.mainTexture = slotQueue[1].tex;
        }

        public void OnDoubleExit()
        {
            imageBravoMaterial.DOFade(0f, 0.2f);
            imageCharlieMaterial.DOFade(0f, 0.2f);
        }


        void AddTexture(Texture newtex, int newslot)
        {
            slotQueue.RemoveAll(x=>x.slot == newslot);
            slotQueue.Insert(0, new HUDSlotData(){ slot = newslot, tex = newtex, ttl = defaultTTL});
            slotDirty = true;

        }

        public void SetHealth(int val)
        {
            SpriteBoss sb = SpriteBoss.Instance;
            int slot = 0;
            Texture tex = null;
            switch(val)
            {
                case 0:
                    tex = sb.NoHealth;
                    break;
                case 1:
                case 2:
                case 3:
                    tex = sb.LowHealth;
                    break;
                case 4:
                case 5:
                case 6:
                    tex = sb.MedHealth;
                    break;
                case 7:
                case 8:
                case 9:
                default:
                    tex = sb.FullHealth;
                    break;
            }
            AddTexture( tex, slot );
        }

        public void SetFood(int val)
        {
            SpriteBoss sb = SpriteBoss.Instance;
            int slot = 1;
            Texture tex = null;
            switch(val)
            {
                case 0:
                    tex = sb.NoFood;
                    break;
                case 1:
                case 2:
                case 3:
                    tex = sb.LowFood;
                    break;
                case 4:
                case 5:
                case 6:
                    tex = sb.MedFood; 
                    break;
                case 7:
                case 8:
                case 9:
                default:
                    tex = sb.FullFood;
                    break;
            }
            AddTexture( tex, slot );
        }

        public void SetDesire(int val)
        {
            SpriteBoss sb = SpriteBoss.Instance;
            int slot = 2;
            Texture tex = null;
            switch(val)
            {
                case 0:
                    tex = sb.GoFood;
                    break;
                case 1:
                    tex = sb.GoFish;
                    break;
                case 2:
                    tex = sb.GoFear;
                    break;
                case 3:
                    tex = sb.GoTired;
                    break;
                default:
                case 4:
                    tex = sb.GoHappy;
                    break;
            }
            AddTexture( tex, slot );
        }


        public void HandleFacingChange(bool val)
        {
            //bubble.transform.Rotate(new Vector3(0f, 180f, 0f)); 
            imageAlpha.transform.SetParent(val ? facingRight : facingLeft, false);
            imageBravo.transform.SetParent(val ? facingRight : facingLeft, false);
            imageBravo.transform.localPosition = new Vector3(-0.5f,0,0);
            imageCharlie.transform.SetParent(val ? facingRight : facingLeft, false);
            imageCharlie.transform.localPosition = new Vector3(0.5f,0,0);
        }
    }
}