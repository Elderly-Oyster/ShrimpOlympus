using System;
using UnityEngine;
using UnityEngine.AI;

namespace Modules.Base.DeliveryTycoon.Scripts.GamePlay.Cars.NPCCars
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class NPCCarController : MonoBehaviour
    {
        private NavMeshAgent _agent;
        private float _speed = 10f;
        public CarType Type { get; set; }

        public event Action<NPCCarController, CarType> OnFinishPointReached;
        
        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _agent.baseOffset = 0;
            _agent.speed = _speed;
        }

        public void MoveTo(Vector3 destination,  Quaternion rotation)
        {
            _agent.SetDestination(destination); 
            _agent.isStopped = false;
            _agent.speed = _speed;
            _agent.stoppingDistance = 2f;
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<DespawnZone>())
            {
                _agent.speed = 0f;
                _agent.isStopped = true;
                OnFinishPointReached?.Invoke(this, Type);
            }
        }

        public void ResetParameters()
        {
            transform.position = Vector3.zero;
            _agent.speed = _speed;
            _agent.isStopped = false;
            _agent.ResetPath();
        }
        
    }
}