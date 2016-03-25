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

# $? list of dependencies changed more recently than current target
# $@ name of current target
# $< name of current dependency

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


main_source_file=src/nuclear_invaders.fsb
secondary_source_files=$(sort $(wildcard src/00*.fsb))
library_source_files=$(sort $(wildcard src/lib/*.fsb))

source_files = \
	$(main_source_file)\ $(secondary_source_files)\ $(library_source_files)

tmp/disk_2_nuclear_invaders.fsb: \
	$(secondary_source_files) \
	$(library_source_files) \
	$(main_source_file)
	cat \
		$(secondary_source_files) \
		$(library_source_files) \
		$(main_source_file) \
		> tmp/disk_2_nuclear_invaders.fsb

disk_2_nuclear_invaders.mgt: tmp/disk_2_nuclear_invaders.fsb
	fsb2-mgt tmp/disk_2_nuclear_invaders.fsb ;\
	mv tmp/disk_2_nuclear_invaders.mgt .

