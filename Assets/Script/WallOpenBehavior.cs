using UnityEngine;
using VIVE.OpenXR.Passthrough;
using VIVE.OpenXR.Samples;
namespace VIVE.OpenXR.CompositionLayer.Samples.Passthrough
{
    public class WallOpenBehavior : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        int ID;
        OpenXR.Passthrough.XrPassthroughHTC pass;
        void Start()
        {
            PassthroughAPI.CreatePlanarPassthrough(out pass, LayerType.Overlay);
            
        }

        // Update is called once per frame
        void Update()
        {
            if (Time.realtimeSinceStartup > 10)
            {
                PassthroughAPI.DestroyPassthrough(pass);
            }
        }
    }
}
