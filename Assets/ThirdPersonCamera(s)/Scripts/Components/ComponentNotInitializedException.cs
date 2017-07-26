using System;

namespace AdvancedUtilities.Cameras.Components
{
    /// <summary>
    /// An error thrown by CameraComponents when accessing something that requires the CameraComponent to have been initialized first.
    /// </summary>
    public class ComponentNotInitializedException : Exception
    {
        public ComponentNotInitializedException(string message) :base(message) { }
    }
}
