# Developer Guide — Moq.QuickMock

This guide covers the development process, build system, and release pipeline for Moq.QuickMock.

---

## Prerequisites

### Required Tools

- **Visual Studio 2022** or **Visual Studio 2026** (with C# workload)
  - For older Visual Studio versions, open `Moq.QuickMock.sln`
  - For Visual Studio 2026, open `Moq.QuickMock.slnx` (new XML-based format)
- **.NET SDK 8.0** or later (for command-line builds)
- **MSBuild** (required to build the VSIX project; included with Visual Studio)
- **Git** (for cloning and managing the repository)

### Cloning the Repository

```bash
git clone https://github.com/rpbeukes/Moq.QuickMock.git
cd Moq.QuickMock
```

---

## Project Structure

The solution contains three projects:

| Project | Framework | Purpose |
|---------|-----------|---------|
| `Source/Moq.QuickMock` | netstandard2.0 | Core refactoring provider (Roslyn analyzers) |
| `Source/Moq.QuickMock.Vsix` | net472 | Visual Studio 2022 extension packaging |
| `Source/Moq.QuickMock.Vsix.Tests` | net8.0 | xUnit unit tests for the extension |

Also included:
- `DemoProject` — Sample C# project for testing the extension locally
- `DemoProjectUnitTests` — Tests that exercise the refactoring provider

---

## Building Locally

### Restore Dependencies

```bash
dotnet restore
```

### Build the Core Library

```bash
dotnet build --configuration Release ./Source/Moq.QuickMock/Moq.QuickMock.csproj
```

### Build the Test Project

```bash
dotnet build --configuration Release ./Source/Moq.QuickMock.Vsix.Tests/Moq.QuickMock.Vsix.Tests.csproj
```

### Run Tests

```bash
dotnet test ./Source/Moq.QuickMock.Vsix.Tests/Moq.QuickMock.Vsix.Tests.csproj --configuration Release --no-build
```

### Build the VSIX

The VSIX project requires **MSBuild** and cannot be built with `dotnet build`.

```bash
msbuild ./Source/Moq.QuickMock.Vsix/Moq.QuickMock.Vsix.csproj -t:rebuild /p:Configuration=Release
```

The built `.vsix` file will be at:
```
Source/Moq.QuickMock.Vsix/bin/Release/net472/Moq.QuickMock.Vsix.vsix
```

### Using Visual Studio IDE

1. Open `Moq.QuickMock.slnx` (VS 2026) or `Moq.QuickMock.sln` (VS 2022)
2. Build → Build Solution (Ctrl+Shift+B)
3. Run → Start Debugging (F5) to launch an experimental VS instance with the extension

---

## Versioning

### Base Version

The base version is defined in:
```
Source/Moq.QuickMock.Vsix/source.extension.vsixmanifest
```

Currently: **1.0.8**

### Version Format

The final version is a **4-part semantic version** in the format: `MAJOR.MINOR.PATCH.BUILD`

- `MAJOR.MINOR.PATCH` = base version from `source.extension.vsixmanifest`
- `BUILD` = GitHub Actions run number (appended at CI time)

**Example:** Base version `1.0.8` + run #95 = final version `1.0.8.95`

### Version Lock

The version is **locked at CI build time** — it is not chosen during release. You cannot pick a specific version for a release; the version is determined by which CI artifact you release.

---

## CI Pipeline (`CI_main.yml`)

### Trigger

The CI pipeline runs automatically on:
- Push to `main` branch
- Pull requests targeting `main`
- Manual trigger via **Actions** tab → **CI** → **Run workflow**

### What It Does

1. **Checkout code** from the branch
2. **Setup .NET SDK 8.0**
3. **Extract base version** from `source.extension.vsixmanifest` and append `github.run_number`
   - Updates the manifest with the full version
4. **Restore NuGet dependencies**
5. **Build core library** (`Moq.QuickMock.csproj`)
6. **Build test project** (`Moq.QuickMock.Vsix.Tests.csproj`)
7. **Run xUnit tests** (fails the build if tests fail)
8. **Setup MSBuild**
9. **Build VSIX project** with MSBuild
10. **Compute SHA256 hash** of the VSIX file
11. **Package release bundle:**
    - VSIX file: `Moq.QuickMock.<version>.vsix`
    - SHA256 file: `Moq.QuickMock.<version>.vsix.sha256`
12. **Upload artifact** with 90-day retention

### Artifact

- **Name format:** `Moq.QuickMock.1.0.8.95.vsix` (version embedded in name)
- **Contents:** Two files:
  - `Moq.QuickMock.1.0.8.95.vsix` — The extension package
  - `Moq.QuickMock.1.0.8.95.vsix.sha256` — SHA256 hash file
- **Retention:** 90 days (after which it cannot be used for releases)

### Finding the CI Artifact

Navigate to:
```
GitHub → Actions → CI → [Latest successful run] → Artifacts
```

Download the artifact and extract it to inspect the VSIX.

---

## Release Process (`CD_release.yml`)

### Full Step-by-Step

#### 1. Merge PR to Main

Complete your feature branch and merge the PR to `main`. This triggers the CI pipeline.

#### 2. Wait for CI to Succeed

Wait for the GitHub Actions **CI** workflow to complete successfully. Note the **version number** from the artifact name.

Example: If the artifact is named `Moq.QuickMock.1.0.8.95.vsix`, the version is `1.0.8.95`.

#### 3. Create a GitHub Release

Go to:
```
GitHub → Releases → Draft a new release
```

#### 4. Create a Tag

- **Tag name:** `v1.0.8.95` (lowercase `v` required; uppercase `V` also accepted for backward compatibility)
- **Tag version must exactly match** the CI artifact version (e.g., `1.0.8.95`)
- **Target:** `main` branch

#### 5. Write Release Notes

- **Title:** `v1.0.8.95` (or `V1.0.8.95`)
- **Description:** Document changes, new features, bug fixes, etc.
- Follow the existing release notes format in the repository

#### 6. Publish Release

Click **Publish release**.

### CD Workflow Execution

Once the release is published, the **CD Release** workflow fires automatically:

1. **Parse release tag** — Validates tag format (`v1.0.8.95` or `V1.0.8.95`)
2. **Find CI artifact** — Queries GitHub Actions API for `Moq.QuickMock.1.0.8.95.vsix` on `main` branch
3. **Download artifact** — Downloads the artifact ZIP
4. **Verify SHA256** — Extracts the VSIX and verifies checksum against `.vsix.sha256`
5. **Attach to release** — Uploads both the VSIX and SHA256 file as GitHub Release assets
6. **Publish to VS Marketplace:**
   - Locates `VsixPublisher.exe` on the runner
   - Creates a publish manifest with extension metadata
   - Publishes using `VsixPublisher.exe` with a Personal Access Token
7. **Post summary** — Logs deployment details to the workflow summary

### Release Workflow Expectations

- ✅ Release assets appear on GitHub within 1-2 minutes
- ✅ Extension appears on Visual Studio Marketplace within 5-10 minutes (after VS Marketplace processes the publish)
- ✅ Users can install the extension via Visual Studio → Extensions → Manage Extensions

### Tag Format Rules

| Tag | Status |
|-----|--------|
| `v1.0.8.95` | ✅ Valid (lowercase v) |
| `V1.0.8.95` | ✅ Valid (uppercase V, backward compatible) |
| `1.0.8.95` | ❌ Invalid (missing v prefix) |
| `version-1.0.8.95` | ❌ Invalid (wrong format) |

---

## SHA256 Integrity Verification

Every released VSIX includes a companion SHA256 hash file to verify integrity.

### What Is It?

- **File:** `Moq.QuickMock.1.0.8.95.vsix.sha256`
- **Format:** SHA256 hash followed by `*filename`
- **Example content:**
  ```
  a1b2c3d4e5f6... *Moq.QuickMock.1.0.8.95.vsix
  ```

### How to Verify

**On Windows PowerShell:**

```powershell
$expected = (Get-Content Moq.QuickMock.1.0.8.95.vsix.sha256 -Raw).Trim().Split(' ')[0].ToLowerInvariant()
$actual = (Get-FileHash Moq.QuickMock.1.0.8.95.vsix -Algorithm SHA256).Hash.ToLowerInvariant()
if ($actual -eq $expected) { Write-Host "Verified!" } else { Write-Host "Mismatch!" }
```

**On Linux/macOS:**

```bash
sha256sum --check Moq.QuickMock.1.0.8.95.vsix.sha256
```

### Where to Find It

Both files are attached to every GitHub Release:
```
GitHub → Releases → [Release] → Assets
```

---

## Setting Up VS_MARKETPLACE_PAT

The CD workflow requires a **Personal Access Token** to publish to the Visual Studio Marketplace.

### Create a PAT

1. Go to: https://dev.azure.com/
2. Click your profile icon → **Personal access tokens**
3. Click **+ New Token**
4. Configure:
   - **Name:** `Moq.QuickMock Marketplace`
   - **Organization:** Select the organization (or **All accessible organizations**)
   - **Expiration:** 90 days or longer
   - **Scopes:** Select **Marketplace** → **Publish**
5. Click **Create**
6. **Copy the token** (shown only once)

### Add to GitHub Secrets

1. Go to: `GitHub → Settings → Secrets and variables → Actions`
2. Click **New repository secret**
3. **Name:** `VS_MARKETPLACE_PAT`
4. **Value:** Paste the token you just created
5. Click **Add secret**

### Verify

After the first successful release, the extension will appear on the Visual Studio Marketplace in 5-10 minutes. Users can then install it directly from Visual Studio.

---

## First-Time Pipeline Validation

After setting up `VS_MARKETPLACE_PAT`, here's what to expect on the first release:

### Successful Scenario

1. ✅ CD workflow runs after you publish the release
2. ✅ Artifact is found from the CI pipeline
3. ✅ SHA256 verification passes
4. ✅ Assets are attached to the GitHub Release
5. ✅ `VsixPublisher.exe` publishes successfully
6. ✅ Extension appears on VS Marketplace (5-10 minutes later)

### Troubleshooting

**CD workflow fails with "No non-expired CI artifact found"**
- The CI artifact has expired (>90 days old)
- Re-run the CI workflow on `main` to generate a fresh artifact, then create a new release

**VsixPublisher.exe emits warnings**
- Some manifest or extension metadata may trigger non-fatal warnings
- If warnings appear, add `-ignoreWarnings` flag to the publish step in `CD_release.yml`:
  ```powershell
  & "${{ steps.publisher.outputs.path }}" publish `
    -payload "${{ steps.verify.outputs.vsix_path }}" `
    -publishManifest "publish-manifest.json" `
    -personalAccessToken "$env:VS_MARKETPLACE_PAT" `
    -ignoreWarnings
  ```

**Extension doesn't appear on Marketplace after 10 minutes**
- Check the **CD** workflow logs for errors
- Verify the PAT has the correct scope (`Marketplace → Publish`)
- Verify the PAT is not expired

---

## Quick Reference

### Common Commands

```bash
# Restore dependencies
dotnet restore

# Build core library
dotnet build --configuration Release ./Source/Moq.QuickMock/Moq.QuickMock.csproj

# Build and run tests
dotnet test ./Source/Moq.QuickMock.Vsix.Tests/Moq.QuickMock.Vsix.Tests.csproj --configuration Release

# Build VSIX (requires MSBuild)
msbuild ./Source/Moq.QuickMock.Vsix/Moq.QuickMock.Vsix.csproj -t:rebuild /p:Configuration=Release

# Build entire solution (non-VSIX projects only with dotnet)
dotnet build Moq.QuickMock.slnx --configuration Release
```

### Release Checklist

- [ ] PR is merged to `main`
- [ ] CI workflow passes (check artifact version)
- [ ] Create GitHub Release with tag `v1.0.8.95` (matching artifact version)
- [ ] Write release notes
- [ ] Publish release
- [ ] CD workflow completes successfully
- [ ] Verify extension on VS Marketplace

---

## Support

For questions or issues:
- **GitHub Issues:** https://github.com/rpbeukes/Moq.QuickMock/issues
- **Repository:** https://github.com/rpbeukes/Moq.QuickMock
