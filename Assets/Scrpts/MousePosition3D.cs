using UnityEngine;

public class MousePosition3D : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;

    private void Update()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            transform.position = hit.point;
            Debug.Log(hit.point);
        }
    }
}