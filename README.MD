# Project ANA - Open Source Conversation Suite

[![License: GPL v3](https://img.shields.io/badge/License-GPL%20v3-blue.svg)](http://www.gnu.org/licenses/gpl-3.0)

World's first **Open Source Conversation Platform** which comes with a **Graphical Chat Flow Designer** and **Chat Simulator**.
Supported channels are Web, Facebook, Android & iOS (SDKs available).

----------

[![Studio and Simulator Image](https://github.com/Kitsune-tools/ProjectANA/blob/master/Assets/anaflow.png)](https://github.com/Kitsune-tools/ProjectANA/blob/master/Assets/anaflow.png)

----------

## Installation

**Prerequisites:**

* Windows 10 PC with *developer mode* enabled. 
  - To enable developer mode, go to your PC Settings > Update & Security > For Developers. Under 'Use developer features', choose 'Developer Mode'. 

**Installing ANA Conversation Studio:**

1.  Download `ANA Conversation Studio Setup x.x.x.msi` from [releases](https://github.com/Kitsune-tools/ProjectANA/releases/latest).
2.  Run it and follow the on screen instructions. 
    - If developer mode is not already enabled in your PC, installer will ask you to enable it.  
    - Once installation is complete, you can run the ANA Conversation Studio from the desktop or from start menu.

 **Note:** 
 
 To use localhost APIs in your chat, you need to run a small command which will unblock location APIs being used from the ANA Conversation Simulator. 
   - Open command prompt and run `checknetisolation loopbackexempt -a -n=44cabbc3-0a2f-4255-b5bb-b099cd5762f9_hvc7j3s42jt6e`. 
   - By default, localhost URLs are blocked in UWP Apps for security, above command will unblock it for only this particular app. 

----------

## Note

This is just the setup documentation. ANA Conversation Suite has much more scope than predefined flows. You can call your own APIs from the chat, capture and save information from the user. You can take different types of inputs like button clicks, text, image, video etc. For now, explore the code to get to know all the possibilities.

Detailed documentation is on its way.

## License

ANA Conversation Suite is available under the [GNU GPLv3 license](https://www.gnu.org/licenses/gpl-3.0.en.html).

