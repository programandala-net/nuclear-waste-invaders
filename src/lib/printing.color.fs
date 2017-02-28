  \ printing.color.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201702241533

  \ -----------------------------------------------------------
  \ Description

  \ Words related to color.

  \ -----------------------------------------------------------
  \ Author

  \ Marcos Cruz (programandala.net), 2015, 2016, 2017.

  \ -----------------------------------------------------------
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

  \ -----------------------------------------------------------
  \ History

  \ 2016-05-01: Start. New words:
  \
  \   color@ color! color-mask@ color-mask! color 2color
  \   permcolor@ permcolor! permcolor-mask@ permcolor-mask!
  \   permcolor 2permcolor paper@ paper! ink@ ink!  bright@
  \   bright! flash! flash@
  \
  \ 2016-05-04: Move `inverse` and `overprint` from the kernel.
  \
  \ 2016-08-01: Move color constants, `papery`, `brighty` and
  \ `flashy` from _Nuclear Invaders_
  \ (http://programandala.net/en.program.nuclear_invaders.html).
  \
  \ 2016-12-02: Fix `bright!`.
  \
  \ 2016-12-03: Rename `>paper` to `paper>attr` and `paper>` to
  \ `attr>paper`, and rewrite them in Z80: much faster, and 2
  \ bytes smaller each.
  \
  \ 2016-12-16: Make `color@`, `color!`, `color-mask@`,
  \ `color-mask!`, `color` and `2color` individually accesible
  \ to `need`.
  \
  \ 2016-12-20: Rename `jpnext` to `jpnext,` after the change
  \ in the kernel.
  \
  \ 2017-01-12: Rewrite `papery`, `brighty` and `flashy` in Z80
  \ (smaller and faster code) and document them. Improve
  \ `permcolor`.
  \
  \ 2017-01-18: Remove `exit` at the end of conditional
  \ interpretation.
  \
  \ 2017-01-22: Rewrite `color!` and `color-mask!` in Z80.
  \
  \ 2017-01-24: Rename all words that fetch and store the
  \ system attributes: prefix "color" to "attr", prefix
  \ "permcolor" to "perm-attr". Rewrite `perm-attr!`,
  \ `perm-attr-mask!`, `attr@`, `attr-mask@`, `perm-attr@`,
  \ `perm-attr-mask@` in Z80. Improve documentation. Make all
  \ words individually accessible to `need`. Add `inverse-on`
  \ and `inverse-off` and rewrite `inverse` after them.  Add
  \ `overprint-on` and `overprint-off` and rewrite `overprint`
  \ after them.
  \
  \ 2017-01-25: Remove `permcolor` and `2permcolor`: they are
  \ hardly useful.  Rename `color` to `attr-setter`, and
  \ `2color` to `mask+attr-setter`.
  \
  \ 2017-01-27: Fix or improve several assembly jumps. Add
  \ `mask+attr!` and `mask+attr@`. Improve documentation.
  \ Improve `mask+attr-setter`.
  \
  \ 2017-01-31: Fix requirement of `paper!`. Fix Z80 opcode in
  \ `paper>attr`. Rename `paper!`, `paper@` and family to
  \ `set-paper`, `get-paper`, etc. Move `paper.`, `ink.`,
  \ `bright.` and `flash.` from the kernel. Improve
  \ documentation. Remove `paper>attr`; use `papery` instead.
  \ Rewrite `set-paper` and `set-ink` in Z80.
  \
  \ 2017-02-01: Make color constants individually accessible to
  \ `need`. Add `contrast`. Add `attr>ink`. Improve and update
  \ the documentation: use the name "current attribute" for the
  \ OS temporary attribute. Add `bright-mask`, `unbright-mask`,
  \ `flash-mask`, `unflash-mask`, `ink-mask`, `unink-mask`,
  \ `paper-mask`, `unpaper-mask`.
  \
  \ 2017-02-02: Move `permanent-colors` from the library and
  \ rename it to `mask+attr>perm`.
  \
  \ 2017-02-15: Fix typo in documentation.
  \
  \ 2017-02-16: Fix typos in documentation.
  \
  \ 2017-02-17: Update cross references.
  \
  \ 2017-02-24: Fix typos in documentation.

  \ -----------------------------------------------------------
  \ Operatyng system variables

  \ (From the ZX Spectrum +3 manual transcribed by Russell
  \ Marks et al.; and from the ZX Spectrum ROM disassembly.)

  \ 23693 = ATTR_P -- permanent colors

  \         {fl}{br}{   paper   }{  ink    }
  \          ___ ___ ___ ___ ___ ___ ___ ___
  \ ATTR_P  |   |   |   |   |   |   |   |   |
  \         |   |   |   |   |   |   |   |   |
  \ 23693   |___|___|___|___|___|___|___|___|
  \           7   6   5   4   3   2   1   0

  \ 23694 = MASK_P -- permanent mask
  \ MASK_P is used for transparent colours. Any bit that is 1
  \ shows that the corresponding attribute is taken not from
  \ ATTR_P but from what is already on the screen.

  \         {fl}{br}{   paper   }{  ink    }
  \          ___ ___ ___ ___ ___ ___ ___ ___
  \ MASK_P  |   |   |   |   |   |   |   |   |
  \         |   |   |   |   |   |   |   |   |
  \ 23694   |___|___|___|___|___|___|___|___|
  \           7   6   5   4   3   2   1   0

  \ 23695 = ATTR_T -- temporary colors

  \         {fl}{br}{   paper   }{  ink    }
  \          ___ ___ ___ ___ ___ ___ ___ ___
  \ ATTR_T  |   |   |   |   |   |   |   |   |
  \         |   |   |   |   |   |   |   |   |
  \ 23695   |___|___|___|___|___|___|___|___|
  \           7   6   5   4   3   2   1   0

  \ 23696 = MASK_T -- temporary mask
  \ MASK_T is used for transparent colours. Any bit that is 1
  \ shows that the corresponding attribute is taken not from
  \ ATTR_T but from what is already on the screen.

  \         {fl}{br}{   paper   }{  ink    }
  \          ___ ___ ___ ___ ___ ___ ___ ___
  \ MASK_T  |   |   |   |   |   |   |   |   |
  \         |   |   |   |   |   |   |   |   |
  \ 23696   |___|___|___|___|___|___|___|___|
  \           7   6   5   4   3   2   1   0

  \ P_FLAG holds the print flags.  Even bits are the temporary
  \ flags; odd bits are the permanent flags.

  \         {paper9 }{ ink9 }{ inv1 }{ over1}
  \          ___ ___ ___ ___ ___ ___ ___ ___
  \ P_FLAG  |   |   |   |   |   |   |   |   |
  \         | p | t | p | t | p | t | p | t |
  \ 23697   |___|___|___|___|___|___|___|___|
  \           7   6   5   4   3   2   1   0

( black blue red magenta green cyan yellow white contrast )

[unneeded] black ?\ 0 cconstant black

  \ doc{
  \
  \ black ( -- b )
  \
  \ A constant that returns 0, the value that represents the
  \ black color.
  \
  \ See also: `black`, `blue`, `red`, `magenta`, `green`,
  \ `cyan`, `yellow`, `white`, `contrast`.
  \
  \ }doc

[unneeded] blue ?\ 1 cconstant blue

  \ doc{
  \
  \ blue ( -- b )
  \
  \ A constant that returns 1, the value that represents the
  \ blue color.
  \
  \ See also: `black`, `red`, `magenta`, `green`, `cyan`,
  \ `yellow`, `white`, `contrast`.
  \
  \ }doc

[unneeded] red ?\ 2 cconstant red

  \ doc{
  \
  \ red ( -- b )
  \
  \ A constant that returns 2, the value that represents the
  \ red color.
  \
  \ See also: `black`, `blue`, `magenta`, `green`, `cyan`,
  \ `yellow`, `white`, `contrast`.
  \
  \ }doc

[unneeded] magenta ?\ 3 cconstant magenta

  \ doc{
  \
  \ magenta ( -- b )
  \
  \ A constant that returns 3, the value that represents the
  \ magenta color.
  \
  \ See also: `black`, `blue`, `red`, `green`, `cyan`,
  \ `yellow`, `white`, `contrast`.
  \
  \ }doc

[unneeded] green ?\ 4 cconstant green

  \ doc{
  \
  \ green ( -- b )
  \
  \ A constant that returns 4, the value that represents the
  \ green color.
  \
  \ See also: `black`, `blue`, `red`, `magenta`, `cyan`,
  \ `yellow`, `white`, `contrast`.
  \
  \ }doc

[unneeded] cyan ?\ 5 cconstant cyan

  \ doc{
  \
  \ cyan ( -- b )
  \
  \ A constant that returns 5, the value that represents the
  \ cyan color.
  \
  \ See also: `black`, `blue`, `red`, `magenta`, `green`,
  \ `yellow`, `white`, `contrast`.
  \
  \ }doc

[unneeded] yellow ?\ 6 cconstant yellow

  \ doc{
  \
  \ yellow ( -- b )
  \
  \ A constant that returns 6, the value that represents the
  \ yellow color.
  \
  \ See also: `black`, `blue`, `red`, `magenta`, `green`,
  \ `cyan`, `white`, `contrast`.
  \
  \ }doc

[unneeded] white ?\ 7 cconstant white

  \ doc{
  \
  \ white ( -- b )
  \
  \ A constant that returns 7, the value that represents the
  \ white color.
  \
  \ See also: `black`, `blue`, `red`, `magenta`, `green`,
  \ `cyan`, `yellow`, `contrast`.
  \
  \ }doc

[unneeded] contrast ?( need white need green

: contrast ( b1 -- b1 ) green < abs white * ; ?)

  \ doc{
  \
  \ contrast ( b1 -- b2 )
  \
  \ Convert color _b1_ to its contrast color _b2_.  _b2_ is
  \ `white` (7) if _b1_ is a dark color (`black`, `blue`, `red`
  \ or `magenta`); _b2_ is `black` (0) if _b1_ is a light
  \ colour (`green`, `cyan`, `yellow` or `white`).
  \
  \ }doc

( papery brighty flashy attr>paper attr>ink )

[unneeded] papery ?( need 8* need alias

' 8* alias papery ( b1 -- b2 ) ?)

  \ doc{
  \
  \ papery   ( b1 -- b2 )
  \
  \ Convert paper color _b1_ to its equivalent attribute _b2_.
  \
  \ This word is an alias of `8*`, which is written in Z80.
  \ Its equivalent in Forth is the following:

  \ ----
  \ : papery ( b1 -- b2 ) 8 * ;
  \ ----

  \ See also: `brighty`, `flashy`, `attr>paper`.
  \
  \ }doc

[unneeded] brighty ?(

code brighty ( b1 -- b2 )
  E1 c, CB c, C0 6 8 * + 5 + c, jppushhl, end-code ?)
  \ pop hl
  \ set 6,l
  \ jp pushhl

  \ doc{
  \
  \ brighty ( b1 -- b2 )
  \
  \ Convert attribute _b1_ to its brighty equivalent _b2_.
  \
  \ This word is written in Z80. Its equivalent in Forth is the
  \ following:

  \ ----
  \ : brighty ( b1 -- b2 )  bright-mask or ;
  \ ----

  \ See also: `papery`, `flashy`.
  \
  \ }doc

[unneeded] flashy ?(

code flashy   ( b1 -- b2 )
  E1 c, CB c, C0 7 8 * + 5 + c, jppushhl, end-code ?)
  \ pop hl
  \ set 7,l
  \ jp pushhl

  \ doc{
  \
  \ flashy   ( b1 -- b2 )
  \
  \ Convert attribute _b1_ to its flashy equivalent _b2_.
  \
  \ This word is written in Z80. Its equivalent in Forth is the
  \ following:

  \ ----
  \ : flashy   ( b1 -- b2 )  flash-mask or ;
  \ ----

  \ See also: `papery`, `brighty`.
  \
  \ }doc

[unneeded] attr>paper ?(

code attr>paper ( b1 -- b2 )
  E1 c, 7D c, E6 c, %00111000 c,
  \ pop hl
  \ ld a,l
  \ and %00111000 ; paper-mask
  CB c, 3F c, CB c, 3F c, CB c, 3F c, pusha jp, end-code ?)
  \ srl a
  \ srl a
  \ srl a
  \ jp push_a

  \ doc{
  \
  \ attr>paper ( b1 -- b2 )
  \
  \ Convert attribute _b1_ to its paper color number _b2_.
  \
  \ This word is written in Z80. The equivalent code in Forth
  \ is the following:

  \ ----
  \ : attr>paper ( b1 -- b2 ) paper-mask and 3 rshift ;
  \ ----

  \ See also: `attr>ink`, `papery`.
  \
  \ }doc

[unneeded] attr>ink ?(

code attr>ink ( b1 -- b2 )
  E1 c, 7D c, E6 c, %111 c, pusha jp, end-code ?)
  \ pop hl
  \ ld a,l
  \ and %111 ; ink-mask
  \ jp push_a

  \ doc{
  \
  \ attr>ink ( b1 -- b2 )
  \
  \ Convert attribute _b1_ to its ink color number _b2_.
  \
  \ This word is written in Z80. The equivalent code in Forth
  \ is the following:

  \ ----
  \ : attr>ink ( b1 -- b2 ) ink-mask and ;
  \ ----

  \ See also: `attr>paper`.
  \
  \ }doc

( attr@ attr! attr-mask@ attr-mask! )

[unneeded] attr@ ?( need os-attr-t

code attr@ ( -- b ) 3A c, os-attr-t , pusha jp, end-code ?)
  \ ld a,(sys_attr_t)
  \ jp push_a

  \ doc{
  \
  \ attr@ ( -- b )
  \
  \ Get the current attribute _b_.
  \
  \ See also: `attr!`, `perm-attr@`.
  \
  \ }doc

[unneeded] attr! ?( need os-attr-t

code attr! ( b -- )
  D1 c, 78 03 + c, 32 c, os-attr-t , jpnext, end-code ?)

  \                   ; T  B
  \                   ; -----
  \ pop de            ; 10 01
  \ ld a,e            ; 04 01
  \ ld (sys_attr_t),a ; 13 03
  \ jp (ix)           ; 08 02
  \                   ; -----
  \                   ; 35 07

  \ doc{
  \
  \ attr! ( b -- )
  \
  \ Set _b_ as the current attribute.
  \
  \ See also: `attr@`, `perm-attr!`.
  \
  \ }doc

[unneeded] attr-mask@ ?( need os-mask-t

code attr-mask@ ( -- b )
  3A c, os-mask-t , pusha jp, end-code ?)
  \ ld a,(sys_mask_t)
  \ jp push_a

  \ doc{
  \
  \ attr-mask@ ( -- b )
  \
  \ Get the current attribute mask _b_.
  \
  \ See also: `attr-mask!`, `perm-attr-mask@`.
  \
  \ }doc

[unneeded] attr-mask! ?( need os-mask-t

code attr-mask! ( b -- )
  D1 c, 78 03 + c, 32 c, os-mask-t , jpnext, end-code ?)

  \                   ; T  B
  \                   ; -----
  \ pop de            ; 10 01
  \ ld a,e            ; 04 01
  \ ld (sys_mask_t),a ; 13 03
  \ jp (ix)           ; 08 02
  \                   ; -----
  \                   ; 35 07

  \ doc{
  \
  \ attr-mask! ( b -- )
  \
  \ Set _b_ as the current attribute mask.
  \
  \ See also: `attr-mask@`, `perm-attr-mask!`.
  \
  \ }doc

code mask+attr>perm ( -- ) $1CAD call, jpnext, end-code
  \ call rom_set_permanent_colors_0x1CAD
  \ _jp_next

  \ doc{
  \
  \ mask+attr>perm ( -- )
  \
  \ Make the current attribute and mask permanent.
  \
  \ Note: Words that use attributes don't use the OS permanent
  \ attribute but the temporary one, which is called "current
  \ attribute" in Solo Forth.
  \
  \ }doc

( mask+attr! mask+attr@ attr-setter mask+attr-setter )

[unneeded] mask+attr! ?( need os-attr-t

code mask+attr! ( b1 b2 -- )
  E1 c, D1 c, 60 03 + c, 22 c, os-attr-t , jpnext, end-code ?)

  \                    ; T  B
  \                    ; -----
  \ pop hl             ; 10 01
  \ pop de             ; 10 01
  \ ld h,e             ; 04 01
  \ ld (sys_attr_t),hl ; 16 03
  \ _jp_next           ; 08 02 ; jp (ix)
  \                    ; -----
  \                    ; 48 08

  \ doc{
  \
  \ mask+attr! ( b1 b2 -- )
  \
  \ Set _b1_ as the current attribute mask
  \ and _b2_ as the current attribute.
  \
  \ See also: `mask+attr@`, `attr!`, `attr-mask!`
  \
  \ }doc

[unneeded] mask+attr@ ?( need os-attr-t

code mask+attr@ ( -- b1 b2 )
  26 c, 00 c, ED c, 5B c, os-attr-t , 68 02 + c, E5 c,
  \ ld h,0
  \ ld de,(sys_attr_t)
  \ ld l,d
  \ push hl
  68 03 + c, jppushhl, end-code ?)
  \ ld l,e
  \ _jp_pushhl

  \ doc{
  \
  \ mask+attr@ ( -- b1 b2 )
  \
  \ Set _b_ as the current attribute mask.
  \
  \ See also: `attr-mask!`, `perm-attr-mask@`.
  \
  \ }doc

[unneeded] attr-setter ?( need attr!

: attr-setter ( b "name" -- )
  create c,  does> ( -- ) ( pfa ) c@ attr! ; ?)

  \ doc{
  \
  \ attr-setter ( b "name" -- )
  \
  \ Create a definition "name" that, when executed, will
  \ set _b_ as the current attribute.
  \
  \ See also: `mask+attr-setter`.
  \
  \ }doc

[unneeded] mask+attr-setter ?( need mask+attr!

: mask+attr-setter ( b1 b2 "name" -- )
  create 2,  does> ( -- ) ( pfa ) 2@ mask+attr! ; ?)

  \ doc{
  \
  \ mask+attr-setter ( b1 b2 "name" -- )
  \
  \ Create a definition "name" that, when executed, will set
  \ _b1_ as the current attribute mask and _b2_ as the
  \ current attribute.
  \
  \ See also: `attr-setter`.
  \
  \ }doc

( perm-attr@ perm-attr! perm-attr-mask@ perm-attr-mask! )

[unneeded] perm-attr@ ?( need os-attr-p

code perm-attr@ ( -- b )
  3A c, os-attr-p , pusha jp, end-code ?)
  \ ld a,(sys_attr_p)
  \ jp push_a

  \ doc{
  \
  \ perm-attr@ ( -- b )
  \
  \ Get the permanent attribute _b_.
  \
  \ Note: Words that use attributes don't use the OS permanent
  \ attribute but the temporary one, which is called "current
  \ attribute" in Solo Forth.
  \
  \ See also: `perm-attr!`, `attr@`.
  \
  \ }doc

[unneeded] perm-attr! ?( need os-attr-p

code perm-attr! ( b -- )
  D1 c, 78 03 + c, 32 c, os-attr-p , jpnext, end-code ?)

  \                   ; T  B
  \                   ; -----
  \ pop de            ; 10 01
  \ ld a,e            ; 04 01
  \ ld (sys_attr_p),a ; 13 03
  \ jp (ix)           ; 08 02
  \                   ; -----
  \                   ; 35 07

  \ doc{
  \
  \ perm-attr! ( b -- )
  \
  \ Set _b_ as the permanent attribute.
  \
  \ Note: Words that use attributes don't use the OS permanent
  \ attribute but the temporary one, which is called "current
  \ attribute" in Solo Forth.
  \
  \ See also: `perm-attr@`, `attr!`.
  \
  \ }doc

[unneeded] perm-attr-mask@ ?( need os-mask-p

code perm-attr-mask@ ( -- b )
  3A c, os-mask-p , pusha jp, end-code ?)
  \ ld a,(sys_mask_p)
  \ jp push_a

  \ doc{
  \
  \ perm-attr-mask@ ( -- b )
  \
  \ Get the permanent attribute mask _b_.
  \
  \ Note: Words that use attributes don't use the OS permanent
  \ attribute but the temporary one, which is called "current
  \ attribute" in Solo Forth.
  \
  \ See also: `perm-attr-mask!`, `attr-mask@`.
  \
  \ }doc

[unneeded] perm-attr-mask! ?( need os-mask-p

code perm-attr-mask! ( b -- )
  D1 c, 78 03 + c, 32 c, os-mask-p , jpnext, end-code ?)

  \                   ; T  B
  \                   ; -----
  \ pop de            ; 10 01
  \ ld a,e            ; 04 01
  \ ld (sys_mask_p),a ; 13 03
  \ jp (ix)           ; 08 02
  \                   ; -----
  \                   ; 35 07

  \ doc{
  \
  \ perm-attr-mask! ( b -- )
  \
  \ Set _b_ as the permanent attribute mask.
  \
  \ Note: Words that use attributes don't use the OS permanent
  \ attribute but the temporary one, which is called "current
  \ attribute" in Solo Forth.
  \
  \ See also: `perm-attr-mask@`, `attr-mask!`.
  \
  \ }doc

( get-paper set-paper get-ink set-ink )

[unneeded] get-paper ?( need attr@ need attr>paper

: get-paper ( -- b ) attr@ attr>paper ; ?)

[unneeded] set-paper ?( need os-attr-t

code set-paper ( b -- )
  E1 c, 7D c, E6 c, %111 c,
  \ pop hl
  \ ld a,l
  \ and %111 ; ink-mask
  CB c, 27 c, CB c, 27 c, CB c, 27 c, 58 07 + c,
  \ sla a
  \ sla a
  \ sla a
  \ ld e,a
  21 c, os-attr-t , 3E c, %1100111 c, A6 c, B0 03 + c,
  \ ld hl,sys_attr_t
  \ ld a,%11000111 ; unpaper-mask
  \ and (hl)
  \ or e
  70 07 + c, jpnext, end-code ?)
  \ ld (hl),a
  \ _jp_next

  \ doc{
  \
  \ set-paper ( b -- )
  \
  \ Set paper color _b_ (0..7) by modifying the corresponding
  \ bits of the current attribute.
  \
  \ This word is written in Z80. Its equivalent code in Forth
  \ is the following:

  \ ----
  \ : set-paper ( b -- )
  \   papery attr@ unpaper-mask and or attr! ;
  \ ----

  \ See also: `paper.`, `set-ink`, `set-flash`, `set-bright`.
  \
  \ }doc

[unneeded] get-ink ?( need attr@ need attr>ink

: get-ink ( -- b ) attr@ attr>ink ; ?)

[unneeded] set-ink ?( need os-attr-t

code set-ink ( b -- )
  D1 c, 21 c, os-attr-t ,
  \ pop de
  \ ld hl,sys_attr_t
  3E c, %11111000 c, A6 c, B3 c, 77 c, jpnext, end-code ?)
  \ ld a,%11111000 ; unink-mask
  \ and (hl)
  \ or e
  \ ld (hl),a
  \ _jp_next

  \ doc{
  \
  \ set-ink ( b -- )
  \
  \ Set ink color _b_ by modifying bits 0-2 of the current
  \ attribute. Note that _b_ is supposed to be in the range
  \ 0..7 and its bits 3-7 are not masked. Therefore any value
  \ other than 0..7 will corrupt the current attribute.
  \
  \ This word is written in Z80. Its equivalent code in Forth
  \ is the following:

  \ ----
  \ : set-ink ( b -- )
  \   attr@ %11111000 and or attr! ;
  \ ----

  \ See also: `ink.`, `set-paper`, `set-flash`, `set-bright`.
  \
  \ }doc

( ink-mask unink-mask paper-mask unpaper-mask )

[unneeded] ink-mask       ?\ %00000111 cconstant ink-mask
[unneeded] unink-mask     ?\ %11111000 cconstant unink-mask

[unneeded] paper-mask     ?\ %00111000 cconstant paper-mask
[unneeded] unpaper-mask   ?\ %11000111 cconstant unpaper-mask

( bright-mask unbright-mask get-bright set-bright )

[unneeded] bright-mask    ?\ %01000000 cconstant bright-mask
[unneeded] unbright-mask  ?\ %10111111 cconstant unbright-mask

[unneeded] get-bright ?( need attr@ need bright-mask

: get-bright ( -- f )
  attr@ [ bright-mask ] cliteral and 0= ; ?)

[unneeded] set-bright ?(

need bright-mask need attr@ need unbright-mask need attr!

: set-bright ( f -- )
  [ bright-mask ] cliteral and
  attr@ [ unbright-mask ] cliteral and or attr! ; ?)

( flash-mask unflash-mask get-flash set-flash )

[unneeded] flash-mask   ?\ %10000000 cconstant flash-mask
[unneeded] unflash-mask ?\ %01111111 cconstant unflash-mask

[unneeded] get-flash ?( need attr@ need flash-mask

: get-flash ( -- f )
  attr@ [ flash-mask ] literal and 0= ; ?)

[unneeded] set-flash ?(

need flash-mask need attr@ need unflash-mask need attr!

: set-flash ( f -- )
  [ flash-mask ] cliteral and
  attr@ [ unflash-mask ] cliteral and or attr! ; ?)

( inverse-on inverse-off inverse )

[unneeded] inverse-on ?(

code inverse-on ( f -- )
  FD c, CB c, 57 c, C6 08 02 * + c,  jpnext, end-code ?)
    \ set 2,(iy+sys_p_flag_offset) ; temporary inverse mode flag
    \ _jp_next

  \ doc{
  \
  \ inverse-on ( f -- )
  \
  \ Turn the inverse printing mode on.
  \
  \ See also: `inverse-off`, `inverse`.
  \
  \ }doc

[unneeded] inverse-off ?(

code inverse-off ( -- )
  FD c, CB c, 57 c, 86 08 02 * + c,  jpnext, end-code ?)
    \ res 2,(iy+sys_p_flag_offset) ; temporary inverse mode flag
    \ _jp_next

  \ doc{
  \
  \ inverse-off ( -- )
  \
  \ Turn the inverse printing mode off.
  \
  \ See also: `inverse-on`, `inverse`.
  \
  \ }doc

[unneeded] inverse ?( need inverse-off need inverse-on

code inverse ( f -- )
  E1 c, 78 04 + c, B0 05 + c,
    \ pop hl
    \ ld a,h
    \ or l
  CA c, ' inverse-off , ' inverse-on jp, end-code ?)
    \ jp z,inverse_off_
    \ jp inverse_on_

  \ doc{
  \
  \ inverse ( f -- )
  \
  \ If _f_ is zero, turn the inverse printing mode
  \ off; else turn it on.
  \
  \ See also: `inverse-off`, `inverse-on`, `overprint`.
  \
  \ }doc

( overprint-on overprint-off overprint )

[unneeded] overprint-on ?(

code overprint-on ( -- )
  FD c, CB c, 57 c, C6 08 00 * + c,  jpnext, end-code ?)
    \ set 0,(iy+sys_p_flag_offset) ; temporary overprint mode flag

  \ doc{
  \
  \ overprint-on ( -- )
  \
  \ Turn the overprint mode on.
  \
  \ See also: `overprint-off`, `overprint`.
  \
  \ }doc

[unneeded] overprint-off ?(

code overprint-off ( -- )
  FD c, CB c, 57 c, 86 08 00 * + c,  jpnext, end-code ?)
    \ res 0,(iy+sys_p_flag_offset) ; temporary overprint mode flag
    \ _jp_next

  \ doc{
  \
  \ overprint-off ( -- )
  \
  \ Turn the overprint mode off.
  \
  \ See also: `overprint-on`, `overprint`.
  \
  \ }doc

[unneeded] overprint ?( need overprint-on need overprint-off

code overprint ( f -- )
  E1 c, 78 04 + c, B0 05 + c,
    \ pop hl
    \ ld a,h
    \ or l
  CA c, ' overprint-off , ' overprint-on jp, end-code ?)
    \ jp z,overprint_off_
    \ jp plus_inverse_

  \ doc{
  \
  \ overprint ( f -- )
  \
  \ If _f_ is zero, turn the overprint mode off; else
  \ turn it on.
  \
  \ See also: `overprint-on`, `overprint-off`, `inverse`.
  \
  \ }doc

( paper. ink. (0-9-color. flash. bright. (0-1-8-color. )

[unneeded] paper. ?( need (0-9-color.

code paper. ( b -- ) 3E c, 11 c, (0-9-color. jp, end-code ?)
  \ ld a,paper_control_char
  \ jp print_0_9_color

  \ doc{
  \
  \ paper. ( b -- )
  \
  \ Set paper color to _b_ (0..9), by printing the
  \ corresponding control characters.  If _b_ is greater than
  \ 9, 9 is used instead.
  \
  \ This word is much slower than `set-paper` or `attr!`, but
  \ it can handle pseudo-colors 8 (transparent) and 9
  \ (contrast), setting the correspondent system variables and
  \ flags accordingly.
  \
  \ See also: `ink.`, `(0-9-color.`.
  \
  \ }doc

[unneeded] ink. ?( need (0-9-color.

code ink. ( b -- ) 3E c, 10 c, (0-9-color. jp, end-code ?)
  \ ld a,ink_control_char
  \ jp print_0_9_color

  \ doc{
  \
  \ ink. ( b -- )
  \
  \ Set ink color to _b_ (0..9), by printing the corresponding
  \ control characters.  If _b_ is greater than 9, 9 is used
  \ instead.
  \
  \ This word is much slower than `set-ink` or `attr!`, but it
  \ can handle pseudo-colors 8 (transparent) and 9 (contrast),
  \ setting the correspondent system variables and flags
  \ accordingly.
  \
  \ See also: `paper.`, `(0-9-color.`.
  \
  \ }doc

[unneeded] (0-9-color. ?( need assembler

create (0-9-color. ( -- a ) asm

  prt, h pop, l a ld, 0A cp#, nc? rif  09 a ld#,  rthen prt,

  \   rst $10 ; print the control char in A
  \   pop hl
  \   ld a,l ; A = color
  \   cp $0A ; value is 0..9?
  \   jr c,print_0_9_attribute.valid
  \   ld a,$09
  \ print_0_9_attribute.valid:
  \ ; A = color 0..9
  \   rst $10 ; print the attribute value in A (0..9)
  jpnext, end-asm ?)
  \   _jp_next

  \ doc{
  \
  \ (0-9-color. ( -- a )
  \
  \ Return the address _a_ of a routine used by `paper.` and
  \ `ink.`.  This routine prints a color attribute in the range
  \ 0..9.

  \ Input:
  \ - A = attribute control char ($10 for ink, $11 for paper)
  \ - TOS = attribute value (0..9)

  \ Note: If TOS is greater than 9, 9 is used instead.

  \ }doc

[unneeded] flash.

?\ need (0-1-8-color.  : flash. ( n -- ) 18 (0-1-8-color. ;

  \ doc{
  \
  \ flash. ( n -- )
  \
  \ Set flash _n_ by printing the corresponding control
  \ characters.  If _n_ is zero, turn flash off; if _n_ is one,
  \ turn flash on; if _n_ is eight, set transparent flash.
  \ Other values of _n_ are converted as follows:

  \ - 2, 4 and 6 are converted to 0.
  \ - 3, 5 and 7 are converted to 1.
  \ - Values greater than 8 or less than 0 are converted to 8.

  \
  \ This word is much slower than `set-flash` or `attr!`, but
  \ it can handle pseudo-color 8 (transparent).
  \
  \ See also: `bright.`, `(0-1-8-color.`.
  \
  \ }doc

[unneeded] bright.

?\ need (0-1-8-color.  : bright. ( n -- ) 19 (0-1-8-color. ;

  \ doc{
  \
  \ bright. ( n -- )
  \
  \ Set bright _n_ by printing the corresponding control
  \ characters.  If _n_ is zero, turn bright off; if _n_ is one,
  \ turn bright on; if _n_ is eight, set transparent bright. Other
  \ values of _n_ are converted as follows:

  \ - 2, 4 and 6 are converted to 0.
  \ - 3, 5 and 7 are converted to 1.
  \ - Values greater than 8 or less than 0 are converted to 8.

  \ This word is much slower than `set-bright` or `attr!`, but
  \ it can handle pseudo-color 8 (transparent).
  \
  \ See also: `flash.`, `(0-1-8-color.`.
  \
  \ }doc

[unneeded] (0-1-8-color.

?\ : (0-1-8-color. ( n c -- ) emit %1001 and 8 min emit ;

  \ doc{
  \
  \ (0-1-8-color. ( n c -- )
  \
  \ Print control character _c_. Then convert _n_ to the set 0,
  \ 1 and 8 and print it as a character. The conversion of _n_
  \ is done as follows:

  \ - 0, 1 and 8 are not changed.
  \ - 2, 4 and 6 are converted to 0.
  \ - 3, 5 and 7 are converted to 1.
  \ - Values greater than 8 or less than 0 are converted to 8.

  \ This word is a factor of `flash.` and `bright.`.
  \
  \ }doc

  \ vim: filetype=soloforth