module AST

type Pos = int * int (* (line, column) *)

type Name = string

type Expression =
    IntLit of int * Pos
  | BoolLit of bool * Pos
  | Symbol of string * Pos

type Routine =
    Routine of (Name * Pos) * Expression list

type Program = Routine list

let formatExpression (e : Expression) : string =
    match e with
    | IntLit (n, _) -> sprintf "%i" n
    | BoolLit (b, _) -> if b then "true" else "false"
    | Symbol (s, _) -> s

let formatRoutine (r : Routine) : string =
    match r with
    | Routine ((name, _pos), exps) ->
        name + ": "
        + (String.concat " " <| List.map formatExpression exps)

let formatProgram = String.concat "\n\n" << List.map formatRoutine
