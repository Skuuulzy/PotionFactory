using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StudioController : MonoBehaviour
{
    //Private variables
    private List<CameraPreset> _cameraPresets = new List<CameraPreset>();

    private CameraPreset _selecteCameraPreset;

    private Transform _presetsParentTransform;

    //Public variables
    public List<CameraPreset> CameraPresets => _cameraPresets;
    public CameraPreset SelectedCameraPreset => _selecteCameraPreset;
    public Transform PresetsParentTransform { get => _presetsParentTransform; set => _presetsParentTransform = value; }

    //Screenshot
    public enum ImageFormat { PNG, JPG }

    public void AddPreset()
    {
        _cameraPresets.Add(new CameraPreset());

        if (_cameraPresets.Count == 1)
        {
            SelectCameraPreset(_cameraPresets[0]);
        }

        Debug.Log("New camera preset created!");
    }

    public void WantRemove(CameraPreset cameraPreset)
    {
        cameraPreset.WantRemoved = !cameraPreset.WantRemoved;
    }

    public void RemovePreset(CameraPreset cameraPreset, bool removeInHierarchy = false)
    {
        _cameraPresets.Remove(cameraPreset);

        if (removeInHierarchy && cameraPreset.PresetParent != null)
        {
            foreach (Transform child in _presetsParentTransform)
            {
                if (child.gameObject == cameraPreset.PresetParent)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }

        Debug.Log($"Camera preset [{cameraPreset.PresetName}] is removed");
    }

    public void SetUpPreset(CameraPreset cameraPreset)
    {
        GameObject presetParent = new GameObject(cameraPreset.PresetName);
        presetParent.transform.parent = _presetsParentTransform;
        cameraPreset.PresetParent = presetParent;

        GameObject camera = new GameObject($"{cameraPreset.PresetName} camera");
        camera.transform.parent = presetParent.transform;
        cameraPreset.Camera = camera.AddComponent<Camera>();

        cameraPreset.IsSetup = true;
        Debug.Log($"Camera preset [{cameraPreset.PresetName}] is setup");
    }

    public void SelectCameraPreset(CameraPreset cameraPreset)
    {
        _selecteCameraPreset = cameraPreset;

        foreach (CameraPreset preset in _cameraPresets)
        {
            preset.PresetParent.SetActive(preset == _selecteCameraPreset);
        }

        Debug.Log($"Camera preset [{cameraPreset.PresetName}] is selected");
    }

    //Screenshot
    public void TakeScreenshot(string filePath, ImageFormat format, int width, int height)
    {
        // Créer une texture temporaire avec la taille souhaitée
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGBA32, false);

        // Capturer le screenshot
        Rect rect = new Rect(0, 0, width, height);
        screenshot.ReadPixels(rect, 0, 0);
        screenshot.Apply();

        // Convertir la texture en bytes selon le format choisi
        byte[] bytes = null;
        if (format == ImageFormat.PNG)
        {
            bytes = screenshot.EncodeToPNG();
        }
        else if (format == ImageFormat.JPG)
        {
            bytes = screenshot.EncodeToJPG();
        }

        filePath = Path.Combine(Application.dataPath, "XXX" + "." + format.ToString().ToLower());

        // Enregistrer le fichier
        if (bytes != null)
        {
            File.WriteAllBytes(filePath, bytes);
            Debug.Log($"Screenshot saved to {filePath}");
        }
        else
        {
            Debug.LogError("Failed to encode screenshot");
        }

        // Libérer la texture
        DestroyImmediate(screenshot);
    }
}

[System.Serializable]
public class CameraPreset
{
    //Private variables
    private string _presetName;
    private GameObject _presetParent;
    private Camera _camera;
    private bool _isSetUp;
    private bool _wantRemove = false;
    private bool _wantDeleteFromHierarchie = false;


    //Public variables
    public string PresetName { get => _presetName; set => _presetName = value; }
    public GameObject PresetParent { get => _presetParent; set => _presetParent = value; }
    public Camera Camera { get => _camera; set => _camera = value; }
    public bool IsSetup { get => _isSetUp; set => _isSetUp = value; }
    public bool WantRemoved { get => _wantRemove; set => _wantRemove = value; }
    public bool WantDeleteFromHierarchie { get => _wantDeleteFromHierarchie; set => _wantDeleteFromHierarchie = value; }
}
