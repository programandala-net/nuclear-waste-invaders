  \ 001_loader.fs
  \
  \ This file is part of Nuclear Invaders
  \ http://programandala.net/en.program.nuclear_invaders.html

( Nuclear Invaders -- load block  )

[defined] need
?\ 2 load
  \ Compile the `need` utility, which must be in block 2.

  \ need decode need words need order need dump
  \ XXX TMP -- for debugging

need load-app

load-app nuclear-invaders

  \ Compile the game.

  \ vim: filetype=soloforth