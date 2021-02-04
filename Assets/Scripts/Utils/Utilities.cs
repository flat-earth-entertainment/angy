using UnityEngine;

namespace Utils
{
    public static class Utilities
    {
        public static GameObject SafeFindWithThisTag(this string tag)
        {
            var foundObject = GameObject.FindWithTag(tag);

            if (foundObject == null)
            {
                Debug.LogError("Can't find a game object with tag: " + tag +
                               "! Make sure it exists before calling this method!");
            }

            return foundObject;
        }
    }
}