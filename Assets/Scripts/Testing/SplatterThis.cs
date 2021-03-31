using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Testing
{
    public class SplatterThis : MonoBehaviour
    {
        public GameObject shroom;
        public ParticleSystem splatter;

        private void Update()
        {
            if (Input.GetButtonDown("Fire1"))
            {
                // Destroy(shroom);
                splatter.Play(true);
                // StartCoroutine(SpawnSchoom());
                StartCoroutine(SplitMesh(true));
            }
        }

        private IEnumerator SpawnSchoom()
        {
            yield return new WaitForSeconds(3f);
            Instantiate(shroom, transform.position, quaternion.identity);
        }

        public IEnumerator SplitMesh(bool destroy)
        {
            if (GetComponent<MeshFilter>() == null || GetComponent<SkinnedMeshRenderer>() == null)
            {
                yield return null;
            }

            if (GetComponent<Collider>())
            {
                GetComponent<Collider>().enabled = false;
            }

            var m = new Mesh();
            if (GetComponent<MeshFilter>())
            {
                m = GetComponent<MeshFilter>().mesh;
            }
            else if (GetComponent<SkinnedMeshRenderer>())
            {
                m = GetComponent<SkinnedMeshRenderer>().sharedMesh;
            }

            var materials = new Material[0];
            if (GetComponent<MeshRenderer>())
            {
                materials = GetComponent<MeshRenderer>().materials;
            }
            else if (GetComponent<SkinnedMeshRenderer>())
            {
                materials = GetComponent<SkinnedMeshRenderer>().materials;
            }

            var verts = m.vertices;
            var normals = m.normals;
            var uvs = m.uv;
            for (var submesh = 0; submesh < m.subMeshCount; submesh++)
            {
                var indices = m.GetTriangles(submesh);

                for (var i = 0; i < indices.Length; i += 3)
                {
                    var newVerts = new Vector3[3];
                    var newNormals = new Vector3[3];
                    var newUvs = new Vector2[3];
                    for (var n = 0; n < 3; n++)
                    {
                        var index = indices[i + n];
                        newVerts[n] = verts[index];
                        newUvs[n] = uvs[index];
                        newNormals[n] = normals[index];
                    }

                    var mesh = new Mesh();
                    mesh.vertices = newVerts;
                    mesh.normals = newNormals;
                    mesh.uv = newUvs;

                    mesh.triangles = new[] {0, 1, 2, 2, 1, 0};

                    var go = new GameObject("Triangle " + i / 3);
                    //GO.layer = LayerMask.NameToLayer("Particle");
                    go.transform.position = transform.position;
                    go.transform.rotation = transform.rotation;
                    go.AddComponent<MeshRenderer>().material = materials[submesh];
                    go.AddComponent<MeshFilter>().mesh = mesh;
                    go.AddComponent<BoxCollider>();
                    var explosionPos = new Vector3(transform.position.x + Random.Range(-0.5f, 0.5f),
                        transform.position.y + Random.Range(0f, 0.5f), transform.position.z + Random.Range(-0.5f, 0.5f));
                    go.AddComponent<Rigidbody>().AddExplosionForce(Random.Range(300, 500), explosionPos, 5);
                    Destroy(go, 5 + Random.Range(0.0f, 5.0f));
                }
            }

            GetComponent<Renderer>().enabled = false;

            yield return new WaitForSeconds(1.0f);
            if (destroy)
            {
                Destroy(gameObject);
            }
        }
    }
}