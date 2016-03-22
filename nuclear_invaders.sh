#!/bin/sh

# nuclear_invaders.sh

# This file is part of Nuclear Invaders
# http://programandala.net/en.program.nuclear_invaders.html

# 2016-03-22

fuse-sdl \
  --speed 100 \
	--machine 128 \
	--no-divide \
	--plusd \
  --plusddisk ./disk_1_solo_forth.mgt \
	$* \
	&

