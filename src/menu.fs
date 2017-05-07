  \ menu.fs
  \
  \ This file is part of Nuclear Waste Invaders
  \ http://programandala.net/en.program.nuclear_waste_invaders.html

  \ XXX UNDER DEVELOPMENT -- not used yet
  \
  \ Last modified 201703111732
  \

  \ ===========================================================
  \ Data

package menu-package

6 cconstant options       \ number of menu options
2 cconstant lines/option  \ lines occupied per menu option

contents-line 2+ cconstant menu-line

create menu-keys  ( -- a )  options allot
  \ A table to store the menu keys. They change depending on
  \ the current language. This table is updated every time the
  \ menu is printed, using the initials of the menu options.

: menu-key  ( n -- c )  menu-keys + c@  ;
  \ Convert menu option _n_ to its menu key _c_.

  \ ===========================================================
  \ Commands

: change-language  ( -- )
  lang 1+ dup langs < abs *  to lang lang-udg wipe-contents  ;
  \ Change the current language.

: about  ( -- )
  whole-page copyright cr cr authors more license more page  ;

: instructions  ( -- )  whole-page (instructions) more page  ;

: finish  ( -- )
  default-colors page -font greeting cr quit-message quit  ;

create menu-commands
  ] change-language instructions configure game about finish [
  \ Execution table of the menu commands.

  \ Note: Every command must leave the screen ready to print
  \ the menu: Commands that use the whole screen must call
  \ `page`; commands that use only the contents zone must call
  \ `wipe-contents` (or `more`, that calls it); commands that
  \ change only their own menu option must set ink to white.

: menu-command  ( n -- xt )  cells menu-commands + @  ;
  \ Convert menu option _n_ to its command _xt_.

  \ ===========================================================
  \ Options

: initial!  ( n c -- )  swap menu-keys + c!  ;
  \ Store char _c_ into position _n_ of the `menu-keys` table.

create menu-options  ( -- a )
  ] option-0$ option-1$ option-2$
    option-3$ option-4$ option-5$ [
  \ Table that holds the execution tokens of the words that
  \ return the localized texts of the menu options.

: option-initial  ( ca n -- )
  swap c@ upper dup initial. initial!  ;
  \ Print the initial of the menu option number _n_, stored at
  \ _ca_ and also store it into the `menu-keys` table.

: option>line  (  n -- line )  lines/option * menu-line +  ;
  \ Convert a menu option number _n_ to its line _line_.

: option>string  ( n -- ca len )  menu-options swap +perform  ;
  \ Convert a menu option number _n_ to its localized text _ca
  \ len_.

  \ ===========================================================
  \ Option

: option  ( n -- )
  dup >r                    ( n ) ( R: n )
  dup option>line           ( n line )
  >r option>string          ( ca len ) ( R: n line )
  dup centered              ( ca len col )
  r> swap                   ( ca len line col ) ( R: n )
  2dup at-xy 2over drop r>  ( ca len line col ca n ) ( R: )
  option-initial            ( ca len line col )
  1+ at-xy 1 /string type  ;
  \ Print menu option number n.

  \ XXX TODO -- make it simpler and faster: calculate
  \ the coordinates when the strings are created, and
  \ store them in a table.

  \ ===========================================================
  \ Valid option

: valid-option?  ( c -- xt true | false )
  upper
  false swap  \ default exit flag
  options 0 ?do
    dup i menu-key =
    if  2drop i menu-command true dup leave  then
  loop  drop  ;
  \ Is the given character a valid menu option in the current
  \ language? If so, return the xt of its associated command
  \ and a true flag; otherwise return a false flag.

: key-sound  ( -- )  ;
  \ XXX TODO --

public

: valid-option  ( -- xt )
  begin  key valid-option?  until  key-sound  ;
  \ Wait until a valid menu option is chosen and then
  \ return the xt of its associated command.

: menu  ( -- )
  options 0 do  i option  loop  reveal-contents 256 ms  ;
  \ Print the menu.

end-package

  \ ===========================================================
  \ Boot

: init  ( -- )
  only forth nuclear-wordlist >order
  default-colors page  ;

only forth definitions

: run  ( -- )
  init begin  menu valid-option execute  again  ;
  \ Endless loop: Show the menu and execute the chosen option.

  \ ===========================================================
  \ Change log

  \ 2016-12-04: Start. Based on code from Tron 0xF
  \ (http://programandala.net/en.program.tron_0xf.html).

  \ vim: filetype=soloforth:colorcolumn=64
