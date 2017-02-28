  \ random.fs
  \
  \ This file is part of Solo Forth
  \ http://programandala.net/en.program.solo_forth.html

  \ Last modified: 201702221550

  \ -----------------------------------------------------------
  \ Description

  \ Pseudo-random number generators.
  \
  \ See benchmark results in <meta.benchmark.rng.fsb>.

  \ -----------------------------------------------------------
  \ Author

  \ Marcos Cruz (programandala.net), 2015, 2016, 2017.

  \ -----------------------------------------------------------
  \ License

  \ You may do whatever you want with this work, so long as you
  \ retain every copyright, credit and authorship notice, and
  \ this license.  There is no warranty.

  \ -----------------------------------------------------------
  \ Latest changes

  \ 2015-12-25: Add `crnd`.
  \
  \ 2016-03-31: Adapted C. G. Montgomery's `rnd`.
  \
  \ 2016-04-08: Updated the literal in C. G. Montgomery's `rnd`
  \ after the latest benchmarks.
  \
  \ 2016-10-18: Update the name of the benchmarks library
  \ module.
  \
  \ 2016-12-06: Add `-1|1`. Improve documentation and needing
  \ of `randomize` and `randomize0`.
  \
  \ 2016-12-12: Fix needing of `-1|1` and `randomize`.
  \
  \ 2016-12-20: Rename `jppushhl` to `jppushhl,` after the
  \ change in the kernel.
  \
  \ 2016-12-30: Compact the code, saving one block.
  \
  \ 2017-01-02: Convert `fast-rnd` from `z80-asm` to
  \ `z80-asm,`.
  \
  \ 2017-01-04: Convert `crnd` from `z80-asm` to `z80-asm,`;
  \ add its missing requirement. Make `crnd` and `crandom`
  \ accessible to `need`; improve their documentation.
  \
  \ 2017-01-05: Update `need z80-asm,` to `need assembler`.
  \
  \ 2017-01-12: Add `-1..1`.
  \
  \ 2017-01-18: Remove `exit` at the end of conditional
  \ interpretation.
  \
  \ 2017-02-17: Update cross references.

( rnd random random-range )

[unneeded] rnd [unneeded] random and ?(

2variable rnd-seed  $0111 rnd-seed !

: rnd ( -- u )
  rnd-seed 2@ $62DC um* rot 0 d+ over rnd-seed 2! ;

: random ( n -- 0..n-1 ) rnd um* nip ; ?)

  \ Credit:
  \
  \ Random Number Generator by C. G. Montgomery: `random` and
  \ `rnd`.
  \
  \ Found here (2015-12-13):
  \ http://web.archive.org/web/20060707001752/http://www.tinyboot.com/index.html

[unneeded] random-range  ?( need random

: random-range ( n1 n2 -- n3 ) over - 1+ random + ;

  \ doc{
  \
  \ random-range ( n1 n2 -- n3 )
  \
  \ Return a random number from _n1_ (min) to _n2_ (max).
  \
  \ }doc

?)

( fast-rnd fast-random )

need assembler need os-seed

code fast-rnd ( -- u )

  os-seed fthl, h d ldp,
    \ ld hl,(seed)
    \ ld c,l
    \ ld b,h
  h addp, d addp, h addp, d addp, h addp,
  d addp, h addp, h addp, h addp, h addp, d addp,
    \ add hl,hl
    \ add hl,de
    \ add hl,hl
    \ add hl,de
    \ add hl,hl
    \ add hl,de
    \ add hl,hl
    \ add hl,hl
    \ add hl,hl
    \ add hl,hl
    \ add hl,de
  h inc, h incp, os-seed sthl, jppushhl, end-code
    \ inc h
    \ inc hl
    \ ld (seed),hl
    \ jp push_hl

: fast-random ( n -- 0..n-1 ) fast-rnd um* nip ;

  \ Credit:
  \
  \ Code adapted from:
  \ http://z80-heaven.wikidot.com/math#toc40

  \ Original code:

  \ ----
  \ PseudoRandWord:
  \
  \ ; this generates a sequence of pseudo-random values
  \ ; that has a cycle of 65536 (so it will hit every
  \ ; single number):
  \
  \ ;f(n+1)=241f(n)+257   ;65536
  \ ;181 cycles, add 17 if called
  \
  \ ;Outputs:
  \ ;     BC was the previous pseudorandom value
  \ ;     HL is the next pseudorandom value
  \ ;Notes:
  \ ;     You can also use B,C,H,L as pseudorandom 8-bit values
  \ ;     this will generate all 8-bit values
  \      .db 21h    ;start of ld hl,**
  \ randSeed:
  \      .dw 0
  \      ld c,l
  \      ld b,h
  \      add hl,hl
  \      add hl,bc
  \      add hl,hl
  \      add hl,bc
  \      add hl,hl
  \      add hl,bc
  \      add hl,hl
  \      add hl,hl
  \      add hl,hl
  \      add hl,hl
  \      add hl,bc
  \      inc h
  \      inc hl
  \      ld (randSeed),hl
  \      ret
  \ ----

( crnd crandom )

[unneeded] crandom
?\ need crnd  : crandom ( b1 -- b2 ) crnd um* nip ;

  \ doc{
  \
  \ crandom ( b1 -- b2 )
  \
  \ Return a random 8-bit number _b2_ in range _0..b1-1_
  \
  \ }doc

[unneeded] crnd ?exit

need assembler need os-seed

code crnd ( -- b )
  os-seed fthl,
    \ ld      hl,(randData)
  ED c, 5F c,  a d ld, m e ld, de addp, l add, h xor,
    \ ld      a,r
    \ ld      d,a
    \ ld      e,(hl)
    \ add     hl,de
    \ add     a,l
    \ xor     h
  os-seed sthl, pusha jp, end-code
    \ ld      (randData),hl
    \ jp push_a

  \ Credit:
  \
  \ http://wikiti.brandonw.net/index.php?title=Z80_Routines:Math:Random
  \ Joe Wingbermuehle

  \ Original code:

  \ ----
  \ ; ouput a=answer 0<=a<=255
  \ ; all registers are preserved except: af
  \ random:
  \         push    hl
  \         push    de
  \         ld      hl,(randData)
  \         ld      a,r
  \         ld      d,a
  \         ld      e,(hl)
  \         add     hl,de
  \         add     a,l
  \         xor     h
  \         ld      (randData),hl
  \         pop     de
  \         pop     hl
  \         ret
  \ ----

  \ doc{
  \
  \ crnd ( -- b )
  \
  \ Return a random 8-bit number _b_ (0..255).
  \
  \ See also: `crandom`.
  \
  \ }doc

( -1|1 -1..1 randomize randomize0 )

[unneeded] -1|1
?\ need random  : -1|1 ( -- -1|1 ) 2 random 2* 1- ;

  \ doc{
  \
  \ -1|1 ( -- -1|1 )
  \
  \ Return a random number: -1 or 1.
  \
  \ }doc

[unneeded] -1..1
?\ need random  : -1..1 ( -- -1|0|1 ) 3 random 1- ;

  \ doc{
  \
  \ -1..1 ( -- -1|0|1 )
  \
  \ Return a random number: -1, 0 or 1.
  \
  \ }doc

[unneeded] randomize
?\ need os-seed  : randomize ( n -- ) os-seed ! ;

  \ doc{
  \
  \ randomize ( n -- )
  \
  \ Set the seed of the random number generator to _n_.
  \
  \ See also: `randomize0`.
  \
  \ }doc

[unneeded] randomize0 ?exit

need os-frames need randomize

: randomize0 ( n -- )
  ?dup 0=  if  os-frames @  then  randomize ;

  \ doc{
  \
  \ randomize0 ( -- )
  \
  \ Set the seed of the random number generator to _n_;
  \ if _n_ is zero use the system frames counter instead.
  \
  \ See also: `randomize`.
  \
  \ }doc

  \ vim: filetype=soloforth