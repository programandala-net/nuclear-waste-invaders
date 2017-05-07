  \ config.fs
  \
  \ This file is part of Nuclear Waste Invaders
  \ http://programandala.net/en.program.nuclear_waste_invaders.html
  \
  \ XXX UNDER DEVELOPMENT -- not used yet
  \
  \ Last modified 201612021651

9 cconstant max-level

0
  field: ~player-order
  field: ~player-active
  field: ~player-name
  field: ~player-score
  field: ~player-record
  field: ~player-max-level
  field: ~player-level
  field: ~player-lifes
  field: ~player-left-key
  field: ~player-right-key
  field: ~player-fire-key
constant /player

wordlist dup constant players-config-wid
             >order definitions

: title$  ( -- ca len )  s" PLAYERS"  ;

: center-dh-type  ( ca len -- )  ;
  \ Type the string _ca len_ with double-height at the center
  \ of the current line.
  \ XXX TODO --

: title  ( -- )  title$ center-dh-type  ;

: table-header  ( -- )
   2 table-row at-xy s" Name" type
  15 table-row at-xy 'c' emit
  17 table-row at-xy 'y' emit
  19 table-row at-xy '1' emit
  21 table-row at-xy '2' emit
  23 table-row at-xy s" Score" type  \ XXX TODO -- 64 cpl
  28 table-row at-xy s" Record" type  \ XXX TODO -- 64 cpl
  ;

: player>row  ( n1 -- n2 )  2* [ table-y 2+ ] literal +  ;

: table-row  ( player -- )
  dup >r player>row 0 swap at-xy .
      r@ .player-name space
      r@ .player-left space
      r@ .player-right space
      r@ .player-fire1 space
      r@ .player-fire2 space
      r@ .player-level space
      r@ .player-score space
      r> .player-record  ;

: table-contents  ( -- )
  max-player 0 ?do  i table-row  loop  ;

: table  ( -- )  table-header table-contents  ;

: config-screen  ( -- )  cls title help table  ;

: config-players  ( -- )
  config-screen
  begin  get-player 2dup  while  config-player  repeat  ;

  \ vim: filetype=soloforth:colorcolumn=64
