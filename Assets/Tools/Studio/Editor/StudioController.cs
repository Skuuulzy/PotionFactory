using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Collections;
using UnityEditor;
using System;

public class StudioController : MonoBehaviour
{
    //Private variables
    [SerializeField] private List<CameraPreset> _cameraPresets = new List<CameraPreset>();

    [SerializeField] private CameraPreset _selecteCameraPreset;

    [SerializeField] private Transform _presetsParentTransform;

    [SerializeField] private Transform _objectToCaptureParentTransform;

    //Public variables
    public List<CameraPreset> CameraPresets => _cameraPresets;
    public CameraPreset SelectedCameraPreset => _selecteCameraPreset;
    public Transform PresetsParentTransform { get => _presetsParentTransform; set => _presetsParentTransform = value; }
    public Transform ObjectToCaptureParentTransform { get => _objectToCaptureParentTransform; set => _objectToCaptureParentTransform = value; }


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

        Camera cameraComponent = camera.AddComponent<Camera>();
        cameraComponent.clearFlags = CameraClearFlags.SolidColor;
        //cameraComponent.backgroundColor = Color.clear;

        cameraPreset.Camera = cameraComponent;

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

    //Capture
    public void TakeCapture(string filePath, ImageFormat format, int width, int height, string objectCapture = null)
    {
        Camera cam = SelectedCameraPreset.Camera;
        int Fwidth = SelectedCameraPreset.ImageSize == ImageSize.Screen ? Screen.currentResolution.width : width;
        int Fheight = SelectedCameraPreset.ImageSize == ImageSize.Screen ? Screen.currentResolution.height : height;

        RenderTexture rt;
        rt = new RenderTexture(Fwidth, Fheight, 24);
        cam.targetTexture = rt;

        Texture2D capture = new Texture2D(Fwidth, Fheight, format == ImageFormat.PNG ? TextureFormat.RGBA32 : TextureFormat.RGB24, false);
        cam.Render();

        RenderTexture.active = rt;
        capture.ReadPixels(new Rect(0, 0, Fwidth, Fheight), 0, 0);
        capture.Apply();

        cam.targetTexture = null;
        RenderTexture.active = null;

        DestroyImmediate(rt);

        // Save capture
        byte[] bytes = format == ImageFormat.PNG ? capture.EncodeToPNG() : capture.EncodeToJPG();
        string extension = format == ImageFormat.PNG ? "png" : "jpg";

        if (objectCapture == null)
        {
            filePath = Path.Combine(filePath, $"Screenshot_{System.DateTime.Now:yyyyMMdd_HHmmss}.{extension}");
        }
        else
        {
            filePath = Path.Combine(filePath, $"{objectCapture}.{extension}");
        }

        File.WriteAllBytes(filePath, bytes);

        DestroyImmediate(capture);

        Debug.Log($"Capture saved to {filePath}");
    }

    public void TakeAllCapture(string filePath, ImageFormat format, int width, int height)
    {
        StartEditorCoroutine(TakeAllCaptureCoroutine(filePath, format, width, height, ObjectToCaptureParentTransform, this));
    }

    public static void StartEditorCoroutine(IEnumerator routine)
    {
        EditorApplication.update += () =>
        {
            if (!routine.MoveNext())
            {
                EditorApplication.update -= () => StartEditorCoroutine(routine);
            }
        };
    }

    static IEnumerator TakeAllCaptureCoroutine(string filePath, ImageFormat format, int width, int height, Transform objectToCapture, StudioController studioController)
    {
        foreach (Transform child in objectToCapture)
        {
            child.gameObject.SetActive(false);
        }

        foreach (Transform child in objectToCapture)
        {
            child.gameObject.SetActive(true);
            studioController.TakeCapture(filePath, format, width, height, child.name);
            yield return new WaitForSeconds(0.5f);
            child.gameObject.SetActive(false);
        }
    }
}

[System.Serializable]
public class CameraPreset
{
    //Private variables
    [SerializeField] private string _presetName;
    [SerializeField] private GameObject _presetParent;
    [SerializeField] private Camera _camera;
    [SerializeField] private bool _isSetUp;
    [SerializeField] private bool _wantRemove = false;
    [SerializeField] private bool _wantDeleteFromHierarchie = false;
    [SerializeField] private string _filePath = Application.dataPath;
    [SerializeField] private ImageFormat _imageFormat = ImageFormat.PNG;
    [SerializeField] private ImageSize _imageSize = ImageSize.Screen;
    [SerializeField] private Vector2Int _imageSizeValue = Vector2Int.zero;


    //Public variables
    public string PresetName { get => _presetName; set => _presetName = value; }
    public GameObject PresetParent { get => _presetParent; set => _presetParent = value; }
    public Camera Camera { get => _camera; set => _camera = value; }
    public bool IsSetup { get => _isSetUp; set => _isSetUp = value; }
    public bool WantRemoved { get => _wantRemove; set => _wantRemove = value; }
    public bool WantDeleteFromHierarchie { get => _wantDeleteFromHierarchie; set => _wantDeleteFromHierarchie = value; }
    public string FilePath { get => _filePath; set => _filePath = value; }
    public ImageFormat ImageFormat { get => _imageFormat; set => _imageFormat = value; }
    public ImageSize ImageSize { get => _imageSize; set => _imageSize = value; }
    public Vector2Int ImageSizeValue { get => _imageSizeValue; set => _imageSizeValue = value; }
}

public enum ImageFormat { PNG, JPG }
public enum ImageSize { Screen, Custom }
