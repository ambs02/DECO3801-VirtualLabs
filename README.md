# DECO3801

how to setup body tracking 

follow tutorial: https://www.youtube.com/watch?v=xKRPwEDwzNg
typed tutorial is below:

https://github.com/Unity-Technologies/barracuda-release
download this zip file 
[bg_remove/bg_tutorial_images/NNCam_project_files.png]

in unity project 
* windows 
* package manager
* add package from disk 
* select barracuda folder
* select pacakge.json
* should b version 3.0.1 but doesnt rly matter 



https://github.com/keijiro/NNCam
download this project 
NNCAM
assets
create new folder VirtualBG
drag and drop all files to folder VirtualBG
* effect, effect2, nncam, onyx, pose, room, etc. 
￼


open room scene 
click camera
define camera divide name in webcam input script to b laptop camera



https://github.com/keijiro/NNCam/releases/tag/v0.0.1
download the bodypixonnxx zip file 


open VirtualBG folder 
open ONNX folder
copy paste all the downloaded files into the folder

in room scene camera
choose nncam resource
then open nncam
set model to stride16
￼




now go to this GitHub 
https://github.com/ambs02/DECO3801-VirtualLabs

and in the bg_remove folder
you’ll find NNCam and Room scripts 
in the corresponding files, replace all these scripts


￼
￼
￼
after adding, go to Room scene, and ensure all the inspector settings look like this ^^
VERY IMPORTANT: in the colour, make sure that A value is set to 0

change the screen size from Free Aspect -> Full HD
￼
run the game and it should work 
example:
￼

