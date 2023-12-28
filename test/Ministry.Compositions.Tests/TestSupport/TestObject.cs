namespace Ministry.Compositions.Tests.TestSupport
{
    /// <summary>
    /// Pointless test object
    /// </summary>
    internal class TestObject
    {
        #region | Construction |

        /// <summary>
        /// Initializes a new instance of the <see cref="TestObject"/> class.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="name">The name.</param>
        internal TestObject(int id, string name)
        {
            Id = id;
            Name = name;
        }

        #endregion

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
    }
}
