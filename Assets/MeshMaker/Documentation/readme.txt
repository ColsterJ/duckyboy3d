Mesh Maker - Modeling & Editing Collection.
v2.8.3 - Released 13/06/2017 by Alan Baylis
----------------------------------------


Foreword
----------------------------------------
Thank you for your purchase of the Mesh Maker collection. This package contains a bundle of 15 utilities for creating and working with meshes & prefabs within Unity.

Level Editor - Fast, Easy & Flexible Modeling
Geom 2.0 - Quick and Easy Geometric Modeling
Mesh Editor - Advanced Mesh Editing Utility
Isosurface - Mathematics Inspired Meshes
CSG - Perform Boolean Operations on Meshes
MeshPainter - Paint Directly on Meshes
Blenderizer - Object Focused Editor Camera
Metaballs - Create New Organic Base Meshes
Mesh Tools - Eleven Editing Tools in One
Sculptor - Solids of Revolution
3D Brush - Fully Featured Prefab Painting Tool
Mesh Cutter - Cut, Copy & Paste Parts of Meshes
Prefab Maker - Combine, Atlas & Prefab creation
Construction - Building Made Quick & Easy
Snap To Grid - Fast & Easy Positioning

Notes
----------------------------------------
The Mesh Maker menu is located in the Window section on the Unity menu bar.

The main menu now pops up on the scene view window and hides itself in the bottom left corner when not in use. To show the menu simply place the mouse over the visible title.

In Unity 5 it is recommended to turn of continuous baking of lightmaps by going to the Unity menu and navigating to Window/Lighting/Lightmaps and unchecking the checkbox. This will prevent lag in the editor.

You can open up all of the programs at the same time and use them in combination, though this has not been thoroughly tested in all cases. If you have any problems try restarting Mesh Maker or Unity to see if that fixes the problem.

Level Editor:
You may run into an situation where the faces don't get created. Often it is because the corners of the lines don't meet properly. Deleting the lines and trying again or adding extra lines often fixes this type of problem. If you get a warning message in the console it is recommened to undo the last operation, save your work and try again, possibly in a new way.   
  
Geom:
Duplicates of grids made in the hierarchy will not be automatically added to Geom.
Don't delete the Framework or the Grids from the hierarchy, use Geom to delete grids or use the Exit button to end the build session.
Renaming the Grids in the hierarchy will not change their names within Geom.

Mesh Editor:
All of the functions work on skinned meshes except for triangle and edge extrusion.
To work on different sized models you can now change the epsilon value by going into the setting and changing the epsilon value.
In the last update the keys to extrude have been changed from the shift key to the ctrl+shift keys to avoid deselecting triangles and edges when extruding.
When extruding multiple triangles that meet at a single point they sometimes have inverted faces. The solution is to extrude the triangles individually.

Metaballs:
This is an early version of the program and is suitable for working with up to a dozen or so quality metaballs but more if you work in a lower resolution. Ideal for creating a base mesh that can be tweaked with the Mesh Editor program.

CSG:
It is not recommended to do more that a few operations on the same target object or the geometry may become corrupt. While the program works well on simple objects it may fail on very complicated objects or objects with underlying problems like holes in their geometry. It is highly recommended that you save your work and scene before using this software.

Construction:
After entering and leaving play mode, click on the Construction window once before continuing to edit the building.
When adjusting the roof or stairs pitch for too long it may throw an error such as "Could not create actor. Maybe you are using too many colliders or rigidbodies in your scene?".


To-do List
----------------------------------------
Geom:
Done - Improved normal smoothing.
Done - More CPU friendly.
Export to FBX.
Done - Spline based frameworks.
Add holes to mesh design. 

Mesh Cutter:
Done - Editing of skinned meshes.

Mesh Editor:
Done - Extrusion and deformation of meshes
Done - Skinned mesh support
Done - Bridge edges
Done - Combine/Split vertices
Done - Vertex snap
Done - Full mesh optimization
Done - Edge Splitting
Done - Modify Normals
- Edge loops

Mesh Painter:
Done - Direct painting of meshes in scene view window.
Better painting across triangle edges.
Different brushes.
Fill option.

CSG:
Mesh optimization to remove excess triangles after each operation.
Done - Option to group triangles that share the same materials.
Done - Recalculate bounding boxes and other fixes.

Mesh Tools:
Done - Mesh Tools work with submeshes.
Done - More advanced UV Mapping.
Split with plane, with fill option.

Metaballs:
Option to use different formulas for calculating the metaball fields.
Finite support formula.
Auto resizing of the dimensions.
Process only the cubes around the mesh surface and other optimizations.

Sculptor:
Partial revolutions.
Choice of degree to rotate between 0 and 360.
Cap ends.

Prefab Maker:
Skinned meshes.
Tiled textures.
Hidden triangle removal.
Check with all types of shaders.

Construction:
This program is an early release and there are a lot of opportunities to improve the speed of the program so that it works well with larger buildings.
Fix stairs material selection where the picking is hitting the wrong collider.
The window movement locks up after showing a dialog, but you can click in the scene view to remedy this for now.
The memory management will also be improved to eliminate the mesh leaks.
Options for using box colliders and simplified mesh colliders when building the final prefab will also be added.
New features like a top down view of the floor plan will be added to the program to allow for faster construction and the placing of furniture objects.
The roofs and eaves will be improved to allow for individually adjustable pitch and eave widths.
Double or triple width walls that can fit larger windows and other features.
Done: Other optimizations will be made to the final prefab meshes by combining them based on their materials and using submeshes where possible.


Common Issues / FAQ
----------------------------------------
Please visit the home page at http://www.meshmaker.com for the latest news and help forum.


Attributions
----------------------------------------
The textures used in the videos were made By Dexsoft Games and are available for free on the Asset Store. (http://www.assetstore.unity3d.com/en/#!/content/1809)

The resources feature of Prefab Maker is kindly provided by Simon Oliver to the public domain. (https://github.com/handcircus/Unity-Resource-Checker)

The example assets used with 3D Brush are available for free from Unity Technologies at the following locations:

Standard Terrain Assets
Available in Unity by clicking on the Assets menu item and going to Import Package/Terrain Assets.

Terrain Assets
https://www.assetstore.unity3d.com/en/#!/content/6

Shanty Town: Brush Vegetation
https://www.assetstore.unity3d.com/en/#!/content/41

The city model used in the screenshots and videos for Mesh Cutter is called Japanese Otaku City and was created by ZENRIN CO. LTD. I would like to thank the creators of this great asset which is available for free on the asset store at  https://www.assetstore.unity3d.com/en/#!/content/20359

The scene used in the Mesh Editor v1.4 update video was kindly provided for free by Patryk Zatylny here on the UAS at https://www.assetstore.unity3d.com/en/#!/content/35361

First person drifter version 1.0 by Benjamin (http://torahhorse.com)


Contact
----------------------------------------
Alan Baylis
www.meshmaker.com
support@meshmaker.com


Update Log
-----------------------------------------
v1.0.0 released 20/09/13
First release of Mesh Maker. 

v1.0.1 released 5/10/13
Improved smoothing of normals.
More CPU friendly.

v1.2.0 released 6/01/14
New feature for creating meshes using the marching cubes algorithm and isosurfaces.
New Isosurface tutorial.
Added option to create inverted meshes.
Single grid meshes are now possible, ideal for billboards and decals.
User defined maximum smooth angle.
Texture planar mapping is now possible from three directions.
Included 60 primitive meshes and 17 frameworks.

v1.3.0 released 26/01/14 
A new toolkit called Mesh Tools has been added with the following features:
Clone Meshes
Move/Rotate Pivot Point
Uniform Scaling
Snap To Grid
Invert Meshes
Flip Meshes

v1.4.0 released 26/02/14
A new texturing utility called Mesh Painter has been added.
New Mesh Painter tutorial.
Added support for meshes with submeshes to Mesh Tools.
New GUI look.
Source moved to Editor folder to allow test builds.

v1.5.0 released 15/05/14
Mesh Painter now paints directly on objects in the Scene View window.
Fixed a bug in Mesh Tools related to Snap To Grid timing out after about 10 minutes.
Box Projection UV mapping now labeled correctly in Mesh Maker.
Restored planar UV mapping in the XY plane. 
Original Mesh Painter functionality moved to Mesh Tools.
Mesh Tools now contains 8 utilities including Split Mesh and Transform Textures.

v1.6.0 released 04/06/14
Added Boolean Ops (CSG/BSP operations) to the programs list. 
Cleaned up Mesh Painter memory usage.
First release of Blenderizer.

v1.7.0 released 02/08/14
Beta release (v0.7) of Mesh Editor.
Original Mesh Maker program is now called Geom.
Integrated Blenderizer into Mesh Editor.
Added feature to Geom to set control point position explicitly.

v1.7.1 released 06/08/14
Mesh Editor:
Fixed bug when deleting multiple triangles.
Fixed undo/redo step names
Fixed double undo for delete
Cleaned up more memory leaks.
Added option to change epsilon values.

v1.8.0 released 03/09/14
Beta release (v0.5) release of Metaballs.
Improved main menu which discretely hides itself.
New GUI layout for all windows.
Added three new tools to Mesh Tools. Real Scale, Double Sided Meshes and Save Mesh.
Small fixes here and there.

v1.9.0 released 26/09/14
Beta release (v0.8) of Sculptor.
Fixed Undo/Redo bug in Mesh Editor.
Main folder is now portable to subfolders.

v1.9.1 released 06/11/14
Added individual menus for each program

v1.9.2 released 31/03/15
Checked all programs for Unity 5 compatibility
Upgraded Geom to version 2.0 with the following new features:
- New 2D interface with 3 orthogonal views.
- Now possible to move points in the Y axis.
- Insert and Delete control points.
- Unlimited grid sizes.
- Undo/Redo feature.
- Easily enter accurate point positions.
- Save/Load program settings.
- Save/Load frameworks to XML files.
- Custom colors for 3D and 2D views.
- Show/Hide 2D interface.
- Change background picture for tracing objects.
- 15 new starting geometric shapes.
- Numbered control points in 2D and 3D views.
- Synchronized point movement across all grids.
- Automatic prefab creation with new meshes.

v1.9.3 released 24/04/15
First release of 3D Brush v0.9.
Fix to Geom for scroll wheel in 2D interface.

v1.9.4 released 18/05/15
First release of Mesh Cutter v0.9.
Changed the example brush scale in 3D Brush
3D Brush fix to duplication of save settings in the XML file
3D Brush now saves all settings

v1.9.5 released 15/06/15
Complete rebuild of Mesh Editor v1.0
Moved Mesh Maker menu to the main Unity menu bar
Programs now listed in alphabetical order
Main menu now works in all scene view windows including isometric views

v1.9.6 released 31/07/15
First release of Prefab Maker v0.9
Changed the mesh copy dialog for Mesh Cutter & Mesh Editor
Added select all triangles by material to Mesh Cutter
Mesh Cutter Alt key now blocks triangle selection
Added Mac friendly hotkey selection to Mesh Cutter

v1.9.7 released 14/08/15
Mesh Cutter now works with skinned meshes.
Mesh Editor now works with skinned meshes.
Subdivision of triangles added to Mesh Editor.

v1.9.8 released 04/10/15
Mesh Maker:
New splash screen
Real time news updates
Resources folder renamed to Files
New settings for Main Menu
Fixed namespace clashes
Main folder is now moveable within project folder

Mesh Editor:
New option to show normals
New option to recalculate normals
New option to recalculate tangents
Update UVs now works with normal editing
Locked object stays locked after delete, etc
Fix to Snap To Grid
Fix to shader warning
Fix to handle position after extrude
Fix to double sided normals
Added tool to generate secondary UV set
Added tool to recalculate normals

3D Brush:
Added option to generate random names

Prefab Maker:
Added clear warning if selected folder is not empty
Suppressed log warnings if materials and submeshes don't match

v2.0 released 08/12/15 
First release of Construction v0.9

v2.0.1 released 16/12/15
Construction: 
Fixed bug where features would rotate due to the reverse fence option.
Fixed bug where insertion of foundation walls would move everything up too high.
Fixed bug where features added directly from project window would disappear on load design.
Fixed bug where features that were not persistent would disappear on load design.  The program now allows you to save the feature first. The window movement locks up after showing the dialog but you can click in the scene view to remedy this.
Fixed bug with half wall generation on foundation levels. Now they act like standard foundation walls.
Fixed bug where material would change name to stairs material.
Fixed null reference exception for splash screen in Close() method.
Fixed missing reference exception when using the Exit button.
Updated the final building prefab to combine all of the individual meshes into one mesh with the minimum number of materials.
Optimized the Construction object to use minimum number of materials.
Added settings to select new colors for the grid, selection rectangle and floor labels.

v2.0.2 released 25/01/16
All Programs:
Fixed missing empty folders on import
Added keys to program as a backup to the keys.txt file

v2.0.3 released 29/04/16
All Programs:
Improved support for Unity 5.x and hotkey on Mac

Mesh Editor:
Added Edge Splitting
Added Triangle/Hole Filling
Added Individual UV Editing
Added Procedural Geometry - Basic 2D/3D Shapes
Added Modify Normals
Added Weld Selected Vertices/Edges/Triangles
Added Un-Weld Selected Vertices/Edges/Triangles
Added Join Vertices
Added Join Edges
Added Check For Unused Vertices and Zero Area Triangles
Added Dedicated OBJ Export Function
Updated Rotate/Translate pivot point handles
Added UV and Normal colors to saved settings
Added Custom Skins
Changed Snap To Grid (Removed check boxes and changed to current axis handle only)
Updated selection when switching between unique and coincident vertices/edges 
Fixed Edge selection/deselection when Coincident is off
Changed the extrude key combination from Shift to Ctrl+Shift to avoid creating unwanted edges and triangles when deselecting


Construction:
Now has a hotkey selector in Settings.

Mesh Painter:
Now uses the Ctrl/Command key and the left mouse button to paint

Mesh Tools:
Now uses the Ctrl/Command key and the left mouse button to edit meshes

v2.0.5 released 29/09/16
Updated artwork

Mesh Editor:
Added: Edge and Triangle splitting now updates tangents by default
Fixed: Instructions on extruding to readme.txt and tutorial

Prefab Maker:
Fixed: Linked to latest Unity DLLs to support GetTexDim in Prefab Maker

v2.4 released 13/12/16
Beta release of Level Editor.

v2.5 released 14/01/17
Mesh Editor:
Added Toolbar
Updated GUI, single column
Added Pages for each settings foldout
Added Custom skin
Added Support for Blend Shapes
Added Check for missing normals and UVs and add if necessary
Added Programmable hotkeys

Level Editor:
Faster texture transforms
Transformed features are recalculated to find intersections and new faces
Snap to grid when moving vertices, edges and faces now only snaps in the direction of the current handle
Added enable and disable option for cursors
Created new GUI for rendering modes
Fixed namespace errors for Addons
Added smoothing & faceting faces (duplicate triangles may be affecting calculations)
Added option to constrain proportions when scaling textures
Fixed movable directory
Adjusted eraser sensitivity
Added feature to convert Unity static meshes to lines
Now saves all selected parts to mesh and prefab
Changed rotate and scale around selected vertices unless pivot point has been moved

v2.6 released 05/02/17
Updated 3D Brush GUI
Complete rewrite of Boolean Ops and renamed the program to CSG
Added experimental SVG loading to Level Editor
Changed Level Editor GUI, moved Parts to front page
Added field for entering blueprint scale directly in Level Editor

v2.7 released 09/03/17
Updated GUI for all programs to make them uniform
Added custom skins for all programs
General fixes and small changes

v2.8 released 27/03/17
First release of Snap To Grid v1.0
Added new door & window models specifically designed for Construction

v2.8.1 released 24/04/17
Update to Snap To Grid

v2.8.2 released 07/06/17
Fixed Snap To Grid, no longer crashing on Mac
Fixed hotkeys on the Mac
Fixed Sculptor, no errors on Mac
Fixed Scene Menu, no errors on Mac

v2.8.3 released 13/06/17
Fixed problem with Mesh Editor and Level Editor

