using UnityEngine;
using System.Collections.Generic;


namespace GoodFish
{
    public enum ActorType
    {
        BOAT,
        HOOK,
        FISH_SMALL,
        FISH_MEDIUM,
        FISH_LARGE,
        FISH_GIANT,
        FOOD,
    }

    public class Utils
    {

        public static bool IsFish(ActorType ty)
        {
            switch(ty)
            {
                case ActorType.FISH_SMALL: 
                case ActorType.FISH_MEDIUM: 
                case ActorType.FISH_LARGE: 
                case ActorType.FISH_GIANT:
                    return true;
            }
            return false;
        }
        public static  bool IsBoat(ActorType ty)
        {
            return ty == ActorType.BOAT;
        }
        public static bool IsHook(ActorType ty)
        {
            return ty == ActorType.HOOK;
        }
        public static  bool IsFood(ActorType ty)
        {
            return ty == ActorType.FOOD;
        }
        public static  bool CanInteract(Actor a, Actor b)
        {
            return CanInteract(a.actorType, b.actorType);   
        }
        public static  bool CanInteract(ActorType a, ActorType b)
        {
            if( IsFish(a) )
            {
                return IsFish(b) || IsHook(b) || IsFood(b);
            }
            if( IsHook(a) )
            {
                return IsFish(b) || IsBoat(b);
            }
            if( IsFood(a) )
            {
                return IsFish(b);
            }
            if( IsBoat(a) )
            {
                return IsHook(b);
            }
            return false;
        }

    }
}