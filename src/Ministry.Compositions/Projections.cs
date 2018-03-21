using Ministry.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Ministry.Compositions
{
    /// <summary>
    /// Extension methods to add additional projection features.
    /// </summary>
    /// <remarks>
    /// All of the functions within this class are 'Pure' in a functional sense, returning new objects and do not mutate the input parameters.
    /// This contrasts with the <seealso cref="Compositions"/> class where mutability is a given.
    /// </remarks>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public static class Projections
    {
        #region | Project |

        /// <summary>
        /// Projects all property values from the input object on the passed in target object, if they are present in both.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="input">The input object.</param>
        /// <param name="targetObject">The target object.</param>
        /// <param name="persistExistingValuesOnTarget">if set to <c>true</c> preserves any existing values on the target, 
        /// copying over only new values to replace null, "" or 0.</param>
        /// <returns>
        /// A new type.
        /// </returns>
        /// <remarks>
        /// This is a really simple way of mapping matching data between types.
        /// If choosing to persist existing values please note that, due to it's binary nature, boolean values will not persist
        /// and must be re-set. 
        /// </remarks>
        [SuppressMessage("ReSharper", "CompareOfFloatsByEqualityOperator")]
        public static TResult Project<TInput, TResult>(this TInput input, TResult targetObject, bool persistExistingValuesOnTarget = false)
        {
            foreach (var propertyInfo in input.GetType().GetTypeInfo().GetProperties())
            {
                var targetPropertyInfo = targetObject.GetPropertyInfo(propertyInfo.Name);

                if (!persistExistingValuesOnTarget || 
                    targetPropertyInfo.GetValue(targetObject) == null || targetPropertyInfo.PropertyType == typeof(bool) ||
                    (targetPropertyInfo.PropertyType == typeof(string)) && (string)targetPropertyInfo.GetValue(targetObject) == "" ||
                    (targetPropertyInfo.PropertyType == typeof(int)) && (int)targetPropertyInfo.GetValue(targetObject) == 0 ||
                    (targetPropertyInfo.PropertyType == typeof(long)) && (long)targetPropertyInfo.GetValue(targetObject) == 0 ||
                    (targetPropertyInfo.PropertyType == typeof(short)) && (short)targetPropertyInfo.GetValue(targetObject) == 0 ||
                    (targetPropertyInfo.PropertyType == typeof(float)) && (float)targetPropertyInfo.GetValue(targetObject) == 0 ||
                    (targetPropertyInfo.PropertyType == typeof(double)) && (double)targetPropertyInfo.GetValue(targetObject) == 0 ||
                    (targetPropertyInfo.PropertyType == typeof(decimal)) && (decimal)targetPropertyInfo.GetValue(targetObject) == 0 ||
                    (targetPropertyInfo.PropertyType == typeof(byte)) && (byte)targetPropertyInfo.GetValue(targetObject) == 0 ||
                    (targetPropertyInfo.PropertyType == typeof(sbyte)) && (sbyte)targetPropertyInfo.GetValue(targetObject) == 0 ||
                    (targetPropertyInfo.PropertyType == typeof(uint)) && (uint)targetPropertyInfo.GetValue(targetObject) == 0 ||
                    (targetPropertyInfo.PropertyType == typeof(ushort)) && (ushort)targetPropertyInfo.GetValue(targetObject) == 0 ||
                    (targetPropertyInfo.PropertyType == typeof(ulong)) && (ulong)targetPropertyInfo.GetValue(targetObject) == 0 ||
                    (targetPropertyInfo.PropertyType == typeof(DateTime)) && (DateTime)targetPropertyInfo.GetValue(targetObject) == DateTime.MinValue)
                {
                    targetPropertyInfo?.SetValue(targetObject, propertyInfo.GetValue(input));
                }
            }

            return targetObject;
        }

        /// <summary>
        /// Uses the provided function to project one type into another type.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="input">The input object.</param>
        /// <param name="projectionMapping">The function to project the new type.</param>
        /// <returns>A new type.</returns>
        public static TResult Project<TInput, TResult>(this TInput input, Func<TInput, TResult> projectionMapping)
            => projectionMapping(input);

        /// <summary>
        /// Uses the provided function to project one type into another type and increments an accumulator.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TAccumulatedValue">The type of the accumulated value.</typeparam>
        /// <typeparam name="TIncrementingValue">The type of the incrementing value.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="input">The input object.</param>
        /// <param name="projectionMapping">The function to project the new type.</param>
        /// <param name="accumulator">The accumulator.</param>
        /// <param name="initialAccumulatorValue">The initial accumulator value.</param>
        /// <param name="increment">The increment.</param>
        /// <returns>
        /// A new type.
        /// </returns>
        public static TResult Project<TInput, TAccumulatedValue, TIncrementingValue, TResult>(this TInput input, Func<TInput, TAccumulatedValue, TResult> projectionMapping, 
            Func<TAccumulatedValue, TIncrementingValue, TAccumulatedValue> accumulator, TAccumulatedValue initialAccumulatorValue, TIncrementingValue increment)
            => projectionMapping(input, accumulator(initialAccumulatorValue, increment));

        #endregion

        /// <summary>
        /// Partitions the specified list into blocks of the provided size.
        /// </summary>
        /// <typeparam name="T">The type of the items in the list.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="size">The size.</param>
        /// <returns>A collection of lists.</returns>
        public static IEnumerable<List<T>> Partition<T>(this IList<T> source, int size)
        {
            for (var i = 0; i < Math.Ceiling(source.Count / (double)size); i++)
                yield return new List<T>(source.Skip(size * i).Take(size));
        }

        #region | Select |

        /// <summary>
        /// Maps the specified predicate to project each instance of one type in the collection into another type using an accumulator to build value increments for the mapping.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TAccumulatedValue">The type of the accumulated value.</typeparam>
        /// <typeparam name="TIncrementingValue">The type of the incrementing value.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="input">The input collection.</param>
        /// <param name="projectionMapping">The function to project the new type.</param>
        /// <param name="accumulator">The accumulator.</param>
        /// <param name="initialAccumulatorValue">The initial accumulator value.</param>
        /// <param name="increment">The increment.</param>
        /// <returns>
        /// A collection of objects of the new type.
        /// </returns>
        /// <remarks>
        /// Works in a similar way to the JavaScript 'map' method.
        /// </remarks>
        public static IEnumerable<TResult> Select<TInput, TAccumulatedValue, TIncrementingValue, TResult>(this IEnumerable<TInput> input,
            Func<TInput, TAccumulatedValue, TResult> projectionMapping, Func<TAccumulatedValue, TIncrementingValue, TAccumulatedValue> accumulator, 
            TAccumulatedValue initialAccumulatorValue, TIncrementingValue increment)
        {
            var accumulatedValue = initialAccumulatorValue;
            foreach (var item in input)
                yield return projectionMapping(item, accumulatedValue = accumulator(accumulatedValue, increment));
        }

        /// <summary>
        /// Maps the specified predicate to project each instance of one type in the collection into another type using an accumulator to build value increments for the mapping.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="input">The input collection.</param>
        /// <param name="projectionMapping">The function to project the new type.</param>
        /// <param name="initialAccumulatorValue">The initial accumulator value.</param>
        /// <param name="increment">The increment.</param>
        /// <returns>
        /// A collection of objects of the new type.
        /// </returns>
        /// <remarks>
        /// Works in a similar way to the JavaScript 'map' method.
        /// </remarks>
        public static IEnumerable<TResult> Select<TInput, TResult>(this IEnumerable<TInput> input,
            Func<TInput, DateTime, TResult> projectionMapping, DateTime initialAccumulatorValue, TimeSpan increment)
            => input.Select(projectionMapping, (acc, inc) => acc.Add(increment), initialAccumulatorValue, increment);

        /// <summary>
        /// Maps the specified predicate to project each instance of one type in the collection into another type using an accumulator to build value increments for the mapping.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="input">The input collection.</param>
        /// <param name="projectionMapping">The function to project the new type.</param>
        /// <param name="initialAccumulatorValue">The initial accumulator value.</param>
        /// <param name="increment">The increment.</param>
        /// <returns>
        /// A collection of objects of the new type.
        /// </returns>
        /// <remarks>
        /// Works in a similar way to the JavaScript 'map' method.
        /// </remarks>
        public static IEnumerable<TResult> Select<TInput, TResult>(this IEnumerable<TInput> input,
            Func<TInput, int, TResult> projectionMapping, int initialAccumulatorValue, int increment = 1)
            => input.Select(projectionMapping, (acc, inc) => acc + inc, initialAccumulatorValue -1, increment);

        #endregion

        #region | Flatten |

        /// <summary>
        /// Flattens a collection of collections down to a single collection.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the collections.</typeparam>
        /// <param name="input">The input collection of collections.</param>
        /// <returns>
        /// A single collection constructed from the input collection set.
        /// </returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> input)
            => input.Aggregate(new List<T>(), (current, item) => current.Concat(item).ToList());

        /// <summary>
        /// Flattens a collection of collections down to a single collection.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the collections.</typeparam>
        /// <param name="input">The input collection of collections.</param>
        /// <param name="comparison">The comparison used to sort the flattened collection.</param>
        /// <returns>
        /// A single collection constructed from the input collection set.
        /// </returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> input, Comparison<T> comparison)
            => input.Aggregate(new List<T>(), (current, item) => current.Concat(item).ToList()).Sorted(comparison);

        #endregion

        /// <summary>
        /// Sorts the specified collection according to the comparison provided.
        /// </summary>
        /// <typeparam name="T">The type of the objects in the collection.</typeparam>
        /// <param name="input">The input.</param>
        /// <param name="comparison">The comparison.</param>
        /// <remarks>
        /// Prefer using OrderBy when available as it will perform better.
        /// </remarks>
        /// <returns>A new collection containing sorted elements.</returns>
        public static IEnumerable<T> Sorted<T>(this IEnumerable<T> input, Comparison<T> comparison)
        {
            var retVal = input.ToList();
            retVal.Sort(comparison);
            return retVal;
        }
    }
}
