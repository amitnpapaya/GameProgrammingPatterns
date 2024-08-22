using UnityEngine;

public class InputController : MonoBehaviour
{
    [SerializeField] private Camera _controlCamera;
    [SerializeField] private Actor _actor;
    
    void Update()
    {
        var leftClick = Input.GetMouseButtonDown(0);
        var rightClick = Input.GetMouseButtonDown(1);
        
        if (!leftClick && !rightClick) 
            return;
        
        var ray = _controlCamera.ScreenPointToRay(Input.mousePosition);
            
        if (!Physics.Raycast(ray, out var hit)) 
            return;

        if (leftClick)
        {
            if (hit.collider.gameObject.CompareTag("PlayerActor"))
            {
                _actor.SetSelected(true);
            }
            else
            {
                _actor.SetSelected(false);
            }
        }
        
        else if (rightClick)
        {
            if (_actor.Selected)
            {
                var destinationVector = hit.point;
                _actor.SetMovementDestination(destinationVector);   
            }
        }
    }
}