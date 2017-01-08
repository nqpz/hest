# HEST

This repository encompasses two projects:

  + "Hest", a stack-based concatenative programming language.
  + `hest`, an interpreter for Hest.

This project exists mostly for me to learn to use F#.


## Language




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
./bin/hest --help
```

to learn how to use it.
