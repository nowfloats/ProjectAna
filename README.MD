# Project ANA - Open Source Conversation Suite

[![License: GPL v3](https://img.shields.io/badge/License-GPL%20v3-blue.svg)](http://www.gnu.org/licenses/gpl-3.0)

World's first **Open Source Conversation Platform** which comes with a **Graphical Chat Flow Designer** and **Chat Simulator**. Absolutely modular architecture, ability to add your own customizations and infinitely scalable. 

Setup time less than 30 minutes. Edit content and flow without changing any code.
Android & iOS SDK available on request. 

**Launching soon:**
 
Ability to switch to an agent within the chat and switch back! (Hybrid chat), Web & Messenger SDK

----------

[![Studio and Simulator Image](https://s3-ap-southeast-1.amazonaws.com/nfcontent-cdn/public-assets/github/ProjectANA/ANA-Studio-Simulator.jpg)](https://s3-ap-southeast-1.amazonaws.com/nfcontent-cdn/public-assets/github/ProjectANA/ANA-Studio-Simulator.jpg)

----------



## Components Overview

**ANA Conversation Suite contains 3 components:**

1.  **Platform:** Backend API which serves the chat requests. This component is responsible for merging the chat flow template with the chat content and returning the flow when requested by the clients.
2.  **Studio:** Graphical designer of the chat flow. This can be helpful when designing the chat flow and reduce the amount of unnecessary effort needed down by 80%. This also enables non-devs to edit the flow and it's content at any given point of time, changes being reflected instantly.
3.  **Simulator:** Used to test and experience the chat flow.

## Installation

**Pre-requisites:**

1.  Windows/Linux/Mac to host the Backend API
2.  Mongo DB
3.  Windows PC with .NET Framework 4.5 to run Conversation Studio
4.  Windows 10 PC to use Conversation Simulator (UWP App)

**Setup Platform:**

1.  Clone the repo.
2.  In ANA Conversation Platform project, open appsettings-Sample.json and edit DatabaseConnectionSettings section to match your MongoDB details.
3.  Change Collection names to new/empty collections.
4.  Save and Rename appsettings-Sample.json to appsettings.json
4.  Build and publish the project to the server as usual. It's an ASP.NET Core 1.1 project, which can be hosted on Windows, Linux and Mac.

**Setup Studio:**

1.  Build the ANA Conversation Studio project yourself or download the binary directly. Open the exe once done.
2.  Add the details of the same collections and MongoDB connection string used in the Platform Setup in (File -> Manage Connections->Add. Click 'Save and close'.
3.  An additional MongoDB Collection name is asked as "LayoutCollectionName", this should be a new/empty collection.
3.  To create a sample chat flow, right click on the studio document(white area) and click 'Create new node'.
    1.  Add buttons and sections to it using the Node Editor and Section Button Editor. Also, add content to each button and section you have added with the help of Content Editor.
    2.  Set the 'IsStartNode' of the Node to 'true'.
3.  Save the flow.

**Setup Simulator:**

1.  To build the ANA Conversation Simulator project, you will need Visual Studio 2017 + UWP SDK.
2.  Already built app package is also provided in releases section. Download, extract and run the .ps1 script as admin using PowerShell. That will install the Simulator into your system as a Windows 10 App. *Developer mode* should be turned on on your PC.
3.  Open the app by searching in the Start Menu or from ANA Conversation Studio->Tools->Launch ANA Conversation Simulator.
4.  First time of the launch, you will be asked for the Chat and UploadFile APIs. Edit it to match the URL of your hosted ANA Conversation Platform.
5.  If everything went as it should, your chat should have started and you should see the content of the first node -> first section.
6.  If you choose to build the app instead of using the built package, obtain your own MapToken required by the UWP Map Control from your developer.microsoft.com account and place it in Config-Sample.json. Then rename the file to Config.json and build the project.

**Creating your first flow**

[![ProjectANA Sample Chat Flow](https://s3-ap-southeast-1.amazonaws.com/nfcontent-cdn/public-assets/github/ProjectANA/ProjectANA-Sample-Chat-Flow.gif)](https://s3-ap-southeast-1.amazonaws.com/nfcontent-cdn/public-assets/github/ProjectANA/ProjectANA-Sample-Chat-Flow.gif)

## Note

This is just the setup documentation. ANA Conversation Platform has much more scope than predefined flows. You can call your own APIs from the chat nodes. Create and store variables at the mobile client app, use them to take
decisions at the server and direct the user to respected nodes. You can take different types of inputs from the user like button click, text, image, video and act on them at the server side. For now, explore the code to get to know all the possibilities.

Detailed documentation is on its way.

## License

ANA Conversation Suite is available under the [GNU GPLv3 license](https://www.gnu.org/licenses/gpl-3.0.en.html).

