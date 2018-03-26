# ANA Chat Server Setup Guide

ANA Server is required to publish chat flows and serve published content across differant channels including Web, Android, iOS and Facebook (and many more to come soon). ANA server is decomposed into microservices, organized around certain business domains.

Following are the required microservices to run ANA server

1) service-registry
2) config-server
3) api-gateway
4) ws-customers
5) business-service
6) user-service
7) core
8) history service (optional)
9) fcm-plugin (optional)
10) agents-service (optional)
11) file-service (optional)
12) analytics (optional)

## Installation

### Development mode

**System Requirements**

 1. Ubuntu Machine (Will be windows desktop/server and mac compatible very soon)
 2. 8GB+ RAM
 3. 2+ CPU Cores
 4. 10 GB+ free disk space

**Software Requirements**

 1. Docker and docker compose installed.
   - Click [here](https://docs.docker.com/engine/installation/linux/docker-ce/ubuntu/) for docker installation guidelines
   - Click [here](https://docs.docker.com/compose/install/) for docker-compose installation guidelines

#### Setup Steps
 1. Download the docker-compose.yml file using the following command
     ```bash
     wget https://gist.githubusercontent.com/menneni/70ea1a174961f653cd62c2ae3b22ec1b/raw/docker-compose.yml
     ```
 3. Pull and fire up the dockers using following commands
    ```bash
    docker-compose pull
    docker-compose build
    docker-compose up -d
#### Important endpoints
 - http://localhost:80   - eureka dashboard/service-registry
 - http://localhost:8080 - api-gateway
 - http://localhost:8888 - config-server
 - http://localhost:9500 - core
 - http://localhost:8088 - ws-customers
 - http://localhost:8087 - business-service
 - http://localhost:8089 - user-service
    
 #### Notes
 
 Services running with spring boot requires already running service-registry and config-server for startup. docker-compose starts all microservices simultaniously but ensures dependencies using `depends_on` and `health check` option.
 
 Health check for core service - http://localhost:9500/bot/health
 
 Also service discovery mechanism needs some time after all applications startup. To ensure all required microservices for ANA server are up and running, check eureka dashboard (http://localhost:80), it should show all the required microservices instances in the eureka dashboard.  
    
