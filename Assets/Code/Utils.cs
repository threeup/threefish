using UnityEngine;
using System.Collections.Generic;


namespace GoodFish
{
    public enum HudType
    {
        HEALTH,
        FOOD,
        SCORE,
        DESIRE,
    }


    public enum ActorType
    {
        BOAT,
        HOOK,
        FISH_SMALL,
        FISH_MEDIUM,
        FISH_LARGE,
        FISH_SHARK,
        FOOD,
    }

    public enum Direction
    {
        NORTH,
        NORTHEAST,
        EAST,
        SOUTHEAST,
        SOUTH,
        SOUTHWEST,
        WEST,
        NORTHWEST,
        CENTER
    }

    public class Utils
    {
        public static float HalfRootTwo = Mathf.Sqrt(2f)/2f;

        public static Direction GetDirection(Actor self, Actor other)
        {
            Vector3 diff = other.transform.position - self.transform.position;
            float angle = Mathf.Atan2(diff.z, diff.y)*Mathf.Rad2Deg;
            if( angle < 0 ) 
            {
                angle += 360;
            }
            if( angle < 30f) { return Direction.NORTH; }
            if( angle < 60f) { return Direction.NORTHEAST; }
            if( angle < 120f) { return Direction.EAST; }
            if( angle < 150f) { return Direction.SOUTHEAST; }
            if( angle < 210f) { return Direction.SOUTH; }
            if( angle < 240f) { return Direction.SOUTHWEST; }
            if( angle < 300f) { return Direction.WEST; }
            if( angle < 330f) { return Direction.NORTHWEST; }
            if( angle < 360f) { return Direction.NORTH; }
            return Direction.CENTER;

        }

        public static Vector3 GetVector(int dir)
        {
            Direction d = (Direction)dir;
            return GetVector(d);
        }

        public static Vector3 GetVector(Direction dir)
        {
            switch(dir)
            {
                case Direction.NORTH: 
                    return new Vector3(0,1,0);
                case Direction.NORTHEAST: 
                    return new Vector3(0,HalfRootTwo,HalfRootTwo);
                case Direction.EAST: 
                    return new Vector3(0,0,1);
                case Direction.SOUTHEAST: 
                    return new Vector3(0,-HalfRootTwo,HalfRootTwo);
                case Direction.SOUTH: 
                    return new Vector3(0,-1,0);
                case Direction.SOUTHWEST: 
                    return new Vector3(0,-HalfRootTwo,-HalfRootTwo);
                case Direction.WEST: 
                    return new Vector3(0,0,-1);
                case Direction.NORTHWEST: 
                    return new Vector3(0,HalfRootTwo,-HalfRootTwo);
                default:
                case Direction.CENTER: 
                    break;
            }
            return Vector3.zero;
        }

        public static float DistanceToBounds(Direction dir, Vector3 pos, Bounds liveable)
        {
            switch(dir)
            {
                case Direction.NORTH:
                    return liveable.max.y - pos.y;
                case Direction.NORTHEAST:
                    return Mathf.Min(liveable.max.y - pos.y, liveable.max.z - pos.z);
                case Direction.EAST:
                    return liveable.max.z - pos.z;
                case Direction.SOUTHEAST:
                    return Mathf.Min(pos.y - liveable.min.y, liveable.max.z - pos.z);
                case Direction.SOUTH:
                    return pos.y - liveable.min.y;
                case Direction.SOUTHWEST:
                    return Mathf.Min(pos.y - liveable.min.y, pos.z - liveable.min.z);
                case Direction.WEST:
                    return pos.z - liveable.min.z;
                case Direction.NORTHWEST:
                    return Mathf.Min(liveable.max.y - pos.y, pos.z - liveable.min.z);
                default:
                case Direction.CENTER:
                    break;
            }
            return 99999f;
        }

        public static bool IsFish(ActorType ty)
        {
            switch(ty)
            {
                case ActorType.FISH_SMALL: 
                case ActorType.FISH_MEDIUM: 
                case ActorType.FISH_LARGE: 
                case ActorType.FISH_SHARK:
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