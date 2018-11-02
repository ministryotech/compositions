#if NETSTANDARD1_6
using Ministry.Reflection;
#endif

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace Ministry.Compositions
{
    /// <summary>
    /// A collection of static functions for composing objects, returning items in the chain.
    /// </summary>
    /// <remarks>
    /// In contrast with the <seealso cref="Projections"/> class, the functions are not guaranteed pure or immutable and will often mutate the state of the first parameter.
    /// </remarks>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class Compositions
    {
        #region | Collection Manipulation |

        /// <summary>
        /// Adds the given new object to a collection and returns the collection.
        /// </summary>
        /// <remarks>
        /// To be used as a preferred form to traditional collection Add when chaining is necessary or when updating an EF object tree.
        /// FOR EF: Usage of this method ensures object trees are populated in the correct order for persistence of IDs.
        /// </remarks>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="this">The collection property for the collection to add to.</param>
        /// <param name="newObject">The new object to add.</param>
        /// <returns>The collection.</returns>
        public static ICollection<T> AddItem<T>(this ICollection<T> @this, T newObject)
        {
            @this.Add(newObject);
            return @this;
        }

        /// <summary>
        /// Adds the given new collection to a collection and returns the collection.
        /// </summary>
        /// <remarks>
        /// To be used as a preferred form to traditional collection Add when chaining is necessary or when updating an EF object tree.
        /// FOR EF: Usage of this method ensures object trees are populated in the correct order for persistence of IDs.
        /// </remarks>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="this">The collection property for the collection to add to.</param>
        /// <param name="newObjects">The new object to add.</param>
        /// <returns>The collection.</returns>
        public static ICollection<T> AddItems<T>(this ICollection<T> @this, IEnumerable<T> newObjects)
            => @this.AddItems(newObjects.ToArray());

        /// <summary>
        /// Adds the given new collection to a collection and returns the collection.
        /// </summary>
        /// <remarks>
        /// To be used as a preferred form to traditional collection Add when chaining is necessary or when updating an EF object tree.
        /// FOR EF: Usage of this method ensures object trees are populated in the correct order for persistence of IDs.
        /// </remarks>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="this">The collection property for the collection to add to.</param>
        /// <param name="newObjects">The new object to add.</param>
        /// <returns>The collection.</returns>
        public static ICollection<T> AddItems<T>(this ICollection<T> @this, params T[] newObjects)
        {
            if (@this is List<T> thisList)
            {
                thisList.AddRange(newObjects);
            }
            else
            {
                foreach (var item in newObjects)
                    @this.Add(item);
            }
            return @this;
        }

        /// <summary>
        /// Adds the given new collection, evaluating each item against a given predicate, to a collection and returns the collection.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="this">The collection property for the collection to add to.</param>
        /// <param name="newObjects">The new object to add.</param>
        /// <param name="predicate">The predicate to evaluate whether a given item should be included.</param>
        /// <returns>The collection.</returns>
        /// <remarks>
        /// Allows adding collections conditionally, for example, removing duplicates.
        /// </remarks>
        public static ICollection<T> AddItemsIf<T>(this ICollection<T> @this, Func<T, bool> predicate, IEnumerable<T> newObjects)
            => @this.AddItemsIf(predicate, newObjects.ToArray());

        /// <summary>
        /// Adds the given new collection, evaluating each item against a given predicate, to a collection and returns the collection.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="this">The collection property for the collection to add to.</param>
        /// <param name="newObjects">The new object to add.</param>
        /// <param name="predicate">The predicate to evaluate whether a given item should be included.</param>
        /// <returns>The collection.</returns>
        /// <remarks>
        /// Allows adding collections conditionally, for example, removing duplicates.
        /// </remarks>
        public static ICollection<T> AddItemsIf<T>(this ICollection<T> @this, Func<T, bool> predicate, params T[] newObjects)
        {
            foreach (var item in newObjects)
                if (predicate(item)) @this.Add(item);

            return @this;
        }

        #if NETSTANDARD1_6

        /// <summary>
        /// Adds the given new collection, evaluating each item against a given predicate, to the specified collection property and returns the parent object.
        /// </summary>
        /// <typeparam name="TParentObject">The type of the parent object.</typeparam>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="this">The collection property for the collection to add to.</param>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="predicate">The predicate to evaluate whether a given item should be included.</param>
        /// <param name="newObjects">The new object to add.</param>
        /// <returns>The parent object.</returns>
        /// <remarks>
        /// Allows adding collections conditionally, for example, removing duplicates.
        /// </remarks>
        public static TParentObject AddItemsToIf<TParentObject, T>(this TParentObject @this,
            Expression<Func<TParentObject, T>> propertyExpression, Func<T, bool> predicate, IEnumerable<T> newObjects)
            => @this.AddItemsToIf(propertyExpression, predicate, newObjects.ToArray());

        /// <summary>
        /// Adds the given new collection, evaluating each item against a given predicate, to the specified collection property and returns the parent object.
        /// </summary>
        /// <typeparam name="TParentObject">The type of the parent object.</typeparam>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="this">The collection property for the collection to add to.</param>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="predicate">The predicate to evaluate whether a given item should be included.</param>
        /// <param name="newObjects">The new object to add.</param>
        /// <returns>The parent object.</returns>
        /// <remarks>
        /// Allows adding collections conditionally, for example, removing duplicates.
        /// </remarks>
        public static TParentObject AddItemsToIf<TParentObject, T>(this TParentObject @this, 
            Expression<Func<TParentObject, T>> propertyExpression, Func<T, bool> predicate, params T[] newObjects)
        {
            var currentValue = @this.GetPropertyValue(propertyExpression) as ICollection<T>;
            SetProperty(@this, @this.GetPropertyInfo(propertyExpression).Name,
                currentValue.AddItemsIf(predicate, newObjects));
            return @this;
        }

        /// <summary>
        /// Adds the given new collection, evaluating each item against a given predicate, to the specified collection property and returns the parent object.
        /// </summary>
        /// <typeparam name="TParentObject">The type of the parent object.</typeparam>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="this">The collection property for the collection to add to.</param>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="predicate">The predicate to evaluate whether a given item should be included.</param>
        /// <param name="newObjects">The new object to add.</param>
        /// <returns>The parent object.</returns>
        /// <remarks>
        /// Allows adding collections conditionally, for example, removing duplicates.
        /// </remarks>
        public static async Task<TParentObject> AddItemsToIfAsync<TParentObject, T>(this Task<TParentObject> @this,
            Expression<Func<TParentObject, T>> propertyExpression, Func<T, bool> predicate, IEnumerable<T> newObjects)
            => await @this.AddItemsToIfAsync(propertyExpression, predicate, newObjects.ToArray());

        /// <summary>
        /// Adds the given new collection, evaluating each item against a given predicate, to the specified collection property and returns the parent object.
        /// </summary>
        /// <typeparam name="TParentObject">The type of the parent object.</typeparam>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="this">The collection property for the collection to add to.</param>
        /// <param name="propertyExpression">The property expression.</param>
        /// <param name="predicate">The predicate to evaluate whether a given item should be included.</param>
        /// <param name="newObjects">The new object to add.</param>
        /// <returns>The parent object.</returns>
        /// <remarks>
        /// Allows adding collections conditionally, for example, removing duplicates.
        /// </remarks>
        public static async Task<TParentObject> AddItemsToIfAsync<TParentObject, T>(this Task<TParentObject> @this,
            Expression<Func<TParentObject, T>> propertyExpression, Func<T, bool> predicate, params T[] newObjects)
            => (await @this).AddItemsToIf(propertyExpression, predicate, newObjects);

        #endif

        /// <summary>
        /// Adds the given new object to a collection and returns the object that was added.
        /// </summary>
        /// <remarks>
        /// To be used as a preferred form to traditional collection Add when chaining is necessary or when updating an EF object tree.
        /// FOR EF: Usage of this method ensures object trees are populated in the correct order for persistence of IDs.
        /// </remarks>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="this">The collection property for the collection to add to.</param>
        /// <param name="newObject">The new object to add.</param>
        /// <returns>The object that was added.</returns>
        public static T AddAndReturnItem<T>(this ICollection<T> @this, T newObject)
        {
            @this.Add(newObject);
            return newObject;
        }

        /// <summary>
        /// Removes the given new object from a collection and returns the collection.
        /// </summary>
        /// <remarks>
        /// To be used as a preferred form to traditional collection Add when chaining is necessary or when updating an EF object tree.
        /// FOR EF: Usage of this method ensures object trees are populated in the correct order for persistence of IDs.
        /// </remarks>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="this">The collection property for the collection to remove from.</param>
        /// <param name="removeObject">The object to remove.</param>
        /// <returns>The collection.</returns>
        public static ICollection<T> RemoveItem<T>(this ICollection<T> @this, T removeObject)
        {
            @this.Remove(removeObject);
            return @this;
        }

        /// <summary>
        /// Evaluates each item in a collection against a given predicate, removes the offending items, then returns the collection.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="this">The collection property for the collection to remove from.</param>
        /// <param name="predicate">The predicate to evaluate whether a given item should be removed.</param>
        /// <returns>
        /// The collection.
        /// </returns>
        /// <remarks>
        /// Allows removing items from collections conditionally, for example, removing duplicates.
        /// </remarks>
        public static ICollection<T> RemoveItemIf<T>(this ICollection<T> @this, Func<T, bool> predicate)
        {
            var itemsToRemove = @this.Where(predicate).ToList();

            foreach (var item in itemsToRemove)
                @this.Remove(item);

            return @this;
        }

        #endregion

        #region | Compose |

        /// <summary>
        /// Enables functional composition of a method, enabling chaining.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="this">the object.</param>
        /// <param name="method">The method.</param>
        /// <returns>The object post functional changes.</returns>
        [DebuggerStepThrough]
        public static T Compose<T>(this T @this, Func<T, T> method)
            => method(@this);

        #if NETSTANDARD

        /// <summary>
        /// Enables functional composition of a method, enabling chaining.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="this">the object.</param>
        /// <param name="method">The method.</param>
        /// <returns>the object post functional changes.</returns>
        [DebuggerStepThrough]
        public static async Task<T> ComposeAsync<T>(this T @this, Func<T, Task<T>> method)
            => await method(@this);

        /// <summary>
        /// Enables functional composition of a method, enabling chaining.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="this">the object.</param>
        /// <param name="method">The method.</param>
        /// <returns>the object post functional changes.</returns>
        [DebuggerStepThrough]
        public static async Task<T> ComposeAsync<T>(this Task<T> @this, Func<T, Task<T>> method)
            => await method(await @this);

        /// <summary>
        /// Enables functional composition of a method, enabling chaining.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="this">the object.</param>
        /// <param name="method">The method.</param>
        /// <returns>the object post functional changes.</returns>
        [DebuggerStepThrough]
        public static async Task<T> Compose<T>(this Task<T> @this, Func<T, T> method)
            => method(await @this);

        #endif

        /// <summary>
        /// Enables functional composition of a method, enabling chaining.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <typeparam name="TParam1">The type of the first parameter.</typeparam>
        /// <param name="this">the object.</param>
        /// <param name="method">The method.</param>
        /// <param name="param1">The first parameter.</param>
        /// <returns>the object post functional changes.</returns>
        [DebuggerStepThrough]
        public static T Compose<T, TParam1>(this T @this, Func<T, TParam1, T> method, TParam1 param1)
            => method(@this, param1);

        #if NETSTANDARD

        /// <summary>
        /// Enables functional composition of a method, enabling chaining.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <typeparam name="TParam1">The type of the first parameter.</typeparam>
        /// <param name="this">the object.</param>
        /// <param name="method">The method.</param>
        /// <param name="param1">The first parameter.</param>
        /// <returns>the object post functional changes.</returns>
        [DebuggerStepThrough]
        public static async Task<T> ComposeAsync<T, TParam1>(this T @this, Func<T, TParam1, Task<T>> method, TParam1 param1)
            => await method(@this, param1);

        /// <summary>
        /// Enables functional composition of a method, enabling chaining.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <typeparam name="TParam1">The type of the first parameter.</typeparam>
        /// <param name="this">the object.</param>
        /// <param name="method">The method.</param>
        /// <param name="param1">The first parameter.</param>
        /// <returns>the object post functional changes.</returns>
        [DebuggerStepThrough]
        public static async Task<T> ComposeAsync<T, TParam1>(this Task<T> @this, Func<T, TParam1, Task<T>> method, TParam1 param1)
            => await method(await @this, param1);

        /// <summary>
        /// Enables functional composition of a method, enabling chaining.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <typeparam name="TParam1">The type of the first parameter.</typeparam>
        /// <param name="this">the object.</param>
        /// <param name="method">The method.</param>
        /// <param name="param1">The first parameter.</param>
        /// <returns>the object post functional changes.</returns>
        [DebuggerStepThrough]
        public static async Task<T> Compose<T, TParam1>(this Task<T> @this, Func<T, TParam1, T> method, TParam1 param1)
            => method(await @this, param1);

        #endif

        /// <summary>
        /// Enables functional composition of a method, enabling chaining.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <typeparam name="TParam1">The type of the first parameter.</typeparam>
        /// <typeparam name="TParam2">The type of the second parameter.</typeparam>
        /// <param name="this">the object.</param>
        /// <param name="method">The method.</param>
        /// <param name="param1">The first parameter.</param>
        /// <param name="param2">The second parameter.</param>
        /// <returns>the object post functional changes.</returns>
        [DebuggerStepThrough]
        public static T Compose<T, TParam1, TParam2>(this T @this, Func<T, TParam1, TParam2, T> method, TParam1 param1, TParam2 param2)
            => method(@this, param1, param2);

        #if NETSTANDARD

        /// <summary>
        /// Enables functional composition of a method, enabling chaining.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <typeparam name="TParam1">The type of the first parameter.</typeparam>
        /// <typeparam name="TParam2">The type of the second parameter.</typeparam>
        /// <param name="this">the object.</param>
        /// <param name="method">The method.</param>
        /// <param name="param1">The first parameter.</param>
        /// <param name="param2">The second parameter.</param>
        /// <returns>the object post functional changes.</returns>
        [DebuggerStepThrough]
        public static async Task<T> ComposeAsync<T, TParam1, TParam2>(this T @this, Func<T, TParam1, TParam2, Task<T>> method, TParam1 param1, TParam2 param2)
            => await method(@this, param1, param2);

        /// <summary>
        /// Enables functional composition of a method, enabling chaining.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <typeparam name="TParam1">The type of the first parameter.</typeparam>
        /// <typeparam name="TParam2">The type of the second parameter.</typeparam>
        /// <param name="this">the object.</param>
        /// <param name="method">The method.</param>
        /// <param name="param1">The first parameter.</param>
        /// <param name="param2">The second parameter.</param>
        /// <returns>the object post functional changes.</returns>
        [DebuggerStepThrough]
        public static async Task<T> ComposeAsync<T, TParam1, TParam2>(this Task<T> @this, Func<T, TParam1, TParam2, Task<T>> method, TParam1 param1, TParam2 param2)
            => await method(await @this, param1, param2);

        /// <summary>
        /// Enables functional composition of a method, enabling chaining.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <typeparam name="TParam1">The type of the first parameter.</typeparam>
        /// <typeparam name="TParam2">The type of the second parameter.</typeparam>
        /// <param name="this">the object.</param>
        /// <param name="method">The method.</param>
        /// <param name="param1">The first parameter.</param>
        /// <param name="param2">The second parameter.</param>
        /// <returns>the object post functional changes.</returns>
        [DebuggerStepThrough]
        public static async Task<T> Compose<T, TParam1, TParam2>(this Task<T> @this, Func<T, TParam1, TParam2, T> method, TParam1 param1, TParam2 param2)
            => method(await @this, param1, param2);

        #endif

        #endregion

        #region | SetAndReturnProperty |

        #if NETSTANDARD

        /// <summary>
        /// Sets the specified navigation property value on an object to enable chaining and to ensure that the object tree is built in the right order.
        /// </summary>
        /// <typeparam name="T">The type of the object navigation property.</typeparam>
        /// <typeparam name="TParentObject">The type of the parent object that contains the navigation property to set.</typeparam>
        /// <param name="this">The parent object that contains the navigation property to set.</param>
        /// <param name="property">The navigation property name.</param>
        /// <param name="newValue">The new value to set the navigation property to.</param>
        /// <returns>The new value, now set as a navigation property.</returns>
        private static T SetAndReturnProperty<T, TParentObject>(this TParentObject @this, string property,
            T newValue)
        {
            var propertyInfo = @this.GetType().GetTypeInfo().GetProperty(property);
            propertyInfo.SetMethod.Invoke(@this, new object[] { newValue });
            return newValue;
        }

        /// <summary>
        /// Sets the specified navigation property value on an object to enable chaining and to ensure that the object tree is built in the right order.
        /// </summary>
        /// <typeparam name="T">The type of the object navigation property.</typeparam>
        /// <typeparam name="TParentObject">The type of the parent object that contains the navigation property to set.</typeparam>
        /// <param name="this">The parent object that contains the navigation property to set.</param>
        /// <param name="propertyExpression">The expression to access the navigation property.</param>
        /// <param name="newValue">The new value to set the navigation property to.</param>
        /// <returns>The new value, now set as a navigation property.</returns>
        public static T SetAndReturnProperty<T, TParentObject>(this TParentObject @this,
            Expression<Func<TParentObject, T>> propertyExpression, T newValue)
            => SetAndReturnProperty(@this, @this.GetPropertyInfo(propertyExpression).Name, newValue);

        /// <summary>
        /// Sets the specified navigation property value on an object to enable chaining and to ensure that the object tree is built in the right order.
        /// </summary>
        /// <typeparam name="T">The type of the object navigation property.</typeparam>
        /// <typeparam name="TParentObject">The type of the parent object that contains the navigation property to set.</typeparam>
        /// <param name="this">The parent object that contains the navigation property to set.</param>
        /// <param name="propertyExpression">The expression to access the navigation property.</param>
        /// <param name="newValue">The new value to set the navigation property to.</param>
        /// <returns>The new value, now set as a navigation property.</returns>
        public static async Task<T> SetAndReturnPropertyAsync<T, TParentObject>(this Task<TParentObject> @this,
            Expression<Func<TParentObject, T>> propertyExpression, T newValue)
            => SetAndReturnProperty(await @this, propertyExpression, newValue);

        #endif

        #endregion

        #region | SetProperty |

        #if NETSTANDARD

        /// <summary>
        /// Sets the specified navigation property value on an object to enable chaining and to ensure that the object tree is built in the right order.
        /// </summary>
        /// <typeparam name="T">The type of the object navigation property.</typeparam>
        /// <typeparam name="TParentObject">The type of the parent object that contains the navigation property to set.</typeparam>
        /// <param name="this">The parent object that contains the navigation property to set.</param>
        /// <param name="property">The navigation property name.</param>
        /// <param name="newValue">The new value to set the navigation property to.</param>
        /// <returns>The parent object value with the new property value applied.</returns>
        public static TParentObject SetProperty<T, TParentObject>(this TParentObject @this, string property,
            T newValue)
        {
            var propertyInfo = @this.GetType().GetTypeInfo().GetProperty(property);
            propertyInfo.SetMethod.Invoke(@this, new object[] { newValue });
            return @this;
        }

        /// <summary>
        /// Sets the specified navigation property value on an object to enable chaining and to ensure that the object tree is built in the right order.
        /// </summary>
        /// <typeparam name="T">The type of the object navigation property.</typeparam>
        /// <typeparam name="TParentObject">The type of the parent object that contains the navigation property to set.</typeparam>
        /// <param name="this">The parent object that contains the navigation property to set.</param>
        /// <param name="propertyExpression">The expression to access the navigation property.</param>
        /// <param name="newValue">The new value to set the navigation property to.</param>
        /// <returns>The parent object value with the new property value applied.</returns>
        public static TParentObject SetProperty<T, TParentObject>(this TParentObject @this,
            Expression<Func<TParentObject, T>> propertyExpression, T newValue)
            => SetProperty(@this, @this.GetPropertyInfo(propertyExpression).Name, newValue);

        /// <summary>
        /// Sets the specified navigation property value on an object to enable chaining and to ensure that the object tree is built in the right order.
        /// </summary>
        /// <typeparam name="T">The type of the object navigation property.</typeparam>
        /// <typeparam name="TParentObject">The type of the parent object that contains the navigation property to set.</typeparam>
        /// <param name="this">The parent object that contains the navigation property to set.</param>
        /// <param name="propertyExpression">The expression to access the navigation property.</param>
        /// <param name="newValue">The new value to set the navigation property to.</param>
        /// <returns>The parent object value with the new property value applied.</returns>
        public static async Task<TParentObject> SetPropertyAsync<T, TParentObject>(this Task<TParentObject> @this,
            Expression<Func<TParentObject, T>> propertyExpression, T newValue)
            => SetProperty(await @this, propertyExpression, newValue);

        #endif

        #endregion
    }
}