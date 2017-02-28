  \ system_variables.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201702220020

  \ -----------------------------------------------------------
  \ Description

  \ Constants for the system variables.

  \ -----------------------------------------------------------
  \ Author

  \ Marcos Cruz (programandala.net), 2015, 2016, 2017.

  \ -----------------------------------------------------------
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

  \ -----------------------------------------------------------
  \ Latest changes

  \ 2016-04-23: Fix: graphic coordinates variables were not
  \ included in the block title.
  \
  \ 2016-05-01: Add `os-attr-p`, `os-mask-p`, `os-attr-t`,
  \ `os-mask-t`
  \
  \ 2017-01-10: Document all words.
  \
  \ 2017-01-18: Improve documentation of `os-frames`.
  \
  \ 2017-01-31: Add the decimal prefix to addresses, in case
  \ `base` is not decimal.
  \
  \ 2017-02-17: Update cross references.

( os-chars os-chans os-flags2 os-seed os-frames os-udg )

[unneeded] os-chars ?\ #23606 constant os-chars

  \ doc{
  \
  \ os-chars ( -- a )
  \
  \ A constant that returns the address of system variable
  \ CHARS, which holds the bitmap address of character 0 of the
  \ current font (actual characters 32..127). By default this
  \ system variables holds ROM address 15360 ($3C00).
  \
  \ See also: `set-font`, `get-font`, `rom-font`, `os-udg`.
  \
  \ }doc

[unneeded] os-chans ?\ #23631 constant os-chans

  \ doc{
  \
  \ os-chans ( -- a )
  \
  \ A constant that returns the address _a_ of system variable
  \ CHANS, which holds the address of channels data.
  \
  \ }doc

[unneeded] os-flags2 ?\ #23658 constant os-flags2

  \ doc{
  \
  \ os-flags2 ( -- ca )
  \
  \ A constant that returns the address _ca_ of 1-byte system
  \ variable FLAGS2, which holds several flags.
  \
  \ See also: `capslock`.
  \
  \ }doc

[unneeded] os-seed ?\ #23670 constant os-seed

  \ doc{
  \
  \ os-seed ( -- a )
  \
  \ A constant that returns the address _a_ of system variable
  \ SEED, which holds the seed of the BASIC random number
  \ generator.
  \
  \ }doc

[unneeded] os-frames ?\ #23672 constant os-frames

  \ doc{
  \
  \ os-frames ( -- a )
  \
  \ A constant that returns the address _a_ of 3-byte system
  \ variable FRAMES (least significant byte first), which holds
  \ a frame counter incremented every 20 ms.
  \
  \ See also: `frames@`, `frames!`, `reset-frames`, `frames`,
  \ `?frames`.
  \
  \ }doc

[unneeded] os-udg ?\ #23675 constant os-udg

  \ doc{
  \
  \ os-udg ( -- a )
  \
  \ A constant that returns the address _a_ of system variable
  \ UDG, which holds the address of the first character bitmap
  \ of the current User Defined Graphics set (characters
  \ 128..255 or 0..255, depending on the words used to access
  \ them).
  \
  \ See also: `set-udg`, `get-udg`, `os-chars`.
  \
  \ }doc

( os-coords os-coordx os-coordy )

[unneeded] os-coords ?\ #23677 constant os-coords

  \ doc{
  \
  \ os-coords ( -- a )
  \
  \ A constant that returns the address _a_ of 2-byte system
  \ variable COORDS which holds the graphic coordinates of the
  \ last point plotted.
  \
  \ See also: `set-pixel`, `plot`, `os-coordx`, `os-coordy`.
  \
  \ }doc

[unneeded] os-coordx ?\ #23677 constant os-coordx

  \ doc{
  \
  \ os-coordx ( -- ca )
  \
  \ A constant that returns the address _a_ of 1-byte system
  \ variable COORDX which holds the graphic x coordinate of the
  \ last point plotted.
  \
  \ See also: `set-pixel`, `plot`, `os-coords`, `os-coordy`.
  \
  \ }doc

[unneeded] os-coordy ?\ #23678 constant os-coordy

  \ doc{
  \
  \ os-coordy ( -- ca )
  \
  \ A constant that returns the address _a_ of 1-byte system
  \ variable COORDY which holds the graphic y coordinate of the
  \ last point plotted.
  \
  \ See also: `set-pixel`, `plot`, `os-coords`, `os-coordx`.
  \
  \ }doc

( os-attr-p os-mask-p os-attr-t os-mask-t os-p-flag )

[unneeded] os-attr-p ?\ #23693 constant os-attr-p

  \ doc{
  \
  \ os-attr-p ( -- ca )
  \
  \ A constant that returns the address _a_ of 1-byte system
  \ variable ATTR_P, which holds the current permanent color
  \ attribute, as set up by color statements.
  \
  \ See also: `os-attr-t`, `as-mask-p`.
  \
  \ }doc

[unneeded] os-mask-p ?\ #23694 constant os-mask-p

  \ doc{
  \
  \ os-mask-p ( -- ca )
  \
  \ A constant that returns the address _a_ of 1-byte system
  \ variable MASK_P, which holds the permanent color attribute
  \ mask, used for transparent colors, etc. Any bit that is 1
  \ shows that the corresponding attribute bit is taken not
  \ from `os-attr-p` but from what is already on the screen.
  \
  \ See also: `os-attr-p`, `os-mask-t`.
  \
  \ }doc

[unneeded] os-attr-t ?\ #23695 constant os-attr-t

  \ doc{
  \
  \ os-attr-t ( -- ca )
  \
  \ A constant that returns the address _a_ of 1-byte system
  \ variable ATTR_T, which holds the current temporary color
  \ attribute, as set up by color statements.
  \
  \ See also: `os-attr-p`, `os-mask-t`.
  \
  \ }doc

[unneeded] os-mask-t ?\ #23696 constant os-mask-t

  \ doc{
  \
  \ os-mask-t ( -- ca )
  \
  \ A constant that returns the address _a_ of 1-byte system
  \ variable MASK_T, which holds the temporary color attribute
  \ mask, used for transparent colors, etc. Any bit that is 1
  \ shows that the corresponding attribute bit is taken not
  \ from `os-attr-t` but from what is already on the screen.
  \
  \ See also: `os-attr-t`, `os-mask-p`.
  \
  \ }doc


[unneeded] os-p-flag ?\ #23697 constant os-mask-t

  \ doc{
  \
  \ os-p-flag ( -- ca )
  \
  \ A constant that returns the address _a_ of 1-byte system
  \ variable P_FLAG, which holds some flags related to
  \ printing.
  \
  \ }doc

  \ vim: filetype=soloforth