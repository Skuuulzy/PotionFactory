using Components.Machines;
using UnityEngine;

public class MachinePreviewController : MonoBehaviour
{
    [SerializeField] private Transform _3dViewHolder;
    
    public void InstantiatePreview(MachineTemplate machineTemplate, float scale)
    {
        DeletePreview();
            
        Instantiate(machineTemplate.GridView, _3dViewHolder);
        transform.localScale = new Vector3(scale, scale, scale);
    }

    public void DeletePreview()
    {
		foreach (Transform obj in _3dViewHolder)
		{
			Destroy(obj.gameObject);
		}
	}
}
