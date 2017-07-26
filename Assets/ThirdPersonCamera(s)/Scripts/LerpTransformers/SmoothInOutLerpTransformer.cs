namespace AdvancedUtilities.LerpTransformers
{
    /// <summary>
    /// Produces a smoothing effect at the beginning and end of lerping.
    /// </summary>
    public class SmoothInOutLerpTransformer : DelegateLerpTransformer
    {
        /// <summary>
        /// Constructs a SmoothInOutLerpTransformer used to smooth the beginnings and ends of lerping.
        /// </summary>
        public SmoothInOutLerpTransformer() : 
        base(
            // passes delegate to the base class. A bit of a weird looking constructor.
            (t) => t * t * (3f - 2f * t)
        )
        {}

        /// <summary>
        /// Processes the given t and returns a new t value.
        /// </summary>
        /// <param name="t">Given t.</param>
        /// <returns>Processed t.</returns>
        public override float Process(float t)
        {
            return Function(t);
        }
    }
}
