# Beeline

Cut out the middleman and serialize directly from a `DbDataReader` to UTF8 JSON bytes.

## What fresh lunacy is this?

I seem to be writing a lot of "microservices" that essentially run SQL queries
against a database and serialize the results to JSON.
Whether you use Entity Framework Core or Dapper, you're creating a whole bunch of C#
objects purely so you can pass them to [JSON.NET](https://www.newtonsoft.com/json)
to be serialized to the `HttpResponse`. *"That's a whole bunch of allocations and
overhead,"* I thought to myself; *"why not just grab the values from the
DbDataReader and write them to JSON directly?"* So this is **Beeline**: that's
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

That's the next thing on my list, promise.

### Obviously it's not for everything

If your microservice is doing funky stuff with data before returning it, 
and you can't do it in the SQL you use to get it from the database, then
obviously you're going to want to hydrate objects and do things the normal way.
And this isn't for incoming data, just for queries.

## Expected Usage

The cost of constructing the `RowSerializer` type is non-trival,
so you'd want to do it once and then hang onto the result. It's stateless and
thread-safe, I think

So what you'd probably have is something like this:

```csharp
public class BobbinDataService
{
    private ArrayWriter _getAllWriter;

    public async Task GetAll(Stream stream, CancellationToken ct = default)
    {
        using (var cn = new SqlConnection("..."))
        using (var cmd = cn.CreateCommand())
        {
            cmd.CommandText = "SELECT * FROM [Bobbins]";
            await cn.OpenAsync(ct).ConfigureAwait(false);
            using (var reader = cmd.ExecuteReaderAsync(ct).ConfigureAwait(false))
            {
                if (_getAllWriter == null)
                {
                    var serializer = RowSerializer.For(reader);
                    _getAllWriter = new ArrayWriter(serializer, 2048);
                }
                await _getAllWriter.Write(reader, stream, ct).ConfigureAwait(false);
            }
        }
    }
}
```

Make sure `BobbinDataService` is a singleton and that should work. I'll throw together
a sample ASP.NET Core app with it soon.

There are overloads on the `RowSerializer.For` method that let
you pass in a flag for camelCase or a `Func<string, string>` to modify the column
name from the database if they're more complicated (spaces, underscores, that sort
of thing).

## Possible features

- Provider-specific and app-specific extensions, so you can add custom Writer
implmentations.
- More control over the serialization by passing an Options object when creating
the `RowSerializer`, providing overrides for property names or format strings or
whatever.

## Other Notes

- I've been working on this in Rider 2017.3 and it puts red squiggles all over
the uses of `Span<T>`, and tells you the `Span` property of `Memory<T>` is get-only
when you write to it using the indexer, but everything seems to build and run OK.
VS Code has the same squiggles. Haven't tried it in VS2017.
- If you want to comment on this project without opening an issue, you can
[find me on Twitter](https://twitter.com/markrendle).
