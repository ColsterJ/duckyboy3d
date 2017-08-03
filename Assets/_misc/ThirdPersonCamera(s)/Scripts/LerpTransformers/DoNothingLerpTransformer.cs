namespace AdvancedUtilities.LerpTransformers
{
    /// <summary>
    /// Does nothing to the passed value and simply passes it back.
    /// </summary>
    public class DoNothingLerpTransformer : ILerpTransformer
    {
        /// <summary>
        /// Processes the given t and returns a new t value.
        /// </summary>
        /// <param name="t">Given t.</param>
        /// <returns>Processed t.</returns>
        public float Process(float t)
        {
            return t;
        }
    }
}
