using System.Collections;
using UnityEngine;

public class HandAI : MonoBehaviour
{
    [SerializeField] private Transform _handPalm;
    [SerializeField] private Transform _body;
    [SerializeField] private HandMoveObject[] _moveObjects;

    private void Start() {
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        float timer = 0;
        bool worldPosition = false;
        float speed = 0f;

        while (true)
        {
            for (int i = 0; i < _moveObjects.Length; i++)
            {
                timer = _moveObjects[i].ExecutionTime;
                worldPosition = _moveObjects[i].WorldPosition;
                speed = _moveObjects[i].Speed;
                Vector3 fixedPosition = _moveObjects[i].GetMovePoint(_body.position);

                while (timer > 0)
                {
                    if (worldPosition)
                        _handPalm.position = Vector3.Lerp(_handPalm.position, fixedPosition, speed * Time.deltaTime);
                    else
                        _handPalm.position = Vector3.Lerp(_handPalm.position, _moveObjects[i].GetMovePoint(_body.position), speed * Time.deltaTime);

                    timer -= Time.deltaTime;
                    yield return new WaitForFixedUpdate();
                }
            }

            yield return new WaitForFixedUpdate();
        }
    }

    [System.Serializable]
    private class HandMoveObject
    {
        private enum MoveType
        {
            FixedPosition = 0,
            RandomPosition = 1,
            NearestToPlayerPoint = 2
        }

        [SerializeField] private MoveType _moveType;
        [SerializeField] private bool _worldPosition = false;
        [Space()]
        [SerializeField] private Vector3 _fixedLocalPoint;
        [SerializeField] private float _circleRadius;
        [SerializeField] private float _followSpeed;
        [SerializeField] private float _executionTime;

        public bool WorldPosition => _worldPosition;
        public float Speed => _followSpeed;
        public float ExecutionTime => _executionTime;

        public Vector3 GetMovePoint(Vector3 startPosition)
        {
            switch (_moveType)
            {
                case MoveType.FixedPosition:
                    return startPosition + _fixedLocalPoint;
                    
                case MoveType.RandomPosition:
                    return startPosition + (Vector3)Random.insideUnitCircle * _circleRadius;

                case MoveType.NearestToPlayerPoint:
                    return startPosition + (Player.PlayerTransform.position - startPosition).normalized * _circleRadius;
                default:
                    throw new System.Exception("Несуществующий MoveType");
            }
        }
    }    
}



