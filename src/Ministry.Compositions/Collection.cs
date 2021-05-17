using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace Ministry.Compositions
{
    /// <summary>
    /// Extension methods to mutate collections.
    /// </summary>
    /// <remarks>
    /// All of the functions within this class are 'Impure' in a functional sense, returning new objects and do not mutate the input parameters.
    /// This contrasts with the <seealso cref="Projections"/> class where immutability is the goal.
    /// </remarks>
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "Shared Library")]
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds the provided collection to the existing collection.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="this">The initial collection to add to.</param>
        /// <param name="newItems">The new items.</param>
        public static void AddRange<T>(this ICollection<T> @this, IEnumerable<T> newItems)
        {
            @this.ThrowIfNull(nameof(@this));
            foreach (var item in newItems.ThrowIfNull(nameof(newItems)))
                @this.Add(item);
        }

        /// <summary>
        /// Fluent ForEach to mutate an existing collection.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="this">The collection to apply the function to.</param>
        /// <param name="action">The action to mutate the collection.</param>
        /// <remarks>
        /// This method is NOT appropriate for use with async functions within the <see cref="Action" /> parameter as they will not be awaited.
        /// For asynchronous functions use <see cref="ForEachAsync{T}" /> instead.
        /// </remarks>
        public static void ForEach<T>(this IEnumerable<T> @this, Action<T> action)
        {
            var items = @this.ToArray();
            foreach (var item in items)
            {
                action(item);
            }
        }


        /// <summary>
        /// Fluent ForEach to mutate an existing collection.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="this">The collection to apply the function to.</param>
        /// <param name="action">The action to mutate the collection.</param>
        /// <remarks>
        /// This method is NOT appropriate for use with async functions within the <see cref="Action" /> parameter as they will not be awaited.
        /// For asynchronous functions use <see cref="ForEachAsync{T}" /> instead.
        /// </remarks>
        [Obsolete("Executing an async method within a synchronous ForEach method is not supported. Use ForEachAsync instead.")]
        [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Extension override.")]
        public static void ForEach<T>(this IEnumerable<T> @this, Func<T, Task> action)
            => throw new InvalidOperationException(
                "Executing an async method within a synchronous ForEach method is not supported. Use ForEachAsync instead.");

        /// <summary>
        /// Fluent ForEach to mutate an existing collection.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="this">The collection to apply the function to.</param>
        /// <param name="action">The action to mutate the collection.</param>
        /// <remarks>
        /// This method is to be used with async functions and awaited.
        /// </remarks>
        public static async Task ForEachAsync<T>(this IEnumerable<T> @this, Func<T, Task> action)
        {
            var items = @this.ToArray();
            foreach (var item in items)
            {
                await action(item);
            }
        }
    }
}
