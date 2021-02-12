﻿using System.Collections.Generic;
using System.Linq;
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

        /// <summary>
        /// Returns a random element from the given collection.
        /// </summary>
        /// <exception cref="System.InvalidOperationException">Thrown if the collection is empty.</exception>
        public static T RandomElement<T>(this IEnumerable<T> collection)
        {
            var count = collection.Count();

            if (count < 2)
                return collection.First();

            return collection.ElementAt(Random.Range(0, count));
        }
    }
}