  \ memory.ports.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201702221550

  \ -----------------------------------------------------------
  \ Description

  \ Words for ports input and output.

  \ -----------------------------------------------------------
  \ Author

  \ Marcos Cruz (programandala.net), 2015, 2016.

  \ -----------------------------------------------------------
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

  \ -----------------------------------------------------------
  \ Latest changes

  \ 2016-05-07: Improve documentation. Compact the blocks.
  \
  \ 2016-12-20: Rename `jppushhl` to `jppushhl,` and `jpnext`
  \ to `jpnext,`, after the change in the kernel.

( @p !p )

[unneeded] @p ?(

code @p ( a -- b )
  E1 c,  C5 c,  48 05 + c,  40 04 + c,  ED c, 68 c,
    \ pop hl
    \ push bc
    \ ld c,l
    \ ld b,h
    \ in l,(c)
  C1 c,  26 c, 00 c,  jppushhl, end-code
    \ pop bc
    \ ld h,0x00
    \ jp pushhl

  \ doc{
  \
  \ @p ( a -- b )
  \
  \ Input byte _b_ from port _a_.
  \
  \ }doc

?)

[unneeded] !p ?(

code !p ( b a -- )
  E1 c,  D1 c,  C5 c,  48 05 + c,  40 04 + c,  ED c, 59 c,
    \ pop hl
    \ pop de ; char in e
    \ push bc
    \ ld c,l
    \ ld b,h
    \ out (c),e
  C1 c,  jpnext, end-code
    \ pop bc
    \ jp next

  \ doc{
  \
  \ !p ( b a -- )
  \
  \ Output byte _b_ to port _a_.
  \
  \ }doc

?)

  \ vim: filetype=soloforth