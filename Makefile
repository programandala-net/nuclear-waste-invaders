# Nuclear Invaders Makefile

# Author: Marcos Cruz (programandala.net), 2016, 2017

# This file is part of Nuclear Invaders
# http://programandala.net/en.program.nuclear_invaders.html

# Last modified: 201702272252

# ==============================================================
# Requirements

# cat (from the GNU coreutils)

# fsb2 (by Marcos Cruz)
# 	http://programandala.net/en.program.fsb2.html

# mkmgt (by Marcos Cruz)
# 	http://programandala.net/en.program.mkmgt.html

# ==============================================================
# History

# 2016-03-22: First version, based on the Makefile of Solo Forth.
#
# 2016-03-23: Adapted to the new splitted library of Solo Forth.
#
# 2016-05-13: Modified after the new format of the main source.
#
# 2017-02-27: Update after the changes in Solo Forth (the filename extension
# "fs" is used instead of "fsb") and fsb2 (the filename extension of the input
# file is not preserved).

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
all: disk_2_nuclear_invaders.mgt

.PHONY : clean
clean:
	rm -f tmp/*
	rm -f disk_2_nuclear_invaders.mgt

secondary_source_files=$(sort $(wildcard src/00*.fs))
library_source_files=$(sort $(wildcard src/lib/*.fs))

tmp/nuclear_invaders.fba: src/nuclear_invaders.fs
	./make/fs2fba.sh $<
	mv $(basename $<).fba $@

tmp/library.fs: \
	$(secondary_source_files) \
	$(library_source_files)
	cat $^ > $@

tmp/library.fb: tmp/library.fs
	fsb2 $<

tmp/disk_2_nuclear_invaders.fb: \
	tmp/library.fb \
	tmp/nuclear_invaders.fba
	cat $^ > $@

disk_2_nuclear_invaders.mgt: tmp/disk_2_nuclear_invaders.fb
	cp $< $<.copy
	make/fb2mgt.sh tmp/disk_2_nuclear_invaders.fb
	mv tmp/disk_2_nuclear_invaders.mgt .
	mv $<.copy $<
