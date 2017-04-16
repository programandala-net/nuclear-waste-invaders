  \ graphics.attributes.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201703132351
  \ See change log at the end of the file

  \ ===========================================================
  \ Description

  \ Words related to screen attributes.

  \ ===========================================================
  \ Author

  \ Marcos Cruz (programandala.net), 2015, 2016, 2017.

  \ ===========================================================
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

  \ ===========================================================
  \ Change log

  \ 2016-10-15: Improve the format of the documentation.
  \
  \ 2016-12-20: Rename `jppushhl` to `jppushhl,` after the
  \ change in the kernel.
  \
  \ 2016-12-30: Compact the code, saving one block.
  \
  \ 2016-12-31: Rewrite `attr`, `attr-addr` and `(attr-addr)`
  \ with Z80 opcodes, without assembler. Compact the code,
  \ saving one block.
  \
  \ 2017-01-18: Remove `exit` at the end of conditional
  \ interpretation.
  \
  \ 2017-01-19: Remove remaining `exit` at the end of
  \ conditional interpretation, after `end-asm`.
  \
  \ 2017-03-13: Rename: `(attr-addr)` to `xy>attra_`,
  \ `attr-addr` to `xy>attra`.  `attr` to `xy>attr`.

  \ vim: filetype=soloforth
