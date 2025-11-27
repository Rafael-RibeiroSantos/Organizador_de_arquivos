@echo off
REM ===========================================================
REM Script para publicar o projeto Windows Forms como EXE único
REM Autônomo (inclui runtime .NET) – pronto para outro computador
REM ===========================================================

REM --- CONFIGURAÇÕES ---
SET SOLUTION_PATH="C:\Users\rafae\OrganizadorDeArquivos\OrganizadorDeArquivos.csproj
SET OUTPUT_PATH="C:\Users\rafae\OrganizadorDeArquivos\bin\Release\net9.0-windows\win-x64
SET RUNTIME=win-x64

REM --- COMANDO DE PUBLICAÇÃO ---
dotnet publish %SOLUTION_PATH% ^
  -c Release ^
  -r %RUNTIME% ^
  --self-contained true ^
  /p:PublishSingleFile=true ^
  /p:IncludeAllContentForSelfExtract=true ^
  /p:PublishTrimmed=false ^
  -o %OUTPUT_PATH%

REM --- CONCLUÍDO ---
echo.
echo ===========================================================
echo Publicacao concluida!
echo Executavel gerado em: %OUTPUT_PATH%
echo ===========================================================
pause
 