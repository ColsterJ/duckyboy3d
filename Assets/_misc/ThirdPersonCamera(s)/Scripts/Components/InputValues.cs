
using UnityEngine;

namespace AdvancedUtilities.Cameras.Components
{
    /// <summary>
    /// A class that contains a set of values to be input into the InputComponent
    /// </summary>
    public class InputValues
    {
        #region Properties

        /// <summary>
        /// The input for Horizontal input.
        /// </summary>
        public float? Horizontal { get; set; }

        /// <summary>
        /// The input for Vertical input.
        /// </summary>
        public float? Vertical { get; set; }

        /// <summary>
        /// The input for zooming in input.
        /// </summary>
        public float? ZoomIn { get; set; }

        /// <summary>
        /// The input for zooming out input.
        /// </summary>
        public float? ZoomOut { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor providing values for each input.
        /// </summary>
        /// <param name="horizontal">The horizontal input value.</param>
        /// <param name="vertical">The vertical input value.</param>
        /// <param name="zoomIn">The zoom in input value.</param>
        /// <param name="zoomOut">The zoom out input value.</param>
        public InputValues(float? horizontal, float? vertical, float? zoomIn, float? zoomOut)
        {
            Horizontal = horizontal;
            Vertical = vertical;
            ZoomIn = zoomIn;
            ZoomOut = zoomOut;
        }

        /// <summary>
        /// Empty constructor leaving all values null.
        /// </summary>
        public InputValues()
        {
            // empty contructor
        }

        #endregion

        /// <summary>
        /// Returns a new InputValues object with copied data from this InputValues object.
        /// </summary>
        /// <returns></returns>
        public InputValues Copy()
        {
            return new InputValues(Horizontal, Vertical, ZoomIn, ZoomOut);
        }

        /// <summary>
        /// Applies all non-null input properties in the given Input to this object.
        /// This overrides the current properties with the given properties if they are not null.
        /// </summary>
        /// <param name="input">Input to apply properties over this objects properties.</param>
        public void ApplyOver(InputValues input)
        {
            Horizontal = input.Horizontal ?? Horizontal;
            Vertical = input.Vertical ?? Vertical;
            ZoomIn = input.ZoomIn ?? ZoomIn;
            ZoomOut = input.ZoomOut ?? ZoomOut;
        }

        /// <summary>
        /// Applies all non-null input properties in the given Input to this object's null properties.
        /// This only sets null properties with the values on the given input and will not override any of it's set properties.
        /// </summary>
        /// <param name="input">Input to apply properties under this objects properties.</param>
        public void ApplyUnder(InputValues input)
        {
            Horizontal = !Horizontal.HasValue ? input.Horizontal : null;
            Vertical = !Vertical.HasValue ? input.Vertical : null;
            ZoomIn = !ZoomIn.HasValue ? input.ZoomIn : null;
            ZoomOut = !ZoomOut.HasValue ? input.ZoomOut : null;
        }

        /// <summary>
        /// Adds each non-null value to the current values.
        /// </summary>
        /// <param name="horizontal">Multiply Horizontal by.</param>
        /// <param name="vertical">Multiply Vertical by.</param>
        /// <param name="zoomIn">Multiply ZoomIn by.</param>
        /// <param name="zoomOut">Multiply ZoomOut by.</param>
        public void AddValues(float? horizontal, float? vertical, float? zoomIn, float? zoomOut)
        {
            Horizontal = Horizontal.HasValue ? Horizontal + horizontal : horizontal;
            Vertical = Vertical.HasValue ? Vertical + vertical : vertical;
            ZoomIn = ZoomIn.HasValue ? ZoomIn + zoomIn : zoomIn;
            ZoomOut = ZoomOut.HasValue ? ZoomOut + zoomOut : zoomOut;
        }

        /// <summary>
        /// Multiplies each non-null property by its corresponding float value.
        /// </summary>
        /// <param name="horizontal">Multiply Horizontal by.</param>
        /// <param name="vertical">Multiply Vertical by.</param>
        /// <param name="zoomIn">Multiply ZoomIn by.</param>
        /// <param name="zoomOut">Multiply ZoomOut by.</param>
        public void MultiplyBy(float? horizontal, float? vertical, float? zoomIn, float? zoomOut)
        {
            Horizontal *= horizontal.HasValue ? horizontal : 1;
            Vertical *= vertical.HasValue ? vertical : 1;
            ZoomIn *= zoomIn.HasValue ? zoomIn : 1;
            ZoomOut *= zoomOut.HasValue ? zoomOut : 1;
        }

        /// <summary>
        /// Sets each corresponding property given a true value to null.
        /// </summary>
        /// <param name="horizontal">Set Horizontal to null.</param>
        /// <param name="vertical">Set Vertical to null.</param>
        /// <param name="zoomIn">Set ZoomIn to null.</param>
        /// <param name="zoomOut">Set ZoomOut to null.</param>
        public void NullOutValues(bool horizontal, bool vertical, bool zoomIn, bool zoomOut)
        {
            Horizontal = horizontal ? null : Horizontal;
            Vertical = vertical ? null : Vertical;
            ZoomIn = zoomIn ? null : ZoomIn;
            ZoomOut = zoomOut ? null : ZoomOut;
        }

        /// <summary>
        /// Inverts each corresponding property given a true value, multiplying it by -1.
        /// </summary>
        /// <param name="horizontal">Invert Horizontal.</param>
        /// <param name="vertical">Invert Vertical.</param>
        /// <param name="zoomIn">Invert ZoomIn.</param>
        /// <param name="zoomOut">Invert ZoomOut.</param>
        public void InvertValues(bool horizontal, bool vertical, bool zoomIn, bool zoomOut)
        {
            Horizontal *= horizontal ? -1 : 1;
            Vertical *= vertical ? -1 : 1;
            ZoomIn *= zoomIn ? -1 : 1;
            ZoomOut *= zoomOut ? -1 : 1;
        }

        /// <summary>
        /// Multiplies this input by delta time.
        /// </summary>
        public void MultiplyByDeltaTime()
        {
            Horizontal *= Time.deltaTime;
            Vertical *= Time.deltaTime;
            ZoomIn *= Time.deltaTime;
            ZoomOut *= Time.deltaTime;
        }

        /// <summary>
        /// Replaces any null input values currently on this input with 0;
        /// </summary>
        public void SetNullValuesToZero()
        {
            Horizontal = Horizontal.HasValue ? Horizontal : 0;
            Vertical = Vertical.HasValue ? Vertical : 0;
            ZoomIn = ZoomIn.HasValue ? ZoomIn : 0;
            ZoomOut = ZoomOut.HasValue ? ZoomOut : 0;
        }

        public override string ToString()
        {
            return string.Format(
                "Horizontal: {0}, Vertical: {1}, ZoomIn: {2}, ZoomOut: {3}", 
                Horizontal.HasValue ? Horizontal.Value.ToString() : "null",
                Vertical.HasValue ? Vertical.Value.ToString() : "null",
                ZoomIn.HasValue ? ZoomIn.Value.ToString() : "null",
                ZoomOut.HasValue ? ZoomOut.Value.ToString() : "null"
            );
        }
    }
}
