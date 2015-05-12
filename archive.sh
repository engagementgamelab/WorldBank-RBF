#/bin/sh
 
# World Bank RBF
# Created by Engagement Lab, 2015
# ==============
#  archive.sh
#  Archive the build.

#  Created by Johnny Richardson on 5/6/15.
# ==============

WORKSPACE=$1;
OUTPUT_NAME=$2;
BUILD_NUM=$3;
EXTERNAL_BUILDS_DIR="/Library/BuildArtifacts";

output_dir="$WORKSPACE/WorldBank-RBF/Output"
target_tar="$EXTERNAL_BUILDS_DIR/$OUTPUT_NAME.tgz";

# Create tar of latest build with a readme
echo "Build is from most recent successful Jenkins build ($BUILD_NUM)" > "$output_dir/readme.txt";

cd $WORKSPACE/WorldBank-RBF/Output && tar -czf $target_tar .;