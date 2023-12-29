using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ministry.Compositions.Tests.TestSupport;
using NSubstitute;
using System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Ministry.Compositions.Tests
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public sealed class CompositionsTests
    {
        #region | Collections |

        [Fact]
        public void CanAddAnObjectToACollectionAndReturnThatObject()
        {
            const int testObj = 12;

            var testCollection = new List<int>();
            var result = testCollection.AddAndReturnItem(testObj);

            Assert.Equal(testObj, result);
        }

        [Fact]
        public void CanAddAnObjectToACollectionAndReturnTheCollection()
        {
            const int testObj = 12;

            var testCollection = new List<int>();
            var result = testCollection.AddItem(testObj);

            Assert.Equal(testCollection, result);
        }

        [Fact]
        public void CanRemoveAnObjectFromACollectionAndReturnTheCollection()
        {
            const int testObj = 12;

            var testCollection = new List<int> { 12, 45, 7 };
            var result = testCollection.RemoveItem(testObj);

            Assert.Equal(testCollection, result);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void CanMutateACollectionFluently()
        {
            var testCollection = new TestObject[]
            {
                new (1, "First"),
                new (3, "Second"),
                new (5, "Third")
            };

            testCollection.ForEach(to => to.Id++);

            Assert.Equal(3, testCollection.Length);
            Assert.Equal(2, testCollection.First(to => to.Name == "First").Id);
            Assert.Equal(4, testCollection.First(to => to.Name == "Second").Id);
            Assert.Equal(6, testCollection.First(to => to.Name == "Third").Id);
        }

        [Fact]
        public async Task CanMutateACollectionFluentlyAndAsynchronously()
        {
            #pragma warning disable 1998
            static async Task<int> TestFunc(int id) => id + 2;
            #pragma warning restore 1998

            var testCollection = new TestObject[]
            {
                new (1, "First"),
                new (3, "Second"),
                new (5, "Third")
            };

            await testCollection.ForEachAsync(async to => to.Id = await TestFunc(to.Id));

            Assert.Equal(3, testCollection.Length);
            Assert.Equal(3, testCollection.First(to => to.Name == "First").Id);
            Assert.Equal(5, testCollection.First(to => to.Name == "Second").Id);
            Assert.Equal(7, testCollection.First(to => to.Name == "Third").Id);
        }

        [Fact]
        public void CannotMutateACollectionFluentlyUsingTheWrongMethod()
        {
            #pragma warning disable 1998
            static async Task<int> TestFunc(int id) => id + 2;
            #pragma warning restore 1998

            var testCollection = new TestObject[]
            {
                new (1, "First"),
                new (3, "Second"),
                new (5, "Third")
            };

            Assert.Throws<InvalidOperationException>(() =>
                #pragma warning disable CS0618 // Type or member is obsolete
                testCollection.ForEach(async to => to.Id = await TestFunc(to.Id)));
                #pragma warning restore CS0618 // Type or member is obsolete
        }

        #endregion

        #region | Compose |

        [Fact]
        public void CanComposeAFunctionThatTakesAnEntityAndReturnsThatEntityEnablingChaining()
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = subEntity.Compose(IncrementId).Compose(IncrementId).Compose(IncrementId);

            Assert.Equal(subEntity, result);
            Assert.Equal(26, result.ID);
        }

        [Theory]
        [InlineData(1, 24)]
        [InlineData(3, 26)]
        public void CanComposeAFunctionThatTakesAnEntityAndAParameterAndReturnsThatEntity(int incrementValue, int expectedId)
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = subEntity.Compose(IncrementId, incrementValue);

            Assert.Equal(subEntity, result);
            Assert.Equal(expectedId, result.ID);
        }

        [Theory]
        [InlineData(1, 26)]
        [InlineData(3, 30)]
        public void CanComposeAFunctionThatTakesAnEntityAndAParameterAndReturnsThatEntityEnablingChaining(int incrementValue, int expectedId)
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = subEntity.Compose(IncrementId).Compose(IncrementId, incrementValue).Compose(IncrementId, incrementValue);

            Assert.Equal(subEntity, result);
            Assert.Equal(expectedId, result.ID);
        }

        [Theory]
        [InlineData(1, 48)]
        [InlineData(3, 52)]
        public void CanComposeAFunctionThatTakesAnEntityAnd2ParametersAndReturnsThatEntity(int incrementValue, int expectedId)
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = subEntity.Compose(IncrementId, incrementValue, true);

            Assert.Equal(subEntity, result);
            Assert.Equal(expectedId, result.ID);
        }

        [Theory]
        [InlineData(1, 52)]
        [InlineData(3, 60)]
        public void CanComposeAFunctionThatTakesAnEntityAnd2ParametersAndReturnsThatEntityEnablingChaining(int incrementValue, int expectedId)
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = subEntity.Compose(IncrementId).Compose(IncrementId, incrementValue, false).Compose(IncrementId, incrementValue, true);

            Assert.Equal(subEntity, result);
            Assert.Equal(expectedId, result.ID);
        }

        [Fact]
        public void CanSubstituteTheFunctionBeingComposedAndVerifyItIsUsed()
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);
            var subIncrementor = Substitute.For<ISubbingClass>();

            subEntity.Compose(subIncrementor.IncrementId, 4);

            subIncrementor.Received().IncrementId(Arg.Any<IEntity<long>>(), Arg.Any<int>());
            subIncrementor.Received().IncrementId(subEntity, 4);
        }

        [Fact]
        public void CanSubstituteTheFunctionBeingComposedAndVerifyItIsUsedWhileChaining()
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);
            var subIncrementor = Substitute.For<ISubbingClass>();

            subIncrementor.IncrementId(Arg.Any<IEntity<long>>(), 4).Returns(subEntity);
            subIncrementor.IncrementId(Arg.Any<IEntity<long>>(), 1).Returns(subEntity);

            subEntity.Compose(subIncrementor.IncrementId, 4)
                .Compose(subIncrementor.IncrementId, 1);

            subIncrementor.Received(2).IncrementId(Arg.Any<IEntity<long>>(), Arg.Any<int>());
            subIncrementor.Received(1).IncrementId(subEntity, 4);
            subIncrementor.Received(1).IncrementId(subEntity, 1);
        }

        [Fact]
        public async void CanComposeAnAsyncFunctionThatTakesAnEntityAndReturnsThatEntity()
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = await subEntity.ComposeAsync(IncrementIdAsync);

            Assert.Equal(subEntity, result);
            Assert.Equal(24, result.ID);
        }

        [Fact]
        public async void CanComposeAnAsyncFunctionThatTakesAnEntityAndReturnsThatEntityEnablingChaining()
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = await subEntity.Compose(IncrementId).ComposeAsync(IncrementIdAsync).ComposeAsync(IncrementIdAsync);

            Assert.Equal(subEntity, result);
            Assert.Equal(26, result.ID);
        }

        [Theory]
        [InlineData(1, 24)]
        [InlineData(3, 26)]
        public async void CanComposeAnAsyncFunctionThatTakesAnEntityAndAParameterAndReturnsThatEntity(int incrementValue, int expectedId)
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = await subEntity.ComposeAsync(IncrementIdAsync, incrementValue);

            Assert.Equal(subEntity, result);
            Assert.Equal(expectedId, result.ID);
        }

        [Theory]
        [InlineData(1, 26)]
        [InlineData(3, 30)]
        public async void CanComposeAnAsyncFunctionThatTakesAnEntityAndAParameterAndReturnsThatEntityEnablingChaining(int incrementValue, int expectedId)
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = await subEntity.Compose(IncrementId).ComposeAsync(IncrementIdAsync, incrementValue).ComposeAsync(IncrementIdAsync, incrementValue);

            Assert.Equal(subEntity, result);
            Assert.Equal(expectedId, result.ID);
        }

        [Theory]
        [InlineData(1, 48)]
        [InlineData(3, 52)]
        public async void CanComposeAnAsyncFunctionThatTakesAnEntityAnd2ParametersAndReturnsThatEntity(int incrementValue, int expectedId)
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = await subEntity.ComposeAsync(IncrementIdAsync, incrementValue, true);

            Assert.Equal(subEntity, result);
            Assert.Equal(expectedId, result.ID);
        }

        [Theory]
        [InlineData(1, 52)]
        [InlineData(3, 60)]
        public async void CanComposeAnAsyncFunctionThatTakesAnEntityAnd2ParametersAndReturnsThatEntityEnablingChaining(int incrementValue, int expectedId)
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = await subEntity.Compose(IncrementId).ComposeAsync(IncrementIdAsync, incrementValue, false).ComposeAsync(IncrementIdAsync, incrementValue, true);

            Assert.Equal(subEntity, result);
            Assert.Equal(expectedId, result.ID);
        }

        [Fact]
        public async void CanSubstituteTheFunctionBeingComposedAsyncAndVerifyItIsUsed()
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);
            var subIncrementor = Substitute.For<ISubbingClass>();

            await subEntity.ComposeAsync(subIncrementor.IncrementIdAsync, 4);

            await subIncrementor.Received().IncrementIdAsync(Arg.Any<IEntity<long>>(), Arg.Any<int>());
            await subIncrementor.Received().IncrementIdAsync(subEntity, 4);
        }

        [Fact]
        public async void CanSubstituteTheFunctionBeingComposedAsyncAndVerifyItIsUsedWhileChaining()
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);
            var subIncrementor = Substitute.For<ISubbingClass>();

            subIncrementor.IncrementIdAsync(Arg.Any<IEntity<long>>(), 4).Returns(subEntity);
            subIncrementor.IncrementIdAsync(Arg.Any<IEntity<long>>(), 1).Returns(subEntity);

            await subEntity.ComposeAsync(subIncrementor.IncrementIdAsync, 4)
                .ComposeAsync(subIncrementor.IncrementIdAsync, 1);

            await subIncrementor.Received(2).IncrementIdAsync(Arg.Any<IEntity<long>>(), Arg.Any<int>());
            await subIncrementor.Received(1).IncrementIdAsync(subEntity, 4);
            await subIncrementor.Received(1).IncrementIdAsync(subEntity, 1);
        }

        #endregion

        #region | Supporting Methods (and other stuff) |

        private IEntity<long> IncrementId(IEntity<long> entity)
            => IncrementId(entity, 1);

        private IEntity<long> IncrementId(IEntity<long> entity, int incrementBy)
            => IncrementId(entity, incrementBy, false);

        private IEntity<long> IncrementId(IEntity<long> entity, int incrementBy, bool doubleValue)
        {
            entity.ID += incrementBy;
            if (doubleValue) entity.ID *= 2;
            return entity;
        }

        private async Task<IEntity<long>> IncrementIdAsync(IEntity<long> entity)
            => await IncrementIdAsync(entity, 1);

        private async Task<IEntity<long>> IncrementIdAsync(IEntity<long> entity, int incrementBy)
            => await IncrementIdAsync(entity, incrementBy, false);

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<IEntity<long>> IncrementIdAsync(IEntity<long> entity, int incrementBy, bool doubleValue)
        {
            entity.ID += incrementBy;
            if (doubleValue) entity.ID *= 2;
            return entity;
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        public interface ISubbingClass
        {
            IEntity<long> IncrementId(IEntity<long> entity, int incrementBy);
            Task<IEntity<long>> IncrementIdAsync(IEntity<long> entity, int incrementBy);
        }

        [SuppressMessage("ReSharper", "UnusedType.Global", Justification = "Testing")]
        public class SubbingClass : ISubbingClass
        {
            public IEntity<long> IncrementId(IEntity<long> entity, int incrementBy)
            {
                entity.ID += incrementBy;
                return entity;
            }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            public async Task<IEntity<long>> IncrementIdAsync(IEntity<long> entity, int incrementBy)
            {
                entity.ID += incrementBy;
                return entity;
            }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        }

        #endregion
    }
}
