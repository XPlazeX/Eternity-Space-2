using UnityEngine;
 
//The effect is also triggered when it is not running
[ExecuteInEditMode]
 //Screen post-processing special effects generally need to be bound to the camera
[RequireComponent(typeof(Camera))]
 //Provide a post-processing base class, the main function is to directly drag the shader through the Inspector panel to generate the material corresponding to the shader
public class ScreenPostEffectBase : MonoBehaviour
{
 
         //Drag directly on the Inspector panel
    public Shader shader = null;
    private Material _material = null;
    public Material _Material
    {
        get
        {
            if (_material == null)
                _material = GenerateMaterial(shader);
            return _material;
        }
    }
 
         //Create a material for screen special effects according to the shader
    protected Material GenerateMaterial(Shader shader)
    {
                 // Does the system support
        // if (!SystemInfo.supportsImageEffects)
        // {            
        //     return null;
        // }
 
        if (shader == null)
            return null;
                 //Need to determine whether the shader supports
        if (shader.isSupported == false)
            return null;
        Material material = new Material(shader);
        material.hideFlags = HideFlags.DontSave;
        if (material)
            return material;
        return null;
    }
 
  void OnDisable()
    {
        if (_material != null)
        {
            DestroyImmediate(_material);
        }
    }
 
}
