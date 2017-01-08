.PHONY: all clean

FSLEX=mono $(FSLEXYAC_BIN)/fslex.exe
FSYACC=mono $(FSLEXYAC_BIN)/fsyacc.exe

all: bin/hest.exe

bin/hest.exe: bin/AST.dll bin/Interpreter.dll bin/Parser.dll bin/Lexer.dll hest.fsx
	fsharpc hest.fsx -o bin/hest.exe -r bin/FSharp.PowerPack.dll -r bin/AST.dll -r bin/Interpreter.dll -r bin/Lexer.dll -r bin/Parser.dll

bin/Parser.fs: Parser.fsp
	$(FSYACC) Parser.fsp -o bin/Parser.fs.temp0
	bash -c 'cat <(echo module Parser) bin/Parser.fs.temp0 > bin/Parser.fs.temp1'
	python -c 'p = open("bin/Parser.fs.temp1").read(); h = open("Parser.hack").read(); a, b = p.split("type tokenId", 1); f = open("bin/Parser.fs", "w"); f.write(a + "\n" + h + "\n" + "type tokenId" + b); f.close();'
	rm -f bin/Parser.fs.temp*

bin/Parser.dll: bin/Parser.fs bin/AST.dll
	fsharpc -a bin/Parser.fs -o bin/Parser.dll -r bin/FSharp.PowerPack.dll -r bin/AST.dll

bin/Lexer.fs: Lexer.fsl
	$(FSLEX) Lexer.fsl -o bin/Lexer.fs.temp
	bash -c 'cat <(echo module Lexer) bin/Lexer.fs.temp > bin/Lexer.fs'
	rm -f bin/Lexer.fs.temp

bin/Lexer.dll: bin/Lexer.fs bin/AST.dll bin/Parser.dll
	fsharpc -a bin/Lexer.fs -o bin/Lexer.dll -r bin/FSharp.PowerPack.dll -r bin/AST.dll -r bin/Parser.dll

bin/Interpreter.dll: Interpreter.fs bin/AST.dll
	fsharpc -a Interpreter.fs -o bin/Interpreter.dll -r bin/AST.dll

bin/%.dll: %.fs
	fsharpc -a $< -o $@

clean:
	cd bin && mv FSharp.PowerPack.dll temp && rm -f *.dll *.exe Parser.fsi Parser.fs.fsi Parser.fs Lexer.fs && mv temp FSharp.PowerPack.dll
