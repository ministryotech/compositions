# Introduction
This project provides a suite of extension methods that enable fluent, functional style coding when manipulating objects.

# Methods

## Collection Compositions

### ICollection.AddItem()
Adds the given new object to a collection and returns the collection. To be used as a preferred form to traditional collection Add when chaining is necessary or when updating an EF object tree.

**FOR EF:** Usage of this method ensures object trees are populated in the correct order for persistence of IDs.

### ICollection.AddItems()
Adds the given new collection to a collection and returns the collection. To be used as a preferred form to traditional collection Add when chaining is necessary or when updating an EF object tree.

**FOR EF:** Usage of this method ensures object trees are populated in the correct order for persistence of IDs.

### ICollection.AddItemsIf()
Adds the given new collection, evaluating each item against a given predicate, to a collection and returns the collection. Allows adding collections conditionally, for example, removing duplicates.

### object.AddItemsToIf() / object.AddItemsToIfAsync()
Adds the given new collection, evaluating each item against a given predicate, to the specified collection property and returns the parent object. Allows adding collections conditionally, for example, removing duplicates.

### object.AddAndReturnItem()
Adds the given new object to a collection and returns the object that was added. To be used as a preferred form to traditional collection Add when chaining is necessary or when updating an EF object tree.

**FOR EF:** Usage of this method ensures object trees are populated in the correct order for persistence of IDs.

### ICollection.RemoveItem()
Removes the given new object from a collection and returns the collection. To be used as a preferred form to traditional collection Add when chaining is necessary or when updating an EF object tree.

**FOR EF:** Usage of this method ensures object trees are populated in the correct order for persistence of IDs.

### ICollection.RemoveItemIf()
Evaluates each item in a collection against a given predicate, removes the offending items, then returns the collection. Allows removing items from collections conditionally, for example, removing duplicates.

## Object Compositions

### object.Compose() / object.ComposeAsync()
Enables functional composition of a method, enabling chaining. Currently methods with up to 3 parameters are supported. Feel free to form an update to add more parameters - these updates would be most welcome.

### object.SetProperty() / object.SetPropertyAsync()
Sets the specified navigation property value on an object to enable chaining and to ensure that the object tree is built in the right order, returning the object.

### object.SetAndReturnProperty() / object.SetAndReturnPropertyAsync()
Sets the specified navigation property value on an object to enable chaining and to ensure that the object tree is built in the right order, returning the object property value.

## Collection Mutations
These are void return methods.

### ICollection.AddRange()
Adds the provided collection to the existing collection.

### ICollection.ForEach()
A fluent version of the for eanch loop.

## Projections

### object.Project(object)
Projects all property values from the input object on the passed in target object, if they are present in both. This is a really simple way of mapping matching data between types. If choosing to persist existing values please note that, due to it's binary nature, boolean values will not persist and must be re-set. Returns the output object.

### object.Project(Func)
Uses the provided function to project one type into another type. Returns the output object.

### object.Project(Func, Accumulator)
Uses the provided function to project one type into another type and increments an accumulator. Returns the output object.

### IList.Partition()
Partitions the specified list into blocks of the provided size.

### IEnumerable.Select()
Various additional overloads for Select. Maps the specified predicate to project each instance of one type in the collection into another type using an accumulator to build value increments for the mapping.

### IEnumerable.Flatten()
Flattens a collection of collections down to a single collection.

### IEnumerable.Sorted
Sorts the specified collection according to the comparison provided.

## The Ministry of Technology Open Source Products
Welcome to The Ministry of Technology open source products. All open source Ministry of Technology products are distributed under the MIT License for maximum re-usability.
Our other open source repositories can be found here...

* [https://github.com/ministryotech](https://github.com/ministryotech)
* [https://github.com/tiefling](https://github.com/tiefling)

### Where can I get it?
You can download the package for this project from any of the following package managers...

- **NUGET** - [https://www.nuget.org/packages/Ministry.Compositions](https://www.nuget.org/packages/Ministry.Compositions)

### Contribution guidelines
If you would like to contribute to the project, please contact me.

### Who do I talk to?
* Keith Jackson - temporal-net@live.co.uk
