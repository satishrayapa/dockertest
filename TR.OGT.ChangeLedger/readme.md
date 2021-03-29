# how to manually deploy code to ec2 instance

## Build docker image
1. Copy tr.ogt.common library to DockerNuGet folder
- tr.ogt.common library can be found "C:\Users\Some_Name\.nuget\packages"
- this TR library is used to build dotnet solution. Either provide access to TR nuget repo and specify credentials or manually place this library here.
- NuGet.config file is used to specify the path to this library at the build time.
2. run "docker build -t my-app ."

## Copy image to ec2 instance
3. save built image to tar archive, run "docker save -o myapp.tar my-app"
4. upload archive to ec2 instance
- if you are using Windows open git bash
- run "cloud-tool login"
- run "cloud-tool copy-files -t -n a205822-contentloader-asg "/c/Users/Some_Name/Desktop/myapp.tar" "/some-dir/myapp.tar""
- (-t is short for --copy-to-ec2 and -n is short for --name, a205822-contentloader-asg is the name of ec2 instance)

## run docker image on ec2
5. in the same git bash app run "cloud-tool ssh -n a205822-contentloader-asg" to log in to ec2 instance
6. cd to destination folder where you copied archive (4th step "/some-dir/myapp.tar")
7. install docker if not yet, follow https://serverfault.com/a/852298
8. run "docker load --input myapp.tar"
9. run "docker image ls" to list all docker images


## additional notes
- to avoid manually writing profile name and region when using cloud tool set default values, run "cloud-tool configure"
- to access ec2 instance use either name (-n some-name) or ip (--private-ip 0.0.0.0)
- to find name of ec2 instance run "cloud-tool list instances"

