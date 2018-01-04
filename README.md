# Beeline

Cut out the middleman and serialize directly from a DbDataReader to UTF8 Json bytes.

## What fresh lunacy is this?

I seem to be writing a lot of "microservices" that essentially run SQL queries
against a database and serialize the results to JSON.
Whether you use Entity Framework Core or Dapper, you're creating a whole bunch of C#
objects purely so you can pass them to [JSON.NET](https://www.newtonsoft.com/json)
to be serialized to the `HttpResponse`. That's a whole bunch of allocations and
overhead, I thought to myself; why not just grab the values from the
DbDataReader and write JSON objects directly. So this is **Beeline**: that's
what it does.

## Important and Relevant Notes That You Should Read

### This project is targeting .NET Core 2.1 (Preview).

.NET Core 2.1 has a whole bunch of things that are designed for writing
high-performance, low-allocation code. Things like `Span<T>`, and the very helpful
[`Utf8Formatter`](https://github.com/dotnet/corefx/tree/master/src/System.Memory/src/System/Buffers/Text/Utf8Formatter)
which writes primitive values (e.g. `int`, `DateTimeOffset`) directly to a
`Span<byte>`.

### This project is a spike

Unless I get a bunch of feedback along the lines of *"Rendle, you
blithering idiot, there are extremely good reasons why this is a completely terrible
idea"* I will probably look to harden it up and release a NuGet package, but for now
it lives here and you're welcome to clone/fork/download and play with it if you like.

### This project is not 100% optimized

Right now, I just wanted to test this idea out and put it up for comment. There are
a bunch of things that could be done to improve performance, like using runtime
codegen instead of the Writer funcs, and probably some other stuff that I don't
even know about because I'm an imposter (don't tell anyone).

### I haven't even benchmarked this yet

That's the next thing on my list, honest.