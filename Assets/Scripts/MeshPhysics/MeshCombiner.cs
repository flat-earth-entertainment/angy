using System.Collections.Generic;
using UnityEngine;

namespace MeshPhysics
{
    public class MeshCombiner : MonoBehaviour
    {
        public bool Baked { get; private set; }
        
        [SerializeField]
        private GameObject obstacleParent;

        private void Awake()
        {
            var combineInstance = new List<CombineInstance>();

            foreach (Transform child in obstacleParent.transform)
            {
                if (child.TryGetComponent(out MeshFilter meshFilter))
                {
                    var instance = new CombineInstance
                        {mesh = meshFilter.mesh, transform = meshFilter.transform.localToWorldMatrix};

                    combineInstance.Add(instance);

                    child.GetComponent<Collider>().enabled = false;
                }
            }

            var combinedMesh = transform.GetComponent<MeshFilter>().mesh = new Mesh();
            transform.GetComponent<MeshFilter>().mesh.CombineMeshes(combineInstance.ToArray());

            transform.GetComponent<MeshCollider>().sharedMesh = combinedMesh;

            Baked = true;
        }
    }
}