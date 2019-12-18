# DynamicXml
[Dynamically Deserialize XML](https://github.com/MikePreston17/DynamicXml) to typed C# classes (POCOs) by use of the dynamic keyword - without the use of the default Serialization library!  [source code for XML here](https://github.com/MikePreston17/DynamicXml/blob/master/Parsely/DynamicXml/Classes/XmlStreamer.cs)  More parsers to come later!

## Purpose :8ball:
This library is intended as a proof-of-concept substitute for the XMLSerializer from System.Xml.Serialization and can be used for extracting streams of POCOs from long chunks of XML inline.  No need for custom XElement parsers!  Just specify the type you're looking to extract from say, SOAP or XML


## Most Basic Usage :wrench:

Say we have a C# class, Store:

```csharp
class Store
{
    public string Name { get; set; }
    public Product[] Products { get; set; }
    public Customer[] Customers { get; set; }        

    public override string ToString()
    {
        return $"{Name}\n Products: {Products.Length}\n Customers: {Customers.Length}";
    }
}

```
With XML:
```xml
<State>
    <City>
        ...
        <Store>
            <Name "Ace's Hardware"/>
            <Products>
                <Product>
                    <Name>Hammer</Name>
                    <Cost>12.00</Cost>
                    ...
            </Product>
        </Store>
        ...
        <Store>
            ...
        </Store>
    </City>
</State>
...
    
```


We can deserialize instances of Store within an iterator (from IEnumerable)...

```csharp
var stores = XmlStreamer.StreamInstances<Store>(xml);
```

...which can be iterated over, like so:

```csharp
foreach (var store in stores)
{
    Console.WriteLine(store.ToString());
}
```

## Limitations :thumbsdown:
* No support for Streams.
* No support for attributes and none planned in the future (though conceiveably possible).

## Future Support
- [ ] In-file multi-threaded parsing (see: [Fastest Way to Read in C#](https://cc.davelozinski.com/c-sharp/the-fastest-way-to-read-and-process-text-files) and [Reading and Processing in Parallel](https://cc.davelozinski.com/code/c-sharp-code/read-lines-in-batches-process-in-parallel)) and conversion for optimal speed. :camel:
- [ ] [De]serialize JSON to CS objects. :apple:
- [ ] IAsyncEnumerables and DotNetCore support
- [ ] Parser building infrastructure - Use one configurable entrypoint to decide which parser to use (Parsely) {XMLStreamer, JSONStreamer, YAMLStreamer, MarkdownStreamer, etc.)
