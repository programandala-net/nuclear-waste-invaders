#! /usr/bin/env gforth

\ chato.fs

\ This file is part of Nuclear Waste Invaders
\ http://programandala.net/en.program.nuclear_waste_invaders.html

\ Last modified: 201704201651
\ See change log at the end of the file

\ ==============================================================
\ Description

\ The "Chato" font, which was bundled with ZX-ALFA, a font
\ editor for ZX Spectrum written by Einar Saukas & Eduardo Ito,
\ ZX-SOFT Brasil Ltda, 1989.
\
\ The font was extracted to a TAP file, then converted to binary
\ grids by FantomoUDG
\ (http://programandala.net/en.program.fantomoudg.html), and
\ finally manually edited, in order to integrate it into the
\ build chain of the project.
\
\ This file is written in Forth (http://forth-standard.org) with
\ Gforth (http://gnu.org/software/gforth).

\ ==============================================================
\ Authors

\ Original "Chato" font, by Einar Saukas & Eduardo Ito, ZX-SOFT
\ Brasil Ltda 1989.
\
\ Adapted by Marcos Cruz (programandala.net), 2017.

\ ==============================================================
\ License

\ You may do whatever you want with this work, so long as you
\ retain the copyright/authorship/acknowledgment/credit
\ notice(s) and this license in all redistributed copies and
\ derived works.  There is no warranty.


\ ==============================================================

2 base !

\ ==============================================================
\ Original font

\ UDG position 0 = UDG 144 in a UDG set = character 32 (' ') in a font
00000000 emit
00000000 emit
00000000 emit
00000000 emit
00000000 emit
00000000 emit
00000000 emit
00000000 emit

\ UDG position 1 = UDG 145 in a UDG set = character 33 ('!') in a font
00000000 emit
00000000 emit
00010000 emit
00010000 emit
00010000 emit
00000000 emit
00010000 emit
00000000 emit

\ UDG position 2 = UDG 146 in a UDG set = character 34 ('"') in a font
00000000 emit
00000000 emit
00100100 emit
00100100 emit
00000000 emit
00000000 emit
00000000 emit
00000000 emit

\ UDG position 3 = UDG 147 in a UDG set = character 35 ('#') in a font
00000000 emit
00000000 emit
00100100 emit
01111110 emit
00100100 emit
01111110 emit
00100100 emit
00000000 emit

\ UDG position 4 = UDG 148 in a UDG set = character 36 ('$') in a font
00000000 emit
00001000 emit
00111110 emit
00101000 emit
00111110 emit
00001010 emit
00111110 emit
00001000 emit

\ UDG position 5 = UDG 149 in a UDG set = character 37 ('%') in a font
00000000 emit
01100010 emit
01100100 emit
00001000 emit
00010000 emit
00100110 emit
01000110 emit
00000000 emit

\ UDG position 6 = UDG 150 in a UDG set = character 38 ('&') in a font
00000000 emit
00010000 emit
00101000 emit
00010000 emit
00101010 emit
01000100 emit
00111010 emit
00000000 emit

\ UDG position 7 = UDG 151 in a UDG set = character 39 (''') in a font
00000000 emit
00000000 emit
00001000 emit
00010000 emit
00000000 emit
00000000 emit
00000000 emit
00000000 emit

\ UDG position 8 = UDG 152 in a UDG set = character 40 ('(') in a font
00000000 emit
00000000 emit
00000100 emit
00001000 emit
00001000 emit
00001000 emit
00000100 emit
00000000 emit

\ UDG position 9 = UDG 153 in a UDG set = character 41 (')') in a font
00000000 emit
00000000 emit
00100000 emit
00010000 emit
00010000 emit
00010000 emit
00100000 emit
00000000 emit

\ UDG position 10 = UDG 154 in a UDG set = character 42 ('*') in a font
00000000 emit
00000000 emit
00010100 emit
00001000 emit
00111110 emit
00001000 emit
00010100 emit
00000000 emit

\ UDG position 11 = UDG 155 in a UDG set = character 43 ('+') in a font
00000000 emit
00000000 emit
00001000 emit
00001000 emit
00111110 emit
00001000 emit
00001000 emit
00000000 emit

\ UDG position 12 = UDG 156 in a UDG set = character 44 (',') in a font
00000000 emit
00000000 emit
00000000 emit
00000000 emit
00000000 emit
00001000 emit
00001000 emit
00010000 emit

\ UDG position 13 = UDG 157 in a UDG set = character 45 ('-') in a font
00000000 emit
00000000 emit
00000000 emit
00000000 emit
00111110 emit
00000000 emit
00000000 emit
00000000 emit

\ UDG position 14 = UDG 158 in a UDG set = character 46 ('.') in a font
00000000 emit
00000000 emit
00000000 emit
00000000 emit
00000000 emit
00011000 emit
00011000 emit
00000000 emit

\ UDG position 15 = UDG 159 in a UDG set = character 47 ('/') in a font
00000000 emit
00000000 emit
00000000 emit
00000100 emit
00001000 emit
00010000 emit
00100000 emit
00000000 emit

\ UDG position 16 = UDG 160 in a UDG set = character 48 ('0') in a font
00000000 emit
00000000 emit
00111100 emit
01000110 emit
01011010 emit
01100010 emit
00111100 emit
00000000 emit

\ UDG position 17 = UDG 161 in a UDG set = character 49 ('1') in a font
00000000 emit
00000000 emit
00011000 emit
00101000 emit
00001000 emit
00001000 emit
00111110 emit
00000000 emit

\ UDG position 18 = UDG 162 in a UDG set = character 50 ('2') in a font
00000000 emit
00000000 emit
01111100 emit
00000010 emit
00111100 emit
01000000 emit
01111110 emit
00000000 emit

\ UDG position 19 = UDG 163 in a UDG set = character 51 ('3') in a font
00000000 emit
00000000 emit
00111100 emit
01000010 emit
00001100 emit
01000010 emit
00111100 emit
00000000 emit

\ UDG position 20 = UDG 164 in a UDG set = character 52 ('4') in a font
00000000 emit
00000000 emit
00001000 emit
00111000 emit
01001000 emit
01111110 emit
00001000 emit
00000000 emit

\ UDG position 21 = UDG 165 in a UDG set = character 53 ('5') in a font
00000000 emit
00000000 emit
01111110 emit
01000000 emit
01111100 emit
00000010 emit
01111100 emit
00000000 emit

\ UDG position 22 = UDG 166 in a UDG set = character 54 ('6') in a font
00000000 emit
00000000 emit
00111100 emit
01000000 emit
01111100 emit
01000010 emit
00111100 emit
00000000 emit

\ UDG position 23 = UDG 167 in a UDG set = character 55 ('7') in a font
00000000 emit
00000000 emit
01111110 emit
00000100 emit
00001000 emit
00010000 emit
00010000 emit
00000000 emit

\ UDG position 24 = UDG 168 in a UDG set = character 56 ('8') in a font
00000000 emit
00000000 emit
00111100 emit
01000010 emit
00111100 emit
01000010 emit
00111100 emit
00000000 emit

\ UDG position 25 = UDG 169 in a UDG set = character 57 ('9') in a font
00000000 emit
00000000 emit
00111100 emit
01000010 emit
00111110 emit
00000010 emit
00111100 emit
00000000 emit

\ UDG position 26 = UDG 170 in a UDG set = character 58 (':') in a font
00000000 emit
00000000 emit
00000000 emit
00010000 emit
00000000 emit
00000000 emit
00010000 emit
00000000 emit

\ UDG position 27 = UDG 171 in a UDG set = character 59 (';') in a font
00000000 emit
00000000 emit
00010000 emit
00000000 emit
00000000 emit
00010000 emit
00010000 emit
00100000 emit

\ UDG position 28 = UDG 172 in a UDG set = character 60 ('<') in a font
00000000 emit
00000000 emit
00000100 emit
00001000 emit
00010000 emit
00001000 emit
00000100 emit
00000000 emit

\ UDG position 29 = UDG 173 in a UDG set = character 61 ('=') in a font
00000000 emit
00000000 emit
00000000 emit
00111110 emit
00000000 emit
00111110 emit
00000000 emit
00000000 emit

\ UDG position 30 = UDG 174 in a UDG set = character 62 ('>') in a font
00000000 emit
00000000 emit
00010000 emit
00001000 emit
00000100 emit
00001000 emit
00010000 emit
00000000 emit

\ UDG position 31 = UDG 175 in a UDG set = character 63 ('?') in a font
00000000 emit
00000000 emit
00111100 emit
01000110 emit
00001000 emit
00000000 emit
00001000 emit
00000000 emit

\ UDG position 32 = UDG 176 in a UDG set = character 64 ('@') in a font
00000000 emit
00111100 emit
01001010 emit
01010110 emit
01011110 emit
01000000 emit
00111100 emit
00000000 emit

\ UDG position 33 = UDG 177 in a UDG set = character 65 ('A') in a font
00000000 emit
00000000 emit
00111100 emit
01000010 emit
01111110 emit
01000010 emit
01000010 emit
00000000 emit

\ UDG position 34 = UDG 178 in a UDG set = character 66 ('B') in a font
00000000 emit
00000000 emit
01111100 emit
01000010 emit
01111100 emit
01000010 emit
01111100 emit
00000000 emit

\ UDG position 35 = UDG 179 in a UDG set = character 67 ('C') in a font
00000000 emit
00000000 emit
00111100 emit
01000010 emit
01000000 emit
01000010 emit
00111100 emit
00000000 emit

\ UDG position 36 = UDG 180 in a UDG set = character 68 ('D') in a font
00000000 emit
00000000 emit
01111000 emit
01000100 emit
01000010 emit
01000100 emit
01111000 emit
00000000 emit

\ UDG position 37 = UDG 181 in a UDG set = character 69 ('E') in a font
00000000 emit
00000000 emit
01111110 emit
01000000 emit
01111100 emit
01000000 emit
01111110 emit
00000000 emit

\ UDG position 38 = UDG 182 in a UDG set = character 70 ('F') in a font
00000000 emit
00000000 emit
01111110 emit
01000000 emit
01111100 emit
01000000 emit
01000000 emit
00000000 emit

\ UDG position 39 = UDG 183 in a UDG set = character 71 ('G') in a font
00000000 emit
00000000 emit
00111100 emit
01000000 emit
01001110 emit
01000010 emit
00111100 emit
00000000 emit

\ UDG position 40 = UDG 184 in a UDG set = character 72 ('H') in a font
00000000 emit
00000000 emit
01000010 emit
01000010 emit
01111110 emit
01000010 emit
01000010 emit
00000000 emit

\ UDG position 41 = UDG 185 in a UDG set = character 73 ('I') in a font
00000000 emit
00000000 emit
00111110 emit
00001000 emit
00001000 emit
00001000 emit
00111110 emit
00000000 emit

\ UDG position 42 = UDG 186 in a UDG set = character 74 ('J') in a font
00000000 emit
00000000 emit
00000010 emit
00000010 emit
01000010 emit
01000010 emit
00111100 emit
00000000 emit

\ UDG position 43 = UDG 187 in a UDG set = character 75 ('K') in a font
00000000 emit
00000000 emit
01000100 emit
01001000 emit
01110000 emit
01001000 emit
01000100 emit
00000000 emit

\ UDG position 44 = UDG 188 in a UDG set = character 76 ('L') in a font
00000000 emit
00000000 emit
01000000 emit
01000000 emit
01000000 emit
01000000 emit
01111110 emit
00000000 emit

\ UDG position 45 = UDG 189 in a UDG set = character 77 ('M') in a font
00000000 emit
00000000 emit
01000010 emit
01100110 emit
01011010 emit
01000010 emit
01000010 emit
00000000 emit

\ UDG position 46 = UDG 190 in a UDG set = character 78 ('N') in a font
00000000 emit
00000000 emit
01000010 emit
01100010 emit
01011010 emit
01000110 emit
01000010 emit
00000000 emit

\ UDG position 47 = UDG 191 in a UDG set = character 79 ('O') in a font
00000000 emit
00000000 emit
00111100 emit
01000010 emit
01000010 emit
01000010 emit
00111100 emit
00000000 emit

\ UDG position 48 = UDG 192 in a UDG set = character 80 ('P') in a font
00000000 emit
00000000 emit
01111100 emit
01000010 emit
01111100 emit
01000000 emit
01000000 emit
00000000 emit

\ UDG position 49 = UDG 193 in a UDG set = character 81 ('Q') in a font
00000000 emit
00000000 emit
00111100 emit
01000010 emit
01010010 emit
01001010 emit
00111100 emit
00000000 emit

\ UDG position 50 = UDG 194 in a UDG set = character 82 ('R') in a font
00000000 emit
00000000 emit
01111100 emit
01000010 emit
01111100 emit
01000100 emit
01000010 emit
00000000 emit

\ UDG position 51 = UDG 195 in a UDG set = character 83 ('S') in a font
00000000 emit
00000000 emit
00111100 emit
01000000 emit
00111100 emit
00000010 emit
01111100 emit
00000000 emit

\ UDG position 52 = UDG 196 in a UDG set = character 84 ('T') in a font
00000000 emit
00000000 emit
11111110 emit
00010000 emit
00010000 emit
00010000 emit
00010000 emit
00000000 emit

\ UDG position 53 = UDG 197 in a UDG set = character 85 ('U') in a font
00000000 emit
00000000 emit
01000010 emit
01000010 emit
01000010 emit
01000010 emit
00111100 emit
00000000 emit

\ UDG position 54 = UDG 198 in a UDG set = character 86 ('V') in a font
00000000 emit
00000000 emit
01000010 emit
01000010 emit
01000010 emit
00100100 emit
00011000 emit
00000000 emit

\ UDG position 55 = UDG 199 in a UDG set = character 87 ('W') in a font
00000000 emit
00000000 emit
01000010 emit
01000010 emit
01000010 emit
01011010 emit
00100100 emit
00000000 emit

\ UDG position 56 = UDG 200 in a UDG set = character 88 ('X') in a font
00000000 emit
00000000 emit
01000010 emit
00100100 emit
00011000 emit
00100100 emit
01000010 emit
00000000 emit

\ UDG position 57 = UDG 201 in a UDG set = character 89 ('Y') in a font
00000000 emit
00000000 emit
10000010 emit
01000100 emit
00101000 emit
00010000 emit
00010000 emit
00000000 emit

\ UDG position 58 = UDG 202 in a UDG set = character 90 ('Z') in a font
00000000 emit
00000000 emit
01111110 emit
00000100 emit
00011000 emit
00100000 emit
01111110 emit
00000000 emit

\ UDG position 59 = UDG 203 in a UDG set = character 91 ('[') in a font
00000000 emit
00000000 emit
00001110 emit
00001000 emit
00001000 emit
00001000 emit
00001110 emit
00000000 emit

\ UDG position 60 = UDG 204 in a UDG set = character 92 ('\') in a font
00000000 emit
00000000 emit
00000000 emit
00100000 emit
00010000 emit
00001000 emit
00000100 emit
00000000 emit

\ UDG position 61 = UDG 205 in a UDG set = character 93 (']') in a font
00000000 emit
00000000 emit
01110000 emit
00010000 emit
00010000 emit
00010000 emit
01110000 emit
00000000 emit

\ UDG position 62 = UDG 206 in a UDG set = character 94 ('^') in a font
00000000 emit
00000000 emit
00010000 emit
00111000 emit
01010100 emit
00010000 emit
00010000 emit
00000000 emit

\ UDG position 63 = UDG 207 in a UDG set = character 95 ('_') in a font
00000000 emit
00000000 emit
00000000 emit
00000000 emit
00000000 emit
00000000 emit
00000000 emit
11111111 emit

\ UDG position 64 = UDG 208 in a UDG set = character 96 ('`') in a font
00000000 emit
01111110 emit
01000000 emit
00100000 emit
00010000 emit
00100010 emit
01111110 emit
00000000 emit

\ UDG position 65 = UDG 209 in a UDG set = character 97 ('a') in a font
00000000 emit
00000000 emit
00000000 emit
00111000 emit
00000100 emit
01111100 emit
00111100 emit
00000000 emit

\ UDG position 66 = UDG 210 in a UDG set = character 98 ('b') in a font
00000000 emit
00000000 emit
00100000 emit
00100000 emit
00111100 emit
00100010 emit
00111100 emit
00000000 emit

\ UDG position 67 = UDG 211 in a UDG set = character 99 ('c') in a font
00000000 emit
00000000 emit
00000000 emit
00011100 emit
00100000 emit
00100000 emit
00011100 emit
00000000 emit

\ UDG position 68 = UDG 212 in a UDG set = character 100 ('d') in a font
00000000 emit
00000000 emit
00000100 emit
00000100 emit
00111100 emit
01000100 emit
00111100 emit
00000000 emit

\ UDG position 69 = UDG 213 in a UDG set = character 101 ('e') in a font
00000000 emit
00000000 emit
00000000 emit
00111000 emit
01111100 emit
01000000 emit
00111100 emit
00000000 emit

\ UDG position 70 = UDG 214 in a UDG set = character 102 ('f') in a font
00000000 emit
00000000 emit
00001100 emit
00010000 emit
00011000 emit
00010000 emit
00010000 emit
00000000 emit

\ UDG position 71 = UDG 215 in a UDG set = character 103 ('g') in a font
00000000 emit
00000000 emit
00000000 emit
00111100 emit
01000100 emit
00111100 emit
00000100 emit
00111000 emit

\ UDG position 72 = UDG 216 in a UDG set = character 104 ('h') in a font
00000000 emit
00000000 emit
01000000 emit
01000000 emit
01111000 emit
01000100 emit
01000100 emit
00000000 emit

\ UDG position 73 = UDG 217 in a UDG set = character 105 ('i') in a font
00000000 emit
00000000 emit
00010000 emit
00000000 emit
00010000 emit
00010000 emit
00111000 emit
00000000 emit

\ UDG position 74 = UDG 218 in a UDG set = character 106 ('j') in a font
00000000 emit
00000000 emit
00000100 emit
00000000 emit
00000100 emit
00000100 emit
00100100 emit
00011000 emit

\ UDG position 75 = UDG 219 in a UDG set = character 107 ('k') in a font
00000000 emit
00000000 emit
00100000 emit
00101000 emit
00110000 emit
00101000 emit
00100100 emit
00000000 emit

\ UDG position 76 = UDG 220 in a UDG set = character 108 ('l') in a font
00000000 emit
00000000 emit
00010000 emit
00010000 emit
00010000 emit
00010000 emit
00001100 emit
00000000 emit

\ UDG position 77 = UDG 221 in a UDG set = character 109 ('m') in a font
00000000 emit
00000000 emit
00000000 emit
01101000 emit
01010100 emit
01010100 emit
01010100 emit
00000000 emit

\ UDG position 78 = UDG 222 in a UDG set = character 110 ('n') in a font
00000000 emit
00000000 emit
00000000 emit
01111000 emit
01000100 emit
01000100 emit
01000100 emit
00000000 emit

\ UDG position 79 = UDG 223 in a UDG set = character 111 ('o') in a font
00000000 emit
00000000 emit
00000000 emit
00111000 emit
01000100 emit
01000100 emit
00111000 emit
00000000 emit

\ UDG position 80 = UDG 224 in a UDG set = character 112 ('p') in a font
00000000 emit
00000000 emit
00000000 emit
01111000 emit
01000100 emit
01111000 emit
01000000 emit
01000000 emit

\ UDG position 81 = UDG 225 in a UDG set = character 113 ('q') in a font
00000000 emit
00000000 emit
00000000 emit
00111100 emit
01000100 emit
00111100 emit
00000100 emit
00000110 emit

\ UDG position 82 = UDG 226 in a UDG set = character 114 ('r') in a font
00000000 emit
00000000 emit
00000000 emit
00011100 emit
00100000 emit
00100000 emit
00100000 emit
00000000 emit

\ UDG position 83 = UDG 227 in a UDG set = character 115 ('s') in a font
00000000 emit
00000000 emit
00000000 emit
00111000 emit
01111000 emit
00000100 emit
01111000 emit
00000000 emit

\ UDG position 84 = UDG 228 in a UDG set = character 116 ('t') in a font
00000000 emit
00000000 emit
00010000 emit
00111000 emit
00010000 emit
00010000 emit
00001100 emit
00000000 emit

\ UDG position 85 = UDG 229 in a UDG set = character 117 ('u') in a font
00000000 emit
00000000 emit
00000000 emit
01000100 emit
01000100 emit
01000100 emit
00111000 emit
00000000 emit

\ UDG position 86 = UDG 230 in a UDG set = character 118 ('v') in a font
00000000 emit
00000000 emit
00000000 emit
01000100 emit
00101000 emit
00101000 emit
00010000 emit
00000000 emit

\ UDG position 87 = UDG 231 in a UDG set = character 119 ('w') in a font
00000000 emit
00000000 emit
00000000 emit
01000100 emit
01010100 emit
01010100 emit
00101000 emit
00000000 emit

\ UDG position 88 = UDG 232 in a UDG set = character 120 ('x') in a font
00000000 emit
00000000 emit
00000000 emit
01000100 emit
00101000 emit
00111000 emit
01000100 emit
00000000 emit

\ UDG position 89 = UDG 233 in a UDG set = character 121 ('y') in a font
00000000 emit
00000000 emit
00000000 emit
01000100 emit
01000100 emit
00111100 emit
00000100 emit
00111000 emit

\ UDG position 90 = UDG 234 in a UDG set = character 122 ('z') in a font
00000000 emit
00000000 emit
00000000 emit
01111100 emit
00011000 emit
00100000 emit
01111100 emit
00000000 emit

\ UDG position 91 = UDG 235 in a UDG set = character 123 ('{') in a font
00000000 emit
00000000 emit
00001110 emit
00001000 emit
00110000 emit
00001000 emit
00001110 emit
00000000 emit

\ UDG position 92 = UDG 236 in a UDG set = character 124 ('|') in a font
00000000 emit
00000000 emit
00001000 emit
00001000 emit
00001000 emit
00001000 emit
00001000 emit
00000000 emit

\ UDG position 93 = UDG 237 in a UDG set = character 125 ('}') in a font
00000000 emit
00000000 emit
01110000 emit
00010000 emit
00001100 emit
00010000 emit
01110000 emit
00000000 emit

\ UDG position 94 = UDG 238 in a UDG set = character 126 ('~') in a font
00000000 emit
00000000 emit
00110010 emit
01001100 emit
00000000 emit
00000000 emit
00000000 emit
00000000 emit

\ UDG position 95 = UDG 239 in a UDG set = character 127 in a font
00000000 emit
00000000 emit
00000000 emit
00001000 emit
00010100 emit
00100010 emit
01111111 emit
00000000 emit

\ ==============================================================
\ Spanish characters

\ character 128 ('¿')
00000000 emit
00000000 emit
00010000 emit
00000000 emit
00010000 emit
01100010 emit
00111100 emit
00000000 emit

\ character 129 ('¡')
00000000 emit
00001000 emit
00000000 emit
00001000 emit
00001000 emit
00001000 emit
00000000 emit
00000000 emit

\ character 130 ('Á')
00000100 emit
00001000 emit
00111100 emit
01000010 emit
01111110 emit
01000010 emit
01000010 emit
00000000 emit

\ character 131 ('É')
00000100 emit
00001000 emit
01111110 emit
01000000 emit
01111100 emit
01000000 emit
01111110 emit
00000000 emit

\ character 132 ('Í')
00000100 emit
00001000 emit
00111110 emit
00001000 emit
00001000 emit
00001000 emit
00111110 emit
00000000 emit

\ character 133 ('Ó')
00000100 emit
00001000 emit
00111100 emit
01000010 emit
01000010 emit
01000010 emit
00111100 emit
00000000 emit

\ character 134 ('Ú')
00000100 emit
00001000 emit
01000010 emit
01000010 emit
01000010 emit
01000010 emit
00111100 emit
00000000 emit

\ character 135 ('Ü')
01000010 emit
00000000 emit
01000010 emit
01000010 emit
01000010 emit
01000010 emit
00111100 emit
00000000 emit

\ character 136 ('Ñ')
00101000 emit
00010100 emit
01000010 emit
01100010 emit
01011010 emit
01000110 emit
01000010 emit
00000000 emit

\ character 137 ('á')
00000000 emit
00001000 emit
00010000 emit
00111000 emit
00000100 emit
01111100 emit
00111100 emit
00000000 emit

\ character 138 ('é')
00000000 emit
00001000 emit
00010000 emit
00111000 emit
01111100 emit
01000000 emit
00111100 emit
00000000 emit

\ character 139 ('í')
00000000 emit
00001000 emit
00010000 emit
00000000 emit
00010000 emit
00010000 emit
00111000 emit
00000000 emit

\ character 140 ('ó')
00000000 emit
00001000 emit
00010000 emit
00111000 emit
01000100 emit
01000100 emit
00111000 emit
00000000 emit

\ character 141 ('ú')
00000000 emit
00001000 emit
00010000 emit
01000100 emit
01000100 emit
01000100 emit
00111000 emit
00000000 emit

\ character 142 ('ü')
00000000 emit
01000100 emit
00000000 emit
01000100 emit
01000100 emit
01000100 emit
00111000 emit
00000000 emit

\ character 143 ('ñ')
00101000 emit
00010100 emit
00000000 emit
01111000 emit
01000100 emit
01000100 emit
01000100 emit
00000000 emit

\ ==============================================================
\ Esperanto characters

\ character 144 ('Ĉ')
00001000 emit
00010100 emit
00111100 emit
01000010 emit
01000000 emit
01000010 emit
00111100 emit
00000000 emit

\ character 145 ('ĉ')
00001000 emit
00010100 emit
00000000 emit
00011100 emit
00100000 emit
00100000 emit
00011100 emit
00000000 emit

\ character 146 ('Ĝ')
00001000 emit
00010100 emit
00111100 emit
01000000 emit
01001110 emit
01000010 emit
00111100 emit
00000000 emit

\ character 147 ('ĝ')
00001000 emit
00010100 emit
00000000 emit
00111100 emit
01000100 emit
00111100 emit
00000100 emit
00111000 emit

\ character 148 ('Ĥ')
00001000 emit
00010100 emit
01000010 emit
01000010 emit
01111110 emit
01000010 emit
01000010 emit
00000000 emit

\ character 149 ('ĥ')
00001000 emit
00010100 emit
01000000 emit
01000000 emit
01111000 emit
01000100 emit
01000100 emit
00000000 emit

\ character 150 ('Ĵ')
00001000 emit
00010100 emit
00000010 emit
00000010 emit
01000010 emit
01000010 emit
00111100 emit
00000000 emit

\ character 151 ('ĵ')
00000000 emit
00000100 emit
00001010 emit
00000000 emit
00000100 emit
00000100 emit
00100100 emit
00011000 emit

\ character 152 ('Ŝ')
00001000 emit
00010100 emit
00111100 emit
01000000 emit
00111100 emit
00000010 emit
01111100 emit
00000000 emit

\ character 153 ('ŝ')
00010000 emit
00101000 emit
00000000 emit
00111000 emit
01111000 emit
00000100 emit
01111000 emit
00000000 emit

\ character 154 ('Ŭ')
00100100 emit
00011000 emit
01000010 emit
01000010 emit
01000010 emit
01000010 emit
00111100 emit
00000000 emit

\ character 155 ('ŭ')
00100100 emit
00011000 emit
00000000 emit
01000100 emit
01000100 emit
01000100 emit
00111000 emit
00000000 emit

\ ==============================================================
\ French characters

\ character 156 ('â')
00010000 emit
00101000 emit
00000000 emit
00111000 emit
00000100 emit
01111100 emit
00111100 emit
00000000 emit

\ ==============================================================
\ End

bye

\ ==============================================================
\ Change log

\ 2017-04-18: Start.
\
\ 2017-04-20: Add non-English characters.
