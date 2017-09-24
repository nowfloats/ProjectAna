# Project ANA - Open Source Conversation Suite

[![License: GPL v3](https://img.shields.io/badge/License-GPL%20v3-blue.svg)](http://www.gnu.org/licenses/gpl-3.0)

World's first **Open Source Conversation Platform** which comes with a **Graphical Chat Flow Designer** and **Chat Simulator**.
Android & iOS SDK available on request. 

**Launching soon:**
 
Web & Messenger SDK

----------

[![Studio and Simulator Image](https://github.com/Kitsune-tools/ProjectANA/blob/master/Assets/anaflow.png)](https://github.com/Kitsune-tools/ProjectANA/blob/master/Assets/anaflow.png)

----------



## Components Overview

**ANA Conversation Suite contains 3 components:**

1.  **Platform:** Backend API which serves the chat requests. This component is responsible for merging the chat flow template with the chat content and returning the flow when requested by the clients.
2.  **Studio:** Graphical designer of the chat flow. This can be helpful when designing the chat flow and reduces the amount of unnecessary effort needed down by 80%. This also enables non-devs to edit the flow and it's content at any given point of time, changes being reflected instantly.
3.  **Simulator:** Used to test and experience the chat flow.

## Installation

**Pre-requisites:**

1.  Windows/Linux/Mac to host the Platform API
2.  Mongo DB
3.  Windows PC with .NET Framework 4.5 to run Conversation Studio
4.  Windows 10 PC to use Conversation Simulator (UWP App)

**Setup Platform:**

1.  Download the pre-built binary from [releases](https://github.com/Kitsune-tools/ProjectANA/releases/latest) or Clone the repo. <br> You can also build from the docker image hosted on dockerhub. Click [here](https://hub.docker.com/r/projectana/platform/) to see the instructions on Project ANA's dockerhub page.

2. You need to set up your Mongo DB instance first. You can either do that on your local machine or get one from a Database-as-a-Service provider like [MLab](https://mlab.com). <br> Click [here](https://docs.mongodb.com/manual/installation) for instructions on installation and setup of Mongo DB.<br>

3. In ANA Conversation Platform project, open `appsettings-Sample.json` and edit `DatabaseConnectionSettings` section to match your Mongo DB details you had configured earlier.

4. Change collection names to new/empty collections.
5. Save and rename appsettings-Sample.json to appsettings.json.

6. To build the project, you need to have .NET Core 2.0 SDK installed in your system. Click [here](https://www.microsoft.com/net/download/core) to get the latest version. Note: .NET Core is cross platform and will work on windows, linux and mac.

7. If you cloned the repository, follow the usual steps to restore, build and run a .NET Core project using `dotnet` cli or Visual Studio.

8. If you downloaded the pre built binaries, open the platform folder and find the `ANAConversationPlatform.dll` file. <br> Run `dotnet ANAConversationPlatform.dll` to start the Platform API server.

9.  Note down the base URL of your hosted Platform API server (displayed in the console if started using the above cmd).
10. Optionally, set the `APIKey` and `APISecret` in `appsettings.json` under `BasicAuth` section for security.

**Setup Studio:**

1.  Download the binary directly from [releases](https://github.com/Kitsune-tools/ProjectANA/releases/latest) or build the ANA Conversation Studio project yourself. Open the exe once done.
2.  Enter the above noted chat server base URL, APIKey and APISecret (if you have set them) in the 'Chat Server Connection Manager' Window which appears, by clicking on 'Add' button present in the top center of the window.
3.  As soon as you are connected to the chat server, your first project will be created and loaded for you. 
4.  To create a sample chat flow, right click on the studio document(white area) and click 'Create new node'.
    1.  Add sections and buttons to it by double clicking it.
    2.  Check the 'Mark as start node' check box of the node. 
3.  Save the flow.

**Setup Simulator:**

1.  Built app package is provided in [releases](https://github.com/Kitsune-tools/ProjectANA/releases/latest). Download, extract and run the .ps1 script as admin using PowerShell. That will install the Simulator into your system as a Windows 10 App. *Developer mode* should be turned on in your PC.
2.  If you choose to build the ANA Conversation Simulator project, you will need Visual Studio 2017 + UWP SDK.
3.  You can launch the flow in the simulator directly from the studio by going to ANA Conversation Studio -> Tools -> Launch Current Chat in Simulator option, or by just pressing the F5 button. This will fill the chat flow API and lets the chat flow start without any manual effort.
4.  If everything went as it should, your chat will start and you will see the content of the first node -> first section.
5.  Additional chat server related configurations can be done in top right menu -> Configure Chat Server.
6.  If you choose to build the app instead of using the built package, obtain your own MapToken required by the UWP Map Control from your developer.microsoft.com account and place it in Config-Sample.json. Then rename the file to Config.json and build the project.
7. **Note:** To use localhost URLs in the Simulator (In case you ran Platform locally), Open command prompt and run `checknetisolation loopbackexempt -a -n=44cabbc3-0a2f-4255-b5bb-b099cd5762f9_hvc7j3s42jt6e`. By default, localhost URLs are blocked in UWP Apps for security, above command will unblock it for only this Simulator App. 
----------

## Agent Chat
The ability to switch from a predefined structured flow to a live chat agent is now part of ANA Platform. This can be used if users are stuck at some point in the chat flow due to lack of clarity or they want some manual assistance.

1. **Pre-requisites**:
 
	*[Rocket Chat](https://rocket.chat/) server*: Rocket Chat is a full-fledge open source chat application. This is used as the agent chat platform here. A running Rocket Chat Server with admin access is needed to configure Agent Chat. Follow their docs and setup your rocket chat server. Note down the server URL, admin username and password.

2. **Agent Chat Setup**:
 
	This process assumes you have already setup ANA without agent chat by following above instructions.

	Setting up agent chat involves the following steps.
 
	1. Setup LiveClientSockets Server:
		1. Deploy the NodeJS project 'LiveClientSocketsManager' (included in the repo) on a server. The server can be same or different than the one ANA Platform is deployed on. 
		2. *Note*: If you are not using IISNode to host it or you are not hosting it in IIS server, make sure the variables 'auth_username' and 'auth_password' are accessible via process.env to the node js code. IISNode does that automatically as these variables are defined in web.config
		3. Once the code is deployed. Make note of two things..
			1. Server URL: http://<your-domain.com>/
			1. Basic Auth Base64:
				1. Which is the base 64 of <auth_username>:<auth_password> of the values you set as in the web.config.
				1. Default is agentchat:agentchat and its Basic Auth Base64 will be 'YWdlbnRjaGF0OmFnZW50Y2hhdA=='
	2. Configure Rocket Chat:
		1. Login to rocket chat with admin user.
		2. Goto /admin/integrations
			1. Create a new outgoing integration
				1. This is basically setting up a web hook to notify ANA about a new message sent from Rocket Chat.
			2. Set the fields of the integration as following
				1. Event Trigger: Message Sent
				2. Enabled: true
				3. Channel: all_direct_messages
				4. URLs: http://<your-ANA-domain.com>/api/AgentChat/CallbackReceivedFromRocketChatServerAsync
				5. Impersonate: False
				6. Post as: [Use any existing user]. This callback will not post anything using the call back response to this 'Post as' does not matter.
		3. Goto /admin/Layout
			1. Inside 'User Interface' section, set everything to true, mainly, 'Use Real Name' option. This will display the actual name of the user in the rocket chat threads. It is picked up from 'PERSON_NAME' variable from Memory Stack of the simulator. Make sure 'PERSON_NAME' is captured in your chat flow before switching to agent chat.
		4. Create few users in rocket chat other than admin. They will be the users which agents have to login with. ANA automatically assigns agents to new chat requests, depending on availability.
		5. Make another user account which will act as the default user to which ANA will assign new incoming agent chat requests if none of the agents are available.
	3. Configure ANA Platform:
		1. Open appsettings-sample.json
		2. Fill in all the new fields present in the json by the ones which you have noted above and move those settings to your existing appsettings.json file.
	4. Setup the simulator:
		1. Install the latest simulator app and open it.
		2. Choose the 'Update APIs' option from the top right options button (…).
		3. Enter the above noted sockets server URL in the specified field.
		4. Update the 'Chat Flow URL' to end with 'HybridChat' instead of 'Chat', which was previously the case.
		4. Click Ok to save.
	5. Trying out Agent Chat:
		1. Login to a user in rocket chat. Make sure the status is online if the user is not the default agent chat user!
		2. Open the simulator app. Assuming you have your own chat flow setup and working, simulator will start the structured chat as usual. After completing 1-2 steps of your chat flow, to switch to an agent, click the 'Agent Chat' option in the top right options menu. 
		3. If everything is working fine, the current chat flow history should come into the rocket chat thread, text input box should appear in the simulator. From now on, Simulator and Rocket Chat user can chat two ways. Whatever typed in the simulator will be sent to the rocket chat user and vice versa. 
		4. Sending the chat flow user (simulator user) back to a structured flow from agent chat is a planned feature coming soon.

----------

## Note

This is just the setup documentation. ANA Conversation Platform has much more scope than predefined flows. You can call your own APIs from the chat nodes. Create and store variables at the mobile client app, use them to take
decisions at the server and direct the user to respected nodes. You can take different types of inputs from the user like button click, text, image, video and act on them at the server side. For now, explore the code to get to know all the possibilities.

Detailed documentation is on its way.

## License

ANA Conversation Suite is available under the [GNU GPLv3 license](https://www.gnu.org/licenses/gpl-3.0.en.html).

