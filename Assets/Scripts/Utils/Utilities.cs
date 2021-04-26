using System.Collections.Generic;
using System.Linq;
using Config;
using ExitGames.Client.Photon;
using Logic;
using UnityEngine;

namespace Utils
{
    public static class Utilities
    {
        public static void EmitExplosionAtPosition(Vector3 position, float duration = 1f)
        {
            Object.Destroy(
                Object.Instantiate(GameConfig.Instance.HitStop.ImpactParticle, position, Quaternion.identity),
                duration);
        }

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

        /// <summary>
        ///     Returns a random element from the given collection.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if the collection is empty.</exception>
        public static T RandomElement<T>(this IEnumerable<T> collection)
        {
            var count = collection.Count();

            return count < 2 ? collection.First() : collection.ElementAt(Random.Range(0, count));
        }

        /// <summary>
        ///     Changes the string color by surrounding the given string with HTML color tag of supplied color.
        /// </summary>
        /// <param name="color">Color to apply to the string.</param>
        public static string Color(this string stringToColor, Color color)
        {
            return $"<color=#{ColorUtility.ToHtmlStringRGBA(color)}>{stringToColor}</color>";
        }

        public static byte ToByte(this GameEvent gameEvent)
        {
            return (byte) gameEvent;
        }

        public static T Read<T>(this EventData eventData, int itemIndex = 0)
        {
            return (T) (eventData.CustomData as object[])?[itemIndex];
        }
    }
}