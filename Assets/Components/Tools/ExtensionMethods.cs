using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public static class ExtensionMethods
{
    #region LOAD SCENE

    public static List<string> LoadedSceneNames()
    {
        List<string> loadedSceneNames = new List<string>();
        
        for (int i = 0; i < SceneManager.loadedSceneCount; i++)
        {
            string sceneName = SceneManager.GetSceneAt(i).name;
            loadedSceneNames.Add(sceneName);
        }

        return loadedSceneNames;
    }

    #endregion
    
    #region GRAPHIC
    public static void SetAlpha(this UnityEngine.UI.Graphic graphic, float value)
    {
        Color c = graphic.color;
        c.a = value;
        graphic.color = c;
    }
    
    #endregion

    #region QUATERNION

    public static Quaternion ClampAxis(this Quaternion quaternion, Axis axis, float minAngle, float maxAngle)
    {
        Vector3 euler = quaternion.eulerAngles;

        switch (axis)
        {
            case Axis.X:
                euler.x = ClampAngle(euler.x, minAngle, maxAngle);
                break;
            case Axis.Y:
                euler.y = ClampAngle(euler.y, minAngle, maxAngle);
                break;
            case Axis.Z:
                euler.z = ClampAngle(euler.z, minAngle, maxAngle);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
        }

        return Quaternion.Euler(euler);
    }

    // Helper function to handle angle wrapping
    private static float ClampAngle(float angle, float min, float max)
    {
        // Normalize angle to the range [-180, 180]
        angle = (angle > 180) ? angle - 360 : angle;

        return Mathf.Clamp(angle, min, max);
    }
    
    public enum Axis
    {
        X,Y,Z
    }

    #endregion
    
    public static Color GenerateRandomColorWithAlpha(float alpha)
    {
        var r = Random.Range(0f, 1f);
        var g = Random.Range(0f, 1f);
        var b = Random.Range(0f, 1f);

        return new Color(r, g, b, alpha);
    }
}