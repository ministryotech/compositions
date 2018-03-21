namespace Ministry.Compositions.Tests.TestSupport
{
    /// <summary>
    /// Representation of an entity object.
    /// </summary>
    public interface IEntity
    { }

    /// <summary>
    /// Representation of an entity object with a defined ID.
    /// </summary>
    /// <typeparam name="TKey">The type of the ID.</typeparam>
    /// <seealso cref="IEntity" />
    public interface IEntity<TKey> : IEntity
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        TKey ID { get; set; }
    }
}
