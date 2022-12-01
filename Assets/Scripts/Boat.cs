using System;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class Boat : MonoBehaviour
{
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private NavMeshSurface _navMeshSurface;
    [SerializeField] private NavMeshLink _navMeshLink;
    [SerializeField] private float _arriveDistanceOffset = 3f;
    [SerializeField] private string _targetAreaMask = "Water";

    public Action OnArrived { get; set; }
    public Action OnInitialized { get; set; }

    private void Start()
    {
        _navMeshSurface.enabled = false;
        float maxDistance = Vector3.Distance(transform.position, Vector3.zero);

        if (NavMesh.SamplePosition(transform.position, out NavMeshHit hit, maxDistance, 1 << NavMesh.GetAreaFromName(_targetAreaMask)) == true)
        {
            hit.position += (transform.position - hit.position).normalized * _arriveDistanceOffset;
            _agent.SetDestination(hit.position);
        }

        _navMeshSurface.enabled = true;
        _navMeshLink.enabled = false;
        OnInitialized?.Invoke();
    }

    private void Update()
    {
        if (_agent.ReachedDestinationOrGaveUp() == true)
        {
            OnArrived?.Invoke();
            _agent.enabled = false;
            _navMeshLink.enabled = true;
            enabled = false;
        }
    }
}