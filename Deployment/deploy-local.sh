#!/bin/bash

#########################################################
# IMPORTANT! Change this section before run				#
SrcDir=../TR.OGT.ChangeLedger
#SrcDir=../TR.OGT.QueryExtractService
ImageName=cl-app
#ImageName=qes-app
# AWS variables
UserName=MGMT\\MC277876
Password=Xfq9Wb4TFMsW6Fuo
AWSRole=1
Ec2IpAddress=10.0.134.188
#########################################################

# image variables 
ImageArchFileName=$ImageName.tar
ImageArchFileFullName=$SrcDir/$ImageArchFileName

# deploy script to be run on EC2 to deploy docker image and run contatiner
EC2DeployFileName=./deploy-ec2.sh

DockerEnvValuesFileName=./$ImageName"-env.list"

# color setup
RED='\033[0;31m'
NORMAL='\033[0;32m'
NC='\033[0m' # No Color

# set stop execution if any error
set -e

# login with could tool
printf "${NORMAL}"
read -p "Would youe like to login to AWS with 'cloud-tool'?(Y|N): " -n 1 -r
printf "${NC}\n"
if [[ $REPLY =~ ^[Yy]$ ]]
then
cloud-tool --region us-east-1 login << ANSWERS
$UserName
$Password
$AWSRole
ANSWERS
fi

printf "${NC}\n"

# delete archive from src folder
if test -f "$ImageArchFileFullName"; then
	printf "\n${NORMAL}Delete existing image archive '${ImageArchFileFullName}'${NC}\n"
    rm $ImageArchFileFullName	
	printf "${NORMAL}The image archive '${ImageArchFileName}' has been deleted${NC}\n\n"
fi

# build docker image
printf "\n${NORMAL}Start building a docker image '$ImageName'${NC}\n"
docker build -t $ImageName $SrcDir/.
printf "\n${NORMAL}The docker image '$ImageName' has been built${NC}\n\n"

# save docker image as archive
printf "\n${NORMAL}Save the docker image as an archive to '$ImageArchFileFullName'${NC}\n"
docker save -o $ImageArchFileFullName $ImageName
printf "${NORMAL}The docker image '$ImageName' has been saved as the archive '$ImageArchFileName'${NC}\n\n"

# copy deployment file to ec2
read -p "Would youe like to copy deployment file '$EC2DeployFileName' to AWS EC2?(Y|N): " -n 1 -r
printf "${NC}\n"
if [[ $REPLY =~ ^[Yy]$ ]]
then
	if test -f "$EC2DeployFileName"; 
	then
		printf "\n${NORMAL}Copying image deployment script '$EC2DeployFileName' to AWS EC2${NC}\n"
		cloud-tool copy-files --copy-to-ec2 --private-ip $Ec2IpAddress $EC2DeployFileName $EC2DeployFileName
		printf "${NORMAL}Image deployment script '$EC2DeployFileName' has been copied to AWS EC2c\n\n"
	fi
fi

# copy env file to ec2
if test -f "$DockerEnvValuesFileName"; 
then
	printf "\n${NORMAL}Copying env file '$DockerEnvValuesFileName' to AWS EC2${NC}\n"
	cloud-tool copy-files --copy-to-ec2 --private-ip $Ec2IpAddress $DockerEnvValuesFileName $DockerEnvValuesFileName
	printf "${NORMAL}Env files '$DockerEnvValuesFileName' has been copied as $ImageName-env.list to AWS EC2c\n\n"
fi

# copy archive to ec2
if test -f "$ImageArchFileFullName"; 
then
	printf "\n${NORMAL}Copying the docker image archive '$ImageArchFileName' to AWS EC2${NC}\n"
	cloud-tool copy-files --copy-to-ec2 --private-ip $Ec2IpAddress $ImageArchFileFullName $ImageArchFileName
	printf "${NORMAL}The docker image archive '$ImageArchFileName' has been copied to AWS EC2${NC}\n\n"

	# connect to ec2
	printf "\n${NORMAL}Connect via SSH to EC2 with IP '$Ec2IpAddress'${NC}\n"
	cloud-tool ssh --private-ip $Ec2IpAddress
else
	printf "\n${RED}Image archive file '$ImageArchFileName' was not found${NC}\n"
fi
