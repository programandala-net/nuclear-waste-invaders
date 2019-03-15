# Makefile

# Author: Marcos Cruz (programandala.net), 2016, 2017, 2019.

# This file is part of Nuclear Waste Invaders
# http://programandala.net/en.program.nuclear_waste_invaders.html

# Last modified: 201903151338
# See change log at the end of the file

# ==============================================================
# Requirements

# bin2code (by Metalbrain)
# 	http://metalbrain.speccy.org/link-eng.htm

# cat (from the GNU coreutils)

# fsb2 (by Marcos Cruz)
# 	http://programandala.net/en.program.fsb2.html

# Gforth (by Anton Ertl, Bernd Paysan et al.)
# 	http://gnu.org/software/gforth

# split (from the GNU coreutils)

# ZX7 (by Einar Saukas et al.)
#	http://www.worldofspectrum.org/infoseekid.cgi?id=0027996

# ==============================================================
# Notes

# $^ list of all prerequisites
# $? list of prerequisites changed more recently than current target
# $< name of first prerequisite
# $@ name of current target

# ==============================================================
# Config

VPATH = ./

MAKEFLAGS = --no-print-directory

.ONESHELL:

# ==============================================================
# Main

.PHONY: all
all: disk_2_nuclear_waste_invaders.mgt graphics_and_font.tap

.PHONY : clean
clean:
	rm -f tmp/*
	rm -f disk_2_nuclear_waste_invaders.mgt *.tap

# ==============================================================
# Source block disk

secondary_source_files=$(sort $(wildcard src/00*.fs))
library_source_files=$(sort $(wildcard src/lib/*.fs))

tmp/nuclear_waste_invaders_converted_to_zx_spectrum_charset.fs: src/nuclear_waste_invaders.fs
	cp $< $@
	vim -S ./make/utf8_to_zx_spectrum.vim \
		-c "set fileencoding=latin1" \
		-c "wq" $@

%.fba: %.fs
	./make/fs2fba.sh $<

tmp/library.fs: \
	$(secondary_source_files) \
	$(library_source_files)
	cat $^ > $@

tmp/library.fb: tmp/library.fs
	fsb2 $<

tmp/disk_2_nuclear_waste_invaders.fb: \
	tmp/library.fb \
	tmp/nuclear_waste_invaders_converted_to_zx_spectrum_charset.fba
	cat $^ > $@

disk_2_nuclear_waste_invaders.mgt: tmp/disk_2_nuclear_waste_invaders.fb
	cp $< $<.copy
	make/fb2mgt.sh $<
	mv $(basename $<).mgt .
	mv $<.copy $<

# ==============================================================
# Tape

# ----------------------------------------------
# Font

tmp/font.bin: fonts/chato.fs
	gforth $< > $@

tmp/font.tap: tmp/font.bin
	make/bin2code0 $< $@

# ----------------------------------------------
# Landscape graphics

landscapes_scr=$(wildcard graphics/landscapes/*.scr)

# ..............................
# Uncompressed graphics

# XXX OLD - First version

# landscapes_scr_tap=$(addsuffix .tap,$(landscapes_scr))

# # Create a TAP file from a SCR file:
# %.scr.tap: %.scr
# 	make/bin2code0 $< $@ 16384

# # Create the final TAP file with all SCR files:
# landscapes.uncompressed.tap: $(landscapes_scr_tap)
# 	cat $(sort $^) > $@

# ..............................
# Compressed graphics

landscapes_scr_3rd=$(addsuffix .3rd,$(landscapes_scr))

landscapes_scr_3rd_attr=$(addsuffix .attr,$(landscapes_scr_3rd))

landscapes_scr_zx7=\
	$(addsuffix .zx7,$(landscapes_scr_3rd) $(landscapes_scr_3rd_attr))

landscapes_scr_3rd_tap=$(addsuffix .tap,$(landscapes_scr_zx7))

# Extract the last third of the bitmap from a SCR file:
%.scr.3rd: %.scr
	split --bytes=2048 --numeric-suffixes $< $@
	mv $@02 $@
	rm -f $@??

# Extract the last third of the attributes from a SCR file:
%.scr.3rd.attr: %.scr
	split --bytes=256 --numeric-suffixes $< $@
	mv $@26 $@
	rm -f $@??

# Create a TAP file from a ZX7 file:
%.zx7.tap: %.zx7
	make/bin2code0 $< $@

# Compress a file with ZX7:
%.zx7: %
	zx7 $<

# ----------------------------------------------
# Tape

graphics_and_font.tap: $(landscapes_scr_3rd_tap) tmp/font.tap
	cat $(sort $^) > $@

# ==============================================================
# Change log

# 2016-03-22: First version, based on the Makefile of Solo Forth.
#
# 2016-03-23: Adapted to the new splitted library of Solo Forth.
#
# 2016-05-13: Modified after the new format of the main source.
#
# 2017-02-27: Update after the changes in Solo Forth (the filename extension
# "fs" is used instead of "fsb") and fsb2 (the filename extension of the input
# file is not preserved).
#
# 2017-03-11: Update project name.
#
# 2017-03-13: Move the graphics tape to the root directory and sort its files.
#
# 2017-03-22: Compress the landscape graphics.
#
# 2017-04-18: Convert UTF-8 characters to ZX Spectrum character codes.
#
# 2017-04-20: Add font to the tape.
#
# 2019-03-15: Generalize the rule of fs2fba.sh.
