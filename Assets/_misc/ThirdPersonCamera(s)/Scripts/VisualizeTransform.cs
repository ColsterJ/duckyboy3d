using UnityEngine;

namespace AdvancedUtilities
{
    /// <summary>
    /// Used to visualize Transform's vectors for rotation purposes.
    /// </summary>
    [ExecuteInEditMode]
    public class VisualizeTransform : MonoBehaviour
    {
        public float ForwardMagnitude = 1.5f;
        public float RightMagnitude = .75f;
        public float UpMagnitude = .75f;
        public Color Forward = Color.red;
        public Color Up = Color.green;
        public Color Right = Color.blue;

        void Update()
        {
            Debug.DrawLine(this.transform.position, this.transform.position + this.transform.forward * ForwardMagnitude, Forward);
            Debug.DrawLine(this.transform.position, this.transform.position + this.transform.up * UpMagnitude, Up);
            Debug.DrawLine(this.transform.position, this.transform.position + this.transform.right * RightMagnitude, Right);
        }
    }
}
