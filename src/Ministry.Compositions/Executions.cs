using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Ministry.Compositions
{
    /// <summary>
    /// Extension methods to execute queries immediately.
    /// </summary>
    /// <remarks>
    /// When using Compositional methods in a functional style in .net we can use Select statements to build queries.
    /// This allows for clear, tidy functional methods but it's also quite obtuse when you need to ensure that you execute a function as the Select
    /// statement only creates a query, it's not actually executed until it becomes something more concrete, such as an Array or a List.
    /// The Execution extensions here allow you to write these same queries in a more explicit way, using Execute() instead of Select()
    /// the result is automatically executed and converted into an explicit type.
    /// </remarks>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "Library")]
    [SuppressMessage("ReSharper", "UnusedMember.Global", Justification = "Library")]
    [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Library")]
    [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Library")]
    public static class Executions
    {
        /// <summary>
        /// Executes an operation on each item within a collection and returns the mutated items.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>
        /// An array of the original, potentially altered, objects.
        /// </returns>
        public static TInput[] ExecuteOperation<TInput>(this IEnumerable<TInput> source, Func<TInput, TInput> operation)
            => source.Select(operation).ToArray();

        /// <summary>
        /// Maps the specified operation to project each instance of one type in the collection into another type and executes the result.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>
        /// An array of objects of the new type.
        /// </returns>
        public static TResult[] ExecuteOperation<TInput, TResult>(this IEnumerable<TInput> source, Func<TInput, TResult> operation)
            => source.Select(operation).ToArray();

        /// <summary>
        /// Maps the specified operation to project each instance of one type in the collection into another type and executes the result.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>
        /// An array of objects of the new type.
        /// </returns>
        public static TResult[] ExecuteOperation<TInput, TResult>(this IEnumerable<TInput> source, Func<TInput, int, TResult> operation)
            => source.Select(operation).ToArray();

        /// <summary>
        /// Maps the specified operation to project each instance of one type in the collection into another type using an accumulator to build value increments for the mapping and executes the result.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TAccumulatedValue">The type of the accumulated value.</typeparam>
        /// <typeparam name="TIncrementingValue">The type of the incrementing value.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="input">The input collection.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="accumulator">The accumulator.</param>
        /// <param name="initialAccumulatorValue">The initial accumulator value.</param>
        /// <param name="increment">The increment.</param>
        /// <returns>
        /// An array of objects of the new type.
        /// </returns>
        /// <remarks>
        /// Works in a similar way to the JavaScript 'map' method.
        /// </remarks>
        public static TResult[] ExecuteOperation<TInput, TAccumulatedValue, TIncrementingValue, TResult>(
            this IEnumerable<TInput> input,
            Func<TInput, TAccumulatedValue, TResult> operation,
            Func<TAccumulatedValue, TIncrementingValue, TAccumulatedValue> accumulator,
            TAccumulatedValue initialAccumulatorValue, TIncrementingValue increment)
            => input.Select(operation, accumulator, initialAccumulatorValue, increment).ToArray();

        /// <summary>
        /// Maps the specified operation to project each instance of one type in the collection into another type using an accumulator to build value increments for the mapping and executes the result.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="input">The input collection.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="initialAccumulatorValue">The initial accumulator value.</param>
        /// <param name="increment">The increment.</param>
        /// <returns>
        /// An array of objects of the new type.
        /// </returns>
        /// <remarks>
        /// Works in a similar way to the JavaScript 'map' method.
        /// </remarks>
        public static TResult[] ExecuteOperation<TInput, TResult>(this IEnumerable<TInput> input,
            Func<TInput, DateTime, TResult> operation, DateTime initialAccumulatorValue, TimeSpan increment)
            => input.Select(operation, initialAccumulatorValue, increment).ToArray();

        /// <summary>
        /// Maps the specified predicate to project each instance of one type in the collection into another type using an accumulator to build value increments for the mapping and executes the result.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="input">The input collection.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="initialAccumulatorValue">The initial accumulator value.</param>
        /// <param name="increment">The increment.</param>
        /// <returns>
        /// An array of objects of the new type.
        /// </returns>
        /// <remarks>
        /// Works in a similar way to the JavaScript 'map' method.
        /// </remarks>
        public static TResult[] ExecuteOperation<TInput, TResult>(this IEnumerable<TInput> input,
            Func<TInput, int, TResult> operation, int initialAccumulatorValue, int increment = 1)
            => input.Select(operation, initialAccumulatorValue - 1, increment).ToArray();

        /// <summary>
        /// Executes an operation on each item within a collection and returns the mutated items.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>
        /// A list of the original, potentially altered, objects.
        /// </returns>
        public static List<TInput> ExecuteToList<TInput>(this IEnumerable<TInput> source, Func<TInput, TInput> operation)
            => source.Select(operation).ToList();

        /// <summary>
        /// Maps the specified operation to project each instance of one type in the collection into another type and executes the result.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>
        /// A list of objects of the new type.
        /// </returns>
        public static List<TResult> ExecuteToList<TInput, TResult>(this IEnumerable<TInput> source, Func<TInput, TResult> operation)
            => source.Select(operation).ToList();

        /// <summary>
        /// Maps the specified operation to project each instance of one type in the collection into another type and executes the result.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="source">The source.</param>
        /// <param name="operation">The operation.</param>
        /// <returns>
        /// A list of objects of the new type.
        /// </returns>
        public static List<TResult> ExecuteToList<TInput, TResult>(this IEnumerable<TInput> source, Func<TInput, int, TResult> operation)
            => source.Select(operation).ToList();

        /// <summary>
        /// Maps the specified operation to project each instance of one type in the collection into another type using an accumulator to build value increments for the mapping and executes the result.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TAccumulatedValue">The type of the accumulated value.</typeparam>
        /// <typeparam name="TIncrementingValue">The type of the incrementing value.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="input">The input collection.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="accumulator">The accumulator.</param>
        /// <param name="initialAccumulatorValue">The initial accumulator value.</param>
        /// <param name="increment">The increment.</param>
        /// <returns>
        /// A list of objects of the new type.
        /// </returns>
        /// <remarks>
        /// Works in a similar way to the JavaScript 'map' method.
        /// </remarks>
        public static List<TResult> ExecuteToList<TInput, TAccumulatedValue, TIncrementingValue, TResult>(
            this IEnumerable<TInput> input,
            Func<TInput, TAccumulatedValue, TResult> operation,
            Func<TAccumulatedValue, TIncrementingValue, TAccumulatedValue> accumulator,
            TAccumulatedValue initialAccumulatorValue, TIncrementingValue increment)
            => input.Select(operation, accumulator, initialAccumulatorValue, increment).ToList();

        /// <summary>
        /// Maps the specified operation to project each instance of one type in the collection into another type using an accumulator to build value increments for the mapping and executes the result.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="input">The input collection.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="initialAccumulatorValue">The initial accumulator value.</param>
        /// <param name="increment">The increment.</param>
        /// <returns>
        /// A list of objects of the new type.
        /// </returns>
        /// <remarks>
        /// Works in a similar way to the JavaScript 'map' method.
        /// </remarks>
        public static List<TResult> ExecuteToList<TInput, TResult>(this IEnumerable<TInput> input,
            Func<TInput, DateTime, TResult> operation, DateTime initialAccumulatorValue, TimeSpan increment)
            => input.Select(operation, initialAccumulatorValue, increment).ToList();

        /// <summary>
        /// Maps the specified predicate to project each instance of one type in the collection into another type using an accumulator to build value increments for the mapping and executes the result.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="input">The input collection.</param>
        /// <param name="operation">The operation.</param>
        /// <param name="initialAccumulatorValue">The initial accumulator value.</param>
        /// <param name="increment">The increment.</param>
        /// <returns>
        /// A list of objects of the new type.
        /// </returns>
        /// <remarks>
        /// Works in a similar way to the JavaScript 'map' method.
        /// </remarks>
        public static List<TResult> ExecuteToList<TInput, TResult>(this IEnumerable<TInput> input,
            Func<TInput, int, TResult> operation, int initialAccumulatorValue, int increment = 1)
            => input.Select(operation, initialAccumulatorValue - 1, increment).ToList();
    }
}
