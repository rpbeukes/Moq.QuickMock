[![GitHub Workflow Status (branch)](https://img.shields.io/github/workflow/status/rpbeukes/Moq.QuickMock/CI/main)](https://github.com/rpbeukes/Moq.QuickMock/actions/workflows/CI_main.yml?query=branch%3Amain+) [![GitHub](https://img.shields.io/github/license/rpbeukes/Moq.QuickMock)](https://github.com/rpbeukes/Moq.QuickMock/blob/main/LICENSE)

# Moq.QuickMock
Small Visual Studio 2022 extension to help writing C# [Moq](https://github.com/moq/moq) tests.

---

## Download Moq.QuickMock.vsix and install
Download `Moq.QuickMock.vsix` from latest successful [build](https://github.com/rpbeukes/Moq.QuickMock/actions/workflows/CI_main.yml?query=branch%3Amain+is%3Asuccess).

---

## Scenario

Extension will change code from this:

**MyTests.cs**:
```
...
[TestMethod()]
public void TheTestFunction()
{
    var r = new CodeWithBigConstructor();
}
...
```

to this via `Mock ctor (Moq)` refactor:

**MyTests.cs**:
```
...
[TestMethod()]
public void TheTestFunction()
{
    var loggerMock = new Mock<ILogger<CodeWithBigConstructor>>();
    var secureUserMock = new Mock<ISecureUser>();
    var factoryMock = new Mock<Func<SomeCoolFunction>>();

    var r = new CodeWithBigConstructor(loggerMock.Object, secureUserMock.Object, factoryMock.Object);
}
...
```

or to this via `Quick mock ctor (Moq)` refactor:

**MyTests.cs**:
```
...
[TestMethod()]
public void TheTestFunction()
{
    var r = new CodeWithBigConstructor(Mock.Of<ILogger<CodeWithBigConstructor>>(), Mock.Of<ISecureUser>(), Mock.Of<Func<SomeCoolFunction>>());
}
...
```

**CodeWithBigConstructor.cs**:
```
public class CodeWithBigConstructor
{
    public CodeWithBigConstructor(ILogger<CodeWithBigConstructor> logger,
                                  ISecureUser secureUser,
                                  Func<SomeCoolFunction> factory)
    {
        
    }
}
```

---

**NOTE:** This extension will only change code following the file naming convention `*tests.cs` eg: `TheseAreMyHeroTests.cs`.

---

## How to use
Put the `cursor (caret)` between the `()`, and hit `CTRL + .`.

```
var r = new CodeWithBigConstructor(<cursor>);
```

Find `Quick mock ctor (Moq)` Refactor Menu Options:

![RefactorMenuOption](Doco/RefactorMenuDisplay.png)

---

## Know issues
- Extension generates `Fully qualified type` names.
  eg:
```
   var c = new CodeWithBigConstructor(Mock.Of<App.That.Will.Take.Over.The.World.ISecureUser>());
```
The dream:
```
   var c = new CodeWithBigConstructor(Mock.Of<ISecureUser>());
```
---

## ToDos

### Tasks (Priority ordered)
- Remove fully qualified types.
- add test project.
- create better visual explanation (.gif) of the extension's intent.
- auto increment extension version on a successful build.
- add an icon.
- deploy to [Visual Studio Marketplace](https://marketplace.visualstudio.com/), so one gets updates automagically.

### Investigations
- Unit testing extensions (or something like that).