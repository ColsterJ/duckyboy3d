using AdvancedUtilities.Cameras;
using AdvancedUtilities.LerpTransformers;
using UnityEngine;

/// <summary>
/// When the player object hits this trigger, it will cause the side scrolling camera controller to auto rotate.
/// </summary>
public class TESTSCENE_SideScrollingTriggerAutoRotate : MonoBehaviour
{
    public SideScrollingCameraController SideScroller;

    public float HorizontalAmountToRotate = 180f;

    public float VerticalAmountToRotate = 0;

    public float TimeToRotate = 2f;

    public TESTSCENE_RotationStyle StyleOfRotation = TESTSCENE_RotationStyle.RotateBy;

    public bool SmoothRotation = false;

    public enum TESTSCENE_RotationStyle
    {
        RotateBy, RotateTo
    }

    private void OnTriggerEnter(Collider other)
    {
        ILerpTransformer lerp = SmoothRotation ? (ILerpTransformer)new SmoothInOutLerpTransformer() : new DoNothingLerpTransformer();

        if (StyleOfRotation == TESTSCENE_RotationStyle.RotateBy)
        {
            SideScroller.Rotation.AutoRotateBy(HorizontalAmountToRotate, VerticalAmountToRotate, TimeToRotate, lerp);
        }
        else
        {
            SideScroller.Rotation.AutoRotateTo(HorizontalAmountToRotate, VerticalAmountToRotate, TimeToRotate, lerp);
        }
    }
}
