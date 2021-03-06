﻿//
// Copyright (c) 2015-2016, Saritasa. All rights reserved.
// Licensed under the BSD license. See LICENSE file in the project root for full license information.
//

namespace Saritasa.Tools.Tests
{
    using System;
    using Xunit;
    using Saritasa.Tools.Security;

    /// <summary>
    /// Security tests.
    /// </summary>
    public class SecurityTests
    {
        [Fact]
        public void TestPasswordGenerations()
        {
            PasswordGenerator passwordGenerator = new PasswordGenerator(
                25,
                PasswordGenerator.CharacterClass.Digits,
                PasswordGenerator.GeneratorFlag.ShuffleChars
            );
            var password = passwordGenerator.Generate();
            Assert.Equal(25, password.Length);

            PasswordGenerator passwordGenerator2 = new PasswordGenerator();
            passwordGenerator2.PasswordLength = 15;
            passwordGenerator2.SetCharactersPool("1");
            Assert.Equal("111111111111111", passwordGenerator2.Generate());
        }

        [Fact]
        public void TestPasswordEntropyCalculation()
        {
            PasswordGenerator passwordGenerator = new PasswordGenerator();
            passwordGenerator.PasswordLength = 10;
            passwordGenerator.SetCharactersPool("0123456789"); // 10 symbols
            var entropy = passwordGenerator.GetEntropy();
            Assert.InRange(entropy, 33.119, 33.319);
        }

        [Fact]
        public void TestPasswordStrength()
        {
            Assert.Equal(0, PasswordGenerator.EstimatePasswordStrength("1111"));
            Assert.Equal(0, PasswordGenerator.EstimatePasswordStrength("2222222222"));
            Assert.Equal(4, PasswordGenerator.EstimatePasswordStrength("123456789"));
            Assert.Equal(73, PasswordGenerator.EstimatePasswordStrength("123456789A"));
            Assert.Equal(75, PasswordGenerator.EstimatePasswordStrength("123456789AB"));
            Assert.Equal(0, PasswordGenerator.EstimatePasswordStrength("CCCCCCCCC"));
            Assert.Equal(100, PasswordGenerator.EstimatePasswordStrength("8m6y2L2WhalkPDa"));
            Assert.Equal(68, PasswordGenerator.EstimatePasswordStrength("AA11bb00__"));
            Assert.Equal(89, PasswordGenerator.EstimatePasswordStrength("123456_789AB"));
        }
    }
}
