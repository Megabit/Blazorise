@ECHO off
cls

ECHO Deleting all BIN and OBJ folders...
ECHO.

FOR /d /r . %%d in (bin,obj) DO (
    IF EXIST "%%d" (		 	 
        ECHO %%d | FIND /I "\node_modules\" > Nul && ( 
            ECHO.Skipping: %%d
        ) || (
            ECHO.Deleting: %%d
            rd /s/q "%%d"
        )
    )
)

ECHO.
ECHO Deleting ApiDocs folder...
IF EXIST ".\Documentation\Blazorise.Docs\ApiDocs" (
    ECHO Deleting: .\Documentation\Blazorise.Docs\ApiDocs
    rd /s/q ".\Documentation\Blazorise.Docs\ApiDocs"
) ELSE (
    ECHO ApiDocs folder not found.
)

ECHO.
ECHO Deleting __SOURCEGENERATED__ folder...
IF EXIST "Source\Blazorise\__SOURCEGENERATED__" (
    ECHO Deleting: Source\Blazorise\__SOURCEGENERATED__
    rd /s/q "Source\Blazorise\__SOURCEGENERATED__"
) ELSE (
    ECHO __SOURCEGENERATED__ folder not found.
)

ECHO.
ECHO All cleanup operations completed. Press any key to exit.