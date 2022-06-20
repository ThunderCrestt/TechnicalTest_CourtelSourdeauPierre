using UnityEngine;

namespace com.technical.test
{
    /// <summary>
    ///     Rotates a transform with a settable angular speed 
    ///     for a specific duration
    /// </summary>
    public class Rotator : MonoBehaviour
    {
        [SerializeField]
        public string _identifier = "The Amazing Rotator";
        [Header("Rotation parameters")]
        [SerializeField]
        public float _timeBeforeStoppingInSeconds  = 3;
        [SerializeField]
        public bool _shouldReverseRotation = false;

        [SerializeField]
        public RotationSettings _rotationsSettings = default;

        public float _angleModifier = 1f;
        public float _timePassed;

        void Awake()
        {
            CalculateModifierAccordingToReverse();
        }

        private void CalculateModifierAccordingToReverse()
        {
            _angleModifier = 1f;
            if (_shouldReverseRotation)
            {
                _angleModifier = -1f;
            }
        }

        void OnEnable()
        {
            _timePassed = 0f;
        }

        void Update()
        {
            Vector3 angularSpeedPerSeconds = _rotationsSettings.AngleRotation / _rotationsSettings.TimeToRotateInSeconds;
            _rotationsSettings.ObjectToRotate.rotation *= Quaternion.Euler(_angleModifier * Time.deltaTime * angularSpeedPerSeconds);
            _timePassed += Time.deltaTime;
            if (_timePassed > _timeBeforeStoppingInSeconds)
            {
                this.enabled = false;
            }
        }

        void OnValidate()
        {
            CalculateModifierAccordingToReverse();
        }

        public override string ToString()
        {
            return $"{_identifier} makes a rotation with an angle of {_rotationsSettings.AngleRotation} every {_rotationsSettings.TimeToRotateInSeconds} seconds";
        }
    }
}