# CloneRigRotationControllerTool
UnityTool to Save/Duplicate/Share rig controllers rotation.

## Why ?
Based on request from one holdfast game modder
He need to save each pose to commercial usage.

## How it works ?
Select the main rig controller of your rig and the tool will recursively save all child GameObject rotation to a .json file (in /Resources of your project).
You can also do the reverse, select your main rig controller and load your .json, he will recursively set each child of .json to your selected GameObject.
