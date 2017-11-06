# ANA Chat Server Setup Guide

## Prerequisites

**System Requirements**

 1. Ubuntu Machine. (Will be windows desktop/server and mac  compatible very soon)
 2. 4GB+ RAM.
 3. 2+ CPU Cores.

**Software Requirements**

 1. Docker with docker compose installed.
   - Click [here](https://docs.docker.com/engine/installation/linux/docker-ce/ubuntu/) for docker installation guidelines
   - Click [here](https://docs.docker.com/compose/install/) for docker-compose installation guidelines  
 2. MySQL Database Server
   - You can install MySQL Database Server locally on the Ubuntu Machine you are about to use. Click [here](https://www.digitalocean.com/community/tutorials/how-to-install-mysql-on-ubuntu-16-04) for instructions on that. 

## Setup Steps
 1. Login into MySQL Server as root and create a user
    ```bash
    mysql -uroot -p<your_root_password>

    mysql> CREATE USER 'anabot'@'%' IDENTIFIED by 'anabot';
    mysql> GRANT ALL PRIVILEGES ON *.* TO 'anabot'@'%';
    ```
    
 2. Download the docker-compose.yml file using the following command.
     ```bash
     wget https://gist.github.com/NizamLZ/4903f0894c390acdfda3a29bd004b38e/raw/ -O docker-compose.yml
     ```
 3. Pull and fire up the dockers using following commands.
    ```bash
    sudo docker-compose pull
    sudo docker-compose up
    ```
    - You should now see fast random logs being dumped onto the console, once every service is up and running the logs will slow down.  
    - To make sure things are running as they should, head over to `http://<public-ip-of-ubuntu-machine>:8761/`. 
        - This should open the Spring Eureka.
        - It should have three applications under 'Instances currently registered with Eureka', namely, API-GATEWAY, CONFIG-SERVER, WS-CUSTOMERS. This means the ANA Chat Server is up and running. 
  4. ANA Chat Server URL is the http URL of the **public IP address** of the machine (`eg: http://10.1.2.3`). Additionally, you can map a domain to this IP and use that as well.
  
Now, since you have got the ANA Chat Server URL, return to the process you were doing before and continue with it.

