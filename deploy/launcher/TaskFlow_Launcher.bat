@echo off
REM ============================================================
REM TaskFlow NoteManager — Desktop Launcher (Windows)
REM Clique neste arquivo para iniciar o app e abrir no navegador.
REM ============================================================

setlocal enabledelayedexpansion

set "APP_DIR=%APPDATA%\TaskFlow\App"
set "APP_DLL=%APP_DIR%\TaskFlow.Presentation.dll"
set "PORT=5000"
set "URL=http://localhost:%PORT%"
set "LOCK_FILE=%TEMP%\taskflow.lock"

REM Verifica se o app já está rodando (pela porta)
netstat -ano | findstr ":%PORT% " | findstr "LISTENING" >nul 2>&1
if %errorlevel% equ 0 (
    echo TaskFlow ja esta rodando. Abrindo navegador...
    start "" "%URL%"
    exit /b 0
)

REM Remove lock file antigo
if exist "%LOCK_FILE%" del "%LOCK_FILE%"

REM Inicia o app
if exist "%APP_DLL%" (
    cd /d "%APP_DIR%"
    start "" /B dotnet "%APP_DLL%" --urls "http://localhost:%PORT%"
    echo %date% %time% > "%LOCK_FILE%"

    REM Aguarda o servidor iniciar (até 15 segundos)
    for /L %%i in (1,1,30) do (
        curl -s -o NUL -w "%%{http_code}" "%URL%" 2>NUL | findstr "200 302 404" >nul 2>&1
        if !errorlevel! equ 0 goto :open
        timeout /t 1 /nobreak >nul
    )
) else (
    echo Aplicativo nao encontrado em %APP_DIR%.
    echo Execute 'dotnet publish' primeiro ou execute 'dotnet run' no diretorio do projeto.
    pause
    exit /b 1
)

:open
start "" "%URL%"
exit /b 0
