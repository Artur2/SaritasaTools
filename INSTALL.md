Project Setup
=============

Here are steps you need to do to setup environment to be able to develop. You need following software installed:

- Visual Studio 2015 or 2017 (https://www.visualstudio.com/downloads/download-visual-studio-vs.aspx)
- psake (https://github.com/psake/psake)
- PowerShell 5
- GitVersion (`choco install gitversion.portable`)

For documentation:

- python 3 (`choco install python`)
- sphinx (`pip install sphinx`)
- read the docs theme (`pip install sphinx_rtd_theme`)

Code Style Setup
----------------

We are using StyleCop.Analyzers project for extended code style check. It should be installed for every project in solution:

```
Install-Package StyleCop.Analyzers
```

Then you need to install our codestyle ruleset. To do that just run the following cmd command under administrator:

```
cmd:
@powershell -NoProfile -ExecutionPolicy Bypass -Command "iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/saritasa/SaritasaTools/develop/src/Saritasa.Tools/SaritasaRulesetInstall.ps1'))" && SET PATH=%PATH%;

PowerShell.exe (Ensure Get-ExecutionPolicy is at least RemoteSigned):
iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/saritasa/SaritasaTools/develop/src/Saritasa.Tools/SaritasaRulesetInstall.ps1'))

PowerShell v3+ (Ensure Get-ExecutionPolicy is at least RemoteSigned):
iwr https://raw.githubusercontent.com/saritasa/SaritasaTools/develop/src/Saritasa.Tools/SaritasaRulesetInstall.ps1 -UseBasicParsing | iex
```

After that the "Saritasa Code Rules" will be available in "Code Analysis" in project properties.
