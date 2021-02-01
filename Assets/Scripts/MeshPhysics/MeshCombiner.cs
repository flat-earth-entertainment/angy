﻿using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MeshPhysics
{
    public class MeshCombiner : MonoBehaviour
    {
        public bool Baked { get; private set; }

        [SerializeField]
        private GameObject obstacleParent;

        private readonly List<Transform> _meshObjects = new List<Transform>();
        private readonly List<CombineInstance> _combineInstances = new List<CombineInstance>();

        private MeshCollider _meshCollider;
        private MeshFilter _meshFilter;

        private void Awake()
        {
            _meshFilter = transform.GetComponent<MeshFilter>();
            _meshCollider = transform.GetComponent<MeshCollider>();

            foreach (Transform child in obstacleParent.transform)
            {
                if (child.TryGetComponent(out MeshFilter meshFilter))
                {
                    _meshObjects.Add(child);
                    child.gameObject.layer = LayerMask.NameToLayer("IgnoredMap");
                }
            }

            RebakeMesh();
        }

        private void OnEnable()
        {
            GoodNeutralMushroom.BecameHole += OnSomethingBecameHole;
        }

        private void OnDisable()
        {
            GoodNeutralMushroom.BecameHole -= OnSomethingBecameHole;
        }

        private void RebakeMesh()
        {
            Baked = false;

            _combineInstances.Clear();

            foreach (Transform child in _meshObjects)
            {
                if (child.TryGetComponent(out MeshFilter meshFilter))
                {
                    var instance = new CombineInstance
                        {mesh = meshFilter.sharedMesh, transform = meshFilter.transform.localToWorldMatrix};

                    _combineInstances.Add(instance);
                }
            }

            var combinedMesh = _meshFilter.mesh = new Mesh();
            _meshFilter.mesh.CombineMeshes(_combineInstances.ToArray());

            _meshCollider.sharedMesh = null;
            _meshCollider.sharedMesh = combinedMesh;

            Baked = true;
        }

        private void OnSomethingBecameHole(GameObject obj)
        {
            _meshObjects.Remove(obj.transform);
            RebakeMesh();
        }
    }
}