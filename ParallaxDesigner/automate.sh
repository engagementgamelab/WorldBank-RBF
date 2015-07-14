#/bin/sh
 
# World Bank RBF
# Created by Engagement Lab, 2015
# ==============
#  automate.sh
#  Unity texture import automation from external assets folder

#  Created by Johnny Richardson on 3/30/15.
# ==============

EXTERNAL_ASSET_DIR=$2"Cities"
UNITY_ASSET_DIR=$2"TiledCities"

# Clear prior log
> automate.log;

# Logging
echo "   " | tee -a automate.log;
echo "==============" | tee -a automate.log;
echo "$(date +"%D %H-%M")" | tee -a automate.log;
echo "==============" | tee -a automate.log;

# Find all .png files in external dir
for f in $(find $EXTERNAL_ASSET_DIR -name "*.png")
do

	# Get parent dir of file
	dir=$(dirname $f)
	dir=${dir#$EXTERNAL_ASSET_DIR}

	# Get filename w/o extension
	file_no_ext=${f%.png};
	file_no_ext=${file_no_ext##*/};

	# Create path to move file to
	base_new_path="$UNITY_ASSET_DIR$dir";
	new_path="$base_new_path/$file_no_ext/$file_no_ext.png";

	# Create desination dir if missing
	mkdir -p $base_new_path/$file_no_ext;

	if [[ $file_no_ext == *"layer"* ]] && [[ ${#file_no_ext} == 6 ]]; then
		convert $f -crop 2048x2048 $new_path;
		echo "Split '$f' and moved tiles to $new_path" | tee -a automate.log;
	fi

done  

wait

# Logging
echo "DONE" | tee -a automate.log;
echo "\\\==============" | tee -a automate.log;
