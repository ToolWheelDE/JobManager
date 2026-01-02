#!/bin/bash

rm *.sln
rm *.slnx

find "./source" -type f -name "*.sln" -exec rm -v {} \;
find "./source" -type f -name "*.slnx" -exec rm -v {} \;

./Contrib/pjr2sln.sh ./ToolWheel.slnx -Path=./Source
#./Contrib/pjr2sln.sh ./ToolWheel.Applications.slnx -Path=./Source/applications
./Contrib/pjr2sln.sh ./ToolWheel.Extensions.slnx -Path=./Source/extensions
./Contrib/pjr2sln.sh ./ToolWheel.Extensions.JobManager.slnx -Path=./Source/extensions/JobManager

rm *.sln
