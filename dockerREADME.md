## Docker Deployment for STNServices workflow

1. Step One: AWS Fargate Repo and Cluster creation
Notes this section: Create the Repo before the cluster. VS does this for you. Or you can do it. 

2. Step Two: AWS EC2 launchpad setup
Notes this section: Why are you launching this from an EC2? The TL;DR is that DOI has mucked the kernel on your host machine. So, an EC2 instance is the work around. T2.large will do just fine. You need to pick windows server 2016 version 1709 or greater because server 2012 R2 doesn't have a current enough version Hyper-V (which is the windows garbage version of a hypervisor) and only 2016 v 1709 allows you to switch to linux containers. Additionally, you need to get an AMI with docker onboarded. Skip to end if you don't want to know the why. EC2 is basically a giant container, therefore,  the hypervisor won't let the pseudo OS alter the kernel because... there is no kernel. Or at least you can't get to it. It is worth mentioning at this point that we are not building a container on an EC2 instance. I don't even think that could work. We are just gen-ing the docker image. We actually only need docker tools on the docker command line. But, this is windows and containers run on linux so, you need a whole framework. We won't use most of it but it's windows, you have to buy the whole car even if you just need a tire. So two solves here will probably work, 1. Build a cross compiler. No fucking thank you to that. 2. build an image off instance with docker installed and then use this as the AMI. This works, and there is just such an instance in the marketplace (As of writing this Windows server 2019 w/ docker).   
3. Step Three: Visual Studio Install and setup
Notes for this part: Straight forward. Make sure you install every version of donet.core and apsnet from 2.1 to 3.1 to give yourself options.
4. Step Four:GitHub Repo pull down
Notes for this: You obviously need something to run in the kernel. This is that something. DO NOT ADD DB PASSWORD INFO.
5. Step Five: VS extension AWS Toolbox
Notes For this section: This is some glitchy crap and we will eventually not want to use it. For now, this will not install until all msbuild.exe processes have been terminated. You may need to dig them out in task manager.
6. Step Six: Docker file
Notes for this part: This is maybe not a step?? Maybe. Looks like it's not a thing but it is a black box and needs to be understood better.
7. Step Seven: Docker support file and switch Docker to linux containers
Notes for this part: Once everything above is done, click docker support and it will get some .dockerignore and other files. Got to the docker icon on and right click hidden icons on the windows toolbar and select switch to linux containers.
8. Step Eight: Headless IAM USER Daemon Creation
Notes for this part: So Zivaro, our cloud overlords make a bunch of rules that clog the dev process. One big clog was a clumsy execution of MFA. To get around this you need an IAM USER that is headless that can function as a daemon for machine actions. Z will make you one if there isn't one. It's basically a user that is an admin but doesn't have MFA.
9. Step Nine: VS AWS profile creation
Notes for this part: You need the daemon credential CSV.
10. Step Ten: Image launch specs
Notes for this part: Change the appsettings.json file to define a db connect. Exclude all 'https://' and trailing '/' on the url for the database.
11. Step Eleven: LAUNCH!!!!!!!!
12. Step Twelve: Update GitHub 
Notes for this part: REMOVE ALL PASWORD AND USER NAME INFO. Add, commit and push to a test branch on WIMgit
13. Step Thirteen: Security groups
Notes for this part: Set http ports 80 and 443
14. Step Fourteen: Hook up to STN 
Notes for this part: Just starting this.pull origin