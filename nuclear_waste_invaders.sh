#!/bin/sh

# nuclear_waste_invaders.sh

# This file is part of Nuclear Waste Invaders
# http://programandala.net/en.program.nuclear_waste_invaders.html

# 2016-03-22
# 2017-03-11: Update project name.

fuse-sdl \
  --speed 100 \
	--machine 128 \
	--no-divide \
	--plusd \
  --plusddisk ./disk_1_solo_forth.mgt \
	$* \
	&

