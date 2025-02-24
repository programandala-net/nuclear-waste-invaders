# Nuclear Waste Invaders

## English

### Project

*Nuclear Waste Invaders* is a ZX Spectrum 128 remake, under development,
of Jupiter ACE computer’s [Nuclear
Invaders](http://www.zonadepruebas.com/viewtopic.php?t=4231) (Dancresp,
2013).

It is written in [Forth](http://standard-forth.org) with [Solo
Forth](http://programandala.net/en.program.solo_forth.html) ([Solo Forth
in GitHub](http://github.com/programandala-net/solo-forth)).

Web page:
<http://programandala.net/en.program.nuclear_waste_invaders.html>.

### How to compile and run

If you cannot wait until the first public version is resealed, you can
build the program yourself:

1.  Make sure the required programs, listed in the header of the
    \<Makefile\> file, are installed on your system.

2.  Type `make` in the project directory to build the
    \<disk_2_nuclear_waste_invaders.mgt\> disk image and the
    \<graphics.tap\> tape image.

3.  Start a ZX Spectrum 128 emulator with the Plus D interface.

4.  Enter 128 BASIC.

5.  Insert the Solo Forth’s \<disk_0_boot.mgt\> disk image into disk
    drive 1 of your emulator.

6.  Type the BASIC command `run` to load G+DOS and Solo Forth from disk
    drive 1.

7.  Insert the \<disk_2_nuclear_waste_invaders.mgt\> disk image into
    disk drive 1 of your emulator.

8.  Open the \<graphics_and_font.tap\> tape image as the input tape of
    your emulator.

9.  Type Forth command `1 load` to load block 1 from the current drive.
    This will start the compilation. Messages will be shown during the
    process.

10. Set your emulator to its maximum speed and wait. When the
    compilation is finished, the screen is cleared and a message is
    displayed.

11. Set your emulator to the normal speed (100%).

12. In order to save the compiled game, create a SZX snapshot with your
    emulator.

13. Type `run` in Forth to start the game.

### History of the repository

- 2020-12-05: The Git repository was converted to
  [Fossil](https://fossil-scm.org), keeping [GitHub as a
  mirror](http://github.com/programandala-net/nuclear-waste-invaders).

- 2023-04-05: The repository was converted to
  https:://mercurial-scm.org\[Mercurial,role="external"\], enabling a
  better interaction with GitHub.

## Español

### Proyecto

*Nuclear Waste Invaders* (Invasores de residuos nucleares) es una
reinterpretación en desarrollo para ZX Spectrum 128, del [Nuclear
Invaders](http://www.zonadepruebas.com/viewtopic.php?t=4231) (Dancresp,
2013) de la computadora Jupiter Ace.

Está escrito en [Forth](http://standard-forth.org) con [Solo
Forth](http://programandala.net/en.program.solo_forth.html) ([Solo Forth
en GitHub](http://github.com/programandala-net/solo-forth)).

Página en la red:
<http://programandala.net/es.programa.nuclear_waste_invaders.html>.

### Cómo compilar y arrancar

Si no puedes esperar a la salida de la primera versión pública, puedes
construir el programa tú mismo:

1.  Asegúrate de que los programas necesarios, que están listados en la
    cabecera del fichero \<Makefile\>, están instalados en tu sistema.

2.  Da la orden `make` en el directorio del proyecto para construir la
    imagen de disquete \<disk_2_nuclear_waste_invaders.mgt\> y la imagen
    de cinta de casete \<graphics.tap\>.

3.  Arranca un emulador de ZX Spectrum 128 con la interfaz Plus D.

4.  Entra en 128 BASIC.

5.  Inserta la imagen de disquete \<disk_0_boot.mgt\> de Solo Forth en
    la disquetera 1 de tu emulador.

6.  Da el comando de BASIC `run` para cargar G+DOS y Solo Forth desde la
    disquetera 1.

7.  Inserta la imagen de disquete \<disk_2_nuclear_waste_invaders.mgt\>
    en la disquetera 1 de tu emulador.

8.  Abre la imagen de cinta de casete \<graphics_and_font.tap\> como
    cinta de entrada de tu emulador.

9.  Escribe el comando de Forth `1 load` para cargar el bloque 1 de la
    disquetera actual. Esto iniciará la compilación. Durante el proceso
    se mostrarán mensajes en la pantalla.

10. Pon la velocidad de tu emulador al máximo y espera. Cuando la
    compilación haya terminado, se borrará la pantalla y se mostrará un
    mensaje.

11. Pon tu emulador a la velocidad normal (100%).

12. Para guardar el juego compilado, crea una imagen del sistema en
    formato SZX usando tu emulador.

13. Escribe `run` en Forth para empezar el juego.

### Historia del repositorio

- 2020-12-05: El repositorio de Git fue convertido a
  [Fossil](https://fossil-scm.org), dejando [GitHub como
  réplica](http://github/programandala-net/nuclear-waste-invaders).

- 2023-04-05: El repositorio fue convertido a
  [Mercurial](https://mercurial-scm.org), posibilitando una mejor
  interacción con GitHub.

## Esperanto

### Projekto

*Nuclear Waste Invaders* (Atomrubaĵaj Invadantoj) esta programata
reinterpreto, por la komputilo ZX Spectrum 128, el [Nuclear
Invaders](http://www.zonadepruebas.com/viewtopic.php?t=4231) (Dancresp,
2013) de la komputilo Jupiter ACE.

Ĝi estas verkata en [Forth](http://standard-forth.org) per [Solo
Forth](http://programandala.net/en.program.solo_forth.html) ([Solo Forth
en GitHub](http://github.com/programandala-net/solo-forth)).

Retpaĝo:
<http://programandala.net/eo.programo.nuclear_waste_invaders.html>.

### Kiel kompili kaj funkciigi

Se vi ne povas atendi li aperon de la unua publika versio, vi mem povas
konstrui li programon:

1.  Certiĝu ke la necesaj programoj, listigitaj supre de la dosiero
    \<Makefile\>, estas instalitaj en via sistemo.

2.  Tajpu `make` en la dosierujo de la projekto por krei la diskedeskan
    dosieron \<disk_2_nuclear_waste_invaders.mgt\> kaj la kasedeskan
    dosieron \<graphics.tap\>.

3.  Enmetu la diskedeskan dosieron \<disk_0_boot.mgt\> de Solo Forth en
    la diskedujon 1 de via emulilo.

4.  Skribu la BASIC-ordonon `run` por funkciigi G+DOS kaj Solo Forth el
    la diskedujo 1.

5.  Enmetu la diskedeskan dosieron \<disk_2_nuclear_waste_invaders.mgt\>
    en la diskedujon 1 de via emulilo.

6.  Malfermu la kasedeskan dosieron \<graphics_and_font.tap\> kiel
    enir-kasedon de via emulilo.

7.  Skribu ordonon de Forth `1 load` por preni blokon 1 el la nuna
    diskedujo. Ĉi tio komencos la kompiladon, dum kiu mesaĝoj estos
    montrataj sur la ekrano.

8.  Elektu la plej grandan rapidon de via emulilo kaj atendu. Kiam la
    kompilado finiĝos, la ekrano estos forviŝita kaj mesaĝo estos
    printita.

9.  Elektu normalan (100%) rapidon en via emulilo.

10. Por konservi la kompilitan ludon, kreu kopion de la tuta sistemo en
    SZX-formata dosiero, uzante vian emulilon.

11. En Fortho tajpu `run` por komenci la ludon.

### Historio de la deponejo

- 2020-12-05: La Git-deponejo estis konvertita al
  [Fossil](https://fossil-scm.org), konservante [GitHub kiel
  kopion](http://github/programandala-net/nuclear-waste-invaders).

- 2023-04-05: La deponejo estis konvertita al
  [Mercurial](https://mercurial-scm.org), ebligante pli bonan interagon
  kun GitHub.

## Interlingue

### Projecte

*Nuclear Waste Invaders* (Invasores de jetallia nucleari) es un
reinterpretation, developat por ZX Spectrum 128, del [Nuclear
Invaders](http://www.zonadepruebas.com/viewtopic.php?t=4231) (Dancresp,
2013) del computator Jupiter ACE.

It es scrit in [Forth](http://standard-forth.org) con [Solo
Forth](http://programandala.net/en.program.solo_forth.html) ([Solo Forth
in GitHub](http://github.com/programandala-net/solo-forth)).

Págine web:
<http://programandala.net/ie.programa.nuclear_waste_invaders.html>.

### Qualmen compilar e initiar

Si vu ne posse atender li aparition del prim version public, vu self
posse constructer li programma:

1.  Controla que li necessi programmas, quel es listat in li supra del
    document \<Makefile\>, es instalat in vor computator.

2.  Comanda `make` in li documentiere del projecte por constructer li
    disco-replica \<disk_2_nuclear_waste_invaders.mgt\> e li
    bande-replica \<graphics.tap\>.

3.  Inicia un emulator de ZX Spectrum 128 con li interfacie Plus D.

4.  Intra in 128 BASIC.

5.  Inserte li disco-replica \<disk_0_boot.mgt\> de Solo Forth in li
    disciere 1 de vor emulator.

6.  In BASIC comanda `run` por cargar G+DOS e Solo Forth del disciere 1.

7.  Inserte li disco-replica \<disk_2_nuclear_waste_invaders.mgt\> in li
    disciere 1 de vor emulator.

8.  Aperte li bande-replica \<graphics_and_font.tap\> quam bendo de
    intrada de vor emulator.

9.  In Forth comanda `1 load` por cargar li bloc 1 del disciere activ.
    To va comensar li compilation. Durante li operation, divers missages
    va aparerir sur li ecran.

10. Etablisse li maxim rapiditá de vor emulador e atende. Quande li
    compilation es finit, li ecran va nettar se e un missage va aparir.

11. Etablisse un rapiditá normal (100%) in vor emulator.

12. Por conservar li lude compilat, fa un replica del sistema con
    formate SZX usante vor emulator.

13. In Forth comanda `run` por comensar li lude.

### Historie del depositoria

- 2020-12-05: Li depositoria de Git esset convertet a
  [Fossil](https://fossil-scm.org), conservante [GitHub quam un
  copie](http://github/programandala-net/nuclear-waste-invaders).

- 2023-04-05: Li depositoria esset convertet a
  [Mercurial](https://mercurial-scm.org), possibilisante un melior
  interaction con GitHub.
