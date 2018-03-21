using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

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
    public static class CollectionExtensions
    {
        /// <summary>
        /// Adds the provided collection to the existing collection.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="this">The initial colllection to add to.</param>
        /// <param name="newItems">The new items.</param>
        public static void AddRange<T>(this ICollection<T> @this, IEnumerable<T> newItems)
        {
            @this.ThrowIfNull(nameof(@this));
            foreach (var item in newItems.ThrowIfNull(nameof(newItems)))
                @this.Add(item);
        }

        /// <summary>
        /// Fluent ForEach.
        /// </summary>
        /// <typeparam name="T">The type of the items in the collection.</typeparam>
        /// <param name="this">The collection to apply the function to.</param>
        /// <param name="action">The action to mutate the collection.</param>
        public static void ForEach<T>(this IEnumerable<T> @this, Action<T> action)
        {
            var items = @this.ToArray();
            foreach (var item in items)
            {
                action(item);
            }
        }
    }
}
