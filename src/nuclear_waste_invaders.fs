  \ nuclear_waste_invaders.fs

  \ This file is part of Nuclear Waste Invaders
  \ http://programandala.net/en.program.nuclear_waste_invaders.html

( nuclear-waste-invaders )

\ A game for ZX Spectrum 128, written in Forth with Solo Forth
\ (http://programandala.net/en.program.solo_forth.html).

\ Copyright (C) 2016,2017,2018 Marcos Cruz (programandala.net)

\ =============================================================
\ License

\ You may do whatever you want with this work, so long as you
\ retain all copyright, authorship, acknowledgment and credit
\ notices and this license in all redistributed copies and
\ derived works.  There is no warranty.

\ =============================================================
\ Credit

\ Nuclear Waste Invaders was inspired by and initially based on
\ Nuclear Invaders (Copyright 2013 Scainet Soft), written by
\ Dancresp for Jupiter ACE:
\ http://www.zonadepruebas.com/viewtopic.php?t=4231.

\ =============================================================

only forth definitions

wordlist dup constant nuclear-waste-invaders-wordlist
         dup >order set-current

: version$ ( -- ca len ) s" 0.134.0+201801061358" ;

cr cr .( Nuclear Waste Invaders) cr version$ type cr

  \ ===========================================================
  cr .( Options) \ {{{1

  \ Flags for conditional compilation of new features under
  \ development.

false constant [pixel-projectile] immediate
  \ Pixel projectiles (new method) instead of UDG projectiles
  \ (old method)?
  \
  \ XXX TODO -- finish support for pixel projectiles

  \ ===========================================================
  cr .( Library) \ {{{1

blk @ 1- last-locatable !
  \ Don't search this source for requisites, just in case.

forth-wordlist set-current

  \ --------------------------------------------
  \ Resize the stringer

  \ The default size of the stringer is 256 bytes, which is not
  \ enough to load some nested requirements.

need !>

  \ stringer /stringer 2constant old-stringer
  \ Preserve the old `stringer`, to reuse its space later.
  \ XXX TODO --

here 512 dup allot !> /stringer !> stringer empty-stringer
  \ Create a new 512-byte `stringer` in data space.

  \ --------------------------------------------
  cr .(   -Development tools) .s \ {{{2

need [if] need ~~
need warn.message need order need see need rdepth need where
need evaluate need ?depth need see-colon-body>

  \ --------------------------------------------
  cr .(   -Definers) ?depth \ {{{2

need defer need alias need cvariable
need 2const need cenum need c!>

  \ --------------------------------------------
  cr .(   -Strings) ?depth \ {{{2

need upper need s+ need char>string need s\"

  \ --------------------------------------------
  cr .(   -Control structures) ?depth \ {{{2

need case need 0exit need +perform need do need abort"

  \ --------------------------------------------
  cr .(   -Memory) ?depth \ {{{2

need c+! need c-! need c1+! need c1-! need ?c1-! need coff
need dzx7t need bank-start need c@1+ need c@1- need c@2+

  \ --------------------------------------------
  cr .(   -Math) ?depth \ {{{2

need d< need -1|1 need 2/ need between need random need binary
need within need even? need crnd need 8* need random-between
need join need 3*

  \ --------------------------------------------
  cr .(   -Data structures) ?depth \ {{{2

need roll need cfield: need field: need +field-opt-0124
need array> need !> need c!> need 2!>

need sconstants

need xstack need allot-xstack need xdepth need >x need x>
need xclear

need .xs \ XXX TMP -- for debuging

  \ --------------------------------------------
  cr .(   -Display) ?depth \ {{{2

need at-y need at-x need type-left-field need type-right-field
need type-center-field need gigatype-title need mode-32iso

  \ --------------------------------------------
  cr .(   -Graphics) ?depth \ {{{2

need set-udg need /udg
need columns need rows need row need fade-display
need last-column need udg-block need udg! need blackout

need window need wltype need wcr need wcls need wcolor

need ocr

[pixel-projectile] [if]
  need plot need set-pixel need reset-pixel need gxy>attra
  need x>gx need y>gy
[then]

need inverse-off need overprint-off need attr!  need attr@

need black need blue   need red   need magenta need green
need cyan  need yellow need white

need papery need brighty need xy>attr need xy>attra

  \ --------------------------------------------
  cr .(   -Keyboard) ?depth \ {{{2

need kk-ports need kk-1# need pressed? need kk-chars
need #>kk need inkey need new-key

  \ --------------------------------------------
  cr .(   -Time) ?depth \ {{{2

need ticks need ms need seconds need ?seconds need past?

  \ --------------------------------------------
  cr .(   -Sound) ?depth \ {{{2

need bleep need dhz>bleep need shoot
need whip need lightning1

  \ --------------------------------------------
  \ Files

need tape-file> need last-tape-header

  \ --------------------------------------------

nuclear-waste-invaders-wordlist set-current

  \ ===========================================================
  cr .( Debug) ?depth \ {{{1

defer debug-point  defer special-debug-point

defer ((debug-point  ' noop ' ((debug-point defer!

: (debug-point ( -- )
  ((debug-point

  \ order
  \ ." block:" blk ?  ."  latest:" latest .name ." hp:" hp@ u.
  \ order
  \ s" ' -1|1 .( -1|1 =) u." evaluate

  \ XXX TMP -- 2017-04-16: To discover the problem with
  \ `shoot`:
  \ cr ."  latest: " latest .name ." shoot nt:"
  \ s" ' shoot u." evaluate

  depth
  if decimal cr .s #-258 throw then \ stack imbalance

  \ key drop

  ;

  \ ' noop ' debug-point defer!
  ' (debug-point ' debug-point defer!
  \ XXX TMP -- for debugging

  ' noop ' special-debug-point defer!
  \ ' (debug-point ' special-debug-point defer!
  \ XXX TMP -- for debugging

  \ : :
  \   cr blk @ . latest .name ." ..."
  \   s" attributes drop " evaluate : ;
  \ XXX TMP -- for debugging

: ~~ ( -- )
  postpone attr@ postpone >r postpone red postpone attr!
  postpone ~~    postpone r> postpone attr! ;
  immediate compile-only

: XXX ( -- )
  ~~? @ 0= ?exit
  base @ >r decimal latest .name .s r> base !
  key drop ;

'q' ~~quit-key c!  $FF ~~resume-key c!  22 ~~y c!  ~~? off

: ~~stack-info ( -- )
  home ." rdepth:" rdepth . ;
' ~~stack-info ' ~~app-info defer!
  \ XXX TMP -- for debugging

: notice ( -- ) 1024 0 ?do i %11 and border loop ;

: borderx ( n -- )
  1024 0 ?do dup border white border loop drop black border ;

  \ ===========================================================
  cr .( Constants) ?depth debug-point \ {{{1

16384 constant sys-screen  6912 constant /sys-screen
                           6144 constant /sys-screen-bitmap
  \ Address and size of the screen.

22528 constant attributes  768 constant /attributes
  \ Address and size of the screen attributes.

 1 cconstant sky-top-y
15 cconstant sky-bottom-y

sky-bottom-y cconstant tank-y

  \ ===========================================================
  cr .( Screen) ?depth debug-point \ {{{1

/sys-screen negate farlimit +!
farlimit @ constant preserved
  \ Preservation buffer at the top of the far memory.

far-banks 3 + c@ cconstant preservation-bank
  \ The 4th far-memory bank, where `preserved` is.

: preservation ( ca1 ca2 len -- )
  preservation-bank bank cmove default-bank ;
  \ Copy _len_ bytes from _ca1_ to _ca2_,
  \ with the preservation buffer paged in.

: preserve-screen ( -- )
  sys-screen preserved /sys-screen preservation ;

: restore-screen ( -- )
  preserved sys-screen /sys-screen preservation ;

: preserve-attributes ( -- )
  attributes preserved /attributes preservation ;

: restore-attributes ( -- )
  preserved attributes /attributes preservation ;

  \ ===========================================================
  cr .( Landscapes) ?depth debug-point \ {{{1

sys-screen /sys-screen-bitmap 3 / 2 * + constant landscape-scra
  \ Screen address of the landscape.

attributes /attributes 3 / 2 * + constant landscape-attra
  \ Attributes address of the landscape.

8 cconstant landscapes

7 cconstant landscapes-bank
  \ Use the free bank to store the landscapes.

create landscape-bitmap> landscapes cells allot
  \ Array to store the addresses of the compressed landscape
  \ bitmaps (2048 bytes uncompressed).

create landscape-attrs> landscapes cells allot
  \ Array to store the addresses of the compressed landscape
  \ attributes (256 bytes uncompressed).

: landscape>screen ( n -- )
  landscapes-bank bank
  dup landscape-bitmap> array> @ landscape-scra  dzx7t
      landscape-attrs>  array> @ landscape-attra dzx7t
  default-bank ;
  \ Decompress landscape _n_ (0 index) from memory bank to
  \ screen.

variable landscape> bank-start landscape> !
  \ Address where the next compressed landscape bitmap or
  \ attributes will be stored, in the memory bank.

: landscape+! ( -- ) last-tape-length @ landscape> +! ;
  \ Update `landscape>` with the length of the last tape file
  \ loaded.

: (load-landscape ( n a -- )
  array> landscape> @ swap !
  0 0 landscape> @ 0 tape-file> landscape+! ;
  \ Store the address hold in `landscape>` into element _n_ of
  \ cell array _a2_.  Then read the contents of the next tape
  \ file to the address hold in `landscape>`. Finally update
  \ `landscape>` with the length of the loaded file.

: load-landscape ( n -- )
  landscapes-bank bank  dup landscape-attrs>  (load-landscape
                            landscape-bitmap> (load-landscape
  default-bank ;
  \ Load landscape _n_ (0 index) from tape and store it into
  \ the memory bank used for landscape graphics.  Each
  \ landscape consists of two compressed files: first the
  \ attributes (256 bytes uncompressed) and next the bitmap
  \ (2048 bytes uncompressed).

: load-landscapes ( -- )
  landscapes 0 ?do i load-landscape loop ;

  cr .( Insert the tape image)
  cr .( <graphics_and_font.tap>.)

load-landscapes

  \ ===========================================================
  cr .( Font) ?depth debug-point \ {{{1

1000 constant /game-font
create game-font /game-font allot

0 0 game-font /game-font tape-file>

game-font mode-32iso-font !

  \ ===========================================================
  cr .( Localization) ?depth debug-point \ {{{1

  \ XXX TODO -- Move to Solo Forth.

0 cenum en         \ English
  cenum eo         \ Esperanto
  cenum es         \ Spanish
  cconstant langs  \ number of languages

en cconstant lang  \ current language

need n,

: localized, ( x[langs]..x[1] -- ) langs n, ;

: localized-word ( xt[langs]..xt[1] "name" -- )
  create localized,
  does> ( -- ) ( pfa ) lang +perform ;
  \ Create a word _name_ that will execute an execution token
  \ from _xt[langs]..xt[1]_, depending on the current language.
  \ _xt[langs]..xt[1]_, are the execution tokens of the
  \ localized versions.  _xt[langs]..xt[1]_, are ordered by ISO
  \ language code, being TOS the first one.

: localized-string ( ca[langs]..ca[1] "name" -- )
  create localized,
  \ does> ( -- ca len ) ( pfa ) lang cells + @ count ;
  does> ( -- ca len ) ( pfa ) lang swap array> @ count ;
  \ Create a word _name_ that will return a counted string
  \ from _ca[langs]..ca[1]_, depending on the current language.
  \ _ca[langs]..ca[1]_, are the addresses where the localized
  \ strings have been compiled.  _ca[langs]..ca[1]_, are
  \ ordered by ISO language code, being TOS the first one.
  \
  \ XXX TODO -- Benchmark `cells +` vs `swap array>`.

: localized-character ( c[langs]..c[1] "name" -- c )
  create langs 0 ?do c, loop
  does> ( -- c ) ( pfa ) lang + c@ ;
  \ Create a word _name_ that will return a character
  \ from _c[langs]..c[1]_, depending on the current language.
  \ _c[langs]..c[1]_ are ordered by ISO language code, being
  \ TOS the first one.

  \ ===========================================================
  cr .( Texts) ?depth debug-point \ {{{1

here ," Invasores de Residuos Nucleares"
here ," Atomrubaĵaj Invadantoj"
here ," Nuclear Waste Invaders"
localized-string game-title$ ( -- ca len )
  \ Return game title _ca len_ in the current language.

here ," [N]o en español"
here ," [N]e en Esperanto"
here ," [N]ot in English"
localized-string not-in-this-language$ ( -- ca len )
  \ Return string _ca len_ in the current language.

'n' cconstant language-key
  \ Key to change the current language.

here ," puntos "
here ," poentoj"
here ," points "
localized-string points$ ( -- ca len )
  \ Return string _ca len_ in the current language.

here ," puntos extra"
here ," krompoentoj "
here ," bonus       "
localized-string bonus$ ( -- ca len )
  \ Return string _ca len_ in the current language.

here ," PUNTUACIÓN"
here ," POENTARO"
here ," SCORE"
localized-string score$ ( -- ca len )
  \ Return string _ca len_ in the current language.

here ," RÉCOR"
here ," RIKORDO"
here ," RECORD"
localized-string record$ ( -- ca len )
  \ Return string _ca len_ in the current language.

  \ XXX TODO -- Simplify: use `sconstants` instead, using the
  \ language as index and a wrapper word to provide it.

0 [if]

here ," jugadores"
here ," ludantoj"
here ," players"
localized-string players$ ( -- ca len )

[then]

here ," [E]mpezar"
here ," [E]ki"
here ," [S]tart"
localized-string start$ ( -- ca len )
  \ Return string _ca len_ in the current language.

'e' \ [e]mpezar
'e' \ [e]ki
's' \ [s]tart
localized-character start-key ( -- c )
  \ Return key _c_ to start the game from the main menu
  \ in the current language.

0
  here ," Ganta"
  here ," Kassari"
  here ," Zagora" \ XXX TODO -- change
  here ," Las Mesas"
  here ," Châteaubriant"
  here ," Peel"
  here ," Vestmahavn"
  here ," Longyearbyen"
sconstants >town$ ( n -- ca len ) drop
  \ Return name _ca len_ of town _n_.

0
  here ," Nimba"
  here ," " \ XXX TODO
  here ," " \ XXX TODO
  here ," Cuenca"
  here ," Pays de la Loire" \ XXX TODO -- confirm English name
  here ," Isle of Man"
  here ," Faroe Islands"
  here ," Svalbard"
sconstants >en.region$ ( n -- ca len ) drop
  \ Return English name _ca len_ of region _n_.

0
  here ," Nimba"
  here ," " \ XXX TODO
  here ," " \ XXX TODO
  here ," Kŭenko"
  here ," Luarlandoj"
  here ," Manksinsulo"
  here ," Ferooj"
  here ," Svalbardo"
sconstants >eo.region$ ( n -- ca len ) drop
  \ Return Esperanto name _ca len_ of region _n_.

0
  here ," Nimba"
  here ," " \ XXX TODO
  here ," " \ XXX TODO
  here ," Cuenca"
  here ," Países del Loira"
  here ," Isla de Man"
  here ," Islas Feroes"
  here ," Svalbard"
sconstants >es.region$ ( n -- ca len ) drop
  \ Return Spanish name _ca len_ of region _n_.

' >es.region$
' >eo.region$
' >en.region$
localized-word >region$ ( n -- ca len )

0
  here ," Liberia"
  here ," Mauritania"
  here ," Morocco"
  here ," Spain"
  here ," France"
  here ," Great Britain"
  here ," Denmark"
  here ," Norway"
sconstants >en.country$ ( n -- ca len ) drop
  \ Return English name _ca len_ of country _n_.

0
  here ," Liberio"
  here ," Mauritanio"
  here ," Maroko"
  here ," Hispanujo"
  here ," Francujo"
  here ," Britujo"
  here ," Danujo"
  here ," Norvegujo"
sconstants >eo.country$ ( n -- ca len ) drop
  \ Return Esperanto name _ca len_ of country _n_.

0
  here ," Liberia"
  here ," Mauritania"
  here ," Marruecos"
  here ," España"
  here ," Francia"
  here ," Reino Unido"
  here ," Dinamarca"
  here ," Noruega"
sconstants >es.country$ ( n -- ca len ) drop
  \ Return Spanish name _ca len_ of country _n_.

' >es.country$
' >eo.country$
' >en.country$
localized-word >(country)$ ( n -- ca len )
  \ Return name _ca len_ of country _n_ in the current language.

: >country$ ( n -- ca len )
  s" (" rot >(country)$ s+ s" )" s+ ;
  \ Return name _ca len_ (in parens) of country _n_ in the
  \ current language.

here ," Pulsa una tecla."
here ," Premu klavon."
here ," Press any key."
localized-string press-any-key$ ( -- ca len )

  \ ===========================================================
  cr .( Colors) ?depth debug-point \ {{{1

[pixel-projectile]    black and
[pixel-projectile] 0= white and +
                               cconstant sky-attr

                         green cconstant invader-attr
                         green cconstant sane-invader-attr
                        yellow cconstant wounded-invader-attr
                           red cconstant dying-invader-attr
                       magenta cconstant mothership-attr

magenta papery white + brighty cconstant beam-attr

                         white cconstant tank-attr
                        yellow cconstant projectile-attr

                  white papery cconstant unfocus-attr
  white papery brighty white + cconstant hide-report-attr
          white papery brighty cconstant reveal-report-attr
                         white cconstant text-attr

            white papery red + cconstant brick-attr
                         white cconstant door-attr
                           red cconstant broken-wall-attr
                yellow brighty cconstant container-attr
                yellow brighty cconstant radiation-attr

: init-colors ( -- ) [ white black papery + ] cliteral attr!
                     overprint-off inverse-off black border ;

  \ ===========================================================
  cr .( Global variables) ?depth debug-point \ {{{1

cvariable location          \ counter
 variable score             \ counter
 variable record record off \ max score

 cvariable invader# \ current invader's number (0..9)
0 constant invader~ \ current invader's data structure address

variable catastrophe \ flag (game end condition)

: catastrophe? ( -- f ) catastrophe @ ;

  \ ===========================================================
  cr .( Keyboard) ?depth debug-point \ {{{1

13 cconstant enter-key

0 cconstant kk-left#  0. 2constant kk-left
0 cconstant kk-right# 0. 2constant kk-right
0 cconstant kk-fire#  0. 2constant kk-fire

: wait ( -- ) begin inkey until ;
  \ Wait until any key is pressed.

: enter-key? ( -- f ) inkey enter-key = ;
  \ Is the Enter key pressed?

: wait-for-enter ( -- ) begin enter-key? until ;
  \ Wait until the Enter key is pressed.

: kk#>c ( n -- c ) kk-chars + c@ ;
  \ Convert key number _n_ to its char _c_.

: kk#>string ( n -- ca len )
  case  kk-en# of  s" Enter"         endof
        \ kk-sp# of  s" Space"         endof \ XXX OLD
        \ kk-cs# of  s" Caps Shift"    endof \ XXX OLD
        \ kk-ss# of  s" Symbol Shift"  endof \ XXX OLD
        dup kk#>c upper char>string rot \ default
  endcase ;

  \ Controls

3 cconstant /controls
  \ Bytes per item in the `controls` table.

create controls
  \ left    right     fire
  kk-5# c,  kk-8# c,  kk-en# c, \ cursor: 5-8-Enter
  kk-r# c,  kk-t# c,  kk-en# c, \ Spanish Dvorak: R-T-Enter
  kk-z# c,  kk-x# c,  kk-en# c, \ QWERTY: Z-X-Enter
  kk-5# c,  kk-8# c,  kk-0#  c, \ cursor joystick: 5-8-0
  kk-5# c,  kk-8# c,  kk-sp# c, \ cursor: 5-8-Space
  kk-1# c,  kk-2# c,  kk-5#  c, \ Sinclair joystick 1: 1-2-5
  kk-6# c,  kk-7# c,  kk-0#  c, \ Sinclair joystick 2: 6-7-0
  kk-o# c,  kk-p# c,  kk-q#  c, \ QWERTY: O-P-Q
  kk-n# c,  kk-m# c,  kk-q#  c, \ QWERTY: N-M-Q
  kk-q# c,  kk-w# c,  kk-p#  c, \ QWERTY: Q-W-P
  kk-z# c,  kk-x# c,  kk-p#  c, \ QWERTY: Z-X-P

here controls - /controls / cconstant max-controls
  \ Number of controls stored in `controls`.

max-controls 1- cconstant last-control

: >controls ( n -- a ) /controls * controls + ;
  \ Convert controls number _n_ to its address _a_.

: set-controls ( n -- )
  >controls     dup c@  dup c!> kk-left#   #>kk 2!> kk-left
             1+ dup c@  dup c!> kk-right#  #>kk 2!> kk-right
             1+     c@  dup c!> kk-fire#   #>kk 2!> kk-fire ;
  \ Make controls number _n_ (item of the `controls` table) the
  \ current controls.

cvariable current-controls
  \ Index of the current controls in `controls` table.

current-controls coff
current-controls c@ set-controls
  \ Default controls.

: next-controls ( -- )
  current-controls c@1+  dup last-control > 0= abs *
  dup current-controls c!  set-controls ;
  \ Change the current controls.

  \ ===========================================================
  cr .( UDG) ?depth debug-point \ {{{1

               128 cconstant last-udg \ last UDG code used
last-udg 1+ /udg * constant /udg-set \ UDG set size in bytes

create udg-set /udg-set allot  udg-set set-udg
  \ Reserve space for the UDG set.

: udg>bitmap ( c -- a ) /udg * udg-set + ;
  \ Convert UDG char _c_ to the address _a_ of its bitmap.

: >scan ( n c -- a ) udg>bitmap + ;
  \ Convert scan number _n_ of UDG char _c_ to its address _a_.

: scan! ( c b n -- c ) rot >scan c! ;
  \ Store scan _b_ into scan number _n_ of char _c_,
  \ and return _c_ back for further processing.

cvariable used-udgs  used-udgs coff
  \ Counter of UDGs defined.

: udg-overflow? ( -- f ) used-udgs c@ last-udg 1+ > ;
  \ Too many UDGs defined?

: ?udg-overflow ( -- ) udg-overflow? abort" Too many UDGs" ;
  \ Abort if there are too many UDGs defined.

: ?free-udg ( n -- ) used-udgs c+!  ?udg-overflow ;
  \ Abort if there is not free space to define _n_ UDGs.

  \ ===========================================================
  cr .( Font) ?depth debug-point \ {{{1

[pixel-projectile] 0= [if]

cvariable ocr-last

: init-ocr ( -- ) ocr-first c@ udg>bitmap ocr-font !
                  ocr-last c@ ocr-first c@ - 1+ ocr-chars c! ;
  \ Set the UDGs `ocr` will examine to detect collisions.
  \
  \ XXX TODO -- range: only chars that may be detected: brick
  \ and invaders.
  \
  \ XXX TODO -- Remove `init-ocr`. It's needed only once, right
  \ after defining the graphics.

[then]

  \ ===========================================================
  cr .( Score) ?depth debug-point \ {{{1

                     0 cconstant status-bar-y
                     5 cconstant score-digits
          status-bar-y cconstant score-y
                       cvariable score-x
columns score-digits - cconstant record-x

2 cconstant max-player

cvariable players  1 players c! \ 1..max-player
cvariable player   1 player  c! \ 1..max-player

: ?[#] ( n -- ) 0 ?do postpone # loop ; immediate compile-only
  \ Compile `#` _n_ times.

: (.score ( n col row -- )
  at-xy s>d <# [ score-digits ] ?[#] #> text-attr attr! type ;
  \ Display score _n_ at coordinates _col row_.

: score-xy ( -- col row ) score-x c@ score-y ;
  \ Coordinates of the score.

: at-score ( -- ) score-xy at-xy ;
  \ Set the cursor position at the score.

: .score ( -- ) score @ score-xy (.score ;
  \ Display the score.

: .record ( -- ) record @ record-x score-y (.score ;
  \ Display the record.

: update-score ( n -- ) score +! .score ;

  \ ===========================================================
  cr .( Graphics) ?depth debug-point \ {{{1

    cvariable >udg  >udg coff \ next free UDG

cvariable latest-sprite-width
cvariable latest-sprite-height
cvariable latest-sprite-udg

: ?udg ( c -- ) last-udg > abort" Too many UDGs" ;
  \ Abort if UDG _n_ is too high.
  \ XXX TMP -- during the development

: free-udg ( n -- c )
  >udg c@ dup latest-sprite-udg c!
  tuck +  dup >udg c!  1- ?udg ;
  \ Free _n_ consecutive UDGs and return the first one _c_.

: set-latest-sprite-size ( width height -- )
  latest-sprite-height c!  latest-sprite-width c! ;
  \ Update the size of the latest sprited defined.

: emits-udg ( c n -- ) 0 ?do dup emit-udg loop drop ;

' emit-udg alias .1x1sprite ( c -- )
' emits-udg alias .1x1sprites ( c n -- )

: .2x1-udg-sprite ( c -- ) dup emit-udg 1+ emit-udg ;

: (udg-sprite ( width height -- width height c )
  2dup set-latest-sprite-size 2dup * free-udg ;

: udg-sprite ( width height "ccc" -- c )
  (udg-sprite dup >r udg-block r> ;

2 cconstant udg/invader
2 cconstant udg/mothership

0 0 0 0 0 0 0 0 1 free-udg dup cconstant bl-udg udg!

[pixel-projectile] 0= [if]
  >udg c@ ocr-first c!
    \ The first UDG examined by `ocr` must be the first one of
    \ the next sprite.
[then]

  \ -----------------------------------------------------------
  \ Invader species 0

  \ invader species 0, left flying, frame 0:

2 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXX..XX..XXX..
..XXXXXXXXXXXX..
.....XX..XX.....
....XX.XX.XX....
..XX........XX.. cconstant left-flying-invader-0-sprite

  \ invader species 0, left flying, frame 1:

2 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XX..XX..XXXX..
..XXXXXXXXXXXX..
....XXX..XXX....
...XX..XX..XX...
...XX.......XX.. drop

  \ invader species 0, left flying, frame 2:

2 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..X..XX..XXXXX..
..XXXXXXXXXXXX..
....XXX..XXX....
...XX..XX..XX...
....XX......XX.. drop

  \ invader species 0, left flying, frame 3:

2 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XX..XX..XXXX..
..XXXXXXXXXXXX..
....XXX..XXX....
...XX..XX..XX...
...XX.......XX.. drop

  \ invader species 0, right flying, frame 0:

2 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXX..XX..XXX..
..XXXXXXXXXXXX..
.....XX..XX.....
....XX.XX.XX....
..XX........XX.. cconstant right-flying-invader-0-sprite

  \ invader species 0, right flying, frame 1:

2 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXXX..XX..XX..
..XXXXXXXXXXXX..
....XXX..XXX....
...XX..XX..XX...
..XX.......XX... drop

  \ invader species 0, right flying, frame 2:

2 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXXXX..XX..X..
..XXXXXXXXXXXX..
....XXX..XXX....
...XX..XX..XX...
..XX......XX.... drop

  \ invader species 0, right flying, frame 3:

2 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXXX..XX..XX..
..XXXXXXXXXXXX..
....XXX..XXX....
...XX..XX..XX...
..XX.......XX... drop

  \ invader species 0, docked, frame 0:

2 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXX..XX..XXX..
..XXXXXXXXXXXX..
.....XX..XX.....
....XX.XX.XX....
..XX........XX.. cconstant docked-invader-0-sprite

  \ invader species 0, docked, frame 1:

2 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXX..XX..XXX..
..XXXXXXXXXXXX..
.....XX..XX.....
....XX.XX.XX....
..XX........XX.. drop

  \ invader species 0, docked, frame 2:

2 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXXXXXXXXXXX..
..XXXXXXXXXXXX..
.....XX..XX.....
....XX.XX.XX....
..XX........XX.. drop

  \ -----------------------------------------------------------
  \ Invader species 1

  \ invader species 1, left flying, frame 0:

2 1 udg-sprite

......X...X.....
.....X...X......
....XXXXXXX.....
...XX.XXX.XX....
..XXXXXXXXXXX...
..XXXXXXXXXXX...
..X.X.....X.X...
....X.....X..... cconstant left-flying-invader-1-sprite

  \ invader species 1, left flying, frame 1:

2 1 udg-sprite

......X...X.....
.....X...X......
....XXXXXXX.....
XXXX.XXX.XXXXXX.
..XXXXXXXXXXX...
...XXXXXXXXX....
....X.....X.....
.....X.....X.... drop

  \ invader species 1, left flying, frame 2:

2 1 udg-sprite

......X...X.....
..X..X...X..X...
..X.XXXXXXX.X...
..XXX.XXX.XXX...
..XXXXXXXXXXX...
...XXXXXXXXX....
....X.....X.....
....X.....X..... drop

  \ invader species 1, left flying, frame 3:

2 1 udg-sprite

......X...X.....
.....X...X......
....XXXXXXX.....
XXXX.XXX.XXXXXX.
..XXXXXXXXXXX...
...XXXXXXXXX....
....X.....X.....
.....X.....X.... drop

  \ invader species 1, right flying, frame 0:

2 1 udg-sprite

.....X...X......
......X...X.....
.....XXXXXXX....
....XX.XXX.XX...
...XXXXXXXXXXX..
...XXXXXXXXXXX..
...X.X.....X.X..
.....X.....X.... cconstant right-flying-invader-1-sprite

  \ invader species 1, right flying, frame 1:

2 1 udg-sprite

.....X...X......
......X...X.....
.....XXXXXXX....
.XXXXXX.XXX.XXXX
...XXXXXXXXXXX..
....XXXXXXXXX...
.....X.....X....
....X.....X..... drop

  \ invader species 1, right flying, frame 2:

2 1 udg-sprite

.....X...X......
...X..X...X..X..
...X.XXXXXXX.X..
...XXX.XXX.XXX..
...XXXXXXXXXXX..
....XXXXXXXXX...
.....X.....X....
.....X.....X.... drop

  \ invader species 1, right flying, frame 3:

2 1 udg-sprite

.....X...X......
......X...X.....
.....XXXXXXX....
.XXXXXX.XXX.XXXX
...XXXXXXXXXXX..
....XXXXXXXXX...
.....X.....X....
....X.....X..... drop

  \ invader species 1, docked, frame 0:

2 1 udg-sprite

....X.....X.....
.....X...X......
....XXXXXXX.....
...XX.XXX.XX....
..XXXXXXXXXXX...
..XXXXXXXXXXX...
..X.X.....X.X...
.....XX.XX...... cconstant docked-invader-1-sprite

  \ invader species 1, docked, frame 1:

2 1 udg-sprite

....X.....X.....
.....X...X......
....XXXXXXX.....
...XX.XXX.XX....
..XXXXXXXXXXX...
..XXXXXXXXXXX...
..X.X.....X.X...
.....XX.XX...... drop

  \ invader species 1, docked, frame 2:

2 1 udg-sprite

....X.....X.....
.....X...X......
....XXXXXXX.....
...XXXXXXXXX....
..XXXXXXXXXXX...
..XXXXXXXXXXX...
..X.X.....X.X...
.....XX.XX...... drop

  \ -----------------------------------------------------------
  \ Invader species 2

  \ invader species 2, left flying, frame 0:

2 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....XX.XX.XX....
....XXXXXXXX....
......X..X......
.....X.XX.X.....
....X.X..X.X.... cconstant left-flying-invader-2-sprite

  \ invader species 2, left flying, frame 1:

2 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....X.XX.XXX....
....XXXXXXXX....
......X..X......
......X.XX.X....
.....X.X..X.X... drop

  \ invader species 2, left flying, frame 2:

2 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....XX.XX.XX....
....XXXXXXXX....
......X..X......
.......X.XX.X...
......X.X..X.X.. drop

  \ invader species 2, left flying, frame 3:

2 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....X.XX.XXX....
....XXXXXXXX....
......X..X......
......X.XX.X....
.....X.X..X.X... drop

  \ invader species 2, right flying, frame 0:

2 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....XX.XX.XX....
....XXXXXXXX....
......X..X......
.....X.XX.X.....
....X.X..X.X.... cconstant right-flying-invader-2-sprite

  \ invader species 2, right flying, frame 1:

2 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....XXX.XX.X....
....XXXXXXXX....
......X..X......
....X.XX.X......
...X.X..X.X..... drop

  \ invader species 2, right flying, frame 2:

2 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....XX.XX.XX....
....XXXXXXXX....
......X..X......
...X.XX.X.......
..X.X..X.X...... drop

  \ invader species 2, right flying, frame 3:

2 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....XXX.XX.X....
....XXXXXXXX....
......X..X......
....X.XX.X......
...X.X..X.X..... drop

  \ invader species 2, docked, frame 0:

2 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....XX.XX.XX....
....XXXXXXXX....
......X..X......
.....X.XX.X.....
....X.X..X.X.... cconstant docked-invader-2-sprite

  \ invader species 2, docked, frame 1:

2 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....XX.XX.XX....
....XXXXXXXX....
......X..X......
.....X.XX.X.....
....X.X..X.X.... drop

  \ invader species 2, docked, frame 2:

2 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....XXXXXXXX....
....XXXXXXXX....
......X..X......
.....X.XX.X.....
....X.X..X.X.... drop

  \ -----------------------------------------------------------
  \ Mothership

0 cconstant mothership
  \ Configurable constant that contains the first character of
  \ the first sprite frame of the current sprite of the
  \ mothership.

cvariable mothership-frame
  \ Current frame of the current sprite of the mothership.

0 cconstant mothership-frames
  \ Configurable constant that contains the number of frames of
  \ the current sprite of the mothership.

  \ ............................................
  \ Flying mothership

4 cconstant flying-mothership-frames

  \ mothership, frame 0:

2 1 udg-sprite

................
.....XXXXXX.....
...XXXXXXXXXX...
..XXXXXXXXXXXX..
.XX..XX..XX..XX.
XXXXXXXXXXXXXXXX
..XXX..XX..XXX..
...X........X... cconstant flying-mothership-sprite

  \ mothership, frame 1:

2 1 udg-sprite

................
.....XXXXXX.....
...XXXXXXXXXX...
..XXXXXXXXXXXX..
.X..XX..XX..XX..
XXXXXXXXXXXXXXXX
..XXX..XX..XXX..
...X........X... drop

  \ mothership, frame 2:

2 1 udg-sprite

................
.....XXXXXX.....
...XXXXXXXXXX...
..XXXXXXXXXXXX..
...XX..XX..XX...
XXXXXXXXXXXXXXXX
..XXX..XX..XXX..
...X........X... drop

  \ mothership, frame 3:

2 1 udg-sprite

................
.....XXXXXX.....
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XX..XX..XX..X.
XXXXXXXXXXXXXXXX
..XXX..XX..XXX..
...X........X... drop

  \ ............................................
  \ Beaming mothership

2 cconstant beaming-mothership-frames

2 1 udg-sprite

................
.....XXXXXX.....
...XXXXXXXXXX...
..XXXXXXXXXXXX..
.XX..XX..XX..XX.
XXXXXXXXXXXXXXXX
.X.X.X.XX.X.X.X.
X.X.X.X..X.X.X.X cconstant beaming-mothership-sprite

2 1 udg-sprite

................
.....XXXXXX.....
...XXXXXXXXXX...
..XXXXXXXXXXXX..
.XX..XX..XX..XX.
XXXXXXXXXXXXXXXX
X.X.X.X..X.X.X.X
.X.X.X.XX.X.X.X. drop

  \ ............................................
  \ Mothership explosion

2 cconstant exploding-mothership-frames

2 1 udg-sprite

..............X.
..X......XX..X..
.X...XXXXXXX....
....XXXXXXXXX.X.
...XXXX.XX.XX..X
.X..XX..XXXX....
X....XXXXX...X..
..X...XX...X..X. cconstant exploding-mothership-sprite

2 1 udg-sprite

................
...X...X...X....
.....XXXXXXX..X.
.X..X.XXXX.XX...
...XX.XXX.XXX...
..X.X.XXXX.X..X.
.X...XX.XX..X...
.X...XX...X..X.X drop

0 [if]

  \ XXX TODO -- Too many UDGs!

2 1 udg-sprite

.X...X..........
X...X....X...X..
...X.XXXX.XX....
...XXXX.XX.XX..X
X...X.XXX.X.XX..
....X.XX.X.XX..X
...X.XX.XXX.XX..
X...XX.X..X..... drop

[then]

  \ -----------------------------------------------------------
  \ Projectile

[pixel-projectile] 0= [if]

  >udg c@ \ next free UDG
  dup cconstant first-projectile-frame

  1 1 udg-sprite

  ..X.....
  .....X..
  ..X.....
  .....X..
  ..X.....
  .....X..
  ..X.....
  .....X.. cconstant projectile-sprite

  1 1 udg-sprite

  .....X..
  ..X.....
  .....X..
  ..X.....
  .....X..
  ..X.....
  .....X..
  ..X..... drop

  1 1 udg-sprite

  ..X.....
  ..X..X..
  .....X..
  ..X.....
  ..X..X..
  .....X..
  ..X.....
  ..X..X.. drop

  1 1 udg-sprite

  .....X..
  ..X..X..
  ..X.....
  .....X..
  ..X..X..
  ..X.....
  .....X..
  ..X..X.. drop

  1 1 udg-sprite

  ..X.....
  ........
  ..X..X..
  ........
  .....X..
  ..X.....
  ........
  ..X..X.. drop

  1 1 udg-sprite

  .....X..
  ........
  ..X..X..
  ........
  ..X.....
  .....X..
  ........
  ..X..X.. drop

  1 1 udg-sprite

  ..X..X..
  .....X..
  ..X.....
  ........
  ..X..X..
  .....X..
  ..X.....
  ........ drop

  1 1 udg-sprite

  ..X..X..
  ..X.....
  .....X..
  ........
  ..X..X..
  ..X.....
  .....X..
  ........ drop

  1 1 udg-sprite

  ..X.....
  ..X..X..
  ........
  ..X..X..
  ..X.....
  .....X..
  ..X.....
  ..X..X.. drop

  1 1 udg-sprite

  .....X..
  ..X..X..
  ........
  ..X..X..
  .....X..
  ..X.....
  .....X..
  ..X..X.. drop

  >udg c@ swap - cconstant frames/projectile

  >udg c@1- cconstant last-projectile-frame

[then]

  \ -----------------------------------------------------------
  \ Building

1 1 udg-sprite

XXXXX.XX
XXXXX.XX
XXXXX.XX
........
XX.XXXXX
XX.XXXXX
XX.XXXXX
........ cconstant brick

[pixel-projectile] 0= [if]
  >udg c@1- ocr-last c!
    \ The last UDG examined by `ocr` must be the last one
    \ of the latest sprite.
[then]

1 1 udg-sprite

XXXXXXXX
.XXXXXXX
..XXXXXX
..XXXXXX
..XXXXXX
..XXXXXX
..XXXXXX
..XXXXXX cconstant left-door

1 1 udg-sprite

XXXXXXXX
XXXXXXX.
XXXXXX..
XXXXXX..
XXXXXX..
XXXXXX..
XXXXXX..
XXXXXX.. cconstant right-door

1 1 udg-sprite

XXXXXXXX
.XXXXXXX
.X.XXXXX
...XX.XX
......XX
.....XXX
......X.
........ cconstant broken-top-left-brick

1 1 udg-sprite

........
........
.....XXX
......XX
...XX.XX
.XXXXXXX
XXXXXXXX
XXXXXXXX cconstant broken-bottom-left-brick

1 1 udg-sprite

XXXXXXXX
XXXXXXXX
XXXXX...
XXXXXX..
XXX..X..
X.......
........
........ cconstant broken-top-right-brick

1 1 udg-sprite

........
X.......
X.X.....
XXX..X..
XXXXXX..
XXXXX...
XXXXXX.X
XXXXXXXX cconstant broken-bottom-right-brick

  \ -----------------------------------------------------------
  \ Tank

4 cconstant tank-frames

cvariable tank-frame \ counter (0..3)

3 cconstant udg/tank

udg/tank 1 udg-sprite
..........X..X..........
...XXXXXX.X..X.XXXXXXX..
..XXXXXXXXXXXXXXXXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XX.X.X.X.X.X.X.X.X.X.XX
..XX..XX..XX..XX..XX.XX.
...X.XXX.XXX.XXX.XXX.X..
....X.X.X.X.X.X.X.X.X... cconstant tank-sprite

udg/tank 1 udg-sprite
..........X..X..........
...XXXXXX.X..X.XXXXXXX..
..XXXXXXXXXXXXXXXXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XXX.X.X.X.X.X.X.X.X.XXX
..XX.XX..XX..XX..XX..XX.
...X.XXX.XXX.XXX.XXX.X..
...X.X.X.X.X.X.X.X.X.X.. drop

udg/tank 1 udg-sprite
..........X..X..........
...XXXXXX.X..X.XXXXXXX..
..XXXXXXXXXXXXXXXXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XX.X.X.X.X.X.X.X.X.X.XX
..XX.XXX.XXX.XXX.XXX.XX.
...X.XX..XX..XX..XX..X..
....X.X.X.X.X.X.X.X.X... drop

udg/tank 1 udg-sprite
..........X..X..........
...XXXXXX.X..X.XXXXXXX..
..XXXXXXXXXXXXXXXXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XXX.X.X.X.X.X.X.X.X.XXX
..XX.XXX.XXX.XXX.XXX.XX.
...X..XX..XX..XX..XX.X..
...X.X.X.X.X.X.X.X.X.X.. drop

  \ -----------------------------------------------------------
  \ Invader explosion

  \ XXX TODO -- Move to the invaders section.

2 1 udg-sprite

.....X...X......
..X...X.X...X...
...X.......X....
....X.....X.....
.XX.........XX..
.....X....X.....
...X..X.X..X....
..X..X...X..X... cconstant invader-explosion-sprite

  \ -----------------------------------------------------------
  \ Containers

  \ XXX TODO -- Move to the building section.

2 1 udg-sprite

......XXXXX.....
...XXX.....XXX..
..X...XXXXX...X.
..X...........X.
..X....XXX....X.
..X...XXXXX...X.
..X....XXX....X.
..X.....X.....X. cconstant container-top

1 1 udg-sprite

........
...XXX..
..X...X.
..X...X.
..X...X.
..X....X
..X....X
..X....X cconstant broken-top-left-container

1 1 udg-sprite

........
...XXX..
..X...X.
..X...X.
.X....X.
.X....X.
.X....X.
X.....X. cconstant broken-top-right-container

2 1 udg-sprite

..X..X.X.X.X..X.
..X.XXXX.XXXX.X.
..X.XXX...XXX.X.
..X..XX...XX..X.
..X...........X.
...XXX.....XXX..
......XXXXX.....
................ cconstant container-bottom

1 1 udg-sprite

.......X
.....XXX
...XXXX.
..X..XX.
..X.....
...XXX..
......XX
........ cconstant broken-bottom-left-container

1 1 udg-sprite

XX......
.XXX....
..XXXX..
..XX..X.
......X.
...XXX..
XXX.....
........ cconstant broken-bottom-right-container

  \ -----------------------------------------------------------
  \ Icons

2 1 udg-sprite

................
............X...
............XX..
....XXXXXXXXXXX.
....XXXXXXXXXXXX
....XXXXXXXXXXX.
............XX..
............X... cconstant right-arrow

2 1 udg-sprite

................
...X............
..XX............
.XXXXXXXXXXX....
XXXXXXXXXXXX....
.XXXXXXXXXXX....
..XX............
...X............ cconstant left-arrow

2 1 udg-sprite

....XXXXXXXX....
..XX........XX..
..XX........XX..
..X.XXXXXXXX.X..
..X..........X..
..X..........X..
..X..........X..
XXXXXXXXXXXXXXXX cconstant fire-button

  \ ===========================================================
  cr .( Type) ?depth debug-point \ {{{1

: centered ( len -- col ) columns swap - 2/ ;
  \ Convert a string length to the column required
  \ to display the string centered.

: centered-at ( row len -- ) centered swap at-xy ;
  \ Set the cursor position to display string _ca len_ centered
  \ on the given row.

: center-type ( ca len row -- ) over centered-at type ;
  \ Display string _ca len_ centered on the given row.

: type-blank ( ca len -- ) nip spaces ;

: center-type-blank ( ca len row -- )
  over centered-at type-blank ;
  \ Overwrite string _ca len_ with blanks, centered on the
  \ given row.

  \ ===========================================================
  cr .( Game screen) ?depth debug-point \ {{{1

sky-bottom-y sky-top-y - 1+ columns * constant /sky
  \ Number of characters and attributes of the sky.

sky-top-y columns * attributes + constant sky-top-attribute
  \ Address of the first attribute of the sky.

3 cconstant building-top-y

1 cconstant status-bar-rows

status-bar-rows columns * cconstant /status-bar

: .score-label ( -- ) score$ dup 1+ score-x c! home type ;

: >record-label-x ( len -- x )
  [ columns score-digits 1+ - ] cliteral swap - ;
  \ Get the column _x_ of the record label from its length
  \ _len_.

: .record-label ( -- ) record$ dup >record-label-x at-x type ;

: status-bar ( -- )
  text-attr attr! .score-label .score .record-label .record ;

  \ ===========================================================
  cr .( Invaders data) ?depth debug-point \ {{{1

  \ --------------------------------------------
  \ Invader species

3 cconstant #species

0
  cfield: ~flying-left-sprite         \ UDG
  cfield: ~flying-left-sprite-frames  \ count
  cfield: ~flying-right-sprite        \ UDG
  cfield: ~flying-right-sprite-frames \ count
  cfield: ~docked-sprite              \ UDG
  cfield: ~docked-sprite-frames       \ count
cconstant /species

create species #species /species * allot
  \ Invaders species data table.

: species#>~ ( n -- a ) /species * species + ;

: set-species ( c1 c2 c3 c4 -- )
  species#>~ >r r@ ~flying-right-sprite c!
              4 r@ ~flying-right-sprite-frames c!
                r@ ~flying-left-sprite c!
              4 r@ ~flying-left-sprite-frames c!
                r@ ~docked-sprite c!
              3 r> ~docked-sprite-frames c! ;
  \ Init the data of invaders species _c4_:
  \   c1 = docked sprite
  \   c2 = left flying sprite
  \   c3 = right flying sprite

docked-invader-0-sprite
left-flying-invader-0-sprite
right-flying-invader-0-sprite
0 set-species

docked-invader-1-sprite
left-flying-invader-1-sprite
right-flying-invader-1-sprite
1 set-species

docked-invader-2-sprite
left-flying-invader-2-sprite
right-flying-invader-2-sprite
2 set-species

  \ --------------------------------------------

                    0 cconstant invaders-min-x
columns udg/invader - cconstant invaders-max-x

10 cconstant max-invaders

max-invaders 2/ cconstant half-max-invaders

0

  \ XXX TODO -- reorder for speed: place the most used fields
  \ at cell offsets +0, +1, +2, +4

  cfield: ~y             \ row
  cfield: ~x             \ column
  cfield: ~sprite        \ UDG
  cfield: ~frames        \ count
  cfield: ~frame         \ counter
  cfield: ~initial-x     \ column
  field:  ~x-inc         \ -1|1
  field:  ~initial-x-inc \ -1|1
  cfield: ~stamina       \ 0..3
  field:  ~action        \ execution token
  field:  ~species       \ data structure address
cconstant /invader

max-invaders /invader * constant /invaders

create invaders-data /invaders allot
  \ Invaders data table.

: invader#>~ ( n -- a ) /invader * invaders-data + ;
  \ Convert invader number _n_ to its data address _a_.

0 constant flying-to-the-left? ( f )
  \ A configurable constant containing a flag:
  \ Is the current invader flying to the left?

: set-invader ( n -- )
  dup invader# c! invader#>~ !> invader~
  invader~ ~x-inc @ 0< !> flying-to-the-left? ;
  \ Set invader _n_ as the current invader.

: get-invader ( -- n ) invader# c@ ;
  \ Return number _n_ of the current invader.

max-invaders 2/ 1- cconstant top-invader-layer
  \ The number of the highest invader "layer". The pair
  \ of invaders that fly nearest the ground are layer 0.
  \ The pair above them are layer 1, and so on.

top-invader-layer 1+ cconstant invader-layers
  \ Number of invader layers.

2 cconstant rows/layer

building-top-y 1+ cconstant invader-top-y

invader-top-y top-invader-layer rows/layer * +
cconstant invader-bottom-y

: layer>y ( n -- y )
  top-invader-layer swap - rows/layer * invader-top-y + ;
  \ Convert invader layer _n_ to its equivalent row _y_. The
  \ pair of invaders that fly nearest the ground are layer 0.
  \ The pair above them are layer 1, and so on.

: y>layer ( y -- n ) rows/layer / 1- invader-top-y swap - ;
  \ Convert invader row _y_ to its equilavent layer _n_. The
  \ pair of invaders that fly nearest the ground are layer 0.
  \ The pair above them are layer 1, and so on. Note: _y_ is
  \ supposed to be a valid row of an invader layer, otherwise
  \ the result will be wrong.

: y>layer? ( y -- n f )
  invader-top-y - rows/layer /mod swap 0= ;
  \ If _y_ is a valid row of an invader layer, return layer _n_
  \ and _f_ is _true_; otherwise _n_ is invalid and _f_ is
  \ false.  The pair of invaders that fly nearest the ground are
  \ layer 0.  The pair above them are layer 1, and so on.  Note:
  \ If _y_ is greater than the last invader layer, the result
  \ will be wrong.
  \
  \ XXX REMARK -- Not used.

: invader-retreat-points ( -- n ) invader~ ~y c@ y>layer 1+ ;

: invader-destroy-points ( -- n ) invader-retreat-points 10 * ;

: attacking? ( -- f )
  invader~ ~initial-x-inc @ invader~ ~x-inc @ = ;
  \ Is the current invader attacking?

: .y/n ( f -- ) if ." Y" else ." N" then space ;
  \ XXX TMP -- for debugging

: ~~invader-info ( -- )
  home get-invader 2 .r
  ." Att.:" attacking? .y/n
  ." Sta.:" invader~ ~stamina c@ . ;
  \ XXX TMP -- for debugging

  \ ' ~~invader-info ' ~~app-info defer!
  \ XXX TMP -- for debugging

3 cconstant max-stamina

: set-flying-sprite ( -- )
  invader~ ~species @ dup
  flying-to-the-left?
  if   ~flying-left-sprite c@ swap
       ~flying-left-sprite-frames c@
  else ~flying-right-sprite c@ swap
       ~flying-right-sprite-frames c@
  then invader~ ~frames c! invader~ ~sprite c!
  invader~ ~frame coff ;
  \
  \ XXX TODO -- Use double-cell fields to copy both fields with
  \ one operation or use `move`.
  \
  \ XXX TODO -- Rename.
  \
  \ XXX TODO -- If the maximum frames in both directions are
  \ identical, there's no need to initiate `~frame`.
  \
  \ XXX TODO -- Combine with `set-invader-direction`.

: set-docked-sprite ( -- )
  invader~ ~species @ dup
  ~docked-sprite c@ invader~ ~sprite c!
  ~docked-sprite-frames c@ invader~ ~frames c!
  invader~ ~frame coff ;
  \
  \ XXX TODO -- Rename.

: init-invader ( c1 c2 c3 c4 c0 -- )
  set-invader
  invader~ ~stamina coff
  invader~ ~action off
  species#>~ invader~ ~species !
  invader~ ~initial-x-inc ! invader~ ~x-inc off
  dup invader~ ~initial-x c!
      invader~ ~x c!
  invader~ ~y c!
  set-docked-sprite ;
  \ Init invader_c0_ with the given data:
  \   c1 = y
  \   c2 = x = initial x
  \   c3 = initial x inc
  \   c4 = species
  \ The sprite and the frame are set after the species.
  \ All other fields are set to zero.

: init-left-invaders-data ( -- )
  [ 0 layer>y ] cliteral invaders-min-x  1 0 4 init-invader
  [ 1 layer>y ] cliteral invaders-min-x  1 0 3 init-invader
  [ 2 layer>y ] cliteral invaders-min-x  1 1 2 init-invader
  [ 3 layer>y ] cliteral invaders-min-x  1 1 1 init-invader
  [ 4 layer>y ] cliteral invaders-min-x  1 2 0 init-invader ;

: init-right-invaders-data ( -- )
  [ 0 layer>y ] cliteral invaders-max-x -1 0 9 init-invader
  [ 1 layer>y ] cliteral invaders-max-x -1 0 8 init-invader
  [ 2 layer>y ] cliteral invaders-max-x -1 1 7 init-invader
  [ 3 layer>y ] cliteral invaders-max-x -1 1 6 init-invader
  [ 4 layer>y ] cliteral invaders-max-x -1 2 5 init-invader ;

: -invaders-data ( -- ) invaders-data /invaders erase ;
  \ Erase the data of all invaders.

: init-invaders-data ( -- )
  -invaders-data
  init-left-invaders-data init-right-invaders-data ;
  \ Init the data of all invaders.

defer docked-action ( -- )
  \ Action of the invaders that are docked.

: create-invaders ( n1 n2 -- )
  ?do i set-invader
      max-stamina invader~ ~stamina c!
      ['] docked-action invader~ ~action !
  loop ;
  \ Create new docked invaders from _n2_ to _n1-1_.  The data of
  \ those invaders must be set already to their default values.

: create-left-squadron ( -- )
  init-left-invaders-data
  half-max-invaders 0 create-invaders ;

: create-right-squadron ( -- )
  init-right-invaders-data
  max-invaders half-max-invaders create-invaders ;

create stamina-attributes ( -- ca )
  dying-invader-attr    c,
  wounded-invader-attr  c,
  sane-invader-attr     c,
  \ Table to index the invader stamina to its proper attribute.

: invader-proper-attr ( -- c )
  invader~ ~stamina c@ [ stamina-attributes 1- ] literal + c@ ;
  \ Return the proper attribute _c_ for the stamina of the
  \ current invader.

: +invaders ( n0 n1 n2 -- n3 )
  ?do i invader#>~ ~stamina c@ 0<> abs + loop ;
  \ Count the number of alive invaders, from invader
  \ _n2_ to invader _n1-1_, and add the result to _n0_,
  \ giving the final result _n3_.

: left-side-invaders ( -- n )
  0 half-max-invaders 0 +invaders ;
  \ Return number _n_ of alive invaders that are at the left of
  \ the building.

: right-side-invaders ( -- n )
  0 max-invaders half-max-invaders +invaders ;
  \ Return number _n_ of alive invaders that are at the right of
  \ the building.

  \ ===========================================================
  cr .( Building) ?depth debug-point \ {{{1

cvariable old-breachs
  \ Number of breachs in the wall, at the start of the current
  \ attack.

cvariable breachs
  \ Number of new breachs in the wall, during the current
  \ attack.

cvariable battle-breachs
  \ Total number of breachs in the wall, during the current
  \ battle.

: check-breachs ( -- ) breachs c@ old-breachs c! ;
  \ Remember the current number of breachs.

: no-breach ( -- )
  old-breachs coff breachs coff battle-breachs coff ;
  \ Reset the number of breachs.

: breachs? ( -- f ) breachs c@ 0<> ;
  \ Has the building any breach caused during an attack?

: new-breach? ( -- f ) breachs c@ old-breachs c@ > ;
  \ Has got the building any new breach during the current
  \ attack?

building-top-y 11 + cconstant building-bottom-y
  \ XXX TODO -- Rename. This was valid when the building
  \ was "flying".

cvariable building-width

cvariable building-left-x     cvariable building-right-x
cvariable containers-left-x   cvariable containers-right-x

: size-building ( -- )
  [ columns 2/ 1- ] cliteral  \ half of the screen
  location c@1+               \ half width of all containers
  dup 2* 2+ building-width     c!
  2dup 1- - containers-left-x  c!
  2dup    - building-left-x    c!
  2dup    + containers-right-x c!
       1+ + building-right-x   c! ;
  \ Set the size of the building after the current location.

: floor ( y -- )
  building-left-x c@ swap at-xy
  brick-attr attr! brick building-width c@ .1x1sprites ;
  \ Draw a floor of the building at row _y_.

: ground-floor ( y -- )
  building-left-x c@1+ swap at-xy
  door-attr attr!  left-door emit-udg
  brick-attr attr! brick building-width c@ 4 - .1x1sprites
  door-attr attr!  right-door emit-udg ;
  \ Draw the ground floor of the building at row _y_.

: building-top ( -- ) building-top-y floor ;
  \ Draw the top of the building.

: containers-bottom ( n -- )
  container-attr attr!
  0 ?do  container-bottom .2x1-udg-sprite  loop ;
  \ Draw a row of _n_ bottom parts of containers.

: containers-top ( n -- )
  container-attr attr!
  0 ?do  container-top .2x1-udg-sprite  loop ;
  \ Draw a row of _n_ top parts of containers.

: .brick ( -- ) brick-attr attr! brick .1x1sprite ;
  \ Draw a brick.

create containers-half
  ' containers-top ' containers-bottom
    \ execution vectors to display the containers
  building-top-y even? ?\ swap
    \ change their order, depending on the building position
  , ,

: yard ( row -- )
                         0 over at-xy .brick
  [ last-column ] cliteral swap at-xy .brick ;
  \ Draw the yard limits.

variable repaired
  \ Flag: has the building been repaired at the start of the
  \ current attack?

: building ( -- )
  building-top
  location c@1+  building-left-x c@
  building-bottom-y [ building-top-y 1+ ] cliteral
  do   2dup i at-xy .brick
                    i 1 and containers-half array> perform
                    .brick
  loop 2drop tank-y dup building-bottom-y ?do i floor loop
                        dup ground-floor yard
  no-breach ;
  \ Draw the building and the nuclear containers.
  \
  \ XXX TODO -- Simpler and faster.

: repair-building ( -- ) building repaired on check-breachs ;

  \ ===========================================================
  cr .( Locations) ?depth debug-point \ {{{1

8 cconstant locations

: next-location ( -- )
  location c@1+ locations min location c! ;
  \ Increase the location number, but not beyond the maximum.
  \ XXX TMP --
  \ XXX TODO -- Check the limit to finish the game instead.

variable used-projectiles  used-projectiles off
  \ Counter.

: battle-bonus ( -- n ) location c@1+ 500 *
                        battle-breachs c@ 100 * -
                        used-projectiles @ -
                        0 max ;
  \ Calculate bonus _n_ after winning a battle.

: reward ( -- ) battle-bonus update-score ;
  \ Add the won battle bonus to the score.

: travel ( -- ) next-location size-building ;
  \ Travel to the next battle location.

: first-location ( -- ) location coff size-building ;
  \ Init the location number and the related variables.

  \ ===========================================================
  cr .( Tank) ?depth debug-point \ {{{1

cvariable tank-x \ column

variable transmission-damage

variable tank-time
  \ When the ticks clock reaches the contents of this variable,
  \ the tank will move.

: repair-tank ( -- ) transmission-damage off tank-time off ;

columns udg/tank - 2/ cconstant parking-x

: park-tank ( -- ) parking-x tank-x c! tank-frame coff ;

                    1 cconstant tank-min-x
columns udg/tank - 1- cconstant tank-max-x
  \ Mininum and maximum columns of the tank.

: new-projectile-x ( -- col|gx )
  [pixel-projectile]
  [if]   tank-x c@ x>gx [ udg/tank 8 * 2/ ] cliteral +
  [else] tank-x c@1+
  [then] ;
  \ Return the column _col_ or graphic coordinate _gx_ for the
  \ new projectile, depending (at compile time) on the type of
  \ projectile and (at runtime) the position of the tank.

: gun-below-building? ( -- f )
  new-projectile-x
  [pixel-projectile] [if]
    building-left-x c@ x>gx building-right-x c@ x>gx
  [else]
    building-left-x c@ building-right-x c@
  [then] between ;
  \ Is the tank's gun below the building?

: transmission? ( -- f ) rnd transmission-damage @ u> ;
  \ Is the transmission working?

: tank-rudder ( -- -1|0|1 )
  kk-left pressed? kk-right pressed? abs + transmission? and ;
  \ Does the tank move? Return its x increment.

: outside? ( col -- f )
  building-left-x c@1+ building-right-x c@ within 0= ;
  \ Is column _col_ outside the building?
  \ The most left and most right columns of the building
  \ are considered outside, because they are the doors.

: next-col ( col -- ) 1+ 33 swap - 23688 c!  1 23684 +! ;
  \ Set the current column to _col+1_, by modifing the
  \ contents of OS byte variable S_POSN (23688) and increasing
  \ the OS cell variable DF_CC (23684) (printing address in the
  \ screen bitmap).  Unfortunately, a bug in the ROM prevents
  \ control character 9 (cursor right) from working.
  \ This word is needed because `emit-udg` does not update
  \ the current coordinates.

: ?emit-outside ( col1 c -- col2 )
  over outside? if   emit-udg 1+          exit
                then drop dup next-col 1+ ;
  \ If column _col1_ is outside the building, display character
  \ _c_ at the current cursor position.  Increment _col1_ and
  \ return it as _col2_.

: left-tank-udg   ( -- c )
  tank-frame c@ udg/tank * tank-sprite + ;

: middle-tank-udg ( -- c ) left-tank-udg 1+ ;

: right-tank-udg  ( -- c ) left-tank-udg 2+ ;

: tank-frame+ ( n1 -- n2 ) 1+ dup tank-frames < and ;
  \ Increase tank frame _n1_ resulting frame _n2_.
  \ If the limit was reached, _n2_ is zero.
  \
  \ XXX TODO -- Use `tank-max-frame <>` for speed.

: tank-next-frame ( -- )
  tank-frame c@ tank-frame+ tank-frame c! ;
  \ Update the tank frame.

: tank-parts ( col1 -- col2 )
  tank-attr attr! left-tank-udg   ?emit-outside
                  middle-tank-udg ?emit-outside
                  right-tank-udg  ?emit-outside
  tank-next-frame ;
  \ Display every visible part of the tank (the parts that are
  \ outside the building) and update the frame.

: -tank-extreme ( col1 -- col2 )
  sky-attr attr! bl-udg ?emit-outside ;

: at-tank@ ( -- col ) tank-x c@ dup tank-y at-xy ;
  \ Set the cursor position at the tank's coordinates
  \ and return its column _col_.

: tank> ( -- )
  at-tank@ -tank-extreme tank-parts drop 1 tank-x c+! ;
  \ Move the tank to the right.

: (.tank ( -- col ) at-tank@ tank-parts ;
  \ Display the tank at its current position and return column
  \ _col_ at its right.

: .tank ( -- ) (.tank drop ;
  \ Display the tank at its current position.

: new-tank ( -- ) repair-tank park-tank .tank ;

: <tank ( -- ) -1 tank-x c+! (.tank -tank-extreme drop ;
  \ Move the tank to the left.

: ?<tank ( -- ) tank-x c@ tank-min-x = ?exit <tank ;
  \ If the tank column is not the minimum, move the tank to the
  \ left.

: ?tank> ( -- ) tank-x c@ tank-max-x = ?exit tank> ;
  \ If the tank column is not the maximum, move the tank to the
  \ right.

      ' ?<tank , \ move tank to the left
here  0 ,        \ do nothing
      ' ?tank> , \ move tank to the right
constant tank-movements ( -- a )
  \ Execution table of tank movements.

: tank-movement ( -- a ) tank-rudder tank-movements array> ;

8 cconstant tank-interval \ ticks

: schedule-tank ( -- ) ticks tank-interval + tank-time ! ;

: driving ( -- ) tank-time @ past? 0exit
                 tank-movement perform
                 schedule-tank ;

  \ ===========================================================
  cr .( Projectiles) ?depth debug-point \ {{{1

%111 cconstant max-projectile#
  \ Bitmask for the projectile counter (0..7).
  \ XXX TODO -- try %1111 and %11111

max-projectile# 1+ cconstant #projectiles
  \ Maximum number of simultaneous projectiles.

#projectiles allot-xstack xstack
  \ Create and activate an extra stack to store the free
  \ projectiles.

0 cconstant projectile#
  \ Number of the current projectile.

create 'projectile-x #projectiles allot
create 'projectile-y #projectiles allot
  \ Tables for the coordinates of all projectiles.

: projectile-x ( -- ca ) 'projectile-x projectile# + ;
  \ Address of the x coordinate of the current projectile.

: projectile-y ( -- ca ) 'projectile-y projectile# + ;
  \ Address of the y coordinate of the current projectile.

defer .debug-data ( -- )
' noop ' .debug-data defer!

defer debug-data-pause ( -- )
' wait ' debug-data-pause defer!

: (.debug-data ( -- )
  9 23 at-xy ." Ammo:" xdepth .
             ." Depth:" depth .
             ." Curr.:" projectile# .
             debug-data-pause ;
  \ XXX INFORMER

: destroy-projectile ( -- ) projectile-y coff  projectile# >x ;

: new-projectiles ( -- )
  used-projectiles off
  'projectile-y #projectiles erase
  xclear #projectiles 0 do i >x loop ;

: projectile-coords ( -- col row | gx gy )
  projectile-x c@ projectile-y c@ ;
  \ Coordinates of the projectile.

  \ ===========================================================
  cr .( Init) ?depth debug-point \ {{{1

: prepare-war ( -- )
  catastrophe off
  [pixel-projectile] [ 0= ] [if] init-ocr [then]
  first-location score off cls ;


: .parade-invader ( n -- )
  invader#>~ dup >r ~initial-x c@ r@ ~y c@ at-xy
                 r> ~sprite c@ .2x1-udg-sprite ;
  \ Display invader _n_ at its initial position, with the
  \ current attribute.

0 [if]

  \ XXX OLD

: ((parade ( n1 n2 -- ) ?do i .parade-invader loop ;
  \ Display invaders from _n2_ to _n1_, not including _n1_,
  \ using the current attribute.

: (parade ( n1 n2 -- ) invader-attr attr! ((parade ;
  \ Display invaders from _n2_ to _n1_, not including _n1_,
  \ using the proper attribute.

: parade ( -- ) max-invaders 0 (parade ;
  \ Display parade of all invaders.

: left-side-parade ( -- ) half-max-invaders 0
                          (parade ;
  \ Display parade of the left-side invaders.

: right-side-parade ( -- ) max-invaders half-max-invaders
                           (parade ;
  \ Display parade of the right-side invaders.

[then]

  \ ===========================================================
  cr .( Instructions) ?depth debug-point \ {{{1

: game-title ( -- )
  home game-title$ columns type-center-field ;

: game-version ( -- ) version$ 1 center-type ;

: (c) ( -- ) 127 emit ;
  \ Display the copyright symbol.

: (.copyright ( -- )
  row 1 over    at-xy (c) ."  2016..2018 Marcos Cruz"
      8 swap 1+ at-xy           ." (programandala.net)" ;
  \ Display the copyright notice at the current coordinates.

: .copyright ( -- ) 0 22 at-xy (.copyright ;

  \ XXX OLD -- maybe useful in a future version
  \ : .control ( n -- ) ."  = " .kk# 4 spaces ;
  \ : (.controls ( -- )
  \   row dup s" [Space] to change controls:" rot center-type
  \   9 over 2+  at-xy ." Left " kk-left#  .control
  \   9 over 3 + at-xy ." Right" kk-right# .control
  \   9 swap 4 + at-xy ." Fire " kk-fire#  .control ;
  \   \ Display controls at the current row.

: left-key$ ( -- ca len ) kk-left# kk#>string ;

: right-key$ ( -- ca len ) kk-right# kk#>string ;

: fire-key$ ( -- ca len ) kk-fire# kk#>string ;

: .controls-legend ( -- )
  10 at-x left-arrow  .2x1-udg-sprite
  15 at-x fire-button .2x1-udg-sprite
  20 at-x right-arrow .2x1-udg-sprite ;
  \ Display controls legend at the current row.

: .control-keys ( -- )
  10 at-x left-key$  2 type-right-field
  13 at-x fire-key$  6 type-center-field
  20 at-x right-key$ 2 type-left-field ;
  \ Display control keys at the current row.

: (.controls ( -- )
  \ s" [Space] to change controls:" row dup >r center-type
    \ XXX TODO --
  .controls-legend cr .control-keys ;
  \ Display controls at the current row.

: change-players ( -- )
  players c@1+ dup max-player > if drop 1 then players c! ;

false [if] \ XXX TODO --

: (.players ( -- ) players$ type players c@ . ;
   \ Display the number of players at the current coordinates.

: .players ( -- ) 0 8 at-xy (.players ;

[else] \ XXX TMP --

: .players ( -- ) ;

[then]

: .controls ( -- )
  0 12 at-xy (.controls
  \ s" SPACE: change - ENTER: start" 18 center-type  ; XXX TMP
  0 16 at-xy not-in-this-language$ columns type-center-field
  0 19 at-xy start$ columns type-center-field ;
  \ XXX TMP --

: invariable-menu-screen ( -- ) game-version .copyright ;
  \ Display the parts of the menu screen that are invariable,
  \ i.e., don't depend on the current language.

: variable-menu-screen ( -- ) game-title .players .controls ;
  \ Display the parts of the menu screen that are variable,
  \ i.e., depend on the current language.

: menu-screen ( -- )
  cls invariable-menu-screen variable-menu-screen ;

: change-language  ( -- ) lang 1+ dup langs < abs * c!> lang ;
  \ Change the current language.

: quit-game ( -- ) mode-32 quit ;
  \ XXX TMP -- for debugging

: ?quit-game ( -- ) break-key? if quit-game then ;
  \ XXX TMP -- for debugging

: menu ( -- )
  begin
    ?quit-game \ XXX TMP --
    key lower case
    start-key    of  exit           endof \ XXX TMP --
    language-key of change-language variable-menu-screen endof
    \ bl  of  next-controls .controls  endof
    \ 'p' of  change-players .players  endof
    \ XXX TMP --
    endcase
  again ;

: mobilize ( -- )
  mode-32iso init-colors text-attr attr! menu-screen menu ;

  \ ===========================================================
  cr .( Invasion) ?depth debug-point \ {{{1

cvariable invaders \ counter

variable invader-time
  \ When the ticks clock reaches the contents of this variable,
  \ the current invader will move.

: init-invaders ( -- ) init-invaders-data
                       0 set-invader
                       invader-time off
                       invaders coff ;

: at-invader ( -- ) invader~ ~x c@ invader~ ~y c@ at-xy ;
  \ Set the cursor position at the coordinates of the invader.

: invader-frame+ ( n1 -- n2 )
  1+ dup invader~ ~frames c@ < and ;
  \ Increase frame _n1_ resulting frame _n2_.
  \ If the limit was reached, _n2_ is zero.
  \
  \ XXX TODO -- Use `~max-frame <>` for speed.

: invader-udg ( -- c )
  invader~ ~frame c@ dup invader-frame+ invader~ ~frame c!
  [ udg/invader 2 = ] [if] 2* [else] udg/invader * [then]
  invader~ ~sprite c@ + ;
  \ First UDG _c_ of the current frame of the current invader's
  \ sprite, calculated from its sprite and its frame.
  \
  \ XXX TODO -- Add calculation to change the sprite depending
  \ on the flying direction. A flag field is needed to
  \ deactivate this for docked invaders.

: .invader ( -- )
  invader-proper-attr attr! invader-udg .2x1-udg-sprite ;
  \ Display the current invader.

: broken-bricks-coordinates
  ( col1 -- col1 row1 col2 row2 col3 row3 )
  invader~ ~y c@ 2dup 1+ 2dup 2- ;
  \ Convert the x coordinate _col1_ of the broken wall to the
  \ coordinates of the broken brick above the invader, _col3
  \ row3_, below it, _col3 row3_, and in front of it, _col1
  \ row1_.

defer break-the-wall ( col1 row1 col2 row2 -- )
  \ Display the broken wall at the given coordinates of the
  \ broken brick above the invader, _col3 row3_, and below it,
  \ _col2 row2_, and in front of it, _col1 row1_.
  \ The action of this deferred word is set to `break-left-wall`
  \ or `break-right-word`.

: break-left-wall ( col1 row1 col2 row2 -- )
  at-xy broken-top-left-brick .1x1sprite
  at-xy broken-bottom-left-brick .1x1sprite
  at-xy space ;
  \ Display the broken left wall at the given coordinates of the
  \ broken brick above the invader, _col3 row3_, and below it,
  \ _col2 row2_, and in front of it, _col1 row1_.
  \
  \ XXX TODO -- Graphic instead of space.

: break-right-wall ( col1 row1 col2 row2 col3 row3 -- )
  at-xy broken-top-right-brick .1x1sprite
  at-xy broken-bottom-right-brick .1x1sprite
  at-xy space ;
  \ Display the broken right wall at the given coordinates of
  \ the broken brick above the invader, _col3 row3_, and below
  \ it, _col2 row2_, and in front of it, _col1 row1_.
  \
  \ XXX TODO -- Graphic instead of space.

: (break-wall ( col -- )
  broken-bricks-coordinates broken-wall-attr attr!
  break-the-wall ;
  \ Display the broken wall of the building. The wall is at
  \ column _col_, and word _xt_ is used to manage the breaking.

: break-left-container ( -- )
  invader~ ~x c@2+ invader~ ~y c@ at-xy
  broken-top-right-container .1x1sprite
  invader~ ~x c@1+ invader~ ~y c@1+ at-xy
  broken-bottom-left-container .1x1sprite ;
  \ Break the the left side of the container.
  \
  \ XXX TODO -- Calculate alternatives to `c@2+` and `c@1+` at
  \ compile-time, depending on the size of the invaders, just in
  \ case. See `hit-wall?`.

: break-right-container ( -- )
  invader~ ~x c@1- invader~ ~y c@ at-xy
  broken-top-left-container .1x1sprite
  invader~ ~x c@ invader~ ~y c@1+ at-xy
  broken-bottom-right-container .1x1sprite ;
  \ Break the the right side of the container.

: break-container ( -- )
  container-attr attr!
  flying-to-the-left? if   break-right-container exit
                      then break-left-container  ;

: break-container? ( -- f )
  invader~ ~x c@ flying-to-the-left?
  if      containers-right-x c@ = exit
  then 1+ containers-left-x  c@ = ;
  \ Has the current invader broken a container?

: healthy? ( -- f ) invader~ ~stamina c@ max-stamina = ;
  \ Is the current invader healthy? Has it got maximum stamina?

defer invader-left-move-action ( -- )

defer invader-right-move-action ( -- )

      ' invader-left-move-action ,
here  ' noop ,
      ' invader-right-move-action ,
      constant invader-move-actions ( a )
      \ Execution table.

: set-invader-move-action ( -1..1 -- )
  invader-move-actions array> @ invader~ ~action ! ;
  \ Set the invader action to the movement corresponding to
  \ the given x-coordinate increment _-1..1_.

: set-invader-direction ( -1..1 -- )
  dup 0< !> flying-to-the-left?
  dup invader~ ~x-inc !
      set-invader-move-action ;
  \ Set the invader direction corresponding to the given
  \ x-coordinate increment _-1..1_.

: impel-invader ( -- )
  invader~ ~x-inc @ set-invader-move-action ;
  \ Restore the moving action of the current invader, using
  \ its current direction.

: change-direction ( -- )
  invader~ ~x-inc @ negate set-invader-direction ;

: turn-back ( -- ) change-direction set-flying-sprite ;
  \ Make the current invader turn back.

defer breaking-action ( -- )
  \ Action of the invaders that are breaking the wall.

: hit-wall ( -- )
  healthy? if   ['] breaking-action invader~ ~action ! exit
           then turn-back ;
  \ XXX TMP --

: hit-wall? ( -- f )
  invader~ ~x
  [ udg/invader 2 = ]
  [if]   c@2+ flying-to-the-left? 3* +
  [else] [ udg/invader 1 = ]
         [if]   c@1+ flying-to-the-left? 2* +
         [else] c@ udg/invader + flying-to-the-left?
                [ udg/invader 1+ ] cliteral * +
         [then]
  [then]
  invader~ ~y c@ ocr brick = ;
  \ Has the current invader hit the wall of the building?

: ?damages ( -- )
  hit-wall? if hit-wall exit then
  break-container? dup catastrophe ! 0exit break-container ;
  \ Manage the possible damages caused by the current invader.

: undock ( -- ) invader~ ~initial-x-inc @ set-invader-direction
                set-flying-sprite ;
  \ Undock the current invader.

: is-there-a-projectile? ( col row -- f )
  ocr first-projectile-frame last-projectile-frame between ;
  \ XXX TODO -- Accelerate by ocr-ing only the projectiles.

: .sky ( -- ) sky-attr attr! space ;
  \ Display a sky-color space.

: left-of-invader ( -- col row )
  invader~ ~x c@1- invader~ ~y c@ ;
  \ Coordinates _col row_ at the right of the current invader.

: right-of-invader ( -- col row )
  invader~ ~x c@1+ invader~ ~y c@ ;
  \ Coordinates _col row_ at the left of the current invader.

defer ?dock ( -- )
  \ If the current invader is at home, dock it.

: ?flying ( -- ) attacking? if ?damages exit then ?dock ;
  \ If the current invader has reached the wall, the containers
  \ or the dock, manage the situation.

: docked? ( -- f ) invader~ ~x c@ invader~ ~initial-x c@ = ;
  \ Is the current invader at the dock, i.e. at its start
  \ position?

:noname ( -- )
  left-of-invader is-there-a-projectile?
  if docked? ?exit turn-back exit then
  -1 invader~ ~x c+! at-invader .invader .sky ?flying ;
  ' invader-left-move-action defer!
  \ Move the current invader, which is flying to the left,
  \ unless a projectile is at the left.

:noname ( -- )
  right-of-invader is-there-a-projectile?
  if docked? ?exit turn-back exit then
  at-invader .sky .invader 1 invader~ ~x c+! ?flying ;
  ' invader-right-move-action defer!
  \ Move the current invader, which is flying to the right,
  \ unless a projectile is at the right.

cvariable cure-factor  20 cure-factor c!
  \ XXX TMP -- for testing

: difficult-cure? ( -- f )
  max-stamina invader~ ~stamina c@ -
  cure-factor c@ \ XXX TMP -- for testing
  * random 0<> ;
  \ Is it a difficult cure? The flag _f_ is calculated
  \ randomly, based on the stamina: The less stamina, the more
  \ chances to be a difficult cure. This is used to delay the
  \ cure.

: cure ( -- ) invader~ ~stamina c@1+ max-stamina min
              invader~ ~stamina c! ;
  \ Cure the current invader, increasing its stamina.

: ?cure ( -- ) difficult-cure? ?exit cure ;
  \ Cure the current invader, depending on its status.

: ?undock ( -- ) invaders c@ random ?exit undock ;
  \ Undock the current invader randomly, depending on the
  \ number of invaders.
  \
  \ XXX TODO -- Improve the random calculation. Why use
  \ `invaders`?

:noname ( -- )
  healthy? if   ?undock
           else ?cure
           then at-invader .invader ; ' docked-action defer!
  \ Action of the invaders that are docked.

: dock ( -- )
  ['] docked-action invader~ ~action ! set-docked-sprite ;
  \ Dock the current invader.

:noname ( -- ) docked? 0exit dock ; ' ?dock defer!
  \ If the current invader is at the dock, dock it.

: new-breach ( -- ) breachs c1+! battle-breachs c1+! ;

: prepare-wall ( -- col )
  invader~ ~x
  flying-to-the-left? if   c@1-
                           ['] break-right-wall
                      else [ udg/invader 2 = ]
                           [if]   c@2+
                           [else] c@ udg/invader + [then]
                           ['] break-left-wall
                      then ['] break-the-wall defer! ;
  \ Prepare the wall to break: Return the column _col_ of the
  \ wall the current invader has hit, and set the action of
  \ `break-the-wall` accordingly.

: break-wall ( -- )
  prepare-wall (break-wall new-breach impel-invader ;
  \ Break the wall the current invader has hit.

: ?break-wall ( -- ) invaders c@ random ?exit break-wall ;
  \ Break the wall randomly, depending on the number of
  \ invaders.
  \
  \ XXX TODO -- Improve the random calculation. Why use
  \ `invaders`?

:noname ( -- )
  ?break-wall at-invader .invader ; ' breaking-action defer!
  \ Action of the invaders that are breaking the wall.

: last-invader? ( -- f )
  get-invader [ max-invaders 1- ] cliteral = ;
  \ Is the current invader the last one?

: alive? ( -- f ) invader~ ~stamina c@ 0<> ;
  \ Is the current invader alive?

: next-invader ( -- )
  last-invader? if   0 set-invader exit
                then get-invader 1+ set-invader ;
  \ Update the invader to the next one.

1 cconstant invader-interval \ ticks

: schedule-invader ( -- )
  ticks invader-interval + invader-time ! ;

: manage-invaders ( -- )
  invader-time @ past? 0exit
  alive? if invader~ ~action perform then
  next-invader schedule-invader ;
  \ If it's the right time, move the current invader, then
  \ choose the next one.

  \ ===========================================================
  cr .( Mothership) ?depth debug-point \ {{{1

defer mothership-action ( -- )
  \ The current action of the mothership.

defer invisible-mothership-action ( -- )
  \ Action of the mothership when it's invisible.

defer visible-mothership-action ( -- )
  \ Action of the mothership when it's visible.

1 cconstant mothership-y0
  \ Default y coordinate of the mothership.

1 cconstant mothership-y
  \ Current y coordinate of the mothership (0 if destroyed).

variable mothership-x
variable mothership-x-inc

udg/mothership 1- negate  constant visible-mothership-min-x
last-column              cconstant visible-mothership-max-x

                       0 cconstant whole-mothership-min-x
columns udg/mothership -  constant whole-mothership-max-x

variable mothership-stopped  mothership-stopped off
  \ Flag: did the mothership stopped in the current flight?

: ~~mothership-info ( -- )
  home ." x:" mothership-x ?
       ." inc.:" mothership-x-inc ? ;
  \ ' ~~mothership-info ' ~~app-info defer!
  \ XXX TMP -- For debugging.

defer set-mothership-direction ( -1..1 -- )

: mothership-turns-back ( -- )
  mothership-stopped off
  mothership-x-inc @ negate set-mothership-direction ;

32 cconstant mothership-range
  \ Allowed x coordinate positions of the mothership out of the
  \ screen, in either direction.

mothership-range negate     constant mothership-range-min-x
mothership-range columns + cconstant mothership-range-max-x
  \ X-coordinate limits of the mothership range.

: start-mothership ( -- ) -1|1 set-mothership-direction ;

variable mothership-time
  \ When the ticks clock reaches the contents of this variable,
  \ the mothership will move.

: mothership-x0 ( -- n )
  2 random if   mothership-range-min-x
                udg/mothership negate
           else whole-mothership-max-x udg/mothership +
                mothership-range-max-x
           then random-between ;
  \ Return random initial horizontal location _n_ of the
  \ mothership, out of the screen.

: place-mothership ( -- )
  mothership-x0 mothership-x ! mothership-y0 c!> mothership-y ;
  \ Set the initial coordinates of the motherhip, out of the
  \ screen.

: set-mothership-sprite ( c n -- )
  c!> mothership-frames c!> mothership mothership-frame coff ;
  \ Set character _c_ as the first character of the first
  \ sprite of the mothership, and _n_ as the number of frames.

: set-flying-mothership-sprite ( -- )
  flying-mothership-sprite flying-mothership-frames
  set-mothership-sprite ;
  \ Make the mothership use its flying sprite.

: set-beaming-mothership-sprite ( -- )
  beaming-mothership-sprite beaming-mothership-frames
  set-mothership-sprite ;
  \ Make the mothership use its beaming sprite.

: set-exploding-mothership-sprite ( -- )
  exploding-mothership-sprite exploding-mothership-frames
  set-mothership-sprite ;
  \ Make the mothership use its exploding sprite.

: init-mothership ( -- )
  set-flying-mothership-sprite
  ['] invisible-mothership-action ['] mothership-action defer!
  mothership-stopped off mothership-time off
  place-mothership start-mothership ;

: visible-mothership? ( -- f )
  mothership-x @
  visible-mothership-min-x visible-mothership-max-x between ;
  \ Is the mothership visible?

: visible-whole-mothership? ( -- f )
  mothership-x @
  whole-mothership-min-x whole-mothership-max-x between ;
  \ Is the whole mothership visible?

: mothership-coordinates ( -- row col )
  mothership-x @ mothership-y ;
  \ Return the cursor coordinates of the mothership.

: at-mothership ( -- ) mothership-coordinates at-xy ;
  \ Set the cursor position at the coordinates of the
  \ the mothership.

: -mothership ( -- ) at-mothership udg/mothership spaces ;
  \ Delete the whole mothership.

: mothership-frame+ ( n1 -- n2 )
  1+ dup mothership-frames < and ;
  \ Increase frame _n1_ of the mothership, resulting frame _n2_.
  \ If _n1_ is the maximum frame allowed, _n2_ is zero, which is
  \ the first one.
  \
  \ XXX TODO -- Use `mothership-max-frame <>` for speed.

: mothership-udg ( -- c )
  mothership-frame c@ dup mothership-frame+ mothership-frame c!
  [ udg/mothership 2 = ] [if] 2* [else] udg/mothership * [then]
  mothership + ;
  \ UDG _c_ of the mothership.

: advance-mothership ( -- )
  mothership-x-inc @ mothership-x +! ;
  \ Advance the mothership on its current direction,
  \ adding its x increment to its x coordinate.

: mothership-in-range? ( -- f )
  mothership-x @
  mothership-range-min-x mothership-range-max-x within ;
  \ Is the mothership in the range of its flying limit?

: (.mothership ( -- )
  mothership-attr attr! mothership-udg .2x1-udg-sprite ;
  \ Display the mothership, which is fully visible, at the
  \ cursor coordinates.

: .mothership ( -- ) at-mothership (.mothership ;
  \ Display the mothership, which is fully visible, at its
  \ coordinates.

: (.visible-right-mothership ( -- )
  mothership-attr attr!
  mothership-udg [ udg/mothership 1- ] cliteral + emit-udg ;
  \ Display the mothership, which is partially visible (only its
  \ right side is visible) at the cursor coordinates.

: .visible-right-mothership ( -- )
  whole-mothership-min-x mothership-y at-xy
  (.visible-right-mothership ;
  \ Display the mothership, which is partially visible (only its
  \ right side is visible) at its proper coordinates.

: (.visible-left-mothership ( -- )
  mothership-attr attr! mothership-udg emit-udg ;
  \ Display the mothership, which is partially visible (only its
  \ left side is visible) at the cursor coordinates.

: .visible-left-mothership ( -- )
  visible-mothership-max-x mothership-y at-xy
  (.visible-left-mothership ;
  \ Display the mothership, which is partially visible (only its
  \ left side is visible) at its proper coordinates.

: .visible-mothership ( -- )
  mothership-x @ case
    visible-mothership-min-x of .visible-right-mothership endof
    visible-mothership-max-x of .visible-left-mothership  endof
    .mothership
  endcase ;
  \ Display the mothership, which is fully or partially visible.

: right-of-mothership ( -- col row )
  mothership-x @ [ udg/mothership 2 = ] [if]   2+
                                        [else] udg/mothership +
                                        [then] mothership-y ;
  \ Return coordinates _col row_ of the position at the right
  \ of the mothership.

: (move-visible-mothership-right ( -- )
  mothership-x @ case
    whole-mothership-max-x   of
      at-mothership .sky (.visible-left-mothership endof
    visible-mothership-max-x of at-mothership .sky endof
    visible-mothership-min-x of
      whole-mothership-min-x mothership-y at-xy
      (.mothership                                 endof
    at-mothership .sky (.mothership
  endcase advance-mothership ;
  \ Do move the visible mothership to the right.

0 layer>y cconstant invader-min-y

   beam-attr dup join constant beam-cell-attr
invader-attr dup join constant invader-cell-attr
    sky-attr dup join constant sky-cell-attr

 variable beaming beaming off \ flag/direction (1|0|-1)

 variable beam-y       \ row
cvariable beam-first-y \ row
cvariable beam-last-y  \ row

cvariable beam-invader
  \ Number of the next invader to be created by the beam.

defer beam ( -- )
  \ Manage the beam.

: set-beam ( row1 row2 xt -- )
  ['] beam defer!
  2dup beam-last-y c! dup beam-first-y c! beam-y !
       swap - polarity beaming ! ;
  \ Set the beam to grow or shrink from _row1_ to _row2_ with
  \ handler _xt_.

: over-left-invaders? ( -- f )
  mothership-x @ invaders-min-x = ;
  \ Is the mothership over the left initial column of invaders?

: over-right-invaders? ( -- f )
  mothership-x @ invaders-max-x = ;
  \ Is the mothership over the right initial column of invaders?

: enlist-squadron ( -- ) half-max-invaders invaders c+! ;

: create-squadron ( -- )
  over-left-invaders? if   create-left-squadron
                      else create-right-squadron
                      then enlist-squadron ;
  \ Activate the new invaders created by the beam and update
  \ their global count.

: layer-y? ( row -- f )
  dup invader-top-y invader-bottom-y between
      swap rows/layer mod 0= and ;
  \ Is _y_ is a valid row of an invader layer?

: reach-invader? ( -- f ) beam-y @ layer-y? ;
  \ Has the beam reached the row of an invader's layer?

: (beaming-up? ( -- )
  beam-y @ beam-last-y c@ beam-first-y c@ between ;
  \ Is the beam still shrinking?

: beaming-up? ( -- 1|0|-1 )
  beaming @ dup beam-y +! (beaming-up? and dup beaming ! ;
  \ Update variables of a shrinking beam. Also update `beaming`
  \ accordingly and return a copy of its content.

: (beam-up ( -- )
  .mothership
  reach-invader? if invader-cell-attr else sky-cell-attr then
  mothership-x @ beam-y @ xy>attra ! ;
  \ Shrink the beam towards de mothership one character.

: beam-up ( -- ) (beam-up beaming-up? ?exit
                 set-flying-mothership-sprite .mothership
                 create-squadron ;
  \ Manage the beam, which is shrinking up to the mothership.
  \ If it's finished, activate the new invaders.

: beam-off ( -- )
  invader-min-y 1+ mothership-y 1+ ['] beam-up set-beam ;
  \ Turn the mothership's beam off, i.e. start shrinking it.

: create-invader ( -- )
  beam-attr attr! beam-invader dup c@ .parade-invader c1+! ;
  \ Display the new invader and update its number.

: (beaming-down? ( -- f )
  beam-y @ beam-first-y c@ beam-last-y c@ between ;
  \ Is the beam still growing?

: beaming-down? ( -- 1|0|-1 )
  beaming @ dup beam-y +! (beaming-down? and dup beaming ! ;
  \ Update variables of a growing beam. Also update `beaming`
  \ accordingly and return a copy of its content.

: (beam-down ( -- )
  .mothership beam-cell-attr mothership-x @ beam-y @ xy>attra !
  reach-invader? if create-invader then ;
  \ Grow the beam towards the ground one character.

: beam-down ( -- ) (beam-down beaming-down? ?exit beam-off ;
  \ Manage the beam, which is growing down to the ground.
  \ If it's finished, display the new invaders and start
  \ shrinking the beam.

: first-new-invader ( -- n )
  half-max-invaders over-left-invaders? 0= and ;
  \ Return the number of the first invader to create,
  \ depending on the position of the mothership.

: beam-on ( -- )
  set-beaming-mothership-sprite
  first-new-invader beam-invader c!
  mothership-y 1+ invader-min-y 1+ ['] beam-down set-beam ;
  \ Turn the mothership's beam on.

: need-help? ( n -- f ) 0= dup 0exit beam-on ;
  \ If number of invaders _n_ is zero turn the beam on and
  \ return _true_; otherwise do nothing and return _false_.

: help-right-side? ( -- f ) right-side-invaders need-help? ;
  \ If there's no invader at the right side, turn the beam on
  \ and return _true_; otherwise do nothing and return _false_.

: help-left-side? ( -- f ) left-side-invaders need-help? ;
  \ If there's no invader at the left side, turn the beam on and
  \ return _true_; otherwise do nothing and return _false_.

: help-invaders? ( -- f )
  over-left-invaders?  if help-left-side?  exit then
  over-right-invaders? if help-right-side? exit then false ;
  \ If needed, help the invaders below the mothership.
  \ If the help was effective, return _true_; otherwise return
  \ _false_.

: move-visible-mothership-right ( -- )
  right-of-mothership is-there-a-projectile?
  if mothership-turns-back exit then
  help-invaders? ?exit
  (move-visible-mothership-right ;
  \ Move the visible mothership to the right, if possible.

: left-of-mothership ( -- col row )
  mothership-x @ 1- mothership-y ;
  \ Return coordinates _col row_ of the position at the left
  \ of the mothership.

: (move-visible-mothership-left ( -- )
  mothership-x @ case
    whole-mothership-min-x                               of
      .visible-right-mothership .sky advance-mothership  endof
    visible-mothership-min-x                             of
      0 mothership-y at-xy .sky advance-mothership       endof
    [ whole-mothership-max-x udg/mothership + ] cliteral of
      [ last-column ] cliteral mothership-y at-xy
      (.visible-left-mothership advance-mothership       endof
    advance-mothership .mothership .sky
  endcase ;
  \ Do move the visible mothership to the left.

: move-visible-mothership-left ( -- )
  left-of-mothership is-there-a-projectile?
  if mothership-turns-back exit then
  help-invaders? ?exit
  (move-visible-mothership-left ;
  \ Move the visible mothership to the left, if possible.

defer move-visible-mothership ( -- )
  \ Execute the proper movement of the mothership.

      ' move-visible-mothership-left ,
here  ' noop ,
      ' move-visible-mothership-right ,

constant visible-mothership-movements ( -- a )
  \ Execution table.

:noname ( -1..1 -- )
  dup mothership-x-inc !
      visible-mothership-movements array> @
      ['] move-visible-mothership defer! ;
' set-mothership-direction defer!

: above-building? ( -- f )
  mothership-x @
  building-left-x c@
  building-right-x c@ [ udg/mothership 1- ] cliteral -
  between ;
  \ Is the mothership above the building?

: stopped-mothership? ( -- f ) mothership-x-inc @ 0= ;

: stop-mothership ( -- ) 0 set-mothership-direction
                         mothership-stopped on ;

: ?stop-mothership ( -- ) mothership-stopped @ ?exit
                            above-building? 0= ?exit
                                      3 random ?exit
                               stop-mothership ;

: ?start-mothership ( -- ) 9 random ?exit start-mothership ;

:noname ( -- )
  stopped-mothership? if ?start-mothership exit then
  beaming @ if beam exit then
    \ XXX TODO -- Make beaming an action.
  move-visible-mothership
  visible-mothership? 0= if ['] invisible-mothership-action
                            ['] mothership-action defer!  exit
                         then
  ?stop-mothership ; ' visible-mothership-action defer!
  \ Action of the mothership when it's visible.

:noname ( -- )
  advance-mothership
  visible-mothership? if .visible-mothership
                         ['] visible-mothership-action
                         ['] mothership-action defer!  exit
                      then
  mothership-in-range? ?exit mothership-turns-back ;
  ' invisible-mothership-action defer!
  \ Action of the mothership when it's invisible.

6 cconstant mothership-interval \ ticks

: schedule-mothership ( -- )
  ticks mothership-interval + mothership-time ! ;

: manage-mothership ( -- )
  mothership-y            0exit
  mothership-time @ past? 0exit
  mothership-action schedule-mothership ;

  \ ===========================================================
  cr .( Impact) ?depth debug-point \ {{{1

' shoot alias mothership-bang ( -- )
  \ Make the explosion sound of the mothership.
  \ XXX TMP --
  \ XXX TODO -- look for a better sound

100 cconstant mothership-explosion-duration \ in ticks

variable mothership-explosion-time
  \ When the ticks clock reaches the contents of this variable,
  \ the mothership explosion will finish.

: schedule-mothership-explosion ( -- )
  ticks mothership-explosion-duration +
  mothership-explosion-time ! ;

: exploding-mothership-action ( -- )
  .mothership  mothership-explosion-time @ past? 0exit
  -mothership  0 c!> mothership-y ;
  \ Action of the mothership when it's exploding.

: mothership-bonus ( -- n ) location c@1+ 250 * ;
  \ Bonus points for impacting the mothership.

: set-exploding-mothership ( -- )
  set-exploding-mothership-sprite
  ['] exploding-mothership-action ['] mothership-action defer!
  mothership-bang mothership-bonus update-score
  schedule-mothership-explosion ;
  \ The mothership has been impacted. Set it accordingly.

' shoot alias invader-bang ( -- )
  \ Make the explosion sound of an invader.
  \ XXX TMP --
  \ XXX TODO -- look for a better sound

: invader-on-fire ( -- )
  at-invader invader-explosion-sprite .2x1-udg-sprite ;
  \ Display the current invader on fire.

: -invader ( -- ) sky-attr attr! at-invader 2 spaces ;
  \ Delete the current invader.

: invader-explosion ( -- )
  invader-on-fire invader-bang -invader ;
  \ Display the explosion of the current invader.

: impacted-invader ( -- n )
  projectile-y c@ invader-top-y - 2/
  projectile-x c@ [ columns 2/ ] cliteral > abs 5 * + ;
  \ Return the impacted invader (0..9), calculated from the
  \ projectile coordinates: Invaders 0 and 5 are at the top,
  \ one row below the top of the building; 1 and 6 are two rows
  \ below and so on.  0..4 are at the left of the screen; 5..9
  \ are at the right.
  \
  \ XXX TODO -- Change the numbering of invaders to reading
  \ order, to simplify the calculation: 0: top left; 1: top
  \ right; etc..

: explode ( -- )
  invader-destroy-points update-score invader-explosion
  invaders c1-! invader~ ~stamina coff invader~ ~action off ;
  \ The current invader explodes.
  \
  \ XXX TODO -- Set an action to show the explosion.

' lightning1 alias retreat-sound
  \ XXX TMP --
  \ XXX TODO -- look for a proper sound

: retreat ( -- )
  retreat-sound invader-retreat-points update-score turn-back ;
  \ The current invader retreats.

: wounded ( -- )
  invader~ ~stamina c@1- 1 max invader~ ~stamina c! ;
  \ Reduce the invader's stamina after being shoot.

: mortal? ( -- f ) invader~ ~stamina c@ 2* random 0= ;
  \ Is it a mortal impact?  _f_ depends on a random calculation
  \ based on the stamina: The more stamina, the less chances to
  \ be a mortal impact.

: (invader-impacted ( -- ) mortal? if explode exit then
                           wounded attacking? 0exit retreat ;
  \ The current invader has been impacted by the projectile.
  \ It explodes or retreats.
  \
  \ XXX TODO -- Improve the logic: First wound, then check
  \ death.

: invader-impacted ( -- )
  get-invader impacted-invader set-invader (invader-impacted
  set-invader ;
  \ An invader has been impacted by the projectile.
  \ Make it the current one and manage it.

: mothership-impacted? ( -- f )
  [pixel-projectile]
  [if]   projectile-coords gxy>attra c@ mothership-attr =
  [else] projectile-y c@ mothership-y =  [then] ;

: impact ( -- )
  mothership-impacted? if   set-exploding-mothership
                       else invader-impacted
                       then destroy-projectile ;

: hit-something? ( -- f )
  projectile-coords
  [pixel-projectile] [if]   \ gxy>attra c@ sky-attr <>
                            get-pixel \ XXX NEW
                     [else] ocr 0<> [then] ;
  \ Did the projectile hit something?

: impacted? ( -- f ) hit-something? dup if impact then ;
  \ Did the projectil impacted?
  \ If so, do manage the impact.

  \ ===========================================================
  cr .( Shoot) ?depth debug-point \ {{{1

[pixel-projectile] 0= [if]

: at-projectile ( -- ) projectile-coords at-xy ;
  \ Set the cursor position at the coordinates of the
  \ projectile.

: projectile ( -- c )
  projectile-sprite frames/projectile random + ;
  \ Return the UDG _c_ of a random frame of the projectile.

[then]

: .projectile ( -- )
  projectile-attr attr!
  [pixel-projectile] [if]   projectile-coords plots
                     [else] at-projectile projectile .1x1sprite
                     [then] ;
  \ Display the projectile.

' whip alias fire-sound ( -- )

: -projectile ( -- )
  [pixel-projectile]
  [if]   projectile-coords reset-pixel
  [else] projectile-coords xy>attr projectile-attr <> ?exit
         at-projectile .sky
  [then] ;
  \ Delete the projectile.

: projectile-lost? ( -- f )
  projectile-y c@
  [pixel-projectile]
  [if]   [ sky-top-y y>gy ] cliteral >
  [else] [ sky-top-y 1+ ] cliteral <
  [then] ;
  \ Is the projectile lost?

: move-projectile ( -- )
  -projectile projectile-lost? if destroy-projectile exit then
  [pixel-projectile] [if] 7 [else] -1 [then] projectile-y c+!
  impacted? ?exit .projectile ;
  \ Manage the projectile.

cvariable trigger-delay-counter trigger-delay-counter coff

[pixel-projectile] [if]   8
                   [else] 6
                   [then] cconstant trigger-delay

: delay-trigger ( -- )
  trigger-delay trigger-delay-counter c! ;

: damage-transmission ( -- ) 1 transmission-damage +! ;

: fire ( -- )
  1 used-projectiles +!
  x> c!> projectile#
  new-projectile-x projectile-x c!
  [pixel-projectile]
  [if]   [ tank-y y>gy 1+ ] cliteral
  [else] [ tank-y 1- ] cliteral
  [then] projectile-y c!
  .projectile fire-sound delay-trigger damage-transmission ;
  \ The tank fires.
  \ XXX TODO -- confirm `tank-y 1-`

: flying-projectile? ( -- f ) projectile-y c@ 0<> ;
  \ Is the current projectile flying?

: projectile-left? ( -- f ) xdepth 0<> ;
  \ Is there any projectile left?

: update-trigger ( -- ) trigger-delay-counter ?c1-! ;
  \ Decrement the trigger delay, if it's not zero.

: trigger-ready? ( -- f ) trigger-delay-counter c@ 0= ;
  \ Is the trigger ready?

: trigger-pressed? ( -- f ) kk-fire pressed? ;
  \ Is the trigger pressed?

: next-projectile ( -- )
  projectile# 1+ max-projectile# and c!> projectile# ;
  \ Point to the next current projectile.

: fly-projectile ( -- )
  flying-projectile? if move-projectile then next-projectile ;
  \ Manage the shoot.

: lose-projectiles ( -- )
  begin fly-projectile xdepth #projectiles = until ;
  \ Lose all flying projectiles.

: shooting ( -- )
  update-trigger
  trigger-pressed?    0exit
  trigger-ready?      0exit
  projectile-left?    0exit
  gun-below-building? ?exit fire ;
  \ Manage the gun.

: new-record? ( -- f ) score @ record @ > ;
  \ Is there a new record?

: new-record ( -- ) score @ record ! ;
  \ Set the new record.

: check-record ( -- ) new-record? if new-record then ;
  \ Set the new record, if needed.

  \ ===========================================================
  cr .( Players config) ?depth debug-point \ {{{1

  \ XXX TODO --

  \ ===========================================================
  cr .( Location titles) ?depth debug-point \ {{{1

1 gigatype-style c!

: .location ( ca len y -- ) 0 swap at-xy gigatype-title ;
  \ Display location name part _ca len_, centered at row _y_.

: (location-title ( n -- )
  dup >town$     6 .location
  dup >region$  12 .location
      >country$ 18 .location ;
  \ Display the title of location _n_.

: veiled-location-title ( n -- )
  attr@ >r black attr! (location-title r> attr! ;
  \ Display a veiled (black ink on black paper) title of
  \ location _n_.

: unveil ( -- ) attributes /attributes white fill ;
  \ Unveil the contents of the screen, by filling the
  \ attributes with white ink on black paper.

: location-title ( n -- ) veiled-location-title unveil ;
  \ Display the title of location _n_.

: announce ( n -- )
  blackout location-title 3 seconds blackout ;
  \ Announce arriving to location _n_ by displaying its name.

: arrive ( n -- )
  dup announce landscape>screen building repaired off ;
  \ Arrive to location _n_ by displaying its name and scenery.

: settle ( -- ) location c@ arrive status-bar ;
  \ Settle in the current location.

  \ ===========================================================
  cr .( The end) ?depth debug-point \ {{{1

  \ : defeat-tune ( -- )
  \   100 200 do  i 20 beep  -5 +loop ;
  \ XXX TODO -- original code in Ace Forth

: defeat-tune ( -- )
  10470 5233 do  i 20 dhz>bleep bleep  261 +loop ;
  \ XXX REMARK -- sound converted from Ace Forth `beep` to
  \ Solo Forth's `bleep`
  \
  \ XXX TODO -- 128 sound

: radiation ( -- )
  [  attributes /status-bar + ] literal
  [ /attributes /status-bar - ] literal radiation-attr fill ;
  \ Fill the screen with the radiation color, except the status
  \ bar.

: defeat ( -- )
  preserve-attributes
  radiation defeat-tune 2 seconds fade-display
  restore-attributes
  check-record ;
  \ XXX TODO -- Finish.
  \ XXX TODO -- Factor.

  \ ===========================================================
  cr .( Reports) ?depth debug-point \ {{{1

7 3 18 18 window constant paper-report-window
8 4 16 16 window constant report-window

here \ es (Spanish)
  s" ¡Bien hecho!" s,

here \ eo (Esperanto)
   s" Bone farita!" s,

here \ en (English)
  s" Well done!" s,

localized-string well-done$ ( -- ca len )

here \ es (Spanish)
  s" Se prevé un nuevo ataque inminente." s,

here \ eo (Esperanto)
   s" Nova tuja atako antaŭvideblas." s,

here \ en (English)
  s" A new imminent attack is expected." s,

localized-string about-new-attack$ ( -- ca len )

here \ es (Spanish)
  s" El ataque ha sido rechazado, "
  s" pero los muros han sido dañados." s+ s,

here \ eo (Esperanto)
   s" La atako estis repuŝita, sed "
   s" la muroj estas damaĝitaj." s+ s,

here \ en (English)
  s" The attack has been repelled, "
  s" but the walls have been damaged." s+ s,

localized-string about-new-damages$ ( -- ca len )

here \ es (Spanish)
  s" El ataque ha sido rechazado sin "
  s" causar nuevos daños, pero los " s+
  s" muros aún están dañados y deben " s+
  s" ser reparados." s+ s,
  \ XXX TODO -- Improve.

here \ eo (Esperanto)
   s" La atako estis repuŝita sen "
   s" kaŭzi novajn damaĝojn, sed la " s+
   s" muroj ankoraŭ estas damaĝitaj " s+
   s" kaj devas esti riparitaj." s+ s,
  \ XXX TODO -- Improve.

here \ en (English)
  s" The attack has been repelled "
  s" without causing new damages, but " s+
  s" the walls are still damaged and " s+
  s" must be repaired." s+ s,
  \ XXX TODO -- Improve.

localized-string about-old-damages$ ( -- ca len )

here \ es (Spanish)
  s" Los invasores han sido aniquilados "
  s" antes de que pudieran dañar el edificio. " s+ s,
  \ XXX TODO -- Improve.

here \ eo (Esperanto)
  s" La invadantoj estis destruitaj "
  s" antaŭ ol ili povis damaĝi la konstruaĵon. " s+ s,
  \ XXX TODO -- Improve.

here \ en (English)
  s" The invaders have been destroyed "
  s" before they were able to damage the building. " s+ s,
  \ XXX TODO -- Improve.

localized-string about-battle$ ( -- ca len )

here \ es (Spanish)
  s" Ahora se dirigen al sur, "
  s" hacia su próximo objetivo." s+ s,
  \ XXX TODO -- Improve.

here \ eo (Esperanto)
  s" Nun ili flugas suden "
  s" al ilia posta celo." s+ s,
  \ XXX TODO -- Improve.

here \ en (English)
  s" Now they fly south "
  s" toward their next objective." s+ s,
  \ XXX TODO -- Improve.

localized-string about-next-location$ ( -- ca len )

: paragraph ( ca len -- ) wltype wcr wcr ;

: about-attack ( -- )
                   well-done$              paragraph
  new-breach? if   about-new-damages$      paragraph
                   about-new-attack$
              else about-old-damages$ then paragraph ;

: unfocus ( -- ) attributes /attributes unfocus-attr fill ;
  \ Unfocus the screen to contrast the report window.

: end-report ( -- ) reveal-report-attr dup wcolor 2000 ms attr!
                    press-any-key$ wltype new-key drop ;

: open-report ( -- ) unfocus
                     paper-report-window current-window !
                     hide-report-attr attr! wcls
                     report-window current-window ! whome ;

: (attack-report ( -- ) open-report about-attack end-report ;

: attack-report ( -- )
  preserve-screen (attack-report restore-screen ;

: about-battle ( -- ) well-done$           paragraph
                      about-battle$        paragraph
                      about-next-location$ paragraph ;

: battle-report ( -- ) open-report about-battle end-report ;

  \ ===========================================================
  cr .( Main loop) ?depth debug-point \ {{{1

: mothership-destroyed? ( -- f )
  mothership-y mothership-y0 <> ;

: invaders-destroyed? ( -- f ) invaders c@ 0= ;

: extermination? ( -- f )
  invaders-destroyed? mothership-destroyed? and ;

: attack-wave ( -- ) init-mothership init-invaders ;

: fight ( -- )
  ?quit-game \ XXX TMP --
  fly-projectile driving
  fly-projectile shooting
  fly-projectile manage-mothership
  fly-projectile manage-invaders ;

: end-of-attack? ( -- f ) extermination? catastrophe? or ;

: under-attack ( -- )
  check-breachs attack-wave begin fight end-of-attack? until
  lose-projectiles ;

: another-attack? ( -- f ) breachs? catastrophe? 0= and ;

: weapons ( -- ) new-tank new-projectiles ;

: prepare-battle ( -- ) settle weapons ;

: interlude ( -- ) new-breach? ?exit repair-building ;

: battle ( -- )
  prepare-battle begin under-attack another-attack?
                 while attack-report interlude repeat ;

: campaign ( -- ) begin battle catastrophe? 0=
                  while battle-report reward travel repeat ;

: war ( -- ) prepare-war campaign defeat ;

: run ( -- ) begin mobilize war again ;

  \ ===========================================================
  cr .( Debugging tools) ?depth debug-point \ {{{1

: half ( -- ) half-max-invaders c!> max-invaders ;
  \ Reduce the actual invaders to the left half.

: .udgs ( -- ) cr last-udg 1+ 0 do i emit-udg loop ;
  \ Display all game UDGs.

: fp ( -- ) fly-projectile ;
: fp? ( -- f ) flying-projectile? ;
: mp ( -- ) move-projectile ;

: ni ( -- ) next-invader ;
: mi ( -- ) manage-invaders ;
: ia ( -- ) invader~ ~action perform ;
: ini ( -- ) prepare-war prepare-battle attack-wave ;

: h ( -- ) 7 attr! home ; \ home
: b ( -- ) cls building h ; \ building
: t ( -- ) .tank h ;
: tl ( -- ) <tank h ; \ move tank left
: tr ( -- ) tank> h ; \ move tank right

: mm ( -- ) manage-mothership ;
: im ( -- ) invisible-mothership-action ;
: vm ( -- ) visible-mothership-action ;
: am ( -- ) advance-mothership ;
: vm? ( -- f ) visible-mothership? ;
: .m ( -- ) .mothership ;
: .vm ( -- ) .visible-mothership ;
: m? ( -- f ) mothership-in-range? ;
: -m ( -- ) -mothership ;
: mx ( -- col ) mothership-x @ ;
: im ( -- ) init-mothership ;
: m ( -- ) begin key 'q' <> while manage-mothership repeat ;

: beon ( -- ) beam-on ;
: beoff ( -- ) beam-off ;
: beu ( -- ) beam-up ;
: bed ( -- ) beam-down ;
: be ( -- ) beam ;

: test-be ( -- )
  mothership-x off .visible-mothership beam-on m ;

: left-only ( -- ) half-max-invaders c!> max-invaders ;
  \ Reduce the actual invaders to the left half.

: (kill ( n1 n2 -- ) ?do i invader#>~ ~stamina coff loop ;
  \ Kill invaders from _n2_ to _n1_, not including _n1_.

: kill-left ( -- ) half-max-invaders 0 (kill ;
  \ Kill the left-side invaders.

: kill-right ( -- ) max-invaders half-max-invaders (kill ;
  \ Kill the right-side invaders.


: .i ( n -- )
  >r
  ." sprite              " r@ invader#>~ ~sprite c@ . cr
  ." frame               " r@ invader#>~ ~frame c@ . cr
  ." frames              " r@ invader#>~ ~frames c@ . cr
  ." stamina             " r@ invader#>~ ~stamina c@ . cr
  ." x                   " r@ invader#>~ ~x c@ . cr
  ." initial-x           " r@ invader#>~ ~initial-x c@ . cr
  ." x-inc               " r@ invader#>~ ~x-inc ? cr
  ." initial-x-inc       " r@ invader#>~ ~initial-x-inc ? cr
  ." y                   " r@ invader#>~ ~y c@ . cr
  ." action              " r@ invader#>~ ~action @ >name .name
                           cr
  ." species             " r@ invader#>~ ~species dup u. cr @
  ." SPECIES DATA:" cr
  rdrop >r
  ." flying-right-sprite " r@ ~flying-right-sprite c@ . cr
  ." docked-sprite       " r@ ~docked-sprite c@ . cr
  rdrop ;

: bc ( -- )
  cls
  \ top:
  space broken-top-right-container .1x1sprite
  container-top .2x1-udg-sprite
  broken-top-left-container .1x1sprite space cr
  \ bottom:
  container-bottom .2x1-udg-sprite 8 emit 8 emit
  broken-bottom-left-container .1x1sprite
  xy swap 1+ swap at-xy
  container-bottom .2x1-udg-sprite
  container-bottom .2x1-udg-sprite 8 emit
  broken-bottom-right-container .1x1sprite cr ;
  \ Display the graphics of the broken containers.

  \ ===========================================================
  cr .( Development benchmarks) ?depth debug-point \ {{{1

0 [if]

need bench{ need }bench.

: sprite-string-bench ( n -- )
  dup page ." type-udg :"
  bench{ 0 ?do 18 0 at-xy right-arrow$ type-udg
            loop }bench.
  0 1 at-xy ." .2x1-udg-sprite :"
  bench{ 0 ?do 18 1 at-xy right-arrow .2x1-udg-sprite
           loop }bench. ;

  \ 2017-07-27:
  \
  \ |===================================
  \ |       | Frames
  \ |       | ==========================
  \ | Times | type-udg | .2x1-udg-sprite
  \
  \ |  1000 |      112 |              94
  \ | 10000 |     1122 |             937
  \ | 65535 |     7353 |            6142
  \ |===================================

[then]

  \ ===========================================================
  cr .( Greeting) ?depth debug-point \ {{{1

cls .( Nuclear Waste Invaders)
cr version$ type
cr .( Loaded)

cr cr greeting

cr cr .( Type RUN to start) cr

end-program

  \ vim: filetype=soloforth:colorcolumn=64
