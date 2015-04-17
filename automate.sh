#/bin/sh
 
# World Bank RBF
# Created by Engagement Lab, 2015
# ==============
#  automate.sh
#  Unity texture import automation from external assets folder

#  Created by Johnny Richardson on 3/30/15.
# ==============

EXTERNAL_ASSET_DIR=$2"/Content/Art/"
UNITY_ASSET_DIR=$2"/WorldBank-RBF/Assets/Textures/"

# Clear prior log
> automate.log;

# Logging
echo "   " | tee -a automate.log;
echo "==============" | tee -a automate.log;
echo "$(date +"%D %H-%M")" | tee -a automate.log;
echo "==============" | tee -a automate.log;
echo "Started automation from git commit head ($(git rev-parse HEAD)):" | tee -a automate.log;

# Checkout master branch since we're going to modify it
git checkout master

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
	base_new_path="$UNITY_ASSET_DIR$dir";
	new_path="$base_new_path/$file_no_ext.png";

	# Create desination dir if missing
	mkdir -p $base_new_path;

	# Move file
	mv $f $new_path && git add -N $new_path;
	echo "Moved $f to $new_path" | tee -a automate.log;

done  

wait

git commit -am "Dev Server auto-commit for successful Jenkins build $1";

# Logging
echo "DONE" | tee -a automate.log;
echo "\\\==============" | tee -a automate.log;

# Image tiling; for use later
# -crop 256x256 -set filename:tile "%%[fx:page.x/256]_%%[fx:page.y/256]" +repage +adjoin convert/tile-%%[filename:tile].png