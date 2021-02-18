using UnityEngine;

namespace Utils
{
    public static class Constants
    {
        public const int ExpectedWidth = 64;
        public const int ExpectedHeight = ExpectedWidth;

        
    }

    public static class HelperFunctions
    {
        public static Vector2 NormalizeToSkin(Vector2 vector2)
        {
            return new Vector2(
                vector2.x / Constants.ExpectedWidth,
                vector2.y / Constants.ExpectedHeight
            );
        }
    }
}
