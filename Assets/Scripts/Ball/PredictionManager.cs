using System.Collections.Generic;
using Ball.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Ball
{
    public class PredictionManager : Singleton<PredictionManager>
    {
        public GameObject obstacles;
        public int lineLength;

        [HideInInspector]
        public GameObject indicatorHolder;

        private PhysicsScene _currentPhysicsScene;

        private Scene _currentScene;
        private GameObject _dummy;

        private readonly List<GameObject> _dummyObstacles = new List<GameObject>();

        private LineRenderer _lineRenderer;
        private PhysicsScene _predictionPhysicsScene;
        private Scene _predictionScene;

        private void Start()
        {
            Physics.autoSimulation = false;

            _currentScene = SceneManager.GetActiveScene();
            _currentPhysicsScene = _currentScene.GetPhysicsScene();

            var parameters = new CreateSceneParameters(LocalPhysicsMode.Physics3D);
            _predictionScene = SceneManager.CreateScene("Prediction", parameters);
            _predictionPhysicsScene = _predictionScene.GetPhysicsScene();

            _lineRenderer = GetComponent<LineRenderer>();
        }

        private void FixedUpdate()
        {
            if (_currentPhysicsScene.IsValid())
            {
                _currentPhysicsScene.Simulate(Time.fixedDeltaTime);
            }
        }

        private void OnDestroy()
        {
            KillAllObstacles();
        }

        public void CopyAllObstacles()
        {
            foreach (Transform t in obstacles.transform)
            {
                if (t.gameObject.GetComponent<Collider>() != null)
                {
                    var fakeT = Instantiate(t.gameObject);
                    fakeT.transform.position = t.position;
                    fakeT.transform.rotation = t.rotation;
                    var fakeR = fakeT.GetComponent<Renderer>();
                    if (fakeR)
                    {
                        fakeR.enabled = false;
                    }

                    SceneManager.MoveGameObjectToScene(fakeT, _predictionScene);
                    _dummyObstacles.Add(fakeT);
                }
            }
        }

        private void KillAllObstacles()
        {
            foreach (var o in _dummyObstacles)
            {
                Destroy(o);
            }

            _dummyObstacles.Clear();
        }

        public void Predict(GameObject subject, Vector3 currentPosition, Vector3 force)
        {
            if (indicatorHolder != null)
            {
                Destroy(indicatorHolder);
            }

            if (_currentPhysicsScene.IsValid() && _predictionPhysicsScene.IsValid())
            {
                if (_dummy == null)
                {
                    _dummy = Instantiate(subject);
                    SceneManager.MoveGameObjectToScene(_dummy, _predictionScene);
                }

                _dummy.transform.position = currentPosition;
                _dummy.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);
                _lineRenderer.positionCount = 0;
                _lineRenderer.positionCount = lineLength;


                for (var i = 0; i < lineLength; i++)
                {
                    _predictionPhysicsScene.Simulate(Time.fixedDeltaTime * 2);
                    _lineRenderer.SetPosition(i, _dummy.transform.position - new Vector3(0, 0.49f, 0));
                }

                indicatorHolder = _dummy.GetComponent<GroundIndicator>().spawnedIndicator;

                Destroy(_dummy);
            }
        }
    }
}