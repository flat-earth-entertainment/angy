using System.Collections.Generic;
using Ball.Objectives;
using Config;
using UnityEngine;
using Utils;

namespace MeshPhysics
{
    public class MeshCombiner : MonoBehaviour
    {
        private static readonly List<Transform> ObjectsToRemove = new List<Transform>();
        private static readonly List<Transform> ObjectsToAdd = new List<Transform>();
        private readonly List<CombineInstance> _combineInstances = new List<CombineInstance>();
        private readonly List<Transform> _meshObjects = new List<Transform>();
        private MeshCollider _meshCollider;
        private MeshFilter _meshFilter;

        private Transform _obstacleParent;

        private bool _shouldBake;
        public bool Baked { get; private set; }

        private void Awake()
        {
            _obstacleParent = GameConfig.Instance.Tags.MapPhysicalLayoutTag.SafeFindWithThisTag().transform;

            _meshFilter = transform.GetComponent<MeshFilter>();
            _meshCollider = transform.GetComponent<MeshCollider>();
            ObjectsToRemove.Clear();

            foreach (var child in _obstacleParent.GetComponentsInChildren<Collider>())
            {
                if (child.isTrigger)
                    continue;
                _meshObjects.Add(child.transform);
                child.gameObject.layer = LayerMask.NameToLayer("IgnoredMap");
            }

            RebakeMesh();
        }

        private void Update()
        {
            if (ObjectsToRemove.Count > 0)
            {
                foreach (var obj in ObjectsToRemove)
                {
                    _meshObjects.Remove(obj);
                }

                ObjectsToRemove.Clear();
                _shouldBake = true;
            }

            if (ObjectsToAdd.Count > 0)
            {
                foreach (var obj in ObjectsToAdd)
                {
                    _meshObjects.Add(obj);
                }

                ObjectsToAdd.Clear();
                _shouldBake = true;
            }

            if (_shouldBake)
            {
                RebakeMesh();
                _shouldBake = false;
            }
        }

        private void OnEnable()
        {
            GoodNeutralMushroom.BecameHole += OnSomethingBecameHole;
        }

        private void OnDisable()
        {
            GoodNeutralMushroom.BecameHole -= OnSomethingBecameHole;
        }


        public static void AddObject(Transform transform)
        {
            ObjectsToAdd.Add(transform);
        }

        public static void RemoveObject(Transform transform)
        {
            ObjectsToRemove.Add(transform);
        }

        private void RebakeMesh()
        {
            Baked = false;

            _combineInstances.Clear();

            foreach (var child in _meshObjects)
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