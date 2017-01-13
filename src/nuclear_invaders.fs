  \ nuclear_invaders.fs
  \
  \ This file is part of Nuclear Invaders
  \ http://programandala.net/en.program.nuclear_invaders.html

( nuclear-invaders )

  \ XXX UNDER DEVELOPMENT

only forth definitions
need wordlist>vocabulary
wordlist dup constant nuclear-invaders-wordlist
         dup wordlist>vocabulary nuclear-invaders
         dup >order set-current

: version  ( -- ca len )  s" 0.31.0-pre.5+201701131915"  ;

cr cr .( Nuclear Invaders) cr version type cr

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
  cr .( Options)  \ {{{1

  \ Flags for conditional compilation of new features under
  \ development.

false constant [pixel-projectile] immediate
  \ Pixel projectiles (new) instead of UDG projectiles (old)?
  \ XXX TODO -- finish support for pixel projectiles

  \ ===========================================================
  cr .( Library)  \ {{{1

blk @ 1- last-locatable !
  \ Don't search this source for requisites, just in case.

forth-wordlist set-current

  \ --------------------------------------------
  cr .(   -Development tools)  \ {{{2

need [if]

need ~~

need warn.message  need order  need see  need rdepth

  \ --------------------------------------------
  cr .(   -Definers)  \ {{{2

need defer  need alias
need value  need 2value  need cvariable  need 2const

  \ --------------------------------------------
  cr .(   -Strings)  \ {{{2

need s+  need char>string  need s\"

  \ --------------------------------------------
  cr .(   -Control structures)  \ {{{2

need case  need 0exit   need +perform  need abort"

  \ --------------------------------------------
  cr .(   -Memory)  \ {{{2

need c+!

  \ --------------------------------------------
  cr .(   -Math)  \ {{{2

need d<  need -1|1  need 2/  need between  need random

[defined] binary  ?\ : binary  ( -- )  2 base !  ;

  \ --------------------------------------------
  cr .(   -Data structures)  \ {{{2

need roll

need field:    need +field-opt-0124

need allot-xstack  need xdepth  need >x  need x>  need xclear
need .x  \ XXX TMP -- for debuging

  \ --------------------------------------------
  cr .(   -Graphics)  \ {{{2

need os-chars  need os-udg  need pixel-addr

need udg-row[

need columns  need rows  need row

need fade

[pixel-projectile]
[if]    need set-pixel  need reset-pixel  need pixel-attr-addr
[else]  need ocr  [then]

need inverse  need overprint
need color   need color!

need black  need blue    need red    need magenta  need green
need cyan   need yellow  need white

need papery  need brighty  need flashy

need attr

  \ --------------------------------------------
  cr .(   -Keyboard)  \ {{{2

need kk-ports  need kk-1#   need pressed?     need kk-chars
need #>kk  need inkey

  \ --------------------------------------------
  cr .(   -Time)  \ {{{2

need frames@  need ms

  \ --------------------------------------------
  cr .(   -Sound)  \ {{{2

need bleep        need beep>bleep

  \ --------------------------------------------

nuclear-invaders-wordlist set-current

  \ ===========================================================
  cr .( Debug)  \ {{{1

defer debug-point  defer special-debug-point

defer ((debug-point))  ' noop ' ((debug-point)) defer!

: (debug-point)  ( -- )
  ((debug-point))
  \ order
  \ depth ?exit
  \ ." block:" blk ?  ." latest:" latest .name ." hp:" hp@ u.
  \ order
  \ s" ' -1|1 .( -1|1 =) u." evaluate
  \ depth if  cr .s #-258 throw  then  \ stack imbalance
  \ key drop
  ;
  \ Abort if the stack is not empty.
  \ XXX TMP -- for debugging

  \ ' noop ' debug-point defer!
  ' (debug-point) ' debug-point defer!
  \ XXX TMP -- for debugging

  ' noop ' special-debug-point defer!
  \ ' (debug-point) ' special-debug-point defer!
  \ XXX TMP -- for debugging

  \ : :
  \   cr blk @ . latest .name ." ..."
  \   s" attributes drop " evaluate : ;
  \ XXX TMP -- for debugging

: XXX  ( -- )
  ~~? @ 0= ?exit
  base @ >r decimal latest .name .s r> base !
  key drop ;

'q' ~~quit-key !  ~~resume-key on  22 ~~y !  ~~? on

: ~~stack-info  ( -- )
  home ." rdepth:" rdepth .  ;
' ~~stack-info ' ~~app-info defer!
  \ XXX TMP -- for debugging

  \ ===========================================================
  cr .( Constants)  debug-point  \ {{{1

16384 constant sys-screen  6912 constant /sys-screen
                           6144 constant /sys-screen-bitmap
  \ Address and size of the screen.
  \ XXX TODO -- not used yet

22528 constant attributes  768 constant /attributes
  \ Address and size of the screen attributes.

     2 cconstant arena-top-y
    21 cconstant tank-y
tank-y cconstant arena-bottom-y
    23 cconstant status-bar-y
  \ XXX TMP --
  \ XXX TODO --

  \ ===========================================================
  cr .( Colors)  debug-point  \ {{{1

             green cconstant invader-color#

             green cconstant sane-invader-color#
            yellow cconstant wounded-invader-color#
               red cconstant dying-invader-color#

           magenta cconstant ufo-color#
             black cconstant arena-color#
    yellow brighty cconstant radiation-color#
             white cconstant ruler-color#
            yellow cconstant projectile-color#

              white color in-text-color
       arena-color# color in-arena-color
 white papery red + color in-brick-color
                red color in-broken-wall-color
       blue brighty color in-tank-color
               blue color in-life-color
     invader-color# color in-invader-color
     yellow brighty color in-container-color
  projectile-color# color in-projectile-color
         ufo-color# color in-ufo-color

: init-colors  ( -- )
  black paper  white ink  black flash  0 bright
  0 overprint  0 inverse  black border  ;

  \ ===========================================================
  cr .( Global variables)  debug-point  \ {{{1

variable lifes           \ counter (0..4)
variable level           \ counter (1..max-level)
variable score           \ counter
variable record          \ max score
variable current-invader \ element of table  (0..9)
variable catastrophe     \ flag (game end condition)

record off

  \ ===========================================================
  cr .( Keyboard)  debug-point  \ {{{1

13 cconstant enter-key

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

3 cconstant /controls
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

here swap - /controls / cconstant max-controls
  \ Number of controls stored in `controls`.

max-controls 1- cconstant last-control

: >controls  ( n -- a )  /controls * controls +  ;
  \ Convert controls number _n_ to its address _a_.

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
  cr .( UDG)  debug-point  \ {{{1

[defined] first-udg ?\ $80 cconstant first-udg
                         \ first UDG code in Solo Forth
                       $FF cconstant last-udg
                         \ last UDG code in Solo Forth

        128 cconstant udgs       \ number of UDGs \ XXX TMP --
          8 cconstant /udg       \ bytes per UDG
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
  cr .( Font)  debug-point  \ {{{1

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
  \ Set the UDGs `ocr` will examine to detect collisions.
  \ Set the address of the first char bitmap to be
  \ examined, its char code and the number of examined chars.
  \ XXX TODO -- range: only chars that may be detected: brick
  \ and invaders.

[then]

  \ ===========================================================
  cr .( Score)  debug-point  \ {{{1

 1 cconstant score-y
14 cconstant record-x

2 cconstant max-player

variable players  1 players !  \ 1..max-player
variable player   1 player !   \ 1..max-player

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
  cr .( Graphics)  debug-point  \ {{{1

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
  1 free-udg dup cconstant (1x1sprite!)  ;

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
  \ Scans _n0..n7_ are 16-bit: their high parts form the first
  \ available UDG, and their low parts form the next one.

: 2x1sprite  ( n0..n7 "name" -- )
  2 free-udg dup cconstant (2x1sprite!)  ;

: .2x1sprite  ( c -- )  dup emit 1+ emit  ;

2 cconstant udg/invader
2 cconstant udg/ufo

[pixel-projectile] 0= [if]
  >udg @ ocr-first-udg !
    \ The first UDG examined by `ocr` must be the first one of
    \ the next sprite.
[then]

binary

  \ flying invader 1, frame 1
0000001111000000
0001111111111000
0011111111111100
0011100110011100
0011111111111100
0000011001100000
0000110110110000
0011000000001100

2x1sprite flying-invader-1
sprite-string flying-invader-1$  ( -- ca len )

  \ flying invader 1, frame 2
0000001111000000
0001111111111000
0011111111111100
0011100110011100
0011111111111100
0000111001110000
0001100110011000
0001100000011000

2x1sprite!

  \ flying invader 1, frame 3
0000001111000000
0001111111111000
0011111111111100
0011100110011100
0011111111111100
0000111001110000
0001100110011000
0000110000110000

2x1sprite!

  \ flying invader 1, frame 4
0000001111000000
0001111111111000
0011111111111100
0011100110011100
0011111111111100
0000111001110000
0001100110011000
0001100000011000

2x1sprite!

  \ docked invader 1, frame 1
0000001111000000
0001111111111000
0011111111111100
0011001100111100
0011111111111100
0000011001100000
0000110110110000
0011000000001100

2x1sprite docked-invader-1

  \ docked invader 1, frame 2
0000001111000000
0001111111111000
0011111111111100
0011100110011100
0011111111111100
0000011001100000
0000110110110000
0011000000001100

2x1sprite!

  \ docked invader 1, frame 3
0000001111000000
0001111111111000
0011111111111100
0011110011001100
0011111111111100
0000011001100000
0000110110110000
0011000000001100

2x1sprite!

  \ flying invader 2, frame 1
0000100000100000
0000010001000000
0000111111100000
0001101110110000
0011111111111000
0011111111111000
0010100000101000
0000011011000000

2x1sprite flying-invader-2
sprite-string flying-invader-2$  ( -- ca len )

binary

  \ flying invader 2 , frame 2
0000100000100000
0000010001000000
0000111111100000
1111101110111110
0011111111111000
0001111111110000
0000100000100000
0001000000010000

2x1sprite!

  \ flying invader 2 , frame 3
0000100000100000
0010010001001000
0010111111101000
0011101110111000
0011111111111000
0001111111110000
0000100000100000
0001000000010000

2x1sprite!

  \ flying invader 2 , frame 4
0000100000100000
0000010001000000
0000111111100000
1111101110111110
0011111111111000
0001111111110000
0000100000100000
0001000000010000

2x1sprite!

  \ docked invader 2, frame 1
0000100000100000
0000010001000000
0000111111100000
0001011101110000
0011111111111000
0011111111111000
0010100000101000
0000011011000000

2x1sprite docked-invader-2

  \ docked invader 2, frame 2
0000100000100000
0000010001000000
0000111111100000
0001101110110000
0011111111111000
0011111111111000
0010100000101000
0000011011000000

2x1sprite!

  \ docked invader 2, frame 3
0000100000100000
0000010001000000
0000111111100000
0001110111010000
0011111111111000
0011111111111000
0010100000101000
0000011011000000

2x1sprite!

  \ flying invader 3, frame 1
0000000110000000
0000001111000000
0000011111100000
0000110110110000
0000111111110000
0000001001000000
0000010110100000
0000101001010000

2x1sprite flying-invader-3
sprite-string flying-invader-3$  ( -- ca len )

  \ flying invader 3, frame 2
0000000110000000
0000001111000000
0000011111100000
0000110110110000
0000111111110000
0000010110100000
0000100000010000
0000100000010000

2x1sprite!

  \ flying invader 3, frame 3
0000000110000000
0000001111000000
0000011111100000
0000110110110000
0000111111110000
0000010110100000
0000100000010000
0000010000100000

2x1sprite!

  \ flying invader 3, frame 4
0000000110000000
0000001111000000
0000011111100000
0000110110110000
0000111111110000
0000010110100000
0000100000010000
0000100000010000

2x1sprite!

  \ docked invader 3, frame 1
0000000110000000
0000001111000000
0000011111100000
0000101101110000
0000111111110000
0000001001000000
0000010110100000
0000101001010000

2x1sprite docked-invader-3

  \ docked invader 3, frame 2
0000000110000000
0000001111000000
0000011111100000
0000110110110000
0000111111110000
0000001001000000
0000010110100000
0000101001010000

2x1sprite!

  \ docked invader 3, frame 2
0000000110000000
0000001111000000
0000011111100000
0000111011010000
0000111111110000
0000001001000000
0000010110100000
0000101001010000

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

11111111
01111111
01011111
00011011
00000011
00000111
00000010
00000000

1x1sprite broken-top-left-brick

00000000
00000000
00000111
00000011
00011011
01111111
11111111
11111111

1x1sprite broken-bottom-left-brick

11111111
11111111
11111000
11111100
11100100
10000000
00000000
00000000

1x1sprite broken-top-right-brick

00000000
10000000
10100000
11100100
11111100
11111000
11111101
11111111

1x1sprite broken-bottom-right-brick

  \ XXX TODO -- second frame of the tank

  #3 cconstant udg/tank  #3 free-udg udg-row[

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

  >udg @ swap - cconstant frames/projectile

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
  cr .( Type)  debug-point  \ {{{1

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
  \ Overwrite string _ca len_ with blanks, centered on the
  \ given row.

17 cconstant message-y  \ row for game messages

: message  ( ca len -- )
  2dup message-y in-text-color center-type  1500 ms
       message-y center-type-blank  ;
  \ Print a game message _ca len_.

  \ ===========================================================
  cr .( Game screen)  debug-point  \ {{{1

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

: score-titles$  ( -- ca len )
  s"  SCORE<1>    RECORD    SCORE<2>"  ;

: top-status-bar  ( -- )
  home in-text-color score-titles$ type .score .record  ;
  \ XXX TODO -- support player 2

: show-player  ( -- )
  10 0 do  at-score 4 spaces 64 ms  .score 64 ms  loop  ;
  \ Show the current player by making its score blink.

: col>pixel  ( n1 -- n2 )  8 *  ;
  \ Convert a row (0..31) to a pixel y coordinate (0..255).
  \ XXX TODO -- Move to Solo Forth and rewrite in Z80

: row>pixel  ( n1 -- n2 )  8 * 191 swap -  ;
  \ Convert a row (0..23) to a pixel y coordinate (0..191).
  \ XXX TODO -- Move to Solo Forth and rewrite in Z80

: color-ruler  ( -- )
  [ attributes rows 2- columns * + ] literal
  columns ruler-color# fill  ;

: ruler  ( -- )
  [ 0 tank-y row>pixel 8 - pixel-addr nip ] literal
  columns $FF fill  color-ruler  ;
  \ Draw the ruler of the status bar.

: at-lifes  ( -- )  0 status-bar-y at-xy  ;
  \ Set the cursor position to the position of the life icons.

: spare-lifes  ( -- n )  lifes @ 1- 0 max  ;
  \ Number of spare lifes.

: .lifes  ( -- )
  at-lifes in-life-color
  spare-lifes 0 ?do  tank$ type  loop  udg/tank spaces  ;
  \ Print one icon for each spare life.

: bottom-status-bar  ( -- )  ruler .lifes  ;
  \ Draw the status bar.

: status-bars  ( -- )  top-status-bar bottom-status-bar  ;
  \ Show the data bars.

  \ ===========================================================
  cr .( Invaders data)  debug-point  \ {{{1

                    0 cconstant invaders-min-x
columns udg/invader - cconstant invaders-max-x

10 cconstant max-invaders
10 cconstant actual-invaders  \ XXX TMP -- for debugging

: half  ( -- )
  [ max-invaders 2/ ] literal !> actual-invaders  ;
  \ Reduce the actual invaders to the left half.
  \ XXX TMP -- for debugging and testing

0

  \ XXX TODO -- reorder for speed: place the most used fields
  \ at cell offsets +0, +1, +2, +4

  \ XXX TODO -- use `cfield:` for speed

  field: ~active  \ XXX TODO -- use `~stamina` instead
  field: ~y
  field: ~x
  field: ~initial-x
  field: ~sprite
  field: ~flying-sprite
  field: ~docked-sprite
  field: ~frame
  field: ~frames
  field: ~x-inc
  field: ~initial-x-inc
  field: ~destroy-points
  field: ~retreat-points
  field: ~stamina
  field: ~retreating
cconstant /invader

max-invaders /invader * constant /invaders

create invaders-data /invaders allot
  \ Invaders data table.

: 'invader   ( -- a )
  current-invader @ /invader * invaders-data +  ;
  \ Address _a_ of the current invader data.

: invader-active          ( -- a )  'invader ~active  ;
: invader-sprite          ( -- a )  'invader ~sprite  ;
: invader-flying-sprite   ( -- a )  'invader ~flying-sprite  ;
: invader-docked-sprite   ( -- a )  'invader ~docked-sprite  ;
: invader-frame           ( -- a )  'invader ~frame  ;
: invader-frames          ( -- a )  'invader ~frames  ;
: invader-destroy-points  ( -- a )  'invader ~destroy-points  ;
: invader-stamina         ( -- a )  'invader ~stamina  ;
: invader-retreat-points  ( -- a )  'invader ~retreat-points  ;
: invader-retreating      ( -- a )  'invader ~retreating  ;
: invader-x               ( -- a )  'invader ~x  ;
: invader-initial-x       ( -- a )  'invader ~initial-x  ;
: invader-x-inc           ( -- a )  'invader ~x-inc  ;
: invader-initial-x-inc   ( -- a )  'invader ~initial-x-inc  ;
: invader-y               ( -- a )  'invader ~y  ;

: invader-xy@    ( -- x y )  invader-y 2@  ;

: .y/n  ( f -- )  if  ." Y"  else  ." N"  then  space  ;
  \ XXX TMP -- for debugging

: ~~invader-info  ( -- )
  home current-invader @ 2 .r
  ." Ret.:" invader-retreating @ .y/n
  ." Sta.:" invader-stamina @ .  ;
  \ XXX TMP -- for debugging

  \ ' ~~invader-info ' ~~app-info defer!
  \ XXX TMP -- for debugging

3 cconstant max-stamina
4 cconstant undocked-invader-frames
3 cconstant docked-invader-frames

: init-invader-data  ( n1 n2 n3 c4 c5 n6 n7 n0 -- )
  current-invader !  max-stamina invader-stamina !
  invader-retreat-points !  invader-destroy-points !
  invader-flying-sprite !
  dup invader-docked-sprite !  invader-sprite !
  invader-initial-x-inc !
  dup invader-initial-x !  invader-x !  invader-y !
  docked-invader-frames invader-frames !  ;
  \ Init data of invader_n0_ with default values:
  \   n1 = y;
  \   n2 = x = initial x;
  \   n3 = initial x inc;
  \   c4 = docked sprite;
  \   c5 = flying sprite;
  \   n6 = points for destroy;
  \   n7 = points for retreat.
  \ Other fields don't need initialization, because they
  \ contain zero (default) or a constant.

: invader-1-data  ( -- c1 c2 n3 n4 )
  docked-invader-1 flying-invader-1 10 1  ;
  \ Data specific to invader type 1
  \   c1 = docked sprite;
  \   c2 = flying sprite;
  \   n3 = points for destroy;
  \   n4 = points for retreat.

: invader-2-data  ( -- c1 c2 n3 n4 )
  docked-invader-2 flying-invader-2 20 2  ;
  \ Data specific to invader type 2.
  \   c1 = docked sprite;
  \   c2 = flying sprite;
  \   n3 = points for destroy;
  \   n4 = points for retreat.

: invader-3-data  ( -- c1 c2 n3 n4 )
  docked-invader-3 flying-invader-3 30 3  ;
  \ Data specific to invader type 3.
  \   c1 = docked sprite;
  \   c2 = flying sprite;
  \   n3 = points for destroy;
  \   n4 = points for retreat.

: init-invaders-data  ( -- )
  invaders-data /invaders erase
  13 invaders-max-x -1 invader-1-data
  11 invaders-max-x -1 invader-1-data
   9 invaders-max-x -1 invader-2-data
   7 invaders-max-x -1 invader-2-data
   5 invaders-max-x -1 invader-3-data
  13 invaders-min-x  1 invader-1-data
  11 invaders-min-x  1 invader-1-data
   9 invaders-min-x  1 invader-2-data
   7 invaders-min-x  1 invader-2-data
   5 invaders-min-x  1 invader-3-data
  max-invaders 0 ?do  i init-invader-data  loop  ;
  \ Init the data of all invaders.

create invader-colors  ( -- a )
  dying-invader-color#    c,
  wounded-invader-color#  c,
  sane-invader-color#     c,
  \ Table to index the invader stamina to its proper color.

: invader-proper-color  ( -- n )
  invader-stamina @ [ invader-colors 1- ] literal + c@  ;
  \ Invader proper color for its stamina.

  \ ===========================================================
  cr .( Building)  debug-point  \ {{{1

 4 cconstant building-top-y
15 cconstant building-bottom-y

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

  \ ===========================================================
  cr .( Levels)  debug-point  \ {{{1

9 cconstant max-level

: increase-level  ( -- )  level @ 1+ max-level min level !  ;
  \ Increase the level number.

variable used-projectiles  used-projectiles off
  \ Counter.

: level-bonus  ( -- n )
  level @ 100 *  lifes @ 10 * +  used-projectiles @ -  0 max  ;
  \ Return bonus _n_ after finishing a level.

: next-level  ( -- )  level-bonus update-score
                      increase-level  set-building-size  ;
  \ Change to the next level.

: init-level  ( -- )  level off  next-level  ;
  \ Init the level number and the related variables.

: level-message  ( -- ca len )
  in-text-color s" LEVEL " level @ s>d <# # #> s+  ;

: show-level  ( -- )  level-message message  ;

  \ ===========================================================
  cr .( Tank)  debug-point  \ {{{1

variable tank-x  \ column

: init-tank  ( -- )  columns udg/tank - 2/ tank-x !  ;
  \ Init the tank.

                    1 cconstant tank-min-x
columns udg/tank - 1- cconstant tank-max-x
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
  \ Set the cursor position at the tank's coordinates.

: tank-ready  ( -- )  at-tank .tank  ;
  \ Print the tank at its current position.

: -tank  ( -- )  at-tank in-arena-color udg/tank spaces  ;
  \ Delete the tank.

: move-tank  ( -1|1 -- )
  tank-x @ + tank-range dup tank-x ! tank-y at-xy  ;
  \ Increment the column of the tank with the given value, then
  \ set the cursor position to the coordinates of the tank.

: drive  ( -- )
  update-transmission
  moving-tank? ?dup 0= ?exit  -tank move-tank .tank  ;

  \ XXX TODO -- don't delete the whole tank every time, but
  \ only the character not overwritten by the new position

  \ ===========================================================
  cr .( Projectiles)  debug-point  \ {{{1

%111 value max-projectile#
  \ Bitmask for the projectile counter (0..7).
  \ XXX TODO -- try %1111 and %11111

max-projectile# 1+ cconstant #projectiles
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

defer .debug-data  ( -- )
' noop ' .debug-data defer!
defer debug-data-pause  ( -- )
' wait ' debug-data-pause defer!

: (.debug-data)  ( -- )
  9 23 at-xy ." Ammo:" xdepth .
             ." Depth:" depth .
             ." Curr.:" projectile# .
             debug-data-pause  ;
  \ XXX INFORMER

: destroy-projectile  ( -- )
  0 projectile-y c!  projectile# >x
  .debug-data  ;

: init-projectiles  ( -- )
  used-projectiles off
  'projectile-y #projectiles erase
  xclear #projectiles 0 do  i >x  loop  ;

: projectile-coords  ( -- x y )
  projectile-x c@ projectile-y c@  ;
  \ Coordinates of the projectile.

  special-debug-point \ ' -1|1 cr u.  \ XXX INFORMER

  \ ===========================================================
  cr .( Init)  debug-point  \ {{{1

4 cconstant max-lifes
  \ Maximum number of lifes, including the first one.

: init-lifes  ( -- )  max-lifes lifes !  ;

: init-game  ( -- )
  [pixel-projectile] [ 0= ] [if]  init-ocr  [then]
  init-lifes  init-level  score off
  cls status-bars  ;
  \ Init the game.

  special-debug-point \ ' -1|1 cr u.  \ XXX INFORMER

: parade  ( -- )
  in-invader-color
  flying-invader-1 dup flying-invader-2 dup flying-invader-3
  building-bottom-y [ building-top-y 1+ ] literal
  do
    invaders-min-x i at-xy dup .2x1sprite
    invaders-max-x i at-xy     .2x1sprite
  2 +loop  ;
  \ Show the invaders at their initial positions.

  special-debug-point \ ' -1|1 cr u.  \ XXX INFORMER

: init-arena  ( -- )   -arena building tank-ready parade  ;

  \ ===========================================================
  cr .( Instructions)  debug-point  \ {{{1

  special-debug-point \ ' -1|1 cr u.  \ XXX INFORMER

: title  ( -- )
  s" NUCLEAR INVADERS" 0 center-type
  version 1 center-type  ;

  special-debug-point  \ XXX INFORMER

: (c)  ( -- )  127 emit  ;
  \ Print the copyright symbol.

  special-debug-point  \ XXX INFORMER

: .copyright  ( -- )
  row
  1 over    at-xy (c) ."  2016 Marcos Cruz"
  8 swap 1+ at-xy           ." (programandala.net)"  ;
  \ Print the copyright notice at the current coordinates.

  special-debug-point  \ XXX INFORMER

  \ : f  ( "name" -- )
  \   s" show-copyright" defined ?dup 0exit find-name-from u.  ;

: show-copyright  ( -- )  0 22 at-xy .copyright  ;

  \ XXX OLD -- maybe useful in a future version
  \ : .control  ( n -- )  ."  = " .kk# 4 spaces  ;
  \ : .controls  ( -- )
  \   row dup s" [Space] to change controls:" rot center-type
  \   9 over 2+  at-xy ." Left " kk-left#  .control
  \   9 over 3 + at-xy ." Right" kk-right# .control
  \   9 swap 4 + at-xy ." Fire " kk-fire#  .control  ;
  \   \ Print controls at the current row.

  special-debug-point  \ XXX INFORMER

: left-key$   ( -- ca len )  kk-left# kk#>string  ;

  special-debug-point  \ XXX INFORMER

: right-key$  ( -- ca len )  kk-right# kk#>string  ;

  special-debug-point  \ XXX INFORMER

: fire-key$   ( -- ca len )  kk-fire# kk#>string  ;

  special-debug-point  \ XXX INFORMER

: controls$  ( -- ca len )
  left-arrow$ left-key$ s+
  s"   " s+ fire-key$ s+ s"   " s+
  right-key$ s+ right-arrow$ s+  ;
  \ String containing the description of the current controls.
  \ XXX TMP --
  \ XXX TODO -- rewrite

  special-debug-point  \ XXX INFORMER

: .controls  ( -- )
  \ s" [Space] to change controls:" row dup >r center-type
  row >r fire-button$ r@ 2+ center-type
  0 r@ 3 + at-xy columns spaces
  controls$ r> 3 + center-type  ;
  \ Print controls at the current row.
  \ XXX TMP --

  special-debug-point  \ XXX INFORMER

true [if]  \ XXX OLD

: .score-item  ( ca1 len1 ca2 len2 -- )
  type in-text-color ."  = " type  ;
  \ Print an item of the score table, with sprite string _ca2
  \ len2_ and description _ca1 len1_

  special-debug-point  \ XXX INFORMER

: .score-table  ( -- )
  xy 2dup  at-xy s" 10 points"
           in-invader-color flying-invader-1$ .score-item
  2dup 1+  at-xy s" 20 points"
           in-invader-color flying-invader-2$ .score-item
  2dup 2+  at-xy s" 30 points"
           in-invader-color flying-invader-3$ .score-item
       3 + at-xy s" bonus"
           in-ufo-color ufo$ .score-item  ;
   \ Print the score table at the current coordinates.

  special-debug-point  \ XXX INFORMER

[else]  \ XXX NEW

: .score-item  ( c1 c2 n3 n4 -- )
  color! drop swap .2x1sprite
  in-text-color ." = " . ." points" drop  ;
  \ Print an item of the score table:
  \   c1 = docked sprite;
  \   c2 = flying sprite;
  \   n3 = points for destroy;
  \   n4 = points for retreat.
  \   n5 = color

: ufo-data  ( -- x1 c2 n3 x4 )
  docked-invader-1 flying-invader-1 10 1  ;
  \ Data specific to the UFO:
  \   x1 = fake datum;
  \   c2 = sprite;
  \   n3 = points for destroy;
  \   x4 = fake datum.
  \
  \ This is word mimics the correspondent invader words,
  \ in order to use `.score-item` also with the UFO.

: .score-table  ( -- )
  xy 2dup  at-xy invader-1-data invader-color# .score-item
  2dup 1+  at-xy invader-2-data invader-color# .score-item
  2dup 2+  at-xy invader-3-data invader-color# .score-item
       3 + at-xy ufo-data       ufo-color#     .score-item  ;
   \ Print the score table at the current coordinates.

[then]

  special-debug-point  \ XXX INFORMER

: show-score-table  ( -- )  9 4 at-xy .score-table  ;

: change-players  ( -- )
  players @ 1+ dup max-player > if  drop 1  then  players !  ;

: .players  ( -- )  ." [P]layers " players ?  ;
   \ Print the number of players at the current coordinates.

: show-players  ( -- )
  exit  \ XXX TMP --
  0 8 at-xy .players  ;

: show-controls  ( -- )
  0 12 at-xy .controls
  \ s" SPACE: change - ENTER: start" 18 center-type  ; XXX TMP
  s" ENTER: start" 18 center-type  ;
  \ XXX TMP --

: menu  ( -- )
  begin
    break-key? if  quit  then  \ XXX TMP
    key case
    enter-key of  exit  endof  \ XXX TMP --
    \ bl  of  next-controls show-controls  endof
    \ 'p' of  change-players show-players  endof
    \ XXX TMP --
    endcase
  again  ;

: instructions  ( -- )
  init-colors in-text-color cls title
  show-score-table show-players show-controls show-copyright
  menu  ;

  \ ===========================================================
  cr .( Invasion)  debug-point  \ {{{1

variable invaders  \ counter

: init-invaders  ( -- )
  init-invaders-data  current-invader off
  actual-invaders invaders !  ;
  \ Init the invaders.

: at-invader  ( -- )  invader-xy@ at-xy  ;
  \ Set the cursor position at the coordinates of the invader.

: next-frame  ( n1 -- n2 )  1+ dup invader-frames @ < and  ;
  \ Increase frame _n1_ resulting frame _n2_.

: invader-udg  ( -- c )
  invader-frame @ dup next-frame invader-frame !
  [ udg/invader 2 = ] [if]  2*  [else]  udg/invader *  [then]
  invader-sprite @ +  ;
  \ UDG _c_ of the current invader, calculated from its
  \ sprite and its frame.

: .invader  ( -- )
  invader-proper-color color! invader-udg .2x1sprite  ;
  \ Print the current invader.

variable broken-wall-x
  \ Column of the wall broken by the current alien.

: flying-to-the-right?  ( -- f )  invader-x-inc @ 0>  ;
  \ Is the current invader flying to the right?

: retreating?  ( -- f )  invader-retreating @  ;
  \ Is the current invader retreating?

: attacking?  ( -- f )  retreating? 0=  ;
  \ Is the current invader attacking?

: broken-bricks-coordinates  ( -- x1 y1 x2 y2 )
  broken-wall-x @ invader-y @ 1+  2dup 2-  ;
  \ Coordinates of the broken brick above the invader, _x2 y2_,
  \ and below it, _x1 y1_.

: broken-left-wall  ( x1 y1 x2 y2 -- )
  at-xy broken-top-left-brick .1x1sprite
  at-xy broken-bottom-left-brick  .1x1sprite  ;
  \ Print the broken left wall at the given coordinates of the
  \ broken brick above the invader, _x2 y2_, and below it, _x1
  \ y1_.

: broken-right-wall  ( x1 y1 x2 y2 -- )
  at-xy broken-top-right-brick .1x1sprite
  at-xy broken-bottom-right-brick .1x1sprite  ;
  \ Print the broken right wall at the given coordinates of the
  \ broken brick above the invader, _x2 y2_, and below it, _x1
  \ y1_.

: broken-wall  ( -- )
  in-broken-wall-color  broken-bricks-coordinates
  flying-to-the-right? if    broken-left-wall
                       else  broken-right-wall  then  ;
  \ Show the broken wall of the building, hit by the current
  \ invader.

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

: ?damages  ( -- )
  broken-wall? if  broken-wall exit  then
  broken-container? dup if    broken-container
                              \ invader-stamina off
                                \ XXX TMP -- for debugging
                              \ invader-active off
                                \ XXX TMP -- for debugging
                        then  catastrophe !  ;
  \ Manage the possible damages caused by the current invader.

: at-home?  ( -- f )  invader-x @ invader-initial-x @ =  ;
  \ Is the current invader at its start position?

: turn-back  ( -- )
  invader-x-inc @ negate invader-x-inc !
  invader-retreating @ invert invader-retreating !  ;
  \ Make the current invader turn back.
  \
  \ XXX TODO -- write `negate!` and `invert!` in Z80 in Solo
  \ Forth's library

: dock  ( -- )
  invader-active off
  invader-x-inc off  invader-retreating off
  invader-docked-sprite @ invader-sprite !
  invader-frame off
  docked-invader-frames invader-frames !  ;
  \ Dock the current invader.

: undock  ( -- )
  invader-active on
  invader-initial-x-inc @ invader-x-inc !
  invader-flying-sprite @ invader-sprite !
  invader-frame off
  undocked-invader-frames invader-frames !  ;
  \ Undock the current invader.

: ?dock  ( -- )  at-home? 0exit dock  ;
  \ If the current invader is at home, dock it.

: left-flying-invader  ( -- )
  -1 invader-x +! at-invader .invader space  ;
  \ Move the current invader, which is flying to the left.

: right-flying-invader  ( -- )
  at-invader in-arena-color space .invader 1 invader-x +!  ;
  \ Move the current invader, which is flying to the right.

: flying-invader  ( -- )
  flying-to-the-right? if    right-flying-invader
                       else  left-flying-invader  then
           retreating? if    ?dock
                       else  ?damages  then  ;

variable cure-factor  20 cure-factor !
  \ XXX TMP -- for testing

: difficult-cure?  ( -- f )
  max-stamina invader-stamina @ -
  cure-factor @  \ XXX TMP -- for testing
  * random 0<>  ;
  \ Is it a difficult cure? The flag _f_ is calculated
  \ randombly, based on the stamina: The less stamina, the more
  \ chances to be a difficult cure. This is used to delay the
  \ cure.

: cure  ( -- )
  invader-stamina @ 1+ max-stamina min invader-stamina !  ;
  \ Cure the current invader, increasing its stamina.

: ?cure  ( -- )  difficult-cure? ?exit cure  ;
  \ Cure the current invader, depending on its status.

: healthy?  ( -- f )  invader-stamina @ max-stamina =  ;
  \ Is the current invader healthy? Has it got maximum stamina?

: ?undock  ( -- )  invaders @ random ?exit undock  ;
  \ Undock the current invader randomly, depending on the
  \ number of invaders.

: require-invader  ( -- )
  healthy? if    ?undock
           else  ?cure
           then  at-invader .invader  ;
  \ Require the current invader, either inactive or wounded.

: last-invader?  ( -- f )
  \ current-invader @ [ actual-invaders 1- ] literal =  ;
  current-invader @ actual-invaders 1- =  ;
  \ XXX TMP -- for debugging, calculate at run-time
  \ Is the current invader the last one?

: next-invader  ( -- )
  last-invader? if    current-invader off   exit
                then  1 current-invader +!  ;
  \ Update the invader to the next one.

: move-invader  ( -- )
   invader-active @ if  flying-invader exit  then
  invader-stamina @ if  require-invader      then  ;
  \ Move the current invader if it's active; else
  \ just try to activate it, if it's alive.

: (invasion)  ( -- )  move-invader  next-invader  ;
  \ Move the current invader, then choose the next one.

  \ 10. 2const invasion-delay-ms \ XXX TODO --
8 constant invader-time

defer invasion  \ XXX TMP --

: invasion-wait  ( -- )
  frames@ invader-time s>d d+ (invasion)
  begin  frames@ 2over d< 0=  until  2drop  ;
  \ Move the current invader, if there are units left of it,
  \ and then choose the next one.
  \ XXX TMP --
  \ XXX REMARK --
  \ invader-time = 4 -- works, but too slow
  \ invader-time = 3 -- works a bit, but too slow
  \ invader-time = 2 -- no effect

  \ XXX TODO -- alternative to
  \ make sure the action takes always a fixed time:
  \ do `frames@ invader-interval dmod ?exit` at the start.

: invasion-check  ( -- )
  os-frames c@ invader-time mod ?exit (invasion)  ;
  \ XXX TMP --
  \ XXX REMARK --
  \ invader-time = 10 -- they hardly move
  \ invader-time = 4 -- they move very slowly
  \ invader-time = 3 -- they dont move
  \ invader-time = 2 -- they dont move

' (invasion) ' invasion defer!  \ XXX TMP --

  \ ===========================================================
  cr .( UFO)  debug-point  \ {{{1

  \ XXX UNDER DEVELOPMENT
  \ XXX FIXME --

  \ XXX TODO -- simplify: the UFO can be always visible

3 cconstant ufo-y

variable ufo-x
variable ufo-x-inc  -1|1 ufo-x-inc !
variable ufo-frame  \ counter (0..3)

: ~~ufo-info  ( -- )
  home ." x:" ufo-y ?  ." inc.:" ufo-x-inc ?  ;
  \ ' ~~ufo-info ' ~~app-info defer!
  \ XXX TMP -- for debugging

: ufo-returns  ( -- )  ufo-x-inc @ negate ufo-x-inc !  ;

96 constant ufo-limit-x
  \ Limit of the x coordinate of the UFO in either direction.

: init-ufo  ( -- )  ufo-limit-x ufo-x !  ;
  \ Init the UFO.

columns udg/ufo - constant ufo-max-x
                0 constant ufo-min-x

: visible-ufo?  ( -- f )
  ufo-x @ ufo-min-x ufo-max-x between  ;
  \ Is the UFO visible?

: at-ufo  ( -- )  ufo-x @ 0 max ufo-max-x min ufo-y at-xy  ;
  \ Set the cursor position at the coordinates of the visible
  \ part of the UFO.

: -ufo  ( -- )  at-ufo udg/ufo spaces  ;
  \ Delete the visible part of the UFO.

: ufo-udg  ( -- c )
  ufo-frame @ dup next-frame ufo-frame !
  [ udg/ufo 2 = ] [if]  2*  [else]  udg/ufo *  [then]
  ufo +  ;
  \ UDG _c_ of the UFO.

: advance-ufo  ( -- )  ufo-x-inc @ ufo-x +!  ;
  \ Advance the UFO on its current direction,
  \ adding its x increment to its x coordinate.

: ufo-in-range?  ( -- f )  ufo-limit-x ufo-x @ abs <  ;
  \ Is the UFO in the range of its flying limit?

: .ufo  ( -- )  in-ufo-color ufo-udg .2x1sprite  ;

0 [if]  \ XXX OLD

: move-ufo-to-the-right  ( -- )
  at-ufo in-arena-color space .ufo  ;

: move-ufo-to-the-left  ( -- )
  at-ufo .ufo in-arena-color space  ;

      ' move-ufo-to-the-left ,
here  ' noop ,
      ' move-ufo-to-the-right ,

constant ufo-movements  ( -- a )
  \ Execution table to move the UFO.

: move-ufo  ( -- )  ufo-movements ufo-x-inc @ +perform  ;
  \ Execute the proper movement.

[then]

: manage-visible-ufo  ( -- )
  -ufo advance-ufo visible-ufo? 0= ?exit .ufo  ;
  \ Manage the UFO, when it's visible.
  \ XXX TODO -- improve: don't delete the whole UFO

: manage-invisible-ufo  ( -- )
  advance-ufo visible-ufo? if  .ufo exit  then
  ufo-in-range? ?exit ufo-returns  ;
  \ Manage the UFO, when it's invisible.

: manage-ufo  ( -- )
  visible-ufo? if  manage-visible-ufo    exit  then
                   manage-invisible-ufo  ;
  \ Manage the UFO.

  \ ===========================================================
  cr .( Impact)  debug-point  \ {{{1

: ufo-bang  ( -- )  18 12 do  i 15 beep  loop  ;
  \ Make the explosion sound of the UFO.
  \ XXX TODO -- 128 sound

: ufo-on-fire  ( -- )
  ufo-x @ 1+ ufo-y at-xy ufo-explosion$ type  ;
  \ Show the UFO on fire.

: ufo-explosion  ( -- )  ufo-on-fire ufo-bang -ufo  ;
  \ Show the explosion of the UFO.

: ufo-points  ( -- n )  5 random 1+ 10 * 50 +  ;
  \ Random points for impacting the UFO.

: ufo-bonus  ( -- )  ufo-points update-score  ;
  \ Update the score with the UFO bonus.

: ufo-impacted  ( -- )  ufo-explosion ufo-bonus  ;
  \ The UFO has been impacted.

: invader-bang  ( -- ca len )  10 100 beep  ;
  \ Make the explosion sound of the current invader.
  \ XXX TODO -- explosion 128 sound

: invader-on-fire  ( -- )
  at-invader invader-explosion$ type  ;
  \ Show the current invader on fire.

: -invader  ( -- )  in-arena-color at-invader 2 spaces  ;
  \ Delete the current invader.

: invader-explosion  ( -- )
  invader-on-fire invader-bang -invader  ;
  \ Show the explosion of the current invader.

: impacted-invader  ( -- n )
  projectile-y c@ [ building-top-y 1+ ] literal - 2/
  projectile-x c@ [ columns 2/ ] literal > abs 5 * +  ;
  \ Return the impacted invader (0..9), calculated from the
  \ projectile coordinates: Invaders 0 and 5 are at the top,
  \ one row below the top of the building; 1 and 6 are two rows
  \ below and so on.  0..4 are at the left of the screen; 5..9
  \ are at the right.

: explode  ( -- )
  invader-destroy-points @  update-score  invader-explosion
  -1 invaders +!  invader-stamina off  invader-active off  ;
  \ The current invader explodes.

: retreat  ( -- )
  invader-retreat-points @ update-score  turn-back  ;
  \ The current invader retreats.

: wounded  ( -- )  invader-stamina @ 1- invader-stamina !  ;
  \ Reduce the invader's stamina after being shoot.

: mortal?  ( -- f )  invader-stamina @ 2* random 0=  ;
  \ Is it a mortal impact?  _f_ depends on a random calculation
  \ based on the stamina: The more stamina, the less chances to
  \ be true.  If stamina is zero, _f_ is always true.

: (invader-impacted)  ( -- )
  wounded mortal? if    explode
                  else  attacking? if  retreat  then  then  ;
  \ The current invader has been impacted by the projectile.
  \ If the wound is mortal, it explodes; else, if attacking, it
  \ retreats.

: invader-impacted  ( -- )
  current-invader @ >r  impacted-invader current-invader !
  (invader-impacted)  r> current-invader !  ;
  \ An invader has been impacted by the projectile.
  \ Make it the current one and manage it.

: ufo-impacted?  ( -- f )
  [pixel-projectile]
  [if]    projectile-coords pixel-attr-addr c@ ufo-color# =
  [else]  projectile-y c@ ufo-y =  [then]  ;

: impact  ( -- )
  ufo-impacted? if    ufo-impacted
                else  invader-impacted
                then  destroy-projectile  ;

: hit-something?  ( -- f )
  projectile-coords  [pixel-projectile]
  [if]    pixel-attr-addr c@ arena-color# <>
  [else]  ocr 0<>
  [then]  ;
  \ Did the projectile hit something?

: impacted?  ( -- f )
  hit-something? dup if  impact  then  ;
  \ Did the projectil impacted?
  \ If so, do manage the impact.

  \ ===========================================================
  cr .( Shoot)  debug-point  \ {{{1

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
  [else]  projectile-coords attr projectile-color# <> ?exit
          at-projectile in-arena-color space
  [then]  ;
  \ Delete the projectile.

: projectile-lost?  ( -- f )
  projectile-y c@
  [pixel-projectile]
  [if]    [ building-top-y row>pixel ] cliteral >
  [else]  [ building-top-y 1- ] cliteral <
  [then]  ;
  \ Is the projectile lost?

: move-projectile  ( -- )
  -projectile
  projectile-lost? if  destroy-projectile exit  then
  [pixel-projectile] [if]    7
                     [else]  -1
                     [then]  projectile-y c+!
  impacted? ?exit .projectile  ;
  \ Manage the projectile.

variable trigger-delay-counter  trigger-delay-counter off

[pixel-projectile] [if]    8
                   [else]  6
                   [then]  value trigger-delay

: delay-trigger  ( -- )
  trigger-delay trigger-delay-counter !  ;

: fire  ( -- )
  1 used-projectiles +!
  x> to projectile#  .debug-data
  new-projectile-x projectile-x c!
  [pixel-projectile]
  [if]    [ tank-y row>pixel 1+ ] literal
  [else]  [ tank-y 1- ] literal
  [then]  projectile-y c!
  .projectile fire-sound delay-trigger  ;
  \ The tank fires.
  \ XXX TODO -- confirm `tank-y 1-`

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
  update-trigger
  trigger-pressed? 0exit
  trigger-ready? 0exit
  projectile-left? 0exit
  gun-below-building? ?exit  fire  ;
  \ Manage the shoot.

: new-record?   ( -- f )  score @ record @ >  ;
  \ Is there a new record?

: new-record    ( -- )  score @ record !  ;
  \ Set the new record.

: check-record  ( -- )  new-record? if  new-record  then  ;
  \ Set the new record, if needed.

  \ ===========================================================
  cr .( Players config)  debug-point  \ {{{1


  \ ===========================================================
  cr .( Main)  debug-point  \ {{{1

: .game-over  ( -- )  s" GAME OVER" message  ;

: game-over  ( -- )  .game-over check-record  ;

: dead  ( -- )  -1 lifes +!  .lifes  ;
  \ One life lost.

: defeat-tune  ( -- )  100 200 do  i 20 beep  -5 +loop  ;
  \ XXX TODO -- 128 sound

create attributes-backup /attributes allot

: save-attributes  ( -- )
  attributes attributes-backup /attributes cmove  ;

: restore-attributes  ( -- )
  attributes-backup attributes /attributes cmove  ;

: radiation  ( -- )
  [ attributes columns 2 * + ] literal
  [ /attributes columns 4 * - ] literal
  radiation-color# fill  ;

: defeat  ( -- )
  save-attributes
  radiation defeat-tune  2000 ms  fade dead
  restore-attributes  ;
  \ XXX TODO -- finish

: victory?  ( -- f )  invaders @ 0=  ;

variable invasion-delay  8 invasion-delay !
  \ XXX TMP -- debugging

: init-combat  ( -- )
  catastrophe off
  init-invaders init-ufo init-tank init-arena init-projectiles
  show-level show-player  ;

: (combat)  ( -- )
  begin   victory? if  next-level init-combat  then

          break-key? if  quit  then  \ XXX TMP

          fly-projectile  drive
          fly-projectile  shoot
          fly-projectile
          manage-ufo  \ XXX TMP --
          fly-projectile
          \ invasion-delay @ random if \ XXX TMP -- debugging
          invasion
          \ then \ XXX TMP -- debugging
          fly-projectile  catastrophe @
          \ drop false
            \ XXX TMP -- for debugging
  until   defeat  ;

: combat  ( -- )  init-combat (combat)  ;

: defeat?  ( -- f )  lifes @ 0=  ;

: game  ( -- )
  init-game  begin  combat defeat?  until  game-over  ;

: run  ( -- )  begin  instructions game  again  ;

  \ ===========================================================
  cr .( Debugging tools)  debug-point  \ {{{1

: .udgs  ( -- )  cr udgs 0 do  i 128 + emit  loop  ;
  \ Print all game UDGs.

: ni  ( -- )      next-invader  ;
: mi   ( -- )     move-invader  ;
: ini  ( -- )      init-game init-combat  ;

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

cr cr .( Nuclear Invaders)
   cr version type  \ XXX FIXME --
   cr .( Ready)
   cr .unused
   cr .( Type RUN to start) cr

end-app

  \ vim: filetype=soloforth:colorcolumn=64
