using System;
using UnityEngine;

namespace Player
{
    [Serializable]
    public class AngyValues
    {
        [field: SerializeField]
        public int MinAngy { get; private set; }

        [field: SerializeField]
        public int MaxAngy { get; private set; }

        [field: SerializeField]
        public int HitBadObject { get; private set; }

        [field: SerializeField]
        public int FellOutOfTheMap { get; private set; }

        [field: SerializeField]
        public int AfterFellOutOfTheMapAndReachedMaxAngy { get; private set; }

        [field: SerializeField]
        public int EndedTurn { get; private set; }
    }
}