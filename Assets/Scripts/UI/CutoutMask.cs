using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace UI
{
    public class CutoutMask : Image
    {
        private Material _material;
        private static readonly int StencilComp = Shader.PropertyToID("_StencilComp");

        public override Material materialForRendering
        {
            get
            {
                if (_material == null)
                {
                    _material = new Material(base.materialForRendering);
                    _material.SetInt(StencilComp, (int) CompareFunction.NotEqual);
                }

                return _material;
            }
        }
    }
}