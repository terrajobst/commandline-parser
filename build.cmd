@ECHO OFF

"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" %~dp0\build.proj /m /nologo /v:m /flp:verbosity=normal %*