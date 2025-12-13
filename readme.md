# Bitsy

A simple, C-like functional programming language where the only primitives are functions and bits. You gotta build the rest yourself. For now, no IO other than the stream of bits passed to `main()` and the resulting stream of bits it outputs if/when the program completes.

Built as a toy programming language for the sake of doing something neat. I intend to solve leetcode-type problems with it, and maybe use it to study algorithms (i.e. Big O notation stuff where n is the number of bitwise operations performed)

## Primitive types

`Bit` and `Bits` are the only built-in types. `Bits` is an arbitrary-length sequence of bits; it is what is given to `main` as argument and what it outputs. It can be thought of as like a generic `any` type, to do anything useful with a `Bits` value, you need to cast it to some other type. When casting, missing bits are interpreted as zeroes and extra bits are ignored.

`Bit` has the standard bitwise operations available: 

## Defining types

For example, a 4-bit integer type could be defined this way:

```
Integer {
    Bit b1
    Bit b2
    Bit b3
    Bit b4
}
```

Now if you want to do any useful arithmetic with that type, you have to define functions of type `Integer->Integer` that perform bitwise operations, e.g. to build your own full-adder:

```
add(Integer left, Integer right) {
    result_b4 = left.b4 ^ right.b4
    carry = left.b4 & right.b4
    // and so on ...

    return {result_b1, result_b2, result_b3, result_b4} as Integer
}
```

Given the specified input types and the final casting, add is inferred as being of type `(Integer,Integer) -> Integer`. Without the `as Integer`, the output would have been `(Integer,Integer) -> Bits` and then it's the job of the caller of the function to cast it into something meaningful for its purposes.

Since the built-in literals are also limited to just `0` and `1`, you have to define the others yourself to save yourself some trouble:

```
2 = {0,0,1,0} as Integer
3 = {0,0,1,1} as Integer
// and so on
```

`2` and `3` are treated as any other variable, and by variable I mean constant. Everything defined is strictly immutable in this language.

Then you can reuse Integer in further types, for example if you want to define a byte:

```
Byte {
    Integer lower
    Integer upper
}
```

and now you can define 17 like `17 = {16, 1} as Byte`. You can also trivially cast the previously defined 2 to 16 literals as Byte. `2 as Byte` is the same as `(2 as Bits) as Bytes` which is evaluated `{0,0,1,0} as Bytes` and then the missing bits are simply filled with zeroes. For that reason it may be better to do least significant bits first so that `0 as Bytes` and `1 as Bytes` remain consistent. I did it backwards, sorry.

Now provided you previously built a 4-bit full adder with carry output, you can use it to quickly created an 8-bit full adder and so on to write Byte->Byte arithmetic functions.

This language also supports generics

```
Tuple<T> {
    T first
    T second
}
```

and recursive types:

```
LinkedList<T> {
    T current
    LinkedList<T> rest
}
```

Now you have the tools to create a program that casts the input `Bits` as `LinkedList<Byte>` and then do whatever to it. Obviously it's up to you to encode meaningful types and data structures. For example, with the type currently defined this program:

```
// assume add function of type `byte->byte` is defined previously
addRecursively(LinkedList<Byte> list) {
    return add(list.current, addRecursively(list.rest))
}

main(Bits input) {
    list = input as LinkedList<Byte>
    totalSum = addRecursively(list)
    
    return totalSum // whatever type is used here is casted back to Bits. (main is always of type Bits->Bits)
}
```

is perfectly valid, but it would go on forever since it has no way of knowing when the end of the LinkedList has been reached. Casting `Bits` into a recursively type technically goes on forever since the `Bits` expression is treated as being padded with infinitely many zeroes (it's all evaluated lazily, don't worry). It is then up to you for example to define a `Program` type for the sole purpose of containing prefix information about the data you pass to the main function:

```
Program {
    Byte listLength
    LinkedList<Byte> list
}

main(Program program) { // Note you can let the parser do the casting for you implicitly
    totalSum = addRecursively(program.listLength, program.list)

    return totalSum
}
```

Of course you also have to tweak `addRecursively` to have a base case where it only outputs `list.current` when `count` is 1