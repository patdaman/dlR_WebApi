@ECHO OFF
SET ProjectPath=%~dp0

REM **************************
REM **  User Edit Variables **
REM **************************

REM ** Options
REM ** DEBUG		
REM ** RELEASE

REM		\/-- Set Up Here --\/
SET CONFIGURATION=RELEASE
SET TargetComputerName=SG-CA01-DVM-002
SET ProjectName=SignalApi
SET Flavor=BETA
REM SET DeployUser=sgnl\devpublish <-- Not an admin for publishing this way!
REM SET Password+********************
SET DeployUser=sgnl\pdelosreyes
SET Password=May2016!
REM		/\-- Set Up Here --/\
REM **************************
REM * End User Edit Variables*
REM **************************

ECHO %USERNAME% at %Date% %Time%

@ECHO OFF
REM pause

REM Set the Path to MSBuild.exe
pushd "%ProgramFiles(x86)%\MSBuild\14.0\Bin"
SET MSBuild.exe=MSBuild.exe

@ECHO ON
%MSBuild.exe% "%ProjectPath%\%ProjectName%.csproj" /p:Configuration=%CONFIGURATION% /t:Clean
REM ECHO PROJECT CLEAN COMPLETE
@ECHO OFF
REM pause

@ECHO ON
REM Project being compiled and built 
%MSBuild.exe% "%ProjectPath%\%ProjectName%.csproj" /p:Configuration=%CONFIGURATION% /t:Build /p:Flavor=%Flavor% /t:SlowCheetah
ECHO Project Compiled 
@ECHO OFF
REM pause

@ECHO ON
REM Project being deployed
%MSBuild.exe% "%ProjectPath%\%ProjectName%.csproj" /p:Configuration=%CONFIGURATION% /p:DeployOnBuild=true /p:CreatePackageOnPublish=True /t:Package /p:PublishProfile=%ProjectPath%Properties\PublishProfiles\App_%Flavor%.pubxml /p:Password=5Bbubble#$R /p:VisualStudioVersion=14.0
ECHO DEPLOY PACKAGE CREATED
@ECHO OFF
REM pause

pushd "%ProjectPath%\Obj\%CONFIGURATION%\Package"

@ECHO ON
%ProjectName%.deploy.cmd /Y /M:%TargetComputerName% /U:%DeployUser% /P:%Password%
ECHO DEPLOYED

pushd "%ProjectName%"