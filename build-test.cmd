copy Core.Shogi.Console\bin\Debug\netcoreapp1.1\*.pdb Core.Shogi.Tests\bin\Debug\netcoreapp1.1\

@call %USERPROFILE%\.nuget\packages\opencover\4.6.519\tools\OpenCover.Console.exe -target:"%USERPROFILE%\.nuget\packages\xunit.runner.console\2.2.0\tools\xunit.console.exe" -targetargs:"Core.Shogi.Tests/bin/Debug/netcoreapp1.1/Core.Shogi.Tests.dll" -searchdirs:"Core.Shogi.Tests\bin\Debug\netcoreapp1.1" -output:coverage.xml -register:user -filter:"+[*]* -[*]AutoGeneratedProgram" -oldStyle -returntargetcode

REM @call %USERPROFILE%\.nuget\packages\opencover\4.6.519\tools\OpenCover.Console.exe -target:"dotnet.exe" -targetargs:"Core.Shogi.Console\bin\Debug\netcoreapp1.1\Core.Shogi.Console.dll" -output:coverage.xml -register:user -coverbytest:"*"  -oldStyle