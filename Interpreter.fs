module Interpreter

open AST;;

exception Error of string

type Value =
    IntVal of int
  | BoolVal of bool
  | SymVal of string

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

let interpretProgram (routines : Program) : unit =
    let mutable all = []
    let lookup s = lookupMap all s

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
                         | Symbol ("!", pos) -> apply s
                         | Symbol (sym, pos) -> SymVal sym :: s
                interpretWord s1 (Defined es)
    and apply (stack : Stack) : Stack =
        match stack with
        | (SymVal sym) :: xs ->
            interpretWord xs (lookup sym)
        | _ ->
            raise (Error "expecting symbol")

    let hestSwap (stack : Stack) : Stack =
        match stack with
        | x0 :: x1 :: xs ->
            x1 :: x0 :: xs
        | _ -> raise (Error "stack too small")

    let hestDup (stack : Stack) : Stack =
        match stack with
        | x :: xs ->
            x :: x :: xs
        | _ -> raise (Error "stack empty")

    let hestPop (stack : Stack) : Stack =
        match stack with
        | _ :: xs ->
            xs
        | _ -> raise (Error "stack empty")

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
            | SymVal s -> printfn "<%s>" s
            xs
        | [] ->
            raise (Error "stack empty")

    let hestIf (stack : Stack) : Stack =
        match stack with
        | (BoolVal cond) :: (SymVal branch_true) :: (SymVal branch_false) :: s ->
            if cond
            then interpretWord s (lookup branch_true)
            else interpretWord s (lookup branch_false)
        | _ ->
            raise (Error "invalid stack elements")

    let binOp (op : (Value -> Value -> Value)) (stack : Stack) : Stack =
        match stack with
        | x0 :: x1 :: xs ->
            op x0 x1 :: xs
        | _ ->
            raise (Error "stack too small")

    let binOpInt (op : (int -> int -> int)) =
        let inner v0 v1 =
            match (v0, v1) with
            | (IntVal i0, IntVal i1) -> IntVal (op i0 i1)
            | _ -> raise (Error "invalid stack elements")
        binOp inner

    let binOpBool (op : (bool -> bool -> bool)) =
        let inner v0 v1 =
            match (v0, v1) with
            | (BoolVal b0, BoolVal b1) -> BoolVal (op b0 b1)
            | _ -> raise (Error "invalid stack elements")
        binOp inner

    let binOpBoolResult (op : (Value -> Value -> bool)) =
        let inner v0 v1 = BoolVal (op v0 v1)
        binOp inner

    let binOpIntBoolResult (op : (int -> int -> bool)) =
        let inner v0 v1 =
            match (v0, v1) with
            | (IntVal i0, IntVal i1) -> BoolVal (op i0 i1)
            | _ -> raise (Error "invalid stack elements")
        binOp inner

    let builtins : WordMap =
        [
            ("swap", Builtin hestSwap);
            ("dup", Builtin hestDup);
            ("pop", Builtin hestPop);
            ("read", Builtin hestRead)
            ("print", Builtin hestPrint)
            ("if", Builtin hestIf)
            ("+", Builtin <| binOpInt (+));
            ("-", Builtin <| binOpInt (-));
            ("*", Builtin <| binOpInt (*));
            ("%", Builtin <| binOpInt (%));
            ("&", Builtin <| binOpBool (&&));
            ("|", Builtin <| binOpBool (||));
            ("=", Builtin <| binOpBoolResult (=));
            ("<", Builtin <| binOpIntBoolResult (<));
            ("<=", Builtin <| binOpIntBoolResult (<=));
            (">", Builtin <| binOpIntBoolResult (>));
            (">=", Builtin <| binOpIntBoolResult (>=));
        ]

    let defined = List.map (fun (Routine ((name, _pos), exps)) ->
                            (name, Defined exps)) routines
    let main = lookupMap defined "vrinsk"
    all <- List.append builtins defined
    let stackStart = []
    let _stackEnd = interpretWord stackStart main
    ()
