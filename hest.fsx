open System.Text;;
open Microsoft.FSharp.Text.Lexing;;
open Microsoft.FSharp.Text.Parsing;;
open System.IO;;

let parseString (s : string) : AST.Program =
     Parser.Program Lexer.Token
        <| LexBuffer<_>.FromBytes(Encoding.UTF8.GetBytes(s))

[<EntryPoint>]
let main (paramList: string[]) : int =
  let text =
      try
          let inStream = File.OpenText paramList.[0]
          let text = inStream.ReadToEnd()
          inStream.Close()
          text
      with
          | _ex ->
              eprintfn "%s" "error: missing file name"
              exit 1

  let program =
      try
          parseString text
      with
          | Lexer.Error (line, col) ->
              eprintfn "error: lexer: line %d, position %d" line col
              exit 1
          | ex ->
              if ex.Message = "parse error"
              then printfn "error: parser: line %d, position %d"
                     (fst Parser.errorPosition) (snd Parser.errorPosition)
              else eprintfn "%s" ex.Message
              exit 1

  printfn "%s" "Input:"
  printfn "%s" (String.concat "" <| List.replicate 70 "-")
  printf "%s" text
  printfn "%s" (String.concat "" <| List.replicate 70 "-")
  printfn "%s" "\n\nParsed program:"
  printfn "%s" (String.concat "" <| List.replicate 70 "-")
  printfn "%s" (AST.formatProgram program)
  printfn "%s" (String.concat "" <| List.replicate 70 "-")
  printfn "%s" "\n\nInterpreting."
  try
      Interpreter.interpretProgram program
  with
      | Interpreter.Error (info, (line, col)) ->
          eprintfn "error: interpreter: %s at line %d, position %d" info line col
          exit 1
      | ex ->
          eprintfn "%s" ex.Message
          exit 1
  0
