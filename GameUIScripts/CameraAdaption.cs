using UnityEngine;
using System.Collections;
public class CameraAdaption : MonoBehaviour
{
    public static float desiginWidth = 800.0f;
    public static float desiginHeight = 480.0f;
    public static bool sound = true;
    AudioListener audioListener;
    void Awake()
    {
        sound = (PlayerPrefs.GetInt("sound", 1) == 1);
        camera.aspect = 800.0f / 480.0f;
        float lux = (Screen.width - CameraAdaption.desiginWidth * Screen.height / CameraAdaption.desiginHeight) / 2.0f;
        camera.pixelRect = new Rect(lux, 0, Screen.width - 2 * lux, Screen.height);
        audioListener = GetComponent<AudioListener>();
        int soundflag = PlayerPrefs.GetInt("sound", 1);       
        audioListener.enabled = (soundflag == 1);
    }   

    public static Matrix4x4 getMatrix()
    {
        Matrix4x4 guiMatrix = Matrix4x4.identity;
        float lux = (Screen.width - CameraAdaption.desiginWidth * Screen.height / CameraAdaption.desiginHeight) / 2.0f;
        guiMatrix.SetTRS(new Vector3(lux, 0, 0), Quaternion.identity, new Vector3(Screen.height / CameraAdaption.desiginHeight, Screen.height / CameraAdaption.desiginHeight, 1));
        return guiMatrix;
    }
    public static Matrix4x4 getInvertMatrix()
    {
        Matrix4x4 guiInverseMatrix = getMatrix();
        guiInverseMatrix = Matrix4x4.Inverse(guiInverseMatrix);
        return guiInverseMatrix;
    }
}
