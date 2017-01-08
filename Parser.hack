let parse_error_rich =
  Some (fun (ctxt : ParseErrorContext<_>) ->
    errorPosition <-
      match ctxt.CurrentToken with
      | None -> (0, 30)
      | Some token -> match token with
                      | NUM (_, p) -> p
                      | DECLARATION (_, p) -> p
                      | SYMBOL (_, p) -> p
                      | TRUE p -> p
                      | FALSE p -> p
                      | EOF p -> p
  )
