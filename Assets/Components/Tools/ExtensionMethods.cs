using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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

    public static Quaternion ClampAxis(this Quaternion quaternion, Axis axis,float minAngle, float maxAngle)
    {
        Quaternion clampedQuaternion;
        
        switch (axis)
        {
            case Axis.X:
                float x = Mathf.Clamp(quaternion.eulerAngles.x, minAngle, maxAngle);
                clampedQuaternion = Quaternion.Euler(x,quaternion.eulerAngles.y,quaternion.eulerAngles.z);
                break;
            case Axis.Y:
                float y = Mathf.Clamp(quaternion.y, minAngle, maxAngle);
                clampedQuaternion = new Quaternion(quaternion.x, y, quaternion.z, quaternion.w).normalized;
                break;
            case Axis.Z:
                float z = Mathf.Clamp(quaternion.z, minAngle, maxAngle);
                clampedQuaternion = new Quaternion(quaternion.x, quaternion.y, z, quaternion.w).normalized;
                break;
            case Axis.W:
                float w = Mathf.Clamp(quaternion.w, minAngle, maxAngle);
                clampedQuaternion = new Quaternion(quaternion.x, quaternion.y, quaternion.z, w).normalized;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(axis), axis, null);
        }
        
        return clampedQuaternion;
    }
    
    public enum Axis
    {
        X,Y,Z,W
    }

    #endregion
}