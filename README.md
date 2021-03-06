# HEST

This repository encompasses two projects:

  + "Hest", a stack-based concatenative programming language.
  + `hest`, an interpreter for Hest.

This project exists mostly for me to learn to use F#.


## Language

Find examples in the `test` directory.

A program must have an entry named `vrinsk`.  This is the main entry.


## Installation

`hest` is written in F#.  It depends on two packages:

  + https://github.com/fsprojects/FsLexYacc for the two binaries
    `fslex.exe` and `fsyacc.exe`
  + https://fsharppowerpack.codeplex.com for the library
    `FSharp.PowerPack.dll`

Once you have these three files, run:

```
ln -s path/to/FSharp.PowerPack.dll bin/
FSLEXYAC_BIN="path/to/binaries_directory" make
```

If this succeeds, you can then run the inpreter `./bin/hest`.  Run

```
./bin/hest path/to/file.hest
```

to interpret the program "path/to/file.hest".

Alternatively you can also run `make static` to produce the (huge!)
static executable in `bin/hest-static` with no dynamic dependencies on
any Mono library.


## Horse

```
              /\,%,_
              \%%%/,\
            _.-"%%|//%
          .'  .-"  /%%%
      _.-'_.-" 0)   \%%%
     /.\.'           \%%%
     \ /      _,      %%%
      `"---"~`\   _,*'\%%'   _,--""""-,%%,
               )*^     `""~~`          \%%%,
             _/                         \%%%
         _.-`/                           |%%,___
     _.-"   /      ,           ,        ,|%%   .`\
    /\     /      /             `\       \%'   \ /
    \ \ _,/      /`~-._         _,`\      \`""~~`
     `"` /-.,_ /'      `~"----"~    `\     \
   jgs   \___,'                       \.-"`/
                                       `--'
```

(From http://chris.com/ascii/index.php?art=animals/horses)
