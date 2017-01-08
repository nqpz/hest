module Interpreter

open AST;;

exception Error of string * Pos

let interpretProgram (program : Program) : unit =
    ()
