#/bin/sh
 
# World Bank RBF
# Created by Engagement Lab, 2015
# ==============
#  automate.sh
#  Unity texture import automation from external assets folder

#  Created by Johnny Richardson on 3/30/15.
# ==============

EXTERNAL_ASSET_DIR="Content/Art/"
UNITY_ASSET_DIR="WorldBank-RBF/Assets/Textures/"

# Logging
echo "   " | tee -a automate.log;
echo "============== Started automation from git commit head ($(git rev-parse HEAD)):" | tee -a automate.log;

# Find all .png files in external dir
for f in $(find $EXTERNAL_ASSET_DIR -name "*.png")
do

	# Get parent dir of file
	dir=$(dirname $f)
	dir=${dir#$EXTERNAL_ASSET_DIR}
	# dir=$(basename $dir)

	# Get filename w/o extension
	file_no_ext=${f%.png};
	file_no_ext=${file_no_ext##*/};

	# Create path to move file to
	base_new_path=$UNITY_ASSET_DIR$dir
	new_path="$base_new_path/$file_no_ext.png";

	echo "Moved $f to $new_path" | tee -a automate.log;

	# Create desination dir if missing
	mkdir -p $base_new_path;

	# Move file
	mv $f $new_path;

done

# Logging
echo "DONE" | tee -a automate.log;
echo "\\\==============" | tee -a automate.log;