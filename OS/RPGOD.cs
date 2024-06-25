using _UNIX_;
using UnityEngine;
using UnityEngine.Rendering;

namespace _UNIX_
{
    internal partial class RPGOD : NUCLEON
    {
        public static RPGOD rpgod;
        public RenderPipelineAsset urp, hdrp;

        //----------------------------------------------------------------------------------------------------------

        protected override void Awake()
        {
            rpgod = this;
            base.Awake();
        }

        //----------------------------------------------------------------------------------------------------------

        public void SwitchToBuiltInRP() => QualitySettings.renderPipeline = null;
        public void SwitchToURP() => QualitySettings.renderPipeline = urp;
        public string LogCurrentRP() => QualitySettings.renderPipeline == urp ? "URP" : "Built-in RP";

        //----------------------------------------------------------------------------------------------------------

        protected override void OnDestroy()
        {
            base.OnDestroy();
            if (this == rpgod)
                rpgod = null;
        }
    }
}