" utf8_to_zx_spectrum.vim

" This file is part of Nuclear Waste Invaders
" http://programandala.net/en.program.nuclear_waste_invaders.html

" Last modified 201704172351
" See change log at the end of the file

" ==============================================================
" Description

" This program converts the UTF-8 characters in the sources to ZX Spectrum
" User Defined Graphic codes.

" ==============================================================
" Author

" Marcos Cruz (programandala.net), 2016, 2017.

" ==============================================================
" Spanish characters

echo "Converting Spanish UTF-8 characters to ZX Spectrum codes"

%s/¡/\=nr2char(128)/gIe
%s/¿/\=nr2char(129)/gIe
%s/Á/\=nr2char(130)/gIe
%s/É/\=nr2char(131)/gIe
%s/Í/\=nr2char(132)/gIe
%s/Ñ/\=nr2char(133)/gIe
%s/Ó/\=nr2char(134)/gIe
%s/Ú/\=nr2char(135)/gIe
%s/Ü/\=nr2char(136)/gIe
%s/á/\=nr2char(137)/gIe
%s/é/\=nr2char(138)/gIe
%s/í/\=nr2char(139)/gIe
%s/ñ/\=nr2char(140)/gIe
%s/ó/\=nr2char(141)/gIe
%s/ú/\=nr2char(142)/gIe
%s/ü/\=nr2char(143)/gIe

" ==============================================================
" Esperanto characters

echo "Converting Esperanto UTF-8 characters to ZX Spectrum codes"

%s/Ĉ/\=nr2char(128)/gIe
%s/ĉ/\=nr2char(129)/gIe
%s/Ĝ/\=nr2char(130)/gIe
%s/ĝ/\=nr2char(131)/gIe
%s/Ĥ/\=nr2char(132)/gIe
%s/ĥ/\=nr2char(133)/gIe
%s/Ĵ/\=nr2char(134)/gIe
%s/ĵ/\=nr2char(135)/gIe
%s/Ŝ/\=nr2char(136)/gIe
%s/ŝ/\=nr2char(137)/gIe
%s/Ŭ/\=nr2char(138)/gIe
%s/ŭ/\=nr2char(139)/gIe

" ==============================================================
" Change log

" 2017-04-17: Start, based on the code used by Black Flag
" (http://programandala.net/en.program.black_flag.html)

