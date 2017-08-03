namespace AdvancedUtilities.LerpTransformers
{
    /// <summary>
    /// An interface that describes a way to modify the delta t passed to lerp functions.
    /// </summary>
    public interface ILerpTransformer
    {
        /// <summary>
        /// Processes the given t and returns a new t value.
        /// </summary>
        /// <param name="t">Given t.</param>
        /// <returns>Processed t.</returns>
        float Process(float t);
    }
}