#!/bin/bash

# color setup
RED='\033[0;31m'
NORMAL='\033[0;32m'
NC='\033[0m' # No Color

# read image name for input
echo -e "${NORMAL}Please specify image name (e.g.: cl-app, qes-app): ${NC}"
read ImageName
ImageArchFileName=$ImageName.tar

# stop / remove container
if [[ $(docker ps -a -q --filter ancestor="$ImageName" --format="{{.ID}}") != '' ]]; then
	dockerId=$(docker ps -a -q --filter ancestor="$ImageName" --format="{{.ID}}")
	printf "\n${NORMAL}Trying to stop and delete the docker container '${ImageName}' with CONTAINER ID=${dockerId}${NC}\n"
	docker container rm $(docker stop $dockerId)
	printf "\n${NORMAL}The the docker container '${ImageName}' has been stopped and deleted${NC}\n"
fi

# remove image
if [[ $(docker images $ImageName -a -q) != '' ]]; then
	imageId=$(docker images $ImageName -a -q)
	printf "\n${NORMAL}Trying to remove the docker image '${ImageName}' with IMAGE ID=${imageId}${NC}\n"
	docker rmi $imageId
	printf "\n${NORMAL}The the docker image '${ImageName}' has been deleted${NC}\n"
fi

# load image from archive
if test -f "$ImageArchFileName"; 
then
	printf "\n${NORMAL}Loading the docker image from the archive '$ImageArchFileName'${NC}\n"
	docker load --input $ImageArchFileName
	printf "${NORMAL}The docker image has been loaded from the archive '$ImageArchFileName'\n\n"
else
	printf "\n${RED}Image archive file '$ImageArchFileName' was not found${NC}\n"
fi

# start docker container whether with configs or not
printf "\n${NORMAL}Trying to start docker container '${ImageName}'${NC}\n"
if test -f $ImageName"-env.list"; then
    docker run --env-file ./$ImageName"-env.list" -d $ImageName
else
    docker run -d $ImageName
fi

# list all contaners
docker ps -a

printf "\n${NORMAL}DONE!${NC}\n"