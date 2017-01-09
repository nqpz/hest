module Interpreter

open AST;;

exception Error of string

type Value =
    IntVal of int
  | BoolVal of bool

type Stack = Value list

type Word =
    Builtin of (Stack -> Stack)
  | Defined of Expression list

type WordMap = (Name * Word) list

let lookupMap (m : WordMap) (nameTarget : Name) : Word =
    try
        snd <| List.find (fun (name, _) -> name = nameTarget) m
    with
        | :? System.Collections.Generic.KeyNotFoundException ->
            raise (Error ("no routine " + nameTarget))

let hestAdd (stack : Stack) : Stack =
    match stack with
    | x0 :: x1 :: xs ->
        match (x0, x1) with
        | (IntVal i0, IntVal i1) -> IntVal (i0 + i1) :: xs
        | _ -> raise (Error "invalid stack elements")
    | _ ->
        raise (Error "stack too small")

let hestRead (stack : Stack) : Stack =
    let line = stdin.ReadLine()
    let res = match line with
              | "true" -> BoolVal true
              | "false" -> BoolVal false
              | _ -> try
                         IntVal (int line)
                     with
                         | :? System.FormatException ->
                             raise (Error "invalid input")
    res :: stack

let hestPrint (stack : Stack) : Stack =
    match stack with
    | x :: xs ->
        match x with
        | IntVal i -> printfn "%d" i
        | BoolVal b -> printfn "%s" (if b then "true" else "false")
        xs
    | [] ->
        raise (Error "empty stack")

let builtins : WordMap =
    [
        ("+", Builtin hestAdd);
        ("read", Builtin hestRead)
        ("print", Builtin hestPrint)
    ]

let interpretProgram (routines : Program) : unit =
    let defined = List.map (fun (Routine ((name, _pos), exps)) ->
                            (name, Defined exps)) routines
    let main = lookupMap defined "vrinsk"
    let all = List.append builtins defined
    let lookup = lookupMap all

    let rec interpretWord (s : Stack) (word : Word) : Stack =
        match word with
        | Builtin f -> f s
        | Defined exps ->
            match exps with
            | [] -> s
            | (e :: es) ->
                let s1 = match e with
                         | IntLit (i, pos) -> IntVal i :: s
                         | BoolLit (b, pos) -> BoolVal b :: s
                         | Symbol (sym, pos) -> interpretWord s (lookup sym)
                interpretWord s1 (Defined es)

    let stackInitial = []
    let _stackEnd = interpretWord stackInitial main
    ()
