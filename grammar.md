# Bitsy grammar

## Type statement

```
TypeStatement :=
    Identifier
    TypeTemplate?
    LeftBrace
        TypeDefinition
    RightBrace

TypeDefinition :=
    Identifier Identifier
    TypeDefinition?

TypeTemplate :=
    LeftAngle
    TemplateArguments
    RightAngle

TemplateArguments :=
    Identifier
    (Comma TemplateArguments)?
```