# The Ana App

[![License: GPL v3](https://img.shields.io/badge/License-GPL%20v3-blue.svg)](http://www.gnu.org/licenses/gpl-3.0)

A single cross-platform app where you can design and publish ANA chatbots, manage users and check the chatbot analytics.

## License

ANA Conversation Suite is available under the [GNU GPLv3 license](https://www.gnu.org/licenses/gpl-3.0.en.html).

## Installation

Download ANA studio sdk from https://www.ana.chat/downloads.html

## Creating a bot

1. Ensure ANA server is up and running (Instructions for [ANA server](https://github.com/Kitsune-tools/ProjectANA/blob/master/ANA-CHAT-SERVER-SETUP-README.MD) setup)
2. Open ANA studio and configure ANA server url
    1. Click on `login` 
    2. Click on `Add ANA chat server connection`
    3. If doesn't find any saved ANA chat server connections, prompts to add new server connection, Click on `Add ANA chat server connection`
    3. Click on the drop down, enter server name and ANA api-gateway url (ex: http://localhost:8080) 
    4. Save changes
    5. Click on login, select recently added ANA server connection and enter username as 'Admin' and password as 'ana123' an submit
    6. Now we have integrated ANA studio with ANA server successfully. Next we need to create a business and submit flow to ANA server
 3. Now open the studio from home page and create a new project and publish chat flow or import an existing chatflow
