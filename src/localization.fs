  \ localization.fs
  \
  \ This file is part of Nuclear Waste Invaders
  \ http://programandala.net/en.program.nuclear_waste_invaders.html
  \
  \ XXX UNDER DEVELOPMENT -- not used yet
  \
  \ Last modified 201612021651

  \ 2016-12-04: Start. Based on code from Tron 0xF
  \ (http://programandala.net/en.program.tron_0xf.html).

  \ ===========================================================
  \ Languages

get-current  forth-wordlist dup >order set-current

need value  need cenum  need +perform

set-current previous

0   cenum en         \ English
    cenum eo         \ Esperanto
    cenum es         \ Spanish
    cconstant langs  \ number of languages

en value lang  \ current language

: localized ( xt[langs]..xt[1] "name" -- )
  create langs 0 ?do , loop
  does> ( -- ) ( pfa ) lang +perform ;

  \ Create a word _name_ that will execute an execution token
  \ from a table, depending on the current language.
  \
  \ The parameters are the execution tokens of the localized
  \ versions, in reverse order of ISO language code: es, eo,
  \ en.

  \ ===========================================================
  cr .( Menu options)

:noname ( -- ca len ) s" No en español" ;
:noname ( -- ca len ) s" Ne en Esperanto" ;
:noname ( -- ca len ) s" Not in English" ;
localized option-0$ ( -- ca len )

:noname ( -- ca len ) s" Instrucciones" ;
:noname ( -- ca len ) s" Instrukcioj" ;
:noname ( -- ca len ) s" Instructions" ;
localized option-1$ ( -- ca len )

:noname ( -- ca len ) s" Configurar" ;
:noname ( -- ca len ) s" Konfiguri" ;
:noname ( -- ca len ) s" Configure" ;
localized option-2$ ( -- ca len )

:noname ( -- ca len ) s" Jugar" ;
:noname ( -- ca len ) s" Ludi" ;
:noname ( -- ca len ) s" Play" ;
localized option-3$ ( -- ca len )

:noname ( -- ca len ) s" Acerca de" ;
:noname ( -- ca len ) s" Pri" ;
:noname ( -- ca len ) s" About" ;
localized option-5$ ( -- ca len )

:noname ( -- ca len ) s" Salir" ;
:noname ( -- ca len ) s" Eliri" ;
:noname ( -- ca len ) s" Quit" ;
localized option-6$ ( -- ca len )

  \ ===========================================================
  cr .( Localized texts)

' noop
' noop
' noop
localized instructions ( -- )

:noname ( -- ca len ) s" Izquierda" ;
:noname ( -- ca len ) s" Maldekstra" ;
:noname ( -- ca len ) s" Left" ;
localized left$ ( -- ca len )

:noname ( -- ca len ) s" Derecha" ;
:noname ( -- ca len ) s" Dekstra" ;
:noname ( -- ca len ) s" Right" ;
localized right$ ( -- ca len )

:noname ( -- ca len ) s" D1" ;
:noname ( -- ca len ) s" P1" ;
:noname ( -- ca len ) s" F1" ;
localized fire-1$ ( -- ca len )

:noname ( -- ca len ) s" D2" ;
:noname ( -- ca len ) s" P2" ;
:noname ( -- ca len ) s" F1" ;
localized fire-2$ ( -- ca len )

:noname ( -- ca len ) s" Pulsa una tecla" ;
:noname ( -- ca len ) s" Premu klavon" ;
:noname ( -- ca len ) s" Press any key" ;
localized press-any-key$ ( -- ca len )

:noname ( -- ca len ) s" " ;
:noname ( -- ca len ) s" " ;
:noname ( -- ca len ) s" " ;
localized do-stop$ ( -- ca len )

:noname ( -- c ) 'S' ;
:noname ( -- c ) 'J' ;
:noname ( -- c ) 'Y' ;
localized "y" ( -- c )

:noname ( -- ) ;
:noname ( -- ) ;
:noname ( -- ) ;
localized quit-message ( -- )

:noname ( -- ca len ) s" Versión" ;  \ XXX TODO -- ó
:noname ( -- ca len ) s" Versio" ;
:noname ( -- ca len ) s" Version" ;
localized version$ ( -- ca len )

:noname ( -- ca len ) s" " ;
:noname ( -- ca len ) s" " ;
:noname ( -- ca len ) s" " ;
localized menu-help$ ( -- ca len )

  \ ===========================================================
  cr .( Texts that don't need translation)

:noname ( -- ca len ) s" " ;
:noname ( -- ca len ) s" " ;
:noname ( -- ca len ) s" " ;
localized license ( -- )

:noname ( -- ca len ) s" " ;
:noname ( -- ca len ) s" " ;
:noname ( -- ca len ) s" " ;
localized authors ( -- )

  \ vim: filetype=soloforth:colorcolumn=64
