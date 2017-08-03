namespace AdvancedUtilities.Cameras.Components.Events
{
    /// <summary>
    /// An interface that allows a class to listen to the vertical rotation event fired from a RotationComponent.
    /// </summary>
    public interface VerticalDegreesListener
    {
        /// <summary>
        /// Tells the listener the amount of degrees the RotationComponent has rotated by.
        /// </summary>
        /// <param name="degrees">Amount of degrees rotated by.</param>
        void DegreesRotated(float degrees);
    }
}
