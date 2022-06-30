[![GitHub Workflow Status (branch)](https://img.shields.io/github/workflow/status/rpbeukes/Moq.QuickMock/CI/main)](https://github.com/rpbeukes/Moq.QuickMock/actions/workflows/CI_main.yml?query=branch%3Amain+) [![GitHub](https://img.shields.io/github/license/rpbeukes/Moq.QuickMock)](https://github.com/rpbeukes/Moq.QuickMock/blob/main/LICENSE)

# Moq.QuickMock
Small Visual Studio 2022 extension, helping to write [Moq](https://github.com/moq/moq) tests for C#.

---

## Visual Studio Marketplace
[Moq.QuickMock 2022](https://marketplace.visualstudio.com/items?itemName=Rpbeukes.MoqQuickMock2022)

## Download Moq.QuickMock.vsix and install
Download `Moq.QuickMock.vsix` from latest successful [build](https://github.com/rpbeukes/Moq.QuickMock/actions/workflows/CI_main.yml?query=branch%3Amain+is%3Asuccess).

---

## Available Refactors
- [Mock ctor](https://github.com/rpbeukes/Moq.QuickMock#mock-ctor-moq)
- [Quick mock ctor](https://github.com/rpbeukes/Moq.QuickMock#quick-mock-ctor-moq)
- [Mock.Of&lt;T&gt; to new Mock&lt;T&gt;](https://github.com/rpbeukes/Moq.QuickMock#mockoft-to-new-mockt-moq)
- [mock.Object to Mock.Of&lt;T&gt;](https://github.com/rpbeukes/Moq.QuickMock#mockobject-to-mockoft)

---

## Scenario

`DemoClassOnly` class to mock:

```csharp
public DemoClassOnly(ILogger<DemoClassOnly> logger,
                     string stringValue,
                     int intValue,
                     int? nullIntValue,
                     ICurrentUser currentUser,
                     Func<SomeCommand> cmdFactory,
                     Func<IValidator<InvoiceDetailsInput>>  validatorFactory) { }
```

## Refactors

All these examples live in **MyTestDemoClassOnlyTests.cs**.

### Mock ctor (Moq)

Put the `cursor (caret)` between the `()`, and hit `CTRL + .`.

```csharp
var systemUnderTest = new DemoClassOnly(<cursor>);
```

Find `Mock ctor (Moq)` Refactor Menu Options.

![Mock Ctor Demo](Doco/Assets/MockCtor.gif)

Refactor output:

```csharp
var loggerMock = new Mock<ILogger<DemoClassOnly>>();
var currentUserMock = new Mock<ICurrentUser>();
var cmdFactoryMock = new Mock<Func<SomeCommand>>();
var validatorFactoryMock = new Mock<Func<IValidator<InvoiceDetailsInput>>>();
var systemUnderTest = new DemoClassOnly(loggerMock.Object,
                                        It.IsAny<string>(),
                                        It.IsAny<int>(),
                                        It.IsAny<int?>(),
                                        currentUserMock.Object,
                                        cmdFactoryMock.Object,
                                        validatorFactoryMock.Object);
```

---

### Quick mock ctor (Moq)

Put the `cursor (caret)` between the `()`, and hit `CTRL + .`.

```csharp
var systemUnderTest = new DemoClassOnly(<cursor>);
```

Find `Quick mock ctor (Moq)` Refactor Menu Options.

![Quick Mock Ctor Demo](Doco/Assets/QuickMockCtor.gif)

Refactor output:

```csharp
var systemUnderTest = new DemoClassOnly(Mock.Of<ILogger<DemoClassOnly>>(),
                                        It.IsAny<string>(),
                                        It.IsAny<int>(),
                                        It.IsAny<int?>(),
                                        Mock.Of<ICurrentUser>(),
                                        Mock.Of<Func<SomeCommand>>(),
                                        Mock.Of<Func<IValidator<InvoiceDetailsInput>>>());
```

---
### Mock.Of&lt;T&gt; to new Mock&lt;T&gt; (Moq)

Put the `cursor (caret)` on an argument where `Mock.Of<T>` is used.

Find `Mock.Of<T> to new Mock<T> (Moq)` Refactor Menu Options.

Make sure you put the `cursor` on the word `Mock` or just in front of it.

```csharp
var systemUnderTest = new DemoClassOnly(Mock.Of<ILogger<DemoClassOnly>>(),
                                        It.IsAny<string>(),
                                        It.IsAny<int>(),
                                        It.IsAny<int?>(),
                                <cursor>Mock.Of<ICurrentUser>(),
                                        Mock.Of<Func<SomeCommand>>(),
                                        Mock.Of<Func<IValidator<InvoiceDetailsInput>>>());
```

![Mock of to new mock Demo](Doco/Assets/MockOfToNewMock.gif)

Refactor output:

```csharp
var currentUserMock = new Mock<ICurrentUser>();
var systemUnderTest = new DemoClassOnly(Mock.Of<ILogger<DemoClassOnly>>(),
                                        It.IsAny<string>(),
                                        It.IsAny<int>(),
                                        It.IsAny<int?>(),
                                        currentUserMock.Object,
                                        Mock.Of<Func<SomeCommand>>(),
                                        Mock.Of<Func<IValidator<InvoiceDetailsInput>>>());
```

---

### mock.Object to Mock.Of&lt;T&gt;

Put the `cursor (caret)` on an argument where `mock.Object` is used.

Find `mock.Object to new Mock.Of<T> (Moq)` Refactor Menu Options.

Will only remove the variable if it is a local variable, and instantiated the a `Mock` object on th e sane line.

```csharp
// this will not be removed
Mock<Func<SomeCommand>> _cmdFactoryMock = new Mock<Func<SomeCommand>>();

[TestMethod()]
public void DemoClassOnlyTest_MockObject_to_MockOfT_local_variable()
{
   // this will not be removed
   Mock<ICurrentUser> currentUserMock;
   currentUserMock = new Mock<ICurrentUser>();

   // this variable will be removed
   var validatorFactoryMock = new Mock<Func<IValidator<InvoiceDetailsInput>>>();
   

   var systemUnderTest = new DemoClassOnly(Mock.Of<ILogger<DemoClassOnly>>(),
                                          It.IsAny<string>(),
                                          It.IsAny<int>(),
                                          It.IsAny<int?>(),
                                          currentUserMock.Object,
                                          _cmdFactoryMock.Object,
                                          validatorFactoryMock.Object);
}
```

![Mock Object to Mock Of remove local variable Demo](Doco/Assets/MockObjectToMockOfRemoveLocalVariable.gif)

Refactor output:

```csharp
// this will not be removed
Mock<Func<SomeCommand>> _cmdFactoryMock = new Mock<Func<SomeCommand>>();

[TestMethod()]
public void DemoClassOnlyTest_MockObject_to_MockOfT_local_variable()
{
   // this will not be removed
   Mock<ICurrentUser> currentUserMock;
   currentUserMock = new Mock<ICurrentUser>();
   var systemUnderTest = new DemoClassOnly(Mock.Of<ILogger<DemoClassOnly>>(),
                                          It.IsAny<string>(),
                                          It.IsAny<int>(),
                                          It.IsAny<int?>(),
                                          Mock.Of<ICurrentUser>(),
                                          Mock.Of<Func<SomeCommand>>(),
                                          Mock.Of<Func<IValidator<InvoiceDetailsInput>>>());
}
```
---

**NOTE:** This extension will only change code following the file naming convention `*tests.cs` eg: `TheseAreMyHeroTests.cs`.

---

## Know issues
- Currently only supports C#, will need to give the [VB.Net](https://docs.microsoft.com/en-us/dotnet/visual-basic/) folks more love.
---

## ToDos

### Tasks (Priority ordered)
- pipeline to auto deploy to [Visual Studio Marketplace](https://marketplace.visualstudio.com/)
- Get it working for [VB.Net](https://docs.microsoft.com/en-us/dotnet/visual-basic/) .

### Investigations
- Unit testing extensions (or something like that).