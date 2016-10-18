  \ nuclear_invaders.fs
  \
  \ This file is part of Nuclear Invaders
  \ http://programandala.net/en.program.nuclear_invaders.html

( nuclear-invaders )

  \ XXX UNDER DEVELOPMENT

only forth definitions

warnings @ warnings off
: version   ( -- ca len )  s" 0.16.0+201610181638"  ;
warnings !

\ Description

\ This game is a ZX Spectrum port (for Solo Forth:
\ http://programandala.net/en.program.solo_forth.html) of a
\ game written by Dancresp in 2013 for Jupiter ACE
\ (http://www.zonadepruebas.com/viewtopic.php?t=4231).

  \ This version:
\ Copyright (C) 2016 Marcos Cruz (programandala.net)

  \ Original version:
\ Copyright (C) 2013 Scainet Soft

\ License

\ You may do whatever you want with this work, so long as you
\ retain the copyright/authorship/acknowledgment/credit
\ notice(s) and this license in all redistributed copies and
\ derived works.  There is no warranty.

  \ ===========================================================
  \ Options

  \ Flags for conditional compilation of new features under
  \ development.

true constant [multiple-projectiles] immediate
  \ Multiple projectiles (new) instead of one projectile (old)?

false constant [pixel-projectile] immediate
  \ Pixel projectiles (new) instead of UDG projectiles (old)?
  \ XXX TODO -- finish
  \ XXX FIXME --

  \ ===========================================================
  \ Requisites from the library of Solo Forth

blk @ 1- last-locatable !
  \ Don't search this source for requisites, just in case.

need defer  need ~~ 13 ~~key !  need [if]
  \ XXX TMP -- during the development

need roll      need inkey   need bleep        need beep>bleep
need os-chars  need os-udg  need 2/           need abort"
need value     need case    need random       need columns
need rows      need ms      need s+           need udg-row[
need 2value    need row     need char>string  need s\"
need alias     need inverse need pixel-addr   need between
need overprint need column  need color        need color!
need c+!

[pixel-projectile]
[if]    need set-pixel  need reset-pixel  need pixel-attr-addr
[else]  need ocr  [then]

[multiple-projectiles] [if]
  need allot-xstack  need xdepth  need >x  need x>  need xclear
  need .x  \ XXX TMP --
[then]

need black  need blue    need red    need magenta  need green
need cyan   need yellow  need white

need papery  need brighty  need flashy

4 constant /kk
need kk-ports  need kk-1#   need pressed?     need kk-chars

[defined] binary  ?\ : binary  ( -- )  2 base !  ;

  \ ===========================================================
  \ Debug

defer (debug-point)  ' noop ' (debug-point) defer!

: debug-point  ( -- )
  (debug-point)
  cr ." block: " blk ?  ." latest word: " latest .name
  depth if
    \ cr ." Latest word: " latest .name
    cr .s  #-258 throw \ stack imbalance
  then
  \ key drop
  ;
  \ Abort if the stack is not empty.
  \ XXX TMP -- for debugging


  \ ===========================================================
  \ Constants

16384 constant sys-screen  6912 constant /sys-screen
                           6144 constant /sys-screen-bitmap
  \ Address and size of the screen.
  \ XXX TODO -- not used yet

22528 constant attributes  768 constant /attributes
  \ Address and size of the screen attributes.
  \ XXX TODO -- not used yet

     2 constant arena-top-y
    21 constant tank-y
tank-y constant arena-bottom-y
    23 constant status-bar-y
  \ XXX TMP --
  \ XXX TODO --

  \ ===========================================================
  \ Colors

             green constant invader-color#
           magenta constant ufo-color#
black papery red + constant arena-color#

              white color in-text-color
       arena-color# color in-arena-color
 white papery red + color in-brick-color
       blue brighty color in-tank-color
               blue color in-life-color
     invader-color# color in-invader-color
     yellow brighty color in-container-color
             yellow color in-projectile-color
         ufo-color# color in-ufo-color

: init-colors  ( -- )
  black paper  white ink  black flash  0 bright
  0 overprint  0 inverse  black border  ;
  \ XXX TMP --

  \ ===========================================================
  \ Variables

variable tank-x        \ column
variable ufo-x         \ column
variable lifes         \ counter (0..4)
variable level         \ counter (1..5)
variable score         \ counter
variable record        \ max score
variable invaders      \ counter (all units of every type)
variable invader-type  \ element of table  (0..9)
variable catastrophe   \ flag (game end condition)

record off

  \ ===========================================================
  \ Keyboard

13 constant enter-key

0 value kk-left#    0 value kk-right#    0 value kk-fire#
0. 2value kk-left   0. 2value kk-right   0. 2value kk-fire

: wait  ( -- )  begin  inkey  until  ;
  \ Wait until any key is pressed.

: enter-key?  ( -- f )  inkey enter-key =  ;
  \ Is the Enter key pressed?

: wait-for-enter  ( -- )  begin  enter-key?  until  ;
  \ Wait until the Enter key is pressed.

: kk#>c  ( n -- c )  kk-chars + c@  ;
  \ Convert key number _n_ to its char _c_.

: kk#>string  ( n -- ca len )
  case  kk-en# of  s" Enter"         endof
        kk-sp# of  s" Space"         endof
        kk-cs# of  s" Caps Shift"    endof
        kk-ss# of  s" Symbol Shift"  endof
        dup kk#>c upper char>string rot  \ default
  endcase  ;

  \ Controls

3 constant /controls
  \ Bytes per item in the `controls` table.

create controls  here
  \ left    right     fire
  kk-5# c,  kk-8# c,  kk-en# c,  \ cursor: 5-8-Enter
  kk-r# c,  kk-t# c,  kk-en# c,  \ Spanish Dvorak: R-T-Enter
  kk-z# c,  kk-x# c,  kk-en# c,  \ QWERTY: Z-X-Enter
  kk-5# c,  kk-8# c,  kk-0#  c,  \ cursor joystick: 5-8-0
  kk-5# c,  kk-8# c,  kk-sp# c,  \ cursor: 5-8-Space
  kk-1# c,  kk-2# c,  kk-5#  c,  \ Sinclair joystick 1: 1-2-5
  kk-6# c,  kk-7# c,  kk-0#  c,  \ Sinclair joystick 2: 6-7-0
  kk-o# c,  kk-p# c,  kk-q#  c,  \ QWERTY: O-P-Q
  kk-n# c,  kk-m# c,  kk-q#  c,  \ QWERTY: N-M-Q
  kk-q# c,  kk-w# c,  kk-p#  c,  \ QWERTY: Q-W-P
  kk-z# c,  kk-x# c,  kk-p#  c,  \ QWERTY: Z-X-P

here swap - /controls / constant max-controls
  \ Number of controls stored in `controls`.

max-controls 1- constant last-control

: >controls  ( n -- a )  /controls * controls +  ;
  \ Convert controls number _n_ to its address _a_.

: #>kk  ( n -- d )  /kk * kk-ports + kk@  ;
  \ Convert keyboard key number _n_ to its data _d_ (bitmap and
  \ port).
  \ XXX TODO -- move to Solo Forth

: set-controls  ( n -- )
  >controls     dup c@  dup to kk-left#   #>kk 2to kk-left
             1+ dup c@  dup to kk-right#  #>kk 2to kk-right
             1+     c@  dup to kk-fire#   #>kk 2to kk-fire  ;
  \ Make controls number _n_ (item of the `controls` table) the
  \ current controls.

variable current-controls
  \ Index of the current controls in `controls` table.

current-controls off
current-controls @ set-controls
  \ Default controls.

: next-controls  ( -- )
  current-controls @ 1+  dup last-control > 0= abs *
  dup current-controls !  set-controls  ;
  \ Change the current controls.

: beep  ( n1 n2 -- )  beep>bleep bleep  ;
  \ XXX TMP -- compatibility layer for the original code
  \ XXX TODO -- adapt the original beeps

  \ ===========================================================
  \ UDG

[defined] first-udg ?\ $80 constant first-udg
                         \ first UDG code in Solo Forth
                       $FF constant last-udg
                         \ last UDG code in Solo Forth

        128 constant udgs       \ number of UDGs \ XXX TMP --
          8 constant /udg       \ bytes per UDG
udgs /udg * constant /udg-set   \ size of the UDG set in bytes

create udg-set /udg-set allot

udg-set os-udg !
  \ Point system UDG to the game UDG set.
  \ Solo Forth will use this set for chars 128..255.

: udg>bitmap  ( c -- a )  first-udg - /udg * udg-set +  ;
  \ Convert UDG char _c_ to the address _a_ of its bitmap.

: >scan  ( n c -- a )  udg>bitmap +  ;
  \ Convert scan number _n_ of UDG char _c_ to its address _a_.

: scan!  ( c b n -- c )  rot >scan c!  ;
  \ Store scan _b_ into scan number _n_ of char _c_,
  \ and return _c_ back for further processing.

variable used-udgs  used-udgs off
  \ Counter of UDGs defined.

: udg-overflow?  ( -- f )  used-udgs @ udgs >  ;
  \ Too many UDG defined?

: ?udg-overflow  ( -- )  udg-overflow? abort" Too many UDGs"  ;
  \ Abort if there are too many UDG defined.

: ?free-udg  ( n -- )  used-udgs +!  ?udg-overflow  ;
  \ Abort if there is not free space to define _n_ UDG.

  \ ===========================================================
  \ Font

: font!  ( a -- )  os-chars !  ;
  \ Set the current charset to address _a_
  \ (the bitmap of char 0).
  \ XXX TODO -- not used yet
  \ XXX TODO -- move to Solo Forth

: font@  ( -- a )  os-chars @  ;
  \ Fetch the address _a_ of the current charset
  \ (the bitmap of char 0).
  \ XXX TODO -- not used yet
  \ XXX TODO -- move to Solo Forth

: rom-font  ( -- )  15360 font!  ;
  \ Set ROM font for chars 0..127
  \ (in Solo Forth chars 128..255 are UDG).
  \ XXX TODO -- not used yet
  \ XXX TODO -- move to Solo Forth

[pixel-projectile] 0= [if]

variable ocr-first-udg
variable ocr-last-udg
  \ Char codes of the first and last UDG to be examined
  \ by `ocr`.

: init-ocr  ( -- )
  ocr-first-udg @ udg>bitmap ocr-charset !
    \ Set address of the first char bitmap to be examined.
  ocr-first-udg @ ocr-first !
    \ Its char code in the UDG set.
  ocr-last-udg @ ocr-first-udg @ - 1+ ocr-chars !  ;  \ chars
  \ Set the UDGs `ocr` will examine te detect collisions.
  \ Set the address of the first char bitmap to be
  \ examined, its char code and the number of examined chars.
  \ XXX TODO -- range: only chars that may be detected: brick
  \ and invaders.

[then]

  \ ===========================================================
  \ Debug

  \ Configure the debugging tool `~~`, in order to activate the
  \ ROM font before printing the debug information, and restore
  \ the previous font at the end.

warnings @  warnings off

variable ~~base

: ~~(  ( -- )  base @ ~~base ! decimal  ;

: ~~)  ( -- )  ~~base @ base !  ;

: ~~  ( -- )
  postpone ~~(  postpone ~~  postpone ~~)  ; immediate

~~? off

warnings !

: XXX  ( -- )
  ~~? on
  base @ >r decimal latest .name .s r> base !
  key drop ;

  \ ===========================================================
  \ Score

 1 constant score-y
14 constant record-x

variable players  1 players !  \ XXX TODO -- not used yet
variable player   1 player !   \ XXX TODO -- not used yet

: score-x  ( -- x )  3 player @ 1- 22 * +  ;
  \ Column of the score of the current player.

: (.score)  ( n x y -- )
  at-xy s>d <# # # # # #> in-text-color type  ;
  \ Print score _n_ at coordinates _x y_.

: score-xy  ( -- x y )  score-x score-y  ;
  \ Coordinates of the score.

: at-score  ( -- )  score-xy at-xy  ;
  \ Set the cursor position at the score.

: .score  ( -- )  score @ score-xy (.score)  ;
  \ Print the score.

: .record  ( -- )  record @ record-x score-y (.score)  ;
  \ Print the record.

: update-score  ( n -- )  score +! .score  ;

  \ ===========================================================
  \ Graphics

    variable >udg  first-udg >udg !  \ next free UDG

variable latest-sprite-width
variable latest-sprite-height
variable latest-sprite-udg

: ?udg  ( c -- )  last-udg > abort" Too many UDGs"  ;
  \ Abort if UDG _n_ is too high.
  \ XXX TMP -- during the development

: free-udg  ( n -- c )
  >udg @ dup latest-sprite-udg !
  tuck +  dup >udg !  1- ?udg  ;
  \ Free _n_ consecutive UDGs and return the first one _c_.

: latest-sprite-size!  ( width height -- )
  latest-sprite-height !  latest-sprite-width !  ;
  \ Update the size of the latest sprited defined.

: ?sprite-height  ( -- )
  latest-sprite-height @ 1 >
  abort" Sprite height not supported for sprite strings"  ;

: sprite-string  ( "name" -- )
  ?sprite-height
  here latest-sprite-udg @  latest-sprite-width @ dup >r
  0 ?do  dup c, 1+  loop  drop  r> 2constant  ;
  \ Create a definition "name" that will return a string
  \ containing all UDGs of the lastest sprite defined.

: (1x1sprite!)  ( b0..b7 c -- )
  1 ?free-udg  1 1 latest-sprite-size!
  /udg 0 do
    dup /udg 1+ i - roll i scan!
  loop  drop  ;
  \ Store a 1x1 UDG sprite into UDG _c_.

: 1x1sprite!  ( b0..b7 -- )
  1 free-udg (1x1sprite!)  ;
  \ Store a 1x1 UDG sprite into the next available UDG.

: 1x1sprite  ( n0..n7 "name" -- )
  1 free-udg dup constant (1x1sprite!)  ;

' emit alias .1x1sprite   ( c -- )
' emits alias .1x1sprites  ( c n -- )

: (2x1sprite!)  ( n0..n7 c -- )
  2 ?free-udg  2 1 latest-sprite-size!
  /udg 0 do
    dup /udg 1+ i - pick flip i scan! 1+  \ first UDG
    dup /udg 1+ i - roll      i scan! 1-  \ second UDG
  loop  drop  ;
  \ Store a 2x1 UDG sprite into chars _c_ and _c_+1.
  \ Scans _n0..n7_ are 16-bit: high part is char _c_,
  \ and low part is _c_+1.

: 2x1sprite!  ( n0..n7 -- )
  2 free-udg (2x1sprite!)  ;
  \ Store a 2x1 UDG sprite into the next two available UDGs.
  \ Scans _n0..n7_ are 16-bit: high part is char _c_, and low
  \ part is _c_+1.

: 2x1sprite  ( n0..n7 "name" -- )
  2 free-udg dup constant (2x1sprite!)  ;

: .2x1sprite  ( c -- )  dup emit 1+ emit  ;

2 constant udg/invader

[pixel-projectile] 0= [if]
  >udg @ ocr-first-udg !
    \ The first UDG examined by `ocr` must be the first one of
    \ the next sprite.
[then]

binary

  \ invader 1, frame 1
0000001111000000
0001111111111000
0011111111111100
0011100110011100
0011111111111100
0000011001100000
0000110110110000
0011000000001100

2x1sprite invader-1  sprite-string invader-1$  ( -- ca len )

  \ invader 1, frame 2
0000001111000000
0001111111111000
0011111111111100
0011100110011100
0011111111111100
0000111001110000
0001100110011000
0001100000011000

2x1sprite!

  \ invader 1, frame 3
0000001111000000
0001111111111000
0011111111111100
0011100110011100
0011111111111100
0000111001110000
0001100110011000
0000110000110000

2x1sprite!

  \ invader 1, frame 4
0000001111000000
0001111111111000
0011111111111100
0011100110011100
0011111111111100
0000111001110000
0001100110011000
0001100000011000

2x1sprite!

  \ invader 2, frame 1
0000100000100000
0000010001000000
0000111111100000
0001101110110000
0011111111111000
0011111111111000
0010100000101000
0000011011000000

2x1sprite invader-2  sprite-string invader-2$  ( -- ca len )

binary

  \ invader 2 , frame 2
0000100000100000
0000010001000000
0000111111100000
1111101110111110
0011111111111000
0001111111110000
0000100000100000
0001000000010000

2x1sprite!

  \ invader 2 , frame 3
0000100000100000
0010010001001000
0010111111101000
0011101110111000
0011111111111000
0001111111110000
0000100000100000
0001000000010000

2x1sprite!

  \ invader 2 , frame 4
0000100000100000
0000010001000000
0000111111100000
1111101110111110
0011111111111000
0001111111110000
0000100000100000
0001000000010000

2x1sprite!

  \ invader 3, frame 1
0000000110000000
0000001111000000
0000011111100000
0000110110110000
0000111111110000
0000001001000000
0000010110100000
0000101001010000

2x1sprite invader-3  sprite-string invader-3$  ( -- ca len )

  \ invader 3, frame 2
0000000110000000
0000001111000000
0000011111100000
0000110110110000
0000111111110000
0000010110100000
0000100000010000
0000100000010000

2x1sprite!

  \ invader 3, frame 3
0000000110000000
0000001111000000
0000011111100000
0000110110110000
0000111111110000
0000010110100000
0000100000010000
0000010000100000

2x1sprite!

  \ invader 3, frame 2
0000000110000000
0000001111000000
0000011111100000
0000110110110000
0000111111110000
0000010110100000
0000100000010000
0000100000010000

2x1sprite!

  \ ufo, frame 1
0000000000000000
0000011111100000
0001111111111000
0011111111111100
0110110110110110
1111111111111111
0011100110011100
0001000000001000

2x1sprite ufo  sprite-string ufo$  ( -- ca len )

  \ ufo, frame 2
0000000000000000
0000011111100000
0001111111111000
0011111111111100
0011011011011010
1111111111111111
0011100110011100
0001000000001000

2x1sprite!

  \ ufo, frame 3
0000000000000000
0000011111100000
0001111111111000
0011111111111100
0101101101101100
1111111111111111
0011100110011100
0001000000001000

2x1sprite!

  \ ufo, frame 4
0000000000000000
0000011111100000
0001111111111000
0011111111111100
0010110110110110
1111111111111111
0011100110011100
0001000000001000

2x1sprite!

11111011
11111011
11111011
00000000
11011111
11011111
11011111
00000000

1x1sprite brick

[pixel-projectile] 0= [if]
  >udg @ 1- ocr-last-udg !
    \ The last UDG examined by `ocr` must be the last one
    \ of the latest sprite.
[then]

  \ This UDG is used with reversed colors, depending on
  \ the side of the building:

11111111
11111111
11111000
11111100
11100100
10000000
00000000
00000000

1x1sprite broken-top-brick

  \ This UDG is used with reversed colors, depending on
  \ the side of the building:

00000000
10000000
10100000
11100100
11111100
11111000
11111101
11111111

1x1sprite broken-bottom-brick

  \ XXX TODO -- second frame of the tank

  #3 constant udg/tank  #3 free-udg udg-row[

  000000000010010000000000
  000000000010010000000000
  000000000110011000000000
  001111111111111111111100
  011111111111111111111110
  111111111111111111111111
  111111111111111111111111
  011111111111111111111110
  ]udg-row  udg/tank 1 latest-sprite-size!

sprite-string tank$  ( -- ca len )

0000010001000000
0010001010001000
0001000000010000
0000100000100000
0110000000001100
0000010000100000
0001001010010000
0010010001001000

  \ cr latest .name  \ XXX INFORMER
2x1sprite!  sprite-string invader-explosion$  ( -- ca len )
  \ cr latest .name key drop  \ XXX INFORMER

[pixel-projectile] 0= [if]

  >udg @  \ next free UDG

  00100000
  00000100
  00100000
  00000100
  00100000
  00000100
  00100000
  00000100  1x1sprite projectile-frame-0

  00000100
  00100000
  00000100
  00100000
  00000100
  00100000
  00000100
  00100000 1x1sprite!

  00100000
  00100100
  00000100
  00100000
  00100100
  00000100
  00100000
  00100100 1x1sprite!

  00000100
  00100100
  00100000
  00000100
  00100100
  00100000
  00000100
  00100100 1x1sprite!

  00100000
  00000000
  00100100
  00000000
  00000100
  00100000
  00000000
  00100100 1x1sprite!

  00000100
  00000000
  00100100
  00000000
  00100000
  00000100
  00000000
  00100100 1x1sprite!

  00100100
  00000100
  00100000
  00000000
  00100100
  00000100
  00100000
  00000000 1x1sprite!

  00100100
  00100000
  00000100
  00000000
  00100100
  00100000
  00000100
  00000000 1x1sprite!

  00100000
  00100100
  00000000
  00100100
  00100000
  00000100
  00100000
  00100100 1x1sprite!

  00000100
  00100100
  00000000
  00100100
  00000100
  00100000
  00000100
  00100100 1x1sprite!

  >udg @ swap - constant frames/projectile

[then]

0000000000000010
0010000001100100
0100011111110000
0000111111111010
0001111011011001
0100110011110000
1000011111000100
0010001100010010

2x1sprite!  sprite-string ufo-explosion$  ( -- ca len )

0000001111100000
0001110000011100
0010001111100010
0010000000000010
0010000111000010
0010001111100010
0010000111000010
0010000010000010

2x1sprite container-top

00000000
00011100
00100010
00100010
00100010
00100001
00100001
00100001

1x1sprite broken-top-left-container

00000000
00011100
00100010
00100010
01000010
01000010
01000010
10000010

1x1sprite broken-top-right-container

0010010101010010
0010111101111010
0010111000111010
0010011000110010
0010000000000010
0001110000011100
0000001111100000
0000000000000000

2x1sprite container-bottom

00000001
00000111
00011110
00100110
00100000
00011100
00000011
00000000

1x1sprite broken-bottom-left-container

11000000
01110000
00111100
00110010
00000010
00011100
11100000
00000000

1x1sprite broken-bottom-right-container

0000000000000000
0000000000001000
0000000000001100
0000111111111110
0000111111111111
0000111111111110
0000000000001100
0000000000001000

2x1sprite right-arrow
sprite-string right-arrow$  ( -- ca len )

0000000000000000
0001000000000000
0011000000000000
0111111111110000
1111111111110000
0111111111110000
0011000000000000
0001000000000000

2x1sprite left-arrow
sprite-string left-arrow$  ( -- ca len )

0000111111110000
0011000000001100
0011000000001100
0010111111110100
0010000000000100
0010000000000100
0010000000000100
1111111111111111

2x1sprite fire-button
sprite-string fire-button$  ( -- ca len )

decimal

  \ ===========================================================
  \ Type

: centered  ( len -- col )  columns swap - 2/  ;
  \ Convert a string length to the column required
  \ to print the string centered.

: centered-at  ( row len -- )  centered swap at-xy  ;
  \ Set the cursor position to print string _ca len_ centered
  \ on the given row.

: center-type  ( ca len row -- )  over centered-at type  ;
  \ Print string _ca len_ centered on the given row.

: type-blank  ( ca len -- )  nip spaces  ;

: center-type-blank  ( ca len row -- )
  over centered-at type-blank ;
  \ Overwrite string _ca len_ with blanks, centered on the given row.

17 constant message-y  \ row for game messages

: message  ( ca len -- )
  2dup message-y in-text-color center-type  1500 ms
       message-y center-type-blank  ;
  \ Print a game message _ca len_.

  \ ===========================================================
  \ Instructions

: title  ( -- )
  s" NUCLEAR INVADERS" 0 center-type
  version 1 center-type  ;

: (c)  ( -- )  127 emit  ;
  \ Print the copyright symbol.

: .copyright  ( -- )
  row
  1 over    at-xy (c) ."  2016 Marcos Cruz"
  8 swap 1+ at-xy           ." (programandala.net)"  ;
  \ Print the copyright notice at the current row.

  \ XXX OLD -- maybe useful in a future version
  \ : .control  ( n -- )  ."  = " .kk# 4 spaces  ;
  \ : .controls  ( -- )
  \   row dup s" [Space] to change controls:" rot center-type
  \   9 over 2+  at-xy ." Left " kk-left#  .control
  \   9 over 3 + at-xy ." Right" kk-right# .control
  \   9 swap 4 + at-xy ." Fire " kk-fire#  .control  ;
  \   \ Print controls at the current row.

: left-key$   ( -- ca len )  kk-left# kk#>string  ;
: right-key$  ( -- ca len )  kk-right# kk#>string  ;
: fire-key$   ( -- ca len )  kk-fire# kk#>string  ;

: controls$  ( -- ca len )
  left-arrow$ left-key$ s+
  s"   " s+ fire-key$ s+ s"   " s+
  right-key$ s+ right-arrow$ s+  ;
  \ String containing the description of the current controls.
  \ XXX TMP --
  \ XXX TODO -- rewrite

: .controls  ( -- )
  \ s" [Space] to change controls:" row dup >r center-type
  row >r fire-button$ r@ 2+ center-type
  0 r@ 3 + at-xy columns spaces
  controls$ r> 3 + center-type  ;
  \ Print controls at the current row.
  \ XXX TMP --

: .score-table-item  ( ca1 len1 ca2 len2 -- )
  type in-text-color ."  = " type  ;
  \ Print an item of the score table, with sprite string _ca2
  \ len2_ and description _ca1 len1_

9 constant score-table-x

: .score-table  ( -- )
  score-table-x row
  2dup     at-xy s" 10 points"
           in-invader-color invader-1$ .score-table-item
  2dup 2+  at-xy s" 20 points"
           in-invader-color invader-2$ .score-table-item
  2dup 4 + at-xy s" 30 points"
           in-invader-color invader-3$ .score-table-item
       6 + at-xy s" bonus"
           in-ufo-color ufo$ .score-table-item  ;
   \ Print the score table at the current row.

: at-controls  ( -- )  0 12 at-xy  ;

: show-controls  ( -- )  at-controls .controls  ;

: menu  ( -- )
  begin
    break-key? if  quit  then  \ XXX TMP
    key
    dup enter-key = if  drop exit  then
               bl = if  next-controls show-controls  then
  again  ;

: instructions  ( -- )
  in-text-color  cls  title
  0 4 at-xy .score-table
  show-controls
  s" SPACE: change - ENTER: start" 18 center-type
  0 21 at-xy .copyright
  menu  ;

  \ ===========================================================
  \ Game screen

arena-bottom-y arena-top-y - 1+ columns * constant /arena
  \ Number of characters and attributes of the arena.
arena-top-y columns * attributes + constant arena-top-attribute
  \ Address of the first attribute of the arena.

: black-arena  ( -- )  arena-top-attribute /arena erase  ;
  \ Make the arena black.

: wipe-arena  ( -- )  0 arena-top-y at-xy /arena spaces  ;
  \ Clear the arena (the whole screen except the status bars).
  \ XXX TODO -- wipe attributes first

: -arena  ( -- )  black-arena wipe-arena  ;

: score-bar$  ( -- ca len )
  in-text-color s"  SCORE<1>    RECORD    SCORE<2>"  ;

: score-bar  ( -- )
  home score-bar$ type .score .record  ;
  \ XXX TODO -- support player 2

: show-player  ( -- )
  10 0 do  at-score 4 spaces 64 ms  .score 64 ms  loop  ;
  \ Show the current player by making its score blink.

: col>pixel  ( n1 -- n2 )  8 *  ;
  \ Convert a row (0..31) to a pixel y coordinate (0..256).
  \ XXX TODO -- Move to Solo Forth and rewrite in Z80

: row>pixel  ( n1 -- n2 )  8 * 191 swap -  ;
  \ Convert a row (0..23) to a pixel y coordinate (0..191).
  \ XXX TODO -- Move to Solo Forth and rewrite in Z80

: ruler  ( -- )
  [ 0 tank-y row>pixel 8 - pixel-addr nip ] literal
  columns $FF fill  ;
  \ Draw the ruler of the status bar.

: at-lifes  ( -- )  0 status-bar-y at-xy  ;
  \ Set the cursor position to the position of the life icons.

: spare-lifes  ( -- n )  lifes @ 1- 0 max  ;
  \ Number of spare lifes.

: .lifes  ( -- )
  at-lifes in-life-color
  spare-lifes 0 ?do  tank$ type  loop  udg/tank spaces  ;
  \ Print one icon for each spare life.

: status-bar  ( -- )  ruler .lifes  ;
  \ Draw the status bar.

: game-screen  ( -- )  init-colors cls score-bar status-bar  ;
  \ Draw the game screen.

                    0 constant invaders-min-x
columns udg/invader - constant invaders-max-x

  \ Invaders data are stored in a table,
  \ which has the following structure:
  \
  \ +0 = units (0..3)
  \ +2 = active? (0..1)
  \ +4 = y row
  \ +6 = x coordinate (column)
  \ +8 = main graphic (character)

  \ The `invader` variable points to the data of the current
  \ invader in the table.

     10 constant invader-types
7 cells constant /invader-type

create default-invaders-data
  \ Default invaders data table.
  \ This is used to restore the actual data table
  \ before a new game.

here

  \ units active? y   x                sprite      x inc points
    1 ,   0 ,     5 , invaders-min-x , invader-3 ,  1 ,  30 ,
    1 ,   0 ,     7 , invaders-min-x , invader-2 ,  1 ,  20 ,
    1 ,   0 ,     9 , invaders-min-x , invader-2 ,  1 ,  20 ,
    1 ,   0 ,    11 , invaders-min-x , invader-1 ,  1 ,  10 ,
    1 ,   0 ,    13 , invaders-min-x , invader-1 ,  1 ,  10 ,
    1 ,   0 ,     5 , invaders-max-x , invader-3 , -1 ,  30 ,
    1 ,   0 ,     7 , invaders-max-x , invader-2 , -1 ,  20 ,
    1 ,   0 ,     9 , invaders-max-x , invader-2 , -1 ,  20 ,
    1 ,   0 ,    11 , invaders-max-x , invader-1 , -1 ,  10 ,
    1 ,   0 ,    13 , invaders-max-x , invader-1 , -1 ,  10 ,
    \ XXX TMP -- 1 unit per type instead of 3

here swap - constant /invaders-data
  \ Space occupied by the invaders data.

  \ XXX TODO -- convert tables to standard structures

create invaders-data  /invaders-data allot
  \ Current invaders data.

: >invader   ( -- n )  invader-type @ /invader-type *  ;
  \ Pointer of current invader type in a data table.

: 'invader   ( -- a )  >invader invaders-data +  ;
  \ Data address _a_ of the current invader type.

: 'default-invader   ( -- a )
  >invader default-invaders-data +  ;
  \ Default data address _a_ of the current invader type.

: invader-units   ( -- a )  'invader            ;
: invader-active  ( -- a )  'invader cell+      ;
: invader-y       ( -- a )  'invader [ 2 cells ] literal +  ;
: invader-x       ( -- a )  'invader [ 3 cells ] literal +  ;

: invader-xy@    ( -- x y )  invader-y 2@  ;
: invader-char@  ( -- c )  'invader [ 4 cells ] literal + @  ;

: invader-x-inc@  ( -- n )  'invader [ 5 cells ] literal + @  ;

: invader-points@  ( -- n )
  'invader [ 6 cells ] literal + @  ;

: invader-default-x@    ( -- x y )
  'default-invader [ 3 cells ] literal + @  ;

 4 constant building-top-y
15 constant building-bottom-y

variable building-width

variable building-left-x     variable building-right-x
variable containers-left-x   variable containers-right-x

: set-building-size  ( -- )
  level @ 2* 2+  building-width !
  [ columns 2/ 1- ] literal \ half of the screen
  level @ \ half width of all containers
  2dup 1- - containers-left-x !
  2dup    - building-left-x !
  2dup    + containers-right-x !
       1+ + building-right-x !  ;
  \ Set the size of the building after the current level.

5 constant max-level

: increase-level  ( -- )  level @ 1+ max-level min level !  ;
  \ Increase the level number.

: next-level  ( -- )  increase-level set-building-size  ;

: init-level  ( -- )  level off  next-level  ;
  \ Init the level number and the related variables
  \ (the size of the bulding).

  \ ==========================================================
  \ Building

: floor  ( y -- )
  building-left-x @ swap at-xy
  in-brick-color brick building-width @ .1x1sprites  ;
  \ Draw a floor of the building at row _y_.

: building-top  ( -- )  building-top-y floor  ;
  \ Draw the top of the building.

: building-bottom  ( -- )  building-bottom-y  floor  ;
  \ Draw the bottom of the building.

: containers-bottom  ( n -- )
  in-container-color
  0 ?do  container-bottom .2x1sprite  loop  ;
  \ Draw a row of _n_ bottom parts of containers.

: containers-top  ( n -- )
  in-container-color
  0 ?do  container-top .2x1sprite  loop  ;
  \ Draw a row of _n_ top parts of containers.

: .brick  ( -- )  in-brick-color brick .1x1sprite  ;
  \ Draw a brick.

: building  ( -- )
  building-bottom
  level @  building-left-x @
  building-top-y [ building-bottom-y 2- ] literal
  do
    2dup i 1+ at-xy .brick containers-bottom .brick
    2dup i    at-xy .brick containers-top    .brick
  -2 +loop  2drop  building-top  ;
  \ Draw the building and the nuclear containers.

  \ ==========================================================
  \ Tank

                    1 constant tank-min-x
columns udg/tank - 1- constant tank-max-x
  \ Mininum and maximin columns of the tank.

: new-projectile-x  ( -- col|x )
  [pixel-projectile]
  [if]    tank-x @ col>pixel [ udg/tank 8 * 2/ ] literal +
  [else]  tank-x @ 1+
  [then]  ;
  \ Return the column _col_ or graphic coordinate _x_ for the
  \ new projectile, depending (at compile time) on the type of
  \ projectile and (at runtime) the position of the tank.

: gun-below-building?  ( -- f )
  new-projectile-x
  [pixel-projectile] [if]
    building-left-x @ col>pixel building-right-x @ col>pixel
  [else]
    building-left-x @ building-right-x @
  [then]  between  ;
  \ Is the tank's gun below the building?

: tank-range  ( col -- col' )
  tank-max-x min tank-min-x max  ;
  \ Adjust the given column to the limits of the tank.

variable transmission-delay-counter

transmission-delay-counter off

8 value transmission-delay

: update-transmission  ( -- )
  transmission-delay-counter @ 1- 0 max
  transmission-delay-counter !  ;
  \ Decrement the transmission delay. The minimum is zero.

: transmission-ready?  ( -- f )
  transmission-delay-counter @ 0=  ;
  \ Is the transmission ready?

: moving-tank?  ( -- -1|0|1 )
  kk-left pressed? kk-right pressed? abs +
  transmission-ready? and  ;
  \ Does the tank move? Return its x increment.

: .tank  ( -- )  in-tank-color tank$ type  ;
  \ Print the tank at the current cursor position.

: at-tank  ( -- )  tank-x @ tank-y at-xy  ;

: tank-ready  ( -- )  at-tank .tank  ;

: -tank  ( -- )  at-tank in-arena-color udg/tank spaces  ;

: move-tank  ( -1|1 -- )
  tank-x @ + tank-range dup tank-x ! tank-y at-xy  ;
  \ Increment the column of the tank with the given value, then
  \ set the cursor position to the coordinates of the tank.

: drive  ( -- )
  update-transmission
  moving-tank? ?dup 0= ?exit  -tank move-tank .tank  ;

  \ XXX TODO -- don't delete the whole tank every time, but
  \ only the character not overwritten by the new position

  \ ==========================================================
  \ Projectile

[multiple-projectiles] [if]

  \ XXX NEW -- several projectiles on the screen

  \ XXX TODO -- Store coordinates in bytes; this will make some
  \ calculations faster. E.g. `projectile#` can be used as
  \ offset.

%111 value max-projectile#
  \ Bitmask for the projectile counter (0..7).
  \ XXX TODO -- try %1111 and %11111

max-projectile# 1+ constant #projectiles
  \ Maximum number of simultaneous projectiles.

#projectiles allot-xstack free-projectiles free-projectiles
  \ Create and activate an stack to store the free projectiles.

0 value projectile#
  \ Number of the current projectile.

create 'projectile-x #projectiles allot
create 'projectile-y #projectiles allot
  \ Tables for the coordinates of all projectiles.

: projectile-x  ( -- a )  'projectile-x projectile# +  ;
  \ Address of the x coordinate of the current projectile.

: projectile-y  ( -- a )  'projectile-y projectile# +  ;
  \ Address of the y coordinate of the current projectile.

[pixel-projectile] [if]

defer debug-data-pause  ( -- )

' wait ' debug-data-pause defer!

: .debug-data  ( -- )
  9 23 at-xy ." Ammo:" xdepth .
             ." Depth:" depth .
             ." Curr.:" projectile# .
             debug-data-pause  ;
  \ XXX INFORMER

[else]  : .debug-data  ( -- )  ; immediate  [then]

: destroy-projectile  ( -- )
  0 projectile-y c!  projectile# >x  .debug-data  ;

: init-projectiles  ( -- )
  'projectile-y #projectiles erase
  xclear #projectiles 0 do  i >x  loop  ;

[else]

  \ XXX OLD -- one single projectile on the screen

cvariable projectile-x  \ column
cvariable projectile-y  \ row (0 if no shoot)

: destroy-projectile  ( -- )  0 projectile-y c!  ;
  \ Destroy the projectile.

: init-projectiles  ( -- )  destroy-projectile  ;
  \ Init the projectiles.

[then]

: projectile-coords  ( -- x y )
  projectile-x c@ projectile-y c@  ;
  \ Coordinates of the projectile.

  \ ==========================================================
  \ Init

4 constant max-lifes
  \ Maximum number of lifes, including the first one.

: init-lifes  ( -- )  max-lifes lifes !  ;

: init-game  ( -- )
  [pixel-projectile] [ 0= ] [if]  init-ocr  [then]
  init-lifes  init-level  score off  game-screen  ;
  \ Init the game.

: init-invaders-data  ( -- )
  default-invaders-data invaders-data /invaders-data move  ;

-200 constant ufo-initial-x
  \ Initial column of the UFO, out of the screen.

: init-ufo  ( -- )  ufo-initial-x ufo-x !  ;
  \ Init the UFO.

: total-invaders  ( -- n )
  0  invader-types 0 do
       i invader-type ! invader-units @ +
     loop  ;
  \ Total number of invaders (sum of units of all invader
  \ types).

: init-invaders  ( -- )
  init-invaders-data  invader-type off
  total-invaders invaders !  ;
  \ Init the invaders.

: init-tank  ( -- )  columns udg/tank - 2/ tank-x !  ;
  \ Init the tank.

: parade  ( -- )
  in-invader-color
  invader-1 dup invader-2 dup invader-3
  building-bottom-y [ building-top-y 1+ ] literal
  do
    invaders-min-x i at-xy dup .2x1sprite
    invaders-max-x i at-xy     .2x1sprite
  2 +loop  ;
  \ Show the invaders at their initial positions.

: init-arena  ( -- )   -arena building tank-ready parade  ;

: level-message  ( -- ca len )
  in-text-color s" LEVEL " level @ s>d <# # #> s+  ;

: show-level  ( -- )  level-message message  ;

: init-combat  ( -- )
  catastrophe off  init-invaders init-ufo init-tank init-arena
  init-projectiles show-level show-player  ;

  \ ==========================================================
  \ Invasion

: at-invader  ( -- )  invader-xy@ at-xy  ;
  \ Set the cursor position at the coordinates of the invader.

4 constant frames/invader

: sprite>frame  ( c1 col -- c2 )
  frames/invader mod udg/invader * +  ;
  \ Frame _c2_ of sprite _c1_, calculated from its column
  \ _col_.

: invader-frame  ( -- c )
  invader-char@ invader-x @ sprite>frame  ;
  \ Frame of the invader, calculated from its column.

: .invader  ( -- )
  in-invader-color invader-frame .2x1sprite  ;
  \ Print the current invader.

  \ ============================================================
  \ Invasion

variable broken-wall-x
  \ Column of the wall broken by the current alien.

: flying-to-the-right?  ( -- f )  invader-x-inc@ 0>  ;
  \ Is the current invader flying to the right?

red papery c,  here  red c,  constant broken-brick-colors
  \ Address of the broken brick color used for the right
  \ side of the building; the address before contains
  \ the color used for the left side of the building.
  \ This is faster than pointing to a list of two colors,
  \ because a flag (0|-1) can be used as index.

: broken-wall-color  ( -- )
  broken-brick-colors flying-to-the-right? + c@ color!  ;
  \ Set the color of the broken wall.  It depends on the side
  \ of the building, because the same two UDGs are used for
  \ both sides, with inverted colors.

: broken-bricks-coordinates  ( -- x1 y1 x2 y2 )
  broken-wall-x @ invader-y @ 1+  2dup 2-  ;
  \ Coordinates of the broken brick above the invader, _x2 y2_,
  \ and below it, _x1 y1_.

: broken-left-wall  ( -- )
  broken-bricks-coordinates
  at-xy broken-bottom-brick .1x1sprite
  at-xy broken-top-brick  .1x1sprite  ;

: broken-right-wall  ( -- )
  broken-bricks-coordinates
  at-xy broken-top-brick .1x1sprite
  at-xy broken-bottom-brick .1x1sprite  ;

  \ XXX TODO -- detect if the wall is already broken, and
  \ change the graphic accordingly.

: broken-wall  ( -- )
  broken-wall-color flying-to-the-right?
  if  broken-left-wall  else  broken-right-wall  then  ;
  \ Show the broken wall of the building, hit by the current invader.

: broken-wall?  ( -- f )
  invader-x @ flying-to-the-right?
  if    1+ building-left-x
  else  building-right-x
  then  @ dup broken-wall-x ! =  ;
  \ Has the current invader broken the wall of the building?

: broken-left-container  ( -- )
  invader-x @ 2+ invader-y @ at-xy
  broken-top-right-container .1x1sprite
  invader-x @ 1+ invader-y @ 1+ at-xy
  broken-bottom-left-container .1x1sprite  ;
  \ Broke the container on its left side.

: broken-right-container  ( -- )
  invader-x @ 1- invader-y @ at-xy
  broken-top-left-container .1x1sprite
  invader-x @ invader-y @ 1+ at-xy
  broken-bottom-right-container .1x1sprite  ;
  \ Broke the container on its right side.

: broken-container  ( -- )
  in-container-color
  flying-to-the-right?  if    broken-left-container
                        else  broken-right-container  then  ;
  \ Broke the container.

: broken-container?  ( -- f )
  invader-x @ flying-to-the-right?
  if    1+ containers-left-x
  else     containers-right-x  then  @ =  ;
  \ Has the current invader broken a container?

: damages  ( -- )
  broken-wall? if  broken-wall exit  then
  broken-container? dup if    broken-container
                        then  catastrophe !  ;
  \ Manage the possible damages caused by the current invader.

: left-flying-invader  ( -- )
  -1 invader-x +! at-invader .invader space  ;
  \ Move the current invader, which is flying to the left.

: right-flying-invader  ( -- )
  at-invader in-arena-color space .invader 1 invader-x +!  ;
  \ Move the current invader, which is flying to the right.

: flying-invader  ( -- )
  flying-to-the-right?
  if  right-flying-invader  else  left-flying-invader  then  ;

: activate-invader  ( -- )
  invaders @ random 0= invader-active !  ;
  \ Activate the current invader randomly, depending on the
  \ number of aliens.

: last-invader-type?  ( -- f )
  invader-type @ [ invader-types 1- ] literal =  ;
  \ Is the current invader type the last one?

: next-invader  ( -- )
  last-invader-type?
  if  invader-type off  else  1 invader-type +!  then  ;
  \ Update the invader type to the next one.

: move-invader  ( -- )
  invader-active @
  if  flying-invader damages  else  activate-invader  then  ;
  \ Move the current invader if it's active, else
  \ just try to activate it.

: invasion  ( -- )
  invader-units @ if  move-invader  then  next-invader  ;
  \ Move the current invader, if there are units left of it,
  \ and then choose the next one.

  \ ==========================================================
  \ UFO

 3 constant ufo-y       \ row
27 constant ufo-max-x   \ column

: ufo-invisible?  ( -- f )  ufo-x @ 0<  ;
  \ Is the UFO invisible?

: at-ufo  ( -- )  ufo-x @ ufo-y at-xy  ;
  \ Set the cursor position at the coordinates of the UFO.

: -ufo  ( -- )  at-ufo 3 spaces init-ufo  ;
  \ Delete and init the UFO.

: ufo-lost?  ( -- f )  ufo-x @ ufo-max-x >  ;
  \ Is the UFO lost?

: ufo-frame  ( -- c )  ufo ufo-x @ sprite>frame  ;
  \ Current frame _c_ of the UFO.
  \ XXX TODO -- make it faster

: flying-ufo  ( -- )
  1 ufo-x +! at-ufo in-ufo-color space ufo-frame .2x1sprite  ;
  \ Update the position of the UFO and show it.
  \ XXX TODO -- rename `visible-ufo`?
  \ XXX TODO -- factor `1 ufo-x +!`

: (move-ufo)  ( -- )
  ufo-lost?  if  -ufo  else  flying-ufo  then  ;
  \ Manage the UFO.
  \ XXX TODO -- rename

: move-ufo  ( -- )
  ufo-invisible? if  1 ufo-x +!  else  (move-ufo)  then  ;
  \ Manage the UFO, if it's visible.

  \ ==========================================================
  \ Impact

: ufo-bang  ( -- )  18 12 do  i 15 beep  loop  ;
  \ XXX TODO -- 128 sound

: ufo-on-fire  ( -- )
  ufo-x @ 1+ ufo-y at-xy ufo-explosion$ type  ;

: ufo-explosion  ( -- )  ufo-on-fire ufo-bang -ufo  ;

: ufo-points  ( -- n )  5 random 1+ 10 * 50 +  ;
  \ Random points for impacting the UFO.

: ufo-bonus  ( -- )  ufo-points update-score  ;
  \ Update the score with the UFO bonus.

: ufo-impacted  ( -- )  ufo-explosion ufo-bonus  ;

: invader-bonus  ( -- )  invader-points@  update-score  ;
  \ Update the score with the invader bonus.

: invader-bang  ( -- ca len )  10 100 beep  ;
  \ XXX TODO -- explosion 128 sound

: invader-on-fire  ( -- )
  at-invader invader-explosion$ type  ;

: -invader  ( -- )  at-invader 2 spaces  ;
  \ Delete the current invader.

: invader-explosion  ( -- )
  invader-on-fire invader-bang -invader  ;

: impacted-invader  ( -- n )
  projectile-y c@ [ building-top-y 1+ ] literal - 2/
  projectile-x c@ [ columns 2/ ] literal > abs 5 * +  ;
  \ Return the type _n_ (0..9) of the impacted invader,
  \ calculated from the projectile coordinates: Invader types 0
  \ and 5 are at the top, one row below the top of the
  \ building; types 1 and 6 are two rows below and so on.
  \ Types 0..4 are at the left of the screen; types 5..9 are at
  \ the right.

: replace-invader  ( -- )
  invader-active off
  invader-default-x@ invader-x !  at-invader
  in-invader-color invader-char@ .2x1sprite  ;
  \ Replace the current invader with its next unit.
  \ Set it inactive at its start position.

: current-invader-impacted  ( -- )
  invader-bonus invader-explosion
  -1 invaders +!  -1 invader-units +!
  invader-units @ if  replace-invader  then  ;
  \ The current invader has been impacted by the projectile.

: invader-impacted  ( -- )
  invader-type @ >r  impacted-invader invader-type !
  current-invader-impacted  r> invader-type !  ;
  \ An invader has been impacted by the projectile.
  \ Calculate its type, set it the current one and manage it.

: ufo-impacted?  ( -- f )
  [pixel-projectile]
  [if]    projectile-coords pixel-attr-addr c@ ufo-color# =
  [else]  projectile-y c@ ufo-y =
  [then]  ;

: (impact)  ( -- )
  ufo-impacted? if  ufo-impacted exit  then
  invader-impacted  ;
  \ Manage the impact.

: impact  ( -- )
  projectile-y c@
  [pixel-projectile]
  [if]    [ building-bottom-y row>pixel ] literal >
  [else]  building-bottom-y <
  [then]  if  (impact)  then  destroy-projectile  ;
  \ Manage the impact, if the projectil is high enough.
  \ XXX TODO -- impact the building too

: hit?  ( -- f )
  projectile-coords  [pixel-projectile]
  [if]    pixel-attr-addr c@ invader-color# =
  [else]  ocr 0<>
  [then]  ;
  \ Did the projectile hit the target?

: impact?  ( -- f )  hit? dup if  impact  then  ;
  \ Did the projectil impacted?
  \ If so, do manage the impact.

  \ ==========================================================
  \ Shoot

[pixel-projectile] 0= [if]

: at-projectile  ( -- )  projectile-coords at-xy  ;
  \ Set the cursor position at the coordinates of the
  \ projectile.

: projectile  ( -- c )
  projectile-frame-0 frames/projectile random +  ;
  \ Return the UDG _c_ of a random frame of the projectile.

[then]

: .projectile  ( -- )
  [pixel-projectile]
  [if]    projectile-coords set-pixel
  [else]  in-projectile-color
          at-projectile projectile .1x1sprite
  [then]  ;
  \ Show the projectile.

: fire-sound  ( -- )
  \ 50 0 do  7 1 do  i border 0 border  loop  loop
    \ XXX INFORMER
  ;
  \ XXX TODO -- 128 sound

: -projectile  ( -- )
  [pixel-projectile]
  [if]    projectile-coords reset-pixel
  [else]  at-projectile in-arena-color space
  [then]  ;
  \ Delete the projectile.

: projectile-lost?  ( -- f )
  [pixel-projectile]
  [if]    projectile-y c@ [ building-top-y row>pixel ] literal >
  [else]  projectile-y c@ building-top-y <
  [then]  ;
  \ Is the projectile lost?

[multiple-projectiles] [if]

  \ XXX TODO -- improve

: move-projectile  ( -- )
  -projectile
  projectile-lost? if  destroy-projectile exit  then
  [pixel-projectile] [if]    7
                     [else]  -1
                     [then]  projectile-y c+!
  impact? ?exit  .projectile  ;
  \ Manage the projectile.

variable trigger-delay-counter  trigger-delay-counter off

[pixel-projectile] [if]    8
                   [else]  6
                   [then]  value trigger-delay

: delay-trigger  ( -- )
  trigger-delay trigger-delay-counter !  ;

: fire  ( -- )
  x> to projectile#  .debug-data
  new-projectile-x projectile-x c!
  [pixel-projectile]
  [if]    [ tank-y row>pixel 1+ ] literal projectile-y c!
  [else]  [ tank-y 1- ] literal projectile-y c!
  [then]  fire-sound  delay-trigger  ;
  \ The tank fires.

: flying-projectile?  ( -- f )  projectile-y c@ 0<>  ;
  \ Is the current projectile flying?

: projectile-left?  ( -- f )  xdepth 0<>  ;
  \ Is there any projectile left?

: update-trigger  ( -- )
  trigger-delay-counter @ 1- 0 max trigger-delay-counter !  ;
  \ Decrement the trigger delay. The minimum is zero.

: trigger-ready?  ( -- f )  trigger-delay-counter @ 0=  ;
  \ Is the trigger ready?

: trigger-pressed?  ( -- f )  kk-fire pressed?  ;
  \ Is the fire key pressed?

: next-projectile  ( -- )
  projectile# 1+ max-projectile# and to projectile#  ;
  \ Point to the next current projectile.

: fly-projectile  ( -- )
  .debug-data  \ XXX INFORMER
  flying-projectile? if  move-projectile  then
  next-projectile  ;
  \ Manage the shoot.

: shoot  ( -- )
  trigger-pressed? if
    trigger-ready? if
      projectile-left? if
        gun-below-building? 0= if
          fire  then  then  then  then  update-trigger  ;
  \ Manage the shoot.
  \
  \ XXX TODO -- use `?exit` and `0exit` instead of conditionals
  \ and move `update-trigger` to the start.

[else]

: move-projectile  ( -- )
  -projectile
  projectile-lost? if  destroy-projectile exit  then
  [pixel-projectile] [if]    7
                     [else]  -1
                     [then]  projectile-y c+!
  impact? ?exit  .projectile  ;
  \ Manage the projectile.

: fire  ( -- )
  new-projectile-x projectile-x c!
  [ tank-y 1- ] literal projectile-y c!  fire-sound  ;
  \ The tank fires.

: flying-projectile?  ( -- f )  projectile-y c@ 0<>  ;
  \ Is the projectile flying?

: trigger-pressed?  ( -- f )  kk-fire pressed?  ;
  \ Is the fire key pressed?

: fly-projectile  ( -- )
  flying-projectile? if  move-projectile exit  then  ;

: shoot  ( -- )
  trigger-pressed? if
    gun-below-building? 0= if
     fire  then  ;
  \ Manage the shoot.

[then]

: new-record?   ( -- f )  score @ record @ >  ;
  \ Is there a new record?

: new-record    ( -- )  score @ record !  ;
  \ Set the new record.

: check-record  ( -- )  new-record? if  new-record  then  ;
  \ Set the new record, if needed.

  \ ==========================================================
  \ Main

: .game-over  ( -- )  s" GAME OVER" message  ;

: game-over  ( -- )  .game-over check-record  ;

: dead  ( -- )  -1 lifes +!  .lifes  ;
  \ One life lost.

: defeat-tune  ( -- )  100 200 do  i 20 beep  -5 +loop  ;
  \ XXX TODO -- 128 sound

: defeat  ( -- )  defeat-tune  300 ms  dead  ;

: victory?  ( -- f )  invaders @ 0=  ;

: (combat)  ( -- )
  begin   victory? if  next-level init-combat  then

          break-key? if  quit  then  \ XXX TMP

          fly-projectile  drive
          fly-projectile  shoot
          fly-projectile  move-ufo
          fly-projectile  invasion
          fly-projectile  catastrophe @
  until   defeat  ;

: combat  ( -- )  init-combat (combat)  ;

: defeat?  ( -- f )  lifes @ 0=  ;

: game  ( -- )
  init-game  begin  combat defeat?  until  game-over  ;

: run  ( -- )  begin  instructions game  again  ;

  \ ============================================================
  \ Debugging tools

: .udgs  ( -- )  cr udgs 0 do  i 128 + emit  loop  ;
  \ Print all game UDGs.

: ni  ( -- )      next-invader  ;
: m   ( -- )      move-invader broken-container? home .  ;
: in  ( -- )      init-game init-combat  ;

: bc  ( -- )
  cls
  \ top:
  space broken-top-right-container .1x1sprite
  container-top .2x1sprite
  broken-top-left-container .1x1sprite space cr
  \ bottom:
  container-bottom .2x1sprite 8 emit 8 emit
  broken-bottom-left-container .1x1sprite
  xy swap 1+ swap at-xy
  container-bottom .2x1sprite
  container-bottom .2x1sprite 8 emit
  broken-bottom-right-container .1x1sprite cr  ;
  \ Show the graphics of the broken containers.

init-level

cr cr .( Type RUN to start) cr

end-app

  \ vim: filetype=soloforth:colorcolumn=64
