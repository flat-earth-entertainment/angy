using DinoFracture;
using UnityEngine;

namespace MeshPhysics
{
    /// <summary>
    ///     This component will cause a fracture to happen at the point of impact.
    /// </summary>
    [RequireComponent(typeof(FractureGeometry))]
    public class FractureOnTrigger : MonoBehaviour
    {
        /// <summary>
        ///     The minimum amount of force required to fracture this object.
        ///     Set to 0 to have any amount of force cause the fracture.
        /// </summary>
        public float forceThreshold;

        /// <summary>
        ///     Falloff radius for transferring the force of the impact
        ///     to the resulting pieces.  Any piece outside of this falloff
        ///     from the point of impact will have no additional impulse
        ///     set on it.
        /// </summary>
        public float forceFalloffRadius = 1.0f;

        /// <summary>
        ///     If true and this is a kinematic body, an impulse will be
        ///     applied to the colliding body to counter the effects of'
        ///     hitting a kinematic body.  If false and this is a kinematic
        ///     body, the colliding body will bounce off as if this were an
        ///     unmovable wall.
        /// </summary>
        public bool adjustForKinematic = true;

        private Rigidbody _impactBody;
        private float _impactMass;
        private Vector3 _impactPoint;

        private Vector3 _impactVelocity;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Lemming"))
            {
                _impactBody = other.GetComponent<Rigidbody>();
                _impactMass = other.GetComponent<Rigidbody>() != null ? other.GetComponent<Rigidbody>().mass : 1.0f;
                _impactVelocity = other.GetComponent<Rigidbody>().velocity;

                var rb = GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Always have the impact velocity point in our moving direction
                    _impactVelocity *= Mathf.Sign(Vector3.Dot(rb.velocity, _impactVelocity));
                }

                var mag = _impactVelocity.magnitude;
                var force = 0.5f * _impactMass * _impactVelocity * mag;

                if (forceThreshold * forceThreshold <=
                    force.sqrMagnitude)
                {
                    // Finds point between two vectors
                    _impactPoint = transform.position +
                                   0.5f * Vector3.Normalize(other.transform.position - transform.position);

                    var localPoint = transform.worldToLocalMatrix.MultiplyPoint(_impactPoint);

                    GetComponent<FractureGeometry>().Fracture(localPoint);
                }
            }
        }

        private void OnFracture(OnFractureEventArgs args)
        {
            if (args.OriginalObject.gameObject == gameObject)
            {
                var radiusSq = forceFalloffRadius * forceFalloffRadius;

                for (var i = 0; i < args.FracturePiecesRootObject.transform.childCount; i++)
                {
                    var piece = args.FracturePiecesRootObject.transform.GetChild(i);

                    var rb = piece.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        var force = _impactMass * _impactVelocity / (rb.mass + _impactMass);

                        if (forceFalloffRadius > 0.0f)
                        {
                            var distSq = (piece.position - _impactPoint).sqrMagnitude;
                            force *= Mathf.Clamp01(1.0f - distSq / radiusSq);
                        }

                        rb.AddForceAtPosition(force * rb.mass, _impactPoint, ForceMode.Impulse);
                    }
                }

                if (adjustForKinematic)
                {
                    // If the fractured body is kinematic, the collision for the colliding body will
                    // be as if it hit an unmovable wall.  Try to correct for that by adding the same
                    // force to colliding body.
                    var thisBody = GetComponent<Rigidbody>();
                    if (thisBody != null && thisBody.isKinematic && _impactBody != null)
                    {
                        var force = thisBody.mass * _impactVelocity / (thisBody.mass + _impactMass);
                        _impactBody.AddForceAtPosition(force * _impactMass, _impactPoint, ForceMode.Impulse);
                    }
                }
            }
        }
    }
}