@ECHO OFF

"%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe" %~dp0\build.proj /m /nologo /v:m /flp:verbosity=normal %1 %2 %3 %4 %5 %6 %7 %8 %9