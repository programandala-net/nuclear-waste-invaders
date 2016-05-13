# Nuclear Invaders Makefile

# Author: Marcos Cruz (programandala.net), 2016

# This file is part of Nuclear Invaders
# http://programandala.net/en.program.nuclear_invaders.html

################################################################
# Requirements

# cat (from the GNU coreutils)

# fsb2 (by Marcos Cruz)
# 	http://programandala.net/en.program.fsb2.html

# mkmgt (by Marcos Cruz)
# 	http://programandala.net/en.program.mkmgt.html

################################################################
# History

# 2016-03-22: First version, based on the Makefile of Solo Forth.
# 2016-03-23: Adapted to the new splitted library of Solo Forth.

################################################################
# Notes

# $^ list of all prerequisites
# $? list of prerequisites changed more recently than current target
# $< name of first prerequisite
# $@ name of current target

################################################################
# Config

VPATH = ./

MAKEFLAGS = --no-print-directory

.ONESHELL:

################################################################
# Main

.PHONY: all
all: disk_2_nuclear_invaders.mgt

.PHONY : clean
clean:
	rm -f tmp/*
	rm -f disk_2_nuclear_invaders.mgt

secondary_source_files=$(sort $(wildcard src/00*.fsb))
library_source_files=$(sort $(wildcard src/lib/*.fsb))

tmp/nuclear_invaders.fb: src/nuclear_invaders.fs
	./bin/fs2fb-section.sh $<
	mv $(basename $<).fb $@

tmp/library.fsb: \
	$(secondary_source_files) \
	$(library_source_files)
	cat $^ > $@

tmp/library.fb: tmp/library.fsb
	fsb2 $<
	mv $<.fb $@

tmp/disk_2_nuclear_invaders.fb: \
	tmp/library.fb \
	tmp/nuclear_invaders.fb
	cat $^ > $@

disk_2_nuclear_invaders.mgt: tmp/disk_2_nuclear_invaders.fb
	cp $< $<.copy
	bin/fb2mgt.sh tmp/disk_2_nuclear_invaders.fb
	mv tmp/disk_2_nuclear_invaders.mgt .
	mv $<.copy $<
