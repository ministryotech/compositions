using System.Collections.Generic;
using System.Threading.Tasks;
using Ministry.Compositions.Tests.TestSupport;
using NSubstitute;
using Xunit;

namespace Ministry.Compositions.Tests
{
    [Trait("Category", "Extensions")]
    public class CompositionsTests
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

        #endregion

        #region | Compose |

        [Fact]
        public void CanComposeAFunctionThatTakesAnEntityAndReturnsThatEntityEnablingChaining()
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = subEntity.Compose(IncrementID).Compose(IncrementID).Compose(IncrementID);

            Assert.Equal(subEntity, result);
            Assert.Equal(26, result.ID);
        }

        [Theory]
        [InlineData(1, 24)]
        [InlineData(3, 26)]
        public void CanComposeAFunctionThatTakesAnEntityAndAParameterAndReturnsThatEntity(int incrementValue, int expectedID)
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = subEntity.Compose(IncrementID, incrementValue);

            Assert.Equal(subEntity, result);
            Assert.Equal(expectedID, result.ID);
        }

        [Theory]
        [InlineData(1, 26)]
        [InlineData(3, 30)]
        public void CanComposeAFunctionThatTakesAnEntityAndAParameterAndReturnsThatEntityEnablingChaining(int incrementValue, int expectedID)
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = subEntity.Compose(IncrementID).Compose(IncrementID, incrementValue).Compose(IncrementID, incrementValue);

            Assert.Equal(subEntity, result);
            Assert.Equal(expectedID, result.ID);
        }

        [Theory]
        [InlineData(1, 48)]
        [InlineData(3, 52)]
        public void CanComposeAFunctionThatTakesAnEntityAnd2ParametersAndReturnsThatEntity(int incrementValue, int expectedID)
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = subEntity.Compose(IncrementID, incrementValue, true);

            Assert.Equal(subEntity, result);
            Assert.Equal(expectedID, result.ID);
        }

        [Theory]
        [InlineData(1, 52)]
        [InlineData(3, 60)]
        public void CanComposeAFunctionThatTakesAnEntityAnd2ParametersAndReturnsThatEntityEnablingChaining(int incrementValue, int expectedID)
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = subEntity.Compose(IncrementID).Compose(IncrementID, incrementValue, false).Compose(IncrementID, incrementValue, true);

            Assert.Equal(subEntity, result);
            Assert.Equal(expectedID, result.ID);
        }

        [Fact]
        public void CanSubstituteTheFunctionBeingComposedAndVerifyItIsUsed()
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);
            var subIncrementor = Substitute.For<ISubbingClass>();

            subEntity.Compose(subIncrementor.IncrementID, 4);

            subIncrementor.Received().IncrementID(Arg.Any<IEntity<long>>(), Arg.Any<int>());
            subIncrementor.Received().IncrementID(subEntity, 4);
        }

        [Fact]
        public void CanSubstituteTheFunctionBeingComposedAndVerifyItIsUsedWhileChaining()
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);
            var subIncrementor = Substitute.For<ISubbingClass>();

            subIncrementor.IncrementID(Arg.Any<IEntity<long>>(), 4).Returns(subEntity);
            subIncrementor.IncrementID(Arg.Any<IEntity<long>>(), 1).Returns(subEntity);

            subEntity.Compose(subIncrementor.IncrementID, 4)
                .Compose(subIncrementor.IncrementID, 1);

            subIncrementor.Received(2).IncrementID(Arg.Any<IEntity<long>>(), Arg.Any<int>());
            subIncrementor.Received(1).IncrementID(subEntity, 4);
            subIncrementor.Received(1).IncrementID(subEntity, 1);
        }

        [Fact]
        public async void CanComposeAnAsyncFunctionThatTakesAnEntityAndReturnsThatEntity()
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = await subEntity.ComposeAsync(IncrementIDAsync);

            Assert.Equal(subEntity, result);
            Assert.Equal(24, result.ID);
        }

        [Fact]
        public async void CanComposeAnAsyncFunctionThatTakesAnEntityAndReturnsThatEntityEnablingChaining()
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = await subEntity.Compose(IncrementID).ComposeAsync(IncrementIDAsync).ComposeAsync(IncrementIDAsync);

            Assert.Equal(subEntity, result);
            Assert.Equal(26, result.ID);
        }

        [Theory]
        [InlineData(1, 24)]
        [InlineData(3, 26)]
        public async void CanComposeAnAsyncFunctionThatTakesAnEntityAndAParameterAndReturnsThatEntity(int incrementValue, int expectedID)
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = await subEntity.ComposeAsync(IncrementIDAsync, incrementValue);

            Assert.Equal(subEntity, result);
            Assert.Equal(expectedID, result.ID);
        }

        [Theory]
        [InlineData(1, 26)]
        [InlineData(3, 30)]
        public async void CanComposeAnAsyncFunctionThatTakesAnEntityAndAParameterAndReturnsThatEntityEnablingChaining(int incrementValue, int expectedID)
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = await subEntity.Compose(IncrementID).ComposeAsync(IncrementIDAsync, incrementValue).ComposeAsync(IncrementIDAsync, incrementValue);

            Assert.Equal(subEntity, result);
            Assert.Equal(expectedID, result.ID);
        }

        [Theory]
        [InlineData(1, 48)]
        [InlineData(3, 52)]
        public async void CanComposeAnAsyncFunctionThatTakesAnEntityAnd2ParametersAndReturnsThatEntity(int incrementValue, int expectedID)
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = await subEntity.ComposeAsync(IncrementIDAsync, incrementValue, true);

            Assert.Equal(subEntity, result);
            Assert.Equal(expectedID, result.ID);
        }

        [Theory]
        [InlineData(1, 52)]
        [InlineData(3, 60)]
        public async void CanComposeAnAsyncFunctionThatTakesAnEntityAnd2ParametersAndReturnsThatEntityEnablingChaining(int incrementValue, int expectedID)
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);

            var result = await subEntity.Compose(IncrementID).ComposeAsync(IncrementIDAsync, incrementValue, false).ComposeAsync(IncrementIDAsync, incrementValue, true);

            Assert.Equal(subEntity, result);
            Assert.Equal(expectedID, result.ID);
        }

        [Fact]
        public async void CanSubstituteTheFunctionBeingComposedAsyncAndVerifyItIsUsed()
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);
            var subIncrementor = Substitute.For<ISubbingClass>();

            await subEntity.ComposeAsync(subIncrementor.IncrementIDAsync, 4);

            await subIncrementor.Received().IncrementIDAsync(Arg.Any<IEntity<long>>(), Arg.Any<int>());
            await subIncrementor.Received().IncrementIDAsync(subEntity, 4);
        }

        [Fact]
        public async void CanSubstituteTheFunctionBeingComposedAsyncAndVerifyItIsUsedWhileChaining()
        {
            var subEntity = Substitute.For<IEntity<long>>();
            subEntity.ID.Returns(23);
            var subIncrementor = Substitute.For<ISubbingClass>();

            subIncrementor.IncrementIDAsync(Arg.Any<IEntity<long>>(), 4).Returns(subEntity);
            subIncrementor.IncrementIDAsync(Arg.Any<IEntity<long>>(), 1).Returns(subEntity);

            await subEntity.ComposeAsync(subIncrementor.IncrementIDAsync, 4)
                .ComposeAsync(subIncrementor.IncrementIDAsync, 1);

            await subIncrementor.Received(2).IncrementIDAsync(Arg.Any<IEntity<long>>(), Arg.Any<int>());
            await subIncrementor.Received(1).IncrementIDAsync(subEntity, 4);
            await subIncrementor.Received(1).IncrementIDAsync(subEntity, 1);
        }

        #endregion

        #region | Supporting Methods (and other stuff) |

        private IEntity<long> IncrementID(IEntity<long> entity)
            => IncrementID(entity, 1);

        private IEntity<long> IncrementID(IEntity<long> entity, int incrementBy)
            => IncrementID(entity, incrementBy, false);

        private IEntity<long> IncrementID(IEntity<long> entity, int incrementBy, bool doubleValue)
        {
            entity.ID = entity.ID + incrementBy;
            if (doubleValue) entity.ID = entity.ID * 2;
            return entity;
        }

        private async Task<IEntity<long>> IncrementIDAsync(IEntity<long> entity)
            => await IncrementIDAsync(entity, 1);

        private async Task<IEntity<long>> IncrementIDAsync(IEntity<long> entity, int incrementBy)
            => await IncrementIDAsync(entity, incrementBy, false);

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private async Task<IEntity<long>> IncrementIDAsync(IEntity<long> entity, int incrementBy, bool doubleValue)
        {
            entity.ID = entity.ID + incrementBy;
            if (doubleValue) entity.ID = entity.ID * 2;
            return entity;
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        public interface ISubbingClass
        {
            IEntity<long> IncrementID(IEntity<long> entity, int incrementBy);
            Task<IEntity<long>> IncrementIDAsync(IEntity<long> entity, int incrementBy);
        }

        public class SubbingClass : ISubbingClass
        {
            public IEntity<long> IncrementID(IEntity<long> entity, int incrementBy)
            {
                entity.ID = entity.ID + incrementBy;
                return entity;
            }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
            public async Task<IEntity<long>> IncrementIDAsync(IEntity<long> entity, int incrementBy)
            {
                entity.ID = entity.ID + incrementBy;
                return entity;
            }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        }

        #endregion
    }
}
