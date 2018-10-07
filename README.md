# DynamicXml
[Dynamically Deserialize XML](https://github.com/MikePreston17/DynamicXml) to typed C# classes (POCOs) by use of the dynamic keyword - without the use of the default Serialization library!

## Purpose
This library is intended as a proof-of-concept substitute for the XMLSerializer from System.Xml.Serialization and can be used for extracting streams of POCOs from long swathes of XML in real time.


## Most Basic Usage

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

We can deserialize instances of Store in a stream...

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

## Limitations
* No support for Streams.
* No support for attributes and none planned in the future (though conceiveably possible).

## Additional Notes
Folders 'DynamicXmlTests1' and 'DynamicXMLTests2' both do not exist in my local repo, but exist here.  Ignore #2.  #1 has some useful tests.
