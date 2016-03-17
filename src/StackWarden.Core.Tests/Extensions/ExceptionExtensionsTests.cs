using System;
using NUnit.Framework;
using Should;
using StackWarden.Core.Extensions;

namespace StackWarden.Core.Tests.Extensions
{
    public class ExceptionExtensionsTests
    {
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("  ")]
        public void ThrowIfNullOrWhiteSpace_WhenTargetIsNullOrWhiteSpace_WithOnlyMessageProvided_ThrowsExceptionWithProvidedMessage(string target)
        {
            const string message = "Test message";
            var exception = Assert.Throws<ArgumentException>(() => target.ThrowIfNullOrWhiteSpace(message));

            exception.Message.ShouldEqual(message);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("  ")]
        public void ThrowIfNullOrWhiteSpace_WhenTargetIsNullOrWhiteSpace_WithMessageAndArgumentNameProvided_ThrowsExceptionWithProvidedMessageAndArgumentName(string target)
        {
            const string message = "Test message";
            const string argumentName = "test argument name";
            var exception = Assert.Throws<ArgumentException>(() => target.ThrowIfNullOrWhiteSpace(message, argumentName));

            exception.Message.ShouldEqual(message);
            exception.ParamName.ShouldEqual(argumentName);
        }

        [TestCase("a")]
        [TestCase("ab")]
        [TestCase("ab ")]
        [TestCase("ab cd")]
        public void ThrowIfNullOrWhiteSpace_WhenTargetIsNotNullOrWhiteSpace_ReturnsTarget(string target)
        {
            var result = target.ThrowIfNullOrWhiteSpace(nameof(target));

            result.ShouldBeSameAs(target);
        }

        [Test]
        public void ThrowIfNull_WhenProvidedNullAndArgumentName_ThrowsExceptionWithArgumentName()
        {
            object target = null;
            const string argumentName = "test argument name";
            var exception = Assert.Throws<ArgumentNullException>(() => target.ThrowIfNull(argumentName));

            exception.ParamName.ShouldEqual(argumentName);
        }

        [Test]
        public void ThrowIfNull_WhenProvidedNonNull_ReturnsTarget()
        {
            var target = new object();
            const string argumentName = "test argument name";
            var result = target.ThrowIfNull(argumentName);

            result.ShouldBeSameAs(target);
        }
    }
}