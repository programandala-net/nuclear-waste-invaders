#!/bin/sh

# nuclear_waste_invaders.sh

# This file is part of Nuclear Waste Invaders
# http://programandala.net/en.program.nuclear_waste_invaders.html

# ==============================================================
# Description

# This file runs the Fuse emulator with the Solo Forth boot disk
# image and the graphics tape of Nuclear Waste Invaders, in
# order to compile this game.

# ==============================================================
# Instructions

# 1) Boot Solo Forth using 128 BASIC's `run`.
#
# 2) Insert Nuclear Waste Invaders' disk image into Plus D's
# drive 2.
#
# 3) Type `1 load` in Solo Forth.

# ==============================================================

fuse-xlib \
  --speed 100 \
	--machine 128 \
  --graphics-filter 3x \
	--no-divide \
	--plusd \
  --plusddisk ./disk_1_solo_forth.mgt \
  --tape ./graphics_and_font.tap \
	$* \
	&

# ==============================================================
# Change log

# 2016-03-22: Start.
#
# 2017-03-11: Update project name.
#
# 2018-01-21: Add tape image file. Document. Use Xlib Fuse
# instead of SDL Fuse.

# vim: textwidth=64
