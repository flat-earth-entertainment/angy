using System.Collections.Generic;
using Config;
using UnityEngine;
using Utils;

namespace MeshPhysics
{
    public class MeshCombiner : MonoBehaviour
    {
        public bool Baked { get; private set; }

        private static readonly List<Transform> _objectsToRemove = new List<Transform>();
        private static readonly List<Transform> _objectsToAdd = new List<Transform>();
        private readonly List<Transform> _meshObjects = new List<Transform>();
        private readonly List<CombineInstance> _combineInstances = new List<CombineInstance>();

        private Transform _obstacleParent;
        private MeshCollider _meshCollider;
        private MeshFilter _meshFilter;


        public static void AddObject(Transform transform)
        {
            _objectsToAdd.Add(transform);
        }

        public static void RemoveObject(Transform transform)
        {
            _objectsToRemove.Add(transform);
        }

        private void Awake()
        {
            _obstacleParent = GameConfig.Instance.Tags.MapPhysicalLayoutTag.SafeFindWithThisTag().transform;

            _meshFilter = transform.GetComponent<MeshFilter>();
            _meshCollider = transform.GetComponent<MeshCollider>();
            _objectsToRemove.Clear();

            foreach (Collider child in _obstacleParent.GetComponentsInChildren<Collider>())
            {
                if(child.isTrigger)
                    continue;
                _meshObjects.Add(child.transform);
                child.gameObject.layer = LayerMask.NameToLayer("IgnoredMap");
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

        private bool _shouldBake;

        private void Update()
        {
            if (_objectsToRemove.Count > 0)
            {
                foreach (var obj in _objectsToRemove)
                {
                    _meshObjects.Remove(obj);
                }

                _objectsToRemove.Clear();
                _shouldBake = true;
            }

            if (_objectsToAdd.Count > 0)
            {
                foreach (var obj in _objectsToAdd)
                {
                    _meshObjects.Add(obj);
                }

                _objectsToAdd.Clear();
                _shouldBake = true;
            }

            if (_shouldBake)
            {
                RebakeMesh();
                _shouldBake = false;
            }
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