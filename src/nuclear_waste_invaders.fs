  \ nuclear_waste_invaders.fs

  \ This file is part of Nuclear Waste Invaders
  \ http://programandala.net/en.program.nuclear_waste_invaders.html

( nuclear-waste-invaders )

\ Nuclear Waste Invaders

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

: version$ ( -- ca len ) s" 0.185.0+201802082317" ;

cr cr .( Nuclear Waste Invaders) cr version$ type cr

  \ ===========================================================
  cr .( Options) \ {{{1

  \ Flags for conditional compilation of new features under
  \ development.

true constant [breakable] immediate
  \ Make the program breakable with the BREAK key combination?

true constant [udg] immediate
  \ Reference graphics with their UDG codes (old method)
  \ instead of their addresses (new method).

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
  cr .(   -Development tools) \ {{{2

need [if] need ~~
need warn.message need order need see need rdepth need where
need evaluate need ?depth need see-colon-body>

  \ --------------------------------------------
  cr .(   -Definers) ?depth \ {{{2

need defer need action-of need alias need cvariable
need 2const need cenum need c!>

  \ --------------------------------------------
  cr .(   -Strings) ?depth \ {{{2

need upper need s+ need char>string need s\"

  \ --------------------------------------------
  cr .(   -Control structures) ?depth \ {{{2

need case need 0exit need +perform need do need abort"
need cond need thens

  \ --------------------------------------------
  cr .(   -Memory) ?depth \ {{{2

need c+! need c-! need c1+! need c1-! need ?c1-! need coff
need dzx7t need bank-start need c@1+ need c@1- need c@2+
need 1+! need c@2- need con

  \ --------------------------------------------
  cr .(   -Math) ?depth \ {{{2

need d< need -1|1 need 2/ need between need random need binary
need within need even? need 8* need random-between
need join need 3* need polarity

  \ --------------------------------------------
  cr .(   -Data structures) ?depth \ {{{2

need roll need cfield: need field: need +field-opt-0124
need array> need !> need c!> need 2!> need 0dup

need sconstants

need xstack need allot-xstack need xdepth need >x need x>
need xclear

  \ --------------------------------------------
  cr .(   -Display) ?depth \ {{{2

need at-y need at-x need type-left-field need type-right-field
need type-center-field need gigatype-title need mode-32iso

  \ --------------------------------------------
  cr .(   -Graphics) ?depth \ {{{2

need set-udg need /udg need /udg+
need columns need rows need row need fade-display
need last-column need udg-block need udg! need blackout

need window need wltype need wcr need wcls need wcolor

need xy>attr

need inverse-off need overprint-off need attr!  need attr@

need black need blue   need red   need magenta need green
need cyan  need yellow need white

need papery need brighty need xy>attr need xy>attra
need bright-mask

  \ --------------------------------------------
  cr .(   -Keyboard) ?depth \ {{{2

need kk-ports need kk-1# need pressed? need kk-chars
need #>kk need inkey need new-key

  \ --------------------------------------------
  cr .(   -Time) ?depth \ {{{2

need ticks need ms need seconds need ?seconds need past?

  \ --------------------------------------------
  cr .(   -Sound) ?depth \ {{{2

need bleep need dhz>bleep need shoot-sound
need whip-sound need lightning1-sound

  \ --------------------------------------------
  \ Files

need tape-file> need last-tape-header

  \ --------------------------------------------
  \ Debugging tools

need .xs need dump

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
  ~~? @ 0exit
  base @ >r decimal latest .name .s r> base !
  key drop ;

'q' ~~quit-key c!  $FF ~~resume-key c!  22 ~~y c!  ~~? on

: ~~stack-info ( -- )
  0 ~~y c@ at-xy ." D:" depth . ." R:" rdepth . space ;
  \ XXX TMP -- for debugging

' ~~stack-info ' ~~app-info defer!
  \ XXX TMP -- for debugging

: notice ( -- ) 1024 0 ?do i %11 and border loop ;

: borderx ( n -- )
  1024 0 ?do dup border white border loop drop black border ;

  \ ===========================================================
  cr .( Optimization) ?depth \ {{{1

  \ Some words used to optimize during compilation, just in
  \ case the size some important constants are changed in the
  \ future, e.g. the size of the invaders or the containers.

: c@x+ ( n -- )
  case
    1 of postpone c@1+ endof
    2 of postpone c@2+ endof
    postpone c@ dup postpone xliteral postpone +
  endcase ; immediate compile-only
  \ Compile `c@1+`, `c@2+` or `c@ n +`, depending on the value
  \ of _n_.

: c@x- ( n -- )
  case
    1 of postpone c@1- endof
    2 of postpone c@2- endof
    postpone c@ dup postpone xliteral postpone -
  endcase ; immediate compile-only
  \ Compile `c@1-`, `c@2-` or `c@ n -`, depending on the value
  \ of _n_.

: x* ( n -- )
  case
    1 of endof
    2 of postpone 2* endof
    dup postpone xliteral postpone *
  endcase ; immediate compile-only
  \ Compile `2*` or `n *` or nothing, depending on the value of
  \ _n_.

: x+ ( n -- )
  case
    1 of postpone 1+ endof
    2 of postpone 2+ endof
    dup postpone xliteral postpone +
  endcase ; immediate compile-only
  \ Compile `1+`, `2+` or `n +`, depending on the value of _n_.

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

0 [if] \ XXX OLD

here ," PUNTUACIÓN"
here ," POENTARO"
here ," SCORE"
localized-string score$ ( -- ca len )
  \ Return string _ca len_ in the current language.

[then]

here ," Puntos:"
here ," Poentoj:"
here ," Score:"
localized-string score-label$ ( -- ca len )
  \ Return string _ca len_ in the current language.

0 [if] \ XXX OLD

here ," RÉCOR"
here ," RIKORDO"
here ," RECORD"
localized-string record$ ( -- ca len )
  \ Return string _ca len_ in the current language.

  \ XXX TODO -- Simplify: use `sconstants` instead, using the
  \ language as index and a wrapper word to provide it.

[then]

0 [if] \ XXX OLD

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
  \ Return name _ca len_ of country _n_ in the current
  \ language.

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

  \ Note: The attributes of the projectiles must be bright; the
  \ attributes used by the invaders, the walls and the
  \ containers must not be bright.

                         black cconstant sky-attr

                         green cconstant healthy-invader-attr
                        yellow cconstant wounded-invader-attr
                           red cconstant dying-invader-attr

                         white cconstant tank-attr

                   red brighty cconstant bullet-attr
                 white brighty cconstant missile-attr
                  blue brighty cconstant ball-attr

                  white papery cconstant unfocus-attr
  white papery brighty white + cconstant hide-report-attr
          white papery brighty cconstant reveal-report-attr
                         white cconstant text-attr

            white papery red + cconstant brick-attr
                         white cconstant door-attr
                           red cconstant broken-wall-attr
                        yellow cconstant container-attr
                yellow brighty cconstant radiation-attr

: init-colors ( -- ) [ white black papery + ] cliteral attr!
                     overprint-off inverse-off black border ;

  \ ===========================================================
  cr .( Global variables) ?depth debug-point \ {{{1

cvariable location          \ counter
 variable score             \ counter
 variable record record off \ max score

0 constant invader~ \ current invader's data structure address

variable catastrophe \ flag (game end condition)

: catastrophe? ( -- f ) catastrophe @ ;

  \ ===========================================================
  cr .( Keyboard) ?depth debug-point \ {{{1

13 cconstant enter-key

0 cconstant kk-left#  0. 2constant kk-left
0 cconstant kk-right# 0. 2constant kk-right
0 cconstant kk-fire#  0. 2constant kk-fire
0 cconstant kk-down#  0. 2constant kk-down
0 cconstant kk-up#    0. 2constant kk-up

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

5 cconstant /controls
  \ Bytes per item in the `controls` table.

create controls

  \ left    right     fire       down      up
  \ ---------------------------------------------------

  kk-5# c,  kk-8# c,  kk-en# c,  kk-6#  c,  kk-7# c,
    \ cursor+enter
  kk-r# c,  kk-t# c,  kk-en# c,  kk-m#  c,  kk-g# c,
    \ Spanish Dvorak
  kk-z# c,  kk-x# c,  kk-en# c,  kk-sp# c,  kk-p# c,
    \ QWERTY
  kk-5# c,  kk-8# c,  kk-0#  c,  kk-6#  c,  kk-7# c,
    \ cursor joystick
  kk-5# c,  kk-8# c,  kk-sp# c,  kk-6#  c,  kk-7# c,
    \ cursor+space
  kk-1# c,  kk-2# c,  kk-5#  c,  kk-3#  c,  kk-4# c,
    \ Sinclair 1
  kk-6# c,  kk-7# c,  kk-0#  c,  kk-8#  c,  kk-9# c,
    \ Sinclair 2
  kk-o# c,  kk-p# c,  kk-q#  c,  kk-a#  c,  kk-7# c,
    \ QWERTY
  kk-n# c,  kk-m# c,  kk-sp# c,  kk-a#  c,  kk-q# c,
    \ QWERTY
  kk-q# c,  kk-w# c,  kk-en# c,  kk-l#  c,  kk-p# c,
    \ QWERTY
  kk-z# c,  kk-x# c,  kk-sp# c,  kk-l#  c,  kk-p# c,
    \ QWERTY

here controls - /controls / cconstant max-controls
  \ Number of controls stored in `controls`.

max-controls 1- cconstant last-control

: >controls ( n -- a ) /controls * controls + ;
  \ Convert controls number _n_ to its address _a_.

: set-controls ( n -- )
  >controls     dup c@  dup c!> kk-left#   #>kk 2!> kk-left
             1+ dup c@  dup c!> kk-right#  #>kk 2!> kk-right
             1+ dup c@  dup c!> kk-fire#   #>kk 2!> kk-fire
             1+ dup c@  dup c!> kk-down#   #>kk 2!> kk-down
             1+     c@  dup c!> kk-up#     #>kk 2!> kk-up   ;
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

               235 cconstant last-udg \ last UDG code used
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
  cr .( Status bar, part 1) ?depth debug-point \ {{{1

                                 2 cconstant ammo-digits
                                 5 cconstant score-digits

                                 0 cconstant status-bar-y

                                 0 cconstant bullets-icon-x
                 bullets-icon-x 1+ cconstant bullets-x
        bullets-x ammo-digits + 1+ cconstant missiles-icon-x
                missiles-icon-x 1+ cconstant missiles-x
       missiles-x ammo-digits + 1+ cconstant balls-icon-x
                   balls-icon-x 1+ cconstant balls-x

            columns score-digits - cconstant record-x
                       record-x 1- cconstant record-separator-x
 record-separator-x score-digits - cconstant score-x

: [#] ( n -- ) 0 ?do postpone # loop ; immediate compile-only
  \ Compile `#` _n_ times.

: (.score ( n col row -- )
  at-xy s>d <# [ score-digits ] [#] #> text-attr attr! type ;
  \ Display score _n_ at coordinates _col row_.

' xdepth alias projectiles-left ( -- n )

0 cconstant ammo-x

: (.ammo ( n -- )
  projectiles-left s>d <# [ ammo-digits ] [#] #>
  ammo-x status-bar-y at-xy type ;
  \ Display the current ammo left at the status bar,
  \ withe the current attribute.

: .ammo ( n -- ) text-attr attr! (.ammo ;
  \ Display the current ammo left at the status bar.

0 cconstant  bullet-gun#
1 cconstant missile-gun#
2 cconstant    ball-gun#

defer set-gun ( n -- )
  \ Set the current arm:
  \ 0=bullet gun
  \ 1=missile gun
  \ 2=ball gun

: .bullets ( -- ) bullet-gun# set-gun (.ammo ;
  \ Display the number of bullets left.

: .missiles ( -- ) missile-gun# set-gun (.ammo ;
  \ Display the number of bullets left.

: .balls ( -- ) ball-gun# set-gun (.ammo ;
  \ Display the number of balls left.

: .score ( -- ) score @ score-x status-bar-y (.score ;
  \ Display the score.

: .record ( -- ) record @ record-x status-bar-y (.score ;
  \ Display the record.

: update-score ( n -- ) score +! .score ;

  \ ===========================================================
  cr .( Graphics) ?depth debug-point \ {{{1

[udg] [if]

    cvariable >udg  >udg coff \ next free UDG

cvariable latest-sprite-width
cvariable latest-sprite-height
cvariable latest-sprite-udg

: ?udg ( c -- ) last-udg > abort" Too many UDGs" ;
  \ Abort if UDG _c_ is too high.
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

[else]

' emit-udga alias .1x1sprite ( ca -- )

: .1x1sprites ( ca n -- ) 0 ?do dup emit-udga loop drop ;

: .2x1-udg-sprite ( ca -- ) dup emit-udga /udg+ emit-udga ;

: udg-sprite ( width height "ccc" -- ca )
  here -rot ,udg-block ;

[then]

2 cconstant udg/invader
2 cconstant udg/mothership

4 cconstant undocked-invader-sprite-frames
3 cconstant   docked-invader-sprite-frames

[udg] [if]

0 0 0 0 0 0 0 0 1 free-udg dup cconstant bl-udg udg!
  \ An empty UDG to be used as space.

[else]

rom-font bl /udg * constant bl-udg
  \ Address of the ROM font's space, to be used directly as
  \ an UDG.

[then]

[udg] [if] ' cconstant [else] ' constant [then] alias sprite-id

  \ -----------------------------------------------------------
  \ Invader species 0

  \ invader species 0, left flying, frame 0:

udg/invader 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXX..XX..XXX..
..XXXXXXXXXXXX..
.....XX..XX.....
....XX.XX.XX....
..XX........XX.. sprite-id left-flying-species-0-sprite

  \ invader species 0, left flying, frame 1:

udg/invader 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XX..XX..XXXX..
..XXXXXXXXXXXX..
....XXX..XXX....
...XX..XX..XX...
...XX.......XX.. drop

  \ invader species 0, left flying, frame 2:

udg/invader 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..X..XX..XXXXX..
..XXXXXXXXXXXX..
....XXX..XXX....
...XX..XX..XX...
....XX......XX.. drop

  \ invader species 0, left flying, frame 3:

udg/invader 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XX..XX..XXXX..
..XXXXXXXXXXXX..
....XXX..XXX....
...XX..XX..XX...
...XX.......XX.. drop

  \ invader species 0, right flying, frame 0:

udg/invader 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXX..XX..XXX..
..XXXXXXXXXXXX..
.....XX..XX.....
....XX.XX.XX....
..XX........XX.. sprite-id right-flying-species-0-sprite

  \ invader species 0, right flying, frame 1:

udg/invader 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXXX..XX..XX..
..XXXXXXXXXXXX..
....XXX..XXX....
...XX..XX..XX...
..XX.......XX... drop

  \ invader species 0, right flying, frame 2:

udg/invader 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXXXX..XX..X..
..XXXXXXXXXXXX..
....XXX..XXX....
...XX..XX..XX...
..XX......XX.... drop

  \ invader species 0, right flying, frame 3:

udg/invader 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXXX..XX..XX..
..XXXXXXXXXXXX..
....XXX..XXX....
...XX..XX..XX...
..XX.......XX... drop

  \ invader species 0, docked, frame 0:

udg/invader 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXX..XX..XXX..
..XXXXXXXXXXXX..
.....XX..XX.....
....XX.XX.XX....
..XX........XX.. sprite-id docked-species-0-sprite

  \ invader species 0, docked, frame 1:

udg/invader 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXX..XX..XXX..
..XXXXXXXXXXXX..
.....XX..XX.....
....XX.XX.XX....
..XX........XX.. drop

  \ invader species 0, docked, frame 2:

udg/invader 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXXXXXXXXXXX..
..XXXXXXXXXXXX..
.....XX..XX.....
....XX.XX.XX....
..XX........XX.. drop

  \ invader species 0, left breaking, frame 0:

udg/invader 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXX..XX..XXX..
..XXXXXXXXXXXX..
.....XX..XX.....
....XX.XX.XX....
..XX........XX.. sprite-id left-breaking-species-0-sprite

  \ invader species 0, left breaking, frame 1:

udg/invader 1 udg-sprite

.....XXXX.......
..XXXXXXXXXX....
.XXXXXXXXXXXX...
.XX..XX..XXXX...
.XXXXXXXXXXXX...
...XXX..XXX.....
..XX..XX..XX....
..XX.......XX... drop

  \ invader species 0, left breaking, frame 2:

udg/invader 1 udg-sprite

....XXXX........
.XXXXXXXXXX.....
XXXXXXXXXXXX....
X..XX..XXXXX....
XXXXXXXXXXXX....
..XXX..XXX......
.XX..XX..XX.....
..XX......XX.... drop

  \ invader species 0, left breaking, frame 3:

udg/invader 1 udg-sprite

.....XXXX.......
..XXXXXXXXXX....
.XXXXXXXXXXXX...
.XX..XX..XXXX...
.XXXXXXXXXXXX...
...XXX..XXX.....
..XX..XX..XX....
..XX.......XX... drop

  \ invader species 0, right breaking, frame 0:

udg/invader 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXX..XX..XXX..
..XXXXXXXXXXXX..
.....XX..XX.....
....XX.XX.XX....
..XX........XX.. sprite-id right-breaking-species-0-sprite

  \ invader species 0, right breaking, frame 1:

udg/invader 1 udg-sprite

.......XXXX.....
....XXXXXXXXXX..
...XXXXXXXXXXXX.
...XXXX..XX..XX.
...XXXXXXXXXXXX.
.....XXX..XXX...
....XX..XX..XX..
...XX.......XX.. drop

  \ invader species 0, right breaking, frame 2:

udg/invader 1 udg-sprite

........XXXX....
.....XXXXXXXXXX.
....XXXXXXXXXXXX
....XXXXX..XX..X
....XXXXXXXXXXXX
......XXX..XXX..
.....XX..XX..XX.
....XX......XX.. drop

  \ invader species 0, right breaking, frame 3:

udg/invader 1 udg-sprite

.......XXXX.....
....XXXXXXXXXX..
...XXXXXXXXXXXX.
...XXXX..XX..XX.
...XXXXXXXXXXXX.
.....XXX..XXX...
....XX..XX..XX..
...XX.......XX.. drop

  \ -----------------------------------------------------------
  \ Invader species 1

  \ invader species 1, left flying, frame 0:

udg/invader 1 udg-sprite

......X...X.....
.....X...X......
....XXXXXXX.....
...XX.XXX.XX....
..XXXXXXXXXXX...
..XXXXXXXXXXX...
..X.X.....X.X...
....X.....X..... sprite-id left-flying-species-1-sprite

  \ invader species 1, left flying, frame 1:

udg/invader 1 udg-sprite

......X...X.....
.....X...X......
....XXXXXXX.....
XXXX.XXX.XXXXXX.
..XXXXXXXXXXX...
...XXXXXXXXX....
....X.....X.....
.....X.....X.... drop

  \ invader species 1, left flying, frame 2:

udg/invader 1 udg-sprite

......X...X.....
..X..X...X..X...
..X.XXXXXXX.X...
..XXX.XXX.XXX...
..XXXXXXXXXXX...
...XXXXXXXXX....
....X.....X.....
....X.....X..... drop

  \ invader species 1, left flying, frame 3:

udg/invader 1 udg-sprite

......X...X.....
.....X...X......
....XXXXXXX.....
XXXX.XXX.XXXXXX.
..XXXXXXXXXXX...
...XXXXXXXXX....
....X.....X.....
.....X.....X.... drop

  \ invader species 1, right flying, frame 0:

udg/invader 1 udg-sprite

.....X...X......
......X...X.....
.....XXXXXXX....
....XX.XXX.XX...
...XXXXXXXXXXX..
...XXXXXXXXXXX..
...X.X.....X.X..
.....X.....X.... sprite-id right-flying-species-1-sprite

  \ invader species 1, right flying, frame 1:

udg/invader 1 udg-sprite

.....X...X......
......X...X.....
.....XXXXXXX....
.XXXXXX.XXX.XXXX
...XXXXXXXXXXX..
....XXXXXXXXX...
.....X.....X....
....X.....X..... drop

  \ invader species 1, right flying, frame 2:

udg/invader 1 udg-sprite

.....X...X......
...X..X...X..X..
...X.XXXXXXX.X..
...XXX.XXX.XXX..
...XXXXXXXXXXX..
....XXXXXXXXX...
.....X.....X....
.....X.....X.... drop

  \ invader species 1, right flying, frame 3:

udg/invader 1 udg-sprite

.....X...X......
......X...X.....
.....XXXXXXX....
.XXXXXX.XXX.XXXX
...XXXXXXXXXXX..
....XXXXXXXXX...
.....X.....X....
....X.....X..... drop

  \ invader species 1, docked, frame 0:

udg/invader 1 udg-sprite

....X.....X.....
.....X...X......
....XXXXXXX.....
...XX.XXX.XX....
..XXXXXXXXXXX...
..XXXXXXXXXXX...
..X.X.....X.X...
.....XX.XX...... sprite-id docked-species-1-sprite

  \ invader species 1, docked, frame 1:

udg/invader 1 udg-sprite

....X.....X.....
.....X...X......
....XXXXXXX.....
...XX.XXX.XX....
..XXXXXXXXXXX...
..XXXXXXXXXXX...
..X.X.....X.X...
.....XX.XX...... drop

  \ invader species 1, docked, frame 2:

udg/invader 1 udg-sprite

....X.....X.....
.....X...X......
....XXXXXXX.....
...XXXXXXXXX....
..XXXXXXXXXXX...
..XXXXXXXXXXX...
..X.X.....X.X...
.....XX.XX...... drop

  \ invader species 1, left breaking, frame 0:

udg/invader 1 udg-sprite

.....X...X......
.....X...X......
....XXXXXXX.....
...XX.XXX.XX....
..XXXXXXXXXXX...
..XXXXXXXXXXX...
..X.X.....X.X...
....X.....X..... sprite-id left-breaking-species-1-sprite

  \ invader species 1, left breaking, frame 1:

udg/invader 1 udg-sprite

.....X...X......
....X...X.......
...XXXXXXX......
..X.XXX.XXXXXX..
.XXXXXXXXXXX....
.XXXXXXXXXX.....
..XX.....X......
....X.....X..... drop

  \ invader species 1, left breaking, frame 2:

udg/invader 1 udg-sprite

....X...X.......
...X...X..X.....
..XXXXXXX.X.....
.XX.XXX.XXX.....
XXXXXXXXXXX.....
XXXXXXXXXX......
.XX.....X.......
..X.....X....... drop

  \ invader species 1, left breaking, frame 3:

udg/invader 1 udg-sprite

.....X...X......
....X...X.......
...XXXXXXX......
..X.XXX.XXXXXX..
.XXXXXXXXXXX....
.XXXXXXXXXX.....
..XX.....X......
....X.....X..... drop

  \ invader species 1, right breaking, frame 0:

udg/invader 1 udg-sprite

......X...X.....
......X...X.....
.....XXXXXXX....
....XX.XXX.XX...
...XXXXXXXXXXX..
...XXXXXXXXXXX..
...X.X.....X.X..
.....X.....X.... sprite-id right-breaking-species-1-sprite

  \ invader species 1, right breaking, frame 1:

udg/invader 1 udg-sprite

......X...X.....
.......X...X....
......XXXXXXX...
..XXXXXX.XXX.X..
....XXXXXXXXXXX.
.....XXXXXXXXXX.
......X.....XX..
.....X.....X.... drop

  \ invader species 1, right breaking, frame 2:

udg/invader 1 udg-sprite

.......X...X....
.....X..X...X...
.....X.XXXXXXX..
.....XXX.XXX.XX.
.....XXXXXXXXXXX
......XXXXXXXXXX
.......X.....XX.
.......X.....X.. drop

  \ invader species 1, right breaking, frame 3:

udg/invader 1 udg-sprite

......X...X.....
.......X...X....
......XXXXXXX...
..XXXXXX.XXX.X..
....XXXXXXXXXXX.
.....XXXXXXXXXX.
......X.....XX..
.....X.....X.... drop

  \ -----------------------------------------------------------
  \ Invader species 2

  \ invader species 2, left flying, frame 0:

udg/invader 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....XX.XX.XX....
....XXXXXXXX....
......X..X......
.....X.XX.X.....
....X.X..X.X.... sprite-id left-flying-species-2-sprite

  \ invader species 2, left flying, frame 1:

udg/invader 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....X.XX.XXX....
....XXXXXXXX....
......X..X......
......X.XX.X....
.....X.X..X.X... drop

  \ invader species 2, left flying, frame 2:

udg/invader 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....XX.XX.XX....
....XXXXXXXX....
......X..X......
.......X.XX.X...
......X.X..X.X.. drop

  \ invader species 2, left flying, frame 3:

udg/invader 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....X.XX.XXX....
....XXXXXXXX....
......X..X......
......X.XX.X....
.....X.X..X.X... drop

  \ invader species 2, right flying, frame 0:

udg/invader 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....XX.XX.XX....
....XXXXXXXX....
......X..X......
.....X.XX.X.....
....X.X..X.X.... sprite-id right-flying-species-2-sprite

  \ invader species 2, right flying, frame 1:

udg/invader 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....XXX.XX.X....
....XXXXXXXX....
......X..X......
....X.XX.X......
...X.X..X.X..... drop

  \ invader species 2, right flying, frame 2:

udg/invader 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....XX.XX.XX....
....XXXXXXXX....
......X..X......
...X.XX.X.......
..X.X..X.X...... drop

  \ invader species 2, right flying, frame 3:

udg/invader 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....XXX.XX.X....
....XXXXXXXX....
......X..X......
....X.XX.X......
...X.X..X.X..... drop

  \ invader species 2, docked, frame 0:

udg/invader 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....XX.XX.XX....
....XXXXXXXX....
......X..X......
.....X.XX.X.....
....X.X..X.X.... sprite-id docked-species-2-sprite

  \ invader species 2, docked, frame 1:

udg/invader 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....XX.XX.XX....
....XXXXXXXX....
......X..X......
.....X.XX.X.....
....X.X..X.X.... drop

  \ invader species 2, docked, frame 2:

udg/invader 1 udg-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....XXXXXXXX....
....XXXXXXXX....
......X..X......
.....X.XX.X.....
....X.X..X.X.... drop

  \ invader species 2, left breaking, frame 0:

udg/invader 1 udg-sprite

.....XX.........
....XXXX........
...XXXXXX.......
..XX.XX.XX......
..XXXXXXXX......
....X..X........
...X.XX.X.......
..X.X..X.X...... sprite-id left-breaking-species-2-sprite

  \ invader species 2, left breaking, frame 1:

udg/invader 1 udg-sprite

....XX..........
...XXXX.........
..XXXXXX........
.X.XX.XXX.......
.XXXXXXXX.......
...X..X.........
...X.XX.X.......
..X.X..X.X...... drop

  \ invader species 2, left breaking, frame 2:

udg/invader 1 udg-sprite

...XX...........
..XXXX..........
.XXXXXX.........
XX.XX.XX........
XXXXXXXX........
..X..X..........
...X.XX.X.......
..X.X..X.X...... drop

  \ invader species 2, left breaking, frame 3:

udg/invader 1 udg-sprite

....XX..........
...XXXX.........
..XXXXXX........
.X.XX.XXX.......
.XXXXXXXX.......
...X..X.........
...X.XX.X.......
..X.X..X.X...... drop

  \ invader species 2, right breaking, frame 0:

udg/invader 1 udg-sprite

.........XX.....
........XXXX....
.......XXXXXX...
......XX.XX.XX..
......XXXXXXXX..
........X..X....
.......X.XX.X...
......X.X..X.X.. sprite-id right-breaking-species-2-sprite

  \ invader species 2, right breaking, frame 1:

udg/invader 1 udg-sprite

..........XX....
.........XXXX...
........XXXXXX..
.......XXX.XX.X.
.......XXXXXXXX.
.........X..X...
.......X.XX.X...
......X.X..X.X.. drop

  \ invader species 2, right breaking, frame 2:

udg/invader 1 udg-sprite

...........XX...
..........XXXX..
.........XXXXXX.
........XX.XX.XX
........XXXXXXXX
..........X..X..
.......X.XX.X...
......X.X..X.X.. drop

  \ invader species 2, right breaking, frame 3:

udg/invader 1 udg-sprite

..........XX....
.........XXXX...
........XXXXXX..
.......XXX.XX.X.
.......XXXXXXXX.
.........X..X...
.......X.XX.X...
......X.X..X.X.. drop

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

  \ mothership, frame 0:

udg/mothership 1 udg-sprite

................
.....XXXXXX.....
...XXXXXXXXXX...
..XXXXXXXXXXXX..
.XX..XX..XX..XX.
XXXXXXXXXXXXXXXX
..XXX..XX..XXX..
...X........X... sprite-id flying-mothership-sprite

  \ mothership, frame 1:

udg/mothership 1 udg-sprite

................
.....XXXXXX.....
...XXXXXXXXXX...
..XXXXXXXXXXXX..
.X..XX..XX..XX..
XXXXXXXXXXXXXXXX
..XXX..XX..XXX..
...X........X... drop

  \ mothership, frame 2:

udg/mothership 1 udg-sprite

................
.....XXXXXX.....
...XXXXXXXXXX...
..XXXXXXXXXXXX..
...XX..XX..XX...
XXXXXXXXXXXXXXXX
..XXX..XX..XXX..
...X........X... drop

  \ mothership, frame 3:

udg/mothership 1 udg-sprite

................
.....XXXXXX.....
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XX..XX..XX..X.
XXXXXXXXXXXXXXXX
..XXX..XX..XXX..
...X........X... drop

flying-mothership-sprite [udg]
[if]   >udg c@ swap -
[else] here swap /udg /
[then] udg/mothership / cconstant flying-mothership-frames

  \ ............................................
  \ Beaming mothership

udg/mothership 1 udg-sprite

................
.....XXXXXX.....
...XXXXXXXXXX...
..XXXXXXXXXXXX..
.XX..XX..XX..XX.
XXXXXXXXXXXXXXXX
.X.X.X.XX.X.X.X.
X.X.X.X..X.X.X.X sprite-id beaming-mothership-sprite

udg/mothership 1 udg-sprite

................
.....XXXXXX.....
...XXXXXXXXXX...
..XXXXXXXXXXXX..
.XX..XX..XX..XX.
XXXXXXXXXXXXXXXX
X.X.X.X..X.X.X.X
.X.X.X.XX.X.X.X. drop

beaming-mothership-sprite [udg]
[if]   >udg c@ swap -
[else] here swap /udg /
[then] udg/mothership / cconstant beaming-mothership-frames

  \ -----------------------------------------------------------
  \ Explosion

2 cconstant udg/explosion

udg/explosion 1 udg-sprite

..............X.
..X......XX..X..
.X...XXXXXXX....
....XXXXXXXXX.X.
...XXXX.XX.XX..X
.X..XX..XXXX....
X....XXXXX...X..
..X...XX...X..X. sprite-id explosion-sprite

udg/explosion 1 udg-sprite

................
...X...X...X....
.....XXXXXXX..X.
.X..X.XXXX.XX...
...XX.XXX.XXX...
..X.X.XXXX.X..X.
.X...XX.XX..X...
.X...XX...X..X.X drop

udg/explosion 1 udg-sprite

.X...X..........
X...X....X...X..
...X.XXXX.XX....
...XXXX.XX.XX..X
X...X.XXX.X.XX..
....X.XX.X.XX..X
...X.XX.XXX.XX..
X...XX.X..X..... drop

udg/explosion 1 udg-sprite

X......X........
....X....X......
...X.X.XX.X.....
X..X.X...X.XX..X
....X.X.X.X.XX..
..X...X..X.XX...
...X.XX.X.X.XX..
.X..X..........X drop

udg/explosion 1 udg-sprite

..X.............
....X....X..X...
X..X.X.X..X..X..
...X..X..X......
....X.....X.X...
.X....X..X.X..X.
...X.X....X.....
....X....X...X.. drop

udg/explosion 1 udg-sprite

X............X..
....X....X......
.......X..X..X..
..X...........X.
..........X.....
.X....X.......X.
..........X.....
..X.......X....X drop

udg/explosion 1 udg-sprite

.X.........X....
.....X.....X..X.
...............X
................
X...............
...........X....
X.....X........X
..X........X.... drop

udg/explosion 1 udg-sprite

....X.......X..X
................
................
................
................
...........X....
.............X..
......X......... drop

udg/explosion 1 udg-sprite

................
................
................
................
................
................
................
................ drop

explosion-sprite [udg]
[if]   >udg c@ swap -
[else] here swap /udg /
[then] udg/explosion / cconstant explosion-frames

  \ --------------------------------------------
  \ Projectiles

  \ ............................
  \ Bullet

1 1 udg-sprite

..X.....
.....X..
..X.....
.....X..
..X.....
.....X..
..X.....
.....X.. sprite-id bullet-sprite

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

bullet-sprite [udg] [if]   >udg c@ swap -
                    [else] here swap /udg /
                    [then] cconstant bullet-frames

  \ ............................
  \ Missile

1 1 udg-sprite

...XX...
...XX...
...XX...
..XXXX..
..XXXX..
..XXXX..
...X.X..
..X.X... sprite-id missile-sprite

1 1 udg-sprite

...XX...
...XX...
...XX...
..XXXX..
..XXXX..
..XXXX..
..X.X...
...X.X.. drop

missile-sprite [udg] [if]   >udg c@ swap -
                     [else] here swap /udg /
                     [then] cconstant missile-frames

  \ ............................
  \ Ball

1 1 udg-sprite

..XXXX..
.XXXXXX.
XXXXX.XX
XXXXXX.X
XXXXXXXX
XXXXXXXX
.XXXXXX.
..XXXX.. sprite-id ball-sprite

1 1 udg-sprite

..XXXX..
.XXXXXX.
XXXXXXXX
XXXXXXXX
XXXXXX.X
XXXXX.XX
.XXXXXX.
..XXXX.. drop

1 1 udg-sprite

..XXXX..
.XXXXXX.
XXXXXXXX
XXXXXXXX
XXXXXXXX
XXX..XXX
.XXXXXX.
..XXXX.. drop

1 1 udg-sprite

..XXXX..
.XXXXXX.
XXXXXXXX
X.XXXXXX
XX.XXXXX
XXXXXXXX
.XXXXXX.
..XXXX.. drop

1 1 udg-sprite

..XXXX..
.XX.XXX.
XX.XXXXX
XXXXXXXX
XXXXXXXX
XXXXXXXX
.XXXXXX.
..XXXX.. drop

ball-sprite [udg] [if]   >udg c@ swap -
                  [else] here swap /udg /
                  [then] cconstant ball-frames

1 1 udg-sprite

XXXX.....
XXXXX....
XXX.XX...
XXXX.X...
XXXXXX...
XXXXXX...
XXXXX....
XXXX..... sprite-id right-wall-ball-sprite

1 1 udg-sprite

XXXX.....
XXXXX....
XXXXXX...
XXXXXX...
XXXX.X...
XXX.XX...
XXXXX....
XXXX..... drop

1 1 udg-sprite

XXXX.....
XXXXX....
XXXXXX...
XXXXXX...
XXXXXX...
X..XXX...
XXXXX....
XXXX..... drop

1 1 udg-sprite

XXXX.....
XXXXX....
XXXXXX...
XXXXXX...
.XXXXX...
XXXXXX...
XXXXX....
XXXX..... drop

1 1 udg-sprite

XXXX.....
X.XXX....
.XXXXX...
XXXXXX...
XXXXXX...
XXXXXX...
XXXXX....
XXXX..... drop

right-wall-ball-sprite
[udg] [if]   >udg c@ swap -
      [else] here swap /udg /
      [then] cconstant right-wall-ball-frames

1 1 udg-sprite

....XXXX
...XXXXX
..XXXXX.
..XXXXXX
..XXXXXX
..XXXXXX
...XXXXX
....XXXX sprite-id left-wall-ball-sprite

1 1 udg-sprite

....XXXX
...XXXXX
..XXXXXX
..XXXXXX
..XXXXXX
..XXXXX.
...XXXXX
....XXXX drop

1 1 udg-sprite

....XXXX
...XXXXX
..XXXXXX
..XXXXXX
..XXXXXX
..XXX..X
...XXXXX
....XXXX drop

1 1 udg-sprite

....XXXX
...XXXXX
..XXXXXX
..X.XXXX
..XX.XXX
..XXXXXX
...XXXXX
....XXXX drop

1 1 udg-sprite

....XXXX
...XX.XX
..XX.XXX
..XXXXXX
..XXXXXX
..XXXXXX
...XXXXX
....XXXX drop

left-wall-ball-sprite
[udg] [if]   >udg c@ swap -
      [else] here swap /udg /
      [then] cconstant left-wall-ball-frames

  \ ............................
  \ Explosion

  \ XXX TMP --

1 1 udg-sprite

........
........
..X.X...
...X.X..
..X.X...
...X.X..
........
........ sprite-id projectile-explosion-sprite

1 1 udg-sprite

........
..X.X...
.....X..
..X...X.
.X.X.X..
..X...X.
.X...X..
........ drop

1 1 udg-sprite

.X...X..
....X...
.....X..
X.X...X.
...X.X..
..X...X.
X....X..
..X...X. drop

1 1 udg-sprite

........
.X..X...
.....X..
X..X....
..X...X.
.X..X...
..X..X..
....X... drop

1 1 udg-sprite

........
....X...
..X..X..
...X..X.
..X..X..
...X..X.
..X..X..
........ drop

1 1 udg-sprite

........
........
........
...X.X..
..X..X..
...XX...
........
........ drop

1 1 udg-sprite

........
........
........
........
........
........
........
........ drop

projectile-explosion-sprite
[udg] [if]   >udg c@ swap -
      [else] here swap /udg /
      [then] cconstant projectile-explosion-frames

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
........ sprite-id brick

1 1 udg-sprite

XXXXXXXX
.XXXXXXX
..XXXXXX
..XXXXXX
..XXXXXX
..XXXXXX
..XXXXXX
..XXXXXX sprite-id left-door

1 1 udg-sprite

XXXXXXXX
XXXXXXX.
XXXXXX..
XXXXXX..
XXXXXX..
XXXXXX..
XXXXXX..
XXXXXX.. sprite-id right-door

1 1 udg-sprite

XXXXXXXX
.XXXXXXX
.X.XXXXX
...XX.XX
......XX
.....XXX
......X.
........ sprite-id broken-top-left-brick

1 1 udg-sprite

........
........
.....XXX
......XX
...XX.XX
.XXXXXXX
XXXXXXXX
XXXXXXXX sprite-id broken-bottom-left-brick

1 1 udg-sprite

XXXXXXXX
XXXXXXXX
XXXXX...
XXXXXX..
XXX..X..
X.......
........
........ sprite-id broken-top-right-brick

1 1 udg-sprite

........
X.......
X.X.....
XXX..X..
XXXXXX..
XXXXX...
XXXXXX.X
XXXXXXXX sprite-id broken-bottom-right-brick

  \ -----------------------------------------------------------
  \ Tank

4 cconstant tank-frames

tank-frames 1- cconstant tank-max-frame

0 cconstant tank-sprite
  \ A configurable constant. The current sprite of the tank.

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
....X.X.X.X.X.X.X.X.X... sprite-id bullet-gun-tank-sprite

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

udg/tank 1 udg-sprite
..........XXXX..........
...XXXXXX.XXXX.XXXXXXX..
..XXXXXXX.XXXX.XXXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XX.X.X.X.X.X.X.X.X.X.XX
..XX..XX..XX..XX..XX.XX.
...X.XXX.XXX.XXX.XXX.X..
....X.X.X.X.X.X.X.X.X... sprite-id missile-gun-tank-sprite

udg/tank 1 udg-sprite
..........XXXX..........
...XXXXXX.XXXX.XXXXXXX..
..XXXXXXX.XXXX.XXXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XXX.X.X.X.X.X.X.X.X.XXX
..XX.XX..XX..XX..XX..XX.
...X.XXX.XXX.XXX.XXX.X..
...X.X.X.X.X.X.X.X.X.X.. drop

udg/tank 1 udg-sprite
..........XXXX..........
...XXXXXX.XXXX.XXXXXXX..
..XXXXXXX.XXXX.XXXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XX.X.X.X.X.X.X.X.X.X.XX
..XX.XXX.XXX.XXX.XXX.XX.
...X.XX..XX..XX..XX..X..
....X.X.X.X.X.X.X.X.X... drop

udg/tank 1 udg-sprite
..........XXXX..........
...XXXXXX.XXXX.XXXXXXX..
..XXXXXXX.XXXX.XXXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XXX.X.X.X.X.X.X.X.X.XXX
..XX.XXX.XXX.XXX.XXX.XX.
...X..XX..XX..XX..XX.X..
...X.X.X.X.X.X.X.X.X.X.. drop

udg/tank 1 udg-sprite
........XXXXXXXX........
...XXXX..XXXXXX..XXXXX..
..XXXXXX..XXXX..XXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XX.X.X.X.X.X.X.X.X.X.XX
..XX..XX..XX..XX..XX.XX.
...X.XXX.XXX.XXX.XXX.X..
....X.X.X.X.X.X.X.X.X... sprite-id ball-gun-tank-sprite

udg/tank 1 udg-sprite
........XXXXXXXX........
...XXXX..XXXXXX..XXXXX..
..XXXXXX..XXXX..XXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XXX.X.X.X.X.X.X.X.X.XXX
..XX.XX..XX..XX..XX..XX.
...X.XXX.XXX.XXX.XXX.X..
...X.X.X.X.X.X.X.X.X.X.. drop

udg/tank 1 udg-sprite
........XXXXXXXX........
...XXXX..XXXXXX..XXXXX..
..XXXXXX..XXXX..XXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XX.X.X.X.X.X.X.X.X.X.XX
..XX.XXX.XXX.XXX.XXX.XX.
...X.XX..XX..XX..XX..X..
....X.X.X.X.X.X.X.X.X... drop

udg/tank 1 udg-sprite
........XXXXXXXX........
...XXXX..XXXXXX..XXXXX..
..XXXXXX..XXXX..XXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XXX.X.X.X.X.X.X.X.X.XXX
..XX.XXX.XXX.XXX.XXX.XX.
...X..XX..XX..XX..XX.X..
...X.X.X.X.X.X.X.X.X.X.. drop

  \ -----------------------------------------------------------
  \ Containers

2 cconstant udg/container

udg/container 1 udg-sprite

......XXXXX.....
...XXX.....XXX..
..X...XXXXX...X.
..X...........X.
..X....XXX....X.
..X...XXXXX...X.
..X....XXX....X.
..X.....X.....X. sprite-id container-top

udg/container 2/ 1 udg-sprite

........
...XXX..
..X...X.
..X...X.
..X...X.
..X....X
..X....X
..X....X sprite-id broken-top-left-container

udg/container 2/ 1 udg-sprite

........
...XXX..
..X...X.
..X...X.
.X....X.
.X....X.
.X....X.
X.....X. sprite-id broken-top-right-container

udg/container 1 udg-sprite

..X..X.X.X.X..X.
..X.XXXX.XXXX.X.
..X.XXX...XXX.X.
..X..XX...XX..X.
..X...........X.
...XXX.....XXX..
......XXXXX.....
................ sprite-id container-bottom

udg/container 2/ 1 udg-sprite

.......X
.....XXX
...XXXX.
..X..XX.
..X.....
...XXX..
......XX
........ sprite-id broken-bottom-left-container

udg/container 2/ 1 udg-sprite

XX......
.XXX....
..XXXX..
..XX..X.
......X.
...XXX..
XXX.....
........ sprite-id broken-bottom-right-container

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
............X... sprite-id right-arrow

2 1 udg-sprite

................
...X............
..XX............
.XXXXXXXXXXX....
XXXXXXXXXXXX....
.XXXXXXXXXXX....
..XX............
...X............ sprite-id left-arrow

2 1 udg-sprite

....XXXXXXXX....
..XX........XX..
..XX........XX..
..X.XXXXXXXX.X..
..X..........X..
..X..........X..
..X..........X..
XXXXXXXXXXXXXXXX sprite-id fire-button

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
  \ Characters occupied by the status bar.

  \ ===========================================================
  cr .( Invaders data) ?depth debug-point \ {{{1

  \ --------------------------------------------
  \ Invader species

3 cconstant #species

0
  cfield: ~flying-left-sprite           \ UDG
  cfield: ~flying-left-sprite-frames    \ count
  cfield: ~flying-right-sprite          \ UDG
  cfield: ~flying-right-sprite-frames   \ count
  cfield: ~breaking-left-sprite         \ UDG
  cfield: ~breaking-left-sprite-frames  \ count
  cfield: ~breaking-right-sprite        \ UDG
  cfield: ~breaking-right-sprite-frames \ count
  cfield: ~docked-sprite                \ UDG
  cfield: ~docked-sprite-frames         \ count
  cfield: ~species-endurance
cconstant /species
  \ Data structure of an invader species.

create species #species /species * allot
  \ Invaders species data table.

: species#>~ ( n -- a ) /species * species + ;

: set-species ( c1 c2 c3 c4 c5 -- )
  species#>~ >r
  r@ ~species-endurance c!
  r@ ~flying-right-sprite c!
  undocked-invader-sprite-frames
  r@ ~flying-right-sprite-frames c!
  r@ ~flying-left-sprite c!
  undocked-invader-sprite-frames
  r@ ~flying-left-sprite-frames c!
  r@ ~breaking-right-sprite c!
  undocked-invader-sprite-frames
  r@ ~breaking-right-sprite-frames c!
  r@ ~breaking-left-sprite c!
  undocked-invader-sprite-frames
  r@ ~breaking-left-sprite-frames c!
  r@ ~docked-sprite c!
  docked-invader-sprite-frames
  r> ~docked-sprite-frames c! ;
  \ Init the data of invaders species _c5_:
  \   c1 = docked sprite
  \   c2 = left flying sprite
  \   c3 = right flying sprite
  \   c4 = endurance

1 cconstant invader-0-endurance
2 cconstant invader-1-endurance
4 cconstant invader-2-endurance

invader-0-endurance
invader-1-endurance max
invader-2-endurance max cconstant max-endurance

docked-species-0-sprite
left-breaking-species-0-sprite
right-breaking-species-0-sprite
left-flying-species-0-sprite
right-flying-species-0-sprite
invader-0-endurance
0 set-species

docked-species-1-sprite
left-breaking-species-1-sprite
right-breaking-species-1-sprite
left-flying-species-1-sprite
right-flying-species-1-sprite
invader-1-endurance
1 set-species

docked-species-2-sprite
left-breaking-species-2-sprite
right-breaking-species-2-sprite
left-flying-species-2-sprite
right-flying-species-2-sprite
invader-2-endurance
2 set-species

  \ --------------------------------------------

                    0 cconstant invaders-min-x
columns udg/invader - cconstant invaders-max-x

             10 cconstant max-invaders
max-invaders 2/ cconstant half-max-invaders
              0 cconstant first-invader#
max-invaders 1- cconstant last-invader#

0

  \ XXX TODO -- reorder for speed: place the most used fields
  \ at cell offsets +0, +1, +2, +4

  cfield: ~y              \ row
  cfield: ~x              \ column
  cfield: ~sprite         \ UDG
  cfield: ~frames         \ count
  cfield: ~frame          \ counter
  cfield: ~initial-x      \ column
  field:  ~x-inc          \ -1|1
  field:  ~initial-x-inc  \ -1|1
  cfield: ~stamina        \ 0..3
  cfield: ~attr           \ color attribute
  field:  ~action         \ execution token
  field:  ~species        \ data structure address
  field:  ~explosion-time \ ticks clock time
  cfield: ~layer          \ 0 (bottom) .. 4 (top)
  cfield: ~endurance      \ 1..max-endurance
cconstant /invader
  \ Data structure of an species.

max-invaders /invader * constant /invaders

create invaders-data /invaders allot
  \ Invaders data table.

: invader#>~ ( n -- a ) /invader * invaders-data + ;
  \ Convert invader number _n_ to its data address _a_.

first-invader# invader#>~ constant first-invader~
 last-invader# invader#>~ constant last-invader~

: set-invader ( n -- ) invader#>~ !> invader~ ;
  \ Set invader _n_ as the current invader.

max-invaders 2/ 1- cconstant invader-top-layer
  \ The number of the highest invader "layer". The pair
  \ of invaders that fly nearest the ground are layer 0.
  \ The pair above them are layer 1, and so on.

invader-top-layer 1+ cconstant invader-layers
  \ Number of invader layers.

2 cconstant rows/layer

building-top-y 1+ cconstant invader-min-y

invader-min-y invader-top-layer rows/layer * +
cconstant invader-max-y

: layer>y ( n -- row )
  invader-top-layer swap - rows/layer * invader-min-y + ;
  \ Convert invader layer _n_ to its equivalent row _row_. The
  \ pair of invaders that fly nearest the ground are layer 0.
  \ The pair above them are layer 1, and so on.

: y>layer ( row -- n ) invader-max-y - rows/layer / abs ;
  \ Convert invader row _row_ to its equilavent layer _n_. The
  \ pair of invaders that fly nearest the ground are layer 0.
  \ The pair above them are layer 1, and so on. Note: _row_ is
  \ supposed to be a valid row of an invader layer, otherwise
  \ the result will be wrong.

: invader-retreat-bonus ( -- n ) invader~ ~layer c@ 1+ ;
  \ Bonus points for making the invader retreat.

: invader-destroy-bonus ( -- n ) invader-retreat-bonus 8* ;
  \ Bonus points for destroying the invader.

: attacking? ( -- f )
  invader~ ~initial-x-inc @ invader~ ~x-inc @ = ;
  \ Is the current invader attacking?

: .y/n ( f -- ) if ." Y" else ." N" then space ;
  \ XXX TMP -- for debugging

1 cconstant min-stamina

3 cconstant max-stamina

: set-invader-sprite ( c n -- )
  invader~ ~frames c! invader~ ~sprite c!
  invader~ ~frame coff ;
  \ Set character _c_ as the first character of the first
  \ sprite of the current invader, and _n_ as the number of
  \ frames.

: flying-to-the-left? ( -- f ) invader~ ~x-inc @ 0< ;
  \ Is the current invader flying to the left?

: set-<flying-invader-sprite ( -- )
  invader~ ~species @ dup
  ~flying-left-sprite c@ swap
  ~flying-left-sprite-frames c@ set-invader-sprite ;
  \ Set the flying-to-the-left sprite of the current invader.
  \
  \ XXX TODO -- Use double-cell fields to copy both fields with
  \ one operation.
  \
  \ XXX TODO -- If the maximum frames in both directions are
  \ identical, there's no need to initiate `~frame`.

: set-flying>-invader-sprite ( -- )
  invader~ ~species @ dup
  ~flying-right-sprite c@ swap
  ~flying-right-sprite-frames c@ set-invader-sprite ;
  \ Set the flying-to-the-right sprite of the current invader.
  \
  \ XXX TODO -- Use double-cell fields to copy both fields with
  \ one operation.
  \
  \ XXX TODO -- If the maximum frames in both directions are
  \ identical, there's no need to initiate `~frame`.

: set-flying-invader-sprite ( -- )
  flying-to-the-left?  if   set-<flying-invader-sprite
                       else set-flying>-invader-sprite then ;
  \ Set the flying sprite of the current invader.
  \
  \ XXX TODO -- Combine with `set-invader-direction`.

: set-docked-invader-sprite ( -- )
  invader~ ~species @ dup ~docked-sprite c@
                      swap ~docked-sprite-frames c@
  set-invader-sprite ;
  \ Make the current invader use the docked invader sprite.

: init-invader ( c1 c2 c3 c4 c0 -- )
  set-invader
  invader~ ~stamina coff
  invader~ ~action off
  species#>~ dup invader~ ~species !
                 ~species-endurance c@ invader~ ~endurance c!
  invader~ ~initial-x-inc ! invader~ ~x-inc off
  dup invader~ ~initial-x c!
      invader~ ~x c!
  dup invader~ ~y c!
      y>layer invader~ ~layer c!
  set-docked-invader-sprite ;
  \ Init invader_c0_ with the given data:
  \   c1 = row
  \   c2 = column = initial column
  \   c3 = initial column inc
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

defer docked-invader-action ( -- )
  \ Action of the invaders that are docked.

max-stamina cconstant mothership-stamina

create stamina-attributes ( -- ca )   dying-invader-attr c,
                                    wounded-invader-attr c,
                                    healthy-invader-attr c,
  \ Table to index the stamina (1..3) to its proper attribute.

: stamina>attr ( n -- c )
  [ stamina-attributes 1- ] literal + c@ ;
  \ Convert stamina _n_ to its corresponding attribute _c_.

: invader-stamina! ( n -- )
  dup invader~ ~stamina c!  stamina>attr invader~ ~attr c! ;
  \ Make _n_ the stamina of the current invader and change its
  \ attribute accordingly.

: create-invaders ( n1 n2 -- )
  ?do i set-invader
      mothership-stamina invader-stamina!
      ['] docked-invader-action invader~ ~action !
  loop ;
  \ Create new docked invaders from _n2_ to _n1-1_.  The data
  \ of those invaders must be set already to their default
  \ values.

: create-left-squadron ( -- )
  init-left-invaders-data
  half-max-invaders 0 create-invaders ;

: create-right-squadron ( -- )
  init-right-invaders-data
  max-invaders half-max-invaders create-invaders ;

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
  \ Return number _n_ of alive invaders that are at the right
  \ of the building.

  \ ===========================================================
  cr .( Building) ?depth debug-point \ {{{1

cvariable old-breaches
  \ Number of breaches in the wall, at the start of the current
  \ attack.

cvariable breaches
  \ Number of new breaches in the wall, during the current
  \ attack.

cvariable battle-breaches
  \ Total number of breaches in the wall, during the current
  \ battle.

: check-breaches ( -- ) breaches c@ old-breaches c! ;
  \ Remember the current number of breaches.

: no-breach ( -- )
  old-breaches coff breaches coff battle-breaches coff ;
  \ Reset the number of breaches.

: breaches? ( -- f ) breaches c@ 0<> ;
  \ Has the building any breach caused during an attack?

: new-breach? ( -- f ) breaches c@ old-breaches c@ > ;
  \ Has got the building any new breach during the current
  \ attack?

building-top-y 11 + cconstant building-bottom-y

0 cconstant building-width
0 cconstant building-left-x
0 cconstant building-right-x

  \ 0 cconstant containers-left-x
  \ 0 cconstant containers-right-x
  \ XXX OLD -- Not used.

: size-building ( -- )
  [ columns 2/ 1- ] cliteral  \ half of the screen
  location c@1+               \ half width of all containers
  dup 2* 2+ c!> building-width
  \ 2dup 1- - c!> containers-left-x \ XXX OLD
  2dup    - c!> building-left-x
  \ 2dup    + c!> containers-right-x \ XXX OLD
       1+ + c!> building-right-x ;
  \ Set the size of the building after the current location.

: floor ( row -- )
  building-left-x swap at-xy
  brick-attr attr! brick building-width .1x1sprites ;
  \ Draw a floor of the building at row _row_.

: ground-floor ( row -- )
  building-left-x 1+ swap at-xy
  door-attr attr!  left-door emit-udg
  brick-attr attr! brick building-width 4 - .1x1sprites
  door-attr attr!  right-door emit-udg ;
  \ Draw the ground floor of the building at row _row_.

: building-top ( -- ) building-top-y floor ;
  \ Draw the top of the building.

: containers-bottom ( n -- )
  container-attr attr!
  0 ?do container-bottom .2x1-udg-sprite loop ;
  \ Draw a row of _n_ bottom parts of containers.

: containers-top ( n -- )
  container-attr attr!
  0 ?do container-top .2x1-udg-sprite loop ;
  \ Draw a row of _n_ top parts of containers.

: .brick ( -- ) brick-attr attr! brick .1x1sprite ;
  \ Draw a brick.

create containers-half
  ' containers-top ' containers-bottom
    \ execution vectors to display the containers
  building-top-y even? ?\ swap
    \ change their order, depending on the building position
  , ,

: yard ( row -- )                        0 over at-xy .brick
                  [ last-column ] cliteral swap at-xy .brick ;
  \ Draw the yard limits.

variable repaired
  \ Flag: has the building been repaired at the start of the
  \ current attack?

: building ( -- )
  building-top
  location c@1+  building-left-x
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

: repair-building ( -- ) building repaired on check-breaches ;

  \ ===========================================================
  cr .( Locations) ?depth debug-point \ {{{1

8 cconstant locations

: next-location ( -- )
  location c@1+ locations min location c! ;
  \ Increase the location number, but not beyond the maximum.
  \ XXX TMP --
  \ XXX TODO -- Check the limit to finish the game instead.

variable used-projectiles
  \ Counter.

: battle-bonus ( -- n ) location c@1+ 500 *
                        battle-breaches c@ 100 * -
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
  cr .( Projectiles) ?depth debug-point \ {{{1

50 cconstant #bullets
  \ Number of bullets the tank can hold.
  \ XXX TMP -- provisional value, for testing

20 cconstant #missiles
  \ Number of missiles the tank can hold.
  \ XXX TMP -- provisional value, for testing

20 cconstant #balls
  \ Number of balls the tank can hold.
  \ XXX TMP -- provisional value, for testing

#bullets #missiles + #balls + cconstant #projectiles
  \ Total number of projectiles the tank can hold.

#bullets allot-xstack constant bullets-stack
  \ Create an extra stack to store the unused bullets.

#missiles allot-xstack constant missiles-stack
  \ Create an extra stack to store the unused missiles.

#balls allot-xstack constant balls-stack
  \ Create an extra stack to store the unused balls.

#projectiles allot-xstack constant used-projectiles-stack
  \ Create an extra stack to store the used projectiles,
  \ any type.

0
  cfield: ~projectile-y            \ row
  cfield: ~projectile-x            \ column
  cfield: ~projectile-sprite       \ UDG (*)
  cfield: ~projectile-frames       \ count (*)
  cfield: ~projectile-attr         \ attribute (*)
  cfield: ~projectile-altitude     \ row (*)
  cfield: ~projectile-delay        \ counter
  cfield: ~projectile-max-delay    \ bitmask (*)
   field: ~projectile-action       \ xt
  cfield: ~projectile-harmlessness \ level (0..x)
cconstant /projectile
  \ Data structure of a projectile.
  \
  \ (*) = Constant value copied by `get-projectile` from the
  \       structure pointed by `gun~`.

#projectiles /projectile * constant /projectiles

create projectiles /projectiles allot
  \ Projectiles data table.

: projectile#>~ ( n -- a ) /projectile * projectiles + ;
  \ Convert projectile number _n_ to its data address _a_.

0 projectile#>~ constant projectile~
  \ Data address of the current projectile in the data table.

cvariable #flying-projectiles
  \ Counter: number of currently flying projectiles.

cvariable flying-projectile#
  \ Index of the currently managed flying projectile in the
  \ array of flying projectiles.

16 cconstant max-flying-projectiles
  \ Maximum number of projectiles that can be flying at the
  \ same time. This is used only to create the array, i.e.  no
  \ check is done at run-time. Therefore, the value must be
  \ enough according to the configuration of the shooting
  \ intervals.

max-flying-projectiles cells cconstant /flying-projectiles

create flying-projectiles /flying-projectiles allot
  \ Array of flying projectiles.

: start-flying ( a -- )
  #flying-projectiles c@ flying-projectiles array> !
  #flying-projectiles c1+! used-projectiles 1+! ;
  \ Store projectile _a_ into the array of flying projectiles
  \ and update the count of currently flying projectiles.

: -flying-projectile ( n -- )
  flying-projectiles /flying-projectiles rot 1+ cells /string
  over cell- swap cmove
  #flying-projectiles c1-! ;
  \ Remove projectile _n_ from the array of flying projectiles
  \ and update the count of currently flying projectiles.

defer gun-stack ( -- )
  \ Activate the projectile stack of the current gun.

: destroy-projectile ( -- )
  flying-projectile# c@ -flying-projectile
  used-projectiles-stack xstack projectile~ >x gun-stack ;

: recharge ( a n -- )
  0 do used-projectiles-stack xstack x>
                         over xstack >x loop drop ;
  \ Recharge the projectiles stack _a_ with _n_ projectiles
  \ from the used projectiles stack.

: recharge-bullet-gun ( -- )
  bullets-stack dup xstack xclear #bullets recharge ;
  \ Empty and recharge the bullet gun.

: recharge-missile-gun ( -- )
  missiles-stack dup xstack xclear #missiles recharge ;
  \ Empty and recharge the missile gun.

: recharge-ball-gun ( -- )
  balls-stack dup xstack xclear #balls recharge ;
  \ Empty and recharge the ball gun.

: recharge-guns ( -- )
  recharge-bullet-gun recharge-missile-gun recharge-ball-gun ;
  \ Empty and recharge all guns.

: -projectiles ( -- ) projectiles /projectiles erase ;
  \ Erase the projectiles data table.

: -unused-projectiles ( -- )
  used-projectiles-stack xstack xclear
  #projectiles 0 do i projectile#>~ >x loop ;
  \ Fill the stack of unused projectiles with the data
  \ addresses of all projectiles.

tank-y 1- cconstant projectile-y0
  \ Initial row of the projectiles.

projectile-y0 columns * constant /hit-projectiles

here /hit-projectiles allot
     dup columns - constant hit-projectiles>
                   constant hit-projectiles
  \ Byte array to mark the projectiles that have been hit by
  \ another projectile. The array is indexed by rows and
  \ columns, as a logical copy of the attributes area. The size
  \ of the array is calculated to contain only the rows used by
  \ projectiles, except `projectile-y0`, the initial one, where
  \ projectiles can not be hit yet. `hit-projectiles>` returns
  \ the address of one row above the actual data, simulating
  \ the row used by the status bar is part of the array, in
  \ order to save a run-time calculation when the array items
  \ are accessed. `hit-projectiles` returns the address of the
  \ actual data.

: -hit-projectiles ( -- )
  hit-projectiles /hit-projectiles erase ;
  \ Erase the array of hit projectiles.

: xy>hit-projectile ( col row -- ca )
  columns * + hit-projectiles> + ;
  \ Convert projectile coordinates _col row_ into their
  \ corresponding address _ca_ in array `hit-projectiles`.

: projectile-xy ( -- col row )
  projectile~ ~projectile-x c@ projectile~ ~projectile-y c@ ;
  \ Coordinates of the current projectile.

: hit-projectile? ( -- 0f )
  projectile-xy xy>hit-projectile c@ ;
  \ Has the current projectile been hit by other projectile?

: hit-projectile ( -- )
  projectile-xy xy>hit-projectile con ;
  \ Mark the current projectile as hit by other projectile.

: -hit-projectile ( -- )
  projectile-xy xy>hit-projectile coff ;
  \ Mark the current projectile as not hit by other projectile.

: prepare-projectiles ( -- ) #flying-projectiles coff
                             flying-projectile# coff
                             -projectiles
                             -hit-projectiles
                             -unused-projectiles ;

: new-projectiles ( -- ) prepare-projectiles
                         recharge-guns
                         used-projectiles off ;

  \ ===========================================================
  cr .( Tank) ?depth debug-point \ {{{1

cvariable tank-x \ column

variable tank-time
  \ When the ticks clock reaches the contents of this variable,
  \ the tank will move.

variable arming-time
  \ When the ticks clock reaches the contents of this variable,
  \ the tank can change its gun type.

variable trigger-time
  \ When the ticks clock reaches the contents of this variable,
  \ the trigger can work.

: repair-tank ( -- )
  tank-time off arming-time off trigger-time off ;

columns udg/tank - 2/ cconstant parking-x

: park-tank ( -- ) parking-x tank-x c! tank-frame coff ;

                    1 cconstant tank-min-x
columns udg/tank - 1- cconstant tank-max-x
  \ Mininum and maximum columns of the tank.

: gun-x ( -- col ) tank-x c@1+ ;
  \ Return the column _col_ of the tank's gun.

: gun-below-building? ( -- f )
  gun-x building-left-x building-right-x between ;
  \ Is the tank's gun below the building?

: tank-rudder ( -- -1|0|1 )
  kk-left pressed? kk-right pressed? abs + ;
  \ Does the tank move? Return its column increment.

: outside? ( col -- f )
  building-left-x 1+ building-right-x within 0= ;
  \ Is column _col_ outside the building?
  \ The most left and most right columns of the building
  \ are considered outside, because they are the doors.

: next-col ( col -- ) 1+ 33 swap - 23688 c! 23684 1+! ;
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

: tank-frame+ ( n1 -- n2 ) 1+ dup tank-frames <> and ;
  \ Increase tank frame _n1_ resulting frame _n2_.
  \ If the limit was reached, _n2_ is zero.

: tank-frame- ( n1 -- n2 )
  ?dup if 1- else tank-max-frame then ;
  \ Decrease tank frame _n1_ resulting frame _n2_.

: tank-arm-udg ( -- c )
  tank-frame c@ tank-frame- udg/tank * tank-sprite + 1+ ;
  \ Return UDG _c_ of the tank arm. This is identical to
  \ `middle-tank-udg`, except the frame has to be decreased
  \ in order to prevent the tank chains from moving.

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
  at-tank@ -tank-extreme tank-parts drop tank-x c1+! ;
  \ Move the tank to the right.

: (.tank ( -- col ) at-tank@ tank-parts ;
  \ Display the tank at its current position and return column
  \ _col_ at its right.

: .tank ( -- ) (.tank drop ;
  \ Display the tank at its current position.

0 [if]

  \ XXX OLD -- The balls gun is wider than the central UDG of
  \ the tank. Therefore the whole tank must be displayed.

: .tank-arm ( -- )
  tank-attr attr!
  tank-x c@ 1+ dup tank-y at-xy
                   tank-arm-udg ?emit-outside drop ;
  \ If the middle part of the tank is visible (i.e. outside the
  \ building), display it.

[then]

: new-tank ( -- )
  repair-tank bullet-gun# set-gun park-tank .tank ;

: <tank ( -- ) tank-x c1-! (.tank -tank-extreme drop ;
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

: driving ( -- )
  tank-time @ past? 0exit \ exit if too soon
  tank-movement perform schedule-tank ;

: recharge-gun ( -- ) ;
  \ XXX TODO --

: recharging ( -- )
  gun-below-building? 0exit
  kk-up pressed?      0exit
  recharge-gun ;
  \ XXX TODO --

  \ ===========================================================
  cr .( Instructions) ?depth debug-point \ {{{1

: game-title ( -- )
  home game-title$ columns type-center-field ;

: game-version ( -- ) version$ 1 center-type ;

127 cconstant '(c)'
  \ The code of the copyright symbol.

: (.copyright ( -- )
  row 1 over    at-xy '(c)' emit ."  2016..2018 Marcos Cruz"
      8 swap 1+ at-xy            ." (programandala.net)" ;
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

2 cconstant max-player

cvariable players  1 players c! \ 1..max-player
cvariable player   1 player  c! \ 1..max-player

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

[breakable] [if]

: quit-game ( -- ) mode-32 default-colors quit ;
  \ XXX TMP -- for debugging

: ?quit-game ( -- ) break-key? if quit-game then ;
  \ XXX TMP -- for debugging

[then]

: menu ( -- )
  begin
    [breakable] [if] ?quit-game [then] \ XXX TMP --
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

cvariable invaders
  \ Current number of invaders during the attack.

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
  [ udg/invader ] x* invader~ ~sprite c@ + ;
  \ First UDG _c_ of the current frame of the current invader's
  \ sprite, calculated from its sprite and its frame.
  \
  \ XXX TODO -- Add calculation to change the sprite depending
  \ on the flying direction. A flag field is needed to
  \ deactivate this for docked invaders.

: .invader ( -- )
  invader~ ~attr c@ attr! invader-udg .2x1-udg-sprite ;
  \ Display the current invader.  at the cursor coordinates, in
  \ its proper attribute.

: x>bricks-xy ( col1 -- col1 row1 col2 row2 col3 row3 )
  invader~ ~y c@ 2dup 1+ 2dup 2- ;
  \ Convert the column _col1_ of the broken wall to the
  \ coordinates of the broken brick above the invader, _col3
  \ row3_, below it, _col3 row3_, and in front of it, _col1
  \ row1_.

: break-bricks> ( col1 row1 col2 row2 col3 row3 -- )
  broken-wall-attr attr!
  at-xy broken-top-left-brick .1x1sprite
  at-xy broken-bottom-left-brick .1x1sprite
  sky-attr attr! at-xy space ;
  \ Display the broken bricks at the given coordinates, which
  \ are at the right of the current invader: above the invader,
  \ _col3 row3_; below the invader, _col2 row2_; and an empty
  \ space in front of it, _col1 row1_.

: <break-bricks ( col1 row1 col2 row2 col3 row3 -- )
  broken-wall-attr attr!
  at-xy broken-top-right-brick .1x1sprite
  at-xy broken-bottom-right-brick .1x1sprite
  sky-attr attr! at-xy space ;
  \ Display the broken bricks at the given coordinates, which
  \ are at the left of the current invader: above the invader,
  \ _col3 row3_; below it, _col2 row2_; and an empty space in
  \ front of it, _col1 row1_.

0 [if] \ XXX REMARK -- Not used.

: c@udg/invader+ ( -- )
  udg/invader case
    1 of postpone c@1+ endof
    2 of postpone c@2+ endof
    postpone c@ postpone udg/invader postpone +
  endcase ; immediate

[then]

: break-container ( -- ) container-attr attr! catastrophe on ;

: break-container> ( -- )
  break-container
  invader~ ~x c@ [ udg/invader udg/container 1- + ] cliteral +
  invader~ ~y c@ at-xy broken-top-right-container .1x1sprite
  invader~ ~x [ udg/invader ] c@x+ invader~ ~y c@1+ at-xy
  broken-bottom-left-container .1x1sprite ;
  \ Break container that is at the right of the invader.
  \
  \ XXX TODO -- Calculate alternatives to `c@2+` and `c@1+` at
  \ compile-time, depending on the size of the invaders, just
  \ in case.

: <break-container ( -- )
  break-container
  invader~ ~x [ udg/container ] c@x- invader~ ~y c@ at-xy
  broken-top-left-container .1x1sprite
  invader~ ~x c@1- invader~ ~y c@1+ at-xy
  broken-bottom-right-container .1x1sprite ;
  \ Break container that is at the left of the invader.

: healthy? ( -- f ) invader~ ~stamina c@ max-stamina = ;
  \ Is the current invader healthy? Has it got maximum stamina?

defer <attacking-invader-action ( -- )
  \ Action of the invaders that are attacking to the left.

defer attacking>-invader-action ( -- )
  \ Action of the invaders that are attacking to the right.

defer <retreating-invader-action ( -- )
  \ Action of the invaders that are retreating to the left.

defer retreating>-invader-action ( -- )
  \ Action of the invaders that are retreating to the right.

      ' <attacking-invader-action ,
      ' <retreating-invader-action ,
      ' noop ,
here  ' noop ,
      ' attacking>-invader-action ,
      ' retreating>-invader-action ,
      constant flying-invader-actions ( a )
      \ Execution table.

: set-invader-move-action ( -1..1 -- )
  2* attacking? + flying-invader-actions array>
  @ invader~ ~action ! ;
  \ Set the action of the current invader after x-coordinate
  \ increment _-1..1_.

: set-invader-direction ( -1..1 -- )
  dup invader~ ~x-inc !
      set-invader-move-action ;
  \ Set the direction of the current invader after x-coordinate
  \ increment _-1..1_.

: impel-invader ( -- )
  invader~ ~x-inc @ set-invader-move-action ;
  \ Restore the moving action of the current invader, using
  \ its current direction.

: change-direction ( -- )
  invader~ ~x-inc @ negate set-invader-direction ;
  \ Change the direction of the current invader.

: turn-back ( -- ) change-direction set-flying-invader-sprite ;
  \ Make the current invader turn back.

: <turn-back ( -- )
  -1 set-invader-direction set-<flying-invader-sprite ;
  \ Make the current invader turn back to the left.

: turn-back> ( -- )
  1 set-invader-direction set-flying>-invader-sprite ;
  \ Make the current invader turn back to the right.

defer <breaking-invader-action ( -- )
  \ Action of the invaders that are breaking the wall to the
  \ left.

defer breaking>-invader-action ( -- )
  \ Action of the invaders that are breaking the wall to the
  \ right.

: undock ( -- ) invader~ ~initial-x-inc @ set-invader-direction
                set-flying-invader-sprite ;
  \ Undock the current invader.

: is-there-a-projectile? ( col row -- 0f )
  xy>attr bright-mask and ;

: .sky ( -- ) sky-attr attr! space ;
  \ Display a sky-color space.

: left-of-invader ( -- col row )
  invader~ ~x c@1- invader~ ~y c@ ;
  \ Coordinates _col row_ at the right of the current invader.

: right-of-invader ( -- col row )
  invader~ ~x [ udg/invader ] c@x+ invader~ ~y c@ ;
  \ Coordinates _col row_ at the left of the current invader.

defer ?dock ( -- )
  \ If the current invader is at home, dock it.

: docked? ( -- f ) invader~ ~x c@ invader~ ~initial-x c@ = ;
  \ Is the current invader at the dock, i.e. at its start
  \ position?

: is-there-a-wall? ( xy -- f ) xy>attr brick-attr = ;

: is-there-a-container? ( xy -- f ) xy>attr container-attr = ;

: <move-invader ( -- )
  invader~ ~x c1-! at-invader .invader .sky ;
  \ Move the current invader to the left.

: <hit-container ( -- )
  healthy? if   <break-container <move-invader exit
           then turn-back> ;
  \ Hit the container that is at the left of the invader.

: <start-breaking-the-wall ( -- )
  invader~ ~species @ ~breaking-left-sprite c@
  invader~ ~species @ ~breaking-left-sprite-frames c@
  set-invader-sprite
  ['] <breaking-invader-action invader~ ~action ! ;

: <hit-wall ( -- )
  healthy? if   <start-breaking-the-wall exit
           then turn-back> ;
  \ If the current invader is healthy, start breaking the wall
  \ at its left; else turn back.

:noname ( -- )
  left-of-invader cond
    2dup is-there-a-projectile? if 2drop docked?   ?exit
                                         turn-back> exit else
    2dup is-there-a-wall?       if 2drop <hit-wall  exit else
    is-there-a-container?       if <hit-container        else
    <move-invader
  thens
  ; ' <attacking-invader-action defer!
  \ Move the current invader, which is attacking to the left,
  \ detecting projectiles, wall and containers.

:noname ( -- )
  left-of-invader is-there-a-projectile?
  if   turn-back> exit
  then <move-invader ?dock
  ; ' <retreating-invader-action defer!
  \ Move the current invader, which is retreating to the left,
  \ detecting projectiles and dock.

: move-invader> ( -- )
  at-invader .sky .invader invader~ ~x c1+! ;
  \ Move the current invader to the right.

: hit-container> ( -- )
  healthy? if   break-container> move-invader> exit
           then <turn-back ;
  \ Hit the container that is at the right of the invader.

: start-breaking-the-wall> ( -- )
  invader~ ~species @ ~breaking-right-sprite c@
  invader~ ~species @ ~breaking-right-sprite-frames c@
  set-invader-sprite
  ['] breaking>-invader-action invader~ ~action ! ;

: hit-wall> ( -- )
  healthy? if   start-breaking-the-wall> exit
           then <turn-back ;
  \ If the current invader is healthy, start breaking the wall
  \ at its right; else turn back.

:noname ( -- )
  right-of-invader cond
    2dup is-there-a-projectile? if 2drop docked?  ?exit
                                       <turn-back  exit else
    2dup is-there-a-wall?       if 2drop hit-wall> exit else
    is-there-a-container?       if hit-container>       else
    move-invader>
  thens
  ; ' attacking>-invader-action defer!
  \ Move the current invader, which is attacking to the right,
  \ detecting projectiles, wall and containers.

:noname ( -- )
  right-of-invader is-there-a-projectile?
  if   <turn-back exit
  then move-invader> ?dock
  ; ' retreating>-invader-action defer!
  \ Move the current invader, which is retreating to the right,
  \ detecting projectiles and dock.

10 cconstant cure-factor
  \ XXX TMP -- for testing

: difficult-cure? ( -- 0f )
  max-stamina invader~ ~stamina c@ - cure-factor * random ;
  \ Is it a difficult cure?  The less stamina, the more chances
  \ to be a difficult cure. This is used to delay the cure.

: cure ( -- ) invader~ ~stamina c@1+ max-stamina min
              invader-stamina! ;
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
           then at-invader .invader
  ; ' docked-invader-action defer!
  \ Action of the invaders that are docked.

: dock ( -- ) ['] docked-invader-action invader~ ~action !
                  set-docked-invader-sprite ;
  \ Dock the current invader.

:noname ( -- ) docked? 0exit dock ; ' ?dock defer!
  \ If the current invader is at the dock, dock it.

: one-more-breach ( -- ) breaches c1+! battle-breaches c1+! ;

: <break-wall ( -- )
  invader~ ~x c@1- x>bricks-xy
  <break-bricks one-more-breach impel-invader ;
  \ Break the wall at the left of the current invader.

: weak? ( -- f )
  [ max-endurance 2* ] cliteral invader~ ~endurance c@ -
  random ;
  \ Is the current invader too weak to break the wall?

: ?<break-wall ( -- ) weak? ?exit <break-wall ;
  \ If the current invader pushes hard enough, break the wall
  \ at the left.

:noname ( -- ) ?<break-wall at-invader .invader
               ; ' <breaking-invader-action defer!
  \ Action of the invaders that are breaking the wall to the
  \ left.

: break-wall> ( -- )
  invader~ ~x [ udg/invader ] c@x+ x>bricks-xy
  break-bricks> one-more-breach impel-invader ;
  \ Break the wall at the right of the current invader.

: ?break-wall> ( -- ) weak? ?exit break-wall> ;
  \ If the current invader pushes hard enough, break the wall
  \ at the right.

:noname ( -- ) ?break-wall> at-invader .invader
               ; ' breaking>-invader-action defer!
  \ Action of the invaders that are breaking the wall to the
  \ right.

: last-invader? ( -- f ) invader~ last-invader~ = ;
  \ Is the current invader the last one?

: alive? ( -- f ) invader~ ~stamina c@ 0<> ;
  \ Is the current invader alive?

: next-invader ( -- ) last-invader? if   first-invader~
                                    else invader~ /invader +
                                    then !> invader~ ;
  \ Point the current invader to the next one.

1 cconstant invader-interval \ ticks

: schedule-invader ( -- )
  ticks invader-interval + invader-time ! ;

: manage-invaders ( -- )
  invader-time @ past? 0exit \ exit if too soon
  alive? if invader~ ~action perform then
  next-invader schedule-invader ;
  \ If it's the right time, move the current invader, then
  \ choose the next one.

  \ ===========================================================
  cr .( Mothership) ?depth debug-point \ {{{1

cvariable motherships
  \ Number of motherships. Used as a flag.

defer do-mothership-action ( -- )
  \ The current action of the mothership.

: mothership-action! ( xt -- )
  ['] do-mothership-action defer! ;
  \ Set _xt_ as the current action of the mothership.

defer invisible-mothership-action ( -- )
  \ Action of the flying mothership when it's invisible.

: set-invisible-mothership-action ( -- )
  ['] invisible-mothership-action mothership-action! ;
  \ Set `invisible-mothership-action` as the current action of
  \ the mothership.

defer visible-mothership-action ( -- )
  \ Action of the flying mothership when it's visible.

: set-visible-mothership-action ( -- )
  ['] visible-mothership-action mothership-action! ;
  \ Set `visible-mothership-action` as the current action of
  \ the mothership.

defer stopped-mothership-action ( -- )
  \ Action of the mothership when it's stopped above the
  \ building.

: set-stopped-mothership-action ( -- )
  ['] stopped-mothership-action mothership-action! ;
  \ Set `stopped-mothership-action` as the current action of
  \ the mothership.

1 cconstant mothership-y
  \ Row of the mothership.

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

defer set-mothership-x-inc ( -1..1 -- )

: mothership-turns-back ( -- )
  mothership-stopped off
  mothership-x-inc @ negate set-mothership-x-inc ;

32 cconstant mothership-range
  \ Allowed columns of the mothership out of the screen, in
  \ either direction.

mothership-range negate     constant mothership-range-min-x
mothership-range columns + cconstant mothership-range-max-x
  \ X-coordinate limits of the mothership range.

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

: place-mothership ( -- ) mothership-x0 mothership-x ! ;
  \ Set the initial column of the motherhip, out of the screen.

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
  explosion-sprite explosion-frames set-mothership-sprite ;
  \ Make the mothership use the explosion sprite.

defer set-exploding-mothership ( -- )
  \ The mothership has been impacted. Set it accordingly.

: beamy ( c1 -- c2 ) papery white + brighty ;
  \ Convert mothership attribute _c1_ to the corresponding
  \ beam attribute _c2_.

0 cconstant mothership-attr
0  constant mothership-cell-attr
0 cconstant beam-attr
0  constant beam-cell-attr

: set-mothership-stamina ( n -- )
  dup c!> mothership-stamina
      stamina>attr dup c!> mothership-attr
                   dup dup join !> mothership-cell-attr
                       beamy dup c!> beam-attr
                                 dup join !> beam-cell-attr ;
  \ Set mothership stamina to _n_ and set its corresponding
  \ attributes.

' lightning1-sound alias damage-sound
  \ XXX TMP --
  \ XXX TODO -- look for a proper sound

: mothership-retreat-bonus ( -- n ) location c@1+ 8* ;
  \ Bonus points for making the mothership retreat.

: mothership-impacted ( -- )
  mothership-stamina 1- dup c!> mothership-stamina ?dup
  if   set-mothership-stamina damage-sound
       mothership-turns-back
       mothership-retreat-bonus update-score
  else set-exploding-mothership then ;

: init-mothership ( -- )
  1 motherships c!
  max-stamina set-mothership-stamina
  set-flying-mothership-sprite set-invisible-mothership-action
  mothership-stopped off mothership-time off
  place-mothership -1|1 set-mothership-x-inc ;

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

: -mothership ( -- )
  sky-attr attr! at-mothership udg/mothership spaces ;
  \ Delete the whole mothership.

: mothership-frame+ ( n1 -- n2 )
  1+ dup mothership-frames < and ;
  \ Increase frame _n1_ of the mothership, resulting frame
  \ _n2_.  If _n1_ is the maximum frame allowed, _n2_ is zero,
  \ which is the first one.
  \
  \ XXX TODO -- Use `mothership-max-frame <>` for speed.

: mothership-udg ( -- c )
  mothership-frame c@ dup mothership-frame+ mothership-frame c!
  [ udg/mothership ] x* mothership + ;
  \ UDG _c_ of the mothership.

: advance-mothership ( -- )
  mothership-x-inc @ mothership-x +! ;
  \ Advance the mothership on its current direction, adding its
  \ increment to its column.

: mothership-in-range? ( -- f )
  mothership-x @
  mothership-range-min-x mothership-range-max-x within ;
  \ Is the mothership in the range of its flying limit?

: (.mothership ( -- )
  mothership-attr attr! mothership-udg .2x1-udg-sprite ;
  \ Display the mothership, which is fully visible, at the
  \ cursor coordinates in its default attribute.

: .mothership ( -- ) at-mothership (.mothership ;
  \ Display the mothership, which is fully visible, at its
  \ coordinates, in its default attribute.

: (.visible-right-mothership ( -- )
  mothership-attr attr!
  mothership-udg [ udg/mothership 1- ] cliteral + emit-udg ;
  \ Display the mothership, which is partially visible (only
  \ its right side is visible) at the cursor coordinates.

: .visible-right-mothership ( -- )
  whole-mothership-min-x mothership-y at-xy
  (.visible-right-mothership ;
  \ Display the mothership, which is partially visible (only
  \ its right side is visible) at its proper coordinates.

: (.visible-left-mothership ( -- )
  mothership-attr attr! mothership-udg emit-udg ;
  \ Display the mothership, which is partially visible (only
  \ its left side is visible) at the cursor coordinates.

: .visible-left-mothership ( -- )
  visible-mothership-max-x mothership-y at-xy
  (.visible-left-mothership ;
  \ Display the mothership, which is partially visible (only
  \ its left side is visible) at its proper coordinates.

: .visible-mothership ( -- )
  mothership-x @ case
    visible-mothership-min-x of .visible-right-mothership endof
    visible-mothership-max-x of .visible-left-mothership  endof
    .mothership
  endcase ;
  \ Display the mothership, which is fully or partially
  \ visible.

: right-of-mothership ( -- col row )
  mothership-x @ [ udg/mothership ] x+ mothership-y ;
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

healthy-invader-attr dup join
constant healthy-invader-cell-attr

sky-attr dup join constant sky-cell-attr

 variable beam-y-inc   \ 1|-1
cvariable beam-y       \ row
cvariable beam-first-y \ row
cvariable beam-last-y  \ row

cvariable beam-invader#
  \ Number of the next invader to be created by the beam.

: set-beam ( row1 row2 xt -- )
  mothership-action!
  2dup beam-last-y c! dup beam-first-y c! beam-y c!
       swap - polarity beam-y-inc ! ;
  \ Set the beam to grow or shrink from _row1_ to _row2_ with
  \ handler _xt_.

: over-left-invaders? ( -- f )
  mothership-x @ invaders-min-x = ;
  \ Is the mothership over the left initial column of invaders?

: over-right-invaders? ( -- f )
  mothership-x @ invaders-max-x = ;
  \ Is the mothership over the right initial column of
  \ invaders?

: enlist-squadron ( -- ) half-max-invaders invaders c+! ;

: create-squadron ( -- )
  over-left-invaders? if   create-left-squadron
                      else create-right-squadron
                      then enlist-squadron ;
  \ Activate the new invaders created by the beam and update
  \ their global count.

: layer-y? ( row -- f )
  dup invader-min-y invader-max-y between
      swap rows/layer mod 0= and ;
  \ Is _row_ is a valid row of an invader layer?

: reach-invader? ( -- f ) beam-y c@ layer-y? ;
  \ Has the beam reached the row of an invader's layer?

: update-beam ( -- ) beam-y-inc @ beam-y c+! ;

: (beaming-up? ( -- )
  beam-y c@ beam-last-y c@ beam-first-y c@ between ;
  \ Is the beam still shrinking?

: beaming-up? ( -- f ) update-beam (beaming-up? ;
  \ Update a shrinking beam. Is it still shrinking?

: (beam-up ( -- )
  .mothership
  reach-invader? if   mothership-cell-attr
                 else sky-cell-attr
                 then mothership-x @ beam-y c@ xy>attra ! ;
  \ Shrink the beam towards de mothership one character.

: beaming-up-mothership-action ( -- )
  (beam-up beaming-up? ?exit
  create-squadron
  set-flying-mothership-sprite set-visible-mothership-action ;
  \ Action of the mothership when the beam is shrinking.

: beam-off ( -- )
  invader-max-y 1+ mothership-y 1+
  ['] beaming-up-mothership-action set-beam ;
  \ Turn the mothership's beam off, i.e. start shrinking it
  \ back to the mothership.

: .new-invader ( -- )
  invader~ ~initial-x c@ invader~ ~y c@ at-xy
  invader~ ~sprite c@ .2x1-udg-sprite ;
  \ Display invader _n_ at its initial position, with the
  \ current attribute.

: create-invader ( -- )
  invader~
    beam-invader# dup c@ set-invader set-docked-invader-sprite
                      beam-attr attr! .new-invader
                  c1+!
  !> invader~ ;
  \ Display the new invader and update its number.

: (beaming-down? ( -- f )
  beam-y c@ beam-first-y c@ beam-last-y c@ between ;
  \ Is the beam still growing?

: beaming-down? ( -- f ) update-beam (beaming-down? ;
  \ Update a growing beam. Is it still growing?

: (beam-down ( -- )
  .mothership
  beam-cell-attr mothership-x @ beam-y c@ xy>attra !
  reach-invader? if create-invader then ;
  \ Grow the beam towards the ground one character.

: beaming-down-mothership-action ( -- )
  (beam-down beaming-down? ?exit beam-off ;
  \ Manage the beam, which is growing down to the ground.
  \ If it's finished, display the new invaders and start
  \ shrinking the beam.

: first-new-invader# ( -- n )
  half-max-invaders over-left-invaders? 0= and ;
  \ Return the number of the first invader to create,
  \ depending on the position of the mothership.

: beam-on ( -- )
  set-beaming-mothership-sprite
  first-new-invader# beam-invader# c!
  mothership-y 1+ invader-max-y 1+
  ['] beaming-down-mothership-action set-beam ;
  \ Turn the mothership's beam on, i.e. start launching it
  \ towards the ground.

: need-help? ( n -- f ) 0= dup 0exit beam-on ;
  \ If number of invaders _n_ is zero turn the beam on and
  \ return _true_; otherwise do nothing and return _false_.

: help-right-side? ( -- f ) right-side-invaders need-help? ;
  \ If there's no invader at the right side, turn the beam on
  \ and return _true_; otherwise do nothing and return _false_.

: help-left-side? ( -- f ) left-side-invaders need-help? ;
  \ If there's no invader at the left side, turn the beam on
  \ and return _true_; otherwise do nothing and return _false_.

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
      ['] move-visible-mothership defer!
  ; ' set-mothership-x-inc defer!

: above-building? ( -- f )
  mothership-x @
  building-left-x
  building-right-x [ udg/mothership 1- ] cliteral -
  between ;
  \ Is the mothership above the building?

: stop-mothership ( -- ) 0 set-mothership-x-inc
                         set-stopped-mothership-action
                         mothership-stopped on ;

: ?stop-mothership ( -- ) mothership-stopped @ ?exit
                            above-building? 0= ?exit
                                      3 random ?exit
                               stop-mothership ;

: tank<mothership? ( -- f ) tank-x c@ mothership-x @ < ;
  \ Is the tank at the left of the mothership?

: tank>mothership? ( -- f ) tank-x c@ mothership-x @ > ;
  \ Is the tank at the right of the mothership?

: (new-mothership-x-inc ( -- -1|0|1 )
  left-side-invaders  0=
  right-side-invaders 0= abs +
  tank>mothership?           +
  tank<mothership?       abs + polarity ;
  \ Return the new direction of the mothership, which is
  \ stopped above the building.

: new-mothership-x-inc ( -- -1|0|1 )
  5 random 0= 0dup 0exit (new-mothership-x-inc ;
  \ Return the new direction of the mothership, which is
  \ stopped above the building. Four times out of five, the
  \ result is zero.

: ?start-mothership ( -- )
  new-mothership-x-inc ?dup 0exit
  set-mothership-x-inc set-visible-mothership-action ;

:noname ( -- )
  move-visible-mothership
  visible-mothership? 0=
  if set-invisible-mothership-action exit then
  ?stop-mothership
  ; ' visible-mothership-action defer!
  \ Action of the mothership when it's visible and not stopped.

:noname ( -- )
  ?start-mothership
  ; ' stopped-mothership-action defer!
  \ Action of the mothership when it's visible but stopped
  \ above the building.
  \
  \ XXX TODO -- Add curing.

:noname ( -- )
  advance-mothership visible-mothership?
  if   .visible-mothership set-visible-mothership-action exit
  then mothership-in-range? ?exit mothership-turns-back
  ; ' invisible-mothership-action defer!
  \ Action of the mothership when it's invisible.

8 cconstant mothership-interval \ ticks

: schedule-mothership ( -- )
  ticks mothership-interval + mothership-time ! ;

: mothership-destroyed? ( -- f ) motherships c@ 0= ;

: manage-mothership ( -- )
  mothership-destroyed?   ?exit \ exit if destroyed
  mothership-time @ past? 0exit \ exit if too soon
  do-mothership-action schedule-mothership ;

  \ ===========================================================
  cr .( Impact) ?depth debug-point \ {{{1

' shoot-sound alias mothership-bang ( -- )
  \ Make the explosion sound of the mothership.
  \ XXX TMP --
  \ XXX TODO -- look for a better sound

8 cconstant mothership-explosion-interval
  \ Ticks between the frames of the explosion.

variable mothership-explosion-time
  \ When the ticks clock reaches the contents of this variable,
  \ the explosion advances to the next frame.

: schedule-mothership-explosion ( -- )
  ticks mothership-explosion-interval +
  mothership-explosion-time ! ;

: destroy-mothership ( -- )
  -mothership ['] noop mothership-action! motherships ?c1-! ;

: mothership-explosion? ( -- f ) mothership-frame c@ 0<> ;
  \ Is the mothership explosion still active? When the frame
  \ counter is zero (first frame), the explosion cycle has been
  \ completed and _f_ is _false_.

: exploding-mothership-action ( -- )
  mothership-explosion-time @ past? 0exit \ exit if too soon
  at-mothership .mothership
  schedule-mothership-explosion
  mothership-explosion? ?exit destroy-mothership ;
  \ Action of the mothership when it's exploding.

: mothership-destroy-bonus ( -- n )
  mothership-retreat-bonus 8* ;
  \ Bonus points for destroying the mothership.

:noname ( -- )
  set-exploding-mothership-sprite
  .mothership schedule-mothership-explosion
  ['] exploding-mothership-action mothership-action!
  mothership-bang mothership-destroy-bonus update-score
  ; ' set-exploding-mothership defer!
  \ The mothership has been impacted. Set it accordingly.

' shoot-sound alias invader-bang ( -- )
  \ Make the explosion sound of an invader.
  \ XXX TMP --
  \ XXX TODO -- look for a better sound

: -invader ( -- ) sky-attr attr! at-invader 2 spaces ;
  \ Delete the current invader.
  \
  \ XXX TODO -- Use `bl-udg` for speed.

: destroy-invader ( -- )
  -invader invader~ ~stamina coff invader~ ~action off
  invaders c1-! ;

: impacted-invader ( -- n )
  projectile~ ~projectile-y c@ invader-min-y - 2/
  projectile~ ~projectile-x c@ [ columns 2/ ] cliteral > abs
  half-max-invaders * + ;
  \ Return the impacted invader _n_, calculated from the
  \ projectile coordinates: Invaders 0 and 5 are at the top,
  \ one row below the top of the building; 1 and 6 are two rows
  \ below and so on.  0..4 are at the left of the screen; 5..9
  \ are at the right.
  \
  \ XXX TODO -- Change the numbering of invaders to reading
  \ order, to simplify the calculation: 0: top left; 1: top
  \ right; etc..

: invader-explosion? ( -- f ) invader~ ~frame c@ 0<> ;
  \ Is the explosion of the current invader still active? When
  \ the frame counter is zero (first frame), the explosion
  \ cycle has been completed and _f_ is _false_.

8 cconstant invader-explosion-interval
  \ Ticks between the frames of the explosion.

: schedule-invader-explosion ( -- )
  ticks invader-explosion-interval +
  invader~ ~explosion-time ! ;

: exploding-invader-action ( -- )
  invader~ ~explosion-time @ past? 0exit \ exit if too soon
  at-invader .invader schedule-invader-explosion
  invader-explosion? ?exit destroy-invader ;
  \ Action of the invader when it's exploding.

: set-exploding-invader-sprite ( -- )
  explosion-sprite explosion-frames set-invader-sprite ;
  \ Make the invader use the explosion sprite.

: set-exploding-invader ( -- )
  set-exploding-invader-sprite
  at-invader .invader schedule-invader-explosion
  ['] exploding-invader-action invader~ ~action !
  invader-bang invader-destroy-bonus update-score ;
  \ The current invader has been impacted. Set it accordingly.

' lightning1-sound alias retreat-sound
  \ XXX TMP --
  \ XXX TODO -- look for a proper sound

: retreat ( -- )
  retreat-sound turn-back invader-retreat-bonus update-score ;
  \ The current invader retreats.

: wounded ( -- )
  invader~ ~stamina c@1- min-stamina max invader-stamina! ;
  \ Reduce the invader's stamina after being shoot.

: mortal? ( -- f ) invader~ ~stamina c@ 2*
                   invader~ ~layer c@ +
                   invader~ ~endurance c@ +
                   projectile~ ~projectile-harmlessness c@ +
                   random 0= ;
  \ Is it a mortal impact?  The random calculation depends on
  \ the stamina, the altitude, the species' endurance and the
  \ type of projectile.

: invader-exploding? ( -- f )
  invader~ ~action @ ['] exploding-invader-action = ;
  \ Is the current invader exploding?

: (invader-impacted ( -- )
  invader-exploding? ?exit
  mortal? if set-exploding-invader exit then
  wounded attacking? 0exit retreat ;
  \ The current invader has been impacted by the projectile.
  \ It explodes or retreats.

: invader-impacted ( -- )
  invader~ impacted-invader set-invader (invader-impacted
  !> invader~ ;
  \ An invader has been impacted by the projectile.
  \ Make it the current one and manage it.

: mothership-impacted? ( -- f )
  projectile~ ~projectile-y c@ mothership-y = ;

: mothership-exploding? ( -- f )
  action-of do-mothership-action
  ['] exploding-mothership-action = ;
  \ Is the mothership exploding?

: impact ( -- )
  mothership-impacted? if   mothership-exploding? ?exit
                            mothership-impacted
                       else invader-impacted
                       then destroy-projectile ;

: sky-attr<>
  \ Compilation: ( -- )
  \ Run-time:    ( c -- f|0f )
  [ sky-attr 0<> ] [if]   postpone sky-attr postpone <> ( f )
                   [else] ( 0f ) [then] ; immediate
  \ Compile the words needed to check if _c_ is different from
  \ `sky-attr`. If the value of `sky-attr` is zero, compile
  \ nothing. This is used only in case the value of `sky-attr`
  \ changes in future versions.

: impact? ( -- f|0f ) projectile-xy xy>attr sky-attr<> ;
  \ Did the projectile impacted?

  \ ===========================================================
  cr .( Arms) ?depth debug-point \ {{{1

3 cconstant #guns

cvariable gun-type
  \ Identifier of the current gun of the tank:
  \ 0 = bullet gun
  \ 1 = missile gun
  \ 2 = ball gun

0 constant gun~
  \ Data address of the current arm identified by `gun-type`.

0
   field: ~gun-projectile-stack        \ address
  cfield: ~gun-projectile-sprite       \ UDG
  cfield: ~gun-projectile-frames       \ count
  cfield: ~gun-projectile-attr         \ attribute
  cfield: ~gun-projectile-altitude     \ row
  cfield: ~gun-projectile-x            \ column
  cfield: ~gun-tank-sprite             \ UDG
  cfield: ~gun-trigger-interval        \ ticks
  cfield: ~gun-projectile-max-delay    \ bitmask
   field: ~gun-projectile-action       \ xt
  cfield: ~gun-projectile-harmlessness \ level (0..x)
cconstant /gun
  \ Data structure of an arm projectile.

#guns /gun * cconstant /guns

create guns /guns allot

: gun#>~ ( n -- a ) /gun * guns + ;
  \ Convert gun number _n_ to its data address _a_.

 bullet-gun# gun#>~ constant bullet-gun~
missile-gun# gun#>~ constant missile-gun~
   ball-gun# gun#>~ constant ball-gun~

:noname ( -- ) gun~ ~gun-projectile-stack @ xstack
               ; ' gun-stack defer!
  \ Activate the projectile stack of the current gun.

:noname ( n -- )
  dup gun-type c!
      gun#>~ dup !> gun~
             dup ~gun-tank-sprite c@ c!> tank-sprite
                 ~gun-projectile-x c@ c!> ammo-x
  gun-stack
  ; ' set-gun defer!
  \ Set _n_ as the current arm (0=gun machine; 1=missile gun).

  \ --------------------------------------------
  \ Set guns' data

 bullets-stack  bullet-gun~ ~gun-projectile-stack !
missiles-stack missile-gun~ ~gun-projectile-stack !
   balls-stack    ball-gun~ ~gun-projectile-stack !

 bullet-sprite  bullet-gun~ ~gun-projectile-sprite c!
missile-sprite missile-gun~ ~gun-projectile-sprite c!
   ball-sprite    ball-gun~ ~gun-projectile-sprite c!

bullet-frames   bullet-gun~ ~gun-projectile-frames c!
missile-frames missile-gun~ ~gun-projectile-frames c!
ball-frames       ball-gun~ ~gun-projectile-frames c!

bullet-attr   bullet-gun~ ~gun-projectile-attr c!
missile-attr missile-gun~ ~gun-projectile-attr c!
ball-attr       ball-gun~ ~gun-projectile-attr c!

invader-min-y    bullet-gun~ ~gun-projectile-altitude c!
mothership-y 1+ missile-gun~ ~gun-projectile-altitude c!
building-top-y 1+  ball-gun~ ~gun-projectile-altitude c!

 bullets-x  bullet-gun~ ~gun-projectile-x c!
missiles-x missile-gun~ ~gun-projectile-x c!
   balls-x    ball-gun~ ~gun-projectile-x c!

 bullet-gun-tank-sprite  bullet-gun~ ~gun-tank-sprite c!
missile-gun-tank-sprite missile-gun~ ~gun-tank-sprite c!
   ball-gun-tank-sprite    ball-gun~ ~gun-tank-sprite c!

 8  bullet-gun~ ~gun-trigger-interval c!
16 missile-gun~ ~gun-trigger-interval c!
24    ball-gun~ ~gun-trigger-interval c!

%00001  bullet-gun~ ~gun-projectile-max-delay c!
%00111 missile-gun~ ~gun-projectile-max-delay c!
%11111    ball-gun~ ~gun-projectile-max-delay c!

1  bullet-gun~ ~gun-projectile-harmlessness c!
0 missile-gun~ ~gun-projectile-harmlessness c!
3    ball-gun~ ~gun-projectile-harmlessness c!

  \ ===========================================================
  cr .( Shoot) ?depth debug-point \ {{{1

: at-projectile ( -- ) projectile-xy at-xy ;
  \ Set the cursor position at the coordinates of the
  \ projectile.

0 [if]

  \ XXX TODO -- Alternative mode to display projectiles, with
  \ sequential frames, to be used with projectile explosions.

defer projectile ( -- c )
  \ Return the UDG _c_ of a frame of the projectile.

: projectile-frame+ ( n1 -- n2 )
  1+ dup invader~ ~frames c@ < and ;
  \ Increase frame _n1_ resulting frame _n2_.
  \ If the limit was reached, _n2_ is zero.
  \
  \ XXX TODO -- Use `~max-frame <>` for speed.

: sequential-projectile ( -- c )
  projectile~ ~projectile-sprite dup c@
  projectile~ ~projectile-frame c@
  dup projectile-projectile-frame+
  projectile~ ~projectile-frame c!
  projectile~ ~projectile-frames c@ + ;
  \ Return the UDG _c_ of the sequential frame of the
  \ projectile.

: random-projectile ( -- c )
  projectile~ ~projectile-sprite c@
  projectile~ ~projectile-frames c@ random + ;
  \ Return the UDG _c_ of a random frame of the projectile.

[then]

: projectile ( -- c )
  projectile~ ~projectile-sprite c@
  projectile~ ~projectile-frames c@ random + ;
  \ Return the UDG _c_ of a random frame of the projectile.

: .projectile ( -- ) projectile~ ~projectile-attr c@ attr!
                     at-projectile projectile .1x1sprite ;
  \ Display the projectile.

' whip-sound alias fire-sound ( -- )

: -projectile ( -- ) projectile-xy xy>attr
                     projectile~ ~projectile-attr c@ <> ?exit
                     at-projectile .sky ;
  \ Delete the projectile.
  \
  \ XXX REMARK -- Checking the attribute  prevents the
  \ projectile from erasing part of an invader in some cases,
  \ but the solution should be in the movement.

: projectile-lost? ( -- f )
  projectile~ ~projectile-y c@
  projectile~ ~projectile-altitude c@ < ;
  \ Is the projectile lost?

: projectile-delay ( -- n )
  projectile~ ~projectile-delay c@1+
  projectile~ ~projectile-max-delay c@ and dup
  projectile~ ~projectile-delay c! ;
  \ Update the delay counter of the current projectile
  \ and return it. When _n_ is zero, the projectile is ready
  \ to be moved.

: set-projectile-sprite ( c n -- )
  projectile~ ~projectile-frames c!
  projectile~ ~projectile-sprite c! ;
  \ Set character _c_ as the first character of the first
  \ sprite of the current projectile, and _n_ as the number of
  \ frames.

: set-exploding-projectile-sprite ( -- )
  projectile-explosion-sprite projectile-explosion-frames
  set-projectile-sprite ;
  \ Make the projectile use the explosion sprite.

8 cconstant projectile-explosion-interval
  \ Ticks between the frames of the explosion.

variable projectile-explosion-time
  \ When the ticks clock reaches the contents of this variable,
  \ the explosion advances to the next frame.

: schedule-projectile-explosion ( -- )
  ticks projectile-explosion-interval +
  projectile-explosion-time ! ;

cvariable projectile-frame
  \ Current frame of the exploding projectile sprite.

: projectile-explosion? ( -- f ) projectile-frame c@ 0<> ;
  \ Is the projectile explosion still active? When the frame
  \ counter is zero (first frame), the explosion cycle has been
  \ completed and _f_ is _false_.

: exploding-projectile-action ( -- )
  projectile-explosion-time @ past? 0exit \ exit if too soon
  at-projectile .projectile
  schedule-projectile-explosion
  projectile-explosion? ?exit -projectile destroy-projectile ;
  \ Action of the projectile when it's exploding.
  \
  \ XXX TMP -- `-projectile` is used at the end, because the
  \ frame of the explosion sprite is chosen randomly.

' shoot-sound alias projectile-bang ( -- )
  \ Make the explosion sound of the projectile.
  \ XXX TMP --
  \ XXX TODO -- look for a better sound

: set-exploding-projectile ( -- )
  set-exploding-projectile-sprite
  .projectile schedule-projectile-explosion
  ['] exploding-projectile-action
  projectile~ ~projectile-action !  projectile-bang ;

: move-projectile ( -- )
  hit-projectile?
  if -hit-projectile set-exploding-projectile exit then
  projectile-delay ?exit
  -projectile projectile-lost? if destroy-projectile exit then
  projectile~ ~projectile-y c1-!
  projectile-xy is-there-a-projectile?
  if projectile-xy hit-projectile destroy-projectile exit then
  impact? if impact exit then .projectile ;
  \ Default action of the projectiles.
  \
  \ XXX TODO -- Move `hit-something?` here to simplify the
  \ logic.

' move-projectile bullet-gun~ ~gun-projectile-action !
  \ Set the action of bullets.

' move-projectile missile-gun~ ~gun-projectile-action !
  \ Set the action of missiles.

: one-less-breach ( -- ) breaches c1-! battle-breaches c1-! ;

: repair-breach ( col row -- ) 2dup 1+ at-xy .brick
                               2dup    at-xy .brick
                                    1- at-xy .brick
                               one-less-breach ;

: is-there-breach? ( col row -- f ) xy>attr sky-attr = ;
  \ Is there a breach at coordinates _col row_?

: right-of-projectile-xy ( -- col row )
  projectile~ ~x c@ 1+ projectile~ ~y c@ ;

: move-left-wall-ball-projectile ( -- )
  hit-projectile?
  if -hit-projectile set-exploding-projectile exit then
  projectile-delay ?exit -projectile
  right-of-projectile-xy is-there-breach?
  if right-of-projectile-xy repair-breach
     destroy-projectile exit then
  projectile-lost? if destroy-projectile exit then
  projectile~ ~projectile-y c1-!
  impact? if impact exit then .projectile ;
  \ Action of the balls that are flying on the left wall of the
  \ building, and therefore can repaier the brechs.

: left-of-projectile-xy ( -- col row )
  projectile~ ~x c@ 1- projectile~ ~y c@ ;

: move-right-wall-ball-projectile ( -- )
  hit-projectile?
  if -hit-projectile set-exploding-projectile exit then
  projectile-delay ?exit -projectile
  left-of-projectile-xy is-there-breach?
  if left-of-projectile-xy repair-breach
     destroy-projectile exit then
  projectile-lost? if destroy-projectile exit then
  projectile~ ~projectile-y c1-!
  impact? if impact exit then .projectile ;
  \ Action of the balls that are flying on the right wall of
  \ the building, and therefore can repaier the brechs.

: schedule-trigger ( -- )
  ticks gun~ ~gun-trigger-interval c@ + trigger-time ! ;

: get-projectile-sprite&action ( -- )
  gun~ ~gun-projectile-sprite c@
  gun~ ~gun-projectile-frames c@
  set-projectile-sprite
  ['] move-projectile projectile~ ~projectile-action ! ;
  \ Get the sprite and action of the new current projectile.

: get-ball-sprite&action ( -- )
  gun-x case
    building-left-x 1- of
      left-wall-ball-sprite left-wall-ball-frames
      set-projectile-sprite
      ['] move-left-wall-ball-projectile
      projectile~ ~projectile-action !
    endof
    building-right-x 1+ of
      right-wall-ball-sprite right-wall-ball-frames
      set-projectile-sprite
      ['] move-right-wall-ball-projectile
      projectile~ ~projectile-action !
    endof
    get-projectile-sprite&action
  endcase ;
  \ Get the proper sprite and action for the new current ball
  \ projectile.  The sprite and action depend on the position
  \ of the gun and the wall.

: get-projectile ( -- )
  x> !> projectile~
  gun-x projectile~ ~projectile-x c!
  projectile-y0 projectile~ ~projectile-y c!
  gun~ ball-gun~ = if   get-ball-sprite&action
                   else get-projectile-sprite&action then
  gun~ ~gun-projectile-attr c@
  projectile~ ~projectile-attr c!
  gun~ ~gun-projectile-max-delay c@
  projectile~ ~projectile-max-delay c!
  gun~ ~gun-projectile-altitude c@
  projectile~ ~projectile-altitude c!
  gun~ ~gun-projectile-harmlessness c@
  projectile~ ~projectile-harmlessness c! ;
  \ Get a new projectile and set its data according to the
  \ current value of `gun~`.  For the sake of run-time speed,
  \ some fields are copied from the structure pointed by
  \ `gun~`.

: launch-projectile ( -- )
  .projectile projectile~ start-flying fire-sound ;

: fire ( -- ) get-projectile launch-projectile .ammo
              schedule-trigger ;
  \ Fire the gun of the tank.

: flying-projectiles? ( -- f ) #flying-projectiles c@ 0<> ;
  \ Is there any projectile flying?

: trigger-ready? ( -- f ) trigger-time @ past? ;
  \ Is the trigger ready?

: trigger-pressed? ( -- f ) kk-fire pressed? ;
  \ Is the trigger pressed?

: next-flying-projectile ( -- )
  flying-projectile# c@1+ dup #flying-projectiles c@ < and
  dup flying-projectile# c!
      flying-projectiles array> @ !> projectile~ ;
  \ Point to the next flying projectile and make it the current
  \ one.

: manage-projectiles ( -- )
  flying-projectiles? 0exit
  projectile~ ~projectile-action perform
  next-flying-projectile ;
  \ Manage a flying projectile, if any.

: shooting ( -- )
  trigger-pressed?    0exit
  trigger-ready?      0exit
  projectiles-left    0exit
  gun-below-building? ?exit fire ;
  \ Manage the gun.

: tank-previous-frame ( -- )
  tank-frame c@ tank-frame- tank-frame c! ;
  \ Restore the previous frame of the tank.  This is used to
  \ prevent the tank chain from moving when the gun is changed.

: change-gun ( -- ) gun-type c@1+ dup #guns < and set-gun
                    tank-previous-frame .tank ;

10 cconstant arming-interval \ ticks

: schedule-arming ( -- )
  ticks arming-interval + arming-time ! ;

: arming ( -- ) arming-time @ past? 0exit
                kk-down pressed?    0exit
                change-gun schedule-arming ;

: manage-tank ( -- ) driving arming shooting recharging ;

: new-record? ( -- f ) score @ record @ > ;
  \ Is there a new record?

: update-record ( -- ) new-record? 0exit score @ record ! ;
  \ Update the record, if needed.

  \ ===========================================================
  cr .( Players config) ?depth debug-point \ {{{1

  \ XXX TODO --

  \ ===========================================================
  cr .( Location titles) ?depth debug-point \ {{{1

1 gigatype-style c!

: .location ( ca len row -- ) 0 swap at-xy gigatype-title ;
  \ Display location name part _ca len_, centered at row _row_.

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

: settle ( -- ) location c@ arrive ;
  \ Settle in the current location.

  \ ===========================================================
  cr .( The end) ?depth debug-point \ {{{1

  \ : defeat-tune ( -- )
  \   100 200 do  i 20 beep  -5 +loop ;
  \ XXX REMARK -- original code in Ace Forth

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
  update-record ;
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
  cr .( Status bar, part 2 ) ?depth debug-point \ {{{1

: .bullets-icon ( -- ) bullet-sprite .1x1sprite ;

: .missiles-icon ( -- ) missile-sprite .1x1sprite ;

: .balls-icon ( -- ) ball-sprite .1x1sprite ;

: score-label-x ( -- col ) score-x score-label$ nip - ;

: .score-label ( -- )
  score-label$ score-label-x status-bar-y at-xy type ;

: .record-separator ( -- )
  record-separator-x status-bar-y at-xy '/' emit ;

: color-status-bar ( b -- ) attributes columns rot fill ;

: hide-status-bar ( -- ) sky-attr color-status-bar ;

: reveal-status-bar ( -- ) text-attr color-status-bar ;

: (status-bar ( -- )
  gun-type c@  .bullets-icon  .bullets  space
               .missiles-icon .missiles space
               .balls-icon    .balls
  set-gun .score-label .score .record-separator .record ;

: status-bar ( -- )
  hide-status-bar home (status-bar reveal-status-bar ;

  \ ===========================================================
  cr .( Main loop) ?depth debug-point \ {{{1

: invaders-destroyed? ( -- f ) invaders c@ 0= ;

: extermination? ( -- f )
  invaders-destroyed? mothership-destroyed? and ;

: attack-wave ( -- ) init-mothership init-invaders ;

: fight ( -- )
  [breakable] [if] ?quit-game [then] \ XXX TMP --
  \ ~~stack-info \ XXX INFORMER
  manage-projectiles manage-tank
  manage-mothership manage-invaders ;

: end-of-attack? ( -- f ) extermination? catastrophe? or ;

: lose-projectiles ( -- )
  begin driving arming flying-projectiles?
  while manage-projectiles repeat ;
  \ Lose all flying projectiles.
  \ Note `driving` and `arming` are included only to prevent
  \ the projectiles from flying much faster than usual.

: under-attack ( -- )
  check-breaches attack-wave begin fight end-of-attack? until
  lose-projectiles ;

: another-attack? ( -- f ) breaches? catastrophe? 0= and ;

: prepare-attack ( -- ) new-projectiles status-bar ;

: prepare-battle ( -- ) settle new-tank ;

: ?repair-building ( -- ) new-breach? ?exit repair-building ;

: battle ( -- )
  prepare-battle
  begin prepare-attack under-attack another-attack?
  while attack-report ?repair-building repeat ;

: campaign ( -- ) begin battle catastrophe? 0=
                  while battle-report reward travel repeat ;

: prepare-war ( -- )
  catastrophe off first-location score off cls ;

: war ( -- ) prepare-war campaign defeat ;

: run ( -- ) begin mobilize war again ;

  \ ===========================================================
  cr .( Debugging tools) ?depth debug-point \ {{{1

: h ( -- ) home ;

: half ( -- ) half-max-invaders c!> max-invaders ;
  \ Reduce the actual invaders to the left half.

: .udgs ( -- ) cr last-udg 1+ 0 do i emit-udg loop ;
  \ Display all game UDGs.

: mp ( -- ) manage-projectiles ;
: fp? ( -- f ) flying-projectiles? ;
: mop ( -- ) move-projectile ;
: np ( -- ) next-flying-projectile ;

: .fp ( -- )
  #flying-projectiles c@ 0 ?do
    i flying-projectiles array> @ u.
  loop cr ;

: ni ( -- ) next-invader ;
: mi ( -- ) manage-invaders ;
: ia ( -- ) invader~ ~action perform ;
: ini ( -- ) prepare-war prepare-battle prepare-attack
             attack-wave ;

: h ( -- ) 7 attr! home ; \ home
: b ( -- ) cls building h ; \ building
: t ( -- ) .tank h ;
: tl ( -- ) <tank h ; \ move tank left
: tr ( -- ) tank> h ; \ move tank right

: mm ( -- ) manage-mothership ;
: ima ( -- ) invisible-mothership-action ;
: vma ( -- ) visible-mothership-action ;
: am ( -- ) advance-mothership ;
: vm? ( -- f ) visible-mothership? ;
: .m ( -- ) .mothership ;
: .vm ( -- ) .visible-mothership ;
: m? ( -- f ) mothership-in-range? ;
: -m ( -- ) -mothership ;
: mx ( -- col ) mothership-x @ ;
: im ( -- ) init-mothership ;
: mim ( -- ) mothership-impacted ;
: m ( -- ) begin key 'q' <> while manage-mothership repeat ;

: beon ( -- ) beam-on ;
: beoff ( -- ) beam-off ;
: beu ( -- ) beaming-up-mothership-action ;
: bed ( -- ) beaming-down-mothership-action ;

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

: sky ( -- )
  attributes /attributes bounds ?do
    i c@ sky-attr = if   [ cyan papery brighty ] cliteral i c!
                    then loop ;
  \ Reveal the zones of the screen that have the sky attribute,
  \ by coloring them with brighy cyan.  This is useful to
  \ discover attributes that shoud be sky-colored but have been
  \ contaminated by the movement of the sprites, because of
  \ wrong calculations in the code.

  \ ===========================================================
  cr .( Development benchmarks) ?depth debug-point \ {{{1

  \ --------------------------------------------
  \ Compare `type-udg` and `.2x1-udg-sprite`

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

  \ --------------------------------------------
  \ Compare alternative implementations of `hit-wall?`

0 [if]

  \ 2018-01-24

need ticks need timer

0 constant flying-to-the-left? ( f )

: invader-front-xy ( -- col row )
  invader~ ~x
  [ udg/invader 2 = ]
  [if]   c@2+ flying-to-the-left? 3* +
  [else] [ udg/invader 1 = ]
         [if]   c@1+ flying-to-the-left? 2* +
         [else] c@ udg/invader + flying-to-the-left?
                [ udg/invader 1+ ] cliteral * +
         [then]
  [then]
  invader~ ~y c@ ;
  \ Return the coordinates _col row_ at the front of the
  \ current invaders, i.e. the location the invader is heading
  \ to on its current direction.

: hit-wall?-OLD ( -- f )
  invader-front-xy xy>attr brick-attr = ;
  \ Has the current invader hit the wall of the building?

: invader-front-x ( -- col )
  invader~ ~x
  [ udg/invader 2 = ]
  [if]   c@2+ flying-to-the-left? 3* +
  [else] [ udg/invader 1 = ]
         [if]   c@1+ flying-to-the-left? 2* +
         [else] c@ udg/invader + flying-to-the-left?
                [ udg/invader 1+ ] cliteral * +
         [then]
  [then] ;
  \ Return column _col_ at the front of the current invader.

cvariable building-right-x  cvariable building-left-x

: building-near-x1 ( -- col )
  flying-to-the-left? if   building-right-x
                      else building-left-x
                      then c@ ;

: hit-wall?-NEW1 ( -- f ) invader-front-x building-near-x1 = ;
  \ Has the current invader hit the wall of the building?

: building-near-x2 ( -- col )
  flying-to-the-left? if building-right-x c@ exit then
                         building-left-x  c@ ;

: hit-wall?-NEW2 ( -- f ) invader-front-x building-near-x2 = ;
  \ Has the current invader hit the wall of the building?

create building-xs ' building-left-x , ' building-right-x ,

: building-near-x5 ( -- col )
  flying-to-the-left? building-xs array> @ c@ ;

: hit-wall?-NEW5 ( -- f ) invader-front-x building-near-x5 = ;
  \ Has the current invader hit the wall of the building?

0 cconstant building-right-x  0 cconstant building-left-x

: building-near-x3 ( -- col )
  flying-to-the-left? if building-right-x exit then
                         building-left-x  ;

: hit-wall?-NEW3 ( -- f ) invader-front-x building-near-x3 = ;
  \ Has the current invader hit the wall of the building?

create building-xs ' building-left-x , ' building-right-x ,

: building-near-x4 ( -- col )
  flying-to-the-left? building-xs array> perform ;

: hit-wall?-NEW4 ( -- f ) invader-front-x building-near-x4 = ;
  \ Has the current invader hit the wall of the building?

: bench-hit-wall ( n -- )
  dup ticks swap 0 ?do
        i 1 and !> flying-to-the-left?
        hit-wall?-OLD  drop loop cr ." old :" timer
  dup ticks swap 0 ?do
        i 1 and !> flying-to-the-left?
        hit-wall?-NEW1 drop loop cr ." new1:" timer
  dup ticks swap 0 ?do
        i 1 and !> flying-to-the-left?
        hit-wall?-NEW2 drop loop cr ." new2:" timer
  dup ticks swap 0 ?do
        i 1 and !> flying-to-the-left?
        hit-wall?-NEW3 drop loop cr ." new3:" timer
  dup ticks swap 0 ?do
        i 1 and !> flying-to-the-left?
        hit-wall?-NEW4 drop loop cr ." new4:" timer
      ticks swap 0 ?do
        i 1 and !> flying-to-the-left?
        hit-wall?-NEW5 drop loop cr ." new5:" timer ;

  \
  \ |=====================================================
  \ | Times |  old |  new1 |  new2 |  new3 |  new4 |  new5
  \
  \ |  1000 |   42 |    44 |    44 |    44 |    45 |    49
  \ | 10000 |  426 |   442 |   435 |   429 |   447 |   454
  \ | 65535 | 2786 |  2895 |  2852 |  2812 |  2927 |  2956
  \ |=====================================================

[then]

  \ ===========================================================
  cr .( Greeting) ?depth debug-point \ {{{1

cls .( Nuclear Waste Invaders)
cr version$ type
cr .( Loaded)

cr cr greeting

cr cr .( Type RUN to start) cr

[breakable] 0= ?\ cr .( The BREAK key quits the game) cr

end-program

  \ vim: filetype=soloforth:colorcolumn=64:textwidth=63
