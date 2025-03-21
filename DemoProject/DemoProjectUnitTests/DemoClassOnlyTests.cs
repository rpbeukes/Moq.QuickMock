﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Moq;
using Microsoft.Extensions.Logging;

namespace DemoProject.Tests
{
    /// <summary>
    /// This project is not suppose to compile successful
    /// </summary>
    [TestClass()]
    public class DemoClassOnlyTests
    {
        [TestMethod()]
        public void DemoClassOnlyTest_Quick_mock_ctor_OR_Mock_ctor()
        {
            var systemUnderTest = new DemoClassOnly();
        }

        [TestMethod()]
        public void DemoClassOnlyTest_MockOf_to_new_Mock()
        {
            var systemUnderTest = new DemoClassOnly(Mock.Of<ILogger<DemoClassOnly>>(),
                                                   It.IsAny<string>(),
                                                   It.IsAny<int>(),
                                                   It.IsAny<int?>(),
                                                   Mock.Of<ICurrentUser>(),
                                                   Mock.Of<Func<SomeCommand>>(),
                                                   Mock.Of<Func<IValidator<InvoiceDetailsInput>>>());
        }

        // this will not be removed
        Mock<Func<SomeCommand>> _cmdFactoryMock = new Mock<Func<SomeCommand>>();

        [TestMethod()]
        public void DemoClassOnlyTest_MockObject_to_MockOfT_local_variable()
        {
            // this will not be removed
            Mock<ICurrentUser> currentUserMock;
            currentUserMock = new Mock<ICurrentUser>();

            // this will be removed
            var validatorFactoryMock = new Mock<Func<IValidator<InvoiceDetailsInput>>>();
            
            var systemUnderTest = new DemoClassOnly(Mock.Of<ILogger<DemoClassOnly>>(),
                                                   It.IsAny<string>(),
                                                   It.IsAny<int>(),
                                                   It.IsAny<int?>(),
                                                   currentUserMock.Object,
                                                   _cmdFactoryMock.Object,
                                                   validatorFactoryMock.Object);
        }
    }
}