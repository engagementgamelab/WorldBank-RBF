# Parallax Designer Documentation
This is the documentation for the Unity Tool, **Parallax Designer** 

## Contents
* [Quick Start Guide](#quick)
* [New, Save, Load](#files)
* [Editing Layers](#edit)
* [NPCs and Zoom Triggers](#npc)

***

***
#### <a name="quick"> QUICK START GUIDE </a>

<span style="color:green">**1. Open Unity and open Parallax Designer if not already visible.  
2. Load a previous .json city file OR load textures with the *Load City Textures from Directory* button.  
3. Begin making transform changes.**</span>  

***

When opening Unity, the Parallax Designer should be visibile, looking something like this: 

![Parallax Designer]
(parallaxeditor.png)

If the editor is not immediately available, go to **Window > Parallax Designer**

The editor can then be dragged around the Unity window and placed wherever is most convenient for the user. 

The same can be done with the **Parallax Preview**, listed above the Designer in the Window drop-down, which will allow a game-play-like scroll-through without having to actually hit play to test the scene. To zoom, use the left scroller, up to zoom-in, down to zoom-out. Scrolling through the game left to right uses the top scroller, from left to right. 

![Parallax Preview]
(preview.png)

At the very top of the Designer, in bright yellow, the **Clean Up** button can be used to delete any *disabled* or unused layers. 

**New**, **Save**, **Save As** and **Load** are fairly self explainatory, see the following section for more information.  

If the city scene has already been organized into the proper files and folders, then the **Load City Textures from Directory** will load the city folder selected, properly setting up the scene as such. 

The **Layer Count** transform has a minimum value of 1 and a maximum of 10. If hovering next to the text box, you can use the scroll-tip tool to scroll from 1 to 10.
> The Layer Count tool is important for the use of this editor, as one of the main ideas behind the editor is that it uses these layers as copies. That means, if you start with 10 layers then decide you only need 3, the 7 left-over layers will be still be listed in the Unity hierarchy window, though they will be "disabled" by the editor. 

The **City Name** will be the default file name for any new file saves. 

You can click through the selected layers with the arrow buttons, or put in a layer number in the text box to go to it immediately. 

**Layer Settings** is the first drop-down, where you can load layer textures from the organized file networks, for individual layers. This works like loading the city textures, though the corresponding "layer#" folder must be selected. 
>**Local Separation** is a transform that will increase or decrease the default distance the selected layer is away from all other layers. At defaults, all layers are 10 units apart from one another, front-to-back (the z-axis). This transform can alter that distance, moving layers further back (or visa versa) so that, in game-play, they move slower/faster in the distance when scrolling through scenes. 

The final two drop-down sections are for **editing NPCs and Zoom Triggers**. NPCs are important, interactable people in the game, and must be added individually as their own layers with their own textures, put in manually in the "Texture" slot. You can click-through the NPC layers separately from the regular layers.  

The Zoom Triggers are items in the game that call for camera depth, so they trigger a camera zoom around that image layer. Like NPCs, these layers are very important for the scene to work correctly, so they must be added individually. 

Both Zoom Trigger and NPC layers contain their own transforms so they may be moved in the scene, colliders can be altered, etc. The **Refresh** button will update the layer after the texture is changed(no change will be visible before the layer is refreshed), the **Reset** will return all default settings, and the **Delete** button will delete that layer. Further explanation in the final section.

*With all of the sections understood, you shouldn't have any trouble putting together scenes. Further reading is available below, should there be any confusion.*

***

***
#### <a name="files"> NEW, SAVE, LOAD </a>

Although the second row of buttons (New, Save, Save As, and Load) may be familiar in a basic sense, its important to understand how they work in relation to the rest of the Parallax Designer. 

For example, the City Name will be the default file name when you **Save** or **Save As** a new file. It's good to note that when working on a scene, text should appear in the top section of the editor that says "Editing file.json" so be careful to not save over files by hitting **Save** rather than **Save As**. 

**New** will create a new city scene, defaulted with 5 layers and no City Name, NPCs, Zoom Triggers, etc. 

**Load** will load any city scenes that have already been saved in the past. Although **Load** should automatically open to the correct folder, though it may not always. In that case, the correct path is as follows:  
**Assets > Resources > Config > Phase One > Cities**
> All files should be .json files with names of the cities. Each .json file will have a matching .meta file, though those can be ignored for now. 

***

***
#### <a name="edit"> EDITING LAYERS </a>

There are several ways to edit layers using the Parallax Designer. The most simple way of setting up a scene would be to load all of the already organized files and folders using the **Load City Textures from Directory**. This will open the folder of available cities (textures rather than .json files). From there, choose the folder with the name of the city you are trying to load in, and hit "Open". This should look through that city's folder, searching for files organized into "layer1", "layer2", etc folders. This then translates into Unity as actual layers with files attached, easily setting up that city's scene.
> _**NOTE** This does NOT create a .json city file. This only compiles textures that have already been organized in folders. Save or Save As this scene to be able to load it later on._

However, this may not be what you want. Let's say there is one layer that has been changed since the last edit, and so you only want to update a single layer. In order to do that, the **Load Layer Textures from Directory** button under the **Layer Settings** drop-down will work just fine. In order to do this, however, do not simply open the city folder, as textures will be loaded in but incorrectly. Rather, open the city folder that you are working on, then choose the "layer#" folder that corresponds with whichever layer you are editing. This should correctly load all textures into that layer.  

If images are loaded in incorrectly, don't panic. This is probably because you selected the city folder rather than the layer folder. Just reload with the correct folder, and the layer should be loaded without any kinks. 
>_**NOTE** This will only properly work if the textures have already been correctly organized into corresponding "layer#" folders._

***

***
#### <a name="npc"> NPCs AND ZOOM TRIGGERS </a>

The final, and possibly most important, sections for editing layers are the **NPC** and **Zoom Trigger** drop-downs. 

**NPCs** are important game figures that players must interact with during game-play. Because they do not work the same as the rest of the city, which is basically made up of background images layered to add depth, they must be added individually. NPC textures must be put in manually, either by dragging them into the texture slot, or clicking on the little circle to the right of the texture slot, then picking the intended texture from the pop-up menu. 

The NPC's texture will not update immediately after selection, however, and the **Refresh** button must be pressed before any changes can be seen. All other transforms work in real-time, so you can see as the layer is moved, collider is resized, etc. 

Colliders in Unity work as containers around game items that will trigger certain events, such as a dialogue box, upon player interaction. In order to get these triggers to work correctly, the containers should be fairly snug around the actual image. For example, the layer may be 100x100 but much of the background is actually transparent. The player shouldn't get a game response upon click on this transparent area, since it will appear in-game as the background. So, the collider container can be resized and moved around to fit the image as closely as possible. 
*This works the same with Zoom Triggers, though the colliders are for camera detection rather than player interaction.*

In order to keep track of NPCs and help the game-making process running smoothly, there is a **Symbol** section right above the texture slot for the NPCs. This space can be used to identify the different NPCs so that when dialogue is ready for the game, it is easy to locate and match up NPCs with their dialogue. 

Zoom Triggers can be confusing to understand, but to simplify, they are items in the game, usually that make up the background or foreground, that call for the camera to zoom out/in around it. For example, if there is a pillar mid-screen in the front, then this may be set as a Zoom Trigger layer that would cause the camera to zoom out around the pillar then zoom back in to regular settings as it moves past it. 

The Zoom Trigger transforms are pretty basic, like moving the layer along the X and Y axis, altering the collider, and adding in the texture just like with the NPC transforms. However, the **Zoom Target** transform is important, as that defines how far the camera will zoom "out" around the item. The camera actually starts already slightly zoomed in, so zooming "out" would mean setting the Zoom Target at or close to 0, which is the limit at which the camera may zoom out. Setting this number further away from 0, moving into the negatives, will cause the camera to zoom further in when passing this layer. 

> _**NOTE** for both the Zoom Triggers and NPCs, the transforms may not be visible immediately upon a city load. These transforms should appear after attempting to click through the individual layers (not the main layer selection)._
