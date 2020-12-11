﻿using Amo.Lib.RestClient.PolicyConfig;
using System;
using Xunit;

namespace Amo.Lib.RestClient.Tests.PolicyConfig
{
    public class TimeoutPolicyTest
    {
        [Fact]
        public void BaseTest1()
        {
            TimeoutPolicy policy = new TimeoutPolicy();
            Assert.False(policy.IsValid());
            Assert.Null(policy.Get());
            Assert.Null(policy.GetAsync());
        }

        [Fact]
        public void BaseTest2()
        {
            TimeoutPolicy policy = new TimeoutPolicy(6);
            Assert.True(policy.IsValid());
            Assert.NotNull(policy.Get());
            Assert.NotNull(policy.GetAsync());

            Assert.Equal(6, policy.Seconds);
        }

        [Fact]
        public void BaseTest3()
        {
            TimeoutPolicy policy = new TimeoutPolicy(TimeSpan.Zero);
            Assert.False(policy.IsValid());
            Assert.Null(policy.Get());
            Assert.Null(policy.GetAsync());
        }

        [Fact]
        public void BaseTest4()
        {
            TimeoutPolicy policy = new TimeoutPolicy(TimeSpan.FromMilliseconds(2000));
            Assert.True(policy.IsValid());
            Assert.NotNull(policy.Get());
            Assert.NotNull(policy.GetAsync());

            Assert.Equal(TimeSpan.FromMilliseconds(2000).TotalMilliseconds, policy.TimeSpan.TotalMilliseconds);
        }
    }
}
