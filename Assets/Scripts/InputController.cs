using UnityEngine;
using UnityEngine.AI;

public class InputController : MonoBehaviour
{
    [SerializeField] private Camera _controlCamera;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    
    void Update()
    {
        if (!Input.GetMouseButtonDown(0)) 
            return;
        
        var ray = _controlCamera.ScreenPointToRay(Input.mousePosition);
            
        if (!Physics.Raycast(ray, out var hit)) 
            return;
            
        var destinationVector = hit.point;
        _navMeshAgent.SetDestination(destinationVector);
    }
}