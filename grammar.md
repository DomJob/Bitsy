# Bitsy grammar

## Type statement

```
TypeStatement :=
    Identifier
    TypeTemplate?
    LeftBrace
        Identifier Identifier
    RightBrace

TypeTemplate :=
    LeftAngle
    TemplateArguments
    RightAngle

TemplateArguments :=
    Identifier
    (Comma TemplateArguments)?
```