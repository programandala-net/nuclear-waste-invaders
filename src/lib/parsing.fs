  \ parsing.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201702221550

  \ -----------------------------------------------------------
  \ Description

  \ Words related to parsing.

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

  \ 2015-09-13: Add `parse-char`.
  \
  \ 2015-10-06: Move `word` from the kernel.
  \
  \ 2015-10-18: Move `command` from the editor and rename it to
  \ `parse-line`.
  \
  \ 2015-10-22: Fix `parse-char`.
  \
  \ 2016-04-24: Move `char` and `[char]` from the kernel.
  \
  \ 2016-05-10: Fix `[char]`.
  \
  \ 2016-05-14: Compact the blocks. Fix `parse-line` and rename
  \ it to `parse-all`. Finish and document `execute-parsing`.
  \ Move `string>source` and `evaluate` from the kernel.
  \ Rewrite `evaluate` after `execute-parsing`.
  \
  \ 2016-05-31: Update: `cliteral` has been moved to the
  \ kernel.
  \
  \ 2016-11-17: Fix needing `char`, `[char]` and `word`.
  \
  \ 2016-12-30: Compact the code, saving one block.
  \
  \ 2017-01-18: Remove `exit` at the end of conditional
  \ interpretation.

( defined? parse-char parse-all )

[unneeded] defined?

?\ : defined? ( ca len -- f ) undefined? 0= ;

[unneeded] parse-char

?\ : parse-char ( "c"  -- c ) stream drop c@ 1 parsed ;
  \ Parse the next char in the input stream and return its
  \ code.

[unneeded] parse-all ?(

: parse-all ( "ccc<eol>" -- ca len )
  stream dup parsed save-string ; ?)

  \ doc{
  \
  \ parse-all ( "ccc" -- ca len )
  \
  \ Parse the rest of the source.
  \
  \ This word is a useful factor of Specforth editor's `text`.
  \
  \ }doc

( execute-parsing string>source evaluate )

[unneeded] string>source ?(
: string>source ( ca len -- )
  blk off  (source-id) on  set-source ; ?)

  \ doc{
  \
  \ string>source ( ca len -- )
  \
  \ Set the string _ca len_ as the current source.
  \
  \ }doc

[unneeded] execute-parsing ?( need need-here
need-here string>source
: execute-parsing ( ca len xt -- )
  nest-source >r string>source r> execute unnest-source ; ?)

  \ doc{
  \
  \ execute-parsing ( ca len xt -- )
  \
  \ Make _ca len_ the current input source, execute _xt_, then
  \ restore the previous input source.
  \
  \ Origin: Gforth.
  \
  \ }doc

[unneeded] evaluate ?( need need-here
need-here execute-parsing
: evaluate ( i*x ca len -- j*x )
  ['] interpret execute-parsing ; ?)

  \ doc{
  \
  \ evaluate ( i*x ca len -- j*x )
  \
  \ Save the current input source specification. Store
  \ minus-one (-1) in `source-id`. Make the  string described
  \ by _ca len_ both the input source and input buffer,  set
  \ `>in` to zero,  and interpret.  When the  parse area  is
  \ empty, restore the prior input source specification.
  \
  \ Origin: Forth-94 (CORE), Forth-2012 (CORE).
  \
  \ }doc

( char [char] word )

[unneeded] char
?\ : char ( "name" -- c ) parse-name drop c@ ;

  \ doc{
  \
  \ char ( "name" -- c )
  \
  \ Parse "name" and put the value of its first character on
  \ the stack.
  \
  \ Origin: Forth-94 (CORE), Forth-2012 (CORE).
  \
  \ }doc

[uneeded] [char]  ?(

: [char] ( "name" -- c )
  char postpone cliteral ; immediate compile-only ?)

  \ doc{
  \
  \ [char]
  \
  \ Compilation: ( "name" -- )
  \
  \ Parse "name" and append the run-time semantics given below
  \ to the current definition.
  \
  \ Run-time: ( -- c )
  \
  \ Place _c_, the value of the first character of _name_, on
  \ the stack.
  \
  \ Origin: Forth-94 (CORE), Forth-2012 (CORE).
  \
  \ }doc

  \ Credit:
  \
  \ Code from Z88 CamelForth.

[unneeded] word ?(

: word ( c "<chars>ccc<char>" -- ca )
  dup  stream                 ( c c ca len )
  dup >r   rot skip           ( c ca' len' )
  over >r  rot scan           ( ca" len" )
  dup if  char-  then         \ skip trailing delimiter
  r> r> rot -   >in +!        \ update `>in`
  tuck - ( ca' len ) here place  here ( ca )
  bl over count + c! ; ?) \ append trailing blank

  \  doc{
  \
  \  word ( c "<chars>ccc<char>" -- ca )
  \
  \  c = delimiter char
  \
  \  Skip leading _c_ delimiters from the input stream.  Parse
  \  the next text characters from the input stream, until a
  \  delimiter _c_ is found, storing the packed character
  \  string beginning at _ca_, as a counted string (the
  \  character count in the first byte), and with one blank at
  \  the end.  byte), and with one blank at the end (not
  \  included in the count).
  \
  \  Origin: Forth-94 (CORE), Forth-2012 (CORE).
  \
  \  }doc

( save-input restore-input )

  \ XXX UNDER DEVELOPMENT
  \
  \ 2016-01-01: Code copied from m3Forth:
  \ https://github.com/oco2000/m3forth/blob/master/lib/include/core-ext.f

: save-input ( -- xn ... x1 n )
  source-id 0>
  if tib #tib @ 2dup c/l 2 + allocate throw dup >r swap cmove
     r> to tib  >in @
     source-id file-position throw  5
  else blk @ >in @ 2 then ;

: restore-input ( xn ... x1 n -- f ) source-id 0>
  if dup 5 <> if 0 ?do drop loop -1 exit then
     drop source-id reposition-file ?dup
     if >r 2drop drop r> exit then
     >in ! #tib ! to tib false
  else dup 2 <> if 0 ?do drop loop -1 exit then
     drop >in ! blk ! false
  then ;

  \ vim: filetype=soloforth