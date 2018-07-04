# Project Ana - Open Source Conversation Suite

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/d21b873f8f1440e99d1851ab40a8fd77)](https://app.codacy.com/app/sanudatta11/ProjectAna?utm_source=github.com&utm_medium=referral&utm_content=Kitsune-tools/ProjectAna&utm_campaign=badger)
[![License: GPL v3](https://img.shields.io/badge/License-GPL%20v3-blue.svg)](http://www.gnu.org/licenses/gpl-3.0)
[![Build Status](https://travis-ci.org/Kitsune-tools/Ana-App.svg?branch=travisci)](https://travis-ci.org/Kitsune-tools/Ana-App)
[![N|Solid](https://ci.appveyor.com/api/projects/status/github/Kitsune-tools/ProjectAna?branch=master&svg=true)](https://github.com/Kitsune-tools/ProjectAna)

World's first Open Source Conversation Platform which comes with a Graphical Chat Flow Designer and Chat Simulator. Supported channels are Web Chat, Android, iOS and Facebook Messenger.

## The Ana app

A single cross-platform app where you can design and publish Ana chatbots, manage users and check the chatbot analytics.

## Installation

Download The Ana app from https://www.ana.chat/downloads.html

## How to use Ana app?

The goal of The Ana app is to make it easier to build chat flows without much technical knowledge, it even allows you to create and test the bot while designing, using the simulator. 

First, you need to create your bot using the studio in The Ana app. After finalizing the desired chat flow, you can publish it to The Ana server and integrate it with various channels like [Web](https://github.com/Kitsune-tools/ANAChat-Web), [Android](https://github.com/Kitsune-tools/ANAChat-Android), [iOS](https://github.com/Kitsune-tools/ANAChat-iOS) and Facebook. Ana server is responsible for serving published chatbot across different channels. 

## Creating a bot and testing using the simulator

1. Open Ana app and Click on `Studio`
2. Click on `Add new chatbot` to create a new project or 'Import chatbot' to import an existing project
3. Enter the name of the new project and submit
4. Create the desired chat flow and save it
5. Click on `RUN  CHAT` option to test chat flow locally using the simulator

## Publishing bot to Ana server

1. Ensure Ana server is up and running (Instructions for [Ana server](https://github.com/Kitsune-tools/ProjectANA/blob/master/ANA-CHAT-SERVER-SETUP-README.md) setup)
2. Open Ana studio and configure Ana server URL
    1. Click on `login` 
    2. Click on `Add Ana chat server connection`
    3. If it doesn't find any saved Ana chat server connections, you will be prompted to add new server connection, Click on `Add Ana chat server connection`
    3. Click on the drop-down, enter the server name and Ana api-gateway URL (ex: http://localhost:8080) 
    4. Save changes
    5. Click on login, select recently added Ana server connection and enter username as 'Admin' and password as 'ana123' and submit
    6. Now we have integrated Ana studio with Ana server successfully. Next, we need to create a business and submit flow to Ana server
 3. Now open the studio from home page and create a new project and publish chat flow or import an existing chat flow
 
## Deploying your Ana chatbot

The following channels are supported for deploying your Ana chatbot. Follow the links for instructions.
   1. [Web Chat](https://github.com/Kitsune-tools/ANAChat-Web)
   2. [Android](https://github.com/Kitsune-tools/ANAChat-Android)
   3. [iOS](https://github.com/Kitsune-tools/ANAChat-iOS)
   4. Facebook Messenger (Coming soon...)

## Note

This is just the setup documentation. ANA Conversation Suite has much more scope than predefined flows. You can call your own APIs from the chat, capture and save information from the user. You can take different types of inputs like button clicks, text, image, video etc. For now, explore the code to get to know all the possibilities.

## License

Ana Conversation Suite is available under the [GNU GPLv3 license](https://www.gnu.org/licenses/gpl-3.0.en.html).
