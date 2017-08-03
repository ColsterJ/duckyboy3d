using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace AdvancedUtilities.Cameras.Components
{
    /// <summary>
    /// Used for determining if Unity's mouse is over a provided Rect, or over a GUI element.
    /// </summary>
    [Serializable]
    public class PointerOverGui
    {
        /// <summary>
        /// Input will not work while the mouse is over a gui.
        /// Uses "EventSystem.current.IsPointerOverGameObject()"
        /// </summary>
        [Tooltip("Input will not work while the mouse is over a gui. Uses \"EventSystem.current.IsPointerOverGameObject()\"")]
        public bool EventSystemWhenPointerOverGuiElement = true;

        /// <summary>
        /// The pointer will not be considered over the GUI if it is over one of these elements. 
        /// This only works when EventSystemWhenPointerOverGuiElement is true and only works when IsPointerOverGameObject() returns true.
        /// </summary>
        [Tooltip("The pointer will not be considered over the GUI if it is over one of these elements. This only works when EventSystemWhenPointerOverGuiElement is true and only works when IsPointerOverGameObject() returns true.")]
        public List<RectTransform> ExclusionsFromEventSystem = new List<RectTransform>();

        /// <summary>
        /// When the pointer is within any of these rects, it will not provide input.
        /// This is somewhat depreciated and you should probably use RectTransform list instead.
        /// </summary>
        [Tooltip("When the pointer is within any of these rects, it will not provide input. This is somewhat depreciated and you should probably use RectTransform list instead.")]
        public List<Rect> WhenPointerOverRects = new List<Rect>();

        /// <summary>
        /// When the pointer is within any of these RectTransforms, it will not provide input.
        /// </summary>
        [Tooltip("When the pointer is within any of these RectTransforms, it will not provide input.")]
        public List<RectTransform> WhenPointerOverRectTransform = new List<RectTransform>();

        /// <summary>
        /// Returns whether or not the pointer is over a Gui element or within a rect provided.
        /// </summary>
        /// <returns>Whether the pointer is on a GUI element.</returns>
        public bool IsPointerOverGui()
        {
            // This is an absolute check with no exclusions.
            if (EventSystemWhenPointerOverGuiElement && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                if (ExclusionsFromEventSystem.Count == 0)
                {
                    return true;
                }
                else
                {
                    // Check to see if it was an exclusion
                    Vector3 mouse = Input.mousePosition;
                    for (var index = 0; index < ExclusionsFromEventSystem.Count; index++)
                    {
                        var rectTransform = ExclusionsFromEventSystem[index];
                        if (rectTransform.gameObject.activeInHierarchy)
                        {
                            if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mouse))
                            {
                                return false;
                            }
                        }
                    }
                    return true;
                }
            }

            // Defined rectangles on screen.
            Vector3 mousePosition = Input.mousePosition;
            for (var index = 0; index < WhenPointerOverRects.Count; index++)
            {
                var rect = WhenPointerOverRects[index];
                if (rect.Contains(mousePosition))
                {
                    return true;
                }
            }

            // RectTransforms on screen.
            for (var index = 0; index < WhenPointerOverRectTransform.Count; index++)
            {
                var rectTransform = WhenPointerOverRectTransform[index];
                if (rectTransform.gameObject.activeInHierarchy)
                {
                    if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, mousePosition))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Refreshes what RectTransform objects the PointerOverGui object has referenced.
        /// This will blow away WhenPointerOverRectTransform and replace it with a new list containing all found RectTransforms.
        /// </summary>
        public void RefreshWhenPointerOverRectTransformGuiElements()
        {
            RectTransform[] rects = UnityEngine.Object.FindObjectsOfType<RectTransform>();
            WhenPointerOverRectTransform = new List<RectTransform>(rects);
        }
    }
}
