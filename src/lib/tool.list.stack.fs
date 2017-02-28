  \ tool.list.stack.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201702221550

  \ -----------------------------------------------------------
  \ Description

  \ Words to examine the stack.

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

  \ 2015-11-13: Modified `.depth` to print a signed number,
  \ better for debugging.
  \
  \ 2016-04-12: Divided into 3 blocks, in order to reuse
  \ `.depth` for the floating point `.fs`. Fixed the check: the
  \ stacks are not printed when their depth is negative.
  \
  \ 2016-04-24: Remove `[char]`, which has been moved to the
  \ library.
  \
  \ 2016-12-30: Compact the code, saving two blocks. Make
  \ `.depth` 4 bytes smaller.
  \
  \ 2017-01-18: Remove `exit` at the end of conditional
  \ interpretation.
  \
  \ 2017-01-25: Add deferred `(.s)` to `.s` and rewrite `u.s`
  \ after `.s`.
  \
  \ 2017-02-20: Replace `do`, which has been moved to the
  \ library, with `?do`.

( .depth .s u.s )

[unneeded] .depth ?\ : .depth ( n -- ) ." <" 0 .r ." > " ;

[unneeded] .s ?( need .depth

defer (.s) ( x -- ) ' . ' (.s) defer!

: .s   ( -- )
  depth dup .depth 0> if
    sp@ sp0 @ cell- ?do i @ (.s)  [ cell negate ] literal +loop
  then ; ?)

  \ Credit:
  \ Code from Afera. Original algorithm from v.Forth.

[unneeded] u.s ?( need .s

: u.s   ( -- )
  ['] u. ['] (.s) defer!  .s  ['] . ['] (.s) defer! ; ?)

  \ vim: filetype=soloforth