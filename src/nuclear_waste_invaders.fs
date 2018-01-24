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

: version$ ( -- ca len ) s" 0.157.0+201801242114" ;

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
  cr .(   -Development tools) .s \ {{{2

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

  \ --------------------------------------------
  cr .(   -Memory) ?depth \ {{{2

need c+! need c-! need c1+! need c1-! need ?c1-! need coff
need dzx7t need bank-start need c@1+ need c@1- need c@2+
need 1+!

  \ --------------------------------------------
  cr .(   -Math) ?depth \ {{{2

need d< need -1|1 need 2/ need between need random need binary
need within need even? need 8* need random-between
need join need 3*

  \ --------------------------------------------
  cr .(   -Data structures) ?depth \ {{{2

need roll need cfield: need field: need +field-opt-0124
need array> need !> need c!> need 2!>

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

here ," PUNTUACIÓN"
here ," POENTARO"
here ," SCORE"
localized-string score$ ( -- ca len )
  \ Return string _ca len_ in the current language.
  \
  \ XXX REMARK -- Not used.

here ," P:"
here ," P:"
here ," S:"
localized-string score-label$ ( -- ca len )
  \ Return string _ca len_ in the current language.

: missiles-label$ s" M:" ( -- ca len ) ;
  \ Return string _ca len_ in the current language.

here ," B:"
here ," K:"
here ," B:"
localized-string bullets-label$ ( -- ca len )
  \ Return string _ca len_ in the current language.

here ," RÉCOR"
here ," RIKORDO"
here ," RECORD"
localized-string record$ ( -- ca len )
  \ Return string _ca len_ in the current language.
  \
  \ XXX REMARK -- Not used.

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

  \ Note: `projectile-attr` must be different to all attributes
  \ used by the invaders.

                         black cconstant sky-attr

                         green cconstant healthy-invader-attr
                        yellow cconstant wounded-invader-attr
                           red cconstant dying-invader-attr

                         white cconstant tank-attr
                   red brighty cconstant projectile-attr

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
0 cconstant kk-down#  0. 2constant kk-down

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

4 cconstant /controls
  \ Bytes per item in the `controls` table.

create controls
  \ left    right     fire       down
  kk-5# c,  kk-8# c,  kk-en# c,  kk-6#  c, \ cursor+enter
  kk-r# c,  kk-t# c,  kk-en# c,  kk-m#  c, \ Spanish Dvorak
  kk-z# c,  kk-x# c,  kk-en# c,  kk-sp# c, \ QWERTY
  kk-5# c,  kk-8# c,  kk-0#  c,  kk-6#  c, \ cursor joystick
  kk-5# c,  kk-8# c,  kk-sp# c,  kk-6#  c, \ cursor+space
  kk-1# c,  kk-2# c,  kk-5#  c,  kk-3#  c, \ Sinclair 1
  kk-6# c,  kk-7# c,  kk-0#  c,  kk-8#  c, \ Sinclair 2
  kk-o# c,  kk-p# c,  kk-q#  c,  kk-a#  c, \ QWERTY
  kk-n# c,  kk-m# c,  kk-q#  c,  kk-a#  c, \ QWERTY
  kk-q# c,  kk-w# c,  kk-p#  c,  kk-en# c, \ QWERTY
  kk-z# c,  kk-x# c,  kk-p#  c,  kk-en# c, \ QWERTY

here controls - /controls / cconstant max-controls
  \ Number of controls stored in `controls`.

max-controls 1- cconstant last-control

: >controls ( n -- a ) /controls * controls + ;
  \ Convert controls number _n_ to its address _a_.

: set-controls ( n -- )
  >controls     dup c@  dup c!> kk-left#   #>kk 2!> kk-left
             1+ dup c@  dup c!> kk-right#  #>kk 2!> kk-right
             1+ dup c@  dup c!> kk-fire#   #>kk 2!> kk-fire
             1+     c@  dup c!> kk-down#   #>kk 2!> kk-down ;
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

               156 cconstant last-udg \ last UDG code used
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

                bullets-label$ nip cconstant /bullets-label
               missiles-label$ nip cconstant /missiles-label

                                 2 cconstant ammo-digits
                                 5 cconstant score-digits

                                 0 cconstant status-bar-y

                                 0 cconstant bullets-label-x
  bullets-label-x /bullets-label + cconstant bullets-x
        bullets-x ammo-digits + 1+ cconstant missiles-label-x
missiles-label-x /missiles-label + cconstant missiles-x

            columns score-digits - cconstant record-x
                       record-x 1- cconstant record-separator-x
 record-separator-x score-digits - cconstant score-x
        score-x score-label$ nip - cconstant score-label-x

: [#] ( n -- ) 0 ?do postpone # loop ; immediate compile-only
  \ Compile `#` _n_ times.

: (.score ( n col row -- )
  at-xy s>d <# [ score-digits ] [#] #> text-attr attr! type ;
  \ Display score _n_ at coordinates _col row_.

' xdepth alias projectiles-left ( -- n )

0 cconstant ammo-x

: .ammo ( n -- )
  projectiles-left s>d <# [ ammo-digits ] [#] #>
  text-attr attr! ammo-x status-bar-y at-xy type ;
  \ Display the current ammo left at the status bar.

0 cconstant gun-machine-id
1 cconstant missile-gun-id

defer set-arm ( n -- )
  \ Set the current arm (0=gun machine; 1=missile gun).

: .bullets ( -- ) gun-machine-id set-arm .ammo ;
  \ Display the number of bullets left.

: .missiles ( -- ) missile-gun-id set-arm .ammo ;
  \ Display the number of bullets left.
  \
  \ XXX TODO --

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

2 1 udg-sprite

......XXXX......
...XXXXXXXXXX...
..XXXXXXXXXXXX..
..XXX..XX..XXX..
..XXXXXXXXXXXX..
.....XX..XX.....
....XX.XX.XX....
..XX........XX.. sprite-id left-flying-invader-0-sprite

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
..XX........XX.. sprite-id right-flying-invader-0-sprite

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
..XX........XX.. sprite-id docked-invader-0-sprite

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
....X.....X..... sprite-id left-flying-invader-1-sprite

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
.....X.....X.... sprite-id right-flying-invader-1-sprite

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
.....XX.XX...... sprite-id docked-invader-1-sprite

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
....X.X..X.X.... sprite-id left-flying-invader-2-sprite

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
....X.X..X.X.... sprite-id right-flying-invader-2-sprite

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
....X.X..X.X.... sprite-id docked-invader-2-sprite

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
...X........X... sprite-id flying-mothership-sprite

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
X.X.X.X..X.X.X.X sprite-id beaming-mothership-sprite

2 1 udg-sprite

................
.....XXXXXX.....
...XXXXXXXXXX...
..XXXXXXXXXXXX..
.XX..XX..XX..XX.
XXXXXXXXXXXXXXXX
X.X.X.X..X.X.X.X
.X.X.X.XX.X.X.X. drop

  \ -----------------------------------------------------------
  \ Explosion

9 cconstant explosion-frames

2 1 udg-sprite

..............X.
..X......XX..X..
.X...XXXXXXX....
....XXXXXXXXX.X.
...XXXX.XX.XX..X
.X..XX..XXXX....
X....XXXXX...X..
..X...XX...X..X. sprite-id explosion-sprite

2 1 udg-sprite

................
...X...X...X....
.....XXXXXXX..X.
.X..X.XXXX.XX...
...XX.XXX.XXX...
..X.X.XXXX.X..X.
.X...XX.XX..X...
.X...XX...X..X.X drop

2 1 udg-sprite

.X...X..........
X...X....X...X..
...X.XXXX.XX....
...XXXX.XX.XX..X
X...X.XXX.X.XX..
....X.XX.X.XX..X
...X.XX.XXX.XX..
X...XX.X..X..... drop

2 1 udg-sprite

X......X........
....X....X......
...X.X.XX.X.....
X..X.X...X.XX..X
....X.X.X.X.XX..
..X...X..X.XX...
...X.XX.X.X.XX..
.X..X..........X drop

2 1 udg-sprite

..X.............
....X....X..X...
X..X.X.X..X..X..
...X..X..X......
....X.....X.X...
.X....X..X.X..X.
...X.X....X.....
....X....X...X.. drop

2 1 udg-sprite

X............X..
....X....X......
.......X..X..X..
..X...........X.
..........X.....
.X....X.......X.
..........X.....
..X.......X....X drop

2 1 udg-sprite

.X.........X....
.....X.....X..X.
...............X
................
X...............
...........X....
X.....X........X
..X........X.... drop

2 1 udg-sprite

....X.......X..X
................
................
................
................
...........X....
.............X..
......X......... drop

2 1 udg-sprite

................
................
................
................
................
................
................
................ drop

  \ -----------------------------------------------------------
  \ Projectiles

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
                      [then] cconstant frames/bullet

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
                       [then] cconstant frames/missile

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
....X.X.X.X.X.X.X.X.X... sprite-id gun-machine-tank-sprite

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

  \ -----------------------------------------------------------
  \ Containers

2 1 udg-sprite

......XXXXX.....
...XXX.....XXX..
..X...XXXXX...X.
..X...........X.
..X....XXX....X.
..X...XXXXX...X.
..X....XXX....X.
..X.....X.....X. sprite-id container-top

1 1 udg-sprite

........
...XXX..
..X...X.
..X...X.
..X...X.
..X....X
..X....X
..X....X sprite-id broken-top-left-container

1 1 udg-sprite

........
...XXX..
..X...X.
..X...X.
.X....X.
.X....X.
.X....X.
X.....X. sprite-id broken-top-right-container

2 1 udg-sprite

..X..X.X.X.X..X.
..X.XXXX.XXXX.X.
..X.XXX...XXX.X.
..X..XX...XX..X.
..X...........X.
...XXX.....XXX..
......XXXXX.....
................ sprite-id container-bottom

1 1 udg-sprite

.......X
.....XXX
...XXXX.
..X..XX.
..X.....
...XXX..
......XX
........ sprite-id broken-bottom-left-container

1 1 udg-sprite

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
  cfield: ~flying-left-sprite         \ UDG
  cfield: ~flying-left-sprite-frames  \ count
  cfield: ~flying-right-sprite        \ UDG
  cfield: ~flying-right-sprite-frames \ count
  cfield: ~docked-sprite              \ UDG
  cfield: ~docked-sprite-frames       \ count
cconstant /species
  \ Data structure of an invader species.

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

  cfield: ~y              \ row
  cfield: ~x              \ column
  cfield: ~sprite         \ UDG
  cfield: ~frames         \ count
  cfield: ~frame          \ counter
  cfield: ~initial-x      \ column
  field:  ~x-inc          \ -1|1
  field:  ~initial-x-inc  \  -1|1
  cfield: ~stamina        \ 0..3
  field:  ~action         \ execution token
  field:  ~species        \ data structure address
  field:  ~explosion-time \ ticks clock time
  cfield: ~layer          \ 0 (lowest) .. 4 (highest)
cconstant /invader
  \ Data structure of an species.

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

: layer>y ( n -- row )
  top-invader-layer swap - rows/layer * invader-top-y + ;
  \ Convert invader layer _n_ to its equivalent row _row_. The
  \ pair of invaders that fly nearest the ground are layer 0.
  \ The pair above them are layer 1, and so on.

: y>layer ( row -- n ) rows/layer / 1- invader-top-y swap - ;
  \ Convert invader row _row_ to its equilavent layer _n_. The
  \ pair of invaders that fly nearest the ground are layer 0.
  \ The pair above them are layer 1, and so on. Note: _row_ is
  \ supposed to be a valid row of an invader layer, otherwise
  \ the result will be wrong.

: y>layer? ( row -- n f )
  invader-top-y - rows/layer /mod swap 0= ;
  \ If _row_ is a valid row of an invader layer, return layer
  \ _n_ and _f_ is _true_; otherwise _n_ is invalid and _f_ is
  \ false.  The pair of invaders that fly nearest the ground
  \ are layer 0.  The pair above them are layer 1, and so on.
  \ Note: If _row_ is greater than the last invader layer, the
  \ result will be wrong.
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

: set-invader-sprite ( c n -- )
  invader~ ~frames c! invader~ ~sprite c!
  invader~ ~frame coff ;
  \ Set character _c_ as the first character of the first
  \ sprite of the current invader, and _n_ as the number of
  \ frames.

: set-flying-invader-sprite ( -- )
  invader~ ~species @ dup
  flying-to-the-left?
  if   ~flying-left-sprite c@ swap
       ~flying-left-sprite-frames c@
  else ~flying-right-sprite c@ swap
       ~flying-right-sprite-frames c@
  then set-invader-sprite ;
  \
  \ XXX TODO -- Use double-cell fields to copy both fields with
  \ one operation or use `move`.
  \
  \ XXX TODO -- If the maximum frames in both directions are
  \ identical, there's no need to initiate `~frame`.
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
  species#>~ invader~ ~species !
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

: create-invaders ( n1 n2 -- )
  ?do i set-invader
      mothership-stamina invader~ ~stamina c!
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

create stamina-attributes ( -- ca )   dying-invader-attr c,
                                    wounded-invader-attr c,
                                    healthy-invader-attr c,
  \ Table to index the stamina (1..3) to its proper attribute.

: stamina>attr ( n -- c )
  [ stamina-attributes 1- ] literal + c@ ;
  \ Convert stamina _n_ to its corresponding attribute _c_.

: invader-attr ( -- c ) invader~ ~stamina c@ stamina>attr ;
  \ Return attribute _c_ corresponding to the stamina of the
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
  \ Return number _n_ of alive invaders that are at the right
  \ of the building.

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

: floor ( row -- )
  building-left-x c@ swap at-xy
  brick-attr attr! brick building-width c@ .1x1sprites ;
  \ Draw a floor of the building at row _row_.

: ground-floor ( row -- )
  building-left-x c@1+ swap at-xy
  door-attr attr!  left-door emit-udg
  brick-attr attr! brick building-width c@ 4 - .1x1sprites
  door-attr attr!  right-door emit-udg ;
  \ Draw the ground floor of the building at row _row_.

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

variable used-projectiles
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
  gun-x building-left-x c@ building-right-x c@ between ;
  \ Is the tank's gun below the building?

: tank-rudder ( -- -1|0|1 )
  kk-left pressed? kk-right pressed? abs + ;
  \ Does the tank move? Return its column increment.

: outside? ( col -- f )
  building-left-x c@1+ building-right-x c@ within 0= ;
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

: .tank-arm ( -- )
  tank-attr attr!
  tank-x c@ 1+ dup tank-y at-xy
                   tank-arm-udg ?emit-outside drop ;
  \ If the middle part of the tank is visible (i.e. outside the
  \ building), display it.

: new-tank ( -- )
  repair-tank gun-machine-id set-arm park-tank .tank ;

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

  \ ===========================================================
  cr .( Projectiles) ?depth debug-point \ {{{1

50 cconstant #bullets
  \ Number of bullets the tank can hold.

10 cconstant #missiles
  \ Number of missiles the tank can hold.

#bullets #missiles + cconstant #projectiles
  \ Total number of projectiles the tank can hold.

#bullets allot-xstack constant bullets-stack
  \ Create an extra stack to store the unused bullets.

#missiles allot-xstack constant missiles-stack
  \ Create an extra stack to store the unused missiles.

0
  cfield: ~projectile-y         \ row
  cfield: ~projectile-x         \ column
  cfield: ~projectile-sprite    \ UDG (*)
  cfield: ~projectile-frames    \ count (*)
  cfield: ~projectile-altitude  \ row (*)
  cfield: ~projectile-delay     \ counter
  cfield: ~projectile-max-delay \ bitmask (*)
cconstant /projectile
  \ Data structure of a projectile.
  \
  \ (*) = Constant value copied by `get-projectile` from the
  \       structure pointed by `arm~`.


#projectiles /projectile * constant /projectiles

create projectiles /projectiles allot
  \ Projectiles data table.

: projectile#>~ ( n -- a ) /projectile * projectiles + ;
  \ Convert projectile number _n_ to its data address _a_.

0 projectile#>~ constant projectile~
  \ Data address of the current projectile in the data table.

cvariable #flying-projectiles
  \ Counter: number of currently flying projectiles.

cvariable #flying-projectile
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
  \ Array of flying projectiles

: start-flying ( a -- )
  #flying-projectiles c@ flying-projectiles array> !
  #flying-projectiles c1+! used-projectiles 1+! ;
  \ Store projectile _a_ into the array of flying projectiles
  \ and update the count of currently flying projectiles.

: stop-flying ( n -- )
  flying-projectiles /flying-projectiles rot 1+ cells /string
  over cell- swap cmove
  #flying-projectiles c1-! ;
  \ Remove projectile _n_ from the array of flying projectiles
  \ and update the count of currently flying projectiles.

: destroy-projectile ( -- ) #flying-projectile c@ stop-flying ;

: recharge-bullets ( -- )
  bullets-stack xstack xclear
  #bullets 0 do i projectile#>~ >x loop ;

: recharge-missiles ( -- )
  missiles-stack xstack xclear
  #projectiles #bullets do i projectile#>~ >x loop ;

: recharge-projectiles ( -- )
  recharge-bullets recharge-missiles ;

: prepare-projectiles ( -- ) #flying-projectiles coff
                             #flying-projectile coff
                             projectiles /projectiles erase ;

: new-projectiles ( -- ) prepare-projectiles
                         recharge-projectiles
                         used-projectiles off ;

: projectile-coords ( -- col row | gx gy )
  projectile~ ~projectile-x c@ projectile~ ~projectile-y c@ ;
  \ Coordinates of the projectile.

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
  [ udg/invader 2 = ] [if] 2* [else] udg/invader * [then]
  invader~ ~sprite c@ + ;
  \ First UDG _c_ of the current frame of the current invader's
  \ sprite, calculated from its sprite and its frame.
  \
  \ XXX TODO -- Add calculation to change the sprite depending
  \ on the flying direction. A flag field is needed to
  \ deactivate this for docked invaders.

: .invader ( -- )
  invader-attr attr! invader-udg .2x1-udg-sprite ;
  \ Display the current invader.  at the cursor coordinates, in
  \ its proper attribute.

: broken-bricks-coordinates
  ( col1 -- col1 row1 col2 row2 col3 row3 )
  invader~ ~y c@ 2dup 1+ 2dup 2- ;
  \ Convert the column _col1_ of the broken wall to the
  \ coordinates of the broken brick above the invader, _col3
  \ row3_, below it, _col3 row3_, and in front of it, _col1
  \ row1_.

defer break-the-wall ( col1 row1 col2 row2 -- )
  \ Display the broken wall at the given coordinates of the
  \ broken brick above the invader, _col3 row3_, and below it,
  \ _col2 row2_, and in front of it, _col1 row1_.
  \ The action of this deferred word is set to
  \ `break-left-wall` or `break-right-word`.

: break-left-wall ( col1 row1 col2 row2 -- )
  at-xy broken-top-left-brick .1x1sprite
  at-xy broken-bottom-left-brick .1x1sprite
  at-xy space ;
  \ Display the broken left wall at the given coordinates of
  \ the broken brick above the invader, _col3 row3_, and below
  \ it, _col2 row2_, and in front of it, _col1 row1_.
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
  \ compile-time, depending on the size of the invaders, just
  \ in case. See `hit-wall?`.

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

defer flying-left-invader-action ( -- )
  \ Action of the invaders that are moving to the left.

defer flying-right-invader-action ( -- )
  \ Action of the invaders that are moving to the right.

      ' flying-left-invader-action ,
here  ' noop ,
      ' flying-right-invader-action ,
      constant flying-invader-actions ( a )
      \ Execution table.

: set-invader-move-action ( -1..1 -- )
  flying-invader-actions array> @ invader~ ~action ! ;
  \ Set the action of the current invader after x-coordinate
  \ increment _-1..1_.

: set-invader-direction ( -1..1 -- )
  dup 0< !> flying-to-the-left?
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

: turn-back ( -- ) change-direction set-flying-invader-sprite ;
  \ Make the current invader turn back.

defer breaking-invader-action ( -- )
  \ Action of the invaders that are breaking the wall.

: hit-wall ( -- )
  healthy? if   ['] breaking-invader-action invader~ ~action !
                exit
           then turn-back ;
  \ XXX TMP --

: invader-front-xy ( -- col row )
  invader~ ~x
  [ udg/invader 2 = ]
  [if]   c@2+ flying-to-the-left? 3* +
  [else] [ udg/invader 1 = ]
         [if]   c@1+ flying-to-the-left? 2* +
         [else] c@ udg/invader + flying-to-the-left?
                [ udg/invader 1+ ] cliteral * +
         [then]
  [then] invader~ ~y c@ ;
  \ Return the coordinates _col row_ at the front of the
  \ current invader.

: hit-wall? ( -- f ) invader-front-xy xy>attr brick-attr = ;
  \ Has the current invader hit the wall of the building?

: ?damages ( -- )
  hit-wall? if hit-wall exit then
  break-container? dup catastrophe ! 0exit break-container ;
  \ Manage the possible damages caused by the current invader.

: undock ( -- ) invader~ ~initial-x-inc @ set-invader-direction
                set-flying-invader-sprite ;
  \ Undock the current invader.

: is-there-a-projectile? ( col row -- f )
  xy>attr projectile-attr = ;

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
  invader~ ~x c1-! at-invader .invader .sky ?flying ;
  ' flying-left-invader-action defer!
  \ Move the current invader, which is flying to the left,
  \ unless a projectile is at the left.

:noname ( -- )
  right-of-invader is-there-a-projectile?
  if docked? ?exit turn-back exit then
  at-invader .sky .invader invader~ ~x c1+! ?flying ;
  ' flying-right-invader-action defer!
  \ Move the current invader, which is flying to the right,
  \ unless a projectile is at the right.

cvariable cure-factor  20 cure-factor c!
  \ XXX TMP -- for testing

: difficult-cure? ( -- f )
  max-stamina invader~ ~stamina c@ -
  cure-factor c@ * \ XXX TMP -- for testing
  random 0<> ;
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
           then at-invader .invader
  ; ' docked-invader-action defer!
  \ Action of the invaders that are docked.

: dock ( -- ) ['] docked-invader-action invader~ ~action !
                  set-docked-invader-sprite ;
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
  \
  \ XXX TODO -- Reuse the calculation already done in
  \ `hit-wall?`. Better yet: Keep the columns in a table of
  \ constants, two per level, calculated at compile-time.

: break-wall ( -- )
  prepare-wall (break-wall new-breach impel-invader ;
  \ Break the wall the current invader has hit.

: ?break-wall ( -- ) invaders c@ random ?exit break-wall ;
  \ Break the wall randomly, depending on the number of
  \ invaders.
  \
  \ XXX TODO -- Improve the random calculation. Why use
  \ `invaders`?

:noname ( -- ) ?break-wall at-invader .invader
               ; ' breaking-invader-action defer!
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

defer set-mothership-direction ( -1..1 -- )

: mothership-turns-back ( -- )
  mothership-stopped off
  mothership-x-inc @ negate set-mothership-direction ;

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

: mothership-impacted ( -- )
  mothership-stamina 1- dup c!> mothership-stamina ?dup
  if   set-mothership-stamina damage-sound
       mothership-turns-back
  else set-exploding-mothership then ;

: start-mothership ( -- ) -1|1 set-mothership-direction ;

: init-mothership ( -- )
  1 motherships c!
  max-stamina set-mothership-stamina
  set-flying-mothership-sprite set-invisible-mothership-action
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
  [ udg/mothership 2 = ] [if] 2* [else] udg/mothership * [then]
  mothership + ;
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

0 layer>y cconstant invader-min-y \ bottom

4 layer>y cconstant invader-max-y \ top

healthy-invader-attr dup join
constant healthy-invader-cell-attr

sky-attr dup join constant sky-cell-attr

 variable beam-y-inc   \ 1|-1
 variable beam-y       \ row
cvariable beam-first-y \ row
cvariable beam-last-y  \ row

cvariable beam-invader
  \ Number of the next invader to be created by the beam.

: set-beam ( row1 row2 xt -- )
  mothership-action!
  2dup beam-last-y c! dup beam-first-y c! beam-y !
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
  dup invader-top-y invader-bottom-y between
      swap rows/layer mod 0= and ;
  \ Is _row_ is a valid row of an invader layer?

: reach-invader? ( -- f ) beam-y @ layer-y? ;
  \ Has the beam reached the row of an invader's layer?

: update-beam ( -- ) beam-y-inc @ beam-y +! ;

: (beaming-up? ( -- )
  beam-y @ beam-last-y c@ beam-first-y c@ between ;
  \ Is the beam still shrinking?

: beaming-up? ( -- f ) update-beam (beaming-up? ;
  \ Update a shrinking beam. Is it still shrinking?

: (beam-up ( -- )
  .mothership
  reach-invader? if   mothership-cell-attr
                 else sky-cell-attr
                 then mothership-x @ beam-y @ xy>attra ! ;
  \ Shrink the beam towards de mothership one character.

: beaming-up-mothership-action ( -- )
  (beam-up beaming-up? ?exit
  create-squadron
  set-flying-mothership-sprite set-visible-mothership-action ;
  \ Action of the mothership when the beam is shrinking.

: beam-off ( -- )
  invader-min-y 1+ mothership-y 1+
  ['] beaming-up-mothership-action set-beam ;
  \ Turn the mothership's beam off, i.e. start shrinking it
  \ back to the mothership.

: .new-invader ( -- )
  invader~ ~initial-x c@ invader~ ~y c@ at-xy
  invader~ ~sprite c@ .2x1-udg-sprite ;
  \ Display invader _n_ at its initial position, with the
  \ current attribute.

: create-invader ( -- )
  get-invader
    beam-invader dup c@ set-invader set-docked-invader-sprite
                     beam-attr attr! .new-invader
                 c1+!
  set-invader ;
  \ Display the new invader and update its number.

: (beaming-down? ( -- f )
  beam-y @ beam-first-y c@ beam-last-y c@ between ;
  \ Is the beam still growing?

: beaming-down? ( -- f ) update-beam (beaming-down? ;
  \ Update a growing beam. Is it still growing?

: (beam-down ( -- )
  .mothership beam-cell-attr mothership-x @ beam-y @ xy>attra !
  reach-invader? if create-invader then ;
  \ Grow the beam towards the ground one character.

: beaming-down-mothership-action ( -- )
  (beam-down beaming-down? ?exit beam-off ;
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
  mothership-y 1+ invader-min-y 1+
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
  move-visible-mothership
  visible-mothership? 0=
  if set-invisible-mothership-action exit then
  ?stop-mothership ; ' visible-mothership-action defer!
  \ Action of the mothership when it's visible.

:noname ( -- )
  advance-mothership visible-mothership?
  if   .visible-mothership set-visible-mothership-action exit
  then mothership-in-range? ?exit mothership-turns-back ;
  ' invisible-mothership-action defer!
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

: mothership-bonus ( -- n ) location c@1+ 250 * ;
  \ Bonus points for impacting the mothership.

:noname ( -- )
  set-exploding-mothership-sprite
  .mothership schedule-mothership-explosion
  ['] exploding-mothership-action mothership-action!
  mothership-bang mothership-bonus update-score
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
  projectile~ ~projectile-y c@ invader-top-y - 2/
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
  invader-bang invader-destroy-points update-score ;
  \ The current invader has been impacted. Set it accordingly.

' lightning1-sound alias retreat-sound
  \ XXX TMP --
  \ XXX TODO -- look for a proper sound

: retreat ( -- )
  retreat-sound turn-back invader-retreat-points update-score ;
  \ The current invader retreats.

: wounded ( -- )
  invader~ ~stamina c@1- 1 max invader~ ~stamina c! ;
  \ Reduce the invader's stamina after being shoot.

: mortal? ( -- f ) invader~ ~stamina c@ 2*
                   invader~ ~layer   c@    + random 0= ;
  \ Is it a mortal impact?  The random calculation depends on
  \ the stamina and the altitude: The more stamina and the more
  \ altitude, the less chances to be a mortal impact.

: invader-exploding? ( -- f )
  invader~ ~action @ ['] exploding-invader-action = ;
  \ Is the current invader exploding?

: (invader-impacted ( -- )
  invader-exploding? ?exit
  mortal? if set-exploding-invader exit then
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

: hit-something? ( -- f|0f )
  projectile-coords
  xy>attr [ sky-attr 0<> ] [if]   sky-attr <> ( f )
                           [else] ( 0f ) [then] ;
  \ Did the projectile hit something?

: impacted? ( -- f ) hit-something? dup if impact then ;
  \ If the projectil impacted, manage the impact and return
  \ _true_.  Otherwise do nothing and return _false_.

  \ ===========================================================
  cr .( Arms) ?depth debug-point \ {{{1

2 cconstant arms

cvariable arm#
  \ Number of the current tank arm:
  \ 0 = gun machine;
  \ 1 = missile gun.

0 constant arm~
  \ Data address of the current arm identified by `arm#`.

0
   field: ~arm-projectile-stack     \ address
  cfield: ~arm-projectile-sprite    \ UDG
  cfield: ~arm-projectile-frames    \ count
  cfield: ~arm-projectile-altitude  \ row
  cfield: ~arm-projectile-x         \ column
  cfield: ~arm-tank-sprite          \ UDG
  cfield: ~arm-trigger-interval     \ ticks
  cfield: ~arm-projectile-max-delay \ bitmask
cconstant /arm-projectile
  \ Data structure of an arm projectile.

arms /arm-projectile * cconstant /arm-projectiles

create arm-projectiles /arm-projectiles allot

: arm#>~ ( n -- a ) /arm-projectile * arm-projectiles + ;
  \ Convert arm number _n_ to its data address _a_.

gun-machine-id arm#>~ constant gun-machine~

missile-gun-id arm#>~ constant missile-gun~

:noname ( n -- )
  dup arm# c!
      arm#>~ dup !> arm~
             dup ~arm-tank-sprite c@ c!> tank-sprite
             dup ~arm-projectile-stack @ xstack
                 ~arm-projectile-x c@ c!> ammo-x
  ; ' set-arm defer!
  \ Set _n_ as the current arm (0=gun machine; 1=missile gun).

  \ --------------------------------------------
  \ Set arms' data

 bullets-stack gun-machine~ ~arm-projectile-stack !
missiles-stack missile-gun~ ~arm-projectile-stack !

 bullet-sprite gun-machine~ ~arm-projectile-sprite c!
missile-sprite missile-gun~ ~arm-projectile-sprite c!

frames/bullet  gun-machine~ ~arm-projectile-frames c!
frames/missile missile-gun~ ~arm-projectile-frames c!

invader-max-y   gun-machine~ ~arm-projectile-altitude c!
mothership-y 1+ missile-gun~ ~arm-projectile-altitude c!

 bullets-x gun-machine~ ~arm-projectile-x c!
missiles-x missile-gun~ ~arm-projectile-x c!

gun-machine-tank-sprite gun-machine~ ~arm-tank-sprite c!
missile-gun-tank-sprite missile-gun~ ~arm-tank-sprite c!

 8 gun-machine~ ~arm-trigger-interval c!
16 missile-gun~ ~arm-trigger-interval c!

%001 gun-machine~ ~arm-projectile-max-delay c!
%111 missile-gun~ ~arm-projectile-max-delay c!

  \ ===========================================================
  cr .( Shoot) ?depth debug-point \ {{{1

: at-projectile ( -- ) projectile-coords at-xy ;
  \ Set the cursor position at the coordinates of the
  \ projectile.

: projectile ( -- c )
  projectile~ ~projectile-sprite c@
  projectile~ ~projectile-frames c@ random + ;
  \ Return the UDG _c_ of a random frame of the projectile.

: .projectile ( -- )
  projectile-attr attr! at-projectile projectile .1x1sprite ;
  \ Display the projectile.

' whip-sound alias fire-sound ( -- )

: -projectile ( -- )
  projectile-coords xy>attr projectile-attr <> ?exit
  at-projectile .sky ;
  \ Delete the projectile.

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

: move-projectile ( -- )
  projectile-delay ?exit
  -projectile projectile-lost? if destroy-projectile exit then
  projectile~ ~projectile-y c1-!
  impacted? ?exit .projectile ;
  \ Manage the projectile.
  \
  \ XXX TODO -- Move `hit-something?` here to simplify the
  \ logic.

: schedule-trigger ( -- )
  ticks arm~ ~arm-trigger-interval c@ + trigger-time ! ;

: get-projectile ( -- )
  x> !> projectile~
  gun-x projectile~ ~projectile-x c!
  [ tank-y 1- ] cliteral projectile~ ~projectile-y c!
  arm~ ~arm-projectile-sprite c@
  projectile~ ~projectile-sprite c!
  arm~ ~arm-projectile-frames c@
  projectile~ ~projectile-frames c!
  arm~ ~arm-projectile-max-delay c@
  projectile~ ~projectile-max-delay c!
  arm~ ~arm-projectile-altitude c@
  projectile~ ~projectile-altitude c! ;
  \ Get a new projectile and set its data according to the
  \ current value of `arm~`.  For the sake of run-time speed,
  \ some fields are copied from the structure pointed by
  \ `arm~`.

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
  #flying-projectile c@1+ dup #flying-projectiles c@ < and
  dup #flying-projectile c!
      flying-projectiles array> @ !> projectile~ ;
  \ Point to the next flying projectile and make it the current
  \ one.

: manage-projectiles ( -- )
  flying-projectiles? 0exit
  move-projectile next-flying-projectile ;
  \ Manage a flying projectile, if any.

: lose-projectiles ( -- )
  begin manage-projectiles #flying-projectiles c@ 0= until ;
  \ Lose all flying projectiles.

: shooting ( -- )
  trigger-pressed?    0exit
  trigger-ready?      0exit
  projectiles-left    0exit
  gun-below-building? ?exit fire ;
  \ Manage the gun.

: toggle-arm ( -- ) arm# c@ 0= abs set-arm .tank-arm ;

10 cconstant arming-interval \ ticks

: schedule-arming ( -- )
  ticks arming-interval + arming-time ! ;

: arming ( -- ) arming-time @ past? 0exit
                kk-down pressed?    0exit
                toggle-arm schedule-arming ;

: manage-tank ( -- ) driving arming shooting ;

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

: .label ( ca len col -- ) status-bar-y at-xy type ;

: .bullets-label ( -- ) bullets-label$ bullets-label-x .label ;

: .missiles-label ( -- )
  missiles-label$ missiles-label-x .label ;

: .score-label ( -- ) score-label$ score-label-x .label ;

: .record-separator ( -- )
  record-separator-x status-bar-y at-xy '/' emit ;

: status-bar ( -- )
  text-attr attr!
  arm# c@ .bullets-label .bullets .missiles-label .missiles
  set-arm .score-label .score .record-separator .record ;

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

: under-attack ( -- )
  check-breachs attack-wave begin fight end-of-attack? until
  lose-projectiles ;

: another-attack? ( -- f ) breachs? catastrophe? 0= and ;

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

: invader-front-coords ( -- col row )
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
  invader-front-coords xy>attr brick-attr = ;
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
