using System;

namespace AdvancedUtilities.LerpTransformers
{
    /// <summary>
    /// Allows a function to be used against lerp t delta.
    /// </summary>
    public class DelegateLerpTransformer : ILerpTransformer
    {
        /// <summary>
        /// The function used by the delegate transformer.
        /// </summary>
        public Func<float, float> Function { get; private set; }

        /// <summary>
        /// Constructs a DelegateLerpTransformer by passing in a function to operate on the provided t value.
        /// </summary>
        /// <param name="function"></param>
        public DelegateLerpTransformer(Func<float, float> function)
        {
            Function = function;
        }

        /// <summary>
        /// Processes the given t and returns a new t value.
        /// </summary>
        /// <param name="t">Given t.</param>
        /// <returns>Processed t.</returns>
        public virtual float Process(float t)
        {
            return Function(t);
        }
    }
}
