# CloneRigRotationControllerTool
UnityTool to Save/Duplicate/Share rig controllers rotation.

## Get Started 
- Download the lastest release of unitypackage in [releases](https://github.com/OlivierArgentieri/CloneRigRotationControllerTool/releases) matching your unity version.
- Open and import all
> **WARNING** Required .NET 4 : go to edit > project settings > player > other settings and select ".net 4.x ..."

## Why ?
Based on a request from an holdfast game modder, to save each poses to make simpler her commercial pipeline.

Why this tool is in widow mode in unity 2021.2 and not in Overlay ? 
> Because holdfast SDK only run on Unity 2018. Overlay Editor are not present in 2018.


## How it works ?
- Select the main rig controller of your rig and the tool will recursively save all child GameObject rotation to a .json file (in /Resources of your project).
- You can also do the reverse, select your main rig controller and load your .json, he will recursively set each child of .json to each child of your selected GameObject.

## Demo :
![demo](https://github.com/OlivierArgentieri/CloneRigRotationControllerTool/blob/main/assets/Demo.gif)

## Unsecure mode ?
When you try to load .json file in a GameObject, a test is realized to ensure compatibility.

But if you want to "force" loading of this .json, you can check this checkbox to bypass the compatibility check.
This is usefull to load .json generated from beta version of this tools, but be careful it's at your own risk to use this, the result cannot be guaranteed.
