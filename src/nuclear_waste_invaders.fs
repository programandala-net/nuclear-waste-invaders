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

: version$ ( -- ca len ) s" 0.225.2+201802191904" ;

cr cr .( Nuclear Waste Invaders) cr version$ type cr

  \ ===========================================================
  cr .( Options) \ {{{1

  \ Flags for conditional compilation of new features under
  \ development.

true constant [breakable] immediate
  \ Compile code to make the program breakable with the BREAK
  \ key combination?
  \
  \ XXX TMP -- for debugging

false constant [debugging] immediate
  \ Compile debugging code?
  \
  \ XXX TMP -- for debugging

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
need cond need thens need default-of

  \ --------------------------------------------
  cr .(   -Memory) ?depth \ {{{2

need c+! need c-! need c1+! need c1-! need ?c1-! need coff
need dzx7t need bank-start need c@1+ need c@1- need c@2+
need 1+! need c@2- need con

  \ --------------------------------------------
  cr .(   -Math) ?depth \ {{{2

need d< need -1|1 need 2/ need between need random need binary
need within need even? need 8* need random-between need m+
need join need 3* need polarity need <=> need -1..1

  \ --------------------------------------------
  cr .(   -Data structures) ?depth \ {{{2

need roll need cfield: need field: need +field-opt-0124
need array> need !> need c!> need 2!> need 0dup need 2field:

need sconstants

need xstack need allot-xstack need xdepth need >x need x>
need xclear

  \ --------------------------------------------
  cr .(   -Display) ?depth \ {{{2

need at-y need at-x need type-left-field need type-right-field
need type-center-field need gigatype-title need mode-32iso

  \ --------------------------------------------
  cr .(   -Graphics) ?depth \ {{{2

need columns need rows need row need fade-display
need last-column need blackout

need ,udg-block need /udg+ need /udg

need window need wltype need wcr need wcls need wcolor

need xy>attr

need inverse-off need overprint-off need attr! need attr@

need black need blue  need red  need magenta need green
need cyan need yellow need white

need papery need brighty need xy>attr need xy>attra
need bright-mask need inversely need unbright-mask

  \ --------------------------------------------
  cr .(   -Keyboard) ?depth \ {{{2

need kk-ports need kk-1# need pressed? need kk-chars
need kk#>kk need inkey need new-key

  \ --------------------------------------------
  cr .(   -Time) ?depth \ {{{2

need dticks need ms need seconds need ?seconds need dpast?
need reset-dticks

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

rows 1- cconstant debug-bar-y

 0 cconstant depth-x
 6 cconstant rdepth-x
12 cconstant mothership-x-x

white cconstant debug-attr

: .depth ( -- )
  debug-attr attr!
  depth-x debug-bar-y at-xy ." S:" depth . space ;

: .rdepth ( -- )
  debug-attr attr!
  rdepth-x debug-bar-y at-xy ." R:" rdepth . space ;

variable mothership-x

: .mothership-x ( -- )
  debug-attr attr! mothership-x-x debug-bar-y at-xy
  ." M:" mothership-x @ . space ;

: debug-bar ( -- ) .depth .rdepth .mothership-x ;

'd' cconstant debug-key

: debug-key? ( -- f ) key? 0dup if key debug-key = then ;

: ?debug-bar ( -- ) debug-key? 0exit debug-bar ;

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

                         white cconstant tank-attr

                   red brighty cconstant bullet-attr
                 white brighty cconstant missile-attr
                  blue brighty cconstant ball-attr

                         green cconstant healthy-invader-attr
                        yellow cconstant wounded-invader-attr
                           red cconstant dying-invader-attr
   ball-attr unbright-mask and cconstant balled-invader-attr

                  white papery cconstant unfocus-attr
  white papery brighty white + cconstant hide-report-attr
          white papery brighty cconstant reveal-report-attr
                         white cconstant text-attr
                         white cconstant status-attr
         status-attr inversely cconstant ammo-attr

                           red cconstant brick-attr
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

0. 2constant kk-left
0. 2constant kk-right
0. 2constant kk-fire
0. 2constant kk-recharge
0. 2constant kk-previous-gun
0. 2constant kk-next-gun
0. 2constant kk-bullet-gun
0. 2constant kk-missile-gun
0. 2constant kk-ball-gun
  \ Constants to store the bitmask and port address of the
  \ keys used in the game.

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

  \ Controls data

0
  cfield: ~keyset-left
  cfield: ~keyset-right
  cfield: ~keyset-fire
  cfield: ~keyset-recharge
  cfield: ~keyset-next-gun
  cfield: ~keyset-previous-gun
  cfield: ~keyset-bullet-gun
  cfield: ~keyset-missile-gun
  cfield: ~keyset-ball-gun
cconstant /keyset

9 cconstant #keysets
  \ Number of keysets.

create keysets #keysets /keyset * allot
  \ Data table for keysets.

cvariable keyset#
  \ Number of the current keyset in table `keysets`.

0 constant keyset~
  \ Data address of the current keyset in table `keysets`.

: keyset#>~ ( n -- a ) /keyset * keysets + ;
  \ Convert keyset number _n_ to its data address _a_.

  \ next-gun previous-gun recharge fire right left
  \ next-gun previous-gun recharge fire right left

0 keyset#>~ !> keyset~

kk-5#  keyset~ ~keyset-left c!
kk-8#  keyset~ ~keyset-right c!
kk-en# keyset~ ~keyset-fire c!
kk-sp# keyset~ ~keyset-recharge c!
kk-7#  keyset~ ~keyset-next-gun c!
kk-6#  keyset~ ~keyset-previous-gun c!
kk-m#  keyset~ ~keyset-bullet-gun c!
kk-w#  keyset~ ~keyset-missile-gun c!
kk-v#  keyset~ ~keyset-ball-gun c!
  \ Kinesis Advantage keyboard with Spanish Dvorak layout.

1 keyset#>~ !> keyset~

kk-r#  keyset~ ~keyset-left c!
kk-t#  keyset~ ~keyset-right c!
kk-sp# keyset~ ~keyset-fire c!
kk-en# keyset~ ~keyset-recharge c!
kk-i#  keyset~ ~keyset-next-gun c!
kk-a#  keyset~ ~keyset-previous-gun c!
kk-o#  keyset~ ~keyset-bullet-gun c!
kk-e#  keyset~ ~keyset-missile-gun c!
kk-e#  keyset~ ~keyset-ball-gun c!
    \ Spanish Dvorak layout.

2 keyset#>~ !> keyset~

kk-z# keyset~ ~keyset-left c!
kk-c# keyset~ ~keyset-right c!
kk-x# keyset~ ~keyset-fire c!
kk-v# keyset~ ~keyset-recharge c!
kk-q# keyset~ ~keyset-next-gun c!
kk-a# keyset~ ~keyset-previous-gun c!
kk-s# keyset~ ~keyset-bullet-gun c!
kk-d# keyset~ ~keyset-missile-gun c!
kk-f# keyset~ ~keyset-ball-gun c!
    \ QWERTY

3 keyset#>~ !> keyset~

kk-5#  keyset~ ~keyset-left c!
kk-8#  keyset~ ~keyset-right c!
kk-0#  keyset~ ~keyset-fire c!
kk-sp# keyset~ ~keyset-recharge c!
kk-7#  keyset~ ~keyset-next-gun c!
kk-6#  keyset~ ~keyset-previous-gun c!
kk-1#  keyset~ ~keyset-bullet-gun c!
kk-2#  keyset~ ~keyset-missile-gun c!
kk-3#  keyset~ ~keyset-ball-gun c!
    \ cursor joystick + space + digits

4 keyset#>~ !> keyset~

kk-5#  keyset~ ~keyset-left c!
kk-8#  keyset~ ~keyset-right c!
kk-7#  keyset~ ~keyset-fire c!
kk-6#  keyset~ ~keyset-recharge c!
kk-9#  keyset~ ~keyset-next-gun c!
kk-0#  keyset~ ~keyset-previous-gun c!
kk-1#  keyset~ ~keyset-bullet-gun c!
kk-2#  keyset~ ~keyset-missile-gun c!
kk-3#  keyset~ ~keyset-ball-gun c!
    \ cursor+digits

5 keyset#>~ !> keyset~

kk-1# keyset~ ~keyset-left c!
kk-2# keyset~ ~keyset-right c!
kk-5# keyset~ ~keyset-fire c!
kk-q# keyset~ ~keyset-recharge c!
kk-4# keyset~ ~keyset-next-gun c!
kk-3# keyset~ ~keyset-previous-gun c!
kk-w# keyset~ ~keyset-bullet-gun c!
kk-e# keyset~ ~keyset-missile-gun c!
kk-r# keyset~ ~keyset-ball-gun c!
    \ Sinclair 1 + QWERTY

6 keyset#>~ !> keyset~

kk-6# keyset~ ~keyset-left c!
kk-7# keyset~ ~keyset-right c!
kk-0# keyset~ ~keyset-fire c!
kk-u# keyset~ ~keyset-recharge c!
kk-9# keyset~ ~keyset-next-gun c!
kk-8# keyset~ ~keyset-previous-gun c!
kk-i# keyset~ ~keyset-bullet-gun c!
kk-o# keyset~ ~keyset-missile-gun c!
kk-p# keyset~ ~keyset-ball-gun c!
    \ Sinclair 2 + QWERTY

7 keyset#>~ !> keyset~

kk-o#  keyset~ ~keyset-left c!
kk-p#  keyset~ ~keyset-right c!
kk-sp# keyset~ ~keyset-fire c!
kk-en# keyset~ ~keyset-recharge c!
kk-q#  keyset~ ~keyset-next-gun c!
kk-a#  keyset~ ~keyset-previous-gun c!
kk-1#  keyset~ ~keyset-bullet-gun c!
kk-2#  keyset~ ~keyset-missile-gun c!
kk-3#  keyset~ ~keyset-ball-gun c!
    \ QWERTY

8 keyset#>~ !> keyset~

kk-n# keyset~ ~keyset-left c!
kk-m# keyset~ ~keyset-right c!
kk-v# keyset~ ~keyset-fire c!
kk-b# keyset~ ~keyset-recharge c!
kk-q# keyset~ ~keyset-next-gun c!
kk-a# keyset~ ~keyset-previous-gun c!
kk-1# keyset~ ~keyset-bullet-gun c!
kk-2# keyset~ ~keyset-missile-gun c!
kk-3# keyset~ ~keyset-ball-gun c!
    \ QWERTY

: set-keyset ( n -- )
  dup keyset# c! keyset#>~ !> keyset~
  keyset~ ~keyset-left         c@ kk#>kk 2!> kk-left
  keyset~ ~keyset-right        c@ kk#>kk 2!> kk-right
  keyset~ ~keyset-fire         c@ kk#>kk 2!> kk-fire
  keyset~ ~keyset-recharge     c@ kk#>kk 2!> kk-recharge
  keyset~ ~keyset-previous-gun c@ kk#>kk 2!> kk-previous-gun
  keyset~ ~keyset-next-gun     c@ kk#>kk 2!> kk-next-gun
  keyset~ ~keyset-bullet-gun   c@ kk#>kk 2!> kk-bullet-gun
  keyset~ ~keyset-missile-gun  c@ kk#>kk 2!> kk-missile-gun
  keyset~ ~keyset-ball-gun     c@ kk#>kk 2!> kk-ball-gun ;
  \ Make keyset number _n_ the current one.

0 set-keyset

: next-keyset ( -- )
  keyset# c@1+ dup #keysets < and set-keyset ;
  \ Change the current keyset to the next one, in a circular
  \ way.

  \ ===========================================================
  cr .( Status bar, part 1) ?depth debug-point \ {{{1

                                 2 cconstant ammo-digits
                                 4 cconstant score-digits

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
  at-xy 0 <# [ score-digits ] [#] #> text-attr attr! type ;
  \ Display score _n_ at coordinates _col row_.

' xdepth alias projectiles-left ( -- n )

0 cconstant ammo-x
  \ Column of the current gun's ammo figure at the status bar.

: (.ammo ( n -- )
  projectiles-left 0 <# [ ammo-digits ] [#] #>
  ammo-x status-bar-y at-xy type ;
  \ Display the current ammo left, with the current attribute.

: .ammo ( n -- ) ammo-attr attr! (.ammo ;
  \ Display the current ammo left.

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

: emits-udga ( ca n -- ) 0 ?do dup emit-udga loop drop ;

: emit-2udga ( ca -- ) dup emit-udga /udg+ emit-udga ;

: ,udg-block: ( width height "name" "ccc" -- )
  here [ 3 cell + ] cliteral + constant ,udg-block ;
  \ Compile a UDG block _ccc_ of size _width height_ named
  \ _name_. When _name_ is later executed, the address of the
  \ block is returned. Note: `here` is adjusted in order
  \ to skip the space used by the constant: A constant uses 3
  \ bytes for the code field, plus one cell for the value.

2 cconstant udg/invader
2 cconstant udg/mothership

4 cconstant undocked-invader-frames
3 cconstant   docked-invader-frames

rom-font bl /udg * + constant bl-udga
  \ Address of the ROM font's space, to be used directly as
  \ an UDG.

: sprite>udgs ( ca -- n ) here swap - /udg / ;
  \ Convert the address _ca_ of the first UDG of the lastest
  \ sprite defined to the number _n_ of UDGs used by the
  \ sprite.

  \ -----------------------------------------------------------
  \ Invader species 0

  \ invader species 0, left flying, frame 0:

udg/invader 1 ,udg-block: left-flying-species-0-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXX..XX..XXX..
..XXXXXXXXXXXX..
.....XX..XX.....
....XX.XX.XX....
..XX........XX..

  \ invader species 0, left flying, frame 1:

udg/invader 1 ,udg-block

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XX..XX..XXXX..
..XXXXXXXXXXXX..
....XXX..XXX....
...XX..XX..XX...
...XX.......XX..

  \ invader species 0, left flying, frame 2:

udg/invader 1 ,udg-block

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..X..XX..XXXXX..
..XXXXXXXXXXXX..
....XXX..XXX....
...XX..XX..XX...
....XX......XX..

  \ invader species 0, left flying, frame 3:

udg/invader 1 ,udg-block

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XX..XX..XXXX..
..XXXXXXXXXXXX..
....XXX..XXX....
...XX..XX..XX...
...XX.......XX..

  \ invader species 0, right flying, frame 0:

udg/invader 1 ,udg-block: right-flying-species-0-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXX..XX..XXX..
..XXXXXXXXXXXX..
.....XX..XX.....
....XX.XX.XX....
..XX........XX..

  \ invader species 0, right flying, frame 1:

udg/invader 1 ,udg-block

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXXX..XX..XX..
..XXXXXXXXXXXX..
....XXX..XXX....
...XX..XX..XX...
..XX.......XX...

  \ invader species 0, right flying, frame 2:

udg/invader 1 ,udg-block

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXXXX..XX..X..
..XXXXXXXXXXXX..
....XXX..XXX....
...XX..XX..XX...
..XX......XX....

  \ invader species 0, right flying, frame 3:

udg/invader 1 ,udg-block

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXXX..XX..XX..
..XXXXXXXXXXXX..
....XXX..XXX....
...XX..XX..XX...
..XX.......XX...

  \ invader species 0, docked, frame 0:

udg/invader 1 ,udg-block: docked-species-0-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXX..XX..XXX..
..XXXXXXXXXXXX..
.....XX..XX.....
....XX.XX.XX....
..XX........XX..

  \ invader species 0, docked, frame 1:

udg/invader 1 ,udg-block

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXX..XX..XXX..
..XXXXXXXXXXXX..
.....XX..XX.....
....XX.XX.XX....
..XX........XX..

  \ invader species 0, docked, frame 2:

udg/invader 1 ,udg-block

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXXXXXXXXXXX..
..XXXXXXXXXXXX..
.....XX..XX.....
....XX.XX.XX....
..XX........XX..

  \ invader species 0, left breaking, frame 0:

udg/invader 1 ,udg-block: left-breaking-species-0-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXX..XX..XXX..
..XXXXXXXXXXXX..
.....XX..XX.....
....XX.XX.XX....
..XX........XX..

  \ invader species 0, left breaking, frame 1:

udg/invader 1 ,udg-block

.....XXXX.......
..XXXXXXXXXX....
.XXXXXXXXXXXX...
.XX..XX..XXXX...
.XXXXXXXXXXXX...
...XXX..XXX.....
..XX..XX..XX....
..XX.......XX...

  \ invader species 0, left breaking, frame 2:

udg/invader 1 ,udg-block

....XXXX........
.XXXXXXXXXX.....
XXXXXXXXXXXX....
X..XX..XXXXX....
XXXXXXXXXXXX....
..XXX..XXX......
.XX..XX..XX.....
..XX......XX....

  \ invader species 0, left breaking, frame 3:

udg/invader 1 ,udg-block

.....XXXX.......
..XXXXXXXXXX....
.XXXXXXXXXXXX...
.XX..XX..XXXX...
.XXXXXXXXXXXX...
...XXX..XXX.....
..XX..XX..XX....
..XX.......XX...

  \ invader species 0, right breaking, frame 0:

udg/invader 1 ,udg-block: right-breaking-species-0-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXX..XX..XXX..
..XXXXXXXXXXXX..
.....XX..XX.....
....XX.XX.XX....
..XX........XX..

  \ invader species 0, right breaking, frame 1:

udg/invader 1 ,udg-block

.......XXXX.....
....XXXXXXXXXX..
...XXXXXXXXXXXX.
...XXXX..XX..XX.
...XXXXXXXXXXXX.
.....XXX..XXX...
....XX..XX..XX..
...XX.......XX..

  \ invader species 0, right breaking, frame 2:

udg/invader 1 ,udg-block

........XXXX....
.....XXXXXXXXXX.
....XXXXXXXXXXXX
....XXXXX..XX..X
....XXXXXXXXXXXX
......XXX..XXX..
.....XX..XX..XX.
....XX......XX..

  \ invader species 0, right breaking, frame 3:

udg/invader 1 ,udg-block

.......XXXX.....
....XXXXXXXXXX..
...XXXXXXXXXXXX.
...XXXX..XX..XX.
...XXXXXXXXXXXX.
.....XXX..XXX...
....XX..XX..XX..
...XX.......XX..

  \ -----------------------------------------------------------
  \ Invader species 1

  \ invader species 1, left flying, frame 0:

udg/invader 1 ,udg-block: left-flying-species-1-sprite

......X...X.....
.....X...X......
....XXXXXXX.....
...XX.XXX.XX....
..XXXXXXXXXXX...
..XXXXXXXXXXX...
..X.X.....X.X...
....X.....X.....

  \ invader species 1, left flying, frame 1:

udg/invader 1 ,udg-block

......X...X.....
.....X...X......
....XXXXXXX.....
XXXX.XXX.XXXXXX.
..XXXXXXXXXXX...
...XXXXXXXXX....
....X.....X.....
.....X.....X....

  \ invader species 1, left flying, frame 2:

udg/invader 1 ,udg-block

......X...X.....
..X..X...X..X...
..X.XXXXXXX.X...
..XXX.XXX.XXX...
..XXXXXXXXXXX...
...XXXXXXXXX....
....X.....X.....
....X.....X.....

  \ invader species 1, left flying, frame 3:

udg/invader 1 ,udg-block

......X...X.....
.....X...X......
....XXXXXXX.....
XXXX.XXX.XXXXXX.
..XXXXXXXXXXX...
...XXXXXXXXX....
....X.....X.....
.....X.....X....

  \ invader species 1, right flying, frame 0:

udg/invader 1 ,udg-block: right-flying-species-1-sprite

.....X...X......
......X...X.....
.....XXXXXXX....
....XX.XXX.XX...
...XXXXXXXXXXX..
...XXXXXXXXXXX..
...X.X.....X.X..
.....X.....X....

  \ invader species 1, right flying, frame 1:

udg/invader 1 ,udg-block

.....X...X......
......X...X.....
.....XXXXXXX....
.XXXXXX.XXX.XXXX
...XXXXXXXXXXX..
....XXXXXXXXX...
.....X.....X....
....X.....X.....

  \ invader species 1, right flying, frame 2:

udg/invader 1 ,udg-block

.....X...X......
...X..X...X..X..
...X.XXXXXXX.X..
...XXX.XXX.XXX..
...XXXXXXXXXXX..
....XXXXXXXXX...
.....X.....X....
.....X.....X....

  \ invader species 1, right flying, frame 3:

udg/invader 1 ,udg-block

.....X...X......
......X...X.....
.....XXXXXXX....
.XXXXXX.XXX.XXXX
...XXXXXXXXXXX..
....XXXXXXXXX...
.....X.....X....
....X.....X.....

  \ invader species 1, docked, frame 0:

udg/invader 1 ,udg-block: docked-species-1-sprite

....X.....X.....
.....X...X......
....XXXXXXX.....
...XX.XXX.XX....
..XXXXXXXXXXX...
..XXXXXXXXXXX...
..X.X.....X.X...
.....XX.XX......

  \ invader species 1, docked, frame 1:

udg/invader 1 ,udg-block

....X.....X.....
.....X...X......
....XXXXXXX.....
...XX.XXX.XX....
..XXXXXXXXXXX...
..XXXXXXXXXXX...
..X.X.....X.X...
.....XX.XX......

  \ invader species 1, docked, frame 2:

udg/invader 1 ,udg-block

....X.....X.....
.....X...X......
....XXXXXXX.....
...XXXXXXXXX....
..XXXXXXXXXXX...
..XXXXXXXXXXX...
..X.X.....X.X...
.....XX.XX......

  \ invader species 1, left breaking, frame 0:

udg/invader 1 ,udg-block: left-breaking-species-1-sprite

.....X...X......
.....X...X......
....XXXXXXX.....
...XX.XXX.XX....
..XXXXXXXXXXX...
..XXXXXXXXXXX...
..X.X.....X.X...
....X.....X.....

  \ invader species 1, left breaking, frame 1:

udg/invader 1 ,udg-block

.....X...X......
....X...X.......
...XXXXXXX......
..X.XXX.XXXXXX..
.XXXXXXXXXXX....
.XXXXXXXXXX.....
..XX.....X......
....X.....X.....

  \ invader species 1, left breaking, frame 2:

udg/invader 1 ,udg-block

....X...X.......
...X...X..X.....
..XXXXXXX.X.....
.XX.XXX.XXX.....
XXXXXXXXXXX.....
XXXXXXXXXX......
.XX.....X.......
..X.....X.......

  \ invader species 1, left breaking, frame 3:

udg/invader 1 ,udg-block

.....X...X......
....X...X.......
...XXXXXXX......
..X.XXX.XXXXXX..
.XXXXXXXXXXX....
.XXXXXXXXXX.....
..XX.....X......
....X.....X.....

  \ invader species 1, right breaking, frame 0:

udg/invader 1 ,udg-block: right-breaking-species-1-sprite

......X...X.....
......X...X.....
.....XXXXXXX....
....XX.XXX.XX...
...XXXXXXXXXXX..
...XXXXXXXXXXX..
...X.X.....X.X..
.....X.....X....

  \ invader species 1, right breaking, frame 1:

udg/invader 1 ,udg-block

......X...X.....
.......X...X....
......XXXXXXX...
..XXXXXX.XXX.X..
....XXXXXXXXXXX.
.....XXXXXXXXXX.
......X.....XX..
.....X.....X....

  \ invader species 1, right breaking, frame 2:

udg/invader 1 ,udg-block

.......X...X....
.....X..X...X...
.....X.XXXXXXX..
.....XXX.XXX.XX.
.....XXXXXXXXXXX
......XXXXXXXXXX
.......X.....XX.
.......X.....X..

  \ invader species 1, right breaking, frame 3:

udg/invader 1 ,udg-block

......X...X.....
.......X...X....
......XXXXXXX...
..XXXXXX.XXX.X..
....XXXXXXXXXXX.
.....XXXXXXXXXX.
......X.....XX..
.....X.....X....

  \ -----------------------------------------------------------
  \ Invader species 2

  \ invader species 2, left flying, frame 0:

udg/invader 1 ,udg-block: left-flying-species-2-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....XX.XX.XX....
....XXXXXXXX....
......X..X......
.....X.XX.X.....
....X.X..X.X....

  \ invader species 2, left flying, frame 1:

udg/invader 1 ,udg-block

.......XX.......
......XXXX......
.....XXXXXX.....
....X.XX.XXX....
....XXXXXXXX....
......X..X......
......X.XX.X....
.....X.X..X.X...

  \ invader species 2, left flying, frame 2:

udg/invader 1 ,udg-block

.......XX.......
......XXXX......
.....XXXXXX.....
....XX.XX.XX....
....XXXXXXXX....
......X..X......
.......X.XX.X...
......X.X..X.X..

  \ invader species 2, left flying, frame 3:

udg/invader 1 ,udg-block

.......XX.......
......XXXX......
.....XXXXXX.....
....X.XX.XXX....
....XXXXXXXX....
......X..X......
......X.XX.X....
.....X.X..X.X...

  \ invader species 2, right flying, frame 0:

udg/invader 1 ,udg-block: right-flying-species-2-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....XX.XX.XX....
....XXXXXXXX....
......X..X......
.....X.XX.X.....
....X.X..X.X....

  \ invader species 2, right flying, frame 1:

udg/invader 1 ,udg-block

.......XX.......
......XXXX......
.....XXXXXX.....
....XXX.XX.X....
....XXXXXXXX....
......X..X......
....X.XX.X......
...X.X..X.X.....

  \ invader species 2, right flying, frame 2:

udg/invader 1 ,udg-block

.......XX.......
......XXXX......
.....XXXXXX.....
....XX.XX.XX....
....XXXXXXXX....
......X..X......
...X.XX.X.......
..X.X..X.X......

  \ invader species 2, right flying, frame 3:

udg/invader 1 ,udg-block

.......XX.......
......XXXX......
.....XXXXXX.....
....XXX.XX.X....
....XXXXXXXX....
......X..X......
....X.XX.X......
...X.X..X.X.....

  \ invader species 2, docked, frame 0:

udg/invader 1 ,udg-block: docked-species-2-sprite

.......XX.......
......XXXX......
.....XXXXXX.....
....XX.XX.XX....
....XXXXXXXX....
......X..X......
.....X.XX.X.....
....X.X..X.X....

  \ invader species 2, docked, frame 1:

udg/invader 1 ,udg-block

.......XX.......
......XXXX......
.....XXXXXX.....
....XX.XX.XX....
....XXXXXXXX....
......X..X......
.....X.XX.X.....
....X.X..X.X....

  \ invader species 2, docked, frame 2:

udg/invader 1 ,udg-block

.......XX.......
......XXXX......
.....XXXXXX.....
....XXXXXXXX....
....XXXXXXXX....
......X..X......
.....X.XX.X.....
....X.X..X.X....

  \ invader species 2, left breaking, frame 0:

udg/invader 1 ,udg-block: left-breaking-species-2-sprite

.....XX.........
....XXXX........
...XXXXXX.......
..XX.XX.XX......
..XXXXXXXX......
....X..X........
...X.XX.X.......
..X.X..X.X......

  \ invader species 2, left breaking, frame 1:

udg/invader 1 ,udg-block

....XX..........
...XXXX.........
..XXXXXX........
.X.XX.XXX.......
.XXXXXXXX.......
...X..X.........
...X.XX.X.......
..X.X..X.X......

  \ invader species 2, left breaking, frame 2:

udg/invader 1 ,udg-block

...XX...........
..XXXX..........
.XXXXXX.........
XX.XX.XX........
XXXXXXXX........
..X..X..........
...X.XX.X.......
..X.X..X.X......

  \ invader species 2, left breaking, frame 3:

udg/invader 1 ,udg-block

....XX..........
...XXXX.........
..XXXXXX........
.X.XX.XXX.......
.XXXXXXXX.......
...X..X.........
...X.XX.X.......
..X.X..X.X......

  \ invader species 2, right breaking, frame 0:

udg/invader 1 ,udg-block: right-breaking-species-2-sprite

.........XX.....
........XXXX....
.......XXXXXX...
......XX.XX.XX..
......XXXXXXXX..
........X..X....
.......X.XX.X...
......X.X..X.X..

  \ invader species 2, right breaking, frame 1:

udg/invader 1 ,udg-block

..........XX....
.........XXXX...
........XXXXXX..
.......XXX.XX.X.
.......XXXXXXXX.
.........X..X...
.......X.XX.X...
......X.X..X.X..

  \ invader species 2, right breaking, frame 2:

udg/invader 1 ,udg-block

...........XX...
..........XXXX..
.........XXXXXX.
........XX.XX.XX
........XXXXXXXX
..........X..X..
.......X.XX.X...
......X.X..X.X..

  \ invader species 2, right breaking, frame 3:

udg/invader 1 ,udg-block

..........XX....
.........XXXX...
........XXXXXX..
.......XXX.XX.X.
.......XXXXXXXX.
.........X..X...
.......X.XX.X...
......X.X..X.X..

  \ -----------------------------------------------------------
  \ Mothership

0  constant mothership
  \ Address of the first UDG sprite frame of the current sprite
  \ of the mothership.

cvariable mothership-frame
  \ Current frame of the current sprite of the mothership.

0 cconstant mothership-frames
  \ Number of frames of the current sprite of the mothership.

  \ ............................................
  \ Flying mothership

  \ mothership, frame 0:

udg/mothership 1 ,udg-block: flying-mothership-sprite

................
.....XXXXXX.....
...XXXXXXXXXX...
..XXXXXXXXXXXX..
.XX..XX..XX..XX.
XXXXXXXXXXXXXXXX
..XXX..XX..XXX..
...X........X...

  \ mothership, frame 1:

udg/mothership 1 ,udg-block

................
.....XXXXXX.....
...XXXXXXXXXX...
..XXXXXXXXXXXX..
.X..XX..XX..XX..
XXXXXXXXXXXXXXXX
..XXX..XX..XXX..
...X........X...

  \ mothership, frame 2:

udg/mothership 1 ,udg-block

................
.....XXXXXX.....
...XXXXXXXXXX...
..XXXXXXXXXXXX..
...XX..XX..XX...
XXXXXXXXXXXXXXXX
..XXX..XX..XXX..
...X........X...

  \ mothership, frame 3:

udg/mothership 1 ,udg-block

................
.....XXXXXX.....
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XX..XX..XX..X.
XXXXXXXXXXXXXXXX
..XXX..XX..XXX..
...X........X...

flying-mothership-sprite
sprite>udgs udg/mothership / cconstant flying-mothership-frames

  \ ............................................
  \ Beaming mothership

udg/mothership 1 ,udg-block: beaming-mothership-sprite

................
.....XXXXXX.....
...XXXXXXXXXX...
..XXXXXXXXXXXX..
.XX..XX..XX..XX.
XXXXXXXXXXXXXXXX
.X.X.X.XX.X.X.X.
X.X.X.X..X.X.X.X

udg/mothership 1 ,udg-block

................
.....XXXXXX.....
...XXXXXXXXXX...
..XXXXXXXXXXXX..
.XX..XX..XX..XX.
XXXXXXXXXXXXXXXX
X.X.X.X..X.X.X.X
.X.X.X.XX.X.X.X.

beaming-mothership-sprite sprite>udgs udg/mothership /
cconstant beaming-mothership-frames

  \ -----------------------------------------------------------
  \ Explosion

2 cconstant udg/explosion

udg/explosion 1 ,udg-block: explosion-sprite

..............X.
..X......XX..X..
.X...XXXXXXX....
....XXXXXXXXX.X.
...XXXX.XX.XX..X
.X..XX..XXXX....
X....XXXXX...X..
..X...XX...X..X.

udg/explosion 1 ,udg-block

................
...X...X...X....
.....XXXXXXX..X.
.X..X.XXXX.XX...
...XX.XXX.XXX...
..X.X.XXXX.X..X.
.X...XX.XX..X...
.X...XX...X..X.X

udg/explosion 1 ,udg-block

.X...X..........
X...X....X...X..
...X.XXXX.XX....
...XXXX.XX.XX..X
X...X.XXX.X.XX..
....X.XX.X.XX..X
...X.XX.XXX.XX..
X...XX.X..X.....

udg/explosion 1 ,udg-block

X......X........
....X....X......
...X.X.XX.X.....
X..X.X...X.XX..X
....X.X.X.X.XX..
..X...X..X.XX...
...X.XX.X.X.XX..
.X..X..........X

udg/explosion 1 ,udg-block

..X.............
....X....X..X...
X..X.X.X..X..X..
...X..X..X......
....X.....X.X...
.X....X..X.X..X.
...X.X....X.....
....X....X...X..

udg/explosion 1 ,udg-block

X............X..
....X....X......
.......X..X..X..
..X...........X.
..........X.....
.X....X.......X.
..........X.....
..X.......X....X

udg/explosion 1 ,udg-block

.X.........X....
.....X.....X..X.
...............X
................
X...............
...........X....
X.....X........X
..X........X....

udg/explosion 1 ,udg-block

....X.......X..X
................
................
................
................
...........X....
.............X..
......X.........

udg/explosion 1 ,udg-block

................
................
................
................
................
................
................
................

explosion-sprite
sprite>udgs udg/explosion / cconstant explosion-frames

  \ --------------------------------------------
  \ Projectiles

  \ ............................
  \ Bullet

1 1 ,udg-block: bullet-sprite

..X.....
.....X..
..X.....
.....X..
..X.....
.....X..
..X.....
.....X..

1 1 ,udg-block

.....X..
..X.....
.....X..
..X.....
.....X..
..X.....
.....X..
..X.....

1 1 ,udg-block

..X.....
..X..X..
.....X..
..X.....
..X..X..
.....X..
..X.....
..X..X..

1 1 ,udg-block

.....X..
..X..X..
..X.....
.....X..
..X..X..
..X.....
.....X..
..X..X..

1 1 ,udg-block

..X.....
........
..X..X..
........
.....X..
..X.....
........
..X..X..

1 1 ,udg-block

.....X..
........
..X..X..
........
..X.....
.....X..
........
..X..X..

1 1 ,udg-block

..X..X..
.....X..
..X.....
........
..X..X..
.....X..
..X.....
........

1 1 ,udg-block

..X..X..
..X.....
.....X..
........
..X..X..
..X.....
.....X..
........

1 1 ,udg-block

..X.....
..X..X..
........
..X..X..
..X.....
.....X..
..X.....
..X..X..

1 1 ,udg-block

.....X..
..X..X..
........
..X..X..
.....X..
..X.....
.....X..
..X..X..

bullet-sprite sprite>udgs cconstant bullet-frames

  \ ............................
  \ Missile

1 1 ,udg-block: missile-sprite

...XX...
...XX...
...XX...
..XXXX..
..XXXX..
..XXXX..
...X.X..
..X.X...

1 1 ,udg-block

...XX...
...XX...
...XX...
..XXXX..
..XXXX..
..XXXX..
..X.X...
...X.X..

missile-sprite sprite>udgs cconstant missile-frames

  \ ............................
  \ Ball

1 1 ,udg-block: ball-sprite

..XXXX..
.XXXXXX.
XXXXX.XX
XXXXXX.X
XXXXXXXX
XXXXXXXX
.XXXXXX.
..XXXX..

1 1 ,udg-block

..XXXX..
.XXXXXX.
XXXXXXXX
XXXXXXXX
XXXXXX.X
XXXXX.XX
.XXXXXX.
..XXXX..

1 1 ,udg-block

..XXXX..
.XXXXXX.
XXXXXXXX
XXXXXXXX
XXXXXXXX
XXX..XXX
.XXXXXX.
..XXXX..

1 1 ,udg-block

..XXXX..
.XXXXXX.
XXXXXXXX
X.XXXXXX
XX.XXXXX
XXXXXXXX
.XXXXXX.
..XXXX..

1 1 ,udg-block

..XXXX..
.XX.XXX.
XX.XXXXX
XXXXXXXX
XXXXXXXX
XXXXXXXX
.XXXXXX.
..XXXX..

ball-sprite sprite>udgs cconstant ball-frames

1 1 ,udg-block: wall>-ball-sprite

XXXX.....
XXXXX....
XXX.XX...
XXXX.X...
XXXXXX...
XXXXXX...
XXXXX....
XXXX.....

1 1 ,udg-block

XXXX.....
XXXXX....
XXXXXX...
XXXXXX...
XXXX.X...
XXX.XX...
XXXXX....
XXXX.....

1 1 ,udg-block

XXXX.....
XXXXX....
XXXXXX...
XXXXXX...
XXXXXX...
X..XXX...
XXXXX....
XXXX.....

1 1 ,udg-block

XXXX.....
XXXXX....
XXXXXX...
XXXXXX...
.XXXXX...
XXXXXX...
XXXXX....
XXXX.....

1 1 ,udg-block

XXXX.....
X.XXX....
.XXXXX...
XXXXXX...
XXXXXX...
XXXXXX...
XXXXX....
XXXX.....

wall>-ball-sprite sprite>udgs cconstant wall>-ball-frames

1 1 ,udg-block: <wall-ball-sprite

....XXXX
...XXXXX
..XXXXX.
..XXXXXX
..XXXXXX
..XXXXXX
...XXXXX
....XXXX

1 1 ,udg-block

....XXXX
...XXXXX
..XXXXXX
..XXXXXX
..XXXXXX
..XXXXX.
...XXXXX
....XXXX

1 1 ,udg-block

....XXXX
...XXXXX
..XXXXXX
..XXXXXX
..XXXXXX
..XXX..X
...XXXXX
....XXXX

1 1 ,udg-block

....XXXX
...XXXXX
..XXXXXX
..X.XXXX
..XX.XXX
..XXXXXX
...XXXXX
....XXXX

1 1 ,udg-block

....XXXX
...XX.XX
..XX.XXX
..XXXXXX
..XXXXXX
..XXXXXX
...XXXXX
....XXXX

<wall-ball-sprite sprite>udgs cconstant <wall-ball-frames

  \ ............................
  \ Explosion

  \ XXX TMP --

1 1 ,udg-block: projectile-explosion-sprite

........
........
..X.X...
...X.X..
..X.X...
...X.X..
........
........

1 1 ,udg-block

........
..X.X...
.....X..
..X...X.
.X.X.X..
..X...X.
.X...X..
........

1 1 ,udg-block

.X...X..
....X...
.....X..
X.X...X.
...X.X..
..X...X.
X....X..
..X...X.

1 1 ,udg-block

........
.X..X...
.....X..
X..X....
..X...X.
.X..X...
..X..X..
....X...

1 1 ,udg-block

........
....X...
..X..X..
...X..X.
..X..X..
...X..X.
..X..X..
........

1 1 ,udg-block

........
........
........
...X.X..
..X..X..
...XX...
........
........

1 1 ,udg-block

........
........
........
........
........
........
........
........

projectile-explosion-sprite
sprite>udgs cconstant projectile-explosion-frames

  \ -----------------------------------------------------------
  \ Building

1 1 ,udg-block: brick

XXXXX.XX
XXXXX.XX
XXXXX.XX
........
XX.XXXXX
XX.XXXXX
XX.XXXXX
........

1 1 ,udg-block: left-door

XXXXX.XX
.XXXX.XX
..XXX.XX
........
...XXXXX
...XXXXX
.X.XXXXX
XX.XXXXX

1 1 ,udg-block: right-door

XXXXX.XX
XXXXX.X.
XXXXX...
........
XX.XXX..
XX.XXX..
XX.XXXX.
XX.XXXXX

1 1 ,udg-block: broken-top-left-brick

XXXXX.XX
.XXXX.XX
.X.XX.XX
........
......XX
.....XXX
......X.
........

1 1 ,udg-block: broken-bottom-left-brick

........
.......X
.....XXX
........
....X.XX
.X.XXXXX
XX.XXXXX
........

1 1 ,udg-block: broken-top-right-brick

XXXXX.XX
XXXXX.XX
XXXXX...
........
XXX..X..
X.......
........
........

1 1 ,udg-block: broken-bottom-right-brick

........
X.......
X.X.....
........
XX.XXX..
XX.XX...
XX.XXX.X
........

  \ -----------------------------------------------------------
  \ Tank

4 cconstant tank-frames

tank-frames 1- cconstant tank-max-frame

0 constant tank-sprite
  \ The current sprite of the tank.

cvariable tank-frame \ counter (0..3)

3 cconstant udg/tank

udg/tank 1 ,udg-block: bullet-gun-tank-sprite
..........X..X..........
...XXXXXX.X..X.XXXXXXX..
..XXXXXXXXXXXXXXXXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XX.X.X.X.X.X.X.X.X.X.XX
..XX..XX..XX..XX..XX.XX.
...X.XXX.XXX.XXX.XXX.X..
....X.X.X.X.X.X.X.X.X...

udg/tank 1 ,udg-block
..........X..X..........
...XXXXXX.X..X.XXXXXXX..
..XXXXXXXXXXXXXXXXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XXX.X.X.X.X.X.X.X.X.XXX
..XX.XX..XX..XX..XX..XX.
...X.XXX.XXX.XXX.XXX.X..
...X.X.X.X.X.X.X.X.X.X..

udg/tank 1 ,udg-block
..........X..X..........
...XXXXXX.X..X.XXXXXXX..
..XXXXXXXXXXXXXXXXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XX.X.X.X.X.X.X.X.X.X.XX
..XX.XXX.XXX.XXX.XXX.XX.
...X.XX..XX..XX..XX..X..
....X.X.X.X.X.X.X.X.X...

udg/tank 1 ,udg-block
..........X..X..........
...XXXXXX.X..X.XXXXXXX..
..XXXXXXXXXXXXXXXXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XXX.X.X.X.X.X.X.X.X.XXX
..XX.XXX.XXX.XXX.XXX.XX.
...X..XX..XX..XX..XX.X..
...X.X.X.X.X.X.X.X.X.X..

udg/tank 1 ,udg-block: missile-gun-tank-sprite
..........XXXX..........
...XXXXXX.XXXX.XXXXXXX..
..XXXXXXX.XXXX.XXXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XX.X.X.X.X.X.X.X.X.X.XX
..XX..XX..XX..XX..XX.XX.
...X.XXX.XXX.XXX.XXX.X..
....X.X.X.X.X.X.X.X.X...

udg/tank 1 ,udg-block
..........XXXX..........
...XXXXXX.XXXX.XXXXXXX..
..XXXXXXX.XXXX.XXXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XXX.X.X.X.X.X.X.X.X.XXX
..XX.XX..XX..XX..XX..XX.
...X.XXX.XXX.XXX.XXX.X..
...X.X.X.X.X.X.X.X.X.X..

udg/tank 1 ,udg-block
..........XXXX..........
...XXXXXX.XXXX.XXXXXXX..
..XXXXXXX.XXXX.XXXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XX.X.X.X.X.X.X.X.X.X.XX
..XX.XXX.XXX.XXX.XXX.XX.
...X.XX..XX..XX..XX..X..
....X.X.X.X.X.X.X.X.X...

udg/tank 1 ,udg-block
..........XXXX..........
...XXXXXX.XXXX.XXXXXXX..
..XXXXXXX.XXXX.XXXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XXX.X.X.X.X.X.X.X.X.XXX
..XX.XXX.XXX.XXX.XXX.XX.
...X..XX..XX..XX..XX.X..
...X.X.X.X.X.X.X.X.X.X..

udg/tank 1 ,udg-block: ball-gun-tank-sprite
........XXXXXXXX........
...XXXX..XXXXXX..XXXXX..
..XXXXXX..XXXX..XXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XX.X.X.X.X.X.X.X.X.X.XX
..XX..XX..XX..XX..XX.XX.
...X.XXX.XXX.XXX.XXX.X..
....X.X.X.X.X.X.X.X.X...

udg/tank 1 ,udg-block
........XXXXXXXX........
...XXXX..XXXXXX..XXXXX..
..XXXXXX..XXXX..XXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XXX.X.X.X.X.X.X.X.X.XXX
..XX.XX..XX..XX..XX..XX.
...X.XXX.XXX.XXX.XXX.X..
...X.X.X.X.X.X.X.X.X.X..

udg/tank 1 ,udg-block
........XXXXXXXX........
...XXXX..XXXXXX..XXXXX..
..XXXXXX..XXXX..XXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XX.X.X.X.X.X.X.X.X.X.XX
..XX.XXX.XXX.XXX.XXX.XX.
...X.XX..XX..XX..XX..X..
....X.X.X.X.X.X.X.X.X...

udg/tank 1 ,udg-block
........XXXXXXXX........
...XXXX..XXXXXX..XXXXX..
..XXXXXX..XXXX..XXXXXXX.
.XXXXXXXXXXXXXXXXXXXXXXX
.XXX.X.X.X.X.X.X.X.X.XXX
..XX.XXX.XXX.XXX.XXX.XX.
...X..XX..XX..XX..XX.X..
...X.X.X.X.X.X.X.X.X.X..

  \ -----------------------------------------------------------
  \ Containers

2 cconstant udg/container

udg/container 1 ,udg-block: container-top

......XXXXX.....
...XXX.....XXX..
..X...XXXXX...X.
..X...........X.
..X....XXX....X.
..X...XXXXX...X.
..X....XXX....X.
..X.....X.....X.

udg/container 2/ 1 ,udg-block: broken-top-left-container

........
...XXX..
..X...X.
..X...X.
..X...X.
..X....X
..X....X
..X....X

udg/container 2/ 1 ,udg-block: broken-top-right-container

........
...XXX..
..X...X.
..X...X.
.X....X.
.X....X.
.X....X.
X.....X.

udg/container 1 ,udg-block: container-bottom

..X..X.X.X.X..X.
..X.XXXX.XXXX.X.
..X.XXX...XXX.X.
..X..XX...XX..X.
..X...........X.
...XXX.....XXX..
......XXXXX.....
................

udg/container 2/ 1 ,udg-block: broken-bottom-left-container

.......X
.....XXX
...XXXX.
..X..XX.
..X.....
...XXX..
......XX
........

udg/container 2/ 1 ,udg-block: broken-bottom-right-container

XX......
.XXX....
..XXXX..
..XX..X.
......X.
...XXX..
XXX.....
........

  \ -----------------------------------------------------------
  \ Icons

2 1 ,udg-block: right-arrow

................
............X...
............XX..
....XXXXXXXXXXX.
....XXXXXXXXXXXX
....XXXXXXXXXXX.
............XX..
............X...

2 1 ,udg-block: left-arrow

................
...X............
..XX............
.XXXXXXXXXXX....
XXXXXXXXXXXX....
.XXXXXXXXXXX....
..XX............
...X............

2 1 ,udg-block: fire-button

....XXXXXXXX....
..XX........XX..
..XX........XX..
..X.XXXXXXXX.X..
..X..........X..
..X..........X..
..X..........X..
XXXXXXXXXXXXXXXX

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
   field: ~species-<flying-sprite   \ UDG address
  cfield: ~species-<flying-frames   \ count
   field: ~species-flying>-sprite   \ UDG address
  cfield: ~species-flying>-frames   \ count
   field: ~species-<breaking-sprite \ UDG address
  cfield: ~species-<breaking-frames \ count
   field: ~species-breaking>-sprite \ UDG address
  cfield: ~species-breaking>-frames \ count
   field: ~species-docked-sprite    \ UDG address
  cfield: ~species-docked-frames    \ count
  cfield: ~species-endurance
cconstant /species
  \ Data structure of an invader species.

create species #species /species * allot
  \ Invaders species data table.

: species#>~ ( n -- a ) /species * species + ;

: set-species ( c1 c2 c3 c4 c5 -- )
  species#>~ >r
  r@ ~species-endurance c!
  r@ ~species-flying>-sprite !
  undocked-invader-frames r@ ~species-flying>-frames c!
  r@ ~species-<flying-sprite !
  undocked-invader-frames r@ ~species-<flying-frames c!
  r@ ~species-breaking>-sprite !
  undocked-invader-frames r@ ~species-breaking>-frames c!
  r@ ~species-<breaking-sprite !
  undocked-invader-frames r@ ~species-<breaking-frames c!
  r@ ~species-docked-sprite !
  docked-invader-frames r> ~species-docked-frames c! ;
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

  cfield: ~invader-y              \ row
  cfield: ~invader-x              \ column
   field: ~invader-sprite         \ UDG address
  cfield: ~invader-frames         \ count
  cfield: ~invader-frame          \ counter
  cfield: ~invader-initial-x      \ column
   field: ~invader-x-inc          \ -1|1
   field: ~invader-initial-x-inc  \ -1|1
  cfield: ~invader-stamina        \ 0..3
  cfield: ~invader-attr           \ color attribute
   field: ~invader-action         \ execution token
   field: ~invader-species        \ data structure address
  2field: ~invader-explosion-time \ ticks clock time
  cfield: ~invader-layer          \ 0 (bottom) .. 4 (top)
  cfield: ~invader-endurance      \ 1..max-endurance
cconstant /invader
  \ Data structure of an species.

max-invaders /invader * constant /invaders

create invaders /invaders allot
  \ Invaders data table.

: invader#>~ ( n -- a ) /invader * invaders + ;
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

: invader-hit-bonus ( -- n ) invader~ ~invader-layer c@ 1+ ;
  \ Bonus points for making the invader retreat.

: invader-destroy-bonus ( -- n ) invader-hit-bonus 8* ;
  \ Bonus points for destroying the invader.

: attacking? ( -- f ) invader~ ~invader-initial-x-inc @
                      invader~ ~invader-x-inc @ = ;
  \ Is the current invader attacking?

: .y/n ( f -- ) if ." Y" else ." N" then space ;
  \ XXX TMP -- for debugging

1 cconstant min-stamina

3 cconstant max-stamina

: set-invader-sprite ( c n -- )
  invader~ ~invader-frames c! invader~ ~invader-sprite !
  invader~ ~invader-frame coff ;
  \ Set character _c_ as the first character of the first
  \ sprite of the current invader, and _n_ as the number of
  \ frames.

: flying-to-the-left? ( -- f ) invader~ ~invader-x-inc @ 0< ;
  \ Is the current invader flying to the left?

: set-<flying-invader-sprite ( -- )
  invader~ ~invader-species @ dup
  ~species-<flying-sprite @ swap
  ~species-<flying-frames c@ set-invader-sprite ;
  \ Set the flying-to-the-left sprite of the current invader.
  \
  \ XXX TODO -- Use double-cell fields to copy both fields with
  \ one operation.
  \
  \ XXX TODO -- If the maximum frames in both directions are
  \ identical, there's no need to initiate `~frame`.

: set-flying>-invader-sprite ( -- )
  invader~ ~invader-species @ dup
  ~species-flying>-sprite @ swap
  ~species-flying>-frames c@ set-invader-sprite ;
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
  invader~ ~invader-species @ dup
  ~species-docked-sprite @ swap
  ~species-docked-frames c@ set-invader-sprite ;
  \ Make the current invader use the docked invader sprite.

: init-invader ( c1 c2 c3 c4 c0 -- )
  set-invader
  invader~ ~invader-stamina coff
  invader~ ~invader-action off
  species#>~ dup invader~ ~invader-species !
                 ~species-endurance c@
                 invader~ ~invader-endurance c!
  invader~ ~invader-initial-x-inc ! invader~ ~invader-x-inc off
  dup invader~ ~invader-initial-x c!
      invader~ ~invader-x c!
  dup invader~ ~invader-y c!
      y>layer invader~ ~invader-layer c!
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

: -invaders-data ( -- ) invaders /invaders erase ;
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
  \ Table to index the stamina (1..3) to its attribute.

: stamina>attr ( n -- c )
  [ stamina-attributes 1- ] literal + c@ ;
  \ Convert stamina _n_ to its corresponding attribute _c_.

: invader-attr! ( n -- )
  stamina>attr invader~ ~invader-attr c! ;
  \ Set the attribute of the current invader after stamina _n_.

: invader-stamina! ( n -- )
  dup invader~ ~invader-stamina c! invader-attr! ;
  \ Make _n_ the stamina of the current invader and change its
  \ attribute accordingly.

: create-invaders ( n1 n2 -- )
  ?do i set-invader
      mothership-stamina invader-stamina!
      ['] docked-invader-action invader~ ~invader-action !
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
  ?do i invader#>~ ~invader-stamina c@ 0<> + loop abs ;
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

cvariable breaches
  \ Number of current breaches in the wall.

cvariable battle-breaches
  \ Total number of breaches in the wall, during the current
  \ battle, even if they have been repaired.

: no-breach ( -- ) breaches coff battle-breaches coff ;
  \ Reset the number of breaches.

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
  brick-attr attr! brick building-width emits-udga ;
  \ Draw a floor of the building at row _row_.

: ground-floor ( row -- )
  building-left-x 1+ swap at-xy brick-attr attr!
  left-door emit-udga
  brick building-width 4 - emits-udga
  right-door emit-udga ;
  \ Draw the ground floor of the building at row _row_.

: building-top ( -- ) building-top-y floor ;
  \ Draw the top of the building.

: containers-bottom ( n -- )
  container-attr attr!
  0 ?do container-bottom emit-2udga loop ;
  \ Draw a row of _n_ bottom parts of containers.

: containers-top ( n -- )
  container-attr attr!
  0 ?do container-top emit-2udga loop ;
  \ Draw a row of _n_ top parts of containers.

: .brick ( -- ) brick-attr attr! brick emit-udga ;
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

25 cconstant #bullets
  \ Number of bullets the tank can hold.
  \ XXX TMP -- provisional value, for testing

04 cconstant #missiles
  \ Number of missiles the tank can hold.
  \ XXX TMP -- provisional value, for testing

02 cconstant #balls
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
  cfield: ~projectile-y         \ row
  cfield: ~projectile-x         \ column
   field: ~projectile-sprite    \ UDG (*)
  cfield: ~projectile-frames    \ count (*)
  cfield: ~projectile-attr      \ attribute (*)
  cfield: ~projectile-altitude  \ row (*)
  cfield: ~projectile-delay     \ counter
  cfield: ~projectile-max-delay \ bitmask (*)
   field: ~projectile-action    \ xt
  cfield: ~projectile-power     \ 1..x
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
  \ same time.

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

: recharge1 ( a -- )
  used-projectiles-stack xstack x>
                    swap xstack >x .ammo ;
  \ Recharge the projectiles stack _a_, which is the current
  \ one, with one projectile from the used projectiles stack,
  \ and update the ammo in the status bar.

: recharge ( a n -- ) 0 ?do dup recharge1 loop drop ;
  \ Recharge the projectiles stack _a_ with _n_ projectiles
  \ from the used projectiles stack.

: recharge1-bullet-gun ( -- )
  #bullets xdepth = ?exit bullets-stack recharge1 ;
  \ Recharge the bullet gun with one projectile, if possible.

: recharge1-missile-gun ( -- )
  #missiles xdepth = ?exit missiles-stack recharge1 ;
  \ Recharge the missile gun with one projectile, if possible.

: recharge1-ball-gun ( -- )
  #balls xdepth = ?exit balls-stack recharge1 ;
  \ Recharge the ball gun with one projectile, if possible.

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
  \ Byte array which is used to mark the projectiles that have
  \ been hit by another projectile. The array is indexed by
  \ rows and columns, as a logical copy of the attributes area.
  \ The size of the array is calculated to contain only the
  \ rows used by projectiles, except `projectile-y0`, the
  \ initial one, where projectiles can not be hit yet.
  \ `hit-projectiles>` returns the address of one row above the
  \ actual data, simulating the row used by the status bar is
  \ part of the array, in order to save a run-time calculation
  \ when the array items are accessed. `hit-projectiles`
  \ returns the address of the actual data.

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

  \ ===========================================================
  cr .( Tank) ?depth debug-point \ {{{1

cvariable tank-x \ column

2variable tank-time
  \ When `dticks` reaches the contents of this variable,
  \ the tank will move.

2variable arming-time
  \ When `dticks` reaches the contents of this variable,
  \ the tank can change its gun type.

2variable trigger-time
  \ When `dticks` reaches the contents of this variable,
  \ the trigger can work.

: repair-tank ( -- )
  0. tank-time 2! 0. arming-time 2! 0. trigger-time 2! ;

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

: ?emit-outside ( col1 ca -- col2 )
  over outside? if   emit-udga 1+        exit
                then drop dup next-col 1+ ;
  \ If column _col1_ is outside the building, display the UDG
  \ stored at _ca_ at the current cursor position.  Increment
  \ _col1_ and return it as _col2_.

: left-tank-udga ( -- ca )
  tank-frame c@ [ udg/tank /udg* ] cliteral * tank-sprite + ;

: middle-tank-udga ( -- ca )
  left-tank-udga [ /udg ] cliteral + ;

: right-tank-udga ( -- ca )
  left-tank-udga [ /udg 2* ] cliteral + ;

: tank-frame+ ( n1 -- n2 ) 1+ dup tank-frames <> and ;
  \ Increase tank frame _n1_ resulting frame _n2_.
  \ If the limit was reached, _n2_ is zero.

: tank-frame- ( n1 -- n2 )
  ?dup if 1- else tank-max-frame then ;
  \ Decrease tank frame _n1_ resulting frame _n2_.

: tank-arm-udga ( -- c )
  tank-frame c@ tank-frame- udg/tank * tank-sprite + 1+ ;
  \ Return UDG _c_ of the tank arm. This is identical to
  \ `middle-tank-udg`, except the frame has to be decreased
  \ in order to prevent the tank chains from moving.

: tank-next-frame ( -- )
  tank-frame c@ tank-frame+ tank-frame c! ;
  \ Update the tank frame.

: tank-parts ( col1 -- col2 )
  tank-attr attr! left-tank-udga   ?emit-outside
                  middle-tank-udga ?emit-outside
                  right-tank-udga  ?emit-outside
  tank-next-frame ;
  \ Display every visible part of the tank (the parts that are
  \ outside the building) and update the frame.

: -tank-extreme ( col1 -- col2 )
  sky-attr attr! bl-udga ?emit-outside ;

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
                   tank-arm-udga ?emit-outside drop ;
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

: schedule ( a n -- ) dticks rot m+ rot 2! ;
  \ Set the double-cell at _a_ with the current content of the
  \ ticks clock plus _n_ ticks.

: already? ( a -- f ) 2@ dpast? ;
  \ Is the double-cell ticks stored at _a_ greater than the
  \ current ticks clock?

: driving ( -- )
  tank-time already? 0exit
  tank-movement perform
  tank-time tank-interval schedule ;

  \ ===========================================================
  cr .( Instructions) ?depth debug-point \ {{{1

: game-title ( -- )
  home game-title$ columns type-center-field ;

: game-version ( -- ) version$ 1 center-type ;

: .copyright ( -- )
  1 22 at-xy ." (C) 2016..2018 Marcos Cruz"
  5 23 at-xy ." (programandala.net)" ;
  \ Display the copyright notice.

  \ XXX OLD -- maybe useful in a future version
  \ : .keyset ( n -- ) ."  = " .kk# 4 spaces ;
  \ : (.keysets ( -- )
  \   row dup s" [Space] to change controls:" rot center-type
  \   9 over 2+  at-xy ." Left "
  \   keyset~ ~keyset-left c@ .keyset
  \   9 over 3 + at-xy ." Right"
  \   keyset~ ~keyset-right c@ .keyset
  \   9 swap 4 + at-xy ." Fire "
  \   keyset~ ~keyset-fire c@  .keyset ;
  \   \ Display keyset at the current row.

: left-key$ ( -- ca len )
  keyset~ ~keyset-left c@ kk#>string ;

: right-key$ ( -- ca len )
  keyset~ ~keyset-right c@ kk#>string ;

: fire-key$ ( -- ca len )
  keyset~ ~keyset-fire c@ kk#>string ;

: .keyset-legend ( -- )
  10 at-x left-arrow  emit-2udga
  15 at-x fire-button emit-2udga
  20 at-x right-arrow emit-2udga ;
  \ Display keyset legend at the current row.

: .keyset-keys ( -- )
  10 at-x left-key$  2 type-right-field
  13 at-x fire-key$  6 type-center-field
  20 at-x right-key$ 2 type-left-field ;
  \ Display keyset keys at the current row.

: (.keyset ( -- )
  \ s" [Space] to change controls:" row dup >r center-type
    \ XXX TODO --
  .keyset-legend cr .keyset-keys ;
  \ Display keyset at the current row.

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

: .keyset ( -- )
  0 12 at-xy (.keyset
  \ s" SPACE: change - ENTER: start" 18 center-type ; XXX TMP
  0 16 at-xy not-in-this-language$ columns type-center-field
  0 19 at-xy start$ columns type-center-field ;
  \ XXX TMP --

: invariable-menu-screen ( -- ) game-version .copyright ;
  \ Display the parts of the menu screen that are invariable,
  \ i.e., don't depend on the current language.

: variable-menu-screen ( -- ) game-title .players .keyset ;
  \ Display the parts of the menu screen that are variable,
  \ i.e., depend on the current language.

: menu-screen ( -- )
  cls invariable-menu-screen variable-menu-screen ;

: change-language  ( -- ) lang 1+ dup langs < abs * c!> lang ;
  \ Change the current language.

[breakable] [if]

defer quit-game ( -- )

' noop ' quit-game defer!

: (quit-game ( -- ) mode-32 default-colors quit ;

: ?quit-game ( -- ) break-key? 0exit quit-game ;

: breakable ( -- ) ['] (quit-game ['] quit-game defer! ;
  \ Make the game breakable.

[then]

: menu ( -- )
  begin
    [breakable] [if] ?quit-game [then] \ XXX TMP --
    key lower case
    start-key    of  exit           endof \ XXX TMP --
    language-key of change-language variable-menu-screen endof
    \ bl  of  next-keyset .keyset  endof
    \ 'p' of  change-players .players  endof
    \ XXX TMP --
    endcase
  again ;

: mobilize ( -- )
  mode-32iso init-colors text-attr attr! menu-screen menu ;

  \ ===========================================================
  cr .( Invasion) ?depth debug-point \ {{{1

cvariable #invaders
  \ Current number of invaders during the attack.

2variable invader-time
  \ When `dticks` reaches the contents of this variable,
  \ the current invader will move.

: init-invaders ( -- ) init-invaders-data
                       0 set-invader
                       0. invader-time 2!
                       #invaders coff ;

: at-invader ( -- )
  invader~ ~invader-x c@ invader~ ~invader-y c@ at-xy ;
  \ Set the cursor position at the coordinates of the invader.

: invader-frame+ ( n1 -- n2 )
  1+ dup invader~ ~invader-frames c@ < and ;
  \ Increase frame _n1_ resulting frame _n2_.
  \ If the limit was reached, _n2_ is zero.
  \
  \ XXX TODO -- Use `~max-frame <>` for speed.

: invader-udga ( -- ca )
  invader~ ~invader-frame c@
  dup invader-frame+ invader~ ~invader-frame c!
  [ udg/invader /udg * ] cliteral *
  invader~ ~invader-sprite @ + ;

  \ First UDG _c_ of the current frame of the current invader's
  \ sprite, calculated from its sprite and its frame.

: .invader ( -- )
  invader~ ~invader-attr c@ attr! invader-udga emit-2udga ;
  \ Display the current invader.  at the cursor coordinates, in
  \ its proper attribute.

: x>bricks-xy ( col1 -- col1 row1 col2 row2 col3 row3 )
  invader~ ~invader-y c@ 2dup 1+ 2dup 2- ;
  \ Convert the column _col1_ of the broken wall to the
  \ coordinates of the broken brick above the invader, _col3
  \ row3_, below it, _col3 row3_, and in front of it, _col1
  \ row1_.

: break-bricks> ( col1 row1 col2 row2 col3 row3 -- )
  broken-wall-attr attr!
  at-xy broken-top-left-brick emit-udga
  at-xy broken-bottom-left-brick emit-udga
  sky-attr attr! at-xy space ;
  \ Display the broken bricks at the given coordinates, which
  \ are at the right of the current invader: above the invader,
  \ _col3 row3_; below the invader, _col2 row2_; and an empty
  \ space in front of it, _col1 row1_.

: <break-bricks ( col1 row1 col2 row2 col3 row3 -- )
  broken-wall-attr attr!
  at-xy broken-top-right-brick emit-udga
  at-xy broken-bottom-right-brick emit-udga
  sky-attr attr! at-xy space ;
  \ Display the broken bricks at the given coordinates, which
  \ are at the left of the current invader: above the invader,
  \ _col3 row3_; below it, _col2 row2_; and an empty space in
  \ front of it, _col1 row1_.

: break-container ( -- ) container-attr attr! catastrophe on ;

: break-container> ( -- )
  break-container
  invader~ ~invader-x c@
  [ udg/invader udg/container 1- + ] cliteral +
  invader~ ~invader-y c@ at-xy
  broken-top-right-container emit-udga
  invader~ ~invader-x [ udg/invader ] c@x+
  invader~ ~invader-y c@1+ at-xy
  broken-bottom-left-container emit-udga ;
  \ Break the container that is at the right of the invader.

: <break-container ( -- )
  break-container
  invader~ ~invader-x [ udg/container ] c@x-
  invader~ ~invader-y c@ at-xy
  broken-top-left-container emit-udga
  invader~ ~invader-x c@1- invader~ ~invader-y c@1+ at-xy
  broken-bottom-right-container emit-udga ;
  \ Break the container that is at the left of the invader.

: healthy? ( -- f )
  invader~ ~invader-stamina c@ max-stamina = ;
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
  @ invader~ ~invader-action ! ;
  \ Set the action of the current invader after x-coordinate
  \ increment _-1..1_.

: set-invader-direction ( -1..1 -- )
  dup invader~ ~invader-x-inc ! set-invader-move-action ;
  \ Set the direction of the current invader after x-coordinate
  \ increment _-1..1_.

: impel-invader ( -- )
  invader~ ~invader-x-inc @ set-invader-move-action ;
  \ Restore the moving action of the current invader, using
  \ its current direction.

: change-direction ( -- )
  invader~ ~invader-x-inc @ negate set-invader-direction ;
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

: undock ( -- )
  invader~ ~invader-initial-x-inc @ set-invader-direction
  set-flying-invader-sprite ;
  \ Undock the current invader.

: is-there-a-projectile? ( col row -- 0f )
  xy>attr bright-mask and ;

: .sky ( -- ) sky-attr attr! space ;
  \ Display a sky-color space.

: left-of-invader ( -- col row )
  invader~ ~invader-x c@1- invader~ ~invader-y c@ ;
  \ Coordinates _col row_ at the right of the current invader.

: right-of-invader ( -- col row )
  invader~ ~invader-x [ udg/invader ] c@x+
  invader~ ~invader-y c@ ;
  \ Coordinates _col row_ at the left of the current invader.

defer ?dock ( -- )
  \ If the current invader is at home, dock it.

: docked? ( -- f )
  invader~ ~invader-x c@ invader~ ~invader-initial-x c@ = ;
  \ Is the current invader at the dock, i.e. at its start
  \ position?

: is-there-a-wall? ( x y -- f ) xy>attr brick-attr = ;

: is-there-a-container? ( x y -- f ) xy>attr container-attr = ;

: <move-invader ( -- )
  invader~ ~invader-x c1-! at-invader .invader .sky ;
  \ Move the current invader to the left.

: <hit-container ( -- )
  healthy? if   <break-container <move-invader exit
           then turn-back> ;
  \ Hit the container that is at the left of the invader.

: <hit-wall ( -- )
  invader~ ~invader-species @ dup
  ~species-<breaking-sprite @ swap
  ~species-<breaking-frames c@ set-invader-sprite
  ['] <breaking-invader-action invader~ ~invader-action ! ;
  \ Hit the wall that is at the left of the current invader and
  \ start breaking it.

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
  at-invader .sky .invader invader~ ~invader-x c1+! ;
  \ Move the current invader to the right.

: hit-container> ( -- )
  healthy? if   break-container> move-invader> exit
           then <turn-back ;
  \ Hit the container that is at the right of the invader.

: hit-wall> ( -- )
  invader~ ~invader-species @ dup
  ~species-breaking>-sprite @ swap
  ~species-breaking>-frames c@ set-invader-sprite
  ['] breaking>-invader-action invader~ ~invader-action ! ;
  \ Hit the wall that is at the right of the current invader
  \ and start breaking it.

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
  max-stamina invader~ ~invader-stamina c@ - cure-factor *
  random ;
  \ Is it a difficult cure?  The less stamina, the more chances
  \ to be a difficult cure. This is used to delay the cure.

: cure ( -- ) invader~ ~invader-stamina c@1+ max-stamina min
              invader-stamina! ;
  \ Cure the current invader, increasing its stamina.

: ?cure ( -- ) difficult-cure? ?exit cure ;
  \ Cure the current invader, depending on its status.

: ?undock ( -- ) #invaders c@ random ?exit undock ;
  \ Undock the current invader randomly, depending on the
  \ current number of invaders: The few invaders alive, the
  \ more chances to be undock.

:noname ( -- )
  healthy? if   ?undock
           else ?cure
           then at-invader .invader
  ; ' docked-invader-action defer!
  \ Action of the invaders that are docked.

: dock ( -- )
  ['] docked-invader-action invader~ ~invader-action !
  set-docked-invader-sprite ;
  \ Dock the current invader.

:noname ( -- ) docked? 0exit dock ; ' ?dock defer!
  \ If the current invader is at the dock, dock it.

: one-more-breach ( -- ) breaches c1+! battle-breaches c1+! ;

: <break-wall ( -- )
  invader~ ~invader-x c@1- x>bricks-xy
  <break-bricks one-more-breach impel-invader ;
  \ Break the wall at the left of the current invader.

: weak? ( -- 0f )
  [ max-stamina max-endurance + 8 * ] cliteral
  invader~ ~invader-stamina   c@ -
  invader~ ~invader-endurance c@ - random ;
  \ Is the current invader too weak to break the wall or to
  \ unball itself?

: ?<break-wall ( -- ) weak? ?exit <break-wall ;
  \ Break the wall at the left of the current invader, if it's
  \ strong enough to do so.

:noname ( -- ) ?<break-wall at-invader .invader
               ; ' <breaking-invader-action defer!
  \ Action of the invaders that are breaking the wall to the
  \ left.

: break-wall> ( -- )
  invader~ ~invader-x [ udg/invader ] c@x+ x>bricks-xy
  break-bricks> one-more-breach impel-invader ;
  \ Break the wall at the right of the current invader.

: ?break-wall> ( -- ) weak? ?exit break-wall> ;
  \ Break the wall at the left of the current invader, if it's
  \ strong enough to do so.

:noname ( -- ) ?break-wall> at-invader .invader
               ; ' breaking>-invader-action defer!
  \ Action of the invaders that are breaking the wall to the
  \ right.

: last-invader? ( -- f ) invader~ last-invader~ = ;
  \ Is the current invader the last one?

: alive? ( -- f ) invader~ ~invader-stamina c@ 0<> ;
  \ Is the current invader alive?

: next-invader ( -- ) last-invader? if   first-invader~
                                    else invader~ /invader +
                                    then !> invader~ ;
  \ Point the current invader to the next one.

2 cconstant invader-interval \ ticks

: manage-invaders ( -- )
  invader-time already? 0exit
  alive? if invader~ ~invader-action perform then next-invader
  invader-time invader-interval schedule ;
  \ If it's the right time, move the current invader, then
  \ choose the next one.

  \ ===========================================================
  cr .( Mothership) ?depth debug-point \ {{{1

cvariable motherships
  \ Number of motherships. Used as a flag.

defer mothership-action ( -- )
  \ The current action of the mothership.

: mothership-action! ( xt -- ) ['] mothership-action defer! ;
  \ Set _xt_ as the current action of the mothership.

defer invisible-mothership-action ( -- )
  \ Action of the flying mothership when it's invisible.

defer visible-mothership-action ( -- )
  \ Action of the flying mothership when it's visible.

defer stopped-mothership-action ( -- )
  \ Action of the mothership when it's stopped above the
  \ building.

1 cconstant mothership-y
  \ Row of the mothership.

variable mothership-x-inc

udg/mothership 1- negate  constant visible-mothership-min-x
last-column              cconstant visible-mothership-max-x

                       0 cconstant whole-mothership-min-x
columns udg/mothership -  constant whole-mothership-max-x

variable mothership-stopped
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

2variable mothership-time
  \ When `dticks` reaches the contents of this variable,
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

: set-mothership-sprite ( ca n -- )
  c!> mothership-frames !> mothership mothership-frame coff ;
  \ Set character _c_ as the first character of the first
  \ sprite of the mothership, and _n_ as the number of frames.

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
  \ Set mothership stamina to _n_ and modify its attributes
  \ accordingly.

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
  flying-mothership-sprite flying-mothership-frames
  set-mothership-sprite
  ['] invisible-mothership-action mothership-action!
  mothership-stopped off 0. mothership-time 2!
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

: mothership-udga ( -- ca )
  mothership-frame c@ dup mothership-frame+ mothership-frame c!
  [ udg/mothership /udg * ] cliteral * mothership + ;
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
  mothership-attr attr! mothership-udga emit-2udga ;
  \ Display the mothership, which is fully visible, at the
  \ cursor coordinates in its default attribute.

: .mothership ( -- ) at-mothership (.mothership ;
  \ Display the mothership, which is fully visible, at its
  \ coordinates, in its default attribute.

: (.visible-right-mothership ( -- )
  mothership-attr attr!
  mothership-udga
  [ udg/mothership 1- /udg* ] cliteral + emit-udga ;
  \ Display the mothership, which is partially visible (only
  \ its right side is visible) at the cursor coordinates.

: .visible-right-mothership ( -- )
  whole-mothership-min-x mothership-y at-xy
  (.visible-right-mothership ;
  \ Display the mothership, which is partially visible (only
  \ its right side is visible) at its proper coordinates.

: (.visible-left-mothership ( -- )
  mothership-attr attr! mothership-udga emit-udga ;
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

: move-visible-mothership> ( -- )
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

: above-left-invaders? ( -- f )
  mothership-x @ invaders-min-x = ;
  \ Is the mothership above the left initial column of
  \ invaders?

: above-right-invaders? ( -- f )
  mothership-x @ invaders-max-x = ;
  \ Is the mothership above the right initial column of
  \ invaders?

: enlist-squadron ( -- ) half-max-invaders #invaders c+! ;

: create-squadron ( -- )
  above-left-invaders? if   create-left-squadron
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
  flying-mothership-sprite flying-mothership-frames
  set-mothership-sprite
  ['] visible-mothership-action mothership-action! ;
  \ Action of the mothership when the beam is shrinking.

: beam-off ( -- )
  invader-max-y 1+ mothership-y 1+
  ['] beaming-up-mothership-action set-beam ;
  \ Turn the mothership's beam off, i.e. start shrinking it
  \ back to the mothership.

: .new-invader ( -- )
  invader~ ~invader-initial-x c@ invader~ ~invader-y c@ at-xy
  invader~ ~invader-sprite @ emit-2udga ;
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
  reach-invader? 0exit create-invader ;
  \ Grow the beam towards the ground one character.

: beaming-down-mothership-action ( -- )
  (beam-down beaming-down? ?exit beam-off ;
  \ Manage the beam, which is growing down to the ground.
  \ If it's finished, display the new invaders and start
  \ shrinking the beam.

: first-new-invader# ( -- n )
  half-max-invaders above-left-invaders? 0= and ;
  \ Return the number of the first invader to create,
  \ depending on the position of the mothership.

: beam-on ( -- )
  beaming-mothership-sprite beaming-mothership-frames
  set-mothership-sprite
  first-new-invader# beam-invader# c!
  mothership-y 1+ invader-max-y 1+
  ['] beaming-down-mothership-action set-beam ;
  \ Turn the mothership's beam on, i.e. start launching it
  \ towards the ground.

: help-invaders? ( -- f )
  above-left-invaders?  if left-side-invaders  0= dup 0exit
                           beam-on exit then
  above-right-invaders? if right-side-invaders 0= dup 0exit
                           beam-on exit then
  false ;
  \ If needed, help the invaders below the mothership and
  \ return _true_; otherwise return _false_.

: ?move-visible-mothership> ( -- )
  right-of-mothership is-there-a-projectile?
  if mothership-turns-back exit then
  help-invaders? ?exit
  move-visible-mothership> ;
  \ Move the visible mothership to the right, if possible.

: left-of-mothership ( -- col row )
  mothership-x @ 1- mothership-y ;
  \ Return coordinates _col row_ of the position at the left
  \ of the mothership.

: <move-visible-mothership ( -- )
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

: ?<move-visible-mothership ( -- )
  left-of-mothership is-there-a-projectile?
  if mothership-turns-back exit then
  help-invaders? ?exit
  <move-visible-mothership ;
  \ Move the visible mothership to the left, if possible.

defer move-visible-mothership ( -- )
  \ Execute the proper movement of the mothership.

      ' ?<move-visible-mothership ,
here  ' noop ,
      ' ?move-visible-mothership> ,
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

: stop-mothership ( -- )
  0 set-mothership-x-inc
  ['] stopped-mothership-action mothership-action!
  mothership-stopped on ;

: ?stop-mothership ( -- ) mothership-stopped @ ?exit
                            above-building? 0= ?exit
                                      3 random ?exit
                               stop-mothership ;

: missiles-left ( -- n )
  missiles-stack xstack projectiles-left gun-stack ;
  \ Return the number _n_ of missiles left. The current gun is
  \ preserved.

: (new-mothership-x-inc ( -- -1|0|1 )
  left-side-invaders right-side-invaders      <=>
  missiles-left if   mothership-x @ tank-x c@ <=> +
                then                        -1..1 + polarity ;
  \ Return the new direction of the mothership, which is
  \ stopped above the building.

: new-mothership-x-inc ( -- -1|0|1 )
  3 random 0= 0dup 0exit (new-mothership-x-inc ;
  \ Return the new direction of the mothership, which is
  \ stopped above the building. Two times out of three, the
  \ result is zero.

: ?start-mothership ( -- )
  new-mothership-x-inc ?dup 0exit
  set-mothership-x-inc
  ['] visible-mothership-action mothership-action! ;

:noname ( -- )
  move-visible-mothership
  visible-mothership? 0=
  if   ['] invisible-mothership-action mothership-action! exit
  then ?stop-mothership
  ; ' visible-mothership-action defer!
  \ Action of the mothership when it's visible and not stopped.

:noname ( -- )
  .mothership ?start-mothership
  ; ' stopped-mothership-action defer!
  \ Action of the mothership when it's visible but stopped
  \ above the building.
  \
  \ XXX TODO -- Add curing.

:noname ( -- )
  advance-mothership visible-mothership?
  if   .visible-mothership
       ['] visible-mothership-action mothership-action! exit
  then mothership-in-range? ?exit mothership-turns-back
  ; ' invisible-mothership-action defer!
  \ Action of the mothership when it's invisible.

8 cconstant mothership-interval \ ticks

: mothership-destroyed? ( -- f ) motherships c@ 0= ;

: manage-mothership ( -- )
  mothership-destroyed?    ?exit
  mothership-time already? 0exit
  mothership-action
  mothership-time mothership-interval schedule ;

  \ ===========================================================
  cr .( Impact) ?depth debug-point \ {{{1

' shoot-sound alias mothership-bang ( -- )
  \ Make the explosion sound of the mothership.
  \ XXX TMP --
  \ XXX TODO -- look for a better sound

8 cconstant mothership-explosion-interval
  \ Ticks between the frames of the explosion.

2variable mothership-explosion-time
  \ When `dticks` reaches the contents of this variable,
  \ the explosion advances to the next frame.

: destroy-mothership ( -- )
  -mothership ['] noop mothership-action! motherships ?c1-! ;

: mothership-explosion? ( -- f ) mothership-frame c@ 0<> ;
  \ Is the mothership explosion still active? When the frame
  \ counter is zero (first frame), the explosion cycle has been
  \ completed and _f_ is _false_.

: exploding-mothership-action ( -- )
  mothership-explosion-time already? 0exit
  at-mothership .mothership
  mothership-explosion-time mothership-explosion-interval
  schedule
  mothership-explosion? ?exit destroy-mothership ;
  \ Action of the mothership when it's exploding.

: mothership-destroy-bonus ( -- n )
  mothership-retreat-bonus 8* ;
  \ Bonus points for destroying the mothership.

:noname ( -- )
  explosion-sprite explosion-frames set-mothership-sprite
  .mothership
  mothership-explosion-time mothership-explosion-interval
  schedule
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

: destroy-invader ( -- ) -invader
                         invader~ ~invader-stamina coff
                         invader~ ~invader-action off
                         #invaders c1-! ;

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

: invader-explosion? ( -- f ) invader~ ~invader-frame c@ 0<> ;
  \ Is the explosion of the current invader still active? When
  \ the frame counter is zero (first frame), the explosion
  \ cycle has been completed and _f_ is _false_.

8 cconstant invader-explosion-interval
  \ Ticks between the frames of the explosion.

: exploding-invader-action ( -- )
  invader~ ~invader-explosion-time already? 0exit
  at-invader .invader
  invader~ ~invader-explosion-time invader-explosion-interval
  schedule
  invader-explosion? ?exit destroy-invader ;
  \ Action of the invader when it's exploding.

: set-exploding-invader ( -- )
  explosion-sprite explosion-frames set-invader-sprite
  at-invader .invader
  invader~ ~invader-explosion-time invader-explosion-interval
  schedule
  ['] exploding-invader-action invader~ ~invader-action !
  invader-bang invader-destroy-bonus update-score ;
  \ The current invader has been impacted. Set it accordingly.

' lightning1-sound alias wound-sound
  \ XXX TMP --
  \ XXX TODO -- look for a proper sound

: retreat ( -- ) turn-back invader-hit-bonus update-score ;
  \ The current invader retreats.

: wounded ( -- ) wound-sound
                 invader~ ~invader-stamina c@1-
                 min-stamina max invader-stamina! ;
  \ Reduce the invader's stamina after being shoot.

: mortal? ( -- f ) invader~ ~invader-stamina     c@
                   invader~ ~invader-layer       c@ +
                   invader~ ~invader-endurance   c@ +
                   projectile~ ~projectile-power c@ -
                   0 max random 0= ;
  \ Is it a mortal impact?  The random calculation depends on
  \ the stamina, the altitude, the species' endurance and the
  \ type of projectile.

: invader-exploding? ( -- f )
  invader~ ~invader-action @ ['] exploding-invader-action = ;
  \ Is the current invader exploding?

: unball-invader ( -- )
  invader~ ~invader-stamina c@ invader-attr!
  set-flying-invader-sprite impel-invader ;
  \ Unball the current invader.

: balled-invader-action ( -- )
  at-invader .invader weak? ?exit unball-invader ;
  \ Action of the invader when it's balled: Display it and, if
  \ it's strong enough already, unball it.

' shoot-sound alias bubble-sound ( -- )
  \ Make the sound of a balled invader.
  \ XXX TMP --
  \ XXX TODO -- look for a better sound

: ball-invader ( -- )
  set-docked-invader-sprite
  balled-invader-attr invader~ ~invader-attr c!
  at-invader .invader
  ['] balled-invader-action invader~ ~invader-action !
  bubble-sound invader-hit-bonus update-score ;
  \ Ball the current invader, which has been impacted by a
  \ ball.

defer ball-gun? ( -- f )
  \ Is the current gun the ball gun?

: (invader-impacted ( -- )
  invader-exploding? ?exit
  mortal? if set-exploding-invader exit then
  wounded attacking? 0exit retreat ;
  \ The current invader has been impacted by the projectile.

: invader-impacted ( -- )
  invader~ impacted-invader set-invader (invader-impacted
  !> invader~ ;
  \ An invader has been impacted by the projectile.
  \ Make it the current one and manage it.

: mothership-impacted? ( -- f )
  projectile~ ~projectile-y c@ mothership-y = ;

: mothership-exploding? ( -- f )
  action-of mothership-action
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
  [ sky-attr 0<> ] [if]   postpone sky-attr postpone <>
                   [then] ; immediate
  \ Compile, if needed, the words needed to check if _c_ is
  \ different from `sky-attr`.  This is used to save a bit
  \ execution time when `sky-attr` is zero, and also in case
  \ the value changes in future versions.

: impact? ( -- f|0f ) projectile-xy xy>attr sky-attr<> ;
  \ Did the projectile impacted?

  \ ===========================================================
  cr .( Guns) ?depth debug-point \ {{{1

3 cconstant #guns

#guns 1- cconstant max-gun#

cvariable gun#
  \ Identifier of the current gun.

: get-gun ( -- n ) gun# c@ ;
  \ Identifier _n_ of the current gun:
  \ 0 = bullet gun
  \ 1 = missile gun
  \ 2 = ball gun

0 constant gun~
  \ Data address of the current gun.

0
   field: ~gun-projectile-stack     \ address
   field: ~gun-projectile-sprite    \ UDG address
  cfield: ~gun-projectile-frames    \ count
  cfield: ~gun-projectile-attr      \ attribute
  cfield: ~gun-projectile-altitude  \ row
  cfield: ~gun-ammo-x               \ column on status bar
   field: ~gun-tank-sprite          \ UDG address
  cfield: ~gun-trigger-interval     \ ticks
  cfield: ~gun-projectile-max-delay \ bitmask
   field: ~gun-projectile-action    \ xt
  cfield: ~gun-projectile-power     \ 1..x
   field: ~gun-recharger            \ xt
   field: ~gun-icon-displayer       \ xt
cconstant /gun
  \ Data structure of a gun.

#guns /gun * cconstant /guns

create guns /guns allot

: gun#>~ ( n -- a ) /gun * guns + ;
  \ Convert gun number _n_ to its data address _a_.

 bullet-gun# gun#>~ constant bullet-gun~
missile-gun# gun#>~ constant missile-gun~
   ball-gun# gun#>~ constant ball-gun~
  \ Data addresses of the guns.

:noname ( -- ) gun~ ~gun-projectile-stack @ xstack
               ; ' gun-stack defer!
  \ Activate the projectile stack of the current gun.

:noname ( n -- )
  dup gun# c!
      gun#>~ dup !> gun~
             dup ~gun-tank-sprite @ !> tank-sprite
                 ~gun-ammo-x c@ c!> ammo-x
  gun-stack
  ; ' set-gun defer!
  \ Set _n_ as the current gun.

: recharge-gun ( -- ) gun~ ~gun-recharger perform ;
  \ Recharge the current gun.

: recharging ( -- ) gun-below-building?  0exit
                    kk-recharge pressed? 0exit
                    recharge-gun .ammo ;

:noname ( -- f ) gun~ ball-gun~ = ; ' ball-gun? defer!
  \ Is the current gun the ball gun?

  \ --------------------------------------------
  \ Set guns' data

 bullets-stack  bullet-gun~ ~gun-projectile-stack !
missiles-stack missile-gun~ ~gun-projectile-stack !
   balls-stack    ball-gun~ ~gun-projectile-stack !

 bullet-sprite  bullet-gun~ ~gun-projectile-sprite !
missile-sprite missile-gun~ ~gun-projectile-sprite !
   ball-sprite    ball-gun~ ~gun-projectile-sprite !

bullet-frames   bullet-gun~ ~gun-projectile-frames c!
missile-frames missile-gun~ ~gun-projectile-frames c!
ball-frames       ball-gun~ ~gun-projectile-frames c!

bullet-attr   bullet-gun~ ~gun-projectile-attr c!
missile-attr missile-gun~ ~gun-projectile-attr c!
ball-attr       ball-gun~ ~gun-projectile-attr c!

invader-min-y    bullet-gun~ ~gun-projectile-altitude c!
mothership-y 1+ missile-gun~ ~gun-projectile-altitude c!
building-top-y 1+  ball-gun~ ~gun-projectile-altitude c!

 bullets-x  bullet-gun~ ~gun-ammo-x c!
missiles-x missile-gun~ ~gun-ammo-x c!
   balls-x    ball-gun~ ~gun-ammo-x c!

 bullet-gun-tank-sprite  bullet-gun~ ~gun-tank-sprite !
missile-gun-tank-sprite missile-gun~ ~gun-tank-sprite !
   ball-gun-tank-sprite    ball-gun~ ~gun-tank-sprite !

12  bullet-gun~ ~gun-trigger-interval c!
16 missile-gun~ ~gun-trigger-interval c!
24    ball-gun~ ~gun-trigger-interval c!

%00001  bullet-gun~ ~gun-projectile-max-delay c!
%00011 missile-gun~ ~gun-projectile-max-delay c!
%01111    ball-gun~ ~gun-projectile-max-delay c!

2  bullet-gun~ ~gun-projectile-power c!
4 missile-gun~ ~gun-projectile-power c!
1    ball-gun~ ~gun-projectile-power c!

' recharge1-bullet-gun   bullet-gun~ ~gun-recharger !
' recharge1-missile-gun missile-gun~ ~gun-recharger !
' recharge1-ball-gun       ball-gun~ ~gun-recharger !

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
  1+ dup invader~ ~invader-frames c@ < and ;
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
  projectile~ ~projectile-sprite @
  projectile~ ~projectile-frames c@ random + ;
  \ Return the UDG _c_ of a random frame of the projectile.

[then]

: projectile ( -- c )
  projectile~ ~projectile-sprite @
  projectile~ ~projectile-frames c@ random /udg* + ;
  \ Return the UDG _c_ of a random frame of the projectile.

: .projectile ( -- ) projectile~ ~projectile-attr c@ attr!
                     at-projectile projectile emit-udga ;
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

8 cconstant projectile-explosion-interval
  \ Ticks between the frames of the explosion.

2variable projectile-explosion-time
  \ When `dticks` reaches the contents of this variable,
  \ the explosion advances to the next frame.

cvariable projectile-frame
  \ Current frame of the exploding projectile sprite.

: projectile-explosion? ( -- f ) projectile-frame c@ 0<> ;
  \ Is the projectile explosion still active? When the frame
  \ counter is zero (first frame), the explosion cycle has been
  \ completed and _f_ is _false_.

: exploding-projectile-action ( -- )
  projectile-explosion-time already? 0exit
  at-projectile .projectile
  projectile-explosion-time projectile-explosion-interval
  schedule
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
  projectile-explosion-sprite
  projectile~ ~projectile-sprite !
  projectile-explosion-frames
  projectile~ ~projectile-frames c!
  .projectile
  projectile-explosion-time projectile-explosion-interval
  schedule
  ['] exploding-projectile-action
  projectile~ ~projectile-action !  projectile-bang ;

: bullet-and-missile-action ( -- )
  hit-projectile?
  if -hit-projectile set-exploding-projectile exit then
  projectile-delay ?exit
  -projectile projectile-lost? if destroy-projectile exit then
  projectile~ ~projectile-y c1-!
  projectile-xy is-there-a-projectile?
  if projectile-xy hit-projectile destroy-projectile exit then
  impact? if impact exit then .projectile ;
  \ Action of bullets and missiles.

' bullet-and-missile-action
bullet-gun~ ~gun-projectile-action !
  \ Set the action of bullets.

' bullet-and-missile-action
missile-gun~ ~gun-projectile-action !
  \ Set the action of missiles.

: repair-breach ( col row -- ) 2dup 1+ at-xy .brick
                               2dup    at-xy .brick
                                    1- at-xy .brick
                               breaches c1-! ;

: sky-attr=
  \ Compilation: ( -- )
  \ Run-time:    ( c -- f )
  [ sky-attr 0<> ] [if]   postpone sky-attr postpone =
                   [else] postpone 0= [then] ; immediate
  \ Compile the words needed to check if _c_ is equal to
  \ `sky-attr`.  This is used to save a bit execution time when
  \ `sky-attr` is zero, and also in case the value changes in
  \ future versions.

: is-there-breach? ( col row -- f ) xy>attr sky-attr= ;
  \ Is there a breach at coordinates _col row_?

: (invader-balled ( -- )
  invader-exploding? ?exit ball-invader ;
  \ The current invader has been impacted by the ball.

: invader-balled ( -- )
  invader~ impacted-invader set-invader (invader-balled
  !> invader~ ;
  \ An invader has been impacted by the ball.
  \ Make it the current one and manage it.

: right-of-projectile-xy ( -- col row )
  projectile~ ~projectile-x c@ 1+
  projectile~ ~projectile-y c@ ;

: <wall-ball-action ( -- )
  hit-projectile?
  if -hit-projectile set-exploding-projectile exit then
  projectile-delay ?exit -projectile
  right-of-projectile-xy is-there-breach?
  if right-of-projectile-xy repair-breach
     destroy-projectile exit then
  projectile-lost? if destroy-projectile exit then
  projectile~ ~projectile-y c1-!
  impact? if invader-balled destroy-projectile exit then
  .projectile ;
  \ Action of the balls that are flying on the left wall of the
  \ building, and therefore can repair the breaches.

: left-of-projectile-xy ( -- col row )
  projectile~ ~projectile-x c@ 1-
  projectile~ ~projectile-y c@ ;

: wall>-ball-action ( -- )
  hit-projectile?
  if -hit-projectile set-exploding-projectile exit then
  projectile-delay ?exit -projectile
  left-of-projectile-xy is-there-breach?
  if left-of-projectile-xy repair-breach
     destroy-projectile exit then
  projectile-lost? if destroy-projectile exit then
  projectile~ ~projectile-y c1-!
  impact? if invader-balled destroy-projectile exit then
  .projectile ;
  \ Action of the balls that are flying on the right wall of
  \ the building, and therefore can repair the breaches.

: ball-action ( -- )
  projectile-delay ?exit
  -projectile projectile-lost? if destroy-projectile exit then
  projectile~ ~projectile-y c1-!
  impact? if invader-balled destroy-projectile exit then
  .projectile ;
  \ Action of the balls.

' ball-action ball-gun~ ~gun-projectile-action !
  \ Set the action of balls.

: ball-sprite&action ( -- ca n xt )
  gun-x case
    building-left-x 1- of \ on the left wall?
      <wall-ball-sprite <wall-ball-frames ['] <wall-ball-action
    endof
    building-right-x 1+ of \ on the right wall?
      wall>-ball-sprite wall>-ball-frames ['] wall>-ball-action
    endof
    default-of \ any other position
      gun~ ~gun-projectile-sprite @
      gun~ ~gun-projectile-frames c@
      ['] ball-action
    endof
  endcase ;
  \ Return sprite _ca_, number of frames _n_ and action _xt_ of
  \ the new current projectile, which is a ball.

: get-projectile ( -- )
  x> !> projectile~
  gun-x projectile~ ~projectile-x c!
  projectile-y0 projectile~ ~projectile-y c!
  ball-gun? if   ball-sprite&action
            else gun~ ~gun-projectile-sprite @
                 gun~ ~gun-projectile-frames c@
                 ['] bullet-and-missile-action
            then projectile~ ~projectile-action !
                 projectile~ ~projectile-frames c!
                 projectile~ ~projectile-sprite !
  gun~ ~gun-projectile-attr c@
  projectile~ ~projectile-attr c!
  gun~ ~gun-projectile-max-delay c@
  projectile~ ~projectile-max-delay c!
  gun~ ~gun-projectile-altitude c@
  projectile~ ~projectile-altitude c!
  gun~ ~gun-projectile-power c@
  projectile~ ~projectile-power c! ;
  \ Get a new projectile from the stack of unused projectiles
  \ and set its data according to the current value of `gun~`.
  \ For the sake of run-time speed, some fields are copied from
  \ the structure pointed by `gun~` to the structure pointed by
  \ `projectile~`.

: launch-projectile ( -- )
  .projectile projectile~ start-flying fire-sound ;

: fire ( -- )
  get-projectile launch-projectile .ammo
  trigger-time gun~ ~gun-trigger-interval c@ schedule ;
  \ Fire the gun of the tank.

: flying-projectiles? ( -- 0f ) #flying-projectiles c@ ;
  \ Is there any projectile flying?

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

: max-flying-projectiles? ( -- f )
  #flying-projectiles c@ max-flying-projectiles = ;

: shooting ( -- )
  kk-fire pressed?        0exit
  trigger-time already?   0exit
  projectiles-left        0exit
  max-flying-projectiles? ?exit
  gun-below-building?     ?exit fire ;
  \ Manage the gun.

: tank-previous-frame ( -- )
  tank-frame c@ tank-frame- tank-frame c! ;
  \ Restore the previous frame of the tank.  This is used to
  \ prevent the tank chain from moving when the gun is changed.

: .gun-icon ( -- ) gun~ ~gun-icon-displayer perform ;

: highlight-gun ( -- ) ammo-attr attr! .gun-icon (.ammo ;
  \ Highlight the current gun icon and its ammo on the status
  \ bar.

: unhighlight-gun ( -- ) status-attr attr! .gun-icon (.ammo ;
  \ Unhighlight the current gun icon and its ammo on the status
  \ bar.

: change-gun ( n -- ) unhighlight-gun
                      set-gun tank-previous-frame .tank
                      highlight-gun ;
  \ Change the current gun for _n_, updating the status bar and
  \ the tank.

: next-gun ( -- ) get-gun 1+ dup #guns < and change-gun ;

: previous-gun ( -- )
  get-gun 1- dup 0< if drop max-gun# then change-gun ;

10 cconstant arming-interval \ ticks

: schedule-arming ( -- ) arming-time arming-interval schedule ;

: arming ( -- )
  arming-time already? 0exit
  kk-bullet-gun pressed?   if
    bullet-gun#  change-gun schedule-arming exit then
  kk-missile-gun pressed?  if
    missile-gun# change-gun schedule-arming exit then
  kk-ball-gun pressed?     if
    ball-gun#    change-gun schedule-arming exit then
  kk-previous-gun pressed? if
    previous-gun            schedule-arming exit then
  kk-next-gun pressed?     if
    next-gun                schedule-arming      then ;

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

: arrive ( n -- ) dup announce landscape>screen building ;
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
  s" El ataque ha sido rechazado, " s,

here \ eo (Esperanto)
   s" La atako estis repuŝita, " s,

here \ en (English)
  s" The attack has been repelled, " s,

localized-string about-repelled-attack$ ( -- ca len )

here \ es (Spanish)
  s" pero los muros están dañados." s,

here \ eo (Esperanto)
   s" sed la muroj estas damaĝitaj." s,

here \ en (English)
  s" but the walls are damaged." s,

localized-string about-damaged-walls$ ( -- ca len )

here \ es (Spanish)
  s" Se prevé un nuevo ataque inminente." s,

here \ eo (Esperanto)
   s" Nova tuja atako antaŭvideblas." s,

here \ en (English)
  s" A new imminent attack is expected." s,

localized-string about-new-attack$ ( -- ca len )

here \ es (Spanish)
  s" Los invasores han sido aniquilados "
  s" y el edificio está en buen estado. " s+ s,

here \ eo (Esperanto)
  s" La invadantoj estis destruitaj "
  s" kaj la konstruaĵo bonstatas. " s+ s,

here \ en (English)
  s" The invaders have been destroyed "
  s" and the building is in good condition. " s+ s,

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

: any-breach? ( -- f ) breaches c@ 0<> ;

: about-attack ( -- )
  well-done$                                     paragraph
  about-repelled-attack$ about-damaged-walls$ s+ paragraph
  about-new-attack$                              paragraph ;

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

: .bullets-icon ( -- ) bullets-icon-x status-bar-y at-xy
                       bullet-sprite emit-udga ;

: .missiles-icon ( -- ) missiles-icon-x status-bar-y at-xy
                        missile-sprite emit-udga ;

: .balls-icon ( -- ) balls-icon-x status-bar-y at-xy
                     ball-sprite emit-udga ;

' .bullets-icon   bullet-gun~ ~gun-icon-displayer !
' .missiles-icon missile-gun~ ~gun-icon-displayer !
' .balls-icon       ball-gun~ ~gun-icon-displayer !

: .record-separator ( -- )
  record-separator-x status-bar-y at-xy '/' emit ;

: color-status-bar ( b -- ) attributes columns rot fill ;

: hide-status-bar ( -- ) sky-attr color-status-bar ;

: reveal-status-bar ( -- )
  status-attr color-status-bar highlight-gun ;

: (status-bar ( -- )
  get-gun .bullets-icon  .bullets  space
          .missiles-icon .missiles space
          .balls-icon    .balls
  set-gun .score .record-separator .record ;

: status-bar ( -- )
  hide-status-bar (status-bar reveal-status-bar ;

  \ ===========================================================
  cr .( Main loop) ?depth debug-point \ {{{1

: invaders-destroyed? ( -- f ) #invaders c@ 0= ;

: extermination? ( -- f )
  invaders-destroyed? mothership-destroyed? and ;

: attack-wave ( -- )
  init-mothership init-invaders reset-dticks ;

: fight ( -- )
  [debugging] [if] ?debug-bar [then] \ XXX TMP --
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
  attack-wave begin fight end-of-attack? until
  lose-projectiles ;

: another-attack? ( -- f ) any-breach? catastrophe? 0= and ;

: -recharge ( n1 n2 -- )
  set-gun gun~ ~gun-projectile-stack @ dup xstack xclear
                                       swap recharge ;
  \ Make the gun _n2_ the current one, then empty and recharge
  \ it with _n1_ projectiles from the used projectiles stack.

: (recharge-guns ( -- )
  #bullets  bullet-gun#  -recharge unhighlight-gun
  #missiles missile-gun# -recharge unhighlight-gun
  #balls    ball-gun#    -recharge unhighlight-gun ;
  \ Empty and recharge all guns.

: recharge-guns ( -- )
  get-gun unhighlight-gun (recharge-guns
  set-gun   highlight-gun ;
  \ Empty and recharge all guns, preserving the current gun.

: prepare-attack ( -- )
  repair-tank prepare-projectiles status-bar recharge-guns
  used-projectiles off
  [debugging] [if] debug-bar [then] ;

: prepare-battle ( -- ) settle new-tank ;

: battle ( -- )
  prepare-battle
  begin prepare-attack under-attack another-attack?
  while attack-report repeat ;

: campaign ( -- ) begin battle catastrophe? 0=
                  while battle-report reward travel repeat ;

: prepare-war ( -- )
  catastrophe off first-location score off cls ;

: war ( -- ) prepare-war campaign defeat ;

: run ( -- ) begin mobilize war again ;

  \ ===========================================================
  cr .( Debugging tools) ?depth debug-point \ {{{1

: h ( -- ) text-attr attr! home ;

: half ( -- ) half-max-invaders c!> max-invaders ;
  \ Reduce the actual invaders to the left half.

: mp ( -- ) manage-projectiles ;
: fp? ( -- 0f ) flying-projectiles? ;
: pa ( -- ) bullet-and-missile-action ;
: ba ( -- ) ball-action ;
: np ( -- ) next-flying-projectile ;

: .fp ( -- )
  #flying-projectiles c@ 0 ?do
    i flying-projectiles array> @ u.
  loop cr ;

: ni ( -- ) next-invader ;
: mi ( -- ) manage-invaders ;
: ia ( -- ) invader~ ~invader-action perform ;
: ini ( -- ) prepare-war prepare-battle prepare-attack
             attack-wave ;

: b ( -- ) cls building h ; \ building
: t ( -- ) .tank h ;
: tl ( -- ) <tank h ; \ move tank left
: tr ( -- ) tank> h ; \ move tank right

: mm ( -- ) manage-mothership ;
: ima ( -- ) invisible-mothership-action text-attr attr! ;
: vma ( -- ) visible-mothership-action text-attr attr! ;
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
: nmx ( -- ) new-mothership-x-inc . ;
: (nmx ( -- ) (new-mothership-x-inc . ;


: beon ( -- ) beam-on ;
: beoff ( -- ) beam-off ;
: beu ( -- ) beaming-up-mothership-action ;
: bed ( -- ) beaming-down-mothership-action ;

: test-be ( -- )
  mothership-x off .visible-mothership beam-on m ;

: left-only ( -- ) half-max-invaders c!> max-invaders ;
  \ Reduce the actual invaders to the left half.

: (kill ( n1 n2 -- )
  ?do i invader#>~ ~invader-stamina coff loop ;
  \ Kill invaders from _n2_ to _n1_, not including _n1_.

: kill-left ( -- ) half-max-invaders 0 (kill ;
  \ Kill the left-side invaders.

: kill-right ( -- ) max-invaders half-max-invaders (kill ;
  \ Kill the right-side invaders.

: .i ( n -- )
  >r
  ." sprite         " r@ invader#>~ ~invader-sprite @ . cr
  ." frame          " r@ invader#>~ ~invader-frame c@ . cr
  ." frames         " r@ invader#>~ ~invader-frames c@ . cr
  ." stamina        " r@ invader#>~ ~invader-stamina c@ . cr
  ." x              " r@ invader#>~ ~invader-x c@ . cr
  ." initial-x      " r@ invader#>~ ~invader-initial-x c@ . cr
  ." x-inc          " r@ invader#>~ ~invader-x-inc ? cr
  ." initial-x-inc  " r@ invader#>~ ~invader-initial-x-inc ? cr
  ." y              " r@ invader#>~ ~invader-y c@ . cr
  ." action         " r@ invader#>~ ~invader-action @ >name
                         .name cr
  ." species        " r@ invader#>~ ~invader-species dup u.
                         cr @
  ." SPECIES DATA:" cr
  rdrop >r
  ." flying>-sprite " r@ ~species-flying>-sprite @ . cr
  ." docked-sprite  " r@ ~species-docked-sprite @ . cr
  rdrop ;

: bc ( -- )
  cls
  \ top:
  space broken-top-right-container emit-udga
  container-top emit-2udga
  broken-top-left-container emit-udga space cr
  \ bottom:
  container-bottom emit-2udga 8 emit 8 emit
  broken-bottom-left-container emit-udga
  xy swap 1+ swap at-xy
  container-bottom emit-2udga
  container-bottom emit-2udga 8 emit
  broken-bottom-right-container emit-udga cr ;
  \ Display the graphics of the broken containers.

: sky ( -- )
  attributes /attributes bounds ?do
    i c@ sky-attr= if   [ cyan papery brighty ] cliteral i c!
                   then loop ;
  \ Reveal the zones of the screen that have the sky attribute,
  \ by coloring them with brighy cyan.  This is useful to
  \ discover attributes that shoud be sky-colored but have been
  \ contaminated by the movement of the sprites, because of
  \ wrong calculations in the code.

  \ ===========================================================
  cr .( Development benchmarks) ?depth debug-point \ {{{1

  \ --------------------------------------------
  \ Compare double use of `invader~ ~invader-species @`

0 [if]

  \ 2018-02-16
  \ 2018-02-17: Update names.

need timer

: bench-species ( n -- )
  dup ticks swap 0 ?do
        invader~ ~invader-species @
        ~species-<flying-sprite @
        invader~ ~invader-species @
        ~species-<flying-frames c@
        drop drop
        loop cr timer ."  duplicating the fetch"
      ticks swap 0 ?do
        invader~ ~invader-species @ dup
        ~species-<flying-sprite @ swap
        ~species-<flying-frames c@
        drop drop
        loop cr timer ."  duplicating the value" ;

[then]

  \ |==========================================
  \ |       | Frames
  \ |       | =================================
  \ | Times | duplicate fetch | duplicate value
  \
  \ |   100 |               3 |               2
  \ |  1000 |              32 |              25
  \ | 10000 |             322 |             247
  \ | 65535 |            2107 |            1622
  \ |==========================================


  \ --------------------------------------------
  \ Compare `type-udg` and `.2x1-udg-sprite`

0 [if]

  \ 2017-07-27: Benchmark.
  \ 2018-02-17: Update word name.

need bench{ need }bench.

: sprite-string-bench ( n -- )
  dup page ." type-udg :"
  bench{ 0 ?do 18 0 at-xy right-arrow$ type-udg
            loop }bench.
  0 1 at-xy ." emit-2udga :"
  bench{ 0 ?do 18 1 at-xy right-arrow emit-2udga
           loop }bench. ;

  \ |==============================
  \ |       | Frames
  \ |       | =====================
  \ | Times | type-udg | emit-2udga
  \
  \ |  1000 |      112 |         94
  \ | 10000 |     1122 |        937
  \ | 65535 |     7353 |       6142
  \ |==============================

[then]

  \ --------------------------------------------
  \ Compare alternative implementations of `hit-wall?`

0 [if]

  \ 2018-01-24
  \ 2018-02-17: Update field names.

need ticks need timer

0 constant flying-to-the-left? ( f )

: invader-front-xy ( -- col row )
  invader~ ~invader-x
  [ udg/invader 2 = ]
  [if]   c@2+ flying-to-the-left? 3* +
  [else] [ udg/invader 1 = ]
         [if]   c@1+ flying-to-the-left? 2* +
         [else] c@ udg/invader + flying-to-the-left?
                [ udg/invader 1+ ] cliteral * +
         [then]
  [then]
  invader~ ~invader-y c@ ;
  \ Return the coordinates _col row_ at the front of the
  \ current invaders, i.e. the location the invader is heading
  \ to on its current direction.

: hit-wall?-OLD ( -- f )
  invader-front-xy xy>attr brick-attr = ;
  \ Has the current invader hit the wall of the building?

: invader-front-x ( -- col )
  invader~ ~invader-x
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
                         building-left-x ;

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

[breakable] [if]
       \ <------------------------------>
cr cr .( Type BREAKABLE to make the game)
   cr .( breakable by the BREAK key.)
[then]

cr cr .( Type RUN to start.) cr

end-program

  \ vim: filetype=soloforth:colorcolumn=64:textwidth=63
