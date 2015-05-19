# Parallax Designer Documentation
This is the documentation for the Unity Tool, **Parallax Designer** 

## Contents
* Basics
* New, Save, Load
* Textures
* Editing Layers
* NPCs and Zoom Triggers


***
#### BASICS

When opening Unity, the Parallax Designer should be visibile, looking something like this: 
![Parallax Designer]
(editor.png)

If the editor is not immediately available, go to **Window > Parallax Designer**

The editor can then be dragged around the Unity window and placed wherever is most convenient for the user. 

With the editor open, take a minute to familiarize yourself with the various buttons and transform options available. 

At the very top, **Clean Up** will be used to delete any *disabled* or unused layers. 

**New**, **Save**, **Save As** and **Load** will be explained in the next section. 

The **Layer Count** transform has a minimum value of 1 and a maximum of 10. Any number can be entered into the text box, though it will reset to 10 if the number is larger than that. If hovering next to the text box, you can use the scroll-tip tool to scroll from 1 to 10.  
> The Layer Count tool is important for the use of this editor, as one of the main ideas behind the editor is that it uses these layers as copies. That means, if you start with 10 layers then decide you only need 3, the 7 left-over layers will be still be listed in the Unity hierarchy window, though they will be "disabled" by the editor.  

The **City Name** will be the default file name for any new file saves. 

You can click through the selected layers with the arrow buttons, or put in a layer number in the text box to go to it immediately. 

**Local Separation** is a transform that will increase or decrease the default distance the selected layer is away from all other layers. At defaults, all layers are 10 units apart from one another, front-to-back (the z-axis). This transform can alter that distance, moving layers further back (or visa versa) so that, in game-play, they move slower/faster in the distance when scrolling through scenes. 

The final sections are for editing NPCs and Zoom Triggers, to be explained in the final section. 


***
#### NEW, SAVE, LOAD

Although the second row of buttons (New, Save, Save As, and Load) may be familiar in a basic sense, its important to understand how they work in relation to the rest of the Parallax Designer. 

For example, the City Name will be the default file name when you **Save** or **Save As** a new file. It's good to note that when working on a scene, text should appear in the top section of the editor that says "Editing file.json" so be careful to not save over files by hitting **Save** rather than **Save As**. 

**New** will create a new city scene, defaulted with 3 layers and no City Name, NPCs, Zoom Triggers, etc. 

**Load** will load any city scenes that have already been saved in the past. Although **Load** should automatically open to the correct folder, it may not. In that case, the correct path is as follows:  
**Assets > Resources > Config > Phase One > Cities**
> All files should be .json files with names of the cities. Each .json file will have a matching .meta file, though those can be ignored for now. 


***
#### TEXTURES

Editing textures in the Parallax Designer is not as complicated as it may look. Simply clicking the **Load City Textures From Directory** button will open the folder of available cities (textures rather than .json files). From there, choose the folder with the name of the city you are trying to load in, and hit "Open". This should look through that city's folder, searching for files organized into "layer1", "layer2", etc folders. This then translates into Unity as actual layers with files attached, easily setting up that city's scene. 

> _**NOTE** This does NOT create a .json city file. This only compiles textures that have already been organized in folders. Save or Save As this scene to be able to load it later on._

For loading all textures associated with just one layer, first select that layer, then click on the **Load Layer Textures from Director** button. This works similarly to the other load textures button, and will look the same when the file dialogue box opens up. Rather than choosing the city folder only, however, open the folder then look for the layer folder that corresponds with the layer you are looking to change. "Open" this folder and it should reload that layer with the images in the corresponding folder. 
> If images are loaded in incorrectly, don't panic. This is probably because you selected the city folder rather than the layer folder. Just reload with the correct folder, and the layer should be loaded correctly. 


***
#### EDITING LAYERS

