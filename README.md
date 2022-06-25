[![GitHub Workflow Status (branch)](https://img.shields.io/github/workflow/status/rpbeukes/Moq.QuickMock/CI/main)](https://github.com/rpbeukes/Moq.QuickMock/actions/workflows/CI_main.yml?query=branch%3Amain+) [![GitHub](https://img.shields.io/github/license/rpbeukes/Moq.QuickMock)](https://github.com/rpbeukes/Moq.QuickMock/blob/main/LICENSE)

# Moq.QuickMock
Small Visual Studio 2022 extension, helping to write [Moq](https://github.com/moq/moq) tests for C#.

---

## Download Moq.QuickMock.vsix and install
Download `Moq.QuickMock.vsix` from latest successful [build](https://github.com/rpbeukes/Moq.QuickMock/actions/workflows/CI_main.yml?query=branch%3Amain+is%3Asuccess).

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

![Mock Ctor Demo](Doco/MockCtor.gif)

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

![Quick Mock Ctor Demo](Doco/QuickMockCtor.gif)

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

**NOTE:** This extension will only change code following the file naming convention `*tests.cs` eg: `TheseAreMyHeroTests.cs`.

---

## Know issues
- Extension generates `Fully qualified type` names.
This can be tolerated by using `Simply name` refactor provided by Visual Studio.
  eg:
```
   var c = new CodeWithBigConstructor(Mock.Of<App.That.Will.Take.Over.The.World.ISecureUser>());
```
The dream:
```
   var c = new CodeWithBigConstructor(Mock.Of<ISecureUser>());
```
- Currently only supports C#, will need to give the [VB.Net](https://docs.microsoft.com/en-us/dotnet/visual-basic/) folks more love.
---

## ToDos

### Tasks (Priority ordered)
- Remove fully qualified types.
- create better visual explanation (.gif) of the extension's intent.
- add an icon.
- deploy to [Visual Studio Marketplace](https://marketplace.visualstudio.com/), so one gets updates automagically.
- Get it working for [VB.Net](https://docs.microsoft.com/en-us/dotnet/visual-basic/) .

### Investigations
- Unit testing extensions (or something like that).